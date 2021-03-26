using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;

namespace SIMDExperiment
{
    class SISDTest
    {
        private DateTime start;
        private DirectoryInfo imageFolder;
        private bool save;

        public SISDTest(DirectoryInfo imageFolder, bool save)
        {
            this.save = save;
            this.imageFolder = imageFolder;
        }


        public void Start()
        {
            var countdown = new CountdownEvent(imageFolder.GetFiles().Length);
            start = DateTime.Now;

            foreach (FileInfo file in imageFolder.EnumerateFiles())
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    ProcessingTask(this, file);
                    countdown.Signal();
                });
            }

            while (countdown.CurrentCount != 0)
            {
                Thread.Sleep(200);
                Console.WriteLine("Files left to process:{0}", countdown.CurrentCount);
            }
            Console.WriteLine("Time taken for SISD test: {0}", DateTime.Now.Subtract(start).ToString());
        }

        private void ProcessingTask(object obj, FileInfo file)
        {
            Bitmap bmp = new(file.FullName);
            int x, y;

            for (x = 0; x < bmp.Width; x++)
            {
                for (y = 0; y < bmp.Height; y++)
                {
                    var currentcolor = bmp.GetPixel(x, y);
                    bmp.SetPixel(x, y, Color.FromArgb(255, 0, 0));
                }
            }

            if (save)
            {
                string newname = file.DirectoryName + "\\sisd-" + file.Name;
                bmp.Save(newname);
            }

        }
    }
}
