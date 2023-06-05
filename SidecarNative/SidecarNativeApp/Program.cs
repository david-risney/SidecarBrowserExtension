using NativeMessaging;
using SidecarNativeApp;
using System.Reflection;

class Program
{
    static public string AssemblyLoadDirectory
    {
        get
        {
            string codeBase = Assembly.GetEntryAssembly()?.Location ?? throw new Exception("No assembly.");
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path) ?? throw new Exception("No assembly.");
        }
    }

    static public string AssemblyExecuteablePath
    {
        get
        {
            string codeBase = Assembly.GetEntryAssembly()?.Location ?? throw new Exception("No assembly.");
            UriBuilder uri = new UriBuilder(codeBase);
            return Uri.UnescapeDataString(uri.Path);
        }
    }

    static MyHost _host = new MyHost();


    enum CommandLineKind
    {
        Unknown,
        Unregister,
        Register,
        Run
    };

    private Program()
    {
        _host.SupportedBrowsers.Add(ChromiumBrowser.GoogleChrome);
        _host.SupportedBrowsers.Add(ChromiumBrowser.MicrosoftEdge);
    }

    static void Main(String[] args)
    {
        Program program = new Program();
        program.RunFromCommandLine(args);
    }

    private void RunFromCommandLine(String[] args)
    {
        CommandLineKind commandLineKind = CommandLineKind.Unknown;

        for (int argIdx = 0; argIdx < args.Length; ++argIdx)
        {
            if (commandLineKind == CommandLineKind.Unknown)
            {
                string arg = args[argIdx].ToLower().Trim().Trim('-');
                if (arg == "unregister")
                {
                    commandLineKind = CommandLineKind.Unregister;
                    Unregister();
                }
                else if (arg == "register" && argIdx + 1 < args.Length)
                {
                    commandLineKind = CommandLineKind.Register;
                    Register(args[argIdx + 1]);
                }
                else
                {
                    commandLineKind = CommandLineKind.Run;
                    Run();
                }
            }
        }
    }

    private void Register(string extensionOrigin)
    {
        const string description = "Sidecar native app";

        var allowedOrigins = new string[] { extensionOrigin };

        _host.GenerateManifest(description, allowedOrigins);
        _host.Register();
    }

    private void Unregister()
    {
        _host.SupportedBrowsers.Add(ChromiumBrowser.GoogleChrome);
        _host.SupportedBrowsers.Add(ChromiumBrowser.MicrosoftEdge);

        _host.Unregister();
        _host.RemoveManifest();
    }

    private void Run()
    {
        _host.SupportedBrowsers.Add(ChromiumBrowser.GoogleChrome);
        _host.SupportedBrowsers.Add(ChromiumBrowser.MicrosoftEdge);

        _host.Connect();
        _host.Listen();
        _host.Close();
    }
}