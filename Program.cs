using System;
using System.IO;
using System.Numerics;

namespace SIMDExperiment
{
    class Program
    {
        const string imageFolderName = "testimages";
        public static DirectoryInfo imageFolder;

        static void Main(string[] args)
        {


            imageFolder = new DirectoryInfo(imageFolderName);
            if (!imageFolder.Exists) System.IO.Directory.CreateDirectory(imageFolderName);

            Console.WriteLine("Is SIMD enabled? " + Vector.IsHardwareAccelerated);

            Console.WriteLine("Press any key to start");
            Console.ReadLine();

            // Specs for image creation and image creation
            ImageBuilder imageBuilder = new ImageBuilder(imageFolder, 100, 100, 1000);
            imageBuilder.Build();

            //Run Tests
            

            Cleanup();
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

    class SIMDTest
    {

    }


}
