using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

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
                if (nextObject != null) {
                    nextObject.SetValueForKeyPath(value, String.Join(".", paths, 1, paths.Length - 1));
                }
            }
        }

        public delegate void SetValueForKeyInvoker(Object value, String key);
        private delegate void SetterInvoker(object value);

        /// <summary>
        /// Attempts to set a Key-Value Coding compliant value on the given object using the given key. 
        /// A Key-Value Coding compliant value is either a settable property named identically to the key
        /// or a method named <c>SetKey()</c>) that takes a single value.
        /// If the key doesn't exist, attempts to set <c>SetValueForUndefinedKey()</c> on the object, which
        /// by default throws an exception. Equivalent to <c>-setValue:forKey:</c> in Cocoa.
        /// </summary>
        /// <param name="o">The base object.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="key">The key to set.</param>
        public static void SetValueForKey(this Object o, Object value, String key) {
            // First, try to get the setter for a property.
            // Then, try "set<KeyPath>" method. 
            // Then, call "SetValueForUndefinedKey".


            if (typeof(DispatcherObject).IsAssignableFrom(o.GetType())) {

                DispatcherObject dispatcherObj = (DispatcherObject)o;
                if (!dispatcherObj.CheckAccess()) {
                    dispatcherObj.Dispatcher.Invoke(new SetValueForKeyInvoker(o.SetValueForKey), DispatcherPriority.Normal, new object[] { value, key });
                    return;
                }
            }

            if (typeof(IDictionary).IsAssignableFrom(o.GetType())) {
                KNDictionaryKVC.SetValueForKey((IDictionary)o, value, key);
                return;
            }

            if (typeof(KNUserDefaults).IsAssignableFrom(o.GetType())) {
                ((KNUserDefaults)o).SetValueForKey(value, key);
                return;
            }

            Object[] paramArray = new Object[1];
            paramArray[0] = value;

            // Property

            try {
                PropertyInfo property = o.GetType().GetProperty(key);

                if (property != null) {
                    MethodInfo setPropertyMethod = property.GetSetMethod(true);
                    try {
                        setPropertyMethod.Invoke(o, paramArray);
                        return;
                    } catch (Exception e) {
                        // Calling the found method failed. 
                        throw new KNKVCMethodInvokeFailedException(e);
                    }
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

        /// <summary>
        /// Sets a value of the given key to null. Equivalent to Cocoa's <c>–setNilValueForKey:</c>.
        /// </summary>
        /// <param name="o">The base object.</param>
        /// <param name="key">The key to set to null.</param>
        public static void SetNullValueForKey(this Object o, String key) {
            o.SetValueForKeyPath(null, key);
        }

        /// <summary>
        /// Sets a dictionary of keys at once. Calls <c>SetValueForKey()</c> for each 
        /// key in the Dictionary. Equivalent to Cocoa's <c>–setValuesForKeysWithDictionary:</c>.
        /// </summary>
        /// <param name="o">The base object.</param>
        /// <param name="keysAndValues">A Dictionary of keys (NOT key paths) and values to set.</param>
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

        /// <summary>
        /// Called when <c>SetValueForKey()</c> can't find a Key-Value coding compliant setter for the given
        /// key. If you wish to define custom behaviour for your object's KVC compliance, it's recommended that
        /// you override this method. The default implementation throws an exception. Equivalent to Cocoa's
        /// <c>–setValue:forUndefinedKey:</c>.
        /// </summary>
        /// <param name="o">The base object.</param>
        /// <param name="value">The value that should be set.</param>
        /// <param name="key">The key the value should be set for.</param>
        public static void SetValueForUndefinedKey(this Object o, Object value, String key) {
            Exception ex = new Exception("Class " + o.GetType().Name + " is not Key-Value Coding compliant for key \"" + key + "\".");
            throw ex;
        }

    }
}
