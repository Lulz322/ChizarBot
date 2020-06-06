﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChizarBot
{
    public class ReadWriter
    {

        public static List<ulong> TakeList(string path)
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

    }
}