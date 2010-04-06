using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {
    internal class KNKVOObservationChangeTracker {

        public KNKVOObservationChangeTracker(Object anOldValue, String aKeyPath) {
            OldValue = anOldValue;
            KeyPath = aKeyPath;
        }

        public Object OldValue {
            get;
            private set;
        }

        public String KeyPath {
            get;
            private set;
        }

    }
}
