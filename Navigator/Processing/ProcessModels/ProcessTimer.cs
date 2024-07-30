using NavigatorMachine.Models;
using System;

namespace TopEquipment.Domain.Process
{
    public class CProcessTimer : ObservableObject
    { 
        public int StepStartTime { get; set; }

        public int Now { get => Environment.TickCount; }
        public int StepLeadTime => Now - StepStartTime;

        public int[] TaktTimeWatcher { get; } = new int[10];
    }
}
