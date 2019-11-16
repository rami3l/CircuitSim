using System.Collections.Generic;

namespace library {
    public class Device : Component {
        public List<Node> pins;
        // ! a device can be asymmetric, there should be a way of distincting pins

        public virtual double Conductance() => 0;
    }
}
