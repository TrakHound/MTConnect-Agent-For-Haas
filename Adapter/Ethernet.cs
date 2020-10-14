// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using NLog;
using System;
using System.Net.Sockets;
using System.Text;

namespace MTConnect.Adapters.Haas
{
    public class Ethernet
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public string Server;
        public int Port;

        private TcpClient client = null;
        private NetworkStream stream = null;

        public Ethernet(string server, int port)
        {
            Server = server;
            Port = port;
        }

        public void Connect()
        {
            Close();

            try
            {
                client = new TcpClient(Server, Port);
                stream = client.GetStream();
            }
            catch (ArgumentNullException e)
            {
                log.Error("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                log.Error("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                log.Error("Exception: {0}", e);
            }
        }

        public void Close()
        {
            try
            {
                if (client != null) client.Close();
                if (stream != null) stream.Close();
            }
            catch (ArgumentNullException e)
            {
                log.Error("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                log.Error("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                log.Error("Exception: {0}", e);
            }
        }

        public string SendCommand(string cmd)
        {
            string result = null;

            try
            {
                // Connect to Server
                if (client == null || stream == null || (client != null && !client.Connected))
                {
                    Connect();
                }

                if (client != null && stream != null && client.Connected)
                {
                    // Translate the passed message into ASCII and store it as a Byte array.
                    byte[] data = Encoding.ASCII.GetBytes(cmd + "\r\n");

                    // Send the message to the connected TcpServer. 
                    stream.Write(data, 0, data.Length);

                    System.Threading.Thread.Sleep(500);

                    // Buffer to store the response bytes.
                    data = new byte[2048];

                    // String to store the response ASCII representation.
                    string responseData = string.Empty;

                    // Read the first batch of the TcpServer response bytes.
                    int bytes = stream.Read(data, 0, data.Length);
                    responseData = Encoding.ASCII.GetString(data, 0, bytes);

                    if (!string.IsNullOrEmpty(responseData))
                    {
                        responseData = responseData.TrimEnd();
                        responseData = responseData.TrimEnd('>');
                        responseData = responseData.TrimEnd();
                    }

                    result = responseData;
                }
            }
            catch (ArgumentNullException e)
            {
                log.Error("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                log.Error("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                log.Error("Exception: {0}", e);
            }

            return result;
        }
    }
}
