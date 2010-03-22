using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace KNFoundation.KNKVC {
    static class KNDependencyObjectKVC {

        public static Object ValueForKey(this DependencyObject o, String key) {
            return LogicalTreeHelper.FindLogicalNode(o, key);
        }
    }
}
