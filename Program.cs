using System;
using System.Diagnostics;
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


            var simdFile = CreateRecordFile("SIMD");
            var sisdFile = CreateRecordFile("SISD");
            ContinuePrompt("Ready to start testing");

            int numberOfImages = 10;
            for (int i = 0; i < 10; i++)
            {
                // Specs for image creation and image creation
                ImageBuilder imageBuilder = new(imageFolder, 32 * ((int)Math.Pow(2,i)), 32 * ((int)Math.Pow(2, i)), numberOfImages);
                imageBuilder.Build();
                GC.Collect();
                Console.WriteLine(i + ", Image Creation Stage Done");

                byte color = 127; //Invert basically

                //Run Tests
                var runner = new TestRunner(imageFolder, false, color);

                RecordResult(simdFile, runner.Start(SIMD.Test, true, true, numberOfImages), i);
                GC.Collect();
                Console.WriteLine(i + ", SIMD Test Stage Done");

                RecordResult(sisdFile, runner.Start(SISD.Test, true, true, numberOfImages), i);
                GC.Collect();
                Console.WriteLine(i + ", SISD Test Stage Done");

                Cleanup();
                Console.WriteLine(i + ", Cleanup Stage Done");
                Console.WriteLine("\n------------------------\n");
            }
        }

        static void Cleanup()
        {
            foreach (FileInfo file in imageFolder.GetFiles())
            {
                if (file.Extension == ".bmp")
                {
                    bool notdeletedyet = true;
                    while(notdeletedyet)
                    {
                        try
                        {
                            file.Delete();
                            notdeletedyet = false;
                        }
                        catch (Exception e)
                        {
                            notdeletedyet = true;
                        }
                    }
                }
                   
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

        public static void RecordResult(FileInfo file, TimeSpan time, int size)
        {
            using (var writer = new StreamWriter(file.FullName, true))
            {
                writer.Write(time.TotalMilliseconds + "," + size + "\r\n");
            }
        }

        public static FileInfo CreateRecordFile(string name)
        {
            string fullfilename = name + ".csv";
            string filepath = imageFolderName + "\\" + fullfilename;

            using (var file = new FileStream(filepath, FileMode.Create))
            {
            }

            return new FileInfo(filepath);
        }

        private static FileStream GetWriteStream(string path, int timeoutMs)
        {
            var time = Stopwatch.StartNew();
            while (time.ElapsedMilliseconds < timeoutMs)
            {
                try
                {
                    return new FileStream(path, FileMode.Create, FileAccess.Write);
                }
                catch (IOException e)
                {
                    // access error
                    if (e.HResult != -2147024864)
                        throw;
                }
            }

            throw new TimeoutException($"Failed to get a write handle to {path} within {timeoutMs}ms.");
        } // Credit to Almund Stack Overflow
    }
}
