using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace library {
    public class DCAnalysis {
        public static void AddIfNotGrounded(ref Matrix<double> mat, int i, int j, double num) {
            if (i != -1 && j != -1) {
                mat[i, j] += num;
            }
        }

        public static void AddIfNotGrounded(ref Vector<double> mat, int i, double num) {
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
            foreach (var device in c.Devices) {
                var g = device.Conductance();
                if (device.Pins.Count != 2) {
                    throw new NotImplementedException();
                }
                int i = device.Pins[0].ID;
                int j = device.Pins[1].ID;

                AddIfNotGrounded(ref res, i, i, g);
                AddIfNotGrounded(ref res, j, j, g);
                AddIfNotGrounded(ref res, i, j, -g);
                AddIfNotGrounded(ref res, j, i, -g);
            }
            return res;
        }

        static Matrix<double> GenB(Circuit c) {
            var builder = Matrix<double>.Build;
            var res = builder.Dense(c.N, c.M);
            foreach ((var vSource, var i) in c.VSources.Select((v, i) => (v, i))) {
                var p = vSource.Positive.ID;
                var n = vSource.Negative.ID;

                AddIfNotGrounded(ref res, p, i, 1);
                AddIfNotGrounded(ref res, n, i, -1);
            }
            return res;
        }

        static Matrix<double> GenC(Circuit c) => GenB(c).Transpose();

        public static Vector<double> GenZ(Circuit c) {
            var builder = Vector<double>.Build;
            var size = c.N + c.M;
            var res = builder.Dense(size);
            foreach (var iSource in c.ISources) {
                var p = iSource.Positive.ID;
                var n = iSource.Negative.ID;
                var i = iSource.Current;

                AddIfNotGrounded(ref res, p, i);
                AddIfNotGrounded(ref res, n, -i);
            }
            foreach ((var vSource, var i) in c.VSources.Select((v, i) => (v, i))) {
                AddIfNotGrounded(ref res, c.N + i, vSource.Voltage);
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