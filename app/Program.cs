using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
using library;

namespace app {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!");
            TestPlotBasic();
            TestTransAnalysis();
        }

        public static Circuit TestCkt2() {
            var testCkt = new Circuit("testTransAnalysis");
            var gnd = testCkt.ground;
            var node0 = testCkt.GenNode();
            var node1 = testCkt.GenNode();
            testCkt.AddComponent(new Resistor("R1", 1E3, node0, node1));
            testCkt.AddComponent(new Capacitor("C1", 1E-6, node1, gnd));

            var vs = new VSource("vStep", 1, node0, gnd);
            vs.SetStep(0, 0, 1E-9);
            testCkt.AddComponent(vs);
            return testCkt;
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

        static void TestTransAnalysis() {
            var ckt2 = TestCkt2();
            var data = new TransientAnalysisData(ckt2, 3E-5, 6E-3);
            TransientAnalysis.Analyze(ckt2, ref data);
            var dataVList = new List<double>();
            foreach (var item in data.result) {
                dataVList.Add(item[1]);
            }
            double[] dataV = dataVList.ToArray();

            int pointCount = dataV.Length;
            double[] dataXs = ScottPlot.DataGen.Consecutive(pointCount);

            var plt = new ScottPlot.Plot();
            plt.PlotScatter(dataXs, dataV);
            plt.Title("TestTransAnalysis");
            plt.XLabel("Time");
            plt.YLabel("Potential");
            plt.SaveFig("./Plots/TestTransAnalysis.png");
        }
    }
}
