using System.Collections.Generic;

namespace library {
    public class Circuit {
        public string Name;
        public List<Device> Devices;
        public List<PrimVSource> VSources;
        public List<PrimISource> ISources;
        public List<Node> Nodes;
        public Node Ground;

        public int N => this.Nodes.Count; // minus ground
        public int M => this.VSources.Count;

        public Circuit(string name) {
            this.Name = name;

            var gnd = new Node(-1);
            gnd.Name = "Ground";
            this.Ground = gnd;

            this.Devices = new List<Device>();
            this.VSources = new List<PrimVSource>();
            this.ISources = new List<PrimISource>();
            this.Nodes = new List<Node>();
        }

        public object Clone() {
            return this.MemberwiseClone();
        }

        public Node GenNode() {
            var res = new Node(this.N);
            this.Nodes.Add(res);
            return res;
        }

        public void AddComponent(Device device) {
            this.Devices.Add(device);
        }

        public void AddComponent(PrimVSource vSource) {
            this.VSources.Add(vSource);
        }

        public void AddComponent(PrimISource iSource) {
            this.ISources.Add(iSource);
        }
    }
}
