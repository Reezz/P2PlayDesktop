using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2PlayDesktop
{
    // Open port ranges according to IANA
    // 5365 - 5396
    // 
    // Main port: 
    // 5365
    //
    // Assigned ports:
    // 5366 - 5396
    //
    // Maximum clients: 
    // 30
    class ConnectionManager
    {
        private const int listenPort = 5365;
        private const int GID1 = 9; // 9 => 1001
        private const int GID2 = 5; // 5 => 0101
        private const int REQUEST_CONNECT = 0;

        private UdpClient listener;
        private IPEndPoint ep;

        private Dictionary<string, string> ips; // key = username, value = IP as string
        private Dictionary<string, int> ports; // key = port, value = username;
        private List<UDPListener> clientListeners;

        private Form1 main;

        public ConnectionManager(Form1 f)
        {
            main = f;
            ips = new Dictionary<string, string>();
            ports = new Dictionary<string, int>();
            clientListeners = new List<UDPListener>();
            setUpConnection();
        }

        private void setUpConnection()
        {
            listener = new UdpClient(listenPort);
            ep = new IPEndPoint(IPAddress.Any, listenPort);
            beginListening();
        }

        private void beginListening()
        {
            Console.WriteLine("[CM] Starting to listen..");
            listener.BeginReceive(new AsyncCallback(handleReceive), null);
        }

        private void handleReceive(IAsyncResult ar)
        {
            try
            {
                Byte[] data = listener.EndReceive(ar, ref ep);
                handleData(data, ep);
                beginListening();
            }
            catch (ObjectDisposedException e) 
            {
                // Closed connection while waiting for async call
            }
        }

        private void handleData(Byte[] data, IPEndPoint ep)
        {
            if (data[0] == GID1 && data[1] == GID2)
            {
                int request = data[2];

                switch (request)
                {
                    case REQUEST_CONNECT:
                        handleConnectionRequest(data, ep);
                        break;
                }
            }
            else
            {
                // Ignore
            }
            

            string d = Encoding.ASCII.GetString(data);



            // TODO: call handleConnectionRequest(username) if request = "connect"

            // TODO: handle erroneous request 
            // Create custom error class?
            // Throw custom error: invalidRequest ?
        }

        private void handleConnectionRequest(byte[] data, IPEndPoint ep)
        {
            string username = "";
            for (int i = 3; i < data.Length; i++)
            {
                byte[] payload = data; // TODO: get only payload from data
                username += Encoding.ASCII.GetString(payload);
            }

            if (ips.ContainsKey(username))
            {
                // username already exists in userlist
                // if same IP => reconnecting user
                //            => give him same port as last connection
                if (ips[username] == ep.Address.ToString())
                {
                    ports[username];

                    // TODO: Send him to another port
                }
                else
                {
                    // if different IP => duplicate username
                    //                 => refuse connection
                    Console.WriteLine("[CM] Username already in use / Client already connected?");
                }
            }
            else
            {
                // add username and link to IP
                if (getNextFreePort() != -1)
                {
                    ips.Add(username, ep.Address.ToString());
                    Console.WriteLine("[CM] Added username: " + username);
                    Console.WriteLine("[CM] Using port: " + getNextFreePort().ToString());
                    clientListeners.Add(new UDPListener(main, getNextFreePort()));
                    ports.Add(username, getNextFreePort());
                }
                else
                {
                    // refuse client due to ports full
                    Console.WriteLine("[CM] Server is full, please try again later.");
                }
            }
        }

        // returns the port if possible to increase, -1 if ports full
        private int getNextFreePort() 
        {
            for (int i = 5366; i < 5396; i++)
            {
                if (!ports.ContainsValue(i))
                {
                    return i;
                }
            }

            return -1;
        }

        public void closeConnections()
        {
            // TODO: close all UDPListeners
            foreach (UDPListener l in clientListeners) 
            {
                l.closeConnection();
            }

            listener.Close();
        }
    }
}
