using System.Net.WebSockets;
using System.Text;

var webSocket = new ClientWebSocket();

Console.WriteLine("Connection to SERVER");

await webSocket.ConnectAsync(new Uri("ws://localhost:5001/"), CancellationToken.None);

Console.WriteLine("Connected!");

var receiveTask = Task.Run(async () =>
{
    var buffer = new byte[1024];
    while (true)
    {
        var receive = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        
        if (receive.MessageType == WebSocketMessageType.Close)
            break;

        var data = Encoding.UTF8.GetString(buffer, 0, receive.Count);

        Console.WriteLine("Received: " + data);
    }
});

await receiveTask;