// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using NLog;
using System;
using System.Text.RegularExpressions;

namespace MTConnect.Adapters.Haas
{
    partial class EthernetMonitor
    {
        const int HEARTBEAT = 500;

        private static Logger log = LogManager.GetCurrentClassLogger();

        private System.Timers.Timer requestTimer;

        private DeviceConfiguration configuration;

        private string server;

        private int port;

        private HaasAdapter adapter;
        private Ethernet ethernet;

        public EthernetMonitor(DeviceConfiguration config)
        {
            configuration = config;
            server = config.EthernetServer;
            port = config.EthernetPort;

            adapter = new HaasAdapter(config);
            ethernet = new Ethernet(server, port);
        }

        public void Start()
        {
            if (adapter != null) adapter.Start();
            if (ethernet != null) ethernet.Connect();

            requestTimer = new System.Timers.Timer();
            requestTimer.Interval = HEARTBEAT;
            requestTimer.Elapsed += RequestTimer_Elapsed;
            requestTimer.Enabled = true;
        }

        private void RequestTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            requestTimer.Enabled = false;

            ProcessQ100();
            ProcessQ104();
            ProcessQ500();
            ProcessQ600();

            requestTimer.Enabled = true;
        }

        public void Stop()
        {
            if (adapter != null) adapter.Stop();
            if (ethernet != null) ethernet.Close();
            if (requestTimer != null) requestTimer.Enabled = false;
        }


        #region "Standard Variables"

        private void ProcessQ100()
        {
            var response = ethernet.SendCommand("?Q100");
            if (!string.IsNullOrEmpty(response))
            {
                log.Info("Q100 : " + response);

                ProcessAvailability(response);
            }
        }

        private void ProcessQ104()
        {
            string response = ethernet.SendCommand("?Q104");
            if (!string.IsNullOrEmpty(response))
            {
                log.Info("Q104 : " + response);

                ProcessControllerMode(response);
                ProcessZeroReturn(response);
            }
        }

        private void ProcessQ500()
        {
            string response = ethernet.SendCommand("?Q500");
            if (!string.IsNullOrEmpty(response))
            {
                log.Info("Q500 : " + response);

                ProcessExecution(response);
                ProcessEmergencyStop(response);
                ProcessProgramName(response);
                ProcessPartCount(response);
            }
        }


        private void ProcessAvailability(string response)
        {
            var pattern = "^>SERIAL NUMBER, (.*)$";
            var match = new Regex(pattern).Match(response);
            if (match.Success) adapter.mAvail.Value = "AVAILABLE";
            else adapter.mAvail.Value = "UNAVAILABLE";

            adapter.SendChanged();
        }

        private void ProcessExecution(string response)
        {
            var pattern = "^>PROGRAM, .*, (.*), PARTS, [0-9]*$";
            var match = new Regex(pattern).Match(response);
            if (match.Success && match.Groups.Count > 1)
            {
                var val = match.Groups[1].ToString();
                if (val == "IDLE") adapter.mExecution.Value = "READY";
                else if (val == "FEED HOLD") adapter.mExecution.Value = "INTERRUPTED";
                else if (val == "ALARM ON") adapter.mExecution.Value = "STOPPED";
            }

            pattern = "^>STATUS (.*)$";
            match = new Regex(pattern).Match(response);
            if (match.Success && match.Groups.Count > 1)
            {
                var val = match.Groups[1].ToString();
                if (val == "BUSY") adapter.mExecution.Value = "ACTIVE";
            }

            adapter.SendChanged();
        }

        private void ProcessEmergencyStop(string response)
        {
            var pattern = "^>PROGRAM, .*, (.*), PARTS, [0-9]*$";
            var match = new Regex(pattern).Match(response);
            if (match.Success && match.Groups.Count > 1)
            {
                var val = match.Groups[1].ToString();
                if (val == "ALARM ON")
                {
                    adapter.mEstop.Value = "TRIGGERED";
                    adapter.mSystem.Add(Condition.Level.FAULT, "Alarm on indicator");
                }
                else
                {
                    adapter.mEstop.Value = "ARMED";
                    adapter.mSystem.Normal();
                }
            }
            else
            {
                adapter.mEstop.Value = "ARMED";
                adapter.mSystem.Normal();
            }

            adapter.SendChanged();
        }

        private void ProcessProgramName(string response)
        {
            var pattern = "^>PROGRAM, (.*), .*, PARTS, [0-9]*$";
            var match = new Regex(pattern).Match(response);
            if (match.Success && match.Groups.Count > 1)
            {
                var val = match.Groups[1].ToString();
                if (val == "MDI") adapter.mProgram.Value = "";
                else adapter.mProgram.Value = val;

                adapter.SendChanged();
            }
        }

        private void ProcessPartCount(string response)
        {
            var pattern = "^>PROGRAM, .*, .*, PARTS, ([0-9]*)$";
            var match = new Regex(pattern).Match(response);
            if (match.Success && match.Groups.Count > 1)
            {
                adapter.mPartCount.Value = match.Groups[1].ToString();
                adapter.SendChanged();
            }
        }

        private void ProcessControllerMode(string response)
        {
            var pattern = "^>MODE, (.*)$";
            var match = new Regex(pattern).Match(response);
            if (match.Success && match.Groups.Count > 1)
            {
                var val = match.Groups[1].ToString();
                switch (val)
                {
                    case "(MDI)": adapter.mMode.Value = "MANUAL_DATA_INPUT"; break;
                    case "(JOG)": adapter.mMode.Value = "MANUAL"; break;
                    case "(ZERO RET)": adapter.mMode.Value = "MANUAL"; break;
                    default: adapter.mMode.Value = "AUTOMATIC"; break;
                }

                adapter.SendChanged();
            }
        }

        private void ProcessZeroReturn(string response)
        {
            var pattern = "^>MODE, (.*)$";
            var match = new Regex(pattern).Match(response);
            if (match.Success && match.Groups.Count > 1)
            {
                var val = match.Groups[1].ToString();
                switch (val)
                {
                    case "(ZERO RET)": adapter.mZeroRet.Add(Condition.Level.FAULT, "NO ZERO X"); break;
                    default: adapter.mZeroRet.Add(Condition.Level.NORMAL); break;
                }

                adapter.SendChanged();
            }
        }

        #endregion

        #region "Custom Variables (Q600)"

        private void ProcessQ600()
        {
            ProcessAxisActualPositions();
            ProcessSpindle();
        }

        private string GetVariable(int variable)
        {
            string response = ethernet.SendCommand("?Q600 " + variable);
            if (!string.IsNullOrEmpty(response))
            {
                log.Info("Q600 : " + variable.ToString() + " : " + response);

                var pattern = "^>MACRO, (.*)$";
                var match = new Regex(pattern).Match(response);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].ToString();
                }
            }

            return null;
        }

        #region "Axis Positions"

        private void ProcessAxisActualPositions()
        {
            ProcessAxisActualPosition_X();
            ProcessAxisActualPosition_Y();
            ProcessAxisActualPosition_Z();
        }

        private void ProcessAxisActualPosition_X()
        {
            string s = GetVariable(5041);
            if (s != null)
            {
                adapter.mXact.Value = s;
                adapter.SendChanged();
            }
        }

        private void ProcessAxisActualPosition_Y()
        {
            string s = GetVariable(5042);
            if (s != null)
            {
                adapter.mYact.Value = s;
                adapter.SendChanged();
            }
        }

        private void ProcessAxisActualPosition_Z()
        {
            string s = GetVariable(5043);
            if (s != null)
            {
                adapter.mZact.Value = s;
                adapter.SendChanged();
            }
        }

        #endregion

        #region "Spindle"

        private void ProcessSpindle()
        {
            ProcessSpindle_Speed();
        }

        private void ProcessSpindle_Speed()
        {
            string s = GetVariable(3027);
            if (s != null)
            {
                adapter.mSpindleSpeed.Value = s;
                adapter.SendChanged();
            }
        }

        #endregion

        #endregion

    }
}
