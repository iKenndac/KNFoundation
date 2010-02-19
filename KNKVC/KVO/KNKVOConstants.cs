using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNKVC
{
    public static class KNKVOConstants
    {

        
        public const long KNKeyValueObservingOptionNew = 0x1;
        public const long KNKeyValueObservingOptionOld = 0x2;
        public const long KNKeyValueObservingOptionInitial = 0x4; // Send notification immediately on registration.
        public const long KNKeyValueObservingOptionPrior = 0x8; // Send notification on willChange? (change contains KNKeyValueChangeNotificationIsPriorKey = YES)
        
        public const string KNKeyValueChangeKindKey = "KNKeyValueChangeKindKey";
        public const string KNKeyValueChangeNewKey = "KNKeyValueChangeNewKey";
        public const string KNKeyValueChangeOldKey = "KNKeyValueChangeOldKey";
        public const string KNKeyValueChangeIndexesKey = "KNKeyValueChangeIndexesKey";
        public const string KNKeyValueChangeNotificationIsPriorKey = "KNKeyValueChangeNotificationIsPriorKey";

    }
}
