using System.Collections.Generic;
using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace library {
    public class TransientAnalysis {

        public static double stoptime;
        public static double timestep;

        // TODO: Setters for the time parameters


        public Circuit Companion(Circuit ckt, TransientAnalysisResult history) {
            /* Generate a companion of the circuit for the transient analysis.  */
            var res = (Circuit)ckt.Clone();
            var companionDevices = new List<Device>();
            foreach (var device in res.devices) {
                switch (device) {
                    case Capacitor c:
                        // Here we use p and n to represent the positive/negative pin of the imaginary ISource Ieq.
                        var (p, n) = (c.pins[0], c.pins[1]);
                        // Get the previously calculated voltage between the two pins.
                        var vPrev = history.data.Last()[p.ID] - history.data.Last()[n.ID];
                        companionDevices.Add(new Resistor(
                            $"__CompanionRes_{c.name}",
                            timestep / c.capacitance,
                            p,
                            n
                        ));
                        res.iSources.Add(new ISource(
                            $"__CompanionISource_{c.name}",
                            vPrev * timestep / c.capacitance,
                            p,
                            n
                        ));
                        break;
                    case Inductor l:
                        throw new NotImplementedException();
                    // break;
                    default:
                        companionDevices.Add(device);
                        break;
                }
            }
            res.devices = companionDevices;
            return res;
            // throw new NotImplementedException();
        }
    }

    public class TransientAnalysisResult {
        public List<Vector<double>> data;

        public TransientAnalysisResult(Circuit ckt) {
            var builder = Vector<double>.Build;
            var n = ckt.N;
            var m = ckt.M;
            var size = n + m;

            data = new List<Vector<double>>();
            data.Add(builder.Dense(size));
        }
    }
}