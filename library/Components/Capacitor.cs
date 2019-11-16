using System.Collections.Generic;

namespace library {
    public class Capacitor : Device {
        public double capacitance;
        public Capacitor(string cname, double c, Node a, Node b) {
            name = cname;
            capacitance = c;
            pins = new List<Node>();
            pins.Add(a);
            pins.Add(b);
        }
    }
}
