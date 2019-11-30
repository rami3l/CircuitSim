using System.Collections.Generic;

namespace library {
    public class Capacitor : Device {
        public double Capacitance;
        public Capacitor(string name, double capacitance, Node a, Node b) {
            this.Name = name;
            this.Capacitance = capacitance;
            this.Pins = new List<Node>();
            this.Pins.Add(a);
            this.Pins.Add(b);
        }
    }
}
