using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {

    /// <summary>
    /// Extension methods to provide Key-Value Observing methods to objects.
    /// </summary>
    public static class KNKVOObservable {


        /// <summary>
        /// Adds an observer to a KVO-compliant key path of the receiver. 
        /// </summary>
        /// <param name="o">The object to be observed.</param>
        /// <param name="observer">The object to receive the change notifications.</param>
        /// <param name="keyPath">The key path to observe.</param>
        /// <param name="options">A bitwise-OR of the desired options. See the <c>KNKeyValueObservingOptions</c> enum for details.</param>
        /// <param name="context">A unique context to identify this observation.</param>
        /// <seealso cref="KNKeyValueObservingOptions"/>
        public static void AddObserverToKeyPathWithOptions(this Object o, KNKVOObserver observer, String keyPath, KNKeyValueObservingOptions options, Object context) {
            KNKVOCore.SharedCore().AddObserverToKeyPathOfObject(o, keyPath, observer, options, context);
        }

        /// <summary>
        /// Removes an observer from a KVO-compliant key path of the receiver.
        /// </summary>
        /// <param name="o">The object to remove the observation from.</param>
        /// <param name="observer">The observer to remove.</param>
        /// <param name="keyPath">The key path of the observation to remove.</param>
        public static void RemoveObserverFromKeyPath(this Object o, KNKVOObserver observer, String keyPath) {
            KNKVOCore.SharedCore().RemoveObserverFromKeyPathOfObject(o, keyPath, observer);
        }

        /// <summary>
        /// If a given property doesn't support auto-KVO, this MUST be called before changing 
        /// a value for the property to be KVO-compliant. 
        /// </summary>
        /// <param name="o">The object that will change its key.</param>
        /// <param name="key">The key that will change.</param>
        public static void WillChangeValueForKey(this Object o, String key) {
            KNKVOCore.SharedCore().ObjectWillChangeValueForKey(o, key);
        }

        /// <summary>
        /// If a given property doesn't support auto-KVO, this MUST be called after changing 
        /// a value for the property to be KVO-compliant.
        /// </summary>
        /// <param name="o">The object that changed its key.</param>
        /// <param name="key">The key that changed.</param>
        public static void DidChangeValueForKey(this Object o, String key) {
            KNKVOCore.SharedCore().ObjectDidChangeValueForKey(o, key);
        }

    }

}
