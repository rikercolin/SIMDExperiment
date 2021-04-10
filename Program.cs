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

            Console.WriteLine("SIMD System Stats");
            Console.Write("Is SIMD enabled: " + Vector.IsHardwareAccelerated);
            Console.WriteLine("| SIMD byte size, Counter:{0}", Vector<byte>.Count);

            ContinuePrompt("Ready to start image generation");

            // Specs for image creation and image creation
            ImageBuilder imageBuilder = new(imageFolder, (32 * 512), (32 * 512), 10);
            //imageBuilder.Build();
            //GC.Collect();

            ContinuePrompt("Ready to start tests");

            byte color = 127; //Invert basically

            //Run Tests
            var runner = new TestRunner(imageFolder, false, color);
            
            runner.Start(SIMD.Test, true, false);
            GC.Collect();
            ContinuePrompt();
            runner.Start(SISD.Test, true, false);
            GC.Collect();

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

        public static void FixedDisplay(string message)
        {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            Console.SetCursorPosition(2, Console.WindowHeight - 2);
            Console.WriteLine(message);
            Console.SetCursorPosition(left, top);
        }

        public static void ContinuePrompt()
        {
            Console.CursorVisible = true;
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
            Console.CursorVisible = false;
        }

        public static void ContinuePrompt(String message)
        {
            Console.CursorVisible = true;
            Console.WriteLine(message + "\nPress any key to continue");
            Console.ReadLine();
            Console.CursorVisible = false;
        }
    }
}
