using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;

namespace Remapper
{
    class SharePath
    {
        private string _target;
        private Dictionary<string, SharePath> _subpaths;

        public SharePath()
        {
            _subpaths = new Dictionary<string, SharePath>();
        }

        public Dictionary<string, SharePath> SubPaths
        {
            get
            {
                return _subpaths;
            }
        }

        public string Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }

        public SharePath AddPath(string src, string dest)
        {
            SharePath curr = this;

            string[] parts = src.Split('\\');

            foreach (string s in parts)
            {
                string t = s.ToLower();
                if (!curr.SubPaths.ContainsKey(t))
                    curr.SubPaths[t] = new SharePath();

                curr = curr.SubPaths[t];
            }
            curr.Target = dest;
            return curr;
        }

        public string MapPath(string src)
        {
            if (src == null)
                return null;

            src = src.Substring(1).ToLower();
            string[] parts = src.Split('\\');

            SharePath lastmapped = null;
            SharePath current = this;
            int pathidx = -1;

            int i = 0;
            for (i = 0; i < parts.Length; i++)
            {
                if (current.SubPaths.ContainsKey(parts[i]))
                {
                    current = current.SubPaths[parts[i]];
                    if (current.Target != null)
                    {
                        lastmapped = current;
                        pathidx = i;
                    }
                }
                else
                    break;
            }

            string newpath = null;
            if (i == parts.Length)
            {
                newpath = lastmapped.Target;
            }
            else if (i == 0)
                newpath = null;
            else
            {
                newpath = current.Target + "\\" + String.Join("\\", parts, pathidx + 1, parts.Length - pathidx - 1);
            }
            return newpath;
        }
    }
}