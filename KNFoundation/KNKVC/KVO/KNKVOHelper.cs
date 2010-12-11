using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {

    public abstract class KNKVOHelper {

        public abstract KNKVOHelper CopyForNewObject(object aNewObject);

        int retainCount = 0;

        public void Retain() {
            retainCount++;
        }

        public void Release() {
            retainCount--;
        }

        public int RetainCount() {
            return retainCount;
        }

    }
}
