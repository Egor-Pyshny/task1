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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace task1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string read_file = "";
        private string write_file = "";
        private struct _conf {
            public string reading_mode;
            public string writing_mode;

            public _conf(object obj){
                reading_mode = "";
                writing_mode = "";
            }
        };
        private _conf conf = new _conf(null);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file_dialog = new OpenFileDialog();
            file_dialog.ShowDialog();
            read_file = file_dialog.FileName;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file_dialog = new OpenFileDialog();
            file_dialog.ShowDialog();
            write_file = file_dialog.FileName;
        }

        private void Select_read_mode(object sender, RoutedEventArgs e)
        {
            conf.reading_mode = (sender as RadioButton).Content.ToString();
        }

        private void Select_write_mode(object sender, RoutedEventArgs e)
        {
            conf.writing_mode = (sender as RadioButton).Content.ToString();
        }

        delegate IEnumerable<dynamic> method(string text);
        private void Start(object sender, RoutedEventArgs e)
        {
            Dictionary<string, method> readers = new Dictionary<string, method>()
            {

            };
            if (read_file != "" && write_file != "" && conf.reading_mode != "" && conf.reading_mode != "")
            {
                using (StreamReader reader = new StreamReader(read_file)) {
                    string context = reader.ReadToEnd();
                    
                }
            }
        }
    }
}
