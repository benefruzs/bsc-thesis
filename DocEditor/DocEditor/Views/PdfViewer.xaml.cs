using System;
using System.IO;
using System.Windows;

namespace DocEditor.Views
{
    /// <summary>
    /// Interaction logic for PdfViewer.xaml
    /// </summary>
    public partial class PdfViewer : Window
    {
        public PdfViewer()
        {
            InitializeComponent();

            string curDir = Directory.GetCurrentDirectory();
            int n = curDir.Length - 25;
            string str = curDir.Remove(n, 25); //\bin\Debug\net5.0-windows

            this.pdfBrowser.Source = new Uri(String.Format("file:///{0}\\res\\dicthelp.pdf", str));
        }
    }
}
