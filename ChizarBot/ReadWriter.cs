using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChizarBot
{
    public class ReadWriter
    {

        public static List<ulong> TakeUintList(string path)
        {
            List<ulong> FromFile = new List<ulong>();
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                        FromFile.Add(Convert.ToUInt64(sr.ReadLine()));
                    sr.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return FromFile;
        }
        public static List<string> TakeStringList(string path)
        {
            List<string> FromFile = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                        FromFile.Add(sr.ReadLine());
                    sr.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return FromFile;
        }

    }
}