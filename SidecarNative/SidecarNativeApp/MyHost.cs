using NativeMessaging;
using Newtonsoft.Json.Linq;
using SidecarNativeComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SidecarNativeApp
{
    public class MyHost : Host
    {
        private CrossProcCommInNativeApp _crossProc;

        public override string Hostname => "net.deletethis.myhost";

        public MyHost() : base(/*SendConfirmationReceipt*/ false)
        {
            _crossProc = new CrossProcCommInNativeApp(this);
        }

        public void Connect()
        {
            _crossProc.Connect();
        }

        public void Close()
        {
            _crossProc.Close();
        }

        protected override void ProcessReceivedMessage(JObject data)
        {
            base.ProcessReceivedMessage(data);
        }
    }
}
