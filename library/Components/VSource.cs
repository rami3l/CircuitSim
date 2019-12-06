using System;

namespace library {
    public class PrimVSource : Component {
        // A Primitive (Constant) voltage source.
        public double Voltage;
        public Node Positive;
        public Node Negative;
        public PrimVSource(string name, double voltage, Node positive, Node negative) {
            this.Name = name;
            this.Voltage = voltage;
            this.Positive = positive;
            this.Negative = negative;
        }
    }

    public class VSource : PrimVSource {
        public enum WorkingMode {
            Constant,
            Sine,
            Square,
            Step,
            // TODO: Triangle,
        }

        public WorkingMode Mode;

        public double Offset;
        public double Frequency;
        public double Delay;

        // Initial Phase for sine waves
        public double InitialPhase;

        // Rise time for step outputs
        public double RiseTime;

        // Duty Cycle for square waves
        public double DutyCycle;

        public VSource(string name, double voltage, Node positive, Node negative) : base(name, voltage, positive, negative) {
            // Initialize a voltage source.
            // The default working mode is Constant.
            this.Mode = WorkingMode.Constant;
        }

        public void SetSine(double offset, double frequency, double delay, double initialPhase) {
            this.Mode = WorkingMode.Sine;
            this.Offset = offset;
            this.Frequency = frequency;
            this.Delay = delay;
            this.InitialPhase = initialPhase;
        }

        public void SetSquare(double offset, double frequency, double delay, double dutyCycle) {
            this.Mode = WorkingMode.Square;
            this.Offset = offset;
            this.Frequency = frequency;
            this.Delay = delay;
            this.DutyCycle = dutyCycle;
        }

        public void SetStep(double offset, double delay, double riseTime) {
            this.Mode = WorkingMode.Step;
            this.Offset = offset;
            this.Delay = delay;
            this.RiseTime = riseTime;
        }

        public double GetValue(double currentTime) {
            switch (this.Mode) {
                case WorkingMode.Constant: {
                    return this.Voltage;
                }
                case WorkingMode.Sine: {
                    var omega = 2 * Math.PI / this.Frequency;
                    var t = currentTime - Delay;
                    var AC = t < 0 ?
                        this.Voltage * Math.Sin(omega * t + this.InitialPhase) :
                        0;
                    return this.Offset + AC;
                }
                case WorkingMode.Square: {
                    var T = 1 / this.Frequency;
                    var t = currentTime - this.Delay;
                    double AC;
                    if (t < 0) {
                        AC = 0;
                    } else {
                        double r;
                        for (r = t; r >= T; r -= T) {
                            ;
                        }
                        if (r <= T * this.DutyCycle) {
                            AC = this.Voltage;
                        } else {
                            AC = -this.Voltage;
                        }
                    }
                    return this.Offset + AC;
                }
                case WorkingMode.Step: {
                    var t = currentTime - this.Delay;
                    double AC;
                    if (t < 0) {
                        AC = 0;
                    } else if (t >= this.RiseTime) {
                        return this.Voltage;
                    } else {
                        var k = this.Voltage / this.RiseTime;
                        return k * t;
                    }
                    return this.Offset + AC;
                }
                default: {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
