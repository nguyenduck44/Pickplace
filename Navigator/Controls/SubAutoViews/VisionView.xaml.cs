using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Drawing = System.Drawing;
using WpfRectangle = System.Windows.Shapes.Rectangle;
using System.Drawing.Imaging; // Để sử dụng ImageLockMode

namespace NavigatorMachine.Views
{
    public partial class VisionView : UserControl
    {
        private bool isGrayscale = false;
        private Mat originalImage;
        private Mat processedImage;
        private bool isCreatingROI = false;
        private OpenCvSharp.Point roiStartPoint;
        private WpfRectangle currentROI;
        private List<WpfRectangle> roiList = new List<WpfRectangle>();

        public VisionView()
        {
            InitializeComponent();
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = @"D:\PowerlogicsBaThien\History\Picture",
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };

            if (dialog.ShowDialog() == true)
            {
                originalImage = new Mat(dialog.FileName, ImreadModes.Color);
                processedImage = ProcessImage(originalImage);
                imgDisplay.Source = BitmapToImageSource(processedImage);
            }
        }

        private Mat ProcessImage(Mat src)
        {
            Mat gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            return gray;
        }

        private void BtnGrayscale_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage == null || processedImage == null)
                return;

            isGrayscale = !isGrayscale;
            imgDisplay.Source = isGrayscale ? BitmapToImageSource(processedImage) : BitmapToImageSource(originalImage);
            UpdateGrayscaleButtonContent();
        }

        private BitmapSource BitmapToImageSource(Mat bitmap)
        {
            using (var stream = bitmap.ToMemoryStream())
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        private void UpdateGrayscaleButtonContent()
        {
            btnGrayscale.Content = isGrayscale ? "Hiện ảnh màu" : "Chuyển sang ảnh xám";
        }

        private void btnROI_Click(object sender, RoutedEventArgs e)
        {
            isCreatingROI = !isCreatingROI;
            btnROI.Content = isCreatingROI ? "Hủy tạo ROI" : "Tạo ROI";
        }

        private void imageCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isCreatingROI)
            {
                roiStartPoint = new OpenCvSharp.Point(e.GetPosition(imageCanvas).X, e.GetPosition(imageCanvas).Y);
                currentROI = new WpfRectangle
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    Width = 0,
                    Height = 0
                };
                Canvas.SetLeft(currentROI, roiStartPoint.X);
                Canvas.SetTop(currentROI, roiStartPoint.Y);
                imageCanvas.Children.Add(currentROI);
            }
        }

        private void imageCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isCreatingROI && e.LeftButton == MouseButtonState.Pressed)
            {
                double width = e.GetPosition(imageCanvas).X - roiStartPoint.X;
                double height = e.GetPosition(imageCanvas).Y - roiStartPoint.Y;
                currentROI.Width = Math.Abs(width);
                currentROI.Height = Math.Abs(height);
                Canvas.SetLeft(currentROI, Math.Min(roiStartPoint.X, e.GetPosition(imageCanvas).X));
                Canvas.SetTop(currentROI, Math.Min(roiStartPoint.Y, e.GetPosition(imageCanvas).Y));
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (currentROI != null)
            {
                roiList.Add(currentROI);
                currentROI = null;
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (roiList.Count > 0)
            {
                var lastROI = roiList[roiList.Count - 1];
                imageCanvas.Children.Remove(lastROI);
                roiList.RemoveAt(roiList.Count - 1);
            }
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            // 1. Kiểm tra xem đã có ảnh được tải lên hay chưa
            if (originalImage == null)
            {
                MessageBox.Show("Vui lòng tải ảnh trước khi kiểm tra.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 2. Nếu có ảnh, thực hiện phát hiện các đường viền (contour detection) trong các ROI
            Mat grayImage = new Mat();
            Cv2.CvtColor(originalImage, grayImage, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(grayImage, grayImage, 127, 255, ThresholdTypes.Binary);

            Mat contourImage = originalImage.Clone(); // Bản sao của ảnh gốc để vẽ đường viền

            foreach (var roi in roiList)
            {
                // Lấy tọa độ của ROI trên canvas
                double left = Canvas.GetLeft(roi);
                double top = Canvas.GetTop(roi);
                double width = roi.Width;
                double height = roi.Height;

                // Tính toán tỉ lệ scale giữa kích thước của ảnh gốc và kích thước của imgDisplay
                double scaleX = imgDisplay.ActualWidth / originalImage.Width;
                double scaleY = imgDisplay.ActualHeight / originalImage.Height;

                // Tạo ROI trên ảnh xám để chỉ phát hiện đường viền trong vùng này
                // Chuyển tọa độ của ROI từ pixel trên canvas sang pixel trên ảnh gốc
                int roiLeft = (int)(left / scaleX);
                int roiTop = (int)(top / scaleY);
                int roiWidth = (int)(width / scaleX);
                int roiHeight = (int)(height / scaleY);

                // Sử dụng Rect từ OpenCvSharp
                OpenCvSharp.Rect roiRect = new OpenCvSharp.Rect(roiLeft, roiTop, roiWidth, roiHeight);
                Mat roiGray = new Mat(grayImage, roiRect);

                // Tạo một bản sao của ROI trên ảnh gốc để vẽ đường viền
                Mat roiContourImage = new Mat(contourImage, roiRect);

                // Phát hiện các đường viền trong ROI và vẽ lên ảnh gốc
                OpenCvSharp.Point[][] contours;
                HierarchyIndex[] hierarchyIndexes;
                Cv2.FindContours(roiGray, out contours, out hierarchyIndexes, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                Cv2.DrawContours(roiContourImage, contours, -1, Scalar.Red, 2);
            }

            // Hiển thị ảnh có các đường viền được vẽ lên Image
            imgDisplay.Source = contourImage.ToBitmapSource();
        }

    }
    public static class OpenCvSharpExtensions
    {
        public static BitmapSource ToBitmapSource(this Mat mat)
        {
            if (mat == null)
                throw new ArgumentNullException(nameof(mat));

            // Convert OpenCvSharp Mat to System.Drawing.Bitmap
            using (var bitmap = mat.ToBitmap())
            {
                // Convert System.Drawing.Bitmap to BitmapSource
                var bitmapSource = bitmap.ToBitmapSource();
                bitmap.Dispose(); // Dispose System.Drawing.Bitmap
                return bitmapSource;
            }
        }

        public static BitmapSource ToBitmapSource(this Drawing.Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            var bitmapData = bitmap.LockBits(
                new Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, Drawing.Imaging.PixelFormat.Format32bppPArgb);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }

        public static Drawing.Bitmap ToBitmap(this Mat mat)
        {
            if (mat == null)
                throw new ArgumentNullException(nameof(mat));

            // Convert Mat to Bitmap
            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat);
        }
    }
}

