namespace NeuralNetCS {
    internal class NeuronLinear: Neuron {
        public override double GetActivationValue() {
            return GetPreActivationValue();
        }
    }
}
