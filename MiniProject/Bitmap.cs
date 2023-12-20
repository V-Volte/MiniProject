using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static MiniProject.Helpers;

namespace MiniProject
{
    class Bitmap
    {
        static readonly byte[] BMHeaderBegin = { 0x42, 0x4D };
        public uint BMBitmapSize { get; protected set; }
        public ushort BMRes1 { get; protected set; }
        public ushort BMRes2 { get; protected set; }

        public uint BMOffset = 54;

        public uint DIBHeaderSize = 40;
        public int DIBWidth { get; protected set; }
        public int DIBHeight { get; protected set; }
        public ushort DIBColourPlanes { get; protected set; }

        public ushort DIBColourDepth { get; protected set; }

        public uint DIBCompressionMethod { get; protected set; }

        public uint DIBBitmapSize { get; protected set; }

        public uint DIBVerticalResolution { get; protected set; }
        public uint DIBHorizontalResolution { get; protected set; }

        public uint DIBNumberColours { get; protected set; }

        public uint DIBImportantColours { get; protected set; }

        public string filename;

        public byte[] BitmapArray;

        public uint rowWidth;

        public byte[] DIBExtraShit;

        public int colourDepth { get => DIBColourDepth / 8;  }

        public Bitmap(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("Couldn't find the specified file.");
            }

            FileStream fileStream = File.OpenRead(filename);

            using BinaryReader binaryReader = new(fileStream);

            byte[] header = binaryReader.ReadBytes(2);

            if (header[0] != BMHeaderBegin[0] || header[1] != BMHeaderBegin[1])
            {
                throw new Exception("Unsupported file format." + header[0].ToString("X") + header[1].ToString("X"));
            }

            uint v = binaryReader.ReadUInt32();
            Console.WriteLine(v.ToString("X"));
            BMBitmapSize = v;

            BMRes1 = binaryReader.ReadUInt16();

            BMRes2 = (binaryReader.ReadUInt16());

            BMOffset = binaryReader.ReadUInt32();

            DIBHeaderSize = binaryReader.ReadUInt32();

            DIBWidth = (binaryReader.ReadInt32());
            DIBHeight = (binaryReader.ReadInt32());

            DIBColourPlanes = (binaryReader.ReadUInt16());
            DIBColourDepth = (binaryReader.ReadUInt16());
            DIBCompressionMethod = (binaryReader.ReadUInt32());

            DIBBitmapSize = (binaryReader.ReadUInt32());

            DIBVerticalResolution = (binaryReader.ReadUInt32());
            DIBHorizontalResolution = (binaryReader.ReadUInt32());

            DIBNumberColours = (binaryReader.ReadUInt32());
            DIBImportantColours = (binaryReader.ReadUInt32());

            DIBExtraShit = binaryReader.ReadBytes((int) BMOffset - 0x36);

            try
            {
                BitmapArray = binaryReader.ReadBytes((int)(DIBBitmapSize));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;

            }
            Console.WriteLine("BMPArraySize for " + filename + "is " + DIBBitmapSize);

            this.filename = filename;

            rowWidth = (uint)((BMBitmapSize - BMOffset) / DIBHeight);

            fileStream.Close();
        }

        /// <summary>
        /// Get the data of the pixels from the image.
        /// </summary>
        /// <returns>A 2D Array of pixels with the data of each pixel in the image.</returns>
        public RGBAPixel[,] GetPixelData()
        {
            List<string> lines = new List<string>();

            RGBAPixel[,] pixels = new RGBAPixel[DIBHeight, DIBWidth];

            for (int i = 0; i < DIBHeight; ++i)
            {
                for (int j = 0, k = 0; rowWidth - k >= DIBColourDepth / 8 && j < DIBWidth; j++, k += DIBColourDepth / 8)
                {
                    if (DIBColourDepth == 8)
                    {
                        pixels[i, j] = new RGBAPixel { Red = BitmapArray[i * rowWidth + k] };
                    }
                    else if (DIBColourDepth == 24)
                    pixels[i, j] = new RGBAPixel
                    {
                        Blue = BitmapArray[i * rowWidth + k],
                        Green = BitmapArray[i * rowWidth + k + 1],
                        Red = BitmapArray[i * rowWidth + k + 2]
                    };
                    else if (DIBColourDepth == 32)
                    pixels[i, j] = new RGBAPixel
                    {
                        Alpha = (sbyte)((sbyte) BitmapArray[i * rowWidth + k] - (byte) 128),
                        Blue = BitmapArray[i * rowWidth + k + 1],
                        Green = BitmapArray[i * rowWidth + k + 2],
                        Red = BitmapArray[i * rowWidth + k + 3]
                    };

                    else
                    {
                        throw new Exception("Impossible bitmap colour depth.");
                    }
                    lines.Add(pixels[i, j].ToString());
                }
            }

            File.WriteAllLines(String.Format("{0}Log.txt", filename), lines.ToArray());


            return pixels;
        }

        /// <summary>
        /// Sets each pixel in the image as supplied in an array of RGBAPixels. Ensure that the size of the array is correct.
        /// </summary>
        /// <param name="pixels"></param>
        public void SetPixelData(RGBAPixel[,] pixels)
        {
            for (int i = 0; i < DIBHeight; ++i)
            {
                for (int j = 0, k = 0; rowWidth - k >= DIBColourDepth / 8 && j < DIBWidth; ++j, k += DIBColourDepth / 8)
                {
                    if (DIBColourDepth == 8)
                        BitmapArray[i * rowWidth + k] = pixels[i, j].Red;

                    else if (DIBColourDepth == 24)
                    {
                        BitmapArray[i * rowWidth + k] = pixels[i, j].Blue;
                        BitmapArray[i * rowWidth + k + 1] = pixels[i, j].Green;
                        BitmapArray[i * rowWidth + k + 2] = pixels[i, j].Red;
                    }
                    
                    if (DIBColourDepth == 32)
                    {
                        BitmapArray[i * rowWidth + k] = (byte) (pixels[i, j].Alpha + 128);
                        BitmapArray[i * rowWidth + k + 1] = pixels[i, j].Blue;
                        BitmapArray[i * rowWidth + k + 2] = pixels[i, j].Green;
                        BitmapArray[i * rowWidth + k + 3] = pixels[i, j].Red;
                    }
                }
            }
        }

        public RGBAPixel[,] BoxBlur(int blurRadius)
        {
            var pixels = GetPixelData();
            var outpixels = new RGBAPixel[DIBHeight, DIBWidth];

            for(int i = 0; i < DIBHeight; ++i)
            {
                for(int j = 0; j < DIBWidth; ++j)
                {
                    int[] cpix = new int[colourDepth];
                    int npixels = 0;
                    for (int y = Max(0, i - blurRadius); y < Min(i + blurRadius, DIBHeight); ++y)
                    {
                        for(int x = Max(0, j - blurRadius); x < Min(j + blurRadius, DIBWidth); x++)
                        {
                            var cpixvals = pixels[y, x].GetArray(colourDepth);
                            for(int cd = 0; cd < colourDepth; ++cd)
                                cpix[cd] += cpixvals[cd];
                            npixels++;
                        }
                    }

                    //Console.WriteLine($"For pixel [{i},{j}], rowbounds [{Helpers.Max(0, i - blurRadius)},{Helpers.Min(i + blurRadius, DIBHeight)}], colbounds [{Helpers.Max(0, j - blurRadius)},{Helpers.Min(j + blurRadius, DIBWidth)}]");
                    //Console.WriteLine($"PIX Value {pixels[i,j]}");
                    //Console.WriteLine($"CPIX Values ({cpix[0]},{cpix[1]},{cpix[2]}) npixels {npixels}");
                    
                    for (int v = 0; v < colourDepth; ++v) cpix[v] = (int) (cpix[v] / (double) npixels);

                    byte[] opix = new byte[colourDepth];

                    for (int v = 0; v < colourDepth; ++v) opix[v] = (byte)(cpix[v]);

                    outpixels[i, j] = new RGBAPixel(opix);

                }
            }

            return outpixels;
        }


        public RGBAPixel[,] TwoPassGaussianBlur(int stddev = 1)
        {
            int blurRadius = 2 * stddev;
            var pixels = GetPixelData();
            var outpixels = new RGBAPixel[DIBHeight, DIBWidth];
            var pmatrix = new int[DIBHeight, DIBWidth][];
            var G = GenerateGaussianVector(stddev);


            for(int i = 0; i < DIBHeight; ++i)
            {
                for(int j = 0; j < DIBWidth; ++j)
                {
                    int[] cpix = new int[colourDepth];

                    for(int x = i - blurRadius; x < i + blurRadius; ++x)
                    {
                        int acx = x < 0 ? -x : x >= DIBHeight ? 2*DIBHeight - x - 1 : x;

                        var cpixvals = pixels[acx, j].GetArray(colourDepth);

                        for(int cd = 0; cd < colourDepth; ++cd)
                        {
                            var v = cpixvals[cd] * G[Math.Abs((int)x) - i + blurRadius];
                            cpix[cd] += (int)Math.Round(v);
                        }
                    }

                    pmatrix[i, j] = cpix;                    
                }
            }



            for (int i = 0; i < DIBHeight; ++i)
            {
                for (int j = 0; j < DIBWidth; ++j)
                {
                    int[] cpix = new int[colourDepth];

                    for (int x = j - blurRadius; x < j + blurRadius; ++x)
                    {
                        int acx = x < 0 ? -x : x >= DIBWidth ? 2 * DIBWidth - x - 1 : x;

                        var cpixvals = pixels[acx, j].GetArray(colourDepth);

                        for (int cd = 0; cd < colourDepth; ++cd)
                        {
                            var v = cpixvals[cd] * G[Math.Abs((int)x) - j + blurRadius];
                            cpix[cd] += (int)Math.Round(v);
                        }
                    }
                }
            }





            return pixels;
        }

        public RGBAPixel[,] GaussianBlur(int blurRadius, float sigma = 0)
        {
            
            var pixels = GetPixelData();
            var outpixels = new RGBAPixel[DIBHeight, DIBWidth];
            var pmatrix = new int[DIBHeight,DIBWidth][];
            var g = GenerateGaussianKernel(2 * blurRadius + 1, sigma);

            //for(int i = 0; i < 2 * blurRadius + 1; i++)
            //{
            //    Console.Write("[ ");
            //    for(int j = 0; j < 2 * blurRadius + 1; ++j)
            //    {
            //        Console.Write($" {g[i, j]} ");
            //    }
            //    Console.WriteLine("]");
            //}

            for (int i = 0; i < DIBHeight; ++i)
            {
                for (int j = 0; j < DIBWidth; ++j)
                {
                    int[] cpix = new int[colourDepth];
                    for (int y = i - blurRadius; y < i + blurRadius + 1; ++y)
                    {
                        for (int x = j - blurRadius; x < j + blurRadius + 1; x++)
                        {
                            int acx = x < 0 ? -x : x > DIBWidth - 1 ? 2 * DIBWidth - x - 1 : x;
                            int acy = y < 0 ? -y : y > DIBHeight - 1 ? 2 * DIBHeight - y - 1 : y;


                            var cpixvals = pixels[acy, acx].GetArray(colourDepth);
                            for (int cd = 0; cd < colourDepth; ++cd)
                            {
                                //Console.WriteLine($"For pixel [{i},{j}], pixel [{y},{x}] with [{acy}, {acx} has G[{Math.Abs(Math.Abs(y) - i + blurRadius)},{Math.Abs(Math.Abs(x) - j + blurRadius)}] = {g[Math.Abs(Math.Abs(y) - i + blurRadius), Math.Abs(Math.Abs(x) - j + blurRadius)]}");
                                var vcx = (int)(cpixvals[cd] * g[Math.Abs(Math.Abs(y) - i + blurRadius), Math.Abs(Math.Abs(x) - j + blurRadius)]);
                                //Console.Write($" {cpixvals[cd]} => {vcx} | ");
                                cpix[cd] += vcx;
                            }
                            //Console.WriteLine();
                        }
                    }

                    
                    pmatrix[i, j] = cpix;
                }
            }

            int[] max = new int[colourDepth];
            int[] min = new int[colourDepth];

            for(int i = 0; i < colourDepth; ++i)
            {
                max[i] = 0;
                min[i] = 255;
            }

            for(int i = 0; i < DIBHeight; ++i)
            {
                for(int j = 0; j < DIBWidth; ++j)
                {
                    for(int n = 0; n < colourDepth; ++n)
                    {
                        max[n] = Max(pmatrix[i, j][n], max[n]);
                        min[n] = Min(pmatrix[i, j][n], min[n]);
                    }
                }
            }

            for (int i = 0; i < DIBHeight; ++i)
            {
                for (int j = 0; j < DIBWidth; ++j)
                {
                    for (int n = 0; n < colourDepth; ++n)
                    {
                        var v = pmatrix[i, j][n];
                        pmatrix[i, j][n] = (int) Math.Round(Lerp(0, 255, InverseLerp(min[n], max[n], pmatrix[i, j][n])));
                        //Console.Write($" {v:D3} => {pmatrix[i, j][n]:D3} | ");
                    }
                    //Console.WriteLine();

                    outpixels[i, j] = new RGBAPixel(pmatrix[i,j].Select((int n) => (byte) n).ToArray());
                }
            }



            return outpixels;
        }

        public void WriteBitmap(string filename)
        {
            this.filename = filename;

            List<byte> bitmap = new List<byte>();

            bitmap.Add(BMHeaderBegin[0]);
            bitmap.Add(BMHeaderBegin[1]);

            bitmap.AddRange(BitConverter.GetBytes(BMBitmapSize));
            bitmap.AddRange(BitConverter.GetBytes(BMRes1));
            bitmap.AddRange(BitConverter.GetBytes(BMRes2));
            bitmap.AddRange(BitConverter.GetBytes(BMOffset));
            bitmap.AddRange(BitConverter.GetBytes(DIBHeaderSize));
            bitmap.AddRange(BitConverter.GetBytes(DIBWidth));
            bitmap.AddRange(BitConverter.GetBytes(DIBHeight));
            bitmap.AddRange(BitConverter.GetBytes(DIBColourPlanes));
            bitmap.AddRange(BitConverter.GetBytes(DIBColourDepth));
            bitmap.AddRange(BitConverter.GetBytes(DIBCompressionMethod));
            bitmap.AddRange(BitConverter.GetBytes(DIBBitmapSize));
            bitmap.AddRange(BitConverter.GetBytes(DIBVerticalResolution));
            bitmap.AddRange(BitConverter.GetBytes(DIBHorizontalResolution));
            bitmap.AddRange(BitConverter.GetBytes(DIBNumberColours));
            bitmap.AddRange(BitConverter.GetBytes(DIBImportantColours));

            bitmap.AddRange(DIBExtraShit);

            bitmap.AddRange(BitmapArray);

            using BinaryWriter binaryWriter = new BinaryWriter(new FileStream(filename + ".bmp", FileMode.Create));
            foreach (byte x in bitmap)
            {
                binaryWriter.Write(x);
            }
        }

    }
}
