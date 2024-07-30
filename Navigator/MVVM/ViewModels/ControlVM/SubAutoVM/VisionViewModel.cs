using System.Windows.Input;
namespace NavigatorMachine.MVVM.ViewModels
{
    public class VisionViewModel : ViewModelBase
    {
        public ICommand LiveOnCommand { get; }

        public VisionViewModel()
        {
            //LiveOnCommand = new RelayCommand(LiveOnExecute);
        }

        //private void LiveOnExecute()
        //{
        //    // Thực hiện các hành động liên quan đến PictureView
        //    // Ví dụ: Hiển thị ảnh, thay đổi thuộc tính, ...
        //    // Sử dụng PictureViewModel để truy cập đến Image trong VisionView
        //}
    }
}
