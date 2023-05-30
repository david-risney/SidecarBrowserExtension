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
    public class CrossProcCommInClient : ICrossProcCommClient
    {
        private NpClient<ICrossProcCommServer> _server;

        public CrossProcCommInClient() { }

        public void Connect()
        {
            _server = new NpClient<ICrossProcCommServer>(new NpEndPoint(ICrossProcComm.PipeName));
            //_server.Proxy.Client = this;
            //_server.Proxy.MessageReceived += Proxy_MessageReceived;
        }

        //private void Proxy_MessageReceived(object sender, string message) { this.MessageReceived?.Invoke(sender, message); }

        //public event ICrossProcComm.MessageReceivedDelegate MessageReceived;

        public void PostMessage(string messageAsString) { _server.Proxy.PostMessage(messageAsString); }
        public void PostMessage(JObject messageAsJObject) { PostMessage(messageAsJObject.ToString()); }

        public Task<string> PostMessageWithResult(string messageAsString)
        {
            return _server.Proxy.PostMessageWithResult(messageAsString);
        }
    }
}
