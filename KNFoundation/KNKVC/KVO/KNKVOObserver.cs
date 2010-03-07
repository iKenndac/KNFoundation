using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {
    public interface KNKVOObserver {
        void ObserveValueForKeyPathOfObject(String keyPath, Object obj, Dictionary<String, Object> change, Object context);
    }
}
