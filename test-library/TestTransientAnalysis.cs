using System;
using System.Collections.Generic;
using library;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Xunit;

namespace test_library {
    public class TestTransientAnalysis {
        public static Circuit RCTestCircuit() {
            var testCkt = new Circuit("RCTestCircuit");
            var gnd = testCkt.Ground;
            var node0 = testCkt.GenNode();
            var node1 = testCkt.GenNode();
            testCkt.AddComponent(new Resistor("R1", 1E3, node0, node1));
            testCkt.AddComponent(new Capacitor("C1", 1E-6, node1, gnd));

            var vs = new VSource("vStep", 1, node0, gnd);
            vs.SetStep(0, 0, 1E-9);
            testCkt.AddComponent(vs);
            return testCkt;
        }

        [Fact]
        static void RCTestCircuit_TransAnalysis() {
            var ckt = RCTestCircuit();
            var data = new TransientAnalysisData(ckt, 1E-5, 6E-3);
            TransientAnalysis.Analyze(ckt, ref data);
            var dataVList = new List<double>();
            foreach (var item in data.Result) {
                dataVList.Add(item[1]);
            }
            double[] dataV = dataVList.ToArray();

            double[] shouldDataV = {
                0,
                0.009900990099009901,
                0.019703950593079108,
                0.029409852072355552,
                0.039019655517183706,
                0.048534312393251185,
                0.057954764745793245,
                0.06728194529286459,
                0.0765167775176877,
                0.08566017576008682,
                0.09471304530701664,
                0.10367628248219468,
                0.1125507747348462,
                0.12133740072757047,
                0.13003703042333706,
                0.13865052517162083,
                0.14717873779368396,
                0.1556225126670138,
                0.16398268580892456,
                0.1722600849593312,
            };

            for (int i = 0; i < 20; i++) {
                Assert.True(Precision.AlmostEqual(dataV[i], shouldDataV[i], 8));
            }
        }

        public static Circuit RLTestCircuit() {
            var testCkt = new Circuit("RLTestCircuit");
            var gnd = testCkt.Ground;
            var node0 = testCkt.GenNode();
            var node1 = testCkt.GenNode();
            testCkt.AddComponent(new Resistor("R1", 1E3, node0, node1));
            testCkt.AddComponent(new Inductor("L1", 1, node1, gnd));

            var vs = new VSource("vStep", 1, node0, gnd);
            vs.SetStep(0, 0, 1E-9);
            testCkt.AddComponent(vs);
            return testCkt;
        }

        [Fact]
        static void RLTestCircuit_TransAnalysis() {
            var ckt = RLTestCircuit();
            var data = new TransientAnalysisData(ckt, 1E-5, 6E-3);
            TransientAnalysis.Analyze(ckt, ref data);
            var dataVList = new List<double>();
            foreach (var item in data.Result) {
                dataVList.Add(item[0] - item[1]);
            }
            double[] dataV = dataVList.ToArray();

            double[] shouldDataV = {
                0,
                0.009900990099009901,
                0.019703950593079108,
                0.029409852072355552,
                0.039019655517183706,
                0.048534312393251185,
                0.057954764745793245,
                0.06728194529286459,
                0.0765167775176877,
                0.08566017576008682,
                0.09471304530701664,
                0.10367628248219468,
                0.1125507747348462,
                0.12133740072757047,
                0.13003703042333706,
                0.13865052517162083,
                0.14717873779368396,
                0.1556225126670138,
                0.16398268580892456,
                0.1722600849593312,
            };

            for (int i = 0; i < 20; i++) {
                Assert.True(Precision.AlmostEqual(dataV[i], shouldDataV[i], 8));
            }
        }
    }
}
