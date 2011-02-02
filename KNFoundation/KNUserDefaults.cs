using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KNFoundation.KNKVC;
using System.IO;

namespace KNFoundation {


    public class KNUserDefaults {

        private static Dictionary<string, KNUserDefaults> defaultsCache = new Dictionary<string, KNUserDefaults>();

        public static KNUserDefaults StandardUserDefaults() {
            return UserDefaultsForDomain(KNBundle.MainBundle().BundleIdentifier);
        }

        public static KNUserDefaults UserDefaultsForDomain(string domain) {

            if (!String.IsNullOrEmpty(domain) && !defaultsCache.ContainsKey(domain)) {
                defaultsCache.Add(domain, new KNUserDefaults(domain));
            }
            return (KNUserDefaults)defaultsCache.ValueForKey(domain);
        }

        // ----

        private string defaultsDomain;

        private Dictionary<string, object> defaults;
        private Dictionary<string, object> userDefaults;

        private KNUserDefaults(string domain) {
            Domain = domain;

            if (!String.IsNullOrEmpty(domain)) {
                try {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    if (Directory.Exists(path)) {
                        path = Path.Combine(path, Domain + ".plist");
                        if (File.Exists(path)) {
                            Dictionary<string, object> plist = KNPropertyListSerialization.PropertyListWithData(File.ReadAllBytes(path));
                            if (plist != null) {
                                userDefaults = plist;
                            } else {
                                userDefaults = new Dictionary<string, object>();
                            }
                        }
                    }
                } catch { }
            }
        }

        ~KNUserDefaults() {
            try {
                Synchronise();
            } catch { }
        }

        public void Synchronise() {

            if (!String.IsNullOrEmpty(Domain) && userDefaults != null && userDefaults.Keys.Count > 0) {

                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }

                path = Path.Combine(path, Domain + ".plist");

                try {
                    File.WriteAllBytes(path, KNPropertyListSerialization.DataWithPropertyList(userDefaults));
                } catch { }
            }
        }

        public void Clear() {
            userDefaults.Clear();
            Synchronise();

            try {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(path, Domain + ".plist");
                File.Delete(path);
            } catch (Exception) {

            }
        }

        // --

        public object ObjectForKey(string key) {

            if (userDefaults != null) {
                if (userDefaults.ContainsKey(key)) {
                    object val = null;
                    if (userDefaults.TryGetValue(key, out val)) {
                        return val;
                    }
                }
            }

            // Fall back to default values

            if (Defaults != null) {
                if (Defaults.ContainsKey(key)) {
                    object val = null;
                    if (Defaults.TryGetValue(key, out val)) {
                        return val;
                    }
                }
            }

            return null;
        }

        public void SetObjectForKey(object obj, string key) {

            if (!String.IsNullOrEmpty(key)) {

                this.WillChangeValueForKey(key);

                if (userDefaults == null) {
                    userDefaults = new Dictionary<string, object>();
                }
                userDefaults.SetValueForKey(obj, key);

                this.DidChangeValueForKey(key);
            }
        }

        public Object ValueForKey(String key) {
            return ObjectForKey(key);
        }

        public void SetValueForKey(Object value, String key) {
            SetObjectForKey(value, key);
        }

        public string StringForKey(string key) {
            object val = ObjectForKey(key);
            if (val != null) {
                return (string)val;
            } else {
                return "";
            }
        }

        public void SetStringForKey(string value, string key) {
            SetObjectForKey(value, key);
        }

        public int IntegerForKey(string key) {
            object val = ObjectForKey(key);
            if (val != null) {
                return (int)val;
            } else {
                return 0;
            }
        }

        public void SetIntegerForKey(int value, string key) {
            SetObjectForKey(value, key);
        }

        public bool BoolForKey(string key) {
            object val = ObjectForKey(key);
            if (val != null) {
                return (bool)val;
            } else {
                return false;
            }
        }

        public void SetBoolForKey(bool value, string key) {
            SetObjectForKey(value, key);
        }

        public double DoubleForKey(string key) {
            object val = ObjectForKey(key);
            if (val != null) {
                return (double)val;
            } else {
                return 0.0;
            }
        }

        public void SetDoubleForKey(double value, string key) {
            SetObjectForKey(value, key);
        }

        // -- 

        public Dictionary<string, object> Defaults {
            private get { return defaults; }
            set {
                this.WillChangeValueForKey("Defaults");
                defaults = value;
                this.DidChangeValueForKey("Defaults");
            }
        }

        public string Domain {
            get { return defaultsDomain; }
            private set {
                this.WillChangeValueForKey("Domain");
                defaultsDomain = value;
                this.DidChangeValueForKey("Domain");
            }
        }

        

    }
}
