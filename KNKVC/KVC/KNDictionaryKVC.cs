using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace KNKVC
{
    static class KNDictionaryKVC
    {
        public static Object valueForKey(this IDictionary o, String key)
        {
            // First, try to get a value from the dictionary. If not found, call super.
            
            if (o.Contains(key)) {
                return o[key];
            }
            return null;
        }

        public static void setValueForKey(this IDictionary o, Object value, String key)
        {
            if (value == null)
            {
                o.Remove(key);
                return;
            }
                
            if (o.Contains(key))
            {
                o.Remove(key);
            }

            o.Add(key, value);
        }

    }
}
