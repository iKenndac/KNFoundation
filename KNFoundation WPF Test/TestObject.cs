using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KNFoundation.KNKVC;

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
