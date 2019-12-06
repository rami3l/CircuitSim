using System;

namespace library {
    public class Node : IEquatable<Node> {
        public string Name;

        public int ID;

        public Node(int ID, string name = "") {
            this.ID = ID; // -1 for ground, >=0 for others
            if (name == "") {
                this.Name = $"Node {ID}";
            } else {
                this.Name = name;
            }
        }

        public bool Equals(Node other) {
            return this.ID.Equals(other.ID);
        }
    }
}
