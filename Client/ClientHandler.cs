using System.Net;
using System.Net.Sockets;
using Shared;

namespace Client;

public class ClientHandler
{
    private readonly TcpClient _client;

    private ClientHandler(TcpClient client)
    {
        _client = client;
    }

    public static ClientHandler With(TcpClient client)
        => new(client);

    public void Initialize()
    {
        _client.Connect(IPAddress.Parse("127.0.0.1"), 3000);
        Console.WriteLine("Connected to server...");

        var receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        var matrix01 = ReadMatrix("Enter the values of the first matrix:");
        var matrix02 = ReadMatrix("Enter the values of the first matrix:");
        var bytesMatrix01 = StreamDataTransfer.ConvertToBytes(matrix01);
        var bytesMatrix02 = StreamDataTransfer.ConvertToBytes(matrix02);

        var stream = _client.GetStream();
        stream.Write(bytesMatrix01, 0, bytesMatrix01.Length);
        stream.Write(bytesMatrix02, 0, bytesMatrix02.Length);
    }

    private void ReceiveMessages()
    {
        while (true)
        {
            var stream = _client.GetStream();
            var resultBytes = new byte[1024];
            stream.Read(resultBytes, 0, resultBytes.Length);
            var resultMatrix = StreamDataTransfer.ConvertToMatrix(resultBytes);

            Console.WriteLine($"[{resultMatrix[0, 0]} {resultMatrix[0, 1]}]");
            Console.WriteLine($"[{resultMatrix[1, 0]} {resultMatrix[1, 1]}]");
        }
    }

    private int[,] ReadMatrix(string text)
    {
        Console.WriteLine(text);
        var inputs = Console.ReadLine()?.Split(' ');
        if (inputs == null || inputs.Length != 4)
            throw new ArgumentException("Invalid input: matrix must have exactly 4 values");

        var matrix = new int[2, 2];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                var isValidInput = int.TryParse(inputs[2 * i + j], out matrix[i, j]);
                if (!isValidInput)
                    throw new ArgumentException("Invalid input: matrix value must be an integer");
            }
        }

        return matrix;
    }
}