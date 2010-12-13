using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;


namespace KNFoundation.KNKVC {
    internal class KNKVOObservationInfo : KNKVOObserver, IDisposable {
        private Stack<KNKVOObservationChangeTracker> changes;
        private String[] keyPathsRelatedToObservation;

        public KNKVOObservationInfo(String aKey, KNKeyValueObservingOptions someOptions, KNKVOObserver anObserver, Object anObservedObject, Object aContext) {
            changes = new Stack<KNKVOObservationChangeTracker>();
            observedObjectReference = new WeakReference(anObservedObject);
            options = someOptions;
            observer = anObserver;
            key = aKey;
            context = aContext;

            // Get a helper object if needed 

            if (anObservedObject != null) {
                helper = KNKVOCore.SharedCore().HelperForObject(anObservedObject);
            }

            // Check for coupled keys 

            String keyPathsMethodName = "KeyPathsForValuesAffecting" + key.Substring(0, 1).ToUpper() + key.Substring(1);

            MethodInfo keyPathsMethod = observedObject.GetType().GetMethod(keyPathsMethodName, BindingFlags.Static | BindingFlags.Public);
            try {
                if (!(keyPathsMethod == null)) {
                    String[] relatedKeys = (String[])keyPathsMethod.Invoke(observedObject, null);

                    foreach (String relatedKey in relatedKeys) {
                        observedObject.AddObserverToKeyPathWithOptions(
                            this,
                            relatedKey,
                            KNKeyValueObservingOptions.KNKeyValueObservingOptionOld |
                            KNKeyValueObservingOptions.KNKeyValueObservingOptionNew |
                            KNKeyValueObservingOptions.KNKeyValueObservingOptionPrior,
                            null);
                    }

                    keyPathsRelatedToObservation = relatedKeys;
                }
            } catch {
                // Silence! 
            }

            // ----

            if ((options & KNKeyValueObservingOptions.KNKeyValueObservingOptionInitial) == KNKeyValueObservingOptions.KNKeyValueObservingOptionInitial) {
                fireObservation(observedObject.ValueForKey(aKey), null, false);
            }
        }


        public Object context { get; private set; }
        public String key { get; private set; }
        public KNKVOObserver observer { get; private set; }
        public KNKeyValueObservingOptions options { get; private set; }
        private WeakReference observedObjectReference { get; set; }
        private KNKVOHelper helper { get; set; }

        public Object observedObject {
            get {
                if (observedObjectReference == null) {
                    return null;
                } else {
                    return observedObjectReference.Target;
                }
            }
        }

        // 0----------------------------------

        public void valueWillChange() {

            Object oldValue = observedObject.ValueForKeyPath(key);
            changes.Push(new KNKVOObservationChangeTracker(oldValue, key));

            if ((options & KNKeyValueObservingOptions.KNKeyValueObservingOptionPrior) == KNKeyValueObservingOptions.KNKeyValueObservingOptionPrior) {
                fireObservation(null, oldValue, true);
            }

        }

        public void valueDidChange() {
            Object newValue = observedObject.ValueForKeyPath(key);
            KNKVOObservationChangeTracker tracker = changes.Pop();
            Object oldValue = tracker.OldValue;

            fireObservation(newValue, oldValue, false);
        }


        public void ObserveValueForKeyPathOfObject(string keyPath, object obj, Dictionary<string, object> change, object context) {
            // This is fired when an observation of one of the observed object's  
            // keyPathsForValuesAffecting<Key> observers fires.

            if (change.ContainsKey(KNKVOConstants.KNKeyValueChangeNotificationIsPriorKey)) {
                this.valueWillChange();
            } else {
                this.valueDidChange();
            }

        }

        private void fireObservation(Object newValue, Object oldValue, Boolean isPrior) {

            Dictionary<String, Object> change = new Dictionary<String, Object>();

            if ((options & KNKeyValueObservingOptions.KNKeyValueObservingOptionNew) == KNKeyValueObservingOptions.KNKeyValueObservingOptionNew) {
                if (newValue != null) {
                    change.SetValueForKey(newValue, KNKVOConstants.KNKeyValueChangeNewKey);
                }
            }

            if ((options & KNKeyValueObservingOptions.KNKeyValueObservingOptionOld) == KNKeyValueObservingOptions.KNKeyValueObservingOptionOld) {
                if (oldValue != null) {
                    change.SetValueForKey(oldValue, KNKVOConstants.KNKeyValueChangeOldKey);
                }
            }

            if (isPrior) { change.SetValueForKey(true, KNKVOConstants.KNKeyValueChangeNotificationIsPriorKey); }

            observer.ObserveValueForKeyPathOfObject(key, observedObject, change, context);
        }

        public void Dispose() {
            if (keyPathsRelatedToObservation != null) {
                foreach (String relatedKey in keyPathsRelatedToObservation) {
                    observedObject.RemoveObserverFromKeyPath(this, relatedKey);
                }
            }

            if (helper != null) {
                KNKVOCore.SharedCore().HelperIsNoLongerNeeded(helper);
            }
        }

    }


}
