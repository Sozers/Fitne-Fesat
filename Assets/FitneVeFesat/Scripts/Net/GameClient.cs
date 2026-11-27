using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FitneFesat.Net
{
    // Oyuncunun telefonundaki istemci: ev sahibine bağlanır, paket gönderir ve alır.
    public class GameClient
    {
        Connection connection;
        readonly ConcurrentQueue<Packet> inbox = new ConcurrentQueue<Packet>();
        public bool Connected => connection != null && !connection.Closed;

        public void Connect(string host, int port)
        {
            var tcp = new TcpClient();
            var result = tcp.BeginConnect(host, port, null, null);
            if (!result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(4)) || !tcp.Connected)
            {
                try { tcp.Close(); } catch (Exception) { }
                throw new Exception($"{host}:{port} adresine bağlanılamadı.");
            }
            tcp.EndConnect(result);
            connection = new Connection(0, tcp, inbox, null);
        }

        public void Send(string type, object body)
        {
            connection?.Send(type, body);
        }

        public bool TryReceive(out Packet packet)
        {
            return inbox.TryDequeue(out packet);
        }

        public void Close()
        {
            connection?.Close();
            connection = null;
        }
    }

    // Yerel ağdaki odaları bulur: UDP yayını ve aynı bilgisayardaki odalar için 127.0.0.1.
    public class RoomFinder
    {
        public class Room
        {
            public string Name;
            public string Address;
            public int Port;
            public DateTime LastSeen;
        }

        UdpClient udp;
        volatile bool running;
        readonly object roomLock = new object();
        readonly Dictionary<string, Room> rooms = new Dictionary<string, Room>();

        public void Start()
        {
            if (running) return;
            running = true;
            try
            {
                udp = new UdpClient(0) { EnableBroadcast = true };
                new Thread(ReceiveLoop) { IsBackground = true, Name = "FitneFesat oda arama" }.Start();
                new Thread(AskLoop) { IsBackground = true, Name = "FitneFesat oda sorma" }.Start();
            }
            catch (SocketException)
            {
                running = false;
            }
        }

        void AskLoop()
        {
            byte[] ask = Encoding.UTF8.GetBytes(NetConfig.DiscoveryAsk);
            while (running)
            {
                try { udp.Send(ask, ask.Length, new IPEndPoint(IPAddress.Broadcast, NetConfig.DiscoveryPort)); } catch (Exception) { }
                try { udp.Send(ask, ask.Length, new IPEndPoint(IPAddress.Loopback, NetConfig.DiscoveryPort)); } catch (Exception) { }
                Thread.Sleep(1000);
            }
        }

        void ReceiveLoop()
        {
            while (running)
            {
                try
                {
                    var from = new IPEndPoint(IPAddress.Any, 0);
                    string msg = Encoding.UTF8.GetString(udp.Receive(ref from));
                    var parts = msg.Split('|');
                    if (parts.Length == 3 && parts[0] == NetConfig.DiscoveryReply && int.TryParse(parts[2], out int port))
                    {
                        string address = from.Address.ToString();
                        lock (roomLock)
                        {
                            string key = parts[1] + "@" + port;
                            // Aynı oda hem 127.0.0.1 hem ağ adresinden yanıt verebilir; ağ adresi yeğlenir.
                            if (!rooms.TryGetValue(key, out var room) || room.Address == "127.0.0.1")
                                rooms[key] = room = new Room { Name = parts[1], Address = address, Port = port };
                            room.LastSeen = DateTime.UtcNow;
                        }
                    }
                }
                catch (Exception)
                {
                    if (!running) return;
                }
            }
        }

        public List<Room> Rooms()
        {
            lock (roomLock)
            {
                var list = new List<Room>();
                foreach (var r in rooms.Values)
                    if ((DateTime.UtcNow - r.LastSeen).TotalSeconds < 4) list.Add(r);
                return list;
            }
        }

        public void Stop()
        {
            running = false;
            try { udp?.Close(); } catch (Exception) { }
            lock (roomLock) rooms.Clear();
        }
    }
}
