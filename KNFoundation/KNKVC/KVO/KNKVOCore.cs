using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;

namespace KNFoundation.KNKVC {

    class KNKVOCore {
        private static KNKVOCore core;

        private ArrayList internalObservations;
        private ArrayList keyPathObservations;

        public static KNKVOCore SharedCore() {

            if (core == null) {
                core = new KNKVOCore();
            }
            return core;
        }

        private KNKVOCore() {
            internalObservations = new ArrayList();
            keyPathObservations = new ArrayList();
        }

        // ---------------------

        /// <summary>
        /// Creates an observation manager for the given key path. 
        /// </summary>
        /// <param name="observedObj">The object to observe.</param>
        /// <param name="keyPath">The key path to observe.</param>
        /// <param name="observer">The observer.</param>
        /// <param name="options">Bitwise-Or of the desired observation objects.</param>
        /// <param name="context">The context of the observation. Used for comparison only.</param>
        public void AddObserverToKeyPathOfObject(Object observedObj, String keyPath, KNKVOObserver observer, KNKeyValueObservingOptions options, Object context) {
            if (keyPath.Contains('.')) {

                KNKVOKeyPathObserver observerProxy = new KNKVOKeyPathObserver(observedObj, keyPath, observer, options, context);
                keyPathObservations.Add(observerProxy);

            } else {

                KNKVOObservationInfo info = new KNKVOObservationInfo(keyPath, options, observer, observedObj, context);
                internalObservations.Add(info);
            }
        }

        /// <summary>
        /// Removes an observer from the given key path of an object.
        /// </summary>
        /// <param name="observedObj">The observed object.</param>
        /// <param name="keyPath">The key path to remove the observed from.</param>
        /// <param name="observer">The observer to remove.</param>
        public void RemoveObserverFromKeyPathOfObject(Object observedObj, String keyPath, KNKVOObserver observer) {

            if (keyPath.Contains('.')) {
                ArrayList proxiesToRemove = new ArrayList();

                foreach (KNKVOKeyPathObserver observerProxy in keyPathObservations) {
                    if ((observerProxy.observedObject == observedObj) && (observerProxy.keyPath == keyPath) && (observerProxy.observer == observer)) {
                        proxiesToRemove.Add(observerProxy);
                    }
                }

                foreach (KNKVOKeyPathObserver observerProxy in proxiesToRemove) {
                    keyPathObservations.Remove(observerProxy);
                    observerProxy.Dispose();
                }
            } else {

                ArrayList infoToRemove = new ArrayList();

                foreach (KNKVOObservationInfo info in internalObservations) {
                    if (info.key.Equals(keyPath) && (info.observedObject == observedObj) && (info.observer == observer)) {
                        infoToRemove.Add(info);
                    }
                }

                foreach (KNKVOObservationInfo info in infoToRemove) {
                    info.Dispose();
                    internalObservations.Remove(info);
                }

            }

        }

        // ---------------------

        public void ObjectWillChangeValueForKey(Object obj, String key) {

            ArrayList currentInternalObservations = new ArrayList(internalObservations);

            foreach (KNKVOObservationInfo info in currentInternalObservations) {

                if ((info.observedObject == obj) && info.key == key) {
                    info.valueWillChange();
                }
            }
        }

        public void ObjectDidChangeValueForKey(Object obj, String key) {
            ArrayList currentInternalObservations = new ArrayList(internalObservations);

            foreach (KNKVOObservationInfo info in currentInternalObservations) {

                if ((info.observedObject == obj) && info.key == key) {
                    info.valueDidChange();
                }
            }
        }
    }

}

