using System;

namespace Remapper
{
    class NetworkDrive
    {
        private string _driveLetter, _server, _originalServer, _serverPath, _fullPath, _originalPath;

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

        public string Server
        {
            get
            {
                return _server;
            }
        }

        public string OriginalServer
        {
            get
            {
                return _originalServer;
            }
        }

        public string ServerPath
        {
            get
            {
                return _serverPath;
            }
            set
            {
                _serverPath = value;
            }
        }

        public string FullPath
        {
            get
            {
                return _fullPath;
            }
            set
            {
                _fullPath = value;
            }
        }

        public string OriginalPath
        {
            get
            {
                return _originalPath;
            }
            set
            {
                _originalPath = value;
                string s;
                string p;

                int spot = _originalPath.IndexOf('\\', 3);
                s = _originalPath.Substring(2, spot - 2);
                p = _originalPath.Substring(spot);
                _originalServer = s;
                _originalPath = p;

                _server = ServerMapper.NormalizeServerName(s);
            }
        }
    }
}