using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using KNFoundation.KNKVC;

namespace KNFoundation {

    public delegate void KNNotificationDelegate(KNNotification notification);
    
    public class KNNotificationCentre {


        private static KNNotificationCentre sharedCentre;

        public static KNNotificationCentre SharedCentre() {
            if (sharedCentre == null) {
                sharedCentre = new KNNotificationCentre();
            }
            return sharedCentre;
        }
        
        // This is a dictionary of Notification names to delegates interested 
    // in that notification. 

    private Dictionary<string, ArrayList> delegates;

        public KNNotificationCentre() {
            delegates = new Dictionary<string,ArrayList>();
        }

        public void AddObserverForNotificationName(KNNotificationDelegate del, string notificationName) {

            if (!delegates.ContainsKey(notificationName)) {
                delegates.Add(notificationName, new ArrayList());;
            }
            if (!((ArrayList)delegates.ValueForKey(notificationName)).Contains(del)) {
                ((ArrayList)delegates.ValueForKey(notificationName)).Add(del);
            }

        }

        public void RemoveNotificationDelegate(KNNotificationDelegate del) {

            foreach (ArrayList listOfDelegates in delegates.Values) {
                if (listOfDelegates.Contains(del)) {
                    listOfDelegates.Remove(del);
                }
            }
        }

        public void RemoveObserver(object observer) {
            ArrayList itemsToRemove = new ArrayList();

            foreach (ArrayList listOfDelegates in delegates.Values) {
                foreach(KNNotificationDelegate notificationDel in listOfDelegates) {
                    if (Object.ReferenceEquals(notificationDel.Target, observer)) {
                        itemsToRemove.Add(notificationDel);
                    }
                }

                foreach (KNNotificationDelegate removalDel in itemsToRemove) {
                    listOfDelegates.Remove(removalDel);
                }

                itemsToRemove.Clear();
            }
        }

        public void PostNotification(KNNotification notification) {

            if (delegates.ContainsKey(notification.Name)) {

                ArrayList listOfDelegates = (ArrayList)delegates.ValueForKey(notification.Name);

                foreach (KNNotificationDelegate del in listOfDelegates) {
                    if (del != null && del.Target != null) {
                        del.Invoke(notification);
                    }
                }
            }
        }

        public void PostNotificationWithName(string name, object sender) {
            PostNotification(new KNNotification(name, sender, null));
        }

        public void PostNotificationWithNameAndProperties(string name, object sender, Dictionary<string, object> properties) {
            PostNotification(new KNNotification(name, sender, properties));
        }
    }
}
