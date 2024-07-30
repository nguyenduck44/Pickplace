﻿using System;

namespace NavigatorMachine.Devices.PlusE
{
    public class IOOutputBase
    {
        public int Index { get; set; }
        public string Name
        {
            get { return $"[OUT]{_Name}"; }
            set
            {
                _Name = value;
            }
        }

        internal virtual void SetOutput(int index, bool value)
        {
#if Simulation
            output[index] = value;
#else
            // This function must be implemented in child class(es)
            throw new NotImplementedException();
#endif
        }

        internal virtual bool GetOutput(int index)
        {
#if Simulation
            return output[index];
#else
            // This function must be implemented in child class(es)
            throw new NotImplementedException();
#endif
        }

        public bool this[int index]
        {
            get { return GetOutput(index); }
            set
            {
                SetOutput(index, value);
            }
        }

        public IOOutputBase(string name, int index)
        {
            Name = name;
            Index = index;
        }

        #region Privates
        private string _Name;
        private bool[] output { get; set; } = new bool[256];
        #endregion
    }
}
