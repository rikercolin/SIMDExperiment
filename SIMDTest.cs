using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Threading;

namespace SIMDExperiment
{
    class SIMDTest
    {
        private DateTime start;
        private DirectoryInfo imageFolder;
        private bool save;

        public SIMDTest(DirectoryInfo imageFolder, bool save)
        {
            this.imageFolder = imageFolder;
            this.save = save;
        }

        public void Start(Vector<byte> color)
        {
            var countdown = new CountdownEvent(imageFolder.GetFiles().Length);
            start = DateTime.Now;
            int i = 1;

            foreach (FileInfo file in imageFolder.EnumerateFiles())
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    ProcessingTask(this, file, color);
                    countdown.Signal();
                });
            }

            while (countdown.CurrentCount != 0)
            {
                Thread.Sleep(200);
                Console.WriteLine("Files left to process:{0}", countdown.CurrentCount);
            }
            Console.WriteLine("Time taken for SIMD test: {0}", DateTime.Now.Subtract(start).ToString());
        }

        private void ProcessingTask(object obj, FileInfo file, Vector<Byte> color)
        {
            Bitmap bmp = new Bitmap(file.FullName);
            Rectangle rectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpdata = bmp.LockBits(rectangle, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpdata.Scan0;
            int bytes = Math.Abs(bmpdata.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);


            int cap = Vector<byte>.Count;
            int calculationIndex = 0;
            int count = 0;

            for (int i = 0; i < rgbValues.Length; i++)
            {
                if (count < cap)
                {
                    count++;
                }
                else if (count == cap || (i + 1) == rgbValues.Length)
                {
                    Vector<byte> v1 = new Vector<byte>(rgbValues, calculationIndex);
                    (v1 + color).CopyTo(rgbValues, calculationIndex);

                    calculationIndex = i;
                    count = 0;
                }
            }


            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpdata);

            if (save)
            {
                string newname = file.DirectoryName + "\\simd-" + file.Name;
                bmp.Save(newname);
            }
        }
    }
}
