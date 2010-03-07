using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KNFoundation.KNKVC;
using KNFoundation;


namespace KNKVC_Test 
{
    public partial class Form1 : Form, KNKVOObserver
    {
        private TestObject anObj = new TestObject();

        public TestObject myObj()
        {
            return anObj;
        }

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            myObj().AddObserverToKeyPathWithOptions(this, "key", KNKVOConstants.KNKeyValueObservingOptionNew | KNKVOConstants.KNKeyValueObservingOptionOld, null);
        }

        public void ObserveValueForKeyPathOfObject(String keyPath, Object obj, Dictionary<String, Object> change, Object context)
        {
            String message = "KVO change notification received" + Environment.NewLine + Environment.NewLine;

            if (change.ValueForKey(KNKVOConstants.KNKeyValueChangeNewKey) != null)
            {
                message += "New: " + change.ValueForKey(KNKVOConstants.KNKeyValueChangeNewKey).ToString() + Environment.NewLine;
            }
            if (change.ValueForKey(KNKVOConstants.KNKeyValueChangeOldKey) != null)
            {
                message += "Old: " + change.ValueForKey(KNKVOConstants.KNKeyValueChangeOldKey).ToString() + Environment.NewLine;
            }


            MessageBox.Show(message);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            myObj().RemoveObserverFromKeyPath(this, "key");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            myObj().key = "test";
        }

        private void button6_Click(object sender, EventArgs e) {
            KNNotificationCentre.SharedCentre().AddObserverForNotificationName(new KNNotificationDelegate(SomethingHappened), "MyAwesomeNotification");
        }

        private void button4_Click(object sender, EventArgs e) {
            KNNotificationCentre.SharedCentre().RemoveObserver(this);
        }

        private void button5_Click(object sender, EventArgs e) {
            KNNotificationCentre.SharedCentre().PostNotificationWithName("MyAwesomeNotification", this);
        }


        private void SomethingHappened(KNNotification notification) {
            MessageBox.Show(notification.Name);
        }


    }
}
