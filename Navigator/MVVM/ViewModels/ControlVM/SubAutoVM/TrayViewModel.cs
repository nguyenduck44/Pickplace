using NavigatorMachine.Commands;
using NavigatorMachine.Defines;
using NavigatorMachine.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace NavigatorMachine.MVVM.ViewModels
{
    public class TrayViewModel : ObservableObject
    {
		public ObservableCollection<CCell> CurrentTray { get;set; }
        public ObservableCollection<CCell> LastTray { get;set; }
        public CTrayRecipe _trayRecipe => CDef.TrayRecipe;

        public bool IsTrayInspectDone
        {
            get
            {
                bool result = CurrentTray.Count(c => c.Status > EStatus.Processing) == CurrentTray.Count;

                return result;
            }
        }

        public TrayViewModel()
        {
        }

        #region Methods
        public void InitCurrentTray()
        {
            CurrentTray = new ObservableCollection<CCell>();
            InitTray(CurrentTray);
        }

        public void InitLastTray()
        {
            LastTray = new ObservableCollection<CCell>();
            InitTray(LastTray);
        }

        public void InitTray(ObservableCollection<CCell> _tray)
        {
            for (int i = 0; i < _trayRecipe.Rows; i++)
            {
                if (CDef.TrayRecipe.Direction == ETrayDirection.Normal)
                {
                    for (int j = 0; j < _trayRecipe.Columns; j++)
                    {
                        _tray.Add(new CCell()
                        {
                            Index = i * _trayRecipe.Columns + j + 1,
                            Status = EStatus.None
                        });
                    }
                }
                else
                {
                    for (int j = 0; j < _trayRecipe.Columns; j++)
                    {
                        if (i == 0 || i % 2 == 0)
                        {
                            _tray.Add(new CCell()
                            {
                                Index = i * _trayRecipe.Columns + j + 1,
                                Status = EStatus.None
                            });
                        }
                        else
                        {
                            _tray.Add(new CCell()
                            {
                                Index = (i + 1) * _trayRecipe.Columns - j,
                                Status = EStatus.None
                            });
                        }
                    }
                }
            }
        }

        public void BackUpStatusToLastTray()
        {
            foreach (CCell cell in CurrentTray)
            {
                LastTray[CurrentTray.IndexOf(cell)].Status = cell.Status;
                cell.Status = EStatus.None;
            }
        }
        #endregion

        #region Commands
        public RelayCommand SingleCellButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (o is CCell)
                    {
                        //CCell cell = (CCell)o;

                        //CurrentTray.ToList().ForEach(c => c.Status = EStatus.None);

                        //for(int i = 0;i < cell.Index;i++)
                        //{
                        //    CurrentTray[i].Status = EStatus.OK;
                        //}
                    }
                });
            }
        }

        public RelayCommand ResetAllButtonCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CurrentTray.ToList().ForEach(c => c.Status = EStatus.None);
                });
            }
        }
        #endregion

    }
}
