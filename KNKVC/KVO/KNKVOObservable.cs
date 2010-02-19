using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KNKVC;

namespace KNKVC {
    public static class KNKVOObservable {

        public static void addObserverToKeyPathWithOptions(this Object o, KNKVOObserver observer, String keyPath, long options, Object context)
        {
            KNKVOCore.sharedCore().addObserverToKeyPathOfObject(o, keyPath, observer, options, context);
        }

        public static void removeObserverFromKeyPath(this Object o, KNKVOObserver observer, String keyPath)
        {
            KNKVOCore.sharedCore().removeObserverFromKeyPathOfObject(o, keyPath, observer);
        }

        public static void willChangeValueForKey(this Object o, String key)
        {
            KNKVOCore.sharedCore().objectWillChangeValueForKey(o, key);
        }

        public static void didChangeValueForKey(this Object o, String key)
        {
            KNKVOCore.sharedCore().objectDidChangeValueForKey(o, key);
        }

    }   

}
