using System;

namespace library {
    public class PrimISource : Component {
        // A Primitive (Constant) current source.
        public double current;
        public Node positive;
        public Node negative;
        public PrimISource(string name, double current, Node positive, Node negative) {
            this.name = name;
            this.current = current;
            this.positive = positive;
            this.negative = negative;
        }
    }

    public class ISource : PrimISource {
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

        public ISource(string name, double current, Node positive, Node negative) : base(name, current, positive, negative) {
            // Initialize a current source.
            // The default working mode is Constant.
            this.mode = WorkingMode.Constant;
        }

        public void SetSine(double offset, double frequency, double delay, double initialPhase) {
            this.mode = WorkingMode.Sine;
            this.offset = offset;
            this.frequency = frequency;
            this.delay = delay;
            this.initialPhase = initialPhase;
        }

        public void SetSquare(double offset, double frequency, double delay, double dutyCycle) {
            this.mode = WorkingMode.Square;
            this.offset = offset;
            this.frequency = frequency;
            this.delay = delay;
            this.dutyCycle = dutyCycle;
        }

        public void SetStep(double offset, double delay, double riseTime) {
            this.mode = WorkingMode.Step;
            this.offset = offset;
            this.delay = delay;
            this.riseTime = riseTime;
        }

        public double GetValue(double currentTime) {
            switch (this.mode) {
                case WorkingMode.Constant: {
                        return this.current;
                    }
                case WorkingMode.Sine: {
                        var omega = 2 * Math.PI / this.frequency;
                        var t = currentTime - delay;
                        var AC = t < 0 ?
                            this.current * Math.Sin(omega * t + this.initialPhase) :
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
                                AC = this.current;
                            } else {
                                AC = -this.current;
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
                            return this.current;
                        } else {
                            var k = this.current / this.riseTime;
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
