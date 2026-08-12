namespace NeuralNetCS
{
    internal class InputNeuronLayer : NeuronLayer
    {
        public InputNeuronLayer(int nNeurons) : base(nNeurons) { }

        public override double GetSigmoide(int at)
        {
            return GetPreActivationValue(at);
        }
    }
}
