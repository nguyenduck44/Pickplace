using NavigatorMachine.Defines;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace NavigatorMachine.MVVM.Views
{
    /// <summary>
    /// Interaction logic for AutoView.xaml
    /// </summary>
    public partial class AutoView : UserControl
    {
        public AutoView()
        {
            InitializeComponent();

        }
        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView == null) return;

            if (VisualTreeHelper.GetChildrenCount(listView) > 0)
            {
                Decorator border = VisualTreeHelper.GetChild(listView, 0) as Decorator;
                if (border == null) return;

                ScrollViewer scrollViewer = border.Child as ScrollViewer;
                if (scrollViewer == null) return;

                scrollViewer.ScrollToBottom();
            }
        }

    }
}
