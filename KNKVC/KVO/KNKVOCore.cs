using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace KNKVC {

class KNKVOCore
{
    // Constants, etc

    private static KNKVOCore core;

    private ArrayList observations;

    public static KNKVOCore sharedCore() {

        if (core == null) {
            core = new KNKVOCore();
        }
        return core;
    }

    private KNKVOCore() {
        observations = new ArrayList();

    }

    // ---------------------

    public void addObserverToKeyPathOfObject(Object observedObj, String keyPath, KNKVOObserver observer, long options, Object context)
    {
        KNKVOObservationInfo info = new KNKVOObservationInfo(keyPath, options, observer, observedObj, context);
        observations.Add(info);
    }

    public void removeObserverFromKeyPathOfObject(Object observedObj, String keyPath, KNKVOObserver observer)
    {
        ArrayList infoToRemove = new ArrayList();

        foreach (KNKVOObservationInfo info in observations) {
            if (info.keyPath.Equals(keyPath) && (info.observedObject == observedObj) && (info.observer == observer))
            {
                infoToRemove.Add(info);
            }
        }

        foreach (KNKVOObservationInfo info in infoToRemove)
        {
            observations.Remove(info);
        }

    }

    // ---------------------

    public void objectWillChangeValueForKey(Object obj, String key) {

        foreach (KNKVOObservationInfo info in observations) {

            if ((info.observedObject == obj) && info.keyPath == key)
            {
                info.valueWillChange();
            }
        }
    }

    public void objectDidChangeValueForKey(Object obj, String key)
    {

        foreach (KNKVOObservationInfo info in observations)
        {

            if ((info.observedObject == obj) && info.keyPath == key)
            {
                info.valueDidChange();
            }
        }
    }


   }   
}

