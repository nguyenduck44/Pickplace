using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading.Tasks;
using NavigatorMachine.Properties;
using System.Windows.Threading;
using NavigatorMachine.Views;

namespace NavigatorMachine.Controls
{
    public partial class PictureView : UserControl
    {
        private string imageDirectory = @"D:\PowerlogicsBaThien\History\Picture";
        private string[] imageFiles;
        private int currentIndex = -1;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private NewFrameEventHandler captureHandler;
        private DispatcherTimer imageUpdateTimer;
        private DispatcherTimer directoryUpdateTimer;
        private int deleteDays = 30; // Ngày xóa ảnh mặc định
        private VisionWindow visionWindow = null;// Thêm biến để lưu trữ cửa sổ VisionWindow
        private bool isVisionViewOpen = false;

        public PictureView()
        {
            InitializeComponent();
            UpdateRefreshButtonColor(false); // Cài đặt màu sắc ban đầu khi không refresh
            LoadSettings();
            UpdateCurrentDirectoryDisplay();
            LoadImages();

            // Khởi tạo và cấu hình DispatcherTimer cho việc quét ảnh
            imageUpdateTimer = new DispatcherTimer();
            imageUpdateTimer.Interval = TimeSpan.FromMilliseconds(0); // Thời gian quét ảnh là 100ms
            imageUpdateTimer.Tick += ImageUpdateTimer_Tick;
            imageUpdateTimer.Start();

            // Khởi tạo và cấu hình DispatcherTimer cho việc quét thư mục
            directoryUpdateTimer = new DispatcherTimer();
            directoryUpdateTimer.Interval = TimeSpan.FromMilliseconds(0); // Thời gian quét thư mục là 100ms
            directoryUpdateTimer.Tick += DirectoryUpdateTimer_Tick;
            directoryUpdateTimer.Start();
        }

        private void DirectoryUpdateTimer_Tick(object sender, EventArgs e)
        {
            string latestDirectory = GetLatestSubdirectory();
            if (!string.IsNullOrEmpty(latestDirectory) && latestDirectory != imageDirectory)
            {
                imageDirectory = latestDirectory;
                LoadImages();
                UpdateCurrentDirectoryDisplay();
            }
        }

        private string GetLatestSubdirectory()
        {
            string latestDirectory = null;
            DateTime latestTime = DateTime.MinValue;

            try
            {
                string[] subdirectories = Directory.GetDirectories(@"D:\PowerlogicsBaThien\History\Picture");
                foreach (string subdirectory in subdirectories)
                {
                    DateTime creationTime = Directory.GetCreationTime(subdirectory);
                    if (creationTime > latestTime)
                    {
                        latestTime = creationTime;
                        latestDirectory = subdirectory;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi quét các thư mục: " + ex.Message);
            }

            return latestDirectory;
        }

        private void ImageUpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateRefreshButtonColor(imageUpdateTimer.IsEnabled); // Truyền trạng thái của imageUpdateTimer
            LoadImages();
        }

        private void LoadSettings()
        {
            imageDirectory = Settings.Default.ImageDirectory;
            deleteDays = Settings.Default.DeleteDays;
            if (string.IsNullOrEmpty(imageDirectory))
            {
                imageDirectory = @"D:\PowerlogicsBaThien\History\Picture"; // Đường dẫn mặc định
            }

            txtDeleteDays.Text = deleteDays.ToString(); // Hiển thị giá trị xóa ngày trong TextBox
        }

        private void SaveSettings()
        {
            Settings.Default.ImageDirectory = imageDirectory;
            if (int.TryParse(txtDeleteDays.Text, out int days))
            {
                deleteDays = days;
                Settings.Default.DeleteDays = deleteDays; // Lưu giá trị số ngày xóa ảnh vào Settings
                Settings.Default.Save(); // Lưu các thiết lập
            }
            else
            {
                MessageBox.Show("Vui lòng nhập số ngày hợp lệ");
            }
        }

        private void LoadImages()
        {
            if (!Directory.Exists(imageDirectory))
            {
                Dispatcher.Invoke(() =>
                {
                    txtEmptyDirectory.Visibility = Visibility.Visible;
                    imgDisplay.Source = null;
                });
                return;
            }

            imageFiles = Directory.GetFiles(imageDirectory, "*.jpg");

            if (imageFiles.Length == 0)
            {
                Dispatcher.Invoke(() =>
                {
                    txtEmptyDirectory.Visibility = Visibility.Visible;
                    imgDisplay.Source = null;
                });
                return;
            }

            Dispatcher.Invoke(() =>
            {
                txtEmptyDirectory.Visibility = Visibility.Collapsed;
            });

            // Hiển thị ảnh cuối cùng
            currentIndex = imageFiles.Length - 1;
            ShowCurrentImage();
        }

        private void ShowCurrentImage()
        {
            if (currentIndex >= 0 && currentIndex < imageFiles.Length)
            {
                string currentImagePath = imageFiles[currentIndex];

                // Kiểm tra tập tin có bị khóa hay không
                bool isFileLocked = IsFileLocked(currentImagePath);

                if (isFileLocked)
                {
                    // Thực hiện xử lý khi tập tin bị khóa
                    Dispatcher.Invoke(() =>
                    {
                        txtEmptyDirectory.Visibility = Visibility.Visible;
                        imgDisplay.Source = null;
                    });

                    //MessageBox.Show("Tập tin đang bị khóa bởi một quy trình khác. Vui lòng thử lại sau.");
                    return;
                }

                try
                {
                    // Cố gắng đọc tệp hình ảnh
                    byte[] imageData = File.ReadAllBytes(currentImagePath);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = new MemoryStream(imageData);
                    bitmap.EndInit();
                    bitmap.Freeze();

                    // Cập nhật giao diện người dùng trên luồng UI
                    Dispatcher.Invoke(() =>
                    {
                        txtEmptyDirectory.Visibility = Visibility.Collapsed;
                        imgDisplay.Source = bitmap;
                    });
                }
                catch (IOException ex)
                {
                    // Xử lý lỗi khi truy cập tệp (ví dụ: ghi log, bỏ qua tập tin, thông báo lỗi cho người dùng)
                    Dispatcher.Invoke(() =>
                    {
                        txtEmptyDirectory.Visibility = Visibility.Visible;
                        imgDisplay.Source = null;
                    });

                    // Ghi log ngoại lệ hoặc thông báo lỗi cho người dùng
                    MessageBox.Show($"Lỗi khi tải hình ảnh: {ex.Message}");
                }
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    txtEmptyDirectory.Visibility = Visibility.Visible;
                    imgDisplay.Source = null;
                });
            }
        }

        // Phương thức kiểm tra tập tin có bị khóa hay không
        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    // Nếu mở tệp thành công, đó là tệp không bị khóa
                    fs.Close();
                    return false;
                }
            }
            catch (IOException)
            {
                // Xử lý ngoại lệ khi không thể mở tệp do bị khóa
                return true;
            }
        }

        private void ConnectCamera_Click(object sender, RoutedEventArgs e)
        {
            if (videoDevices == null)
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            }

            if (videoDevices.Count == 0)
            {
                MessageBox.Show("Không tìm thấy thiết bị camera.");
                return;
            }

            if (videoSource == null)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                videoSource.Start();
            }

            // Tắt chức năng refresh hình ảnh khi bật camera
            if (imageUpdateTimer.IsEnabled)
            {
                imageUpdateTimer.Stop();
                UpdateRefreshButtonColor(false); // Truyền đúng đối số khi dừng refresh
            }
        }

        private async void DisconnectCamera_Click(object sender, RoutedEventArgs e)
        {
            if (videoSource != null)
            {
                if (videoSource.IsRunning)
                {
                    // Signal to stop the video source
                    videoSource.SignalToStop();

                    // Wait until the video source has completely stopped in a background task
                    await Task.Run(() => videoSource.WaitForStop());
                }

                // Unsubscribe from the NewFrame event
                videoSource.NewFrame -= captureHandler;

                // Release the video source
                videoSource = null;

                // Clear the image display in the UI thread
                Dispatcher.Invoke(() => imgDisplay.Source = null);

                ShowCurrentImage();

                // Tự động bật lại chức năng refresh hình ảnh khi tắt camera
                if (!imageUpdateTimer.IsEnabled)
                {
                    imageUpdateTimer.Start();
                    UpdateRefreshButtonColor(true); // Truyền đúng đối số khi bắt đầu refresh
                }
            }
        }

        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                bitmapimage.Freeze(); // Freeze the BitmapImage to avoid further modifications
                return bitmapimage;
            }
        }

        private void NavigateBack_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                ShowCurrentImage();
            }
        }

        private void NavigateNext_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex < imageFiles.Length - 1)
            {
                currentIndex++;
                ShowCurrentImage();
            }
        }

        private async void CaptureImage_Click(object sender, RoutedEventArgs e)
        {
            if (videoSource == null || !videoSource.IsRunning)
            {
                // Bật camera nếu nó chưa được kết nối
                ConnectCamera_Click(sender, e);

                // Kiểm tra lại nếu vẫn không chạy, hiển thị thông báo và thoát
                if (videoSource == null || !videoSource.IsRunning)
                {
                    MessageBox.Show("Không thể kết nối được camera. Vui lòng thử lại.");
                    return;
                }
            }

            // Chờ một khoảng thời gian để đảm bảo camera đã sẵn sàng
            await Task.Delay(1000); // Thời gian chờ 1000ms (1 giây)

            // Tiếp tục quá trình chụp ảnh
            // Tạo biến lưu frame hiện tại
            Bitmap currentFrame = null;

            // Đăng ký sự kiện NewFrame để chụp frame hiện tại
            NewFrameEventHandler captureHandler = null;
            captureHandler = (s, eventArgs) =>
            {
                currentFrame = (Bitmap)eventArgs.Frame.Clone();

                // Ngắt kết nối sự kiện NewFrame để không chụp thêm frame nữa
                videoSource.NewFrame -= captureHandler;

                // Chờ một khoảng thời gian để đảm bảo frame được chụp đầy đủ
                Task.Delay(200).ContinueWith(t =>
                {
                    // Tạo đường dẫn và lưu ảnh
                    string filePath = Path.Combine(imageDirectory, $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.jpg");
                    currentFrame.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                    // Cập nhật danh sách ảnh và hiển thị ảnh mới nhất
                    imageFiles = Directory.GetFiles(imageDirectory, "*.jpg");
                    Array.Sort(imageFiles);
                    currentIndex = imageFiles.Length - 1;

                    // Sử dụng Dispatcher để cập nhật giao diện người dùng
                    Dispatcher.Invoke(() =>
                    {
                        ShowCurrentImage();
                        DisconnectCamera_Click(sender, e);
                    });
                });
            };

            // Đăng ký sự kiện NewFrame để bắt frame và chụp ảnh
            videoSource.NewFrame += captureHandler;
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Get the new frame from the camera
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

            // Convert the bitmap to a BitmapImage
            BitmapImage bitmapimage = BitmapToBitmapImage(bitmap);

            // Update the Image control in the UI thread
            Dispatcher.Invoke(() =>
            {
                imgDisplay.Source = bitmapimage;
            });

            // Dispose the bitmap to free up memory
            bitmap.Dispose();
        }

        private void ChangeDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Chọn thư mục chứa ảnh";

            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                imageDirectory = dialog.FileName;
                LoadImages(); // Reload images from the new directory
            }
        }

        private void UpdateCurrentDirectoryDisplay()
        {
            txtCurrentDirectory.Text = $"Directory: {imageDirectory}";
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Code to save settings, such as imageDirectory
            MessageBox.Show("Đã lưu cài đặt");
        }

        private void DeleteImage_Click(object sender, RoutedEventArgs e)
        {
            int deleteDays;
            if (int.TryParse(txtDeleteDays.Text, out deleteDays))
            {
                DateTime deleteThreshold = DateTime.Now.AddDays(-deleteDays);
                foreach (string file in imageFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < deleteThreshold)
                    {
                        File.Delete(file);
                    }
                }
                MessageBox.Show($"Đã xóa các ảnh cũ hơn {deleteDays} ngày");
                LoadImages(); // Cập nhật lại danh sách ảnh sau khi xóa

                // Lưu cài đặt sau khi thực hiện xóa ảnh
                SaveSettings();
            }
            else
            {
                MessageBox.Show("Vui lòng nhập số ngày hợp lệ");
            }
        }

        private void btnPictureRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (imageUpdateTimer.IsEnabled)
            {
                imageUpdateTimer.Stop();
                UpdateRefreshButtonColor(false); // Truyền đúng đối số khi dừng refresh
            }
            else
            {
                imageUpdateTimer.Start();
                UpdateRefreshButtonColor(true); // Truyền đúng đối số khi bắt đầu refresh
            }
        }

        private void UpdateRefreshButtonColor(bool isRefreshing)
        {
            if (isRefreshing)
            {
                btnPictureRefresh.Background = System.Windows.Media.Brushes.Green;
                btnPictureRefresh.Foreground = System.Windows.Media.Brushes.Black;
                btnPictureRefresh.Content = "Stop Refresh";
            }
            else
            {
                btnPictureRefresh.Background = System.Windows.Media.Brushes.Green;
                btnPictureRefresh.Foreground = System.Windows.Media.Brushes.White;
                btnPictureRefresh.Content = "Refresh";
            }
        }

        private void btnVisionTeaching_Click(object sender, RoutedEventArgs e)
        {
            if (!isVisionViewOpen)
            {
                visionWindow = new VisionWindow();
                visionWindow.Owner = Window.GetWindow(this); // Đặt owner là window hiện tại
                visionWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen; // Hiển thị giữa màn hình
                visionWindow.Closed += VisionViewInstance_Closed; // Đăng ký sự kiện khi đóng VisionView
                visionWindow.Show();
                isVisionViewOpen = true;
            }
            else
            {
                visionWindow.Activate(); // Đưa VisionView hiện tại ra phía trước
            }
        }

        private void VisionViewInstance_Closed(object sender, EventArgs e)
        {
            isVisionViewOpen = false; // Đánh dấu rằng VisionView đã được đóng
            visionWindow = null; // Đặt thể hiện của VisionView về null
        }
    }
}
