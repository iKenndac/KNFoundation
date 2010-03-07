using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNFoundation {
    public class KNNotification {


        private string notificationName;
        private object notificationSender;
        private Dictionary<string, object> notificationProperties;

        public KNNotification(string name, object sender, Dictionary<string, object> properties) {
            notificationName = name;
            if (properties != null) {
                notificationProperties = properties;
            } else {
                notificationProperties = new Dictionary<string,object>();
            }

        }

        public string Name {
            get { return notificationName; }
        }

        public object Sender {
            get { return notificationSender; }
        }

        public Dictionary<string, object> Properties {
            get { return notificationProperties; }
        }
    }
}
