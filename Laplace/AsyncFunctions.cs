using System.Runtime.InteropServices;


namespace Laplace
{
    public class AsyncFunctions
    {
        public static volatile List<Tuple<int, int, byte[]>> threadValues;
#if TRUE
// Dll z CPP
        [DllImport(@"C:\Users\Achim\Desktop\studia\Laplace\x64\Release\CppDLL.dll")]
    static extern int ApplyFilterCpp(IntPtr inputArrayPointer, IntPtr outputArrayPointer,
               int bitmapLength, int bitmapWidth, int startIndex, int indicesToFilter);
        // Dll z CPP
        [DllImport(@"C:\Users\Achim\Desktop\studia\Laplace\x64\Release\AsmDLL.dll")]
        static extern int ApplyFilterAsm(IntPtr inputArrayPointer, IntPtr outputArrayPointer,
               int bitmapLength, int bitmapWidth, int startIndex, int indicesToFilter);
#else
        // Dll z CPP
        [DllImport(@"C:\Users\Achim\Desktop\studia\Laplace\x64\Debug\CppDLL.dll")]
    static extern int ApplyFilterCpp(IntPtr inputArrayPointer, IntPtr outputArrayPointer,
               int bitmapLength, int bitmapWidth, int startIndex, int indicesToFilter);
        // Dll z CPP
        [DllImport(@"C:\Users\Achim\Desktop\studia\Laplace\x64\Debug\AsmDLL.dll")]
        static extern int ApplyFilterAsm(IntPtr inputArrayPointer, IntPtr outputArrayPointer,
               int bitmapLength, int bitmapWidth, int startIndex, int indicesToFilter);
#endif

        public static async Task<byte[]> UseAsmAlgorithm(byte[] bitmap, int threads)
        {
            byte[] widthBytes = new byte[4];
            for (int i = 0; i < 4; i++)
                widthBytes[i] = bitmap[i + 18];
            int width = BitConverter.ToInt32(widthBytes, 0) * 3;
            byte[] noHeaderbitmap = new byte[bitmap.Length - 54];
            Array.Copy(bitmap, 54, noHeaderbitmap, 0, noHeaderbitmap.Length);
            int temp = 0;
            int countElems = 0;
            threadValues = new List<Tuple<int, int, byte[]>>();
            for (int i = 0; i < noHeaderbitmap.Length; i += width)
            {
                int idThread = i;
                if (i == threads - 1)
                    countElems = noHeaderbitmap.Length - temp;
                else
                {
                    countElems = noHeaderbitmap.Length / threads;
                    countElems -= countElems % 3;
                }

                temp += countElems;
                byte[] tempArray = new byte[countElems];
                threadValues.Add(new Tuple<int, int, byte[]>(idThread, countElems, tempArray));
            }
            //------------------//
            List<Thread> threadList = new List<Thread>();
            int index = 0;
            for (int i = 0; i < threads; i++)
            {
                int start = index;
                var tuple = threadValues[i];
                var thread = new Thread(() =>
                {
                    var currentOutput = tuple.Item2;
                    var outputCopy = new byte[noHeaderbitmap.Length];
                    unsafe
                    {
                        fixed (byte* inputArrayPointer = &noHeaderbitmap[0])
                        fixed (byte* outputArrayPointer = &outputCopy[0])
                        {
                            var inputArrayPointerInt = new IntPtr(inputArrayPointer);
                            var outputArrayPointerInt = new IntPtr(outputArrayPointer);

                            ApplyFilterAsm(inputArrayPointerInt, outputArrayPointerInt, outputCopy.Length, width, start, tuple.Item2);

                            Marshal.Copy(outputArrayPointerInt, tuple.Item3, 0, tuple.Item2);
                        }
                    }
                });
                thread.Start();
                threadList.Add(thread);
                index += tuple.Item2;
            }
            threadList.ForEach(t => t.Join());

            byte[] outputBitmap = Array.Empty<byte>();

            threadValues.OrderBy(t => t.Item1).ToList().ForEach(t => outputBitmap = outputBitmap.Concat(t.Item3).ToArray());
            byte[] finalOutput = new byte[54 + outputBitmap.Length];
            Array.Copy(bitmap, 0, finalOutput, 0, 54);
            Array.Copy(outputBitmap, 0, finalOutput, 54, outputBitmap.Length);
            return finalOutput;
        }
        public static async Task<byte[]> UseCppAlgorithm(byte[] bitmap, int threads)
        {
            byte[] widthBytes = new byte[4];
            for (int i = 0; i < 4; i++)
                widthBytes[i] = bitmap[i + 18];
            int width = BitConverter.ToInt32(widthBytes, 0) * 3;
            byte[] noHeaderbitmap = new byte[bitmap.Length - 54];
            Array.Copy(bitmap, 54, noHeaderbitmap, 0, noHeaderbitmap.Length);
            int temp = 0;
            int countElems = 0;
            threadValues = new List<Tuple<int, int, byte[]>>();
            for (int i = 0; i < noHeaderbitmap.Length; i += width)
            {
                int idThread = i;
                if (i == threads - 1)
                    countElems = noHeaderbitmap.Length - temp;
                else
                {
                    countElems = noHeaderbitmap.Length / threads;
                    countElems -= countElems % 3;
                }

                temp += countElems;
                byte[] tempArray = new byte[countElems];
                threadValues.Add(new Tuple<int, int, byte[]>(idThread, countElems, tempArray));
            }
            //------------------//
            List<Thread> threadList = new List<Thread>();
            int index = 0;
            for (int i = 0; i < threads; i++)
            {
                int start = index;
                var tuple = threadValues[i];
                var thread = new Thread(() =>
                {
                    var currentOutput = tuple.Item2;
                    var outputCopy = new byte[noHeaderbitmap.Length];
                    unsafe
                    {
                        fixed (byte* inputArrayPointer = &noHeaderbitmap[0])
                        fixed (byte* outputArrayPointer = &outputCopy[0])
                        {
                            var inputArrayPointerInt = new IntPtr(inputArrayPointer);
                            var outputArrayPointerInt = new IntPtr(outputArrayPointer);

                            ApplyFilterCpp(inputArrayPointerInt, outputArrayPointerInt, outputCopy.Length, width, start, tuple.Item2);

                            Marshal.Copy(outputArrayPointerInt, tuple.Item3, 0, tuple.Item2);
                        }
                    }
                });
                thread.Start();
                threadList.Add(thread);
                index += tuple.Item2;
            }
            threadList.ForEach(t => t.Join());

            byte[] outputBitmap = Array.Empty<byte>();

            threadValues.OrderBy(t => t.Item1).ToList().ForEach(t => outputBitmap = outputBitmap.Concat(t.Item3).ToArray());
            byte[] finalOutput = new byte[54 + outputBitmap.Length];
            Array.Copy(bitmap, 0, finalOutput, 0, 54);
            Array.Copy(outputBitmap, 0, finalOutput, 54, outputBitmap.Length);
            return finalOutput;
        }
    }


}
