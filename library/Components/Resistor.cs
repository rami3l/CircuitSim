using System.Collections.Generic;

namespace library {
    public class Resistor : Device {
        double Resistance;
        public Resistor(string name, double resistance, Node a, Node b) {
            this.Name = name;
            this.Resistance = resistance;
            this.Pins = new List<Node>();
            this.Pins.Add(a);
            this.Pins.Add(b);
        }

        public override double Conductance() => 1 / this.Resistance;
    }
}
