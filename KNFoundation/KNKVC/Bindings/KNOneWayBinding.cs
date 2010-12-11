using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {

    public enum KNValueTransformer {
        None,
        KNNegateBoolean,
        KNIsNull,
        KNIsNotNull
    }

    public class KNOneWayBinding : KNKVOObserver {

        public static void BindKeyPathOfTargetToKeyPathOfSource(
            string targetKeyPath,
            object target,
            string sourceKeyPath,
            object source,
            object nullValuePlaceholder
            ) {
                bindings.Add(new KNOneWayBinding(targetKeyPath, target, sourceKeyPath, source, nullValuePlaceholder, KNValueTransformer.None));
        }

        public static void BindKeyPathOfTargetToKeyPathOfSource(
            string targetKeyPath,
            object target,
            string sourceKeyPath, 
            object source, 
            KNValueTransformer transformer
            ) {

                bindings.Add(new KNOneWayBinding(targetKeyPath, target, sourceKeyPath, source, null, transformer));
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
            object nullValuePlaceholder,
            KNValueTransformer transformer
            ) {

                TargetKeyPath = targetKeyPath;
                Target = target;
                SourceKeyPath = sourceKeyPath;
                Source = source;
                NullPlaceHolder = nullValuePlaceholder;
                ValueTransformer = transformer;

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

                if (ValueTransformer == KNValueTransformer.KNIsNotNull) {
                    value = (Boolean)(value != null);
                } else if (ValueTransformer == KNValueTransformer.KNIsNull) {
                    value = (Boolean)(value == null);
                } else if (ValueTransformer == KNValueTransformer.KNNegateBoolean) {

                    Boolean aBool;
                    if (value == null) {
                        aBool = false;
                    } else {
                        aBool = !(Boolean)value;
                    }
                    value = aBool;
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
        private KNValueTransformer ValueTransformer { get; set; }

    }
}
