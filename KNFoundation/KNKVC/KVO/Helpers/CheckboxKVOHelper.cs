using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KNFoundation.KNKVC.KVO.Helpers {
    class CheckboxKVOHelper : KNKVOHelper {

        public CheckboxKVOHelper() {}

        CheckboxKVOHelper(CheckBox aCheckBox) {
            TargetCheckBox = aCheckBox;
            TargetCheckBox.Checked += CheckBoxDidPerformAction;
            TargetCheckBox.Unchecked += CheckBoxDidPerformAction;
        }

        ~CheckboxKVOHelper() {
            if (TargetCheckBox != null) {
                TargetCheckBox.Checked -= CheckBoxDidPerformAction;
                TargetCheckBox.Unchecked -= CheckBoxDidPerformAction;
            }
        }

        private void CheckBoxDidPerformAction(object sender, EventArgs e) {
            TargetCheckBox.WillChangeValueForKey("IsChecked");
            TargetCheckBox.DidChangeValueForKey("IsChecked");
        }

        private CheckBox TargetCheckBox { get; set; }

        public override KNKVOHelper CopyForNewObject(object aNewObject) {
            return new CheckboxKVOHelper((CheckBox)aNewObject);
        }
    }
}
