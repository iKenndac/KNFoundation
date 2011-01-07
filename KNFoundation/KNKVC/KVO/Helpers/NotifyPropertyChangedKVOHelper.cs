using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace KNFoundation.KNKVC.KVO.Helpers {
    class NotifyPropertyChangedKVOHelper : KNKVOHelper {

        public NotifyPropertyChangedKVOHelper() {}

        NotifyPropertyChangedKVOHelper(INotifyPropertyChanged anObject) {
            TargetObject = anObject;
            TargetObject.PropertyChanged += PropertyDidChange;

            if (typeof(INotifyPropertyChanging).IsAssignableFrom(anObject.GetType())) {
                targetSupportsPropertyChanging = true;
                ((INotifyPropertyChanging)anObject).PropertyChanging += PropertyWillChange;
            }
        }

        ~NotifyPropertyChangedKVOHelper() {
            if (TargetObject != null) {
                TargetObject.PropertyChanged -= PropertyDidChange;

                if (typeof(INotifyPropertyChanging).IsAssignableFrom(TargetObject.GetType())) {
                    ((INotifyPropertyChanging)TargetObject).PropertyChanging -= PropertyWillChange;
                }
            }
        }

        private void PropertyDidChange(object sender, PropertyChangedEventArgs e) {

            if (!targetSupportsPropertyChanging) {
                TargetObject.WillChangeValueForKey(e.PropertyName);
            }

            TargetObject.DidChangeValueForKey(e.PropertyName);
        }

        private void PropertyWillChange(object sender, PropertyChangingEventArgs e) {
            TargetObject.WillChangeValueForKey(e.PropertyName);
        }

        private INotifyPropertyChanged TargetObject { get; set; }
        private Boolean targetSupportsPropertyChanging;

        public override KNKVOHelper CopyForNewObject(object aNewObject) {
            return new NotifyPropertyChangedKVOHelper((INotifyPropertyChanged)aNewObject);
        }

    }
}
