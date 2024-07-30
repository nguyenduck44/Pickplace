using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NavigatorMachine.Models
{
    public class CTaktTime : ObservableObject
    {
        #region Properties
        public double LastIndex
		{
			get { return _lastIndex; }
			set
			{ 
				if (ListTaktTime != null)
				{
					ListTaktTime.Add(value);
				}

				_lastIndex = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(AvgTaktTime));
            }
        }

		public double LastTray
		{
			get { return _lastTray; }
			set
			{ 
				_lastTray = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<double> ListTaktTime
		{
			get { return _listTaktTime; }
			set
			{ 
				_listTaktTime = value;
            }
		}

		public double AvgTaktTime => ListTaktTime.Count > 0 ? Math.Round(ListTaktTime.Average(),3) : 0;
        #endregion
        
		#region Methods
        public void Clear()
        {
            LastIndex = 0;
			LastTray = 0;
			ListTaktTime.Clear();
			OnPropertyChanged(nameof(AvgTaktTime));
			OnPropertyChanged(nameof(LastTray));
			OnPropertyChanged(nameof(LastIndex));
        }
        #endregion
       
		#region Privates
        private double _lastIndex;
		private double _lastTray;
        private ObservableCollection<double> _listTaktTime = new ObservableCollection<double>();
        #endregion

    }
}
