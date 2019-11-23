using System;

namespace library {
    public class PrimVSource : Component {
        // A Primitive (Constant) voltage source.
        public double voltage;
        public Node positive;
        public Node negative;
        public PrimVSource(string name, double voltage, Node positive, Node negative) {
            this.name = name;
            this.voltage = voltage;
            this.positive = positive;
            this.negative = negative;
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

        public WorkingMode mode;

        public double offset;
        public double frequency;
        public double delay;

        // Initial Phase for sine waves
        public double initialPhase;

        // Rise time for step outputs
        public double riseTime;

        // Duty Cycle for square waves
        public double dutyCycle;

        public VSource(string name, double voltage, Node positive, Node negative) : base(name, voltage, positive, negative) {
            // Initialize a voltage source.
            // The default working mode is Constant.
            this.mode = WorkingMode.Constant;
        }

        public void SetSine(double offset, double frequency, double delay, double initialPhase) {
            this.offset = offset;
            this.frequency = frequency;
            this.delay = delay;
            this.initialPhase = initialPhase;
        }

        public void SetSquare(double offset, double frequency, double delay, double dutyCycle) {
            this.offset = offset;
            this.frequency = frequency;
            this.delay = delay;
            this.dutyCycle = dutyCycle;
        }

        public void SetStep(double offset, double delay, double riseTime) {
            this.offset = offset;
            this.delay = delay;
            this.riseTime = riseTime;
        }

        public double GetValue(double currentTime) {
            switch (this.mode) {
                case WorkingMode.Constant: {
                        return this.voltage;
                    }
                case WorkingMode.Sine: {
                        var omega = 2 * Math.PI / this.frequency;
                        var t = currentTime - delay;
                        var AC = t < 0 ?
                            this.voltage * Math.Sin(omega * t + this.initialPhase) :
                            0;
                        return this.offset + AC;
                    }
                case WorkingMode.Square: {
                        var T = 1 / this.frequency;
                        var t = currentTime - this.delay;
                        double AC;
                        if (t < 0) {
                            AC = 0;
                        } else {
                            double r;
                            for (r = t; r >= T; r -= T) { }
                            if (r <= T * this.dutyCycle) {
                                AC = this.voltage;
                            } else {
                                AC = -this.voltage;
                            }
                        }
                        return this.offset + AC;
                    }
                case WorkingMode.Step: {
                        var t = currentTime - this.delay;
                        double AC;
                        if (t < 0) {
                            AC = 0;
                        } else if (t >= this.riseTime) {
                            return this.voltage;
                        } else {
                            var k = this.voltage / this.riseTime;
                            return k * t;
                        }
                        return this.offset + AC;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
