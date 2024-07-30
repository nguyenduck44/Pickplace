using NavigatorMachine.Core;

namespace NavigatorMachine.Models
{
    public enum ETrayDirection
    {
        Normal,
        ZigZag,
    }
    public class CTrayRecipe : CRecipeBase, ITrayRecipe
    {
        #region Properties
        public int Rows
        {
            get { return _rows; }
            set
            {
                _rows = value;
                OnPropertyChanged();

                ChangeColumnRowEvent?.Invoke();
                Save();
            }
        }

        public int Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
                OnPropertyChanged();

                ChangeColumnRowEvent?.Invoke();
                Save();
            }
        }


        public ETrayDirection Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                OnPropertyChanged();

                ChangeColumnRowEvent?.Invoke();
                Save();
            }
        }

        #endregion

        #region Constructors
        #endregion

        #region Privates
        private ETrayDirection _direction = ETrayDirection.Normal;
        private int _rows = 5;
        private int _columns = 5;
        public event EventHandler ChangeColumnRowEvent;
        #endregion
    }
}
