using System.Collections.Generic;
using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace library {
    public class TransientAnalysis {
        private static double CheckLastVoltage(Node n, List<Vector<double>> result) {
            if (n.ID == -1) {
                return 0;
            } else {
                return result.Last()[n.ID];
            }
        }

        public static Circuit GenCompanion(in Circuit ckt, in double currentTime, in TransientAnalysisData data) {
            /* Generate a companion of the circuit for the transient analysis. */
            var companion = (Circuit)ckt.Clone();
            companion.name = $"__Companion_{ckt.name}";

            companion.vSources = new List<PrimVSource>();
            foreach (var pvs in ckt.vSources) {
                if (pvs is VSource) {
                    VSource vs = (VSource)pvs;
                    switch (vs.mode) {
                        case VSource.WorkingMode.Constant:
                            companion.AddComponent(pvs);
                            break;

                        default:
                            companion.AddComponent(new PrimVSource(
                                $"__CurrentVS_{vs.name}",
                                vs.GetValue(currentTime),
                                vs.positive,
                                vs.negative
                            ));
                            break;
                    }
                } else {
                    companion.AddComponent(pvs);
                }
            }

            //* ISource
            companion.iSources = new List<PrimISource>();
            foreach (var pis in ckt.iSources) {
                if (pis is ISource) {
                    ISource isc = (ISource)pis;
                    switch (isc.mode) {
                        case ISource.WorkingMode.Constant:
                            companion.AddComponent(pis);
                            break;

                        default:
                            companion.AddComponent(new PrimVSource(
                                $"__CurrentIS_{isc.name}",
                                isc.GetValue(currentTime),
                                isc.positive,
                                isc.negative
                            ));
                            break;
                    }
                } else {
                    companion.AddComponent(pis);
                }
            }

            companion.devices = new List<Device>();
            foreach (var device in ckt.devices) {
                switch (device) {
                    case Capacitor c:
                        // Here we use p and n to represent the positive/negative pin of the imaginary ISource Ieq.
                        var (p, n) = (c.pins[0], c.pins[1]);
                        // Get the previously calculated voltage between the two pins.
                        var vPrev = data.result.Count() != 0 ?
                            CheckLastVoltage(p, data.result) - CheckLastVoltage(n, data.result) :
                            0;
                        var gEq = c.capacitance / data.timestep;
                        companion.AddComponent(new Resistor(
                            $"__CompanionRes_{c.name}",
                            1 / gEq,
                            p,
                            n
                        ));
                        companion.AddComponent(new PrimISource(
                            $"__CompanionISource_{c.name}",
                            vPrev * gEq,
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


        public static void Analyze(in Circuit ckt, ref TransientAnalysisData data) {
            /* Perform the transient analysis and store the result in the given TransientAnalysisData variable. */
            for (double currentTime = 0; currentTime < data.stoptime; currentTime += data.timestep) {
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