using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {
    /// <summary>
    /// Defines methods implemented by an object that wishes to register as an observer for a key path.
    /// </summary>
    public interface KNKVOObserver {
        /// <summary>
        /// Called when an observed key path changes.
        /// </summary>
        /// <param name="keyPath">The key path that changed.</param>
        /// <param name="obj">The object on which the key path changed.</param>
        /// <param name="change">The change dictionary. See <c>KNKVOConstants</c> for possible keys.</param>
        /// <param name="context">The context of the observation.</param>
        void ObserveValueForKeyPathOfObject(String keyPath, Object obj, Dictionary<String, Object> change, Object context);
    }
}
