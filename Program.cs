using System.Net.WebSockets;
using System.Text;

var webSocket = new ClientWebSocket();

string username;

while (true)
{
    Console.Write("Input username:");

    username = Console.ReadLine();

    break;
}

Console.WriteLine("Connecting to SERVER");

await webSocket.ConnectAsync(new Uri($"ws://localhost:5001/?username={username}"), CancellationToken.None);

Console.WriteLine("Connected!");

var receiveTask = Task.Run(async () =>
{
    var buffer = new byte[1024];
    while (true)
    {
        var receive = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        if (receive.MessageType == WebSocketMessageType.Close)
        {
            break;
        }

        var data = Encoding.UTF8.GetString(buffer, 0, receive.Count);

        Console.WriteLine(data);
    }
});

var sendTask = Task.Run(async () =>
{
    while (true)
    {
        var message = Console.ReadLine();

        if (message == "exit")
        {
            break;
        }

        var bytes = Encoding.UTF8.GetBytes(message);

        await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }
});


await Task.WhenAny(sendTask, receiveTask);

if (webSocket.State != WebSocketState.Closed)
{
    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
}

await Task.WhenAll(sendTask, receiveTask);
