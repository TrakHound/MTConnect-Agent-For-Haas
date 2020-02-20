// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Simulators.Haas
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true) new TcpServer(5051);
        }
    }
}
