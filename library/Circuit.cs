using System.Linq;
using System.Collections.Generic;

namespace library {
    public class Circuit {
        public string Name;
        public List<Device> Devices;
        public List<VSource> VSources;
        public List<ISource> ISources;
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
            this.VSources = new List<VSource>();
            this.ISources = new List<ISource>();
            this.Nodes = new List<Node>();
        }

        public object Clone() {
            return this.MemberwiseClone();
        }

        public Node GenNode(string name = "") {
            var res = new Node(this.N, name);
            this.Nodes.Add(res);
            return res;
        }

        public void AddComponent(Device device) {
            this.Devices.Add(device);
        }

        public void AddComponent(VSource vSource) {
            this.VSources.Add(vSource);
        }

        public void AddComponent(ISource iSource) {
            this.ISources.Add(iSource);
        }
    }
}
