using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace DocEditor.Views
{
    /// <summary>
    /// Interaction logic for UserHelpViewer.xaml
    /// </summary>
    public partial class UserHelpViewer : Window
    {
        public UserHelpViewer()
        {
            InitializeComponent();

            string curDir = Directory.GetCurrentDirectory();
            int n = curDir.Length - 25;
            string str = curDir.Remove(n, 25); //\bin\Debug\net5.0-windows

            this.userHelpBrowser.Source = new Uri(String.Format("file:///{0}\\res\\user-help.html", str));
        }
    }
}
