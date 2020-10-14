// Copyright (c) 2020 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using NLog;

namespace MTConnect.Adapters.Haas
{
    public class HaasAdapter
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        // Conditions
        public Condition mZeroRet;
        public Condition mSystem;

        // Events
        public Event mMessage;
        public Event mEstop;
        public Event mExecution;
        public Event mPartCount;
        public Event mProgram;
        public Event mMode;
        public Event mAvail;

        // Samples
        public Sample mXact;
        public Sample mYact;
        public Sample mZact;
        public Sample mSpindleSpeed;


        private Adapter adapter;

        private DeviceConfiguration configuration;

        public HaasAdapter(DeviceConfiguration config)
        {
            configuration = config;

            adapter = new Adapter();
            adapter.Port = config.Port;
            adapter.Heartbeat = config.Heartbeat;

            AddDataItems();
        }

        public void Start()
        {
            adapter.Start();
        }

        public void Stop()
        {
            if (adapter != null) adapter.Stop();
        }

        public void SendChanged()
        {
            if (adapter != null) adapter.SendChanged();
            else log.Warn("Error Sending Changed DataItems");
        }


        private void AddDataItems()
        {
            AddEvents();
            AddConditions();
            AddSamples();
        }

        private void AddConditions()
        {
            mZeroRet = new Condition("zero_ret");
            mZeroRet.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mZeroRet);

            mSystem = new Condition("system", true);
            mSystem.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mSystem);
        }

        private void AddEvents()
        {
            mAvail = new Event("avail");
            mAvail.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mAvail);

            mMessage = new Event("msg");
            mMessage.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mMessage);

            mEstop = new Event("estop");
            mEstop.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mEstop);

            mExecution = new Event("execution");
            mExecution.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mExecution);

            mPartCount = new Event("part_count");
            mPartCount.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mPartCount);

            mProgram = new Event("program");
            mProgram.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mProgram);

            mMode = new Event("mode");
            mMode.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mMode);
        }

        private void AddSamples()
        {
            mXact = new Sample("x_act");
            mXact.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mXact);

            mYact = new Sample("y_act");
            mYact.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mYact);

            mZact = new Sample("z_act");
            mZact.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mZact);

            mSpindleSpeed = new Sample("speed");
            mSpindleSpeed.DevicePrefix = configuration.DeviceName;
            adapter.AddDataItem(mSpindleSpeed);
        }
    }
}
