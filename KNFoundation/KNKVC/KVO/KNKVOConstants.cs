using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {
    /// <summary>
    /// Defines options used when you observe a key path.
    /// </summary>
    [Flags]
    public enum KNKeyValueObservingOptions : long {
        KNKeyValueObservingOptionNone = 0x0,
        /// <summary>
        /// If <c>KNKeyValueObservingOptionNew</c> is specified, the change dictionary will contain the new value
        /// of the observed key path in the key <c>KNKeyValueChangeNewKey</c> This may be <c>null</c>.
        /// </summary>
        KNKeyValueObservingOptionNew = 0x1,
        /// <summary>
        /// If <c>KNKeyValueObservingOptionOld</c> is specified, the change dictionary will contain the new value
        /// of the observed key path in the key <c>KNKeyValueChangeOldKey</c>. This may be <c>null</c>.
        /// </summary>
        KNKeyValueObservingOptionOld = 0x2,
        /// <summary>
        /// If <c>KNKeyValueObservingOptionInitial</c> is specified, a KVO notification will be sent to the observer
        /// before <c>addObserverForKeyPath</c> returns. If <c>KNKeyValueObservingOptionNew</c> is specified, 
        /// the change dictionary will contain the value for the given key path in the key <c>KNKeyValueChangeNewKey</c>.
        /// </summary>
        KNKeyValueObservingOptionInitial = 0x4,
        /// <summary>
        /// If <c>KNKeyValueObservingOptionPrior</c> is specified, a KVO notification will be sent prior to the value changing as well
        /// as after. A change notification send prior to the value changing will contain the boolean <c>true</c> under the 
        /// <c>KNKeyValueChangeNotificationIsPriorKey</c> key, and the current value under the <c>KNKeyValueChangeOldKey</c> if
        /// <c>KNKeyValueObservingOptionOld</c> is specified. Prior notifications never contain the <c>KNKeyValueChangeNewKey</c>
        /// key.
        /// </summary>
        KNKeyValueObservingOptionPrior = 0x8
    }

    /// <summary>
    /// Defines constants used in KNKVC.
    /// </summary>
    public static class KNKVOConstants {
        /// <summary>
        /// Unused.
        /// </summary>
        public const string KNKeyValueChangeKindKey = "KNKeyValueChangeKindKey";
        /// <summary>
        /// KVO notification change dictionaries contain the new value under this key if KNKeyValueObservingOptionNew was specified
        /// when creating the observation.
        /// </summary>
        public const string KNKeyValueChangeNewKey = "KNKeyValueChangeNewKey";
        /// <summary>
        /// KVO notification change dictionaries contain the old value under this key if KNKeyValueObservingOptionOld was specified
        /// when creating the observation.
        /// </summary>
        public const string KNKeyValueChangeOldKey = "KNKeyValueChangeOldKey";
        /// <summary>
        /// Unused.
        /// </summary>
        public const string KNKeyValueChangeIndexesKey = "KNKeyValueChangeIndexesKey";
        /// <summary>
        /// KVO notification change dictionaries contain <c>true</c> under this key if KNKeyValueObservingOptionPrior was specified
        /// when creating the observation and the current change notification is prior to the value changing. If the 
        /// notification isn't a prior notification, this key isn't present.
        /// </summary>
        public const string KNKeyValueChangeNotificationIsPriorKey = "KNKeyValueChangeNotificationIsPriorKey";
    }

}

