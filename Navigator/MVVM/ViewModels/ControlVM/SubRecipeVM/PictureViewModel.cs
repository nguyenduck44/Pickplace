using NavigatorMachine.Commands;
using NavigatorMachine.Core;
using NavigatorMachine.Defines;
using NavigatorMachine.Models;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NavigatorMachine.MVVM.ViewModels
{
    public class PictureViewModel : ViewModelBase
    {
        public ICommand LiveOnCommand { get; }
        public PictureViewModel()
        {
        }
    }
}
