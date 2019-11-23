using System.Collections.Generic;
using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace library {
    public class TransientAnalysis {
        public static Circuit GenCompanion(in Circuit ckt, in double currentTime, in TransientAnalysisData data) {
            /* Generate a companion of the circuit for the transient analysis. */
            var companion = new Circuit($"__Companion_{ckt.name}");
            foreach (var device in ckt.devices) {
                switch (device) {
                    case Capacitor c:
                        // Here we use p and n to represent the positive/negative pin of the imaginary ISource Ieq.
                        var (p, n) = (c.pins[0], c.pins[1]);
                        // Get the previously calculated voltage between the two pins.
                        var vPrev = data.result.Last()[p.ID] - data.result.Last()[n.ID];
                        companion.AddComponent(new Resistor(
                            $"__CompanionRes_{c.name}",
                            data.timestep / c.capacitance,
                            p,
                            n
                        ));
                        companion.AddComponent(new PrimISource(
                            $"__CompanionISource_{c.name}",
                            vPrev * data.timestep / c.capacitance,
                            p,
                            n
                        ));
                        break;
                    case Inductor l:
                        throw new NotImplementedException();
                    default:
                        companion.AddComponent(device);
                        break;
                }
            }
            return companion;
            // throw new NotImplementedException();
        }


        public void Analyze(in Circuit ckt, ref TransientAnalysisData data) {
            /* Perform the transient analysis and store the result in the given TransientAnalysisData variable. */
            // Start the simulation with a DC analysis.
            data.Add(DCAnalysis.SolveX(ckt));
            for (double currentTime = data.timestep; currentTime < data.stoptime; currentTime += data.timestep) {
                var companion = TransientAnalysis.GenCompanion(ckt, currentTime, data);
                data.Add(DCAnalysis.SolveX(companion));
            }
        }
    }

    public class TransientAnalysisData {
        /* A class containing the transient analysis parameters and result. */
        public List<Vector<double>> result;
        public double stoptime;
        public double timestep;

        public TransientAnalysisData(Circuit ckt, double timestep, double stoptime) {
            var builder = Vector<double>.Build;
            var n = ckt.N;
            var m = ckt.M;
            var size = n + m;

            this.result = new List<Vector<double>>();

            this.timestep = timestep;
            this.stoptime = stoptime;
        }

        public void Add(Vector<double> item) {
            // Add a result vector to the history.
            this.result.Add(item);
        }
    }
}