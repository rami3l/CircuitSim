using System;

namespace library {
    public class ISource : SourcePrototype {
        public double Current => this.Value;

        public ISource(string name, double current, Node positive, Node negative) : base(name, current, positive, negative) {
            // The default working mode is Constant.
        }
    }
}