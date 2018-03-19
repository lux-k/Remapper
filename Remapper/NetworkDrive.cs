using System;

namespace Remapper
{
    class NetworkDrive
    {
        private string _driveLetter, _server, _originalServer, _originalFullPath, _originalPath;

        /// <summary>
        /// Constructor to create a new network drive
        /// </summary>
        /// <param name="drive">Drive letter, e.g. D:\</param>
        /// <param name="path">Full UNC path, e.g. \\server\path\to\file</param>
        public NetworkDrive(string drive, string path)
        {
            this.DriveLetter = drive;
            this.OriginalFullPath = path;
        }
        /// <summary>
        /// The local drive letter.
        /// </summary>
        public string DriveLetter
        {
            get
            {
                return _driveLetter;
            }
            set
            {
                string s = value;
                if (s.Contains("\\"))
                {
                    s = s.Substring(0, s.IndexOf("\\"));
                }
                _driveLetter = s;
            }
        }

        /// <summary>
        /// The normalized name of the server (looked up via the ServerMappings.txt file).
        /// </summary>
        public string Server
        {
            get
            {
                return _server;
            }
        }

        /// <summary>
        /// The server name the share originally pointed to.
        /// </summary>
        public string OriginalServer
        {
            get
            {
                return _originalServer;
            }
        }

        /// <summary>
        /// The original mapped path of the server.
        /// </summary>
        public string OriginalPath
        {
            get
            {
                return _originalPath;
            }
            set
            {
                _originalPath = value;
            }
        }

        /// <summary>
        /// The original, full UNC path of the share.
        /// </summary>
        public string OriginalFullPath
        {
            get
            {
                return _originalFullPath;
            }
            set
            {
                _originalFullPath = value;
                string s;
                string p;

                int spot = _originalFullPath.IndexOf('\\', 3);
                s = _originalFullPath.Substring(2, spot - 2);
                p = _originalFullPath.Substring(spot + 1);
                _originalServer = s;
                _originalPath = p;
                _server = ServerMapper.NormalizeServerName(s);
            }
        }

        public string NormalizedFullPath
         {
            get
            {
                return "\\\\" + _server + "\\" + _originalPath;
            }
        }
    }
}