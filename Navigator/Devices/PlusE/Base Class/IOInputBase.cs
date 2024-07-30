﻿using System;

namespace NavigatorMachine.Devices.PlusE
{
    public class IOInputBase 
    {
        public virtual bool[] SimulationInputValue { get; set; } = new bool[256];
        public int Index { get; set; }
        public string Name
        {
            get { return $"[IN]{_Name}"; }
            set
            {
                _Name = value;
            }
        }

        internal virtual bool GetInput(int index)
        {
#if Simulation
            return SimulationInputValue[index];
#else
            // This function must be implemented in child class(es)
            throw new NotImplementedException();
#endif
        }

        public bool this[int index]
        {
            get
            {
                return GetInput(index);
            }
        }

        public IOInputBase(string name, int index)
        {
            Name = name;
            Index = index;
        }

        #region Privates
        private string _Name;
        #endregion
    }
}
