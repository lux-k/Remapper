using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Remapper
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Log("Program log start: " + DateTime.Now);
            Logger.Log("Machine name: " + Environment.MachineName);
            Logger.Log("User name:" + Environment.UserName);
            Logger.Log("");
            SharePath s = ShareMapper.Map;
            Logger.Log("Network drives:");
            foreach (NetworkDrive d in GetNetworkDrives())
            {
                if (d.Server != null)
                {
                    Logger.Log("\t" + d.DriveLetter + " " + d.OriginalPath + " <-> " + s.MapPath(d.OriginalPath));
                }

            }

            Console.ReadLine();
        }

        public static NetworkDrive[] GetNetworkDrives()
        {
            ArrayList a = new ArrayList();

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady && d.DriveType == DriveType.Network)
                {
                    NetworkDrive drive = new NetworkDrive();
                    drive.DriveLetter = d.Name;
                    drive.OriginalPath = GetUNCPath(d.Name);

                    a.Add(drive);
                }
            }
            return (NetworkDrive[])a.ToArray(typeof(NetworkDrive));
        }

        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection(
                    [MarshalAs(UnmanagedType.LPTStr)] string localName,
                    [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
                    ref int length);
        /// <summary>
        /// Given a path, returns the UNC path or the original. (No exceptions
        /// are raised by this function directly). For example, "P:\2008-02-29"
        /// might return: "\\networkserver\Shares\Photos\2008-02-09"
        /// </summary>
        /// <param name="originalPath">The path to convert to a UNC Path</param>
        /// <returns>A UNC path. If a network drive letter is specified, the
        /// drive letter is converted to a UNC or network path. If the 
        /// originalPath cannot be converted, it is returned unchanged.</returns>
        public static string GetUNCPath(string originalPath)
        {
            StringBuilder sb = new StringBuilder(512);
            int size = sb.Capacity;

            // look for the {LETTER}: combination ...
            if (originalPath.Length > 2 && originalPath[1] == ':')
            {
                // don't use char.IsLetter here - as that can be misleading
                // the only valid drive letters are a-z && A-Z.
                char c = originalPath[0];
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    int error = WNetGetConnection(originalPath.Substring(0, 2),
                        sb, ref size);
                    if (error == 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(originalPath);

                        string path = Path.GetFullPath(originalPath)
                            .Substring(Path.GetPathRoot(originalPath).Length);
                        return Path.Combine(sb.ToString().TrimEnd(), path);
                    }
                }
            }

            return originalPath;
        }
    }
}
