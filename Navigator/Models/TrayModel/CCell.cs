using System;

namespace NavigatorMachine.Models
{
    public class CCell : ObservableObject
    {
        #region Properties
        public int Index
		{
			get { return _index; }
			set
			{
                _index = value;
				OnPropertyChanged();
			}
		}

		public EStatus Status
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged();
			}
		}
        #endregion

        #region Privates
        private int _index;
        private EStatus _status;
        #endregion
    }
}
