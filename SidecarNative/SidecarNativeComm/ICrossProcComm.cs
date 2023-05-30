using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SidecarNativeComm
{
    public interface ICrossProcComm
    {
        public const string PipeName = "net.deletethis.myhost.servername";

        //public delegate void MessageReceivedDelegate(object sender, string message);
        //bpublic event MessageReceivedDelegate MessageReceived;

        public void PostMessage(string messageAsString);
        public void PostMessage(JObject messageAsJObject);

        public Task<string> PostMessageWithResult(string messageAsString);
    }

    public interface ICrossProcCommServer : ICrossProcComm
    {
        ICrossProcCommClient Client { get; set; }
    }

    public interface ICrossProcCommClient : ICrossProcComm
    {
    }
}
