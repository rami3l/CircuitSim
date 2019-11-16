using System.Collections.Generic;
using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace library {
    public class TransientAnalysis {

        public static double stoptime;
        public static double timestep;

        // TODO: Setters for the time parameters


        public static Circuit GenCompanion(in Circuit ckt, in TransientAnalysisResult result) {
            /* Generate a companion of the circuit for the transient analysis. */
            var companion = new Circuit($"__Companion_{ckt.title}");
            foreach (var device in ckt.devices) {
                switch (device) {
                    case Capacitor c:
                        // Here we use p and n to represent the positive/negative pin of the imaginary ISource Ieq.
                        var (p, n) = (c.pins[0], c.pins[1]);
                        // Get the previously calculated voltage between the two pins.
                        var vPrev = result.history.Last()[p.ID] - result.history.Last()[n.ID];
                        companion.AddComponent(new Resistor(
                            $"__CompanionRes_{c.name}",
                            timestep / c.capacitance,
                            p,
                            n
                        ));
                        companion.AddComponent(new ISource(
                            $"__CompanionISource_{c.name}",
                            vPrev * timestep / c.capacitance,
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


        public void Analyze(in Circuit ckt, ref TransientAnalysisResult result) {
            // Perform the transient analysis and store the result in the given TransientAnalysisResult variable.
            for (double currentTime = 0; currentTime < stoptime; currentTime += timestep) {
                var companion = TransientAnalysis.GenCompanion(ckt, result);
                result.Add(DCAnalysis.SolveX(companion));
            }
        }
    }

    public class TransientAnalysisResult {
        public List<Vector<double>> history;

        public TransientAnalysisResult(Circuit ckt) {
            var builder = Vector<double>.Build;
            var n = ckt.N;
            var m = ckt.M;
            var size = n + m;

            history = new List<Vector<double>>();
            history.Add(builder.Dense(size));
        }

        public void Add(Vector<double> record) {
            this.history.Add(record);
        }
    }
}