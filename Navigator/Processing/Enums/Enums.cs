using System;

namespace NavigatorMachine.Processing.Enums
{
    public enum EMode
    {
        ToRun = 1,
        Run,
        ToPause, 
        Pause,
        ToStop,
        Stop,
    }

    public enum EPRtnCode
    {
        RtnFail = -1,   // Return code less than 0 is error

        RtnOk = 1,
    }
}
