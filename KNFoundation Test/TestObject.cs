using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KNFoundation.KNKVC;

namespace KNKVC_Test
{
    public class TestObject
    {
        private String aKey;

        public String key
        {
            get { return aKey; }

            set
            {
                this.WillChangeValueForKey("key");
                aKey = value;
                this.DidChangeValueForKey("key");
            }
        }
    }
}
