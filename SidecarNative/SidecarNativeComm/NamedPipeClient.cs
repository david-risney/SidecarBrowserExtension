using System.IO.Pipes;
using System.Security.Principal;

namespace SidecarNativeComm
{
    public class NamedPipeClient
    {
        private string _name;
        private NamedPipeClientStream? _pipeClient;

        public NamedPipeClient(string name)
        {
            _name = name;

        }

        public void Connect()
        { 
            if (_pipeClient == null)
            {
                _pipeClient = new NamedPipeClientStream(".", _name, PipeDirection.InOut);
            }
            if (!_pipeClient.IsConnected)
            { 
                _pipeClient.Connect();
            }
        }

        public void Close()
        {
            if (_pipeClient != null)
            {
                if (_pipeClient.IsConnected)
                {
                    _pipeClient.Close();
                }
                _pipeClient = null;
            }
        }

        public string SendMessageWithReply(string message)
        {
            if (_pipeClient == null)
            {
                throw new Exception("Pipe client is null");
            }
            if (!_pipeClient.IsConnected)
            {
                throw new Exception(message: "Pipe client is not connected");
            }
            var ss = new StreamString(_pipeClient);

            ss.WriteString(message);
            return ss.ReadString();
        }
    }
}
