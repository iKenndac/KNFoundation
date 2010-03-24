using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {

    class KNKVOCore {
        // Constants, etc

        private static KNKVOCore core;

        private ArrayList observations;

        public static KNKVOCore SharedCore() {

            if (core == null) {
                core = new KNKVOCore();
            }
            return core;
        }

        private KNKVOCore() {
            observations = new ArrayList();

        }

        // ---------------------

        public void AddObserverToKeyPathOfObject(Object observedObj, String keyPath, KNKVOObserver observer, long options, Object context) {
            KNKVOObservationInfo info = new KNKVOObservationInfo(keyPath, options, observer, observedObj, context);
            observations.Add(info);
        }

        public void RemoveObserverFromKeyPathOfObject(Object observedObj, String keyPath, KNKVOObserver observer) {
            ArrayList infoToRemove = new ArrayList();

            foreach (KNKVOObservationInfo info in observations) {
                if (info.KeyPath.Equals(keyPath) && (info.ObservedObject == observedObj) && (info.Observer == observer)) {
                    infoToRemove.Add(info);
                }
            }

            foreach (KNKVOObservationInfo info in infoToRemove) {
                observations.Remove(info);
            }

        }

        // ---------------------

        public void ObjectWillChangeValueForKey(Object obj, String key) {

            foreach (KNKVOObservationInfo info in observations) {

                if ((info.ObservedObject == obj) && info.KeyPath == key) {
                    info.ValueWillChange();
                }
            }
        }

        public void ObjectDidChangeValueForKey(Object obj, String key) {

            ArrayList observationsAtBeginningOfOperation = new ArrayList(observations);

            foreach (KNKVOObservationInfo info in observationsAtBeginningOfOperation) {
                // Check that the observation hasn't been removed during some other 
                // observation invocation.
                if (observations.Contains(info)) {
                    if ((info.ObservedObject == obj) && info.KeyPath == key) {
                        info.ValueDidChange();
                    }
                }
            }
        }


    }
}

