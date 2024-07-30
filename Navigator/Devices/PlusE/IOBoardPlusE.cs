using System;

namespace NavigatorMachine.Devices.PlusE
{
    public class IOBoardPlusE : IOBoardBase
    {
        public IOBoardPlusE(string name, int index)
        {
            Name = name;
            Index = index;

            Input = new IOInputPlusE(Name, Index);
            Output = new IOOutputPlusE(Name, Index);
        }
    }
}
