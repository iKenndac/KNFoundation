using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;

namespace KNFoundation.KNKVC {
    public static class KNKVCGettable {

        /* LEFT TO DO:
         * Getting Values
        – mutableArrayValueForKey:  
        – mutableArrayValueForKeyPath:  
        – mutableSetValueForKey:  
        – mutableSetValueForKeyPath:  
        */

        /// <summary>
        /// Attempts to get a value from the given key path of an object by 
        /// recursively calling <c>ValueForKey</c> until the end of the path or null
        /// is encountered. See <c>ValueForKey</c> for more information. Equivalent to Cocoa's <c>-valueForKeyPath:</c>.
        /// </summary>
        /// <param name="o">The base object.</param>
        /// <param name="keyPath">The key path to retrieve, in the form <c>key.key.property</c>.</param>
        /// <returns>The value at the end of the path, or null.</returns>
        public static Object ValueForKeyPath(this Object o, String keyPath) {
            // This is a recursive method. Hooray!

            String[] paths = keyPath.Split('.');

            if (paths.Length == 1) {
                return o.ValueForKey(paths[0]);
            } else {
                Object nextObject = o.ValueForKey(paths[0]);
                return nextObject.ValueForKeyPath(String.Join(".", paths, 1, paths.Length - 1));
            }
        }


        /// <summary>
        /// Attempts to get a Key-Value Coding compliant value from the given object using the given key. 
        /// A Key-Value Coding compliant value is either a gettable property named identically to the key
        /// or a method named identically to the key (i.e., NOT <c>getKey()</c>) that returns a value.
        /// If the key doesn't exist, attempts to return <c>ValueForUndefinedKey()</c> on the object, which
        /// by default throws an exception. Equivalent to <c>-valueForKey:</c> in Cocoa.
        /// </summary>
        /// <param name="o">The base object.</param>
        /// <param name="key">The key to get.</param>
        /// <returns>The value for the specified key.</returns>  
        public static Object ValueForKey(this Object o, String key) {
            // First, try to get the getter for a property.
            // Then, try "key" method. 
            // Then, raise an exception!

            // Property

            if (typeof(IDictionary).IsAssignableFrom(o.GetType())) {
                return KNDictionaryKVC.ValueForKey((IDictionary)o, key);
            }

            if (typeof(DependencyObject).IsAssignableFrom(o.GetType())) {
                
                // Try to search the dependency object for a sub item, then 
                // check its properties.

                Object obj = KNDependencyObjectKVC.ValueForKey((DependencyObject)o, key);
                if (obj != null) {
                    return obj;
                }
            }

            try {
                PropertyInfo property = o.GetType().GetProperty(key);
                MethodInfo getPropertyMethod = property.GetGetMethod(true);

                try {
                    return getPropertyMethod.Invoke(o, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, null, null);
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

                MethodInfo method = o.GetType().GetMethod(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                try {
                    if (!(method == null)) {
                        return method.Invoke(o, null);
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

            return o.ValueForUndefinedKey(key);

        }

        /// <summary>
        /// Returns a dictionary of the values for the array of keys given. Calls <c>ValueForKey</c> for each
        /// key given. Equivalent to Cocoa's <c>-dictionaryWithValuesForKeys:</c>.
        /// </summary>
        /// <param name="o">The base object.</param>
        /// <param name="keys">An array of keys (NOT key paths) to retrieve.</param>
        /// <returns>A <c>Dictionary(String, Object)</c> containing the values for the given keys.</returns>
        
        public static Dictionary<String, Object> DictionaryWithValuesForKeys(this Object o, String[] keys) {
            Dictionary<String, Object> values = new Dictionary<String, Object>();

            foreach (String key in keys) {
                values.Add(key, o.ValueForKey(key));
            }

            return values;
        }

        /// <summary>
        /// Called when <c>ValueForKey()</c> can't find a Key-Value Coding compliant method for the given 
        /// key. If you wish to define custom behaviour for your object's KVC compliance, it's recommended that 
        /// you override this method. The default implementation throws an exception. Equivalent to Cocoa's 
        /// <c>valueForUndefinedKey:</c>.
        /// </summary>
        /// <param name="o">The base object.</param>
        /// <param name="key">The key that couldn't be found.</param>
        /// <returns></returns>
        public static Object ValueForUndefinedKey(this Object o, String key) {
            Exception noKeyExeption = new Exception("Class " + o.GetType().Name + " is not Key-Value Coding compliant for key \"" + key + "\".");
            throw noKeyExeption;
        }


    }
}
