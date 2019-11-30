using System.Collections.Generic;

namespace library {
    public class Inductor : Device {
        public double Inductance;
        public Inductor(string name, double inductance, Node a, Node b) {
            this.Name = name;
            this.Inductance = inductance;
            this.Pins = new List<Node>();
            this.Pins.Add(a);
            this.Pins.Add(b);
        }
    }
}