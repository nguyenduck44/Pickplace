using System;

namespace NavigatorMachine.Models
{
    public class CWorkData : ObservableObject
    {
        #region Properties
        public int Total => OK + NG;
        public double RatePercentOK => OK != 0 ? double.Round( (OK * 100) * 1.0 /Total, 2) : 0;
        public double RatePercentNG => NG != 0 ? double.Round((NG * 100) * 1.0 / Total, 2) : 0;
        public double RatePercentTotal => Total != 0 ? double.Round((Total * 100) * 1.0 / Total, 2) : 0;

        public int OK
		{
			get { return _ok; }
			set
			{
				_ok = value;
				OnPropertyChanged(nameof(OK));
                OnPropertyChanged(nameof(NG));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(RatePercentOK));
                OnPropertyChanged(nameof(RatePercentNG));
            }
		}

        public int NO
        {
            get { return _no; }
            set
            {
                OnPropertyChanged(nameof(OK));
                OnPropertyChanged(nameof(NG));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(RatePercentOK));
                OnPropertyChanged(nameof(RatePercentNG));
            }
        }

        public int NG
		{
			get { return _ng; }
			set
			{
				_ng = value;
                OnPropertyChanged(nameof(OK));
                OnPropertyChanged(nameof(NG));
				OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(RatePercentOK));
                OnPropertyChanged(nameof(RatePercentNG));
            }
		}
        #endregion

        #region Methods
		public void Clear()
		{
			OK = 0;
			NG = 0;
		}
        #endregion

        #region Privates
        private int _ok;
        private int _ng;
        private int _no;
        #endregion
    }
}
