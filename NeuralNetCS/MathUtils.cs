using System;

namespace NeuralNetCS {
    internal static class MathUtils {
        public static double Sigmoide(double value) {
            return 1.0 / (1.0 + Math.Exp(-value));
        }
    }
}
