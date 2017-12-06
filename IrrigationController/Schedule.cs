using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace IrrigationController
{
    public sealed class Schedule
    {
        private bool isSet = false;
        private int id;

        // The date and time to start - the date will increment forward on the interval
        private DateTime schedStart;

        // Days between scheduled irrigation events
        private double schedInterval;

        // Which solenoid valves to operate each time the schedule runs
        public List<string> targetAreas;

        // How long do we run the pump per scheduled run
        private int waterDuration;

        public Schedule(bool isSet, int id)
        {
            this.isSet = isSet;
            this.id = id;
        }

        public DateTime SchedStart { get => schedStart; set => schedStart = value; }
        public int WaterDuration { get => waterDuration; set => waterDuration = value; }
        public double SchedInterval { get => schedInterval; set => schedInterval = value; }
        public bool IsSet { get => isSet; set => isSet = value; }
        public int Id { get => id; set => id = value; }

        public void setSchedStart(TimeSpan userSelectedStart)
        {
            DateTime start = DateTime.Today;
            SchedStart = start.Add(userSelectedStart);

        }

    }

}
