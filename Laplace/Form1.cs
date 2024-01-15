using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.IO;

namespace Laplace
{
    public partial class Form1 : Form
    {
        public String imagePath = @"C:\Users\Achim\Desktop\studia\Laplace\small.bmp";
        public String finalPath = @"C:\Users\Achim\Desktop\studia\Laplace\smallO.bmp";
        public int threads = 20;
        Bitmap image;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private async Task button1_ClickAsync(object sender, EventArgs e)
        {
            bool tmp = false;
            try
            {
                image = new Bitmap(imagePath);
                tmp = true;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("An error occured");
            }
            if (tmp)
            {
                byte[] bitmap = File.ReadAllBytes(imagePath);
                byte[] final = await AsyncFunctions.UseAsmAlgorithm(bitmap, threads);
                File.WriteAllBytes(finalPath, final);
            }
        }

        private async Task button2_ClickAsync(object sender, EventArgs e)
        {
            bool tmp = false;
            try
            {
                image = new Bitmap(imagePath);
                tmp = true;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("An error occured");
            }
            if (tmp)
            {
                byte[] bitmap = File.ReadAllBytes(imagePath);
                byte[] final = await AsyncFunctions.UseCppAlgorithm(bitmap, threads);
                File.WriteAllBytes(finalPath, final);
            }
        }
    }
}