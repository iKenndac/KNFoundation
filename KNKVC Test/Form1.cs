using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KNKVC;


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
            myObj().addObserverToKeyPathWithOptions(this, "key", KNKVOConstants.KNKeyValueObservingOptionNew | KNKVOConstants.KNKeyValueObservingOptionOld, null);
        }

        public void observeValueForKeyPathOfObject(String keyPath, Object obj, Dictionary<String, Object> change, Object context)
        {
            String message = "KVO change notification received" + Environment.NewLine + Environment.NewLine;

            if (change.valueForKey(KNKVOConstants.KNKeyValueChangeNewKey) != null)
            {
                message += "New: " + change.valueForKey(KNKVOConstants.KNKeyValueChangeNewKey).ToString() + Environment.NewLine;
            }
            if (change.valueForKey(KNKVOConstants.KNKeyValueChangeOldKey) != null)
            {
                message += "Old: " + change.valueForKey(KNKVOConstants.KNKeyValueChangeOldKey).ToString() + Environment.NewLine;
            }


            MessageBox.Show(message);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            myObj().removeObserverFromKeyPath(this, "key");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            myObj().key = "test";
        }

    }
}
