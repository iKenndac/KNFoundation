using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNKVC
{
    public interface KNKVOObserver
    {
        void observeValueForKeyPathOfObject(String keyPath, Object obj, Dictionary<String, Object> change, Object context);
    }
}
