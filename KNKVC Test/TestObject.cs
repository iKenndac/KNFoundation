using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KNKVC;

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
                this.willChangeValueForKey("key");
                aKey = value;
                this.didChangeValueForKey("key");
            }
        }
    }
}
