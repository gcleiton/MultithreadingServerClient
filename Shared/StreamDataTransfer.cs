namespace Shared;

public static class StreamDataTransfer
{
    public static int[,] ConvertToMatrix(byte[] bytes)
    {
        var matriz = new int[2, 2];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                var offset = (2 * i + j) * sizeof(int);
                matriz[i, j] = BitConverter.ToInt32(bytes, offset);
            }
        }
        
        return matriz;
    }
    
    public static byte[] ConvertToBytes(int[,] matrix)
    {
        var bytes = new byte[matrix.Length * sizeof(int)];
        Buffer.BlockCopy(matrix, 0, bytes, 0, bytes.Length);
        return bytes;
    }
}