namespace NeuralNetCS {
    class Neuron {
        private double _activationValue = 0d;
        private double _sigma = 0d;

        public double GetSigmoide() {
            return MathUtils.Sigmoide(_activationValue);
        }

        public double GetActivationValue() {
            return _activationValue;
        }

        public double SetActivationValue(double value) {
            _activationValue = value;
            return _activationValue;
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
