using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private string read_file = "C:\\Users\\user\\Downloads\\data.xml";
        private string write_file = "";
        delegate IEnumerable<dynamic> method(string text);
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

        private IEnumerable<dynamic> read_regexp(string text) { 
            List<Dictionary<string, string>> dict = new List<Dictionary<string, string>>();
            string pattern = @"<[^>]*>.*<";
            MatchCollection matches = Regex.Matches(text, pattern);  
            List<Match> list = new List<Match>();
            Dictionary<int,string> tags = new Dictionary<int, string>(), temp = new Dictionary<int, string>();
            int pos=0;
            //проверить порядок занесения тегов в словарь
            foreach (Match match in matches) {
                string tag = match.Value.Replace('<', ' ').Split('>')[0].Trim();
                list.Add(match);
                if (temp.Values.Contains(tag))
                {
                    if (temp.Count > tags.Count)
                    {
                        tags = temp;
                        temp.Clear();
                    }
                    pos = 0;
                }
                else
                {
                    temp.Add(pos,tag);
                    pos++;
                }
            }
            int j = 0;
            int objects_amount = (int)Math.Ceiling((double)(matches.Count / tags.Count));
            for (int i = 0; i < objects_amount; i++) {
                Dictionary<string, string> tmp = new Dictionary<string, string>();
                for(int l=0; l<tags.Count;l++) {
                    if (j < matches.Count && matches[j].Value.Contains(tags[l])) {
                        tmp.Add(tags[l], matches[j].Value.Replace('<', ' ').Replace('>', ' ').Replace(tags[l], "").Trim());
                    }
                    j++;
                }
                dict.Add(tmp);
            }
            return dict;
        }


        private void Start(object sender, RoutedEventArgs e)
        {
            Dictionary<string, method> readers = new Dictionary<string, method>()
            {
                { "Regex", read_regexp },
            };
           /* if (read_file != "" && write_file != "" && conf.reading_mode != "" && conf.reading_mode != "")
            {*/
                using (StreamReader reader = new StreamReader(read_file)) {
                    string context = reader.ReadToEnd();
                    var a = readers[conf.reading_mode](context);
                }
            //}
        }
    }
}
