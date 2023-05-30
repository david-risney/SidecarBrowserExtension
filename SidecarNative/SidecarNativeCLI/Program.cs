using SidecarNativeComm;

Console.WriteLine("Sidecar CLI");

CrossProcCommInClient crossProcCommInClient = new CrossProcCommInClient();
crossProcCommInClient.Connect();

string message = String.Join(" ", args);
Console.WriteLine("Sending: " + message);
string result = await crossProcCommInClient.PostMessageWithResult(message);

Console.WriteLine("Received: " + result);