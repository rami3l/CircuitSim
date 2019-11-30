using System;
using library;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Xunit;

namespace test_library {
    public class TestDCAnalysis {
        public static Circuit DCTestCircuit() {
            var testCkt = new Circuit("DCTestCircuit");
            var gnd = testCkt.Ground;
            var node1 = testCkt.GenNode();
            var node2 = testCkt.GenNode();
            var node3 = testCkt.GenNode();
            testCkt.AddComponent(new Resistor("R1", 2, node1, gnd));
            testCkt.AddComponent(new Resistor("R2", 4, node2, node3));
            testCkt.AddComponent(new Resistor("R3", 8, node2, gnd));
            testCkt.AddComponent(new PrimVSource("Vs1", 32, node2, node1));
            testCkt.AddComponent(new PrimVSource("Vs2", 20, node3, gnd));
            return testCkt;
        }

        [Fact]
        public void DCTestCircuit_GenA() {
            var builder = Matrix<double>.Build;
            var testCkt = DCTestCircuit();
            double[,] shouldArr = {
                {0.5,   0,      0,      -1,     0},
                {0,     0.375,  -0.25,  1,      0},
                {0,     -0.25,  0.25,   0,      1},
                {-1,    1,      0,      0,      0},
                {0,     0,      1,      0,      0},
            };
            var should = builder.DenseOfArray(shouldArr);

            Assert.True(Precision.AlmostEqual(DCAnalysis.GenA(testCkt), should, 8));
        }

        [Fact]
        public void DCTestCircuit_GenZ() {
            var builder = Vector<double>.Build;
            var testCkt = DCTestCircuit();
            double[] shouldArr = { 0, 0, 0, 32, 20 };
            var should = builder.DenseOfArray(shouldArr);

            Assert.True(Precision.AlmostEqual(DCAnalysis.GenZ(testCkt), should, 8));
        }

        [Fact]
        public void DCTestCircuit_SolveX() {
            var builder = Vector<double>.Build;
            var testCkt = DCTestCircuit();
            double[] shouldArr = { -8, 24, 20, -4, 1 };
            var should = builder.DenseOfArray(shouldArr);

            Assert.True(Precision.AlmostEqual(DCAnalysis.SolveX(testCkt), should, 8));
        }
    }
}
