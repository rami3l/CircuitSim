using System;

namespace library {
    public struct Node : IEquatable<Node> {
        public string Name;

        public int ID;

        public Node(int ID) {
            this.ID = ID; // -1 for ground, >=0 for others
            this.Name = $"Node {ID}";
        }

        public bool Equals(Node other) {
            return this.ID.Equals(other.ID);
        }
    }
}
