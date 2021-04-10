using System;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SIMDExperiment
{
    class ImageBuilder
    {
        private readonly DirectoryInfo imageFolder;
        private readonly int imageWidth;
        private readonly int imageHeight;
        private readonly int numberOfImages;

        public ImageBuilder(DirectoryInfo folder, int width, int height, int number)
        {
            this.imageFolder = folder;
            this.imageWidth = width;
            this.imageHeight = height;
            this.numberOfImages = number;
        }

        public void Build()
        {
            var countdown = new CountdownEvent(numberOfImages);
            for (int i = 0; i < numberOfImages; i++)
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    BuildTask(this);
                    countdown.Signal();
                });
            }

            while (countdown.CurrentCount > 0)
            {
                Thread.Sleep(200);

                Program.FixedDisplay("Image creation in progress:" + numberOfImages + "/" + (numberOfImages - countdown.CurrentCount));
            }

            Console.WriteLine("Image creation is done!");
        }

        private void BuildTask(object obj)
        {
            //Create the Structure of an image
            PixelFormat pf = PixelFormats.Bgr32;
            int rawStride = (this.imageWidth * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * this.imageHeight];

            //Fill with random noise
            Random random = new Random();
            random.NextBytes(rawImage);

            //Create the bitmap
            BitmapSource bitmap = BitmapSource.Create(this.imageWidth, this.imageHeight, 96, 96, pf, null, rawImage, rawStride);
            string name = Guid.NewGuid() + ".bmp";
            string url = imageFolder.Name + "\\" + name;

            //Write the bitmap
            using var filestream = new FileStream(url, FileMode.Create);
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(filestream);
            filestream.Close();
        }
    }
}
