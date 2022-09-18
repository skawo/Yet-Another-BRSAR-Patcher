using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace YABRSARP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> Files = Directory.GetFiles(Environment.CurrentDirectory).ToList();

            string brsar = Files.FirstOrDefault(x => x.EndsWith(".brsar"));

            if (brsar == null)
            {
                Console.WriteLine("No BRSAR found");
                return;
            }

            byte[] brsarF = File.ReadAllBytes(brsar);

            Files.Remove(brsar);

            foreach (string file in Files)
            {
                int pos = FindBytes(brsarF, Encoding.ASCII.GetBytes(Path.GetFileName(file)));

                if (pos < 0)
                {
                    Console.WriteLine($"{file} not found in BRSAR.");
                    continue;
                }

                pos -= 0x1B;
                pos -= 0x4;

                brsarF[pos] = 0x7F;
                brsarF[pos + 1] = 0xFF;
                brsarF[pos + 2] = 0xFF;
                brsarF[pos + 3] = 0xFF;


                Console.WriteLine($"Updated {file}");
            }

            Console.WriteLine($"Done.");

            File.WriteAllBytes(brsar + "_out", brsarF);
            return;
        }

        static int FindBytes(byte[] barray, byte[] tofind)
        {
            int mf = barray.Length - tofind.Length + 1;
            for (int i = 0; i < mf; i++)
            {
                if (barray[i] != tofind[0]) 
                    continue;

                for (int j = tofind.Length - 1; j >= 1; j--)
                {
                    if (barray[i + j] != tofind[j]) break;
                    if (j == 1) return i;
                }
            }
            return -1;
        }
    }
}
