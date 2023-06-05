using SidecarNativeComm;

Console.WriteLine("Sidecar CLI");

CrossProcCommInClient crossProcCommInClient = new CrossProcCommInClient();
crossProcCommInClient.Connect();

string message = String.Join(" ", args);
Console.WriteLine("Sending: " + message);
string result = crossProcCommInClient.PostMessageWithResult(message);

Console.WriteLine("Received: " + result);