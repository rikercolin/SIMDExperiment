using System;
using System.IO;
using System.Numerics;


namespace SIMDExperiment
{
    class Program
    {
        const string imageFolderName = "rawimages";

        public static DirectoryInfo imageFolder;

        static void Main(string[] args)
        {
            imageFolder = new DirectoryInfo(imageFolderName);
            if (!imageFolder.Exists) System.IO.Directory.CreateDirectory(imageFolderName);

            Console.WriteLine("Is SIMD enabled: " + Vector.IsHardwareAccelerated);
            Console.WriteLine("SIMD Int size, Counter:{0}", Vector<byte>.Count);

            Console.WriteLine("Press any key to start IMAGE GEN");
            Console.ReadLine();

            // Specs for image creation and image creation
            ImageBuilder imageBuilder = new ImageBuilder(imageFolder, (32 * 100), (32 * 100), 100);
            imageBuilder.Build();

            Console.WriteLine("Press any key to start TEST");
            Console.ReadLine();

            byte[] colorbytes = new byte[32];
            Array.Fill<byte>(colorbytes, 100);
            Vector<byte> color = new Vector<byte>(colorbytes);



            //Run Tests
            var sisd = new SISDTest(imageFolder, true);
            //sisd.Start();

            var simd = new SIMDTest(imageFolder, true);
            simd.Start(color);

            //Cleanup();
        }

        static void Cleanup()
        {
            Console.WriteLine("Enter any key to start Clean up");
            Console.ReadLine();

            foreach (FileInfo file in imageFolder.GetFiles())
            {
                file.Delete();
            }
        }
    }
}
