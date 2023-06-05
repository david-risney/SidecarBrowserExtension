using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceWire.NamedPipes;

namespace SidecarNativeComm
{
    public class CrossProcCommInClient 
    {
        private NpClient<ICrossProcCommServer>? _server;

        public CrossProcCommInClient() { }

        public void Connect()
        {
            _server = new NpClient<ICrossProcCommServer>(new NpEndPoint(Constants.PipeName));
        }

        public string PostMessageWithResult(string messageAsString)
        {
            return _server?.Proxy.PostMessageWithResult(messageAsString);
        }
    }
}
