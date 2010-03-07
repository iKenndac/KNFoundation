using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNFoundation.KNKVC {
    class KNKVCMethodInvokeFailedException : Exception {

        public KNKVCMethodInvokeFailedException(Exception innerException)
            : base("Method invoke failed", innerException) {

        }
    }
}
