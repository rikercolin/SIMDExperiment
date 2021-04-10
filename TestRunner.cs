using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

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

        public TimeSpan Start(Func<byte[], byte, int> test, bool threading, bool save, int size)
        {
            var countdown = new CountdownEvent(size);
            start = DateTime.Now;

            if (threading)
            {
                foreach (FileInfo file in imageFolder.EnumerateFiles())
                {
                    if (file.Extension == ".bmp")
                    {
                        ThreadPool.QueueUserWorkItem(x =>
                        {
                            var rBmp = BitmapToByteArray(file);
                            test(rBmp.ByteArray, color);
                            ByteArrayToBitmap(rBmp, save, test.Method.DeclaringType.Name);
                            countdown.Signal();
                        });
                    }
                }
                countdown.Wait();

                return DateTime.Now.Subtract(start);
            }
            else
            {
                return DateTime.Now.Subtract(DateTime.Now);
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

            if (save)
            {
                var testFolder = new DirectoryInfo(rBmp.FileInformation.DirectoryName + "\\" + testname);
                if (!testFolder.Exists) System.IO.Directory.CreateDirectory(rBmp.FileInformation.DirectoryName + "\\" + testname);
                rBmp.Bmp.Save(rBmp.FileInformation.DirectoryName + "\\" + testname + "\\" + rBmp.FileInformation.Name);
            }
        }
    }
}
