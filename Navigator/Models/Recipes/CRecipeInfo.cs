using System;

namespace NavigatorMachine.Models
{
    public class CRecipeInfo : ObservableObject
    {
        #region Properties
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		public string DateCreated { get; private set; }
        #endregion

        #region Constructors
        public CRecipeInfo(string name = "defaut")
        {
            Name = name;
            DateCreated = DateTime.Now.ToShortTimeString(); ;
        }
        #endregion

        #region Privates
        private string _name;
        #endregion

    }
}
