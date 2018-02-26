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
        // help from http://www.blackwasp.co.uk/MapDriveLetter.aspx

        static void Main(string[] args)
        {
            Logger.Log("----");
            Logger.Log("Program log start: " + DateTime.Now);
            Logger.Log("Machine name: " + Environment.MachineName);
            Logger.Log("User name: " + Environment.UserName);
            Logger.Log("");
            Logger.Log("Network drives:");
            int count = 0;
            foreach (NetworkDrive d in GetNetworkDrives())
            {
                if (d.OriginalServer != null)
                {
                    string newpath = ShareMapper.Map(d.OriginalFullPath);
                    if (count == 0)
                        Logger.Log("\t------------------------------------");
                    Logger.Log("\tDrive: " + d.DriveLetter + "\n\tOriginal: " + d.OriginalFullPath + "\n\tMapped: " + newpath );
                    if (!newpath.ToLower().Equals(d.OriginalFullPath.ToLower()))
                    {
                        Remap(d, newpath);
                    }
                    Logger.Log("\t------------------------------------");
                    count++;
                }

            }
            Logger.Log("Run complete: " + DateTime.Now);
            Logger.Log("----\n");
            Console.ReadLine();
        }

        public static int Unmap(NetworkDrive d)
        {
            
            uint result = WNetCancelConnection2(d.DriveLetter, CONNECT_UPDATE_PROFILE, true);
            Logger.Log("\t\tUnmap " + d.DriveLetter + "; result: " + TransResult(result));
            return (int)result;
        }

        public static int Map(NetworkDrive d, string path)
        {
            NETRESOURCE networkResource = new NETRESOURCE();
            networkResource.dwType = RESOURCETYPE_DISK;
            networkResource.lpLocalName = d.DriveLetter;
            networkResource.lpRemoteName = path;
            networkResource.lpProvider = null;

            uint result = WNetAddConnection2(ref networkResource, null, null, CONNECT_UPDATE_PROFILE);
            Logger.Log("\t\tMap " + d.DriveLetter + " to " + path+ "; result: " + TransResult(result));

            return (int)result;
        }

        public static string TransResult(uint res)
        {
            if (res == 0)
                return "0 (success)";
            else
                return res.ToString() + " (failure)";
        }

        public static void Remap(NetworkDrive d, string s)
        {
            Logger.Log("\t\tAttempting to remap " + d.DriveLetter + "...");
            if (Unmap(d) == 0)
            {
                if (Map(d, s) != 0)
                {
                    Logger.Log("\t\tMapping failed; reverting to previous mapping...");
                    Map(d, d.OriginalFullPath);
                }

            }
            else
                Logger.Log("\t\tUnmapping failed.");
        }

        public static NetworkDrive[] GetNetworkDrives()
        {
            ArrayList a = new ArrayList();

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady && d.DriveType == DriveType.Network)
                {
                    a.Add(new NetworkDrive(d.Name, GetUNCPath(d.Name)));
                }
            }
            return (NetworkDrive[])a.ToArray(typeof(NetworkDrive));
        }

        const uint CONNECT_UPDATE_PROFILE = 0x1;
        const uint CONNECT_INTERACTIVE = 0x8;
        const uint CONNECT_PROMPT = 0x10;
        const uint CONNECT_REDIRECT = 0x80;
        const uint CONNECT_COMMANDLINE = 0x800;
        const uint CONNECT_CMD_SAVECRED = 0x1000;

        [StructLayout(LayoutKind.Sequential)]
        public struct NETRESOURCE
        {
            public uint dwScope;
            public uint dwType;
            public uint dwDisplayType;
            public uint dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }

        const uint RESOURCETYPE_DISK = 1;

        [DllImport("mpr.dll")]
        static extern UInt32 WNetAddConnection2(ref NETRESOURCE lpNetResource, string lpPassword, string lpUsername, uint dwFlags);

        [DllImport("mpr.dll")]
        static extern uint WNetCancelConnection2(string lpName, uint dwFlags, bool bForce);

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
        /// from https://gist.github.com/ambyte/01664dc7ee576f69042c
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
