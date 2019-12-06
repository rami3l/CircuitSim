using System.Linq;
using System.Collections.Generic;
using System;
using library;

namespace app {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!");
            TestPlotBasic();
            // TestTransAnalysis();
            TestACSource();
        }

        static void TestPlotBasic() {
            int pointCount = 50;
            double[] dataXs = ScottPlot.DataGen.Consecutive(pointCount);
            double[] dataSin = ScottPlot.DataGen.Sin(pointCount);
            double[] dataCos = ScottPlot.DataGen.Cos(pointCount);

            var plt = new ScottPlot.Plot();
            plt.PlotScatter(dataXs, dataSin);
            plt.PlotScatter(dataXs, dataCos);
            plt.Title("ScottPlot Quickstart");
            plt.XLabel("Time (seconds)");
            plt.YLabel("Potential (V)");
            plt.SaveFig("./Plots/01a_Quickstart.png");
        }

        public static Circuit TransientTestCkt() {
            var testCkt = new Circuit("testTransAnalysis");
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

        static void TestTransAnalysis() {
            var ckt = TransientTestCkt();
            var data = new TransientAnalysisData(ckt, 1E-5, 6E-3);
            TransientAnalysis.Analyze(ckt, ref data);
            var dataVList = new List<double>();
            foreach (var item in data.Result) {
                dataVList.Add(item[0] - item[1]);
            }
            double[] dataV = dataVList.ToArray();

            /*
            for (int i = 0; i < 20; i++) {
                Console.Write("{0}, ", dataV[i]);
            }
            Console.WriteLine();
            */

            int pointCount = dataV.Length;
            double[] dataXs = ScottPlot.DataGen.Consecutive(pointCount);

            var plt = new ScottPlot.Plot();
            plt.PlotScatter(dataXs, dataV);
            plt.Title("TestTransAnalysis");
            plt.XLabel("Time/(10us)");
            plt.YLabel("Potential/V");
            plt.SaveFig("./Plots/TestTransAnalysis.png");
        }

        public static Circuit ACVSourceTestCkt() {
            var testCkt = new Circuit("ACVSourceTestCkt");
            var gnd = testCkt.Ground;
            var node0 = testCkt.GenNode();
            testCkt.AddComponent(new Resistor("R1", 1, node0, gnd));

            var vs = new VSource("vSin", 1, node0, gnd);
            vs.SetSine(2, 4, 50E-3, -Math.PI / 4);
            testCkt.AddComponent(vs);
            return testCkt;
        }

        static void TestACSource() {
            var ckt1 = ACVSourceTestCkt();
            var data1 = new TransientAnalysisData(ckt1, 1E-2, 2);
            TransientAnalysis.Analyze(ckt1, ref data1);
            var dataVList1 = new List<double>();
            foreach (var item in data1.Result) {
                dataVList1.Add(-item[1]);
            }
            double[] dataV1 = dataVList1.ToArray();

            /*
            for (int i = 0; i < 20; i++) {
                Console.Write("{0}, ", dataV1[i]);
            }
            Console.WriteLine();
            */


            int pointCount = dataV1.Length;
            double[] dataXs = ScottPlot.DataGen.Consecutive(pointCount);

            var plt = new ScottPlot.Plot();
            plt.PlotScatter(dataXs, dataV1);
            plt.Title("TestACSource");
            plt.XLabel("Time/(10ms)");
            plt.YLabel("Result");
            plt.SaveFig("./Plots/TestACSource.png");
        }
    }
}
