using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {

    /// <summary>
    /// IDictionary specific KVC features.
    /// </summary>
    static class KNDictionaryKVC {

        /// <summary>
        /// Gets the value for the given key in the dictionary.
        /// </summary>
        /// <param name="o">The base dictionary.</param>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The value contained within the dictionary for the given key, or null if the key doesn't exist.</returns>
        public static Object ValueForKey(this IDictionary o, String key) {
            // First, try to get a value from the dictionary.

            if (o.Contains(key)) {
                return o[key];
            }
            return null;
        }

        /// <summary>
        /// Adds the given value for the given key in the dictionary. If the key doesn't exist, it will be added. If it does, the existing value will be replaced.
        /// </summary>
        /// <param name="o">The base dictionary.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="key">The key to set.</param>
        public static void SetValueForKey(this IDictionary o, Object value, String key) {
            if (value == null) {
                o.Remove(key);
                return;
            }

            if (o.Contains(key)) {
                o.Remove(key);
            }

            o.Add(key, value);
        }
    }
}
