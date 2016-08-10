// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;

namespace MTConnect_Adapter_For_Haas
{
    public class Server
    {
        private List<Monitor> deviceMonitors = new List<Monitor>();

        public Server()
        {
            var config = Configuration.Read();
            if (config != null)
            {
                foreach (var device in config.Devices)
                {
                    var monitor = new Monitor(device);
                    deviceMonitors.Add(monitor);
                }
            }
        }

        public void Start()
        {
            foreach (var deviceMonitor in deviceMonitors) deviceMonitor.Start();
        }

        public void Stop()
        {
            foreach (var deviceMonitor in deviceMonitors) deviceMonitor.Stop();
        }
    }
}
