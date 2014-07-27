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
        private UdpClient listener;
        private IPEndPoint ep;

        private Dictionary<string, string> clients; // key = username, value = IP as string
        private Dictionary<int, string> ports; // key = port, value = username;
        private List<UDPListener> clientListeners;

        private Form1 main;

        public ConnectionManager(Form1 f)
        {
            main = f;
            clients = new Dictionary<string, string>();
            ports = new Dictionary<int, string>();
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
            main.Log("[CM] Starting to listen..");
            listener.BeginReceive(new AsyncCallback(handleReceive), null);
        }

        private void handleReceive(IAsyncResult ar)
        {
            try
            {
                Byte[] data = listener.EndReceive(ar, ref ep);
                handleConnectionRequest(data, ep);
                beginListening();
            }
            catch (ObjectDisposedException e) 
            {
                // Closed connection while waiting for async call
            }
        }

        private void handleConnectionRequest(Byte[] data, IPEndPoint ep)
        {
            string username = Encoding.ASCII.GetString(data);

            if (clients.ContainsKey(username))
            {
                // username already exists
                // check
                // refuse connection
                main.Log("[CM] Username already in use / Client already connected?");
            }
            else
            {
                // add username and link to IP
                if (getNextFreePort() != -1)
                {
                    clients.Add(username, ep.Address.ToString());
                    main.Log("[CM] Added username: " + username);
                    main.Log("[CM] Using port: " + getNextFreePort().ToString());
                    clientListeners.Add(new UDPListener(main, getNextFreePort()));
                    ports.Add(getNextFreePort(), username);
                }
                else
                {
                    // refuse client due to ports full
                    main.Log("[CM] Server is full, please try again later.");
                }
            }
        }

        // returns the port if possible to increase, -1 if ports full
        private int getNextFreePort() 
        {
            for (int i = 5366; i < 5396; i++)
            {
                if (!ports.ContainsKey(i))
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
