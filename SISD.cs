using System;

namespace SIMDExperiment
{
    class SISD
    {
        static public int Test(byte[] bytes, byte color)
        {
            DateTime timetaken = DateTime.Now;
            Console.WriteLine("Orginal Byte: " + bytes[0] + ", Result: " + (bytes[0] + color));
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] += color;
            }
            Console.WriteLine(bytes[0].ToString() + " | " + bytes.Length.ToString() + "\n");
            return (DateTime.Now.Millisecond - timetaken.Millisecond);
        }
    }
}

