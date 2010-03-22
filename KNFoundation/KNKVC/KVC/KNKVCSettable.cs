using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;

namespace KNFoundation.KNKVC {

    public static class KNKVCSettable {


        // Equivalent to –SetValue:forKeyPath:. Navigates the key path and calls SetValueForKey on the final found object.
        public static void SetValueForKeyPath(this Object o, Object value, String keyPath) {

            // This is a recursive method. Hooray!

            String[] paths = keyPath.Split('.');

            if (paths.Length == 1) {
                o.SetValueForKey(value, paths[0]);
            } else {
                Object nextObject = o.ValueForKey(paths[0]);
                nextObject.SetValueForKeyPath(value, String.Join(".", paths, 1, paths.Length - 1));

            }
        }

        // Equivalent to –SetValue:forKey:. 
        public static void SetValueForKey(this Object o, Object value, String key) {
            // First, try to get the setter for a property.
            // Then, try "set<KeyPath>" method. 
            // Then, call "SetValueForUndefinedKey".

            if (typeof(IDictionary).IsAssignableFrom(o.GetType())) {
                KNDictionaryKVC.SetValueForKey((IDictionary)o, value, key);
                return;
            }

            Object[] paramArray = new Object[1];
            paramArray[0] = value;

            // Property

            try {
                PropertyInfo property = o.GetType().GetProperty(key);
                MethodInfo setPropertyMethod = property.GetSetMethod(true);

                try {
                    setPropertyMethod.Invoke(o, paramArray);
                    return;
                } catch (Exception e) {
                    // Calling the found method failed. 
                    throw new KNKVCMethodInvokeFailedException(e);
                }
            } catch (KNKVCMethodInvokeFailedException ex) {
                // Rethrow
                throw ex;
            } catch {
                // Property method not found. We can continue
            }

            try {
                // convert 'key' to "setKey"
                String methodSignature = "set" + key.Substring(0, 1).ToUpper() + key.Substring(1);
                MethodInfo method = o.GetType().GetMethod(methodSignature, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                try {
                    if (!(method == null)) {
                        method.Invoke(o, paramArray);
                        return;
                    }
                } catch (Exception e) {
                    // Calling the found method failed. 
                    throw new KNKVCMethodInvokeFailedException(e);
                }
            } catch (KNKVCMethodInvokeFailedException ex) {
                throw ex;
            } catch {
                // Not found again. We can continue.
            }

            o.SetValueForUndefinedKey(value, key);
        }

        // Equivalent to –setNilValueForKey:. Simply calls SetValueForKeyPath with a null value.
        public static void SetNullValueForKey(this Object o, String key) {
            o.SetValueForKeyPath(null, key);
        }

        // Equivalent to –SetValuesForKeysWithDictionary:. Loops through the given dictionary and calls SetValueForKeyPath with each.
        public static void SetValuesForKeysWithDictionary(this Object o, Dictionary<String, Object> keysAndValues) {
            foreach (String key in keysAndValues.Keys) {

                Object value = null;

                if (keysAndValues.TryGetValue(key, out value)) {
                    o.SetValueForKeyPath(value, key);
                } else {
                    o.SetNullValueForKey(key);
                }
            }

        }

        // Equivalent to –SetValue:forUndefinedKey:. Override to customise. 
        public static void SetValueForUndefinedKey(this Object o, Object value, String key) {
            Exception ex = new Exception("Class " + o.GetType().Name + " is not Key-Value Coding compliant for key \"" + key + "\".");
            throw ex;
        }

    }
}
