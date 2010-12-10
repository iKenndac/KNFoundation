using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {
    public class KNOneWayBinding : KNKVOObserver {

        public static void BindKeyPathOfTargetToKeyPathOfSource(
            string targetKeyPath,
            object target,
            string sourceKeyPath, 
            object source, 
            object nullValuePlaceholder
            ) {

                bindings.Add(new KNOneWayBinding(targetKeyPath, target, sourceKeyPath, source, nullValuePlaceholder));
        }

        public static void UnbindKeyPathOfTargetFromKeyPathOfSource(
            string targetKeyPath,
            object target,
            string sourceKeyPath, 
            object source
            ) {

                List<KNOneWayBinding> someBindings = new List<KNOneWayBinding>(bindings);

                foreach (KNOneWayBinding binding in someBindings) {
                    if (binding.TargetKeyPath.Equals(targetKeyPath) &&
                        binding.Target == target &&
                        binding.SourceKeyPath.Equals(sourceKeyPath) &&
                        binding.Source == source) {
                            bindings.Remove(binding);
                    }
                }
        }

        private static List<KNOneWayBinding> bindings = new List<KNOneWayBinding>();
        private const string kKNOneWayBindingInternalObservationContext = "kKNOneWayBindingInternalObservationContext";

        private KNOneWayBinding(
            string targetKeyPath,
            object target,
            string sourceKeyPath, 
            object source, 
            object nullValuePlaceholder
            ) {

                TargetKeyPath = targetKeyPath;
                Target = target;
                SourceKeyPath = sourceKeyPath;
                Source = source;
                NullPlaceHolder = nullValuePlaceholder;

                Source.AddObserverToKeyPathWithOptions(
                    this,
                    SourceKeyPath,
                    KNKeyValueObservingOptions.KNKeyValueObservingOptionInitial,
                    kKNOneWayBindingInternalObservationContext
                    );
        }

        ~KNOneWayBinding() {
            Source.RemoveObserverFromKeyPath(this, SourceKeyPath);
        }

        public void ObserveValueForKeyPathOfObject(string keyPath, object obj, Dictionary<string, object> change, object context) {
            if (keyPath.Equals(SourceKeyPath) && obj == Source && context == kKNOneWayBindingInternalObservationContext) {
                object value = Source.ValueForKeyPath(SourceKeyPath);
                if (value == null) {
                    value = NullPlaceHolder;
                }
                Target.SetValueForKeyPath(value, TargetKeyPath);
            } else {
                throw new Exception("Received unexpected KVO notification");
            }
        }

        private string TargetKeyPath { get; set; }
        private object Target { get; set; }
        private string SourceKeyPath { get; set; }
        private object Source { get; set; }
        private object NullPlaceHolder { get; set; }

    }
}
