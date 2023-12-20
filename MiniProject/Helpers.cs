using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject
{
    internal static class Helpers
    {
        public static double Max(double a, double b) 
        {
            return a > b ? a : b;
        }

        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static double Min(double a, double b)
        {
            return a < b ? a : b;
        }

        public static int Min(int a, int b)
        { 
            return a < b ? a : b;
        }

        public static float Min(float a, float b)
        {
            return (a < b) ? a : b;
        }

        public static double[,] GenerateGaussianKernel(int kernelSize, float sigma = 0, float mean = 0)
        {
            double[,] kernel = new double[kernelSize, kernelSize];
            sigma = (sigma == 0) ? (1 - kernelSize) / 4 : sigma;
            double sum = 0;

            int kernelCenter = (kernelSize - 1) / 2;

            for(int i = 0; i < kernelSize; ++i)
            {
                for (int j = 0; j < kernelSize; ++j)
                {
                    kernel[i, j] = Gaussian(j - kernelCenter, i - kernelCenter, sigma);
                    sum += kernel[i, j];
                }
            }

            for (int i = 0; i < kernelSize; ++i)
                for (int j = 0; j < kernelSize; ++j)
                    kernel[i, j] /= sum;

            return kernel;
        }

        public static double[] GenerateGaussianVector(int stddev)
        {
            var vector = new double[2 * stddev * 2 + 1];
            int midPoint = 2 * stddev;

            double sum = 0;

            for(int i = 0; i < vector.Length; ++i)
            {
                vector[i] = Gaussian(i - midPoint, stddev, 0);
                sum += vector[i];
            }

            for(int i = 0; i < vector.Length; ++i) vector[i] /= sum;

            return vector;
        }

        public static double Gaussian(double x, double y, double sigma, double mean = 0)
        {
            return (1 / (2 * Math.PI * sigma)) * Math.Exp(-(x * x + y * y) / (2 * sigma * sigma));
        }

        public static double Gaussian(double x, double sigma, double mean = 0)
        {
            return 1/ (sigma * Math.Sqrt(2 * Math.PI)) * Math.Exp(-(x*x - mean * mean)/(2 * sigma * sigma));
        }


        public static double Clamp(double value, double min, double max)
        {
            return (value > max) ? max : ((value < min) ? min : value);
        }

        public static double Lerp(double firstFloat, double secondFloat, double by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public static double InverseLerp(double from, double to, double value)
        {
            return (value - from) / (to - from);
        }


        //public static void ToByteAdd<T>(ref List<byte> bytes, T x)
        //{
        //    byte[] a;

        //    switch(Type.GetTypeCode(typeof(T)))
        //    {
        //        case TypeCode.Int32:
        //            a = BitConverter.GetBytes((int)(object)x);
        //            break;

        //        default:
        //            a = new byte[a.Length];
        //    }

        //    a = BitConverter.GetBytes(x);
        //    foreach (var b in a)
        //    {
        //        bytes.Add(b);
        //    }
        //}

    }
}
