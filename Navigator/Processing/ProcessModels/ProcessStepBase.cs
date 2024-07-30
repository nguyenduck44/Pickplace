using System;

namespace TopEquipment.Domain.Process
{
    public class CProcessStep 
    {
        public delegate void StepChangeDelegate();
        public StepChangeDelegate StepChangedHandler { get; set; }

        public int IdleStep
        {
            get => _idleStep;
            set
            {
                _idleStep = value;
                StepChangedHandler();
            }
        }

        public int ToHomeStep
        {
            get => _toHomeStep;
            set
            {
                _toHomeStep = value;
                StepChangedHandler();
            }
        }

        public int HomeStep
        {
            get => _homeStep;
            set
            {
                _homeStep = value;
                StepChangedHandler();
            }
        }

        public int ToRunStep
        {
            get { return _toRunStep; }
            set
            {
                _toRunStep = value;
                StepChangedHandler();
            }
        }

        public int RunStep
        {
            get { return _runStep; }
            set
            {
                _runStep = value;
                StepChangedHandler();
            }
        }

        public int SubStep { get; set; }
        public int SpareStep1 { get; set; }
        public int SpareStep2 { get; set; }
        public int SpareStep3 { get; set; }
        public int SpareStep4 { get; set; }
        public int SpareStep5 { get; set; }

        private int _idleStep;
        private int _toHomeStep;
        private int _homeStep;
        private int _runStep;
        private int _toRunStep;
    }
}
