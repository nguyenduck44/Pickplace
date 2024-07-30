using System;

namespace NavigatorMachine.Models
{
    public class CData : ObservableObject
    {
        #region Properties
        public string Header { get; set; }
        public double OldValue
        {
            get { return _OldValue; }
            set
            {
                _OldValue = value;
                OnPropertyChanged();
            }
        }

        public double Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                OnPropertyChanged();
            }
        }

        public double? MaxValue
        {
            get { return _MaxValue; }
            set
            {
                _MaxValue = value;
                OnPropertyChanged();
            }
        }

        public double? MinValue
        {
            get { return _MinValue; }
            set
            {
                _MinValue = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Privates
        private double _Value;
        private double _OldValue;

        private double? _MinValue;
        private double? _MaxValue;
        #endregion

        #region Contructor
        #endregion
    }
}
