using System.Collections.Generic;

namespace library {
    public class Circuit {
        string title;
        public List<Device> devices;
        public List<VSource> vSources;
        public List<ISource> iSources;
        public List<Node> nodes;
        public Node ground;

        public int N => this.nodes.Count; // minus ground
        public int M => this.vSources.Count;

        public Circuit(string name) {
            title = name;

            var gnd = new Node(-1);
            gnd.Name = "Ground";
            ground = gnd;

            devices = new List<Device>();
            vSources = new List<VSource>();
            iSources = new List<ISource>();
            nodes = new List<Node>();
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

        public void AddComponent(VSource vSource) {
            this.vSources.Add(vSource);
        }

        public void AddComponent(ISource iSource) {
            this.iSources.Add(iSource);
        }
    }
}
