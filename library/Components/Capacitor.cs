using System.Collections.Generic;

namespace library {
    public class Capacitor : Device {
        public double capacitance;
        public Capacitor(string name, double capacitance, Node a, Node b) {
            this.name = name;
            this.capacitance = capacitance;
            this.pins = new List<Node>();
            this.pins.Add(a);
            this.pins.Add(b);
        }
    }
}
