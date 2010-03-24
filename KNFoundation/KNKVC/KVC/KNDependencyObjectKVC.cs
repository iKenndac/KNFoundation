using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Threading;

namespace KNFoundation.KNKVC {
    static class KNDependencyObjectKVC {

        public static Object ValueForKey(this DependencyObject o, string key) {

            if (o.Dispatcher.Thread == Thread.CurrentThread) {
                return LogicalTreeHelper.FindLogicalNode(o, key);
            } else {
                return o.Dispatcher.Invoke(new FindNodeDelegate(FindNode), new object[] {o, key});
            }
        }

        private delegate Object FindNodeDelegate(DependencyObject o, string key);

        private static Object FindNode(DependencyObject o, string key) {
            return LogicalTreeHelper.FindLogicalNode(o, key);
        }
    }
}
