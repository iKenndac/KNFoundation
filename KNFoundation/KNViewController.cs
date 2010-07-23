using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.IO;
using System.Reflection;
using KNFoundation.KNKVC;
using System.Xaml;

namespace KNFoundation {

    public class KNViewController {

        private UserControl view;
        private Object representedObject;
        private KNViewController parentViewController;
        private KNWindowController windowController;

        public KNViewController(string viewXamlName) {

            try {
                string xamlPath = KNBundle.MainBundle().PathForResourceOfType(viewXamlName, "xaml");
                if (xamlPath != null) {

                    FileStream s = new FileStream(xamlPath, FileMode.Open);

                    XamlXmlReaderSettings settings = new XamlXmlReaderSettings();
                    settings.LocalAssembly = Assembly.GetCallingAssembly();
                    settings.CloseInput = true;

                    XamlXmlReader reader = new XamlXmlReader(s, settings);

                    DependencyObject rootElement = (DependencyObject)XamlServices.Load(reader);

                    UserControl newView = (UserControl)rootElement;
                    View = newView;
                    View.Initialized += ViewInitialized;

                    // Find our properties, then search for controls with the same name

                    MatchPropertiesToViewTree(rootElement);

                    // Attempt to localise

                    KNBundleGlobalHelpers.AttemptToLocalizeComponent(View);

                    s.Dispose();

                } else {
                    throw new Exception(viewXamlName + " could not be found");
                }

            } catch {
                throw;
            }
        }

        public KNViewController(DependencyObject view) {

            View = (UserControl)view;
            MatchPropertiesToViewTree(view);
            KNBundleGlobalHelpers.AttemptToLocalizeComponent(View);
        }

        public KNViewController() {
        }

        private void ViewInitialized(object sender, EventArgs e) {
            ViewDidLoad();
        }

        protected virtual void ViewDidLoad() {
        
        }

        private void MatchPropertiesToViewTree(DependencyObject obj) {

            Type myType = GetType();
            PropertyInfo[] properties = myType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (PropertyInfo property in properties) {

                if (property.CanWrite) {

                    Object item = LogicalTreeHelper.FindLogicalNode(obj, property.Name);
                    if (item != null) {
                        property.SetValue(this, item, null);
                    }
                }
            }

        }

        #region "Properties"

        public UserControl View {
            get { return view; }
            private set {
                this.WillChangeValueForKey("View");
                view = value;
                this.DidChangeValueForKey("View");
            }
        }

        public Object RepresentedObject {
            get { return representedObject; }
            set {
                this.WillChangeValueForKey("RepresentedObject");
                representedObject = value;
                this.DidChangeValueForKey("RepresentedObject");
            }
        }

        public KNViewController ParentViewController {
            get { return parentViewController; }
            set {
                this.WillChangeValueForKey("ParentViewController");
                parentViewController = value;
                this.DidChangeValueForKey("ParentViewController");
            }
        }

        public KNWindowController WindowController {
            get {
                if (windowController != null) {
                    return windowController;
                } else if (ParentViewController != null) {
                    return ParentViewController.WindowController;
                } else {
                    return null;
                }
            }
            set {
                this.WillChangeValueForKey("WindowController");
                windowController = value;
                this.DidChangeValueForKey("WindowController");
            }
        }
    

        #endregion

    }
}