using System;

namespace NeuralNetCS {
    internal static class MathUtils {
        public static double Sigmoide(double value) {
            return 1 / (1 + Math.Exp(-value));
        }
    }
}
