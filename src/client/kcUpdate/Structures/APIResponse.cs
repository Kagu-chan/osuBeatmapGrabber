using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kcUpdater.Structures
{
    public struct APIResponse
    {
        public bool Success;
        public string Message;
        public Dictionary<string, string> Versions;
        public Dictionary<string, string> Files;
        public string Version;
    }
}
