using System.Net;
using System.Net.Sockets;
using Shared;

var connectedClients = new List<KeyValuePair<TcpClient, Guid>>();

var server = new TcpListener(IPAddress.Any, 3000);
server.Start();
Console.WriteLine("Server started");

while (true)
{
    Console.WriteLine("Waiting for incoming client connections...");
    var tcpClient = server.AcceptTcpClient();

    var connectedClientId = Guid.NewGuid();
    connectedClients.Add(new KeyValuePair<TcpClient, Guid>(tcpClient, connectedClientId));
    Console.WriteLine(
        $"Client {connectedClientId} established connection with IP address: {tcpClient.Client.RemoteEndPoint!} on {DateTime.Now}");

    var clientThread = new Thread(HandleClientRequest!);
    clientThread.Start(tcpClient);
}

void HandleClientRequest(object request)
{
    var client = (TcpClient)request;
    var clientPair = connectedClients.Find(c => c.Key == client);
    var stream = client.GetStream();

    var matrix01Buffer = new byte[1024];
    var matrix02Buffer = new byte[1024];

    try
    {
        while (stream.Read(matrix01Buffer, 0, matrix01Buffer.Length) > 0 &&
               stream.Read(matrix02Buffer, 0, matrix02Buffer.Length) > 0)
        {
            var matrix01 = StreamDataTransfer.ConvertToMatrix(matrix01Buffer);
            var matrix02 = StreamDataTransfer.ConvertToMatrix(matrix02Buffer);

            Console.WriteLine($"Client {clientPair.Value}: calculating the product of two matrices...");
            var matrixProduct = CalculateMatricesProduct(matrix01, matrix02);
            var matrixProductBytes = StreamDataTransfer.ConvertToBytes(matrixProduct);

            Thread.Sleep(10000);

            stream.Write(matrixProductBytes, 0, matrixProductBytes.Length);
            Console.WriteLine($"Client {clientPair.Value}: calculation result sent");
        }
    }
    catch (IOException)
    {
        connectedClients.Remove(clientPair);
        client.Close();
        Console.WriteLine($"Client {clientPair.Value} disconnected");
    }
}

int[,] CalculateMatricesProduct(int[,] matrix01, int[,] matrix02)
{
    var matrix01RowsQuantity = matrix01.GetLength(0);
    var matrix01ColsQuantity = matrix01.GetLength(1);
    var matrix02RowsQuantity = matrix02.GetLength(0);
    var matrix02ColsQuantity = matrix02.GetLength(1);

    if (matrix01ColsQuantity != matrix02RowsQuantity)
        throw new ArgumentException(
            "The number of columns in the first matrix must be equal to the number of rows in the second matrix.");

    var result = new int[matrix01RowsQuantity, matrix02ColsQuantity];

    for (int i = 0; i < matrix01RowsQuantity; i++)
    {
        for (int j = 0; j < matrix02ColsQuantity; j++)
        {
            var sum = 0;
            for (int k = 0; k < matrix01ColsQuantity; k++)
            {
                sum += matrix01[i, k] * matrix02[k, j];
            }

            result[i, j] = sum;
        }
    }

    return result;
}