// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace MTConnect.Adapters.Haas
{
    public static class Serial
    {
        public static string[] SendCommand(string cmd, string port)
        {
            string[] result = null;

            var serial = new SerialPort(port);

            // Set serial options
            serial.BaudRate = 19200;
            serial.Parity = Parity.None;
            serial.StopBits = StopBits.One;
            serial.DataBits = 7;
            serial.ReadTimeout = 10000;

            try
            {
                // Open serial port connection
                serial.Open();

                // Write the command and newline to serial
                serial.Write(cmd + "\r\n");

                Thread.Sleep(1000);

                // Read response
                string response = serial.ReadExisting();

                // Split string and return array of response values
                var list = new List<string>();
                string[] values = response.Split(new char[] { ',', ' ' });
                for (var x = 1; x <= values.Length - 1; x++)
                {
                    string val = values[x].Trim();
                    if (!String.IsNullOrEmpty(val)) list.Add(val);
                }

                result = list.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendCommand() :: Exception :: " + ex.Message);
            }
            finally
            {
                // Close serial connection
                if (serial.IsOpen) serial.Close();
            }

            return result;
        }
    }
}
