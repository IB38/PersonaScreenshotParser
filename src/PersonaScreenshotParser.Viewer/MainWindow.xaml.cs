using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> ScreenshotFilePaths { get; set; }

        public MainWindow()
        {
            DataContext = this;
            ScreenshotFilePaths = new ObservableCollection<string>();

            var path = @"C:\Temp\ocr-test";
            foreach (var fp in Directory.GetFiles(path))
            {
                ScreenshotFilePaths.Add(fp);
            }

            InitializeComponent();
            
        }
    }
}