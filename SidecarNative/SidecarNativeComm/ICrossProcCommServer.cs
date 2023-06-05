using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SidecarNativeComm
{
    public static class Constants
    {
        public const string PipeName = "net.deletethis.myhost.servername";
    }

    public interface ICrossProcCommServer
    {
        public string PostMessageWithResult(string messageAsString);
    }
}
