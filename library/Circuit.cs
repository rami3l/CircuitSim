using System.Collections.Generic;

namespace library {
    public class Circuit {
        public string name;
        public List<Device> devices;
        public List<PrimVSource> vSources;
        public List<PrimISource> iSources;
        public List<Node> nodes;
        public Node ground;

        public int N => this.nodes.Count; // minus ground
        public int M => this.vSources.Count;

        public Circuit(string name) {
            this.name = name;

            var gnd = new Node(-1);
            gnd.Name = "Ground";
            this.ground = gnd;

            this.devices = new List<Device>();
            this.vSources = new List<PrimVSource>();
            this.iSources = new List<PrimISource>();
            this.nodes = new List<Node>();
        }

        public object Clone() {
            return this.MemberwiseClone();
        }

        public Node GenNode() {
            var res = new Node(this.N);
            this.nodes.Add(res);
            return res;
        }

        public void AddComponent(Device device) {
            this.devices.Add(device);
        }

        public void AddComponent(PrimVSource vSource) {
            this.vSources.Add(vSource);
        }

        public void AddComponent(PrimISource iSource) {
            this.iSources.Add(iSource);
        }
    }
}
