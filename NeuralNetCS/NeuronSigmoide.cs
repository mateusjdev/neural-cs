using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetCS {
    internal class NeuronSigmoide: Neuron {
        public override double GetActivationValue() {
            return MathUtils.Sigmoide(GetPreActivationValue());
        }
    }
}
