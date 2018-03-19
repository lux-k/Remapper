using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;

namespace Remapper
{
    class ServerMapper
    {
        private static Dictionary<string, ArrayList> _map;
        public static Dictionary<string, ArrayList> Map
        {
            get
            {
                if (_map == null)
                {
                    InitMap();
                }
                return _map;
            }
        }

        private static void InitMap()
        {
            _map = new Dictionary<string, ArrayList>();
            try
            {
                Assembly _assembly;
                StreamReader tsr;
                _assembly = Assembly.GetExecutingAssembly();
                tsr = new StreamReader(_assembly.GetManifestResourceStream(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".ServerMappings.txt"));
                ArrayList names = new ArrayList();
                string server = null;
                while (!tsr.EndOfStream)
                {
                    string line = tsr.ReadLine();
                    if (line.StartsWith("\t"))
                    {
                        line = line.Remove(0, 1);
                        names.Add(line);
                    }
                    else
                    {
                        if (server != null)
                            _map[server] = names;

                        names = new ArrayList();
                        server = line;
                    }
                }

                _map[server] = names;
            }
            catch
            {
                Console.WriteLine("oops");
            }

        }
        public static string NormalizeServerName(string input)
        {
            foreach(string s in Map.Keys)
            {
                foreach(string alias in Map[s])
                {
                    if (alias.ToLower().Equals(input.ToLower()))
                    {
                        return s;
                    }
                }
            }
            return input;
        }
    }
}
