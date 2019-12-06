using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace library {
    public class TransientAnalysis {
        private static double CheckLastVoltage(Node n, in TransientAnalysisData data) {
            if (n.ID == -1) {
                return 0;
            } else {
                return data.Result.Last()[n.ID];
            }
        }

        private static double CheckLastCurrent(Device d, in TransientAnalysisData data) {
            var ID = data.CompanionVSources[d];
            return data.Result.Last()[ID];
        }

        public static Circuit GenCompanion(in Circuit ckt, in double currentTime, ref TransientAnalysisData data) {
            /* Generate a companion of the circuit for the transient analysis. */
            var companion = (Circuit)ckt.Clone();
            companion.Name = $"__Companion_{ckt.Name}";

            //* Nodes
            companion.Nodes = new List<Node>();
            foreach (var node in ckt.Nodes) {
                companion.Nodes.Add(node);
            }

            //* VSource
            companion.VSources = new List<VSource>();
            foreach (var cvs in ckt.VSources) {
                companion.AddComponent(new VSource(
                    $"__CurrentVS_{cvs.Name}",
                    cvs.GetValue(currentTime),
                    cvs.Positive,
                    cvs.Negative
                ));
            }

            //* ISource
            companion.ISources = new List<ISource>();
            foreach (var cis in ckt.ISources) {
                companion.AddComponent(new VSource(
                    $"__CurrentIS_{cis.Name}",
                    cis.GetValue(currentTime),
                    cis.Positive,
                    cis.Negative
                ));
            }

            companion.Devices = new List<Device>();
            foreach (var device in ckt.Devices) {
                switch (device) {
                    case Capacitor c: {
                        // Here we use p and n to represent the positive/negative pin of the imaginary ISource Ieq.
                        var (p, n) = (c.Pins[0], c.Pins[1]);
                        // Get the previously calculated voltage between the two pins.
                        var vPrev = data.Result.Count() != 0 ?
                            CheckLastVoltage(p, data) - CheckLastVoltage(n, data) :
                            0;
                        var gEq = c.Capacitance / data.Timestep;
                        companion.AddComponent(new Resistor(
                            $"__CompanionRes_{c.Name}",
                            1 / gEq,
                            p,
                            n
                        ));
                        companion.AddComponent(new ISource(
                            $"__CompanionISource_{c.Name}",
                            vPrev * gEq,
                            p,
                            n
                        ));
                        break;
                    }
                    case Inductor l: {
                        // Here we use p and n to represent two pins of the inductor.
                        var (p, n) = (l.Pins[0], l.Pins[1]);
                        // Then we add an extra node m to the circuit.
                        var m = companion.GenNode($"__CompanionNode_{l.Name}");
                        // Get the previously calculated current through the inductor.
                        var iPrev = data.Result.Count() != 0 ?
                            -CheckLastCurrent(l, data) :
                            0;
                        var rEq = l.Inductance / data.Timestep;
                        companion.AddComponent(new Resistor(
                            $"__CompanionRes_{l.Name}",
                            rEq,
                            m,
                            n
                        ));
                        companion.AddComponent(new VSource(
                            $"__CompanionVSource_{l.Name}",
                            iPrev * rEq,
                            p,
                            m
                        ));
                        data.CompanionVSources[l] = companion.N + companion.M - 1;
                        break;
                    }
                    default: {
                        companion.AddComponent(device);
                        break;
                    }
                }
            }

            return companion;
            // throw new NotImplementedException();
        }


        public static void Analyze(in Circuit ckt, ref TransientAnalysisData data) {
            /* Perform the transient analysis and store the result in the given TransientAnalysisData variable. */
            for (double currentTime = 0; currentTime < data.Stoptime; currentTime += data.Timestep) {
                var companion = TransientAnalysis.GenCompanion(ckt, currentTime, ref data);
                data.Add(DCAnalysis.SolveX(companion));
            }
        }
    }

    public class TransientAnalysisData {
        /* A class containing the transient analysis parameters and result. */
        public List<Vector<double>> Result;
        public double Stoptime;
        public double Timestep;

        // A dictionary of companion vSources and their corresponding index in the result vector.
        public Dictionary<Device, int> CompanionVSources;

        public TransientAnalysisData(Circuit ckt, double timestep, double stoptime) {
            var builder = Vector<double>.Build;
            var n = ckt.N;
            var m = ckt.M;
            var size = n + m;

            this.Result = new List<Vector<double>>();
            this.CompanionVSources = new Dictionary<Device, int>();

            this.Timestep = timestep;
            this.Stoptime = stoptime;
        }


        public void Add(Vector<double> item) {
            // Add a result vector to the history.
            this.Result.Add(item);
        }
    }
}