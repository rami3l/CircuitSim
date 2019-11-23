using System.Collections.Generic;

namespace library {
    public class Resistor : Device {
        double resistance;
        public Resistor(string name, double resistance, Node a, Node b) {
            this.name = name;
            this.resistance = resistance;
            this.pins = new List<Node>();
            this.pins.Add(a);
            this.pins.Add(b);
        }

        public override double Conductance() => 1 / this.resistance;
    }
}
