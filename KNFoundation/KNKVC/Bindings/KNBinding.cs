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

    public class KNBinding : KNKVOObserver {

        public static void BindKeyPathOfTargetToKeyPathOfSource(
            string targetKeyPath,
            object target,
            string sourceKeyPath,
            object source
            ) {
            bindings.Add(new KNBinding(targetKeyPath, target, sourceKeyPath, source, null, KNValueTransformer.None));
        }

        public static void BindKeyPathOfTargetToKeyPathOfSource(
            string targetKeyPath,
            object target,
            string sourceKeyPath,
            object source,
            object nullValuePlaceholder
            ) {
                bindings.Add(new KNBinding(targetKeyPath, target, sourceKeyPath, source, nullValuePlaceholder, KNValueTransformer.None));
        }

        public static void BindKeyPathOfTargetToKeyPathOfSource(
            string targetKeyPath,
            object target,
            string sourceKeyPath, 
            object source, 
            KNValueTransformer transformer
            ) {

                bindings.Add(new KNBinding(targetKeyPath, target, sourceKeyPath, source, null, transformer));
        }

        public static void UnbindKeyPathOfTargetFromKeyPathOfSource(
            string targetKeyPath,
            object target,
            string sourceKeyPath, 
            object source
            ) {

                List<KNBinding> someBindings = new List<KNBinding>(bindings);

                foreach (KNBinding binding in someBindings) {
                    if (binding.TargetKeyPath.Equals(targetKeyPath) &&
                        binding.Target == target &&
                        binding.SourceKeyPath.Equals(sourceKeyPath) &&
                        binding.Source == source) {
                            bindings.Remove(binding);
                    }
                }
        }

        private static List<KNBinding> bindings = new List<KNBinding>();
        private const string kKNBindingInternalObservationContext = "kKNBindingInternalObservationContext";

        private KNBinding(
            string targetKeyPath,
            object target,
            string sourceKeyPath, 
            object source, 
            object nullValuePlaceholder,
            KNValueTransformer transformer
            ) {

                ignoreSourceKVO = false;
                ignoreTargetKVO = false;

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
                    kKNBindingInternalObservationContext
                    );

                Target.AddObserverToKeyPathWithOptions(
                    this,
                    TargetKeyPath,
                    KNKeyValueObservingOptions.KNKeyValueObservingOptionNone,
                    kKNBindingInternalObservationContext
                    );
        }

        ~KNBinding() {
            Source.RemoveObserverFromKeyPath(this, SourceKeyPath);
            Target.RemoveObserverFromKeyPath(this, TargetKeyPath);
        }

        public void ObserveValueForKeyPathOfObject(string keyPath, object obj, Dictionary<string, object> change, object context) {

            if (keyPath.Equals(SourceKeyPath) && obj == Source && context == kKNBindingInternalObservationContext) {

                if (!ignoreSourceKVO) {
                    // Source Changed
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

                    ignoreTargetKVO = true;
                    // ^ Faster than removing observer and re-adding
                    try {
                        Target.SetValueForKeyPath(value, TargetKeyPath);
                    } catch (Exception) { }
                    ignoreTargetKVO = false;
                }
            } else if (keyPath.Equals(TargetKeyPath) & obj == Target && context == kKNBindingInternalObservationContext) {

                if (!ignoreTargetKVO) {
                    // Target changed

                    object value = Target.ValueForKeyPath(TargetKeyPath);
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

                    ignoreSourceKVO = true;
                    // ^ Faster than removing observer and re-adding
                    try {
                        Source.SetValueForKeyPath(value, SourceKeyPath);
                    } catch (Exception) { }
                    ignoreSourceKVO = false;
                }

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

        private Boolean ignoreTargetKVO;
        private Boolean ignoreSourceKVO;

    }
}
