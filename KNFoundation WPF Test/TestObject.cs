using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KNFoundation.KNKVC;
using System.ComponentModel;

    public class TestObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private String aKey;

        public String key
        {
            get { return aKey; }

            set
            {
                if (PropertyChanging != null) {
                    PropertyChanging(this, new PropertyChangingEventArgs("key"));
                }
                aKey = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("key"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;
    }
