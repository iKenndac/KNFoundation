using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace KNFoundation.KNKVC {
    /// <summary>
    /// IList specific KVC features.
    /// </summary>
    public static class KNArrayKVC {
        /// <summary>
        /// Gets an array of the receiver's members' values for the given key.
        /// </summary>
        /// <param name="o">The base array.</param>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>An array constructed by calling <c>ValueForKey()</c> on each member of the array in turn.</returns>
        public static ArrayList ValueForKey(this IList o, String key) {
            ArrayList list = new ArrayList();

            if (o == null) {
                return list;
            }

            // First, try to get a value from the dictionary. If not found, call super.

            foreach (Object obj in o) {
                list.Add(obj.ValueForKey(key));
            }

            return list;

        }

        /// <summary>
        /// Calls <c>SetValueForKey()</c> with the passed key and value on each member of the array in turn.
        /// </summary>
        /// <param name="o">The base array.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="key">The key to set the value for.</param>
        public static void SetValueForKey(this IList o, Object value, String key) {
            if (value == null) {
                return;
            }

            foreach (Object obj in o) {
                obj.SetValueForKey(value, key);
            }
        }
    }
}
