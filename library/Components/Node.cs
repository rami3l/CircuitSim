using System;

namespace library {
    public struct Node : IEquatable<Node> {
        private string name;
        public string Name {
            get => name;
            set => name = value;
        }

        private int id;
        public int ID {
            get => id;
            set => id = value;
        }

        public Node(int ID) {
            this.id = ID; // -1 for ground, >=0 for others
            name = $"Node {ID}";
        }

        public bool Equals(Node other) {
            return this.ID.Equals(other.ID);
        }
    }
}
