using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FitneFesat.Net
{
    // Oda kuran telefonun sunucusu: TCP ile bağlantıları kabul eder, UDP ile keşif sorularını yanıtlar.
    public class HostServer
    {
        public struct Incoming
        {
            public int ConnId;
            public Packet Packet;
        }

        public int Port { get; private set; }
        public string RoomName { get; private set; }

        TcpListener listener;
        UdpClient discovery;
        volatile bool running;
        int nextId = 1;

        readonly ConcurrentQueue<Incoming> inbox = new ConcurrentQueue<Incoming>();
        readonly ConcurrentQueue<int> closedQueue = new ConcurrentQueue<int>();
        readonly Dictionary<int, Connection> connections = new Dictionary<int, Connection>();
        readonly Dictionary<int, ConcurrentQueue<Packet>> queues = new Dictionary<int, ConcurrentQueue<Packet>>();
        readonly object connLock = new object();

        public void Start(string roomName)
        {
            RoomName = roomName;
            running = true;

            for (int port = NetConfig.TcpPort; port < NetConfig.TcpPort + 10; port++)
            {
                try
                {
                    listener = new TcpListener(IPAddress.Any, port);
                    listener.Start();
                    Port = port;
                    break;
                }
                catch (SocketException)
                {
                    listener = null;
                }
            }
            if (listener == null) throw new Exception("Oda açılamadı: ağ bağlantı noktaları dolu.");

            new Thread(AcceptLoop) { IsBackground = true, Name = "FitneFesat kabul" }.Start();

            try
            {
                discovery = new UdpClient();
                discovery.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                discovery.Client.Bind(new IPEndPoint(IPAddress.Any, NetConfig.DiscoveryPort));
                new Thread(DiscoveryLoop) { IsBackground = true, Name = "FitneFesat keşif" }.Start();
            }
            catch (SocketException)
            {
                // Keşif çalışmazsa oyuncular adresi elle girerek katılabilir.
                discovery = null;
            }
        }

        void AcceptLoop()
        {
            while (running)
            {
                try
                {
                    var tcp = listener.AcceptTcpClient();
                    int id;
                    var queue = new ConcurrentQueue<Packet>();
                    lock (connLock)
                    {
                        id = nextId++;
                        queues[id] = queue;
                        connections[id] = new Connection(id, tcp, queue, c => closedQueue.Enqueue(c.Id));
                    }
                }
                catch (Exception)
                {
                    if (!running) return;
                }
            }
        }

        void DiscoveryLoop()
        {
            while (running)
            {
                try
                {
                    var from = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = discovery.Receive(ref from);
                    if (Encoding.UTF8.GetString(data) == NetConfig.DiscoveryAsk)
                    {
                        byte[] reply = Encoding.UTF8.GetBytes($"{NetConfig.DiscoveryReply}|{RoomName}|{Port}");
                        discovery.Send(reply, reply.Length, from);
                    }
                }
                catch (Exception)
                {
                    if (!running) return;
                }
            }
        }

        // Ana iş parçacığından çağrılır.
        public bool TryReceive(out Incoming incoming)
        {
            lock (connLock)
            {
                foreach (var kv in queues)
                {
                    if (kv.Value.TryDequeue(out var p))
                    {
                        incoming = new Incoming { ConnId = kv.Key, Packet = p };
                        return true;
                    }
                }
            }
            incoming = default;
            return false;
        }

        public bool TryGetClosed(out int connId)
        {
            if (closedQueue.TryDequeue(out connId))
            {
                lock (connLock)
                {
                    connections.Remove(connId);
                    queues.Remove(connId);
                }
                return true;
            }
            return false;
        }

        public void Send(int connId, string type, object body)
        {
            Connection c;
            lock (connLock) connections.TryGetValue(connId, out c);
            c?.Send(type, body);
        }

        public void Stop()
        {
            running = false;
            try { listener?.Stop(); } catch (Exception) { }
            try { discovery?.Close(); } catch (Exception) { }
            List<Connection> all;
            lock (connLock) all = new List<Connection>(connections.Values);
            foreach (var c in all) c.Close();
        }

        // Kullanıcıya gösterilecek yerel IPv4 adresi.
        public static string LocalAddress()
        {
            try
            {
                using (var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    s.Connect("10.255.255.255", 1);
                    if (s.LocalEndPoint is IPEndPoint ep) return ep.Address.ToString();
                }
            }
            catch (Exception) { }
            return "127.0.0.1";
        }
    }
}
