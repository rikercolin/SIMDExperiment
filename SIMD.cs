using System;
using System.Numerics;

namespace SIMDExperiment
{
    class SIMD
    {
        static public int Test(byte[] bytes, byte color)
        {
            int cap = Vector<byte>.Count; //Calculate the size of the SIMD registers (512 etc)
            int calculationIndex = 0; //Trailing Index
            Vector<byte> colors = new Vector<byte>(color);

            byte[] output = new byte[bytes.Length];

            DateTime timetaken = DateTime.Now;
            for (int i = 0; i < bytes.Length; i += cap)
            {
                Vector<byte> v1 = new(bytes, calculationIndex);
                Vector.Add(v1, colors).CopyTo(output, calculationIndex);
                calculationIndex = i;
            }
            output.CopyTo(bytes, 0); //VERY needed, the price to pay for mutating the orginal data structure :3
            return (DateTime.Now.Millisecond - timetaken.Millisecond);
        }
    }
}