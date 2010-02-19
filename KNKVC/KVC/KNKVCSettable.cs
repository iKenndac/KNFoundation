using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;

namespace KNKVC
{

    public static class KNKVCSettable
    {


        // Equivalent to –setValue:forKeyPath:. Navigates the key path and calls setValueForKey on the final found object.
        public static void setValueForKeyPath(this Object o, Object value, String keyPath)
        {
            
            // This is a recursive method. Hooray!

            String[] paths = keyPath.Split('.');

            if (paths.Length == 1)
            {
                o.setValueForKey(value, paths[0]);
            }
            else
            {
                Object nextObject = o.valueForKey(paths[0]);
                nextObject.setValueForKeyPath(value, String.Join(".", paths, 1, paths.Length - 1));

            }
        }
       
        // Equivalent to –setValue:forKey:. 
        public static void setValueForKey(this Object o, Object value, String key)
        {
            // First, try to get the setter for a property.
            // Then, try "set<KeyPath>" method. 
            // Then, call "setValueForUndefinedKey".

            if (typeof(IDictionary).IsAssignableFrom(o.GetType()))
            {
                KNDictionaryKVC.setValueForKey((IDictionary)o, value, key);
                return;
            }

            Object[] paramArray = new Object[1];
            paramArray[0] = value;

            // Property

            try
            {
                PropertyInfo property = o.GetType().GetProperty(key);
                MethodInfo setPropertyMethod = property.GetSetMethod(true);

                 try
                 {
                     setPropertyMethod.Invoke(o, paramArray);
                     return;
                 }
                 catch (Exception e)
                 {
                     // Calling the found method failed. 
                     throw new KNKVCMethodInvokeFailedException(e);
                 }
            }
            catch (KNKVCMethodInvokeFailedException ex) {
                // Rethrow
               throw ex;
            }
            catch
            {
                // Property method not found. We can continue
            }

            try
            {
                // convert 'key' to "setKey"
                String methodSignature = "set" + key.Substring(0, 1).ToUpper() + key.Substring(1);
                MethodInfo method = o.GetType().GetMethod(methodSignature, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                try
                {
                    if (!(method == null))
                    {
                        method.Invoke(o, paramArray);
                        return;
                    }
                }
                catch (Exception e)
                {
                    // Calling the found method failed. 
                    throw new KNKVCMethodInvokeFailedException(e);
                }
            }
            catch (KNKVCMethodInvokeFailedException ex)
            {
                throw ex;
            }
            catch
            {
                // Not found again. We can continue.
            }

            o.setValueForUndefinedKey(value, key);
        }

        // Equivalent to –setNilValueForKey:. Simply calls setValueForKeyPath with a null value.
        public static void setNullValueForKey(this Object o, String key)
        {
            o.setValueForKeyPath(null, key);
        }

        // Equivalent to –setValuesForKeysWithDictionary:. Loops through the given dictionary and calls setValueForKeyPath with each.
        public static void setValuesForKeysWithDictionary(this Object o, Dictionary<String, Object> keysAndValues)
        {
            foreach (String key in keysAndValues.Keys) {

                Object value = null;

                if (keysAndValues.TryGetValue(key, out value))
                {
                    o.setValueForKeyPath(value, key);
                }
                else
                {
                    o.setNullValueForKey(key);
                }
            }

        }

        // Equivalent to –setValue:forUndefinedKey:. Override to customise. 
        public static void setValueForUndefinedKey(this Object o, Object value, String key)
        {
            Exception ex = new Exception("Class " + o.GetType().Name + " is not Key-Value Coding compliant for key \"" + key + "\".");
            throw ex;
        }

    }
}
