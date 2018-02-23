using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;

namespace Remapper
{
    class ShareMapper
    {
        private static SharePath root;

        public static string Map( string s )
        {
            if (root == null)
                InitMap();

            return root.MapPath(s);
        }

        private static void InitMap()
        {
            root = new SharePath();
            try
            {
                Assembly _assembly;
                StreamReader tsr;
                _assembly = Assembly.GetExecutingAssembly();
                tsr = new StreamReader(_assembly.GetManifestResourceStream(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".ShareMappings.txt"));

                while (!tsr.EndOfStream)
                {
                    string line = tsr.ReadLine();
                    if (line.StartsWith("#"))
                        continue;

                    if (line.Equals(""))
                        continue;

                    string src = line;

                    line = tsr.ReadLine();
                    string dest = line;

                    src = src.Substring(1);

                    if (root == null)
                        root = new SharePath();

                    SharePath tar = root.AddPath(src, dest);
                }
            }
            catch
            {
                Console.WriteLine("oops");
            }

        }
    }
}
