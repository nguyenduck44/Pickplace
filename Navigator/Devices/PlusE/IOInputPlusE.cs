using System;
using NativeLib = FASTECH.EziMOTIONPlusELib;

namespace NavigatorMachine.Devices.PlusE
{
    public class IOInputPlusE : IOInputBase
    {
        private uint[] inputPinMask = new uint[] {
            (uint)EInputPin_PlusE.User_IN0, // đầu vào triger đến từ robot
            (uint)EInputPin_PlusE.User_IN1, // đầu vào vision busy đến từ keygence
            (uint)EInputPin_PlusE.User_IN2, // đầu vào vision Ok đến từ keygence
            (uint)EInputPin_PlusE.User_IN3, // đầu vào vision NG đến từ keygence
            (uint)EInputPin_PlusE.User_IN4, // đầu vào nút bấm start đến từ robot
            (uint)EInputPin_PlusE.User_IN5, // đầu vào nút bấm stop đến từ robot
            (uint)EInputPin_PlusE.PT_A5,    // đầu vào không có hàng đến từ keygence
            (uint)EInputPin_PlusE.PT_A6,
            (uint)EInputPin_PlusE.PT_A7,
        };

        internal override bool GetInput(int pinNumber)
        {
#if Simulation
            return base.GetInput(pinNumber);
#else
            uint ioInput = 0;
            NativeLib.FAS_GetIOInput(Index, ref ioInput);

            return (ioInput & inputPinMask[pinNumber]) == inputPinMask[pinNumber];
#endif
        }

        public IOInputPlusE(string name, int index)
            : base(name, index)
        {
        }
    }
}
