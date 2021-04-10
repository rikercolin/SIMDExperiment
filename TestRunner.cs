using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace SIMDExperiment
{
    public struct RawBitMap
    {
        public RawBitMap(FileInfo file, Bitmap bmp, BitmapData bmpData, IntPtr ptr, byte[] array, int length)
        {
            Bmp = bmp;
            Ptr = ptr;
            ByteArray = array;
            Length = length;
            FileInformation = file;
            BmpData = bmpData;
        }

        public FileInfo FileInformation { get; }
        public Bitmap Bmp { get; }
        public BitmapData BmpData { get; }
        public IntPtr Ptr { get; }
        public byte[] ByteArray { get; }
        public int Length { get; }
    }

    class TestRunner
    {
        private DateTime start;
        private DirectoryInfo imageFolder;
        private bool save;
        private byte color;

        public TestRunner(DirectoryInfo imageFolder, bool save, byte color)
        {
            this.imageFolder = imageFolder;
            this.save = save;
            this.color = color;
        }

        public void Start(Func<byte[], byte, int> test, bool threading, bool save)
        {
            var countdown = new CountdownEvent(imageFolder.GetFiles().Length);
            start = DateTime.Now;

            if (threading)
            {
                foreach (FileInfo file in imageFolder.EnumerateFiles())
                {
                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        var rBmp = BitmapToByteArray(file);
                        test(rBmp.ByteArray, color);
                        ByteArrayToBitmap(rBmp, save, test.Method.DeclaringType.Name);
                        countdown.Signal();
                    });
                }

                while (countdown.CurrentCount != 0)
                {
                    Thread.Sleep(200);
                    Program.FixedDisplay("Files left to process: " + countdown.CurrentCount);
                }
                Console.WriteLine("Time taken for the test: {0}", DateTime.Now.Subtract(start).ToString());
            }
            else
            {
                //TODO
            }
        }

        static public RawBitMap BitmapToByteArray(FileInfo file)
        {
            Bitmap bmp = new Bitmap(file.FullName);
            Rectangle rectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpdata = bmp.LockBits(rectangle, ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr = bmpdata.Scan0;
            int length = Math.Abs(bmpdata.Stride) * bmp.Height;
            byte[] rgbValues = new byte[length];

            Marshal.Copy(ptr, rgbValues, 0, length); //Copy Data into a nice neat array

            return new RawBitMap(file, bmp, bmpdata, ptr, rgbValues, length);

        }

        static public void ByteArrayToBitmap(RawBitMap rBmp, bool save, string testname)
        {
            Marshal.Copy(rBmp.ByteArray, 0, rBmp.Ptr, rBmp.Length);
            rBmp.Bmp.UnlockBits(rBmp.BmpData);

            if (save) {
                var testFolder = new DirectoryInfo(rBmp.FileInformation.DirectoryName + "\\" + testname);
                if (!testFolder.Exists) System.IO.Directory.CreateDirectory(rBmp.FileInformation.DirectoryName + "\\" + testname);
                rBmp.Bmp.Save(rBmp.FileInformation.DirectoryName + "\\" + testname + "\\" + rBmp.FileInformation.Name);
            } 
        }
    }
}
