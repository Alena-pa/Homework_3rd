using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFTP;

public class Server(int port)
{
    private readonly int port = port;
    public static Start()
    {
        using var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        while (true)
        {
            var socket = await listener.AcceptSocketAsync();
            Task.Run(async () =>
            {

                await using var stream = new NetworkStream(socket);
                using var reader = new StreamReader(stream);
                await using var writer = new StreamWriter(stream);
            });
        }
    }

    private static async Task List(StreamWriter writer, string path)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                await writer.WriteLineAsync("-1");
                await writer.Flush();
                return;
            }

            var entries = Directory.GetFileSystemEntries(path);
            var result = new StringBuilder();
            result.Append(entries.Length);

            foreach (string entry in entries)
            {
                string name = Path.GetFileName(entry);
                bool isDir = File.GetAttributes(entry).HasFlag(FileAttributes.Directory);
                result.Append($" {name} {isDir.ToString().ToLower()}");
            }

            result.Append("\n");
            await writer.WriteAsync(result.ToString());
            await writer.FlushAsync();
        }
        catch (Exception)
        {
            await writer.WriteLineAsync("-1");
            await writer.FlushAsync();
        }
    }

    private static async Task Get(Stream stream, StreamWriter writer, string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                await writer.WriteLineAsync("-1");
                await writer.Flush();
                return;
            }

            byte[] content = await File.ReadAllBytesAsync(path);
            string header = content.Length.ToString() + " ";
            byte[] headerBytes = Encoding.UTF8.GetBytes(header);

            await stream.WriteAsync(headerBytes, 0, headerBytes.Length);
            await stream.WriteAsync(content, 0, content.Length);
            await stream.FlushAsync();
        }
        catch
        {
            await writer.WriteLineAsync("-1");
            await writer.Flush();
        }
    }
}