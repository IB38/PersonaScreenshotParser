using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PersonaScreenshotParser.Viewer
{
    /// <summary>
    /// Interaction logic for ScreenshotItem.xaml
    /// </summary>
    public partial class ScreenshotItem : UserControl
    {
        public static readonly DependencyProperty FilePathProperty = 
            DependencyProperty.Register(
                nameof(FilePath), 
                typeof(string), 
                typeof(ScreenshotItem),
                new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnFilePathChanged)) { AffectsRender = true });

        private static void OnFilePathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ScreenshotItem myClass = (ScreenshotItem)sender;
            myClass.FilePath = (string)args.NewValue;
        }

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set 
            {
                FileNameLabel.Content = value ?? "[NULL]";
                UpdateImage(value);
                SetValue(FilePathProperty, value);
            }
        }

        public ScreenshotItem()
        {
            InitializeComponent();
        }

        public void ChangeSelectState(bool selected)
        {
            if (selected)
                FileNameLabel.FontWeight = FontWeights.Bold;
            else
                FileNameLabel.FontWeight = FontWeights.Normal;
        }

        private void UpdateImage(string? filePath)
        {
            // Create source
            BitmapImage img = new BitmapImage();

            if (filePath != null)
            {
                // BitmapImage.UriSource must be in a BeginInit/EndInit block
                img.BeginInit();
                img.UriSource = new Uri(filePath);

                // To save significant application memory, set the DecodePixelWidth or
                // DecodePixelHeight of the BitmapImage value of the image source to the desired
                // height or width of the rendered image. If you don't do this, the application will
                // cache the image as though it were rendered as its normal size rather than just
                // the size that is displayed.
                // Note: In order to preserve aspect ratio, set DecodePixelWidth
                // or DecodePixelHeight but not both.
                img.DecodePixelWidth = 200;
                img.EndInit();
            }
            //set image source

            ScreenshotImage.Source = img;
        }
    }
}
