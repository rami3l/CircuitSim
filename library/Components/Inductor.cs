using System.Collections.Generic;

namespace library {
    public class Inductor : Device {
        double inductance;
        public Inductor(string name, double inductance, Node a, Node b) {
            this.name = name;
            this.inductance = inductance;
            this.pins = new List<Node>();
            this.pins.Add(a);
            this.pins.Add(b);
        }
    }
}