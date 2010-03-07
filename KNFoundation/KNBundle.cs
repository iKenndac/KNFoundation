using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using KNFoundation.KNKVC;
using System.Globalization;
using System.Threading;
using System.Reflection;


namespace KNFoundation {

    public abstract class KNBundleGlobalHelpers {

        public static string KNLocalizedString(string key, string comment) {
            return KNBundle.MainBundle().LocalizedStringForKeyValueTable(key, null, null);
        }

        public static string KNLocalizedStringFromTable(string key, string tableName, string comment) {
            return KNBundle.MainBundle().LocalizedStringForKeyValueTable(key, null, tableName);
        }

        public static string KNLocalizedStringFromTableInBundle(string key, string tableName, KNBundle bundle, string comment) {
            return bundle.LocalizedStringForKeyValueTable(key, null, tableName);
        }

        public static string KNLocalizedStringWithDefaultValue(string key, string tableName, KNBundle bundle, string value, string comment) {
            return bundle.LocalizedStringForKeyValueTable(key, value, tableName);
        }

        // Image cache for BitmapImage.ImageNamed();
        static Dictionary<string, BitmapImage> imageCache = new Dictionary<string, BitmapImage>();

        public static BitmapImage ImageNamed(string imageName) {
            return ImageInDirectoryNamed(imageName, null);
        }

        public static BitmapImage ImageInDirectoryNamed(string directoryName, string imageName) {

            // We cache the images we find for performance reasons. 

            if (String.IsNullOrWhiteSpace(imageName)) {
                return null;
            }

            string name = Path.GetFileName(imageName);

            if (imageCache.ContainsKey(name)) {
                BitmapImage img;
                if (imageCache.TryGetValue(name, out img)) {
                    return img;
                }
            }

            string path = KNBundle.MainBundle().PathForResourceOfTypeInDirectory(imageName, null, directoryName);

            if (!string.IsNullOrWhiteSpace(path)) {
                BitmapImage img = new BitmapImage(new Uri(path));
                imageCache.Add(name, img);
                return img;
            } else {
                return null;
            }
        }
    }

    public class KNBundle {

        // Constants

        public const string KNShortVersionStringKey = "KNShortVersionString";
        public const string KNBundleVersionKey = "KNBundleVersion";
        public const string KNBundleExecutableKey = "KNBundleExecutable";
        public const string KNBundleIdentifierKey = "KNBundleIdentifier";
        public const string KNBundleNameKey = "KNBundleName";
        public const string KNBundleDisplayNameKey = "KNBundleDisplayName";

        // This is an approximation of the NSBundle class. Yay!
        static Dictionary<string, KNBundle> bundleCache = new Dictionary<string, KNBundle>();

        public static KNBundle MainBundle() {

            FileInfo appInfo = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            return KNBundle.BundleWithPath(appInfo.DirectoryName);
        }

        public static KNBundle BundleWithPath(string path) {

            KNBundle bundle;

            if (bundleCache.ContainsKey(path)) {
                if (bundleCache.TryGetValue(path, out bundle)) {
                    return bundle;
                }
            }

            bundle = new KNBundle(path);
            bundleCache.Add(path, bundle);

            return bundle;
        }

        // --------- Instance methods, variables, etc ---------

        private string bundlePath;
        private string cachedLocalisedResourcesPath;
        private Dictionary<string, Dictionary<string, string>> stringsCache;
        private Dictionary<string, object> infoDictionary; // Equivalent to info.plist

        private KNBundle(string path) {
            BundlePath = path;
            infoDictionary = ParseBundleInfoPlist();
            stringsCache = new Dictionary<string, Dictionary<string, string>>();

            // Pre-cache strings files.

            foreach (string stringsFilePath in PathsForResourcesOfType("strings")) {

                Dictionary<string, string> stringsTable = AttemptToParseStringsFile(stringsFilePath);
                if (stringsTable.Keys.Count > 0) {
                    stringsCache.Add(Path.GetFileNameWithoutExtension(stringsFilePath), stringsTable);
                }
            }
        }

        public string LocalizedStringForKeyValueTable(string key, string value, string table) {

            if (String.IsNullOrWhiteSpace(table)) {
                table = "Localizable";
            }

            Dictionary<string, string> stringsTable;

            if (stringsCache.TryGetValue(table, out stringsTable)) {
                string outValue;
                if (stringsTable.TryGetValue(key, out outValue)) {
                    value = outValue;
                }
            }

            return value;
        }


        public string[] PathsForResourcesOfType(string type) {
            return PathsForResourcesOfTypeInDirectory(type, null);
        }

        public string[] PathsForResourcesOfTypeInDirectory(string type, string subDirectory) {

            Dictionary<string, string> namesAndPaths = new Dictionary<string, string>();

            string nonLocalisedPathToSearch = ResourcesPath;
            if (!String.IsNullOrWhiteSpace(subDirectory)) {
                nonLocalisedPathToSearch = Path.Combine(ResourcesPath, subDirectory);
            }

            if (Directory.Exists(nonLocalisedPathToSearch)) {

                foreach (string resourceName in Directory.GetFiles(nonLocalisedPathToSearch)) {

                    string fullPath = Path.Combine(nonLocalisedPathToSearch, resourceName);

                    if (Path.GetExtension(fullPath).Replace(".", "") == type) {
                        namesAndPaths.Add(resourceName, fullPath);
                    }
                }
            }

            // Now do localized. The localized should override the non-localized.

            string localisedPathToSearch = LocalisedResourcesPath;
            if (!String.IsNullOrWhiteSpace(subDirectory)) {
                localisedPathToSearch = Path.Combine(LocalisedResourcesPath, subDirectory);
            }

            if (Directory.Exists(localisedPathToSearch)) {

                foreach (string resourceName in Directory.GetFiles(localisedPathToSearch)) {

                    string fullPath = Path.Combine(localisedPathToSearch, resourceName);

                    if (Path.GetExtension(fullPath).Replace(".", "") == type) {
                        if (namesAndPaths.ContainsKey(resourceName)) {
                            namesAndPaths.Remove(resourceName);
                        }
                        namesAndPaths.Add(resourceName, fullPath);
                    }
                }
            }

            return namesAndPaths.Values.ToArray<string>();
        }

        public string PathForResourceOfType(string resourceName, string resourceType) {
            return PathForResourceOfTypeInDirectory(resourceName, resourceType, null);
        }

        public string PathForResourceOfTypeInDirectory(string resourceName, string resourceType, string subDirectory) {


            string resourceFinalName = resourceName;
            if (!String.IsNullOrWhiteSpace(resourceType)) {
                resourceFinalName += "." + resourceType;
            }

            // Matching NSBundle behaviour, we check for non-resources first.

            string filePath;
            if (!String.IsNullOrWhiteSpace(subDirectory)) {
                filePath = Path.Combine(ResourcesPath, subDirectory, resourceFinalName);
            } else {
                filePath = Path.Combine(ResourcesPath, resourceFinalName);
            }

            if (File.Exists(filePath)) {
                return filePath;
            }

            if (!String.IsNullOrWhiteSpace(subDirectory)) {
                filePath = Path.Combine(LocalisedResourcesPath, subDirectory, resourceFinalName);
            } else {
                filePath = Path.Combine(LocalisedResourcesPath, resourceFinalName);
            }

            if (File.Exists(filePath)) {
                return filePath;
            }

            return null;
        }

        private Dictionary<string, object> ParseBundleInfoPlist() {

            string infoPath = PathForResourceOfType("Info", "plist");

            if (!String.IsNullOrWhiteSpace(infoPath) && File.Exists(infoPath)) {

                try {
                    return KNPropertyListSerialization.PropertyListWithData(File.ReadAllBytes(infoPath));
                } catch (Exception ex) {
                }
            }
            return new Dictionary<string, object>();
        }

        private Dictionary<string, string> AttemptToParseStringsFile(string path) {

            Dictionary<string, string> stringsTable = new Dictionary<string, string>();

            char quoteDelemiter = '"';
            char delemiterEscape = '\\';

            try {

                StreamReader reader = File.OpenText(path);
                string line;

                while ((line = reader.ReadLine()) != null) {
                    // A line is "key" = "value";
                    // Ignore \" as that should be included as a quote. 

                    //TODO: This should be better!
                    try {

                        string key, value;

                        int openingKeyDelimiter = line.IndexOf(quoteDelemiter, 0);
                        while (line.CharacterAtIndexIsEscapedWithCharacter(openingKeyDelimiter, delemiterEscape)) {
                            openingKeyDelimiter = line.IndexOf(quoteDelemiter, openingKeyDelimiter + 1);
                        }

                        int closingKeyDelimiter = line.IndexOf(quoteDelemiter, openingKeyDelimiter + 1);
                        while (line.CharacterAtIndexIsEscapedWithCharacter(closingKeyDelimiter, delemiterEscape)) {
                            closingKeyDelimiter = line.IndexOf(quoteDelemiter, closingKeyDelimiter + 1);
                        }

                        key = line.Substring(openingKeyDelimiter + 1, closingKeyDelimiter - openingKeyDelimiter - 1);

                        int openingValueDelimiter = line.IndexOf(quoteDelemiter, closingKeyDelimiter + 1);
                        while (line.CharacterAtIndexIsEscapedWithCharacter(openingValueDelimiter, delemiterEscape)) {
                            openingValueDelimiter = line.IndexOf(quoteDelemiter, openingValueDelimiter + 1);
                        }

                        int closingValueDelimiter = line.IndexOf(quoteDelemiter, openingValueDelimiter + 1);
                        while (line.CharacterAtIndexIsEscapedWithCharacter(closingValueDelimiter, delemiterEscape)) {
                            closingValueDelimiter = line.IndexOf(quoteDelemiter, closingValueDelimiter + 1);
                        }

                        value = line.Substring(openingValueDelimiter + 1, closingValueDelimiter - openingValueDelimiter - 1);

                        if (!String.IsNullOrWhiteSpace(key) && !String.IsNullOrWhiteSpace(value)) {
                            stringsTable.Add(key.DeEscapedString(), value.DeEscapedString());
                        }

                    } catch (Exception e) {
                        // Don't care. Hooray!
                    }
                }

                reader.Close();
                reader.Dispose();
            } catch (Exception e) {
                // Do nothing!
            }

            return stringsTable;
        }

        #region Info Convenience Properties

        public string Name {
            get {
                if (InfoDictionary.ContainsKey(KNBundleNameKey)) {
                    return (string)InfoDictionary.ValueForKey(KNBundleNameKey);
                } else {
                    return null;
                }
            }
        }

        public string DisplayName {
            get {
                if (InfoDictionary.ContainsKey(KNBundleDisplayNameKey)) {
                    return (string)InfoDictionary.ValueForKey(KNBundleDisplayNameKey);
                } else {
                    return null;
                }
            }
        }

        public string ShortVersionString {
            get {

                if (InfoDictionary.ContainsKey(KNShortVersionStringKey)) {
                    return (string)InfoDictionary.ValueForKey(KNShortVersionStringKey);
                }

                if (InfoDictionary.ContainsKey(KNBundleExecutableKey)) {
                    string assemblyPath = (string)InfoDictionary.ValueForKey(KNBundleExecutableKey);
                    try {
                        AssemblyName name = AssemblyName.GetAssemblyName(assemblyPath);
                        return name.Version.ToString();
                    } catch (Exception ex) {
                    }
                }

                return null;
            }
        }

        public string Version {
            get {
                if (InfoDictionary.ContainsKey(KNBundleVersionKey)) {
                    return (string)InfoDictionary.ValueForKey(KNBundleVersionKey);
                } else {
                    return null;
                }
            }
        }

        public string ExecutablePath {
            get {
                if (InfoDictionary.ContainsKey(KNBundleExecutableKey)) {
                    return (string)InfoDictionary.ValueForKey(KNBundleExecutableKey);
                } else {
                    return null;
                }
            }
        }

        public string BundleIdentifier {
            get {
                if (InfoDictionary.ContainsKey(KNBundleIdentifierKey)) {
                    return (string)InfoDictionary.ValueForKey(KNBundleIdentifierKey);
                } else {
                    return null;
                }
            }
        }

        public Dictionary<string, object> InfoDictionary {
            get { return infoDictionary; }
        }

        #endregion

        #region "Properties"

        public string BundlePath {
            get { return bundlePath; }
            set {
                this.WillChangeValueForKey("Path");
                bundlePath = value;
                this.DidChangeValueForKey("Path");
            }
        }

        public string ResourcesPath {
            get {

                string baseResourcesPath;

                if (Directory.Exists(Path.Combine(BundlePath, "Resources"))) {
                    baseResourcesPath = Path.Combine(BundlePath, "Resources");
                } else {
                    baseResourcesPath = BundlePath;
                }

                return baseResourcesPath;
            }
        }

        public string LocalisedResourcesPath {
            get {

                if (cachedLocalisedResourcesPath != null) {
                    // We cache, because searching through a ton of directories is expensive!
                    return cachedLocalisedResourcesPath;
                }

                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                string resourcesPath = ResourcesPath;
                string foundResourcesPath = resourcesPath;

                string lprojSuffix = ".lproj";

                string currentDirectory = Path.Combine(resourcesPath, currentCulture.Name + lprojSuffix);

                if (Directory.Exists(currentDirectory)) {
                    foundResourcesPath = currentDirectory;
                }

                currentDirectory = Path.Combine(resourcesPath, currentCulture.EnglishName + lprojSuffix);

                if (Directory.Exists(currentDirectory)) {
                    foundResourcesPath = currentDirectory;
                }

                currentDirectory = Path.Combine(resourcesPath, currentCulture.TwoLetterISOLanguageName + lprojSuffix);

                if (Directory.Exists(currentDirectory)) {
                    foundResourcesPath = currentDirectory;
                }

                if (currentCulture.Parent != null) {

                    // Search parent, so for example en-GB or en-US will match en.lproj, English.lproj, etc

                    currentDirectory = Path.Combine(resourcesPath, currentCulture.Parent.Name + lprojSuffix);

                    if (Directory.Exists(currentDirectory)) {
                        foundResourcesPath = currentDirectory;
                    }

                    currentDirectory = Path.Combine(resourcesPath, currentCulture.Parent.EnglishName + lprojSuffix);

                    if (Directory.Exists(currentDirectory)) {
                        foundResourcesPath = currentDirectory;
                    }

                    currentDirectory = Path.Combine(resourcesPath, currentCulture.Parent.TwoLetterISOLanguageName + lprojSuffix);

                    if (Directory.Exists(currentDirectory)) {
                        foundResourcesPath = currentDirectory;
                    }

                }

                // Fall back to english!

                CultureInfo fallbackCulture = new CultureInfo("en");

                currentDirectory = Path.Combine(resourcesPath, fallbackCulture.Name + lprojSuffix);

                if (Directory.Exists(currentDirectory)) {
                    foundResourcesPath = currentDirectory;
                }

                currentDirectory = Path.Combine(resourcesPath, fallbackCulture.EnglishName + lprojSuffix);

                if (Directory.Exists(currentDirectory)) {
                    foundResourcesPath = currentDirectory;
                }

                currentDirectory = Path.Combine(resourcesPath, fallbackCulture.TwoLetterISOLanguageName + lprojSuffix);

                if (Directory.Exists(currentDirectory)) {
                    foundResourcesPath = currentDirectory;
                }

                // Cache the found directory

                cachedLocalisedResourcesPath = foundResourcesPath;

                return foundResourcesPath;

            }
        }

        #endregion

    }
}

