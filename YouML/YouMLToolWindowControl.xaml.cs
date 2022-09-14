using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace YouML
{
    public partial class YouMLToolWindowControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YouMLToolWindowControl"/> class.
        /// </summary>
        public YouMLToolWindowControl()
        {
            this.InitializeComponent();

            DataContext = new YouMLToolWindowViewModel();
        }

        private void SaveToPng(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)UmlImage.Source));
                encoder.Save(fileStream);
            }
        }

        private void UmlImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Save an Image File",
                RestoreDirectory = true,
                FileName = "YouML_image.png"
            };

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                SaveToPng(saveFileDialog.FileName);
            }
        }
    }
}