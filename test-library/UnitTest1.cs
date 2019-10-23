using System;
using library;
using MathNet.Numerics.LinearAlgebra;
using Xunit;

namespace test_library {
    public class UnitTest1 {
        public Circuit TestCkt1() {
            var testCkt = new Circuit("testCkt");
            var gnd = testCkt.ground;
            var node1 = testCkt.GenNode();
            var node2 = testCkt.GenNode();
            var node3 = testCkt.GenNode();
            testCkt.AddComponent(new Resistor("R1", 2, node1, gnd));
            testCkt.AddComponent(new Resistor("R2", 4, node2, node3));
            testCkt.AddComponent(new Resistor("R3", 8, node2, gnd));
            testCkt.AddComponent(new VSource("Vs1", 32, node2, node1));
            testCkt.AddComponent(new VSource("Vs2", 32, node3, gnd));
            return testCkt;
        }

        [Fact]
        public void TestCkt1_GenA() {
            var builder = Matrix<double>.Build;
            var testCkt = TestCkt1();
            double[, ] shouldArr = {
                {
                0.5,
                0,
                0,
                -1,
                0
                },
                {
                0,
                0.375,
                -0.25,
                1,
                0
                },
                {
                0,
                -0.25,
                0.25,
                0,
                1
                },
                {-1,
                1,
                0,
                0,
                0
                },
                {
                0,
                0,
                1,
                0,
                0
                },
            };
            var should = builder.DenseOfArray(shouldArr);

            Assert.Equal(testCkt.GenA(), should);
        }
    }
}
