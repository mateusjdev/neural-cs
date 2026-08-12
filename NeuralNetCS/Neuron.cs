namespace NeuralNetCS {

    abstract class Neuron {
        private double _preActivationBuffer = 0d;
        private double _sigma = 0d;

        public abstract double GetActivationValue();

        protected double GetPreActivationValue() {
            return _preActivationBuffer;
        }

        public double SetPreActivationValue(double value) {
            _preActivationBuffer = value;
            return _preActivationBuffer;
        }

        public double GetSigma() {
            return _sigma;
        }

        public double SetSigma(double value) {
            _sigma = value;
            return _sigma;
        }
    }
}
