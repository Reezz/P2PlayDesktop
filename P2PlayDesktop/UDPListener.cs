using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace P2PlayDesktop
{
    class UDPListener
    {
        private UdpClient listener;
        private IPEndPoint ep;
        private Form1 main;
        private int port;

        public UDPListener(Form1 f, int port)
        {
            main = f;
            this.port = port;
            setUpConnection();
        }

        private void setUpConnection()
        {
            listener = new UdpClient(port);
            ep = new IPEndPoint(IPAddress.Any, port);
            beginListening();
        }

        private void beginListening()
        {
            listener.BeginReceive(new AsyncCallback(handleReceive), null);
        }

        private void handleReceive(IAsyncResult ar)
        {
            try
            {
                Byte[] data = listener.EndReceive(ar, ref ep);
                // Temp connection info in console
                main.Log("[" + port + "] Handling data from " + ep.Address.ToString());
                main.Log("[" + port + "] " + Encoding.ASCII.GetString(data));
                main.Log("");
                // TODO: do call to handle data
                beginListening();
            }
            catch (ObjectDisposedException e)
            {
                // Closed connection while waiting for async call
            }
        }

        public void closeConnection()
        {
            listener.Close();
        }
    }
}
