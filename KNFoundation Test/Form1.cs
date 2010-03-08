using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
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


        Dictionary<string, object> plist = new Dictionary<string, object>();

        public Form1()
        {
            InitializeComponent();


            plist.Add("Name", "Daniel");
            plist.Add("Age", 12);
            plist.Add("Height", 6.2);
            plist.Add("Birthday", new DateTime(1985, 04, 15));
            plist.Add("Awesome?", true);
            plist.Add("Data", new byte[] { 0, 0, 0, 0 });

            ArrayList favouriteThings = new ArrayList();
            favouriteThings.Add("Trains");
            favouriteThings.Add("Cars");
            favouriteThings.Add("Snowboarding");
            favouriteThings.Add("Cheeseburgers");

            plist.Add("A Few Of My Favourite Things", favouriteThings);

            Dictionary<string, object> colors = new Dictionary<string, object>();
            colors.Add("Trousers", "Blue");
            colors.Add("Jumper", "Red");
            colors.Add("Socks", "Black");

            plist.Add("Colors", colors);

            MessageBox.Show(KNBundle.MainBundle().ShortVersionString + Environment.NewLine + KNBundle.MainBundle().BundleIdentifier + Environment.NewLine + KNBundle.MainBundle().ExecutablePath);

            Dictionary<string, object> defaults = new Dictionary<string, object>();
            defaults.Add("testKey", "Default value");

            KNUserDefaults.StandardUserDefaults().Defaults = defaults;


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

        private void button7_Click(object sender, EventArgs e) {

            string plistStr = Encoding.UTF8.GetString(KNPropertyListSerialization.DataWithPropertyList(plist));

            Clipboard.SetText(plistStr);

            MessageBox.Show(plistStr);


        }

        private void button8_Click(object sender, EventArgs e) {

            Dictionary<string, object> prefs = KNPropertyListSerialization.PropertyListWithData(KNPropertyListSerialization.DataWithPropertyList(plist));

            MessageBox.Show(prefs.Keys.Count.ToString());

        }

        private void button10_Click(object sender, EventArgs e) {

            KNUserDefaults.StandardUserDefaults().SetStringForKey("Set Value", "testKey");
        }

        private void button9_Click(object sender, EventArgs e) {
            MessageBox.Show(KNUserDefaults.StandardUserDefaults().StringForKey("testKey"));
        }
    }
}
