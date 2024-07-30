#region origin
using NavigatorMachine.Defines;
using NavigatorMachine.Models;
using NavigatorMachine.MVVM.ViewModels;
using NavigatorMachine.Processing.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopEquipment.Domain.Process;

namespace NavigatorMachine.Processing
{
    public class CProcessRobot : CProcessBase
    {
        private TrayViewModel _trayVM => CDef.MainWindowVM.AutoVM.TrayVM;
        public int WorkIndex { get; set; } = 1;

        public CCell CurrentCell
        {
            get
            {
                return _trayVM.CurrentTray.First(c => c.Index == WorkIndex);
            }
        }

        public string IndexStatus { get; set; } = "None";

        public bool InputTrigerFromRobot
        {
            get
            {
                return CDef.IO.InputTrigerFromRobot;
            }
        }

        public bool OutputTriggerVisionInspect
        {
            get
            {
                return CDef.IO.OutputTriggerVisionInspect;
            }
            set
            {
                CDef.IO.OutputTriggerVisionInspect = value;
            }
        }

        public bool Output_SWControlRobot
        {
            get
            {
                return CDef.IO.Output_SWControlRobot;
            }
            set
            {
                CDef.IO.Output_SWControlRobot = value;
            }
        }

        public CProcessRobot(string processName) : base(processName)
        {
        }

        public override EPRtnCode ProcessToRun()
        {
            Mode = EMode.Run;

            // Reset tray khi chuyển từ Stop sang Run
            if (Mode == EMode.Run && Mode == EMode.Stop)
            {
                _trayVM.ResetAllButtonCommand.Execute(null);
            }

            return base.ProcessToRun();
        }

        public override EPRtnCode ProcessToPause()
        {
            Mode = EMode.Pause;
            if (lastindexResult)
            {
                Task.Run(() =>
                {
                    while (Mode == EMode.Pause)
                    {
                        CDef.IO.Output_TowerLampGreen = true;
                        Sleep(700);
                        CDef.IO.Output_TowerLampGreen = false;
                        Sleep(1000);
                    }
                });
            }

            return base.ProcessToPause();
        }

        public override EPRtnCode ProcessToStop()
        {
            Mode = EMode.Stop;
            return base.ProcessToStop();
        }

        int Step_ReceiveSWControl = 0;
        int Step_ReceiveSWReset = 0;

        public override EPRtnCode PreProcess()
        {
            switch (Step_ReceiveSWControl)
            {
                case 0:
                    if (CDef.IO.Input_SWControl_Robot && !CDef.IO.Output_SWControlRobot)
                    {
                        Step_ReceiveSWControl++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;

                case 1:
                    if (CDef.IO.Input_SWControl_Robot)
                    {
                        Sleep(10);
                        break;
                    }

                    Step_ReceiveSWControl++;
                    break;

                case 2:
                    switch (Mode)
                    {
                        case EMode.ToRun:
                            Mode = EMode.ToPause;
                            break;
                        case EMode.Run:
                            Mode = EMode.ToPause;
                            break;
                        case EMode.ToPause:
                            Mode = EMode.ToRun;
                            break;
                        case EMode.Pause:
                            buzzer_Working = false;
                            Mode = EMode.ToRun;
                            break;
                        case EMode.ToStop:
                            Mode = EMode.ToRun;
                            break;
                        case EMode.Stop:
                            Mode = EMode.ToRun;
                            break;
                    }

                    Step_ReceiveSWControl = 0;
                    break;
            }

            switch (Step_ReceiveSWReset)
            {
                case 0:
                    if (CDef.IO.Input_SWReset_Robot && Mode != EMode.ToRun && Mode != EMode.ToRun)
                    {
                        Step_ReceiveSWReset++;
                    }
                    else
                    {
                        Sleep(10);
                    }
                    break;

                case 1:
                    if (CDef.IO.Input_SWReset_Robot)
                    {
                        Sleep(10);
                        break;
                    }

                    Step_ReceiveSWReset++;
                    break;

                case 2:
                    CDef.MainWindowVM.AutoVM.TrayVM.ResetAllButtonCommand.Execute(null);
                    IndexStatus = "None";
                    OnPropertyChanged(nameof(IndexStatus));
                    Mode = EMode.ToStop;
                    Step_ReceiveSWReset = 0;
                    break;
            }
            return base.PreProcess();
        }

        bool lastindexResult = false;
        bool taskBuzzerRun = false;

        List<bool> ListVisionResult = new List<bool>();
        int counter_TriggerInCycle = 0;

        public override EPRtnCode ProcessRun()
        {
            switch ((EVisionInspectRunStep)Step.RunStep)
            {
                case EVisionInspectRunStep.Start:
                    LOG.Debug("Start");

                    OutputTriggerVisionInspect = false;
                    CDef.IO.Output_SWControlRobot = false;
                    CDef.IO.Output_TowerLampGreen = true;
                    CDef.IO.Output_TowerLampRed = false;
                    SetWorkIndex();
                    ProcessTimer.TaktTimeWatcher[1] = Environment.TickCount;
                    Step.RunStep++;
                    break;

                case EVisionInspectRunStep.TrayCheckFirst:
                    if (_trayVM.IsTrayInspectDone)
                    {
                        Step.RunStep = (int)EVisionInspectRunStep.Buzzer_WorkComplete;
                    }
                    else
                    {
                        LOG.Debug("Wait Input Signal from Robot...");
                        counter_TriggerInCycle = 0;
                        ListVisionResult.Clear();

                        ProcessTimer.TaktTimeWatcher[0] = Environment.TickCount;
                        CurrentCell.Status = EStatus.Processing;
                        IndexStatus = "None";
                        OnPropertyChanged(nameof(IndexStatus));

                        Step.RunStep++;
                    }
                    break;

                case EVisionInspectRunStep.WaitRobotTriggerOn:
                    if (CDef.IO.InputTrigerFromRobot)
                    {
                        LOG.Debug("Input Signalfrom Robot is ON");
                        Step.RunStep++;
                    }
                    else
                    {
                        if (ProcessTimer.StepLeadTime < CDef.CommonRecipe.TimeOut_ReceiveSignalInput * 1000)
                        {
                            Sleep(10);
                        }
                        else 
                        {
                            LOG.Warn("Wait Input signal from Robot time out");
                            Mode = EMode.ToStop;
                            ////MessageBox.Show("Wait Input signal from Robot time out");
                        }
                    }
                    
                    break;

                case EVisionInspectRunStep.VisionStartCheck:
                    if (ProcessTimer.StepLeadTime < CDef.CommonRecipe.DelayTime_VisionInspect * 1000)
                    {
                        break;
                    }
                    LOG.Debug("SET ON Output Trigger Inspect to Camera!");
                    LOG.Debug($" Pause robot continue Vision check");
                    Output_SWControlRobot = true;
                    OutputTriggerVisionInspect = true;
                    Sleep(30);
                    LOG.Debug("SET OFF Output Trigger Inspect to Camera!");
                    OutputTriggerVisionInspect = false;
                    Output_SWControlRobot = false;
                    LOG.Debug($" Vision Inspect Start.....");
                    Sleep(10);
                    Step.RunStep++;
                    break;

                case EVisionInspectRunStep.WaitVisionCheckDone:
                    // Cập nhật kết quả trước khi chờ VisionCheckDone
                    if (CDef.IO.InputVisionInspectOK)
                    {
                        ListVisionResult.Add(true);
                    }
                    else
                    {
                        ListVisionResult.Add(false);
                    }

                    if (CDef.IO.InputVisionChecking)
                    {
                            // Vision check done ! robot continues
                            //Sleep(400);
                            //LOG.Debug($" Continues Vision next index ");
                            //Output_SWControlRobot = true;
                            //Sleep(30);
                            //LOG.Debug($" Continues Vision next index Done ! ");
                            //Output_SWControlRobot = false;
                    }

                    LOG.Debug($" Vision Inspect Done");
                    Step.RunStep++;
                    break;

                case EVisionInspectRunStep.GetVisionResult:
                    Step.RunStep++;
                    break;
                case EVisionInspectRunStep.WaitRobotTriggerOff:
                    if (CDef.IO.InputTrigerFromRobot)
                    {
                        break;
                    }

                    counter_TriggerInCycle++;
                    LOG.Debug("Input Signal from Robot is OFF");
                    Step.RunStep++;
                    break;

                case EVisionInspectRunStep.UpdateData:
                    if (counter_TriggerInCycle < CDef.CommonRecipe.TriggerInCycleCount)
                    {
                        Step.RunStep = (int)EVisionInspectRunStep.WaitRobotTriggerOff;
                        Sleep(100);
                        Step.RunStep++;
                        break;
                    }

                    if (ListVisionResult.Count(result => result == false) == 0)
                    {
                        LOG.Debug($"Index: {CurrentCell.Index}, Result: OK");
                        CurrentCell.Status = EStatus.OK;
                        lastindexResult = true;
                        IndexStatus = "OK";
                        CDef.WorkData.OK++;
                    }

                    else if (CDef.IO.Input_No_Production)
                    {
                        LOG.Debug($"Index: {CurrentCell.Index}, Result: NO");
                        CurrentCell.Status = EStatus.NO;
                        lastindexResult = false;
                        IndexStatus = "NO";
                        CDef.WorkData.NO++;
                    }

                    else
                    {
                        LOG.Debug($"Index: {CurrentCell.Index}, Result: NG");
                        CurrentCell.Status = EStatus.NG;
                        lastindexResult = false;
                        IndexStatus = "NG";
                        CDef.WorkData.NG++;
                    }
                    OnPropertyChanged(nameof(IndexStatus));

                    CDef.TaktTime.LastIndex = Math.Round((Environment.TickCount - ProcessTimer.TaktTimeWatcher[0]) / 1000.0, 3);
                    int lastIndex = CDef.TrayRecipe.Rows * CDef.TrayRecipe.Columns;

                    if (CDef.CommonRecipe.StopMachineIfVisionInspectNg && CurrentCell.Status == EStatus.NG && WorkIndex != lastIndex)
                    {
                        CDef.IO.Output_TowerLampGreen = false;
                        ProcessTimer.TaktTimeWatcher[2] = Environment.TickCount;
                        CDef.IO.Press_SWControlRobot();
                        Mode = EMode.ToPause;

                        Task.Run(() =>
                        {
                            if (taskBuzzerRun) return;

                            taskBuzzerRun = true;

                            while (Environment.TickCount - ProcessTimer.TaktTimeWatcher[2] < 10000 && Mode != EMode.ToRun && Mode != EMode.Run)
                            {
                                CDef.IO.Output_Buzzer = true;
                                Sleep(700);
                                CDef.IO.Output_Buzzer = false;
                                Sleep(1000);
                            }

                            taskBuzzerRun = false;
                            CDef.IO.Output_Buzzer = false;
                        });

                        Task.Run(() =>
                        {
                            while (Mode == EMode.ToPause || Mode == EMode.Pause)
                            {
                                CDef.IO.Output_TowerLampRed = true;
                                Sleep(700);
                                CDef.IO.Output_TowerLampRed = false;
                                Sleep(1000);
                            }
                        });
                        break;

                    }
                    WorkIndex++;
                    Step.RunStep = (int)EVisionInspectRunStep.TrayCheckFirst;

                    // Sau khi cập nhật xong workindex thì kích hoạt robot
                    //Sleep(10);
                    LOG.Debug($" Continues Vision next index ");
                    Output_SWControlRobot = true;
                    Sleep(30);
                    LOG.Debug($" Continues Vision next index Done ! ");
                    Output_SWControlRobot = false;
                    //Sleep(20);
                    break;

                case EVisionInspectRunStep.Buzzer_WorkComplete:
                    CDef.IO.Output_Buzzer = true;
                    Sleep(2000);
                    CDef.IO.Output_Buzzer = false;
                    Step.RunStep++;
                    break;

                case EVisionInspectRunStep.SaveDataToExcelFile:
                    Step.RunStep++;
                    break;

                case EVisionInspectRunStep.End:
                    LOG.Debug("Process End");
                    CDef.TaktTime.LastTray = Math.Round((Environment.TickCount - ProcessTimer.TaktTimeWatcher[1]) / 1000.0, 3);
                    Mode = EMode.ToStop;
                    break;

                default:
                    Sleep(20);
                    break;
            }

            return base.ProcessRun();
        }

        private void SetWorkIndex()
        {
            if (_trayVM.IsTrayInspectDone || _trayVM.CurrentTray.Count(c => c.Status > EStatus.None) == 0)
            {
                WorkIndex = 1;
                _trayVM.ResetAllButtonCommand.Execute(null);
            }
            else
            {
                int count = CDef.TrayRecipe.Rows * CDef.TrayRecipe.Columns;
                for (int i = 1; i <= count; i++)
                {
                    if (_trayVM.CurrentTray.First(c => c.Index == i).Status <= EStatus.Processing)
                    {
                        WorkIndex = i;
                        return;
                    }
                }
            }
            LOG.Debug($"Work Index Start is index #{WorkIndex}");
        }

        bool buzzer_Working = false;

        public override EPRtnCode ProcessPause()
        {
            return base.ProcessPause();
        }

        #region Privates
        // Private methods and variables can be added here if needed
        #endregion
    }

    public enum EVisionInspectRunStep
    {
        Start,
        TrayCheckFirst,
        WaitRobotTriggerOn,
        VisionStartCheck,
        WaitRobotTriggerOff,
        WaitVisionCheckDone,
        GetVisionResult,
        UpdateData,
        Buzzer_WorkComplete,
        SaveDataToExcelFile,

        End
    }
}
#endregion
