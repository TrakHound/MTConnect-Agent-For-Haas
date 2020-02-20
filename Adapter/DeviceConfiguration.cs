// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace MTConnect.Adapters.Haas
{
    public class DeviceConfiguration
    {
        public string DeviceName { get; set; }

        public int Port { get; set; }

        public string EthernetServer { get; set; }

        public int EthernetPort { get; set; }

        public int Heartbeat { get; set; }

        public string COMPort { get; set; }
    }


    public class Configuration
    {
        public Configuration() { Devices = new List<DeviceConfiguration>(); }

        public List<DeviceConfiguration> Devices { get; set; }


        public static Configuration Read()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "config.xml";
            if (File.Exists(path)) return Read(path);
            else return null;
        }

        public static Configuration Read(string path)
        {
            Configuration result = null;

            string rootPath;
            rootPath = Path.GetDirectoryName(path);
            rootPath += @"\";

            if (File.Exists(path))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(path);

                    result = new Configuration();

                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                    {
                        switch (node.Name.ToLower())
                        {
                            case "device": result.Devices.Add(ProcessDevice(node)); break;
                        }
                    }

                    Console.WriteLine("Configuration Successfully Read From : " + path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error During Configuration Read :: " + path);
                }
            }

            return result;
        }

        private static DeviceConfiguration ProcessDevice(XmlNode node)
        {
            var result = new DeviceConfiguration();

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    if (child.InnerText != "")
                    {
                        Type o = typeof(DeviceConfiguration);
                        PropertyInfo info = o.GetProperty(child.Name);

                        if (info != null)
                        {
                            Type t = info.PropertyType;
                            info.SetValue(result, Convert.ChangeType(child.InnerText, t), null);
                        }
                    }
                }
            }

            return result;
        }
    }
}
