namespace library {
    public class ISource : Component {
        public double current;
        public Node positive;
        public Node negative;
        public ISource(string isName, double i, Node pos, Node neg) {
            name = isName;
            current = i;
            positive = pos;
            negative = neg;
        }
    }
}
