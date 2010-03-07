using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace KNFoundation.KNKVC {
    public class KNKVOObservationInfo {
        private Stack<KNKVOObservationChangeTracker> changes;

        public KNKVOObservationInfo(String aKeyPath, long someOptions, KNKVOObserver anObserver, Object anObservedObject, Object aContext) {
            changes = new Stack<KNKVOObservationChangeTracker>();
            ObservedObjectReference = new WeakReference(anObservedObject);
            Options = someOptions;
            Observer = anObserver;
            KeyPath = aKeyPath;
            Context = aContext;

            if ((Options & KNKVOConstants.KNKeyValueObservingOptionInitial) == KNKVOConstants.KNKeyValueObservingOptionInitial) {
                Dictionary<String, Object> change = new Dictionary<String, Object>();

                if ((Options & KNKVOConstants.KNKeyValueObservingOptionNew) == KNKVOConstants.KNKeyValueObservingOptionNew) {
                    change.SetValueForKey(ObservedObject.ValueForKey(KeyPath), KNKVOConstants.KNKeyValueChangeNewKey);
                }

                Observer.ObserveValueForKeyPathOfObject(KeyPath, ObservedObject, change, Context);
            }
        }

        public Object Context { get; private set; }
        public String KeyPath { get; private set; }
        public KNKVOObserver Observer { get; private set; }
        public long Options { get; private set; }
        private WeakReference ObservedObjectReference { get; set; }

        public Object ObservedObject {
            get {
                if (ObservedObjectReference == null) {
                    return null;
                } else {
                    return ObservedObjectReference.Target;
                }
            }
        }

        // 0----------------------------------

        public void ValueWillChange() {

            Object oldValue = ObservedObject.ValueForKey(KeyPath);
            changes.Push(new KNKVOObservationChangeTracker(oldValue));

            if ((Options & KNKVOConstants.KNKeyValueObservingOptionPrior) == KNKVOConstants.KNKeyValueObservingOptionPrior) {
                Dictionary<String, Object> change = new Dictionary<String, Object>();
                change.SetValueForKey(true, KNKVOConstants.KNKeyValueChangeNotificationIsPriorKey);
                change.SetValueForKey(oldValue, KeyPath);
            }

        }

        public void ValueDidChange() {
            Object newValue = ObservedObject.ValueForKey(KeyPath);
            KNKVOObservationChangeTracker tracker = changes.Pop();
            Object oldValue = tracker.OldValue;

            Dictionary<String, Object> change = new Dictionary<String, Object>();

            if ((Options & KNKVOConstants.KNKeyValueObservingOptionNew) == KNKVOConstants.KNKeyValueObservingOptionNew) {
                change.SetValueForKey(newValue, KNKVOConstants.KNKeyValueChangeNewKey);
            }

            if ((Options & KNKVOConstants.KNKeyValueObservingOptionOld) == KNKVOConstants.KNKeyValueObservingOptionOld) {
                change.SetValueForKey(oldValue, KNKVOConstants.KNKeyValueChangeOldKey);
            }

            Observer.ObserveValueForKeyPathOfObject(KeyPath, ObservedObject, change, Context);
        }


        private class KNKVOObservationChangeTracker {

            public KNKVOObservationChangeTracker(Object anOldValue) {
                OldValue = anOldValue;
            }

            public Object OldValue {
                get;
                set;
            }

        }

    }


}
