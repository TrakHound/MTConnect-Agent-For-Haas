// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace MTConnect_Adapter_For_Haas
{
    partial class Monitor
    {
        const int HEARTBEAT = 1000;

        System.Timers.Timer requestTimer;

        private DeviceConfiguration configuration;

        private string comPort;

        private HaasAdapter adapter;

        public Monitor(DeviceConfiguration config)
        {
            configuration = config;
            comPort = config.COMPort;

            adapter = new HaasAdapter(config);
        }

        public void Start()
        {
            if (adapter != null) adapter.Start();

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

            if (requestTimer != null) requestTimer.Enabled = false;
        }


        #region "Standard Variables"

        private void ProcessQ100()
        {
            string[] response = Serial.SendCommand("Q100", comPort);
            if (response != null && response.Length > 1) Console.WriteLine("Q100 : " + response[1]);

            ProcessAvailability(response);
        }

        private void ProcessQ104()
        {
            string[] response = Serial.SendCommand("Q104", comPort);
            if (response != null && response.Length > 1)
            {
                Console.WriteLine("Q104 : " + response[1]);

                ProcessControllerMode(response[1]);
                ProcessZeroReturn(response[1]);
            }
        }

        private void ProcessQ500()
        {
            string[] response = Serial.SendCommand("Q500", comPort);
            if (response != null && response.Length > 1)
            {
                if (response[0] == "PROGRAM")
                {
                    if (response[1] != "MDI")
                    {
                        adapter.mProgram.Value = response[1];
                    }

                    if (response[2] == "IDLE") adapter.mExecution.Value = "READY";
                    else if (response[2] == "FEED HOLD") adapter.mExecution.Value = "INTERRUPTED";

                    if (response[2] == "ALARM ON")
                    {
                        adapter.mExecution.Value = "STOPPED";

                        adapter.mSystem.Add(MTConnect.Condition.Level.FAULT, "Alarm on indicator");
                    }
                    else
                    {
                        adapter.mEstop.Value = "ARMED";
                        adapter.mSystem.Add(MTConnect.Condition.Level.NORMAL);
                    }

                }
                else if (response[0] == "STATUS")
                {
                    if (response[1] == "BUSY")
                    {
                        adapter.mExecution.Value = "ACTIVE";
                    }
                }
            }
        }


        private void ProcessAvailability(string[] response)
        {
            if (response != null && response.Length > 1) adapter.mAvail.Value = "AVAILABLE";
            else adapter.mAvail.Value = "UNAVAILABLE";
            adapter.SendChanged();
        }

        private void ProcessProgramName(string response)
        {
            string r = response;

            if (response == "MDI") r = "";

            adapter.mMode.Value = r;
            adapter.SendChanged();
        }

        private void ProcessControllerMode(string response)
        {
            switch (response)
            {
                case "(MDI)": adapter.mMode.Value = "MANUAL_DATA_INPUT"; break;
                case "(JOG)": adapter.mMode.Value = "MANUAL"; break;
                case "(ZERO RET)": adapter.mMode.Value = "MANUAL"; break;
                default: adapter.mMode.Value = "AUTOMATIC"; break;
            }

            adapter.SendChanged();
        }

        private void ProcessZeroReturn(string response)
        {
            switch (response)
            {
                case "(ZERO RET)": adapter.mZeroRet.Add(MTConnect.Condition.Level.FAULT, "NO ZERO X"); break;
                default: adapter.mZeroRet.Add(MTConnect.Condition.Level.NORMAL); break;
            }

            adapter.SendChanged();
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
            string[] response = Serial.SendCommand("Q600 " + variable, comPort);
            if (response != null && response.Length > 2)
            {
                // Check to make sure the correct variable is returned
                if (response[1] == variable.ToString())
                {
                    return response[2];
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
