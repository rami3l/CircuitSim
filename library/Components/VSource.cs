namespace library {
    public class VSource : Component {
        public double voltage;
        public Node positive;
        public Node negative;
        public VSource(string vsName, double v, Node pos, Node neg) {
            name = vsName;
            voltage = v;
            positive = pos;
            negative = neg;
        }
    }
}
