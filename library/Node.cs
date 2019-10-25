using System;

namespace library {
    public struct Node : IEquatable<Node> {
        private string name;
        public string Name {
            get => name;
            set => name = value;
        }

        private int id;
        public int Id {
            get => id;
            set => id = value;
        }

        public Node(int nodeId) {
            id = nodeId; // -1 for ground, >=0 for others
            name = $"Node {nodeId}";
        }

        public bool Equals(Node other) {
            return this.Id.Equals(other.Id);
        }
    }
}
