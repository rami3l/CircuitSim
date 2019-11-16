using System.Collections.Generic;

namespace library {
    public class Resistor : Device {
        double resistance;
        public Resistor(string rname, double r, Node a, Node b) {
            name = rname;
            resistance = r;
            pins = new List<Node>();
            pins.Add(a);
            pins.Add(b);
        }

        public override double Conductance() => 1 / this.resistance;
    }
}
