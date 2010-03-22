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

namespace KNFoundation {

    public class KNViewController {

        private UserControl view;
        private Object representedObject;


        public KNViewController(string viewXamlName) {

            try {
                string xamlPath = KNBundle.MainBundle().PathForResourceOfType(viewXamlName, "xaml");
                if (xamlPath != null) {

                    FileStream s = new FileStream(xamlPath, FileMode.Open);
                    DependencyObject rootElement = (DependencyObject)XamlReader.Load(s);
                    UserControl newView = (UserControl)rootElement;
                    View = newView;

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

        #endregion

    }
}