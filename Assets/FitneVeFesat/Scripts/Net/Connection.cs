using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace FitneFesat.Net
{
    // Tek bir TCP bağlantısı: 4 baytlık uzunluk + UTF-8 JSON. Okuma ayrı iş parçacığında yapılır,
    // gelen paketler kuyruğa düşer; ana iş parçacığı (Unity Update) kuyruktan okur.
    public class Connection
    {
        const int MaxPacket = 1 << 20;

        public readonly int Id;
        readonly TcpClient client;
        readonly NetworkStream stream;
        readonly object sendLock = new object();
        readonly ConcurrentQueue<Packet> inbox;
        readonly Action<Connection> onClosed;
        volatile bool closed;

        public bool Closed => closed;

        public Connection(int id, TcpClient client, ConcurrentQueue<Packet> inbox, Action<Connection> onClosed)
        {
            Id = id;
            this.client = client;
            this.inbox = inbox;
            this.onClosed = onClosed;
            client.NoDelay = true;
            stream = client.GetStream();
            var reader = new Thread(ReadLoop) { IsBackground = true, Name = "FitneFesat okuma " + id };
            reader.Start();
        }

        public void Send(string type, object body)
        {
            var packet = new Packet { t = type, body = body == null ? "" : JsonUtility.ToJson(body) };
            SendRaw(JsonUtility.ToJson(packet));
        }

        void SendRaw(string json)
        {
            if (closed) return;
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] len = BitConverter.GetBytes(data.Length);
            try
            {
                lock (sendLock)
                {
                    stream.Write(len, 0, 4);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception)
            {
                Close();
            }
        }

        void ReadLoop()
        {
            var header = new byte[4];
            try
            {
                while (!closed)
                {
                    if (!ReadExactly(header, 4)) break;
                    int len = BitConverter.ToInt32(header, 0);
                    if (len < 0 || len > MaxPacket) break;
                    var data = new byte[len];
                    if (!ReadExactly(data, len)) break;
                    var packet = JsonUtility.FromJson<Packet>(Encoding.UTF8.GetString(data));
                    if (packet != null) inbox.Enqueue(packet);
                }
            }
            catch (Exception)
            {
                // Bağlantı koptu.
            }
            Close();
        }

        bool ReadExactly(byte[] buffer, int count)
        {
            int read = 0;
            while (read < count)
            {
                int n = stream.Read(buffer, read, count - read);
                if (n <= 0) return false;
                read += n;
            }
            return true;
        }

        public void Close()
        {
            if (closed) return;
            closed = true;
            try { stream.Close(); } catch (IOException) { } catch (ObjectDisposedException) { }
            try { client.Close(); } catch (Exception) { }
            onClosed?.Invoke(this);
        }
    }
}
