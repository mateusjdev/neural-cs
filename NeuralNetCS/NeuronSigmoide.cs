namespace NeuralNetCS {
    internal class NeuronSigmoide: Neuron {
        public override double GetActivationValue() {
            return MathUtils.Sigmoide(GetPreActivationValue());
        }
    }
}
