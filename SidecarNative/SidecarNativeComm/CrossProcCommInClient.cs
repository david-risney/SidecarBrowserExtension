using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SidecarNativeComm
{
    public class CrossProcCommInClient 
    {
        NamedPipeClient _client;

        public CrossProcCommInClient()
        {
            _client = new NamedPipeClient(NamedPipeServer.DefaultPipeName);
        }

        public void Connect()
        {
            _client.Connect();
        }

        public string PostMessageWithResult(string messageAsString)
        {
            return _client.SendMessageWithReply(messageAsString);
        }
    }
}
