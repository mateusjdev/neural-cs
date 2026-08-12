namespace NeuralNetCS {

    abstract class Neuron {
        private double _preActivation = 0d;
        private double _delta = 0d;

        public abstract double GetActivationValue();

        protected double GetPreActivationValue() {
            return _preActivation;
        }

        public double SetPreActivationValue(double value) {
            _preActivation = value;
            return _preActivation;
        }

        public double GetDelta() {
            return _delta;
        }

        public double SetDelta(double value) {
            _delta = value;
            return _delta;
        }
    }
}
