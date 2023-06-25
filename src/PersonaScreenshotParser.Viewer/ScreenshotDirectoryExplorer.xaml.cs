using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonaScreenshotParser.Viewer
{
    /// <summary>
    /// Interaction logic for ScreenshotDirectoryExplorer.xaml
    /// </summary>
    public partial class ScreenshotDirectoryExplorer : UserControl
    {
        public string DirectoryPath
        {
            get { return (string)GetValue(DirectoryPathProperty); }
            set
            {
                UpdateFilePaths(value);
                SetValue(DirectoryPathProperty, value);
            }
        }

        public ObservableCollection<string> ScreenshotFilePaths { get; }

        public ScreenshotItem? SelectedItem
        {
            get { return (ScreenshotItem?)GetValue(SelectedItemProperty); }
            set
            {
                UpdateSelectedItem(value);
                SetValue(SelectedItemProperty, value);
            }
        }

        public ScreenshotDirectoryExplorer()
        {
            DataContext = this;
            ScreenshotFilePaths = new ObservableCollection<string>();

            InitializeComponent();
        }

        private void UpdateFilePaths(string dirPath)
        {
            ScreenshotFilePaths.Clear();

            if (dirPath == null || !Directory.Exists(dirPath))
                return;

            foreach (var fp in Directory.EnumerateFiles(dirPath))
                ScreenshotFilePaths.Add(fp);
        }

        private void UpdateSelectedItem(ScreenshotItem? item)
        {
            var oldSelectedItem = SelectedItem;
            if (oldSelectedItem?.GetHashCode() != item?.GetHashCode())
            {
                oldSelectedItem?.ChangeSelectState(false);
                item?.ChangeSelectState(true);
            }
        }

        private void ScreenshotItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ScreenshotItem ?? throw new InvalidOperationException($"Unexpected sender type '{sender.GetType().FullName}', expected '{typeof(ScreenshotItem).FullName}'");
            SelectedItem = item;
        }

        #region DependencyProps

        public static readonly DependencyProperty DirectoryPathProperty =
        DependencyProperty.Register(
            nameof(DirectoryPath),
            typeof(string),
            typeof(ScreenshotDirectoryExplorer),
            new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnDirectoryPathChanged)) { AffectsRender = true });

        private static void OnDirectoryPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ScreenshotDirectoryExplorer myClass = (ScreenshotDirectoryExplorer)sender;
            myClass.DirectoryPath = (string)args.NewValue;
        }

        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register(
            nameof(SelectedItem),
            typeof(ScreenshotItem),
            typeof(ScreenshotDirectoryExplorer),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)) { AffectsRender = true });

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ScreenshotDirectoryExplorer myClass = (ScreenshotDirectoryExplorer)sender;
            myClass.SelectedItem = (ScreenshotItem)args.NewValue;

            #endregion DependencyProps
        }
    }
}
