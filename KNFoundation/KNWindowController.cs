using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Xaml;
using System.Reflection;
using System.Activities.XamlIntegration;

using KNFoundation.KNKVC;

namespace KNFoundation {

    public class KNWindowController {
        
        private Window window;
        private KNViewController rootViewController;
        private Thickness glassMargins = new Thickness(0);

        public KNWindowController(string windowXamlName) {

            try {

                string xamlPath = KNBundle.MainBundle().PathForResourceOfType(windowXamlName, "xaml");
                if (xamlPath != null) {

                    FileStream s = new FileStream(xamlPath, FileMode.Open, FileAccess.Read);
                    
                    XamlXmlReaderSettings settings = new XamlXmlReaderSettings();
                    settings.LocalAssembly = Assembly.GetCallingAssembly();
                    settings.CloseInput = true;
                    
                    XamlXmlReader reader = new XamlXmlReader(s, settings);

                    DependencyObject rootElement = (DependencyObject)XamlServices.Load(reader);

                    Window newWindow = (Window)rootElement;
                    newWindow.SourceInitialized += WindowWasInitialized;
                    Window = newWindow;

                    // Attempt to localise

                    KNBundleGlobalHelpers.AttemptToLocalizeComponent(Window);

                    s.Dispose();

                } else {
                    throw new Exception(windowXamlName + " could not be found");
                }

            } catch {
                throw;
            }
        }

        public KNWindowController(Window window) {
            Window = window;
            KNBundleGlobalHelpers.AttemptToLocalizeComponent(Window);

        }

        public void ShowWindow() {
            // We want to make sure the window has been rendered fully 
            // before showing it. 

            Window.Show();
        }

        public void SetWindowGlassMargins(Thickness thickness) {

            glassMargins = thickness;

            if (Window != null) {

                if (Window.IsInitialized) {
                    ExtendGlass(thickness);
                }
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            if (msg == WM_DWMCOMPOSITIONCHANGED) {
                ExtendGlass(glassMargins);
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void WindowWasInitialized(object sender, EventArgs e) {
            ExtendGlass(glassMargins);
        }

        #region "Properties"

        public Window Window {
            get { return window; }
            private set {
                this.WillChangeValueForKey("Window");
                window = value;
                this.DidChangeValueForKey("Window");
            }
        }

        public KNViewController ViewController {
            get { return rootViewController; }
            set {
                this.WillChangeValueForKey("ViewController");
                rootViewController = value;
                if (rootViewController != null) {
                    rootViewController.WindowController = this;
                    Window.Content = rootViewController.View;
                } else {
                    Window.Content = null;
                }                
                this.DidChangeValueForKey("ViewController");
            }
        }



        #endregion

#region Glass

        [StructLayout(LayoutKind.Sequential)]
        struct MARGINS {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        private const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

        [DllImport("dwmapi.dll")]
        static extern int
           DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        extern static int DwmIsCompositionEnabled(ref int en);

        private void ExtendGlass(Thickness thickness) {
            try {
                int isGlassEnabled = 0;
                DwmIsCompositionEnabled(ref isGlassEnabled);
                if (Environment.OSVersion.Version.Major > 5 && isGlassEnabled > 0) {
                    // Get the window handle
                    WindowInteropHelper helper = new WindowInteropHelper(window);

                    if (helper.Handle != IntPtr.Zero) {

                        HwndSource mainWindowSrc = (HwndSource)HwndSource.FromHwnd(helper.Handle);
                        mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;

                        // Set Margins

                        // Get the dpi of the screen
                        System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(mainWindowSrc.Handle);
                        float dpiX = desktop.DpiX / 96;
                        float dpiY = desktop.DpiY / 96;

                        MARGINS margins = new MARGINS() { cxLeftWidth = 0, cxRightWidth = 0, cyBottomHeight = 0, cyTopHeight = 0 };
                        margins.cxLeftWidth = (int)(thickness.Left * dpiX);
                        margins.cxRightWidth = (int)(thickness.Right * dpiX);
                        margins.cyBottomHeight = (int)(thickness.Bottom * dpiY);
                        margins.cyTopHeight = (int)(thickness.Top * dpiY);

                        window.Background = Brushes.Transparent;

                        int hr = DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);

                    }

                } else {
                    window.Background = SystemColors.WindowBrush;
                }
            } catch (DllNotFoundException) {

            }
        }

#endregion

    }

}

