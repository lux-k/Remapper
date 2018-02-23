using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remapper
{
    class Logger
    {
        private static StreamWriter sw = null;
        private static void Init()
        {
            sw = File.AppendText(Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\remapper.txt");
        }

        public static void Log(string s)
        {
            if (sw == null)
                Init();

            Console.WriteLine(s);
            //sw.WriteLine(s);
        }

        ~Logger()
        {
            if (sw != null)
                sw.Close();
        }
    }
}
