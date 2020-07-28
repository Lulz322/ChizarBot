using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    public class ReadWriter
    {
        public async static Task RemoveObj(ulong id, string path)
        {
            List<string> FromFile = new List<string>();
            FromFile = TakeStringList(path);
            foreach (string it in FromFile)
            {
                string[] strtmp = it.Split('|');
                if (Convert.ToUInt64(strtmp[0]) == id)
                {
                    FromFile.Remove(it);
                    break;
                }
            }

            try
            {
                using (StreamWriter sr = new StreamWriter(path))
                {
                    foreach (string it in FromFile)
                    {
                        await sr.WriteAsync(it + "\n");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
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