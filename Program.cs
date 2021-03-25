using System;
using System.IO;


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

            ImageBuilder imageBuilder = new ImageBuilder(imageFolder, 100, 100, 1000);
            imageBuilder.Build();

            Console.WriteLine("Enter any key to start deleting");
            Console.ReadLine();

            foreach (FileInfo file in imageFolder.GetFiles())
            {
                file.Delete();
            }
            
        }
    }

   
}
