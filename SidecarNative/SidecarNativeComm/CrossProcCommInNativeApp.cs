using System;
using System.Runtime.InteropServices.JavaScript;
using NativeMessaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceWire.NamedPipes;
using ServiceWire;

namespace SidecarNativeComm
{
    public class CrossProcCommInNativeApp : ICrossProcCommServer
    {
        private NpHost _npHost;
        private NativeMessaging.Host _host;
        private ICrossProcCommClient _client;

        // public event ICrossProcComm.MessageReceivedDelegate MessageReceived;

        public CrossProcCommInNativeApp(NativeMessaging.Host host)
        {
            _host = host;
            _host.MessageReceived += Host_MessageReceived;
        }

        public void Connect()
        {
            Close();

            var logger = new Logger(logLevel: LogLevel.Debug);
            var stats = new Stats();

            _npHost = new NpHost(ICrossProcComm.PipeName, logger, stats);
            _npHost.AddService<ICrossProcCommServer>(this);

            _npHost.Open();
        }

        public void Close()
        {
            if (_npHost != null)
            {
                _npHost.Close();
                _npHost = null;
            }
        }

        private List<string> _waitingMessages = new List<string>();

        private void Host_MessageReceived(object sender, NativeMessaging.Host.MessageReceivedEventArgs e)
        {
            string messageAsString = e.messageAsJObject.ToString();
            if (_client != null)
            {
                InternalPostMessage(messageAsString);
            }
            else
            {
                _waitingMessages.Add(messageAsString);
            }
        }

        private void InternalPostMessage(string messageAsString)
        {
            _client.PostMessage(messageAsString);
            // this.MessageReceived?.Invoke(this, messageAsString);
        }

        public ICrossProcCommClient Client
        { 
            get 
            {
                return _client;
            }
            set
            { 
                _client = value;
                foreach (var message in _waitingMessages)
                {
                    InternalPostMessage(message);
                }
            }
        }

        public void PostMessage(string messageAsString)
        {
            PostMessage(JObject.Parse(messageAsString));
        }

        public void PostMessage(JObject messageAsJObject)
        {
            _host.SendMessage(messageAsJObject);
        }

        private TaskCompletionSource<string> _tcsWaitingForResponse = null;

        public async Task<string> PostMessageWithResult(string messageAsString)
        {
            _tcsWaitingForResponse = new TaskCompletionSource<string>();
            _host.MessageReceived += Host_MessageReceivedForResponse;
            PostMessage(messageAsString);

            string result = await _tcsWaitingForResponse.Task;
            _host.MessageReceived -= Host_MessageReceivedForResponse;
            _tcsWaitingForResponse = null;

            return result;
        }

        private void Host_MessageReceivedForResponse(object sender, NativeMessaging.Host.MessageReceivedEventArgs e)
        {
            _tcsWaitingForResponse.SetResult(e.messageAsJObject.ToString());
        }
    }
}