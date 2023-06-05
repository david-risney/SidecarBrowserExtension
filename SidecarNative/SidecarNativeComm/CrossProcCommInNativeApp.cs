using System;
using System.Runtime.InteropServices.JavaScript;
using NativeMessaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceWire.NamedPipes;
using ServiceWire;
using System.Diagnostics;
using SidecarNativeApp;
using System.Diagnostics.CodeAnalysis;

namespace SidecarNativeComm
{
    public class CrossProcCommInNativeApp : ICrossProcCommServer
    {
        private NpHost? _npHost;
        private NativeMessaging.Host _host;

        public CrossProcCommInNativeApp(NativeMessaging.Host host)
        {
            _host = host;
        }

        public void Connect()
        {
            Close();

            var logger = new Logger(logLevel: LogLevel.Debug);
            var stats = new Stats();

            _npHost = new NpHost(Constants.PipeName, logger, stats);
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

        public void PostMessage(string messageAsString)
        {
            PostMessage(JObject.Parse(messageAsString));
        }

        public void PostMessage(JObject messageAsJObject)
        {
            _host.SendMessage(messageAsJObject);
        }

        private TaskCompletionSource<string>? _tcsWaitingForResponse = null;

        public string PostMessageWithResult(string messageAsString)
        {
            _tcsWaitingForResponse = new TaskCompletionSource<string>();
            _host.MessageReceived += Host_MessageReceivedForResponse;
            PostMessage(messageAsString);

            string result = _tcsWaitingForResponse.Task.Result;
            result = ReplaceValuesInMessage(result);
            _host.MessageReceived -= Host_MessageReceivedForResponse;
            _tcsWaitingForResponse = null;

            return result;
        }

        // Perform replacements of values in the message. The following replacements are performed:
        // "{SIDECAR:BrowserPid}" -> BrowserPid
        // "{SIDECAR:BrowserExe}" -> BrowserExe
        // "{SIDECAR:BrowserName}" -> BrowserName
        // "{SIDECAR:BrowserVersion}" -> BrowserVersion
        public string ReplaceValuesInMessage(string message)
        {
            message = message.Replace("{SIDECAR:BrowserPid}", BrowserPid.ToString());
            message = message.Replace("{SIDECAR:BrowserExe}", BrowserExe);
            message = message.Replace("{SIDECAR:BrowserName}", BrowserName);
            message = message.Replace("{SIDECAR:BrowserVersion}", BrowserVersion);

            return message;
        }
        

        private void Host_MessageReceivedForResponse(object sender, NativeMessaging.Host.MessageReceivedEventArgs e)
        {
            if (_tcsWaitingForResponse != null)
            {
                _tcsWaitingForResponse.SetResult(e.messageAsJObject?.ToString() ?? "");
            }
            else
            {
                throw new Exception("Received message, but no task is waiting for it");
            }
        }

        private Process? _browserProcess;
        public Process? BrowserProcess
        { 
            get
            {
                if (_browserProcess == null)
                {
                    _browserProcess = Process.GetCurrentProcess().GetParent();
                    if (_browserProcess?.ProcessName == "cmd.exe")
                    {
                        _browserProcess = _browserProcess.GetParent();
                    }
                }

                return _browserProcess;
            }
        }

        string? _browserName;
        public string BrowserName
        {
            get
            {
                if (_browserName == null)
                {
                    _browserName = BrowserProcess?.MainModule?.FileVersionInfo.ProductName;
                }
                return _browserName ?? "Unknown";
            }
            
        }

        string? _browserVersion;
        public string BrowserVersion
        {
            get
            {
                if (_browserVersion == null)
                {
                    _browserVersion = BrowserProcess?.MainModule?.FileVersionInfo.FileVersion;
                }

                return _browserVersion ?? "0.0.0.0";
            }
        }
        public int BrowserPid => BrowserProcess?.Id ?? 0;
        public string BrowserExe => BrowserProcess?.MainModule?.FileName ?? "unknown";
    }
}