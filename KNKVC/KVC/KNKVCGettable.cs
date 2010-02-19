﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;

namespace KNKVC
{
    public static class KNKVCGettable
    {

        /* LEFT TO DO:
         * Getting Values
        – mutableArrayValueForKey:  
        – mutableArrayValueForKeyPath:  
        – mutableSetValueForKey:  
        – mutableSetValueForKeyPath:  
        */

        // Equivalent to -valueForKeyPath:. Navigates the key path and calls setValueForKey on the final found object.
        public static Object valueForKeyPath(this Object o, String keyPath)
        {
            // This is a recursive method. Hooray!

            String[] paths = keyPath.Split('.');

            if (paths.Length == 1)
            {
                return o.valueForKey(paths[0]);
            }
            else
            {
                Object nextObject = o.valueForKey(paths[0]);
                return nextObject.valueForKeyPath(String.Join(".", paths, 1, paths.Length - 1));
            }
        }


        // Equivalent to –valueForKey:  
        public static Object valueForKey(this Object o, String key)
        {
            // First, try to get the getter for a property.
            // Then, try "key" method. 
            // Then, raise an exception!

            // Property

            if (typeof(IDictionary).IsAssignableFrom(o.GetType()))
            {
                return KNDictionaryKVC.valueForKey((IDictionary)o, key);
            }

            try
            {
                PropertyInfo property = o.GetType().GetProperty(key);
                MethodInfo getPropertyMethod = property.GetGetMethod(true);

                try
                {
                    return getPropertyMethod.Invoke(o, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, null, null);
                }
                catch (Exception e)
                {
                    // Calling the found method failed. 
                    throw new KNKVCMethodInvokeFailedException(e);
                }
            }
            catch (KNKVCMethodInvokeFailedException ex)
            {
                // Rethrow
                throw ex;
            }
            catch
            {
                // Property method not found. We can continue
            }

            try
            {
                
                MethodInfo method = o.GetType().GetMethod(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                try
                {
                    if (!(method == null))
                    {
                        return method.Invoke(o, null);
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

            return o.valueForUndefinedKey(key);
           
        }

        // Equivalent to -dictionaryWithValuesForKeys:

        public static Dictionary<String, Object> dictionaryWithValuesForKeys(this Object o, String[] keys)
        {
            Dictionary<String, Object> values = new Dictionary<String, Object>();

            foreach (String key in keys)
            {
                values.Add(key, o.valueForKey(key));
            }

            return values;
        }

        // Equivalent to valueForUndefinedKey:
        public static Object valueForUndefinedKey(this Object o, String key) {
            Exception noKeyExeption = new Exception("Class " + o.GetType().Name + " is not Key-Value Coding compliant for key \"" + key + "\".");
            throw noKeyExeption;
        }


    }
}
