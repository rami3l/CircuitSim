using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

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

    public class Component {
        public string name;
    }

    public class Device : Component {
        public List<Node> pins;
        // ! a device can be asymmetric, there should be a way of distincting pins

        public virtual double Conductance() => 0;
    }

    public class Resistor : Device {
        double resistance;
        public Resistor(string rname, double r, Node a, Node b) {
            name = rname;
            resistance = r;
            pins = new List<Node>();
            pins.Add(a);
            pins.Add(b);
        }

        public override double Conductance() => 1 / this.resistance;
    }

    public class VSource : Component {
        public double voltage;
        public Node positive;
        public Node negative;
        public VSource(string vsName, double v, Node pos, Node neg) {
            name = vsName;
            voltage = v;
            positive = pos;
            negative = neg;
        }
    }

    public class ISource : Component {
        public double current;
        public Node positive;
        public Node negative;
        public ISource(string isName, double i, Node pos, Node neg) {
            name = isName;
            current = i;
            positive = pos;
            negative = neg;
        }
    }

    public class Circuit {
        string title;
        List<Device> devices;
        List<VSource> vSources;
        List<ISource> iSources;
        List<Node> nodes;
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

        private static void AddIfNotGrounded(ref Matrix<double> mat, int i, int j, double num) {
            if (i != -1 && j != -1) {
                mat[i, j] += num;
            }
        }

        public Matrix<double> GenA() {
            /* The matrix A = {{G|B},{C|D}}  */
            var builder = Matrix<double>.Build;
            var n = this.N;
            var m = this.M;
            var size = n + m;
            var res = builder.Dense(size, size);
            var G = this.GenG();
            var B = this.GenB();
            var C = this.GenC();
            for (int i = 0; i < size; i++) {
                if (i < n) {
                    for (int j = 0; j < n; j++) {
                        res[i, j] = G[i, j];
                    }
                    for (int j = n; j < size; j++) {
                        res[i, j] = B[i, j - n];
                    }
                } else {
                    for (int j = 0; j < n; j++) {
                        res[i, j] = C[i - n, j];
                    }
                }
            }
            return res;
        }

        Matrix<double> GenG() {
            var builder = Matrix<double>.Build;
            var res = builder.Dense(this.N, this.N);
            foreach (var device in this.devices) {
                var g = device.Conductance();
                if (device.pins.Count != 2) {
                    throw new NotImplementedException();
                }
                int i = device.pins[0].Id;
                int j = device.pins[1].Id;

                AddIfNotGrounded(ref res, i, i, g);
                AddIfNotGrounded(ref res, j, j, g);
                AddIfNotGrounded(ref res, i, j, -g);
                AddIfNotGrounded(ref res, j, i, -g);
            }
            return res;
        }

        Matrix<double> GenB() {
            var builder = Matrix<double>.Build;
            var res = builder.Dense(this.N, this.M);
            foreach ((var vSource, var i) in this.vSources.Select((v, i) => (v, i))) {
                var p = vSource.positive.Id;
                var n = vSource.negative.Id;

                AddIfNotGrounded(ref res, p, i, 1);
                AddIfNotGrounded(ref res, n, i, -1);
            }
            return res;
        }

        Matrix<double> GenC() => this.GenB().Transpose();

        public Matrix<double> GenZ() {
            var builder = Matrix<double>.Build;
            var size = this.N + this.M;
            var res = builder.Dense(size, 1);
            foreach (var iSource in this.iSources) {
                var p = iSource.positive.Id;
                var n = iSource.negative.Id;
                var i = iSource.current;

                AddIfNotGrounded(ref res, p, 0, i);
                AddIfNotGrounded(ref res, n, 0, -i);
            }
            foreach ((var vSource, var i) in this.vSources.Select((v, i) => (v, i))) {
                AddIfNotGrounded(ref res, this.N + i, 0, vSource.voltage);
            }
            return res;
        }

        public Matrix<double> SolveX() {
            /* Now we can solve the equation AX=Z */
            var A = this.GenA();
            var Z = this.GenZ();
            var X = A.Inverse() * Z;
            return X;
        }
    }
}
