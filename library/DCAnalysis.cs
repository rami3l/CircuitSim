using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace library {
    public class DCAnalysis {
        public static void AddIfNotGrounded(Matrix<double> mat, int i, int j, double num) {
            if (i != -1 && j != -1) {
                mat[i, j] += num;
            }
        }

        public static void AddIfNotGrounded(Vector<double> mat, int i, double num) {
            if (i != -1) {
                mat[i] += num;
            }
        }

        public static Matrix<double> GenA(Circuit c) {
            /* The matrix A = {{G|B},{C|D}}  */
            var builder = Matrix<double>.Build;
            var n = c.N;
            var m = c.M;
            var size = n + m;
            var res = builder.Dense(size, size);
            var G = GenG(c);
            var B = GenB(c);
            var C = GenC(c);
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

        static Matrix<double> GenG(Circuit c) {
            var builder = Matrix<double>.Build;
            var res = builder.Dense(c.N, c.N);
            foreach (var device in c.devices) {
                var g = device.Conductance();
                if (device.pins.Count != 2) {
                    throw new NotImplementedException();
                }
                int i = device.pins[0].ID;
                int j = device.pins[1].ID;

                AddIfNotGrounded(res, i, i, g);
                AddIfNotGrounded(res, j, j, g);
                AddIfNotGrounded(res, i, j, -g);
                AddIfNotGrounded(res, j, i, -g);
            }
            return res;
        }

        static Matrix<double> GenB(Circuit c) {
            var builder = Matrix<double>.Build;
            var res = builder.Dense(c.N, c.M);
            foreach ((var vSource, var i) in c.vSources.Select((v, i) => (v, i))) {
                var p = vSource.positive.ID;
                var n = vSource.negative.ID;

                AddIfNotGrounded(res, p, i, 1);
                AddIfNotGrounded(res, n, i, -1);
            }
            return res;
        }

        static Matrix<double> GenC(Circuit c) => GenB(c).Transpose();

        public static Vector<double> GenZ(Circuit c) {
            var builder = Vector<double>.Build;
            var size = c.N + c.M;
            var res = builder.Dense(size);
            foreach (var iSource in c.iSources) {
                var p = iSource.positive.ID;
                var n = iSource.negative.ID;
                var i = iSource.current;

                AddIfNotGrounded(res, p, i);
                AddIfNotGrounded(res, n, -i);
            }
            foreach ((var vSource, var i) in c.vSources.Select((v, i) => (v, i))) {
                AddIfNotGrounded(res, c.N + i, vSource.voltage);
            }
            return res;
        }

        public static Vector<double> SolveX(Circuit c) {
            // Now we can solve the equation AX=Z.
            // The output vector X holds N elements representing N node voltages, 
            // and then M elements representing the currents through the M independent voltage sources.
            var A = GenA(c);
            var Z = GenZ(c);
            var X = A.Solve(Z);
            return X;
        }
    }
}