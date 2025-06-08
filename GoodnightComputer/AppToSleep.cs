using System;

namespace GoodnightComputer
{
    public class AppToSleep
    {
        public string Name { get; set; }
        public string ProcessName { get; set; }
        public Guid ScheduleGuid { get; set; }

        public AppToSleep(string name, string processName, Guid scheduleGuid)
        {
            Name = name;
            ProcessName = processName;
            ScheduleGuid = scheduleGuid;
        }
    }
}
