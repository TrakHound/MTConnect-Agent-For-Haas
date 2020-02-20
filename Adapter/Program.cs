// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;

namespace MTConnect.Adapters.Haas
{
    static class Program
    {
        private static Server server;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string mode = args[0];

                switch (mode)
                {
                    // Debug (Run as console application)
                    case "debug":

                        Start();
                        Console.ReadLine();
                        break;

                    // Install the Service
                    case "install":

                        InstallService();
                        break;

                    // Uninstall the Service
                    case "uninstall":

                        UninstallService();
                        break;
                }
            }
            else
            {
                // Start as Service
                ServiceBase.Run(new AdapterService());
            }
        }

        public static void Start()
        {
            server = new Server();
            server.Start();
        }

        public static void Stop()
        {
            if (server != null) server.Stop();
        }

        private static void InstallService()
        {
            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
        }

        private static void UninstallService()
        {
            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
        }
    }
}
