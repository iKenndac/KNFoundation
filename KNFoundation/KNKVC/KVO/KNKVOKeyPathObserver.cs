using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {
    internal class KNKVOKeyPathObserver : KNKVOObserver, IDisposable {

        private Stack<KNKVOObservationChangeTracker> changes;
        ArrayList previousObservations = new ArrayList();

        public KNKVOKeyPathObserver(Object aBaseObject, String aKeyPath, KNKVOObserver anObserver, KNKeyValueObservingOptions someOptions, Object aContext) {
            changes = new Stack<KNKVOObservationChangeTracker>();
            previousObservations = new ArrayList();
            baseObjectRef = new WeakReference(aBaseObject);
            keyPath = aKeyPath;
            context = aContext;
            options = someOptions;
            observer = anObserver;

            this.ObserveValueForKeyPathOfObject(aKeyPath, observedObject, null, context);

            if ((options & KNKeyValueObservingOptions.KNKeyValueObservingOptionInitial) == KNKeyValueObservingOptions.KNKeyValueObservingOptionInitial) {
                Dictionary<String, Object> change = new Dictionary<String, Object>();

                if ((options & KNKeyValueObservingOptions.KNKeyValueObservingOptionNew) == KNKeyValueObservingOptions.KNKeyValueObservingOptionNew) {
                    change.SetValueForKey(observedObject.ValueForKeyPath(keyPath), KNKVOConstants.KNKeyValueChangeNewKey);
                }

                observer.ObserveValueForKeyPathOfObject(keyPath, observedObject, change, context);
            }

        }

        private WeakReference baseObjectRef { get; set; }
        public KNKVOObserver observer { get; private set; }
        public KNKeyValueObservingOptions options { get; private set; }
        public String keyPath { get; private set; }
        public Object context { get; private set; }


        public Object observedObject {
            get {

                if (baseObjectRef.IsAlive) {
                    return baseObjectRef.Target;
                } else {
                    return null;
                }
            }

        }

        public void ObserveValueForKeyPathOfObject(String aKeyPath, Object anObj, Dictionary<String, Object> change, Object aContext) {
            if (change.ValueForKey(KNKVOConstants.KNKeyValueChangeNotificationIsPriorKey) != null) {

                Object oldValue = observedObject.ValueForKeyPath(keyPath);
                KNKVOObservationChangeTracker tracker = new KNKVOObservationChangeTracker(oldValue, aKeyPath);
                changes.Push(tracker);

                // Remove our observers completely
                Object currentObj = observedObject;
                ArrayList keys = new ArrayList(keyPath.Split('.'));

                foreach (String key in keys) {

                    previousObservations.Add(new KeyValuePair<Object, String>(currentObj, key));
                    currentObj = currentObj.ValueForKey(key);
                    if (currentObj == null) { break; }
                }

                if ((options & KNKeyValueObservingOptions.KNKeyValueObservingOptionPrior) == KNKeyValueObservingOptions.KNKeyValueObservingOptionPrior) {
                    Dictionary<String, Object> changeDict = new Dictionary<String, Object>();
                    changeDict.SetValueForKey(true, KNKVOConstants.KNKeyValueChangeNotificationIsPriorKey);
                    changeDict.SetValueForKey(oldValue, KNKVOConstants.KNKeyValueChangeOldKey);
                }
            } else {
                foreach (KeyValuePair<Object, String> oldObservation in previousObservations) {
                    oldObservation.Key.RemoveObserverFromKeyPath(this, oldObservation.Value);
                }
                previousObservations.Clear();

                // Add it, and send the notification down
                Object currentObj = observedObject;
                ArrayList keys = new ArrayList(keyPath.Split('.'));

                foreach (String key in keys) {
                    currentObj.AddObserverToKeyPathWithOptions(
                        this,
                        key,
                        KNKeyValueObservingOptions.KNKeyValueObservingOptionPrior | KNKeyValueObservingOptions.KNKeyValueObservingOptionOld | KNKeyValueObservingOptions.KNKeyValueObservingOptionNew,
                        aContext);
                    currentObj = currentObj.ValueForKey(key);
                    if (currentObj == null) { break; }
                }

                Dictionary<String, Object> changeDict = new Dictionary<String, Object>();

                if (changes.Count > 0) {
                    KNKVOObservationChangeTracker tracker = changes.Pop();
                    if (((options & KNKeyValueObservingOptions.KNKeyValueObservingOptionOld) == KNKeyValueObservingOptions.KNKeyValueObservingOptionOld) && (tracker != null)) {
                        changeDict.SetValueForKey(tracker.OldValue, KNKVOConstants.KNKeyValueChangeOldKey);
                    }
                }

                changeDict.SetValueForKey(observedObject.ValueForKeyPath(keyPath), KNKVOConstants.KNKeyValueChangeNewKey);
                observer.ObserveValueForKeyPathOfObject(keyPath, observedObject, changeDict, context);
            }

        }

        public void Dispose() {
            // Remove our observers completely
            Object currentObj = observedObject;
            ArrayList keys = new ArrayList(keyPath.Split('.'));

            foreach (String key in keys) {
                if (currentObj == null) { break; }
                currentObj.RemoveObserverFromKeyPath(this, key);
                currentObj = currentObj.ValueForKey(key);

            }
        }
    }

}
