using System;
using System.Drawing;
using System.Numerics;

namespace SIMDExperiment
{
    class SIMD
    {
        static public int Test(byte[] bytes, byte color)
        {
            int cap = Vector<byte>.Count; //Calculate the size of the SIMD registers (512 etc)
            int calculationIndex = 0; //Trailing Index

            byte[] colorbytes = new byte[cap];
            Array.Fill(colorbytes, color);
            Vector<byte> colors = new Vector<byte>(colorbytes);

            foreach(byte b in colorbytes)
            {
                Console.Write(b + ", ");
            }

            DateTime timetaken = DateTime.Now;
            for (int i = 0; i < bytes.Length; i += cap)
            {
                Vector<byte> v1 = new(bytes, calculationIndex);
                (v1 + colors).CopyTo(bytes, calculationIndex);

                calculationIndex = i;
            }
            Console.WriteLine(bytes[0].ToString() + " | " + bytes.Length.ToString() + "\n");
            return (DateTime.Now.Millisecond - timetaken.Millisecond);
        }
    }
}

