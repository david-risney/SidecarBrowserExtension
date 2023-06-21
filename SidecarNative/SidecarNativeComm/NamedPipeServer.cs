using System.IO.Pipes;
using System.Diagnostics;

namespace SidecarNativeComm
{
    public class NamedPipeServer
    {
        public static string DefaultPipeName = "net.deletethis.myhost.servername";

        private string _name = DefaultPipeName;
        private int _numThreads = 1;
        Thread?[]? _servers;

        public delegate string ProcessMessage(string message);
        public ProcessMessage? OnProcessMessage { get; set; } = null;

        public NamedPipeServer(string name, int numThreads)
        {
            _name = name;
            _numThreads = numThreads;
        }

        public void Listen()
        {
            _servers = new Thread[_numThreads];

            for (int i = 0; i < _numThreads; i++)
            {
                _servers[i] = new Thread(new ThreadStart(this.ServerThread));
                _servers[i]?.Start();
            }
        }

        public void Join()
        {
            int i = _numThreads;
            while (i > 0)
            {
                for (int j = 0; j < _numThreads; j++)
                {
                    Debug.Assert(_servers != null);
                    if (_servers[j] != null)
                    {
                        if (_servers[j]!.Join(250))
                        {
                            //Console.WriteLine("Server thread[{0}] finished.", _servers[j]!.ManagedThreadId);
                            _servers[j] = null;
                            i--;    // decrement the thread watch count
                        }
                    }
                }
            }
            //Console.WriteLine("\nServer threads exhausted, exiting.");
        }

        private void ServerThread()
        {
            Thread.CurrentThread.Name = "NamedPipeServer";

            int threadId = Thread.CurrentThread.ManagedThreadId;

            while (true)
            {
                NamedPipeServerStream pipeServer =
                    new NamedPipeServerStream(_name, PipeDirection.InOut, _numThreads);

                // Wait for a client to connect
                pipeServer.WaitForConnection();

                //Console.WriteLine("Client connected on thread[{0}].", threadId);
                try
                {
                    // Read the request from the client. Once the client has
                    // written to the pipe its security token will be available.

                    StreamString ss = new StreamString(pipeServer);

                    string command = ss.ReadString();

                    if (this.OnProcessMessage == null)
                    {
                        throw new Exception("Caller must set OnProcessMessage");
                    }

                    var result = this.OnProcessMessage(command);
                    ss.WriteString(result);                    
                }
                // Catch the IOException that is raised if the pipe is broken
                // or disconnected.
                catch (IOException)
                {
                    //Console.WriteLine("ERROR: {0}", e.Message);
                }
                pipeServer.Close();
            }
        }
    }
}
