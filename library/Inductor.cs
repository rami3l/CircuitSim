using System.Collections.Generic;

namespace library {
    public class Inductor : Device {
        double inductance;
        public Inductor(string lname, double l, Node a, Node b) {
            name = lname;
            inductance = l;
            pins = new List<Node>();
            pins.Add(a);
            pins.Add(b);
        }
    }
}