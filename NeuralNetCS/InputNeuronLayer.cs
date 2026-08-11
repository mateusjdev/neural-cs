namespace NeuralNetCS
{
    internal class InputNeuronLayer : NeuronLayer
    {
        public InputNeuronLayer(int nNeurons) : base(nNeurons) { }

        public override double GetSigmo(int at)
        {
            return GetActivationValue(at);
        }
    }
}
