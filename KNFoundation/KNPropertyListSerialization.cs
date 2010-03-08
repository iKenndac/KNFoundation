using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using KNFoundation.KNKVC;

namespace KNFoundation {

    

    public class KNPropertyListSerialization {

        public static byte[] DataWithPropertyList(Dictionary<string, object> plist) {
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = new PlistDTDResolver();

            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            doc.AppendChild(doc.CreateDocumentType("plist",
                                                            "-//Apple Computer/DTD PLIST 1.0//EN",
                                                            "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null));

            XmlElement root = doc.CreateElement("plist");
            root.SetAttribute("version", "1.0");

            ArrayList plistNodes = plist.PropertyListRepresentationWithKey(doc, null);
            foreach (XmlElement el in plistNodes) {
                root.AppendChild(el);
            }

            doc.AppendChild(root);

            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            return stream.ToArray();
        }

        /// <summary>
        /// Gets the property list from the given XML data. This implementation of 
        /// KNPropertyListSerialization requires that the base property list is 
        /// a Dictionary<string, object>.
        /// </summary>
        /// <param name="data">The plist XML data.</param>
        /// <returns></returns>
        public static Dictionary<string, object> PropertyListWithData(byte[] data) {

            try {
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = new PlistDTDResolver();
                doc.Load(new MemoryStream(data));

                XmlElement rootEl = doc.DocumentElement;
                if (rootEl.Name.Equals("plist")) {

                    if (rootEl.HasChildNodes) {
                        Dictionary<string, object> plist = DictionaryFromDictElement(rootEl.FirstChild);
                        return plist;
                    }
                }
                return null;
            } catch {
                return null;
            }
        }

        private static Dictionary<string, object> DictionaryFromDictElement(XmlNode dictNode) {

            XmlNodeList nodes = dictNode.ChildNodes;
            Int32 childCount = nodes.Count;
            Int32 currentChild = 0;

            Dictionary<string, Object> dict = new Dictionary<string, object>();

            for (currentChild = 0; currentChild < childCount; currentChild += 2) {

                XmlNode keyNode = nodes[currentChild];
                string key = keyNode.InnerText;

                XmlNode valueNode = nodes[currentChild + 1];

                if (valueNode.Name.Equals("dict")) {
                    dict.Add(key, DictionaryFromDictElement(valueNode));
                } else if (valueNode.Name.Equals("array")) {
                    dict.Add(key, ArrayFromArrayElement(valueNode));
                } else if (valueNode.Name.Equals("string")) {
                    dict.Add(key, StringFromStringElement(valueNode));
                } else if (valueNode.Name.Equals("integer")) {
                    dict.Add(key, IntegerFromIntegerElement(valueNode));
                } else if (valueNode.Name.Equals("date")) {
                    dict.Add(key, DateTimeFromDateElement(valueNode));
                } else if (valueNode.Name.Equals("data")) {
                    dict.Add(key, DataFromDataElement(valueNode));
                } else if (valueNode.Name.Equals("real")) {
                    dict.Add(key, DoubleFromRealElement(valueNode));
                } else if (valueNode.Name.Equals("true")) {
                    dict.Add(key, true);
                } else if (valueNode.Name.Equals("false")) {
                    dict.Add(key, false);
                }               
            }

            return dict;
            
        }

        private static string StringFromStringElement(XmlNode stringNode) {
            return stringNode.InnerText;
        }

        private static int IntegerFromIntegerElement(XmlNode intNode) {
            try {
                return int.Parse(intNode.InnerText);
            } catch {
                return 0;
            }
        }

        private static DateTime DateTimeFromDateElement(XmlNode dateNode) {
            // Todo: This sucks, make it bettter

            string dateString = dateNode.InnerText;

            return new DateTime(int.Parse(dateString.Substring(0, 4)),
                int.Parse(dateString.Substring(5, 2)),
                int.Parse(dateString.Substring(8, 2)),
                int.Parse(dateString.Substring(11, 2)),
                int.Parse(dateString.Substring(14, 2)),
                int.Parse(dateString.Substring(17, 2)));
        }

        private static byte[] DataFromDataElement(XmlNode dataNode) {
            return Convert.FromBase64String(dataNode.InnerText);
        }

        private static double DoubleFromRealElement(XmlNode realNode) {
            try {
                return double.Parse(realNode.InnerText);
            } catch {
                return 0.0;
            }
        }

        private static ArrayList ArrayFromArrayElement(XmlNode arrayNode) {

            ArrayList array = new ArrayList();

            foreach (XmlNode valueNode in arrayNode.ChildNodes) {

                object obj = null;

                if (valueNode.Name.Equals("dict")) {
                    obj = DictionaryFromDictElement(valueNode);
                } else if (valueNode.Name.Equals("array")) {
                    obj = ArrayFromArrayElement(valueNode);
                } else if (valueNode.Name.Equals("string")) {
                    obj = StringFromStringElement(valueNode);
                } else if (valueNode.Name.Equals("integer")) {
                    obj = IntegerFromIntegerElement(valueNode);
                } else if (valueNode.Name.Equals("date")) {
                    obj = DateTimeFromDateElement(valueNode);
                } else if (valueNode.Name.Equals("data")) {
                    obj = DataFromDataElement(valueNode);
                } else if (valueNode.Name.Equals("real")) {
                    obj = DoubleFromRealElement(valueNode);
                } else if (valueNode.Name.Equals("true")) {
                    obj = true;
                } else if (valueNode.Name.Equals("false")) {
                    obj = false;
                }     

                if (obj != null) {
                    array.Add(obj);
                }
            }

            return array;
        }
    }

    internal class PlistDTDResolver : XmlUrlResolver {

        string localDTDName = "PropertyList-1.0.dtd";

        public override Uri ResolveUri(Uri baseUri, string relativeUri) {

            XmlUrlResolver resolver = new XmlUrlResolver();

            if (relativeUri.EndsWith("dtd")) {
                return new Uri(KNBundle.MainBundle().PathForResourceOfType(localDTDName, null));
            } else {
                return base.ResolveUri(baseUri, relativeUri);
            }
        }
    }

    static class KNPropertyListSerializationExtensions {

        private static XmlElement KeyElementForKey(string key, XmlDocument doc) {
            XmlElement keyElement = doc.CreateElement("key");
            keyElement.InnerText = key;
            return keyElement;
        }

        public static ArrayList PropertyListRepresentationWithKey(this string s, XmlDocument doc, string key) {

            ArrayList plistRep = new ArrayList();

            if (!String.IsNullOrWhiteSpace(key)) {
                plistRep.Add(KeyElementForKey(key, doc));
            }

            XmlElement stringEl = doc.CreateElement("string");
            stringEl.InnerText = s;
            plistRep.Add(stringEl);

            return plistRep;
        }

        public static ArrayList PropertyListRepresentationWithKey(this DateTime d, XmlDocument doc, string key) {

            ArrayList plistRep = new ArrayList();

            if (!String.IsNullOrWhiteSpace(key)) {
                plistRep.Add(KeyElementForKey(key, doc));
            }

            XmlElement dateEl = doc.CreateElement("date");
            dateEl.InnerText = d.ToString("yyyy-MM-ddTHH:mm:ssZ");
            plistRep.Add(dateEl);

            return plistRep;
        }

        public static ArrayList PropertyListRepresentationWithKey(this bool b, XmlDocument doc, string key) {

            ArrayList plistRep = new ArrayList();

            if (!String.IsNullOrWhiteSpace(key)) {
                plistRep.Add(KeyElementForKey(key, doc));
            }

            XmlElement boolEl = doc.CreateElement(b.ToString().ToLower());
            plistRep.Add(boolEl);

            return plistRep;
        }

        public static ArrayList PropertyListRepresentationWithKey(this int i, XmlDocument doc, string key) {

            ArrayList plistRep = new ArrayList();

            if (!String.IsNullOrWhiteSpace(key)) {
                plistRep.Add(KeyElementForKey(key, doc));
            }

            XmlElement intEl = doc.CreateElement("integer");
            intEl.InnerText = i.ToString();
            plistRep.Add(intEl);

            return plistRep;
        }

        public static ArrayList PropertyListRepresentationWithKey(this double d, XmlDocument doc, string key) {

            ArrayList plistRep = new ArrayList();

            if (!String.IsNullOrWhiteSpace(key)) {
                plistRep.Add(KeyElementForKey(key, doc));
            }

            XmlElement intEl = doc.CreateElement("real");
            intEl.InnerText = d.ToString();
            plistRep.Add(intEl);

            return plistRep;
        }

        public static ArrayList PropertyListRepresentationWithKey(this byte[] b, XmlDocument doc, string key) {

            ArrayList plistRep = new ArrayList();

            if (!String.IsNullOrWhiteSpace(key)) {
                plistRep.Add(KeyElementForKey(key, doc));
            }

            XmlElement intEl = doc.CreateElement("data");
            intEl.InnerText = Convert.ToBase64String(b);
            plistRep.Add(intEl);

            return plistRep;
        }

        public static ArrayList PropertyListRepresentationWithKey(this Dictionary<string, object> d, XmlDocument doc, string key) {

            ArrayList plistRep = new ArrayList();

            if (!String.IsNullOrWhiteSpace(key)) {
                plistRep.Add(KeyElementForKey(key, doc));
            }

            XmlElement dictEl = doc.CreateElement("dict");

            foreach (string currentKey in d.Keys) {
                ArrayList plistReps = null;

                // Make reps

                object item = null;
                if (d.TryGetValue(currentKey, out item)) {

                    if (item.GetType() == typeof(string)) {
                        plistReps = ((string)item).PropertyListRepresentationWithKey(doc, currentKey);
                    } else if (item.GetType() == typeof(Array)) {
                        plistReps = ((Array)item).PropertyListRepresentationWithKey(doc, currentKey);
                    } else if (item.GetType() == typeof(ArrayList)) {
                        plistReps = (((ArrayList)item).ToArray().PropertyListRepresentationWithKey(doc, currentKey));
                    } else if (item.GetType() == typeof(int)) {
                        plistReps = ((int)item).PropertyListRepresentationWithKey(doc, currentKey);
                    } else if (item.GetType() == typeof(DateTime)) {
                        plistReps = ((DateTime)item).PropertyListRepresentationWithKey(doc, currentKey);
                    } else if (item.GetType() == typeof(bool)) {
                        plistReps = ((bool)item).PropertyListRepresentationWithKey(doc, currentKey);
                    } else if (item.GetType() == typeof(double)) {
                        plistReps = ((double)item).PropertyListRepresentationWithKey(doc, currentKey);
                    } else if (item.GetType() == typeof(Dictionary<string, object>)) {
                        plistReps = ((Dictionary<string, object>)item).PropertyListRepresentationWithKey(doc, currentKey);
                    } else if (item.GetType() == typeof(byte[])) {
                        plistReps = ((byte[])item).PropertyListRepresentationWithKey(doc, currentKey);
                    }

                    if (plistReps != null) {
                        foreach (XmlElement el in plistReps) {
                            dictEl.AppendChild(el);
                        }
                    }
                }
            }

            plistRep.Add(dictEl);
            return plistRep;
        }

        public static ArrayList PropertyListRepresentationWithKey(this Array a, XmlDocument doc, string key) {

            ArrayList plistRep = new ArrayList();

            if (!String.IsNullOrWhiteSpace(key)) {
                plistRep.Add(KeyElementForKey(key, doc));
            }

            XmlElement arrayEl = doc.CreateElement("array");

            foreach (object item in a) {
                ArrayList plistReps = null;

                // Make reps

                if (item.GetType() == typeof(string)) {
                    plistReps = ((string)item).PropertyListRepresentationWithKey(doc, null);
                } else if (item.GetType() == typeof(Array)) {
                    plistReps = ((Array)item).PropertyListRepresentationWithKey(doc, null);
                } else if (item.GetType() == typeof(ArrayList)) {
                    plistReps = (((ArrayList)item).ToArray().PropertyListRepresentationWithKey(doc, null));
                } else if (item.GetType() == typeof(int)) {
                    plistReps = ((int)item).PropertyListRepresentationWithKey(doc, null);
                } else if (item.GetType() == typeof(DateTime)) {
                    plistReps = ((DateTime)item).PropertyListRepresentationWithKey(doc, null);
                } else if (item.GetType() == typeof(bool)) {
                    plistReps = ((bool)item).PropertyListRepresentationWithKey(doc, null);
                } else if (item.GetType() == typeof(double)) {
                    plistReps = ((double)item).PropertyListRepresentationWithKey(doc, null);
                } else if (item.GetType() == typeof(Dictionary<string, object>)) {
                    plistReps = ((Dictionary<string, object>)item).PropertyListRepresentationWithKey(doc, null);
                } else if (item.GetType() == typeof(byte[])) {
                    plistReps = ((byte[])item).PropertyListRepresentationWithKey(doc, null);
                }

                if (plistReps != null) {
                    foreach (XmlElement el in plistReps) {
                        arrayEl.AppendChild(el);
                    }
                }
            }

            plistRep.Add(arrayEl);
            return plistRep;

        }

    }
}
