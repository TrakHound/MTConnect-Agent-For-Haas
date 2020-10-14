// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using NLog;
using System.Collections.Generic;

namespace MTConnect.Adapters.Haas
{
    public class Server
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private List<SerialMonitor> serialMonitors = new List<SerialMonitor>();
        private List<EthernetMonitor> ethernetMonitors = new List<EthernetMonitor>();

        public Server()
        {
            var config = Configuration.Read();
            if (config != null)
            {
                foreach (var device in config.Devices)
                {
                    if (!string.IsNullOrEmpty(device.COMPort))
                    {
                        var monitor = new SerialMonitor(device);
                        serialMonitors.Add(monitor);
                        log.Info("Serial Monitor Created");
                    }
                    else
                    {
                        var monitor = new EthernetMonitor(device);
                        ethernetMonitors.Add(monitor);
                        log.Info("Ethernet Monitor Created : " + device.EthernetServer + " : " + device.EthernetPort + " : " + device.DeviceName);
                    }
                }
            }
        }

        public void Start()
        {
            foreach (var deviceMonitor in serialMonitors) deviceMonitor.Start();
            foreach (var deviceMonitor in ethernetMonitors) deviceMonitor.Start();
        }

        public void Stop()
        {
            foreach (var deviceMonitor in serialMonitors) deviceMonitor.Stop();
            foreach (var deviceMonitor in ethernetMonitors) deviceMonitor.Stop();
        }
    }
}
