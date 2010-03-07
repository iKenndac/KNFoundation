using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {
    public static class KNKVOObservable {

        public static void AddObserverToKeyPathWithOptions(this Object o, KNKVOObserver observer, String keyPath, long options, Object context) {
            KNKVOCore.SharedCore().AddObserverToKeyPathOfObject(o, keyPath, observer, options, context);
        }

        public static void RemoveObserverFromKeyPath(this Object o, KNKVOObserver observer, String keyPath) {
            KNKVOCore.SharedCore().RemoveObserverFromKeyPathOfObject(o, keyPath, observer);
        }

        public static void WillChangeValueForKey(this Object o, String key) {
            KNKVOCore.SharedCore().ObjectWillChangeValueForKey(o, key);
        }

        public static void DidChangeValueForKey(this Object o, String key) {
            KNKVOCore.SharedCore().ObjectDidChangeValueForKey(o, key);
        }

    }

}
