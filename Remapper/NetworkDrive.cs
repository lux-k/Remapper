using System;

namespace Remapper
{
    class NetworkDrive
    {
        private string _driveLetter, _server, _originalServer, _originalFullPath, _originalPath;

        public NetworkDrive(string s, string path)
        {
            this.DriveLetter = s;
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
                _driveLetter = value;
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
    }
}