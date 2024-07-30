using System;

namespace NavigatorMachine.Devices.PlusE
{
    public class IOBoardBase 
    {
        public string Name { get; set; }
        public int Index { get; set; }

        public virtual IOInputBase Input { get; set; }
        public virtual IOOutputBase Output { get; set; }
    }
}
