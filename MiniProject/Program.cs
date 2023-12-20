using System.Xml.Schema;

namespace MiniProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bitmap bmp = new(@"D:\bbs\miniproject bs\barbara.bmp");

            //Bitmap bmp2 = bmp;

            //bmp.SetPixelData(bmp2.GetPixelData());

            //bmp.WriteBitmap(@"D:\bbs\miniproject bs\sanitycheck.bmp");

            //int r = 11;

            //Console.WriteLine(Helpers.Gaussian(0, 0, 0, 0));

            //var x = Helpers.GenerateGaussianKernel(r, 0.84089642f);

            //Console.WriteLine(x.Length);

            //double gsum = 0;

            //foreach (var xx in x) gsum += xx;



            //for (int i = 0; i < x.Length; i += r)
            //{
            //    Console.Write("[ ");
            //    for (int j = 0; j < r; ++j)
            //    {
            //        Console.Write($" {x[i / r, j]:F16} ");
            //    }
            //    Console.WriteLine(" ]");
            //}

            //Console.WriteLine(gsum);

            //var kernelSize = 7;

            //string[,] kernel = new string[kernelSize, kernelSize];


            //int kernelCenter = (kernelSize - 1) / 2;

            //for (int i = 0; i < kernelSize; ++i)
            //{
            //    for (int j = 0; j < kernelSize; ++j)
            //    {
            //        kernel[i, j] = $"{j - kernelCenter:000}, {i - kernelCenter:000}";
            //    }
            //}

            //for (int i = 0; i < kernel.Length; i += 7)
            //{
            //    Console.Write("[ ");
            //    for (int j = 0; j < 7; ++j)
            //    {
            //        Console.Write(kernel[i / 7, j]);
            //    }
            //    Console.WriteLine(" ]");
            //}
            //Bitmap nbmp = bmp;

            //nbmp.SetPixelData(bmp.GaussianBlur(1, 100f));
            //nbmp.WriteBitmap(@"D:\bbs\miniproject bs\gaussianblurredbarbara");
            //nbmp.SetPixelData(bmp.BoxBlur(5));
            //nbmp.WriteBitmap(@"D:\bbs\miniproject bs\boxblurredlena");

            //for(float i = 0.00001f; i < 1.10001f; i += 0.25f)
            //{
            //    for(int j = 1; j < 4; j+= 1)
            //    {

            //        nbmp.SetPixelData(bmp.GaussianBlur(j, i));
            //        nbmp.WriteBitmap($@"D:\bbs\miniproject bs\lena_stddev_{i}_rad_{ j }");
            //        Console.WriteLine($"{i}, {j}");
            //    }
            //}

            var x = Helpers.GenerateGaussianVector(1);
            double sum = 0;

            for (int i = 0; i < x.Length; i++)
            {
                Console.WriteLine(x[i]);
                sum += x[i];
            }

            Console.WriteLine(sum);


            //bmp.WriteBitmap(@"D:\bbs\miniproject bs\gblurredlena.bmp"); bmp.GetPixelData();
        }
    }
}

