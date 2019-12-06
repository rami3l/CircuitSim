using System;

namespace library {
    public class VSource : SourcePrototype {
        public double Voltage => this.Value;

        public VSource(string name, double voltage, Node positive, Node negative) : base(name, voltage, positive, negative) {
            // The default working mode is Constant.
        }
    }
}
