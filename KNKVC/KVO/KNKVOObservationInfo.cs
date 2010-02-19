using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace KNKVC
{
    public class KNKVOObservationInfo
    {
        private Stack<KNKVOObservationChangeTracker> changes;

        public KNKVOObservationInfo(String aKeyPath, long someOptions, KNKVOObserver anObserver, Object anObservedObject, Object aContext)
        {
            changes = new Stack<KNKVOObservationChangeTracker>();
            observedObjectReference = new WeakReference(anObservedObject);
            options = someOptions;
            observer = anObserver;
            keyPath = aKeyPath;
            context = aContext;

            if ((options & KNKVOConstants.KNKeyValueObservingOptionInitial) == KNKVOConstants.KNKeyValueObservingOptionInitial)
            {
                Dictionary<String, Object> change = new Dictionary<String, Object>();

                if ((options & KNKVOConstants.KNKeyValueObservingOptionNew) == KNKVOConstants.KNKeyValueObservingOptionNew)
                {
                    change.setValueForKey(observedObject.valueForKey(keyPath), KNKVOConstants.KNKeyValueChangeNewKey);
                }

                observer.observeValueForKeyPathOfObject(keyPath, observedObject, change, context);
            }
        }

        public Object context { get; private set; }
        public String keyPath { get; private set; }
        public KNKVOObserver observer { get; private set; }
        public long options { get; private set; }
        private WeakReference observedObjectReference { get;  set; }

        public Object observedObject {
            get
            {
                if (observedObjectReference == null)
                {
                    return null;
                }
                else
                {
                    return observedObjectReference.Target;
                }
            }
        }

        // 0----------------------------------

        public void valueWillChange()
        {

            Object oldValue = observedObject.valueForKey(keyPath);
            changes.Push(new KNKVOObservationChangeTracker(oldValue));

            if ((options & KNKVOConstants.KNKeyValueObservingOptionPrior) == KNKVOConstants.KNKeyValueObservingOptionPrior)
            {
                Dictionary<String, Object> change = new Dictionary<String, Object>();
                change.setValueForKey(true, KNKVOConstants.KNKeyValueChangeNotificationIsPriorKey);
                change.setValueForKey(oldValue, keyPath);
            }

        }

        public void valueDidChange()
        {
            Object newValue = observedObject.valueForKey(keyPath);
            KNKVOObservationChangeTracker tracker = changes.Pop();
            Object oldValue = tracker.oldValue;

            Dictionary<String, Object> change = new Dictionary<String, Object>();

            if ((options & KNKVOConstants.KNKeyValueObservingOptionNew) == KNKVOConstants.KNKeyValueObservingOptionNew)
            {
                change.setValueForKey(newValue, KNKVOConstants.KNKeyValueChangeNewKey);
            }

            if ((options & KNKVOConstants.KNKeyValueObservingOptionOld) == KNKVOConstants.KNKeyValueObservingOptionOld)
            {
                change.setValueForKey(oldValue, KNKVOConstants.KNKeyValueChangeOldKey);
            }

            observer.observeValueForKeyPathOfObject(keyPath, observedObject, change, context);
        }


        private class KNKVOObservationChangeTracker
        {

            public KNKVOObservationChangeTracker(Object anOldValue)
            {
                oldValue = anOldValue;
            }

            public Object oldValue
            {
                get;
                set;
            }

        }

    }

    
}
