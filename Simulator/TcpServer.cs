// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Net;
using System.Net.Sockets;

namespace MTConnect.Simulators.Haas
{
    public class TcpServer
    {
        private System.Timers.Timer timer = new System.Timers.Timer();
        private int i = 0;

        public TcpServer(int port)
        {
            timer.Interval = 5000;
            timer.Elapsed += (o, e) =>
            {
                i++;
                if (i > 3) i = 0;
            };
            timer.Enabled = true;


            TcpListener server = null;
            try
            {
                var localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        string response = "";

                        if (!string.IsNullOrEmpty(data))
                        {
                            data = data.TrimEnd();

                            switch (data)
                            {
                                case "?Q100": response = Q100(); break;
                                case "?Q101": response = Q101(); break;
                                case "?Q102": response = Q102(); break;
                                case "?Q104": response = Q104(); break;
                                case "?Q200": response = Q200(); break;
                                case "?Q201": response = Q201(); break;
                                case "?Q300": response = Q300(); break;
                                case "?Q301": response = Q301(); break;
                                case "?Q303": response = Q303(); break;
                                case "?Q304": response = Q304(); break;
                                case "?Q402": response = Q402(); break;
                                case "?Q403": response = Q403(); break;
                                case "?Q500": response = Q500(); break;

                            }

                            // Custom Variables
                            var pattern = "^?Q600 (.*)$";
                            var match = new System.Text.RegularExpressions.Regex(pattern).Match(data);
                            if (match.Success && match.Groups.Count > 1)
                            {
                                if (int.TryParse(match.Groups[1].ToString(), out int var))
                                {
                                    response = Q600(var);
                                }
                            }
                        }

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(">" + data + "\r\n");

                        if (!string.IsNullOrEmpty(response))
                        {
                            msg = System.Text.Encoding.ASCII.GetBytes(">" + response + "\r\n");
                        }

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
        }

        private string Q100()
        {
            return "SERIAL NUMBER, 1234567";
        }

        private string Q101()
        {
            return "SOFTWARE VERSION, 100.17.000.2037";
        }

        private string Q102()
        {
            return "MODEL, CSMD-G2";
        }

        private string Q104()
        {
            return "MODE, ZERO";
        }

        private string Q200()
        {
            return "TOOL CHANGES, 35";
        }

        private string Q201()
        {
            return "USING TOOL, 4";
        }

        private string Q300()
        {
            return "P.O. TIME, 06282:17:13";
        }

        private string Q301()
        {
            return "C.S.TIME, 00098:18:29";
        }

        private string Q303()
        {
            return "LAST CYCLE, 00000:00:13";
        }

        private string Q304()
        {
            return "PREV CYCLE, 00000:00:01";
        }

        private string Q402()
        {
            return "M30 #1, 380";
        }

        private string Q403()
        {
            return "M30 #2, 380";
        }

        private string Q500()
        {
            if (i == 0) return "PROGRAM, O12345, IDLE, PARTS, 380";
            if (i == 1) return "PROGRAM, O12345, ALARM ON, PARTS, 380";
            else return "STATUS BUSY";
        }

        private string Q600(int variable)
        {
            if (variable == 5041) return "MACRO, 234.567";
            if (variable == 5042) return "MACRO, 456.789";
            if (variable == 5043) return "MACRO, 567.891";
            else return "MACRO, 123.456";
        }
    }
}
