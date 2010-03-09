using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Windows.Markup;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;

using KNFoundation.KNKVC;

namespace KNFoundation {

    public class KNWindowController {
        
        private Window window;
        private KNViewController rootViewController;

        public KNWindowController(string windowXamlName) {

            try {

                string xamlPath = KNBundle.MainBundle().PathForResourceOfType(windowXamlName, "xaml");
                if (xamlPath != null) {

                    FileStream s = new FileStream(xamlPath, FileMode.Open);
                    Object rootElement = (Object)XamlReader.Load(s);
                    Window newWindow = (Window)rootElement;
                    Window = newWindow;
                    s.Dispose();

                } else {
                    throw new Exception(windowXamlName + " could not be found");
                }

            } catch {
                throw;
            }
        }

        public void ShowWindow() {
            // We want to make sure the window has been rendered fully 
            // before showing it. 

            Window.Show();

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
                Window.Content = value.View;
                this.DidChangeValueForKey("ViewController");
            }
        }



        #endregion

    }

}

