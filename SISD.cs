using System;

namespace SIMDExperiment
{
    class SISD
    {
        static public int Test(byte[] bytes, byte color)
        {
            DateTime timetaken = DateTime.Now;
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] += color;
            }
            return (DateTime.Now.Millisecond - timetaken.Millisecond);
        }
    }
}

