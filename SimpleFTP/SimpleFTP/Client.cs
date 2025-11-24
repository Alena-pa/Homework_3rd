using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFTP;

public class Client
{
    private readonly TcpClient client;
    private readonly NetworkStream stream;
    private readonly StreamWriter writer;
    private readonly StreamReader reader;

    public void Start(int port, string ip)
    {
        client = new TcpClient(ip, port);
        stream = client.GetStream();
        writer = new StreamWriter(stream);
        reader = new StreamReader(stream);
    }

    public async Task<string> List(string path)
    {
        await writer.WriteLineAsync($"1 {path}");
        return await reader.ReadLineAsync();
    }

    public async Task<byte[]> Get(string path)
    {
        await writer.WriteLineAsync($"2 {path}");

        string size = await reader.ReadLineAsync();
        if (!long.TryParse(size, out size))
        {
            throw new InvalidDataException("Invalid size");
        }
        else
        {
            if (size == -1)
            {
                return null;
            }

            var buffer = new byte[size];
            int totalRead = 0;
            while (totalRead < size)
            {
                int read = await stream.ReadAsync(buffer, totalRead, (int)(size - totalRead));
                totalRead += read;
            }

            return buffer;
        }
    }

    public void Dispose()
    {
        writer.Dispose();
        reader.Dispose();
        stream.Dispose();
        client.Close();
    }
}