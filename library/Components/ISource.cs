namespace library {
    public class PrimISource : Component {
        // A Primitive (Constant) current source.
        public double current;
        public Node positive;
        public Node negative;
        public PrimISource(string isName, double i, Node pos, Node neg) {
            name = isName;
            current = i;
            positive = pos;
            negative = neg;
        }
    }

    public class ISource : PrimISource {
        public enum WorkingMode {
            Constant,
            Sine,
            Square,
            Step,
            Triangle,
        }

        public WorkingMode mode;

        public ISource(string isName, double i, Node pos, Node neg) : base(isName, i, pos, neg) {
            mode = WorkingMode.Constant;
        }
    }
}
