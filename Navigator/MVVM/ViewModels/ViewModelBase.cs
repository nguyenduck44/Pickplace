using log4net;
using NavigatorMachine.Defines;
using NavigatorMachine.Models;

namespace NavigatorMachine.MVVM.ViewModels
{
    public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : ViewModelBase;
    public class ViewModelBase : ObservableObject
    {
        public ILog LOG { get; set; }

        public ViewModelBase()
        {
            LogFactory.Configure();
            LOG = log4net.LogManager.GetLogger(this.GetType().Name);
        }
    }
}
