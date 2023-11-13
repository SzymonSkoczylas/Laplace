using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

namespace Laplace
{
    public partial class Form1 : Form
    {


        Bitmap image;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public static Bitmap DivideBitmap(Bitmap originalBitmap, int numDivisions, int lineSpacing, int lineThickness)
        {
            int width = originalBitmap.Width;
            int height = originalBitmap.Height;
            int divisionSpacing = height / numDivisions;

            Bitmap dividedBitmap = new Bitmap(originalBitmap);

            using (Graphics graphics = Graphics.FromImage(dividedBitmap))
            using (Pen linePen = new Pen(Color.FromArgb(255, 192, 203), 2))
            {
                BitmapData bmpData = dividedBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, dividedBitmap.PixelFormat);

                int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(dividedBitmap.PixelFormat) / 8;

                Parallel.For(1, numDivisions, i =>
                {
                    int y = i * divisionSpacing;

                    for (int x = 0; x < width; x++)
                    {
                        for (int j = 0; j < lineThickness; j++)
                        {
                            unsafe
                            {
                                byte* ptr = (byte*)bmpData.Scan0.ToPointer();
                                byte* pixel = ptr + y * bmpData.Stride + (x + j) * bytesPerPixel;
                                pixel[0] = 255;  // B (blue)
                                pixel[1] = 182;  // G (green)
                                pixel[2] = 193;  // R (red)
                            }
                        }
                    }
                });

                dividedBitmap.UnlockBits(bmpData);
            }

            return dividedBitmap;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            bool tmp = false;
            //String imageJPG = @"C:\Users\achim\Desktop\image.jpg";
            String imageJPG = @"C:\Users\achim\Desktop\small.png";
            try
            {
                image = new Bitmap(imageJPG);
                tmp = true;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("An error occured");
            }
            if (tmp)
            {

#if FALSE
         unsafe
         {
             // Dll z Asemblera
             [DllImport(@"C:\Users\achim\Desktop\Laplace\x64\Release\AsmDLL.dll")]
             static extern int BitmapProc(byte* input);
         };
        //Dll z CPP
        [DllImport(@"C:\Users\achim\Desktop\Laplace\x64\Release\CppDLL.dll")]
        static extern int RetVal();
#else
                unsafe
                {
                    // Dll z Asemblera
                    [DllImport(@"C:\Users\achim\Desktop\Laplace\x64\Debug\AsmDLL.dll")]
                    static extern int BitmapProc(byte* input);
                    int width = image.Width;
                    int height = image.Height;
                    BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    IntPtr scan0 = bitmapData.Scan0;
                    int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(PixelFormat.Format24bppRgb) / 8;
                    int numThreads = 300;
                    unsafe
                    {
                        byte* ptr = (byte*)bitmapData.Scan0;
                        Parallel.For(0, numThreads, i_thread =>
                        {
                            int start = i_thread * (height / numThreads);
                            int end = (i_thread + 1) * (height / numThreads);
                            if (i_thread == numThreads - 1)
                                end = height;
                            BitmapProc(ptr + start * width * bytesPerPixel);
                        });
                    }
                    image.UnlockBits(bitmapData);
                    image.Save(@"C:\Users\achim\Desktop\smallO.png");
                };
#endif
                //int threads = 20;
                //Bitmap dividedIMG = DivideBitmap(image, threads, 20,  100);
                //dividedIMG.Save(@"C:\Users\achim\Desktop\output.jpg");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool tmp = false;
            //String imageJPG = @"C:\Users\achim\Desktop\image.jpg";
            String imageJPG = @"C:\Users\achim\Desktop\small.png";
            try
            {
                image = new Bitmap(imageJPG);
                tmp = true;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("An error occured");
            }
            if (tmp)
            {

#if FALSE
         unsafe
         {
             // Dll z Asemblera
             [DllImport(@"C:\Users\achim\Desktop\Laplace\x64\Release\AsmDLL.dll")]
             static extern int BitmapProc(byte* input);
         };
        //Dll z CPP
        [DllImport(@"C:\Users\achim\Desktop\Laplace\x64\Release\CppDLL.dll")]
        static extern int RetVal();
#else
                unsafe
                {
                    // Dll z Asemblera
                    [DllImport(@"C:\Users\achim\Desktop\Laplace\x64\Debug\CppDLL.dll")]
                    static extern int ImageToBlack(byte* start, byte* end);
                    int width = image.Width;
                    int height = image.Height;
                    BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                 //   IntPtr scan0 = bitmapData.Scan0;
                    int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(PixelFormat.Format24bppRgb) / 8;
                    int numThreads = 10;
                    unsafe
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        byte* ptr = (byte*)bitmapData.Scan0;
                        stopwatch.Start();
                        Parallel.For(0, numThreads, i_thread =>
                        {
                            int start = i_thread * (height / numThreads);
                            int end = (i_thread + 1) * (height / numThreads);
                            if (i_thread == numThreads - 1)
                                end = height;
                            ImageToBlack(ptr + start * width * bytesPerPixel, ptr + end * width * bytesPerPixel);
                        });
                        stopwatch.Stop();
                        long elapsed_time = stopwatch.ElapsedMilliseconds;
                        Console.WriteLine("Elapsed time: {0} ms", elapsed_time);
                    }
                    image.UnlockBits(bitmapData);
                    image.Save(@"C:\Users\achim\Desktop\smallO.png");
                };
#endif
            }
        }
    }
}