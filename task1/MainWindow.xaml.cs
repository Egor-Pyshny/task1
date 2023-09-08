using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;
using OfficeOpenXml;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;
using System.Xml.Serialization;

namespace task1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private string read_file = "C:\\Users\\Пользователь\\Downloads\\data.xml";
        private string write_file_json = Environment.CurrentDirectory + "\\json.txt";
        private string write_file_excel = Environment.CurrentDirectory + "\\excel.xlsx";
        private string write_file_word = Environment.CurrentDirectory + "\\word.docx";
        delegate IEnumerable<Dictionary<string, string>> method(string text);
        delegate void method1(IEnumerable<Dictionary<string, string>> items);
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

        private void Select_read_mode(object sender, RoutedEventArgs e)
        {
            conf.reading_mode = (sender as RadioButton).Content.ToString();
        }

        private void Select_write_mode(object sender, RoutedEventArgs e)
        {
            conf.writing_mode = (sender as RadioButton).Content.ToString();
        }

        private IEnumerable<Dictionary<string, string>> read_regexp(string text) {
            List<Dictionary<string, string>> dict = new List<Dictionary<string, string>>();
            Dictionary<int,string> tags = new Dictionary<int, string>(), temp = new Dictionary<int, string>();
            int depth = 0, max_depth=0;
            for(int i=0;i<text.Length; i++) {
                if (i < text.Length - 1 && text[i] == '<' && text[i + 1] == '/') depth--;
                else if (text[i] == '<') {
                    depth++;
                    if (depth > max_depth) max_depth = depth;
                }                
            }
            max_depth--;
            int k = 0;
            while (max_depth > 0) {
                if (text[k] == '<') max_depth--;
                k++;
            }
            StringBuilder stringBuilder = new StringBuilder();
            while (text[k] != '>') stringBuilder.Append(text[k++]);
            string class_tag = stringBuilder.ToString();
            text = text.Replace("\n", String.Empty).Replace("\r", String.Empty).Replace("\t", String.Empty);
            string pattern = $@"<{class_tag}>(.*?)<\/{class_tag}>";
            MatchCollection matches = Regex.Matches(text, pattern);
            List<MatchCollection> matchCollections = new List<MatchCollection>();
            pattern = @"<[^>]*>[^<]*";
            int max_tags=0;
            Dictionary<string, string> standart = null;
            foreach (Match match in matches) {
                MatchCollection collection = Regex.Matches(match.Value, pattern);
                Dictionary<string, string> item = new Dictionary<string, string>();
                foreach (Match match1 in collection)
                {
                    string[] arr = match1.Value.Replace('<', ' ').Split('>');
                    string tag = arr[0].Trim();
                    string value = arr[1].Trim();
                    if (value != "") item.Add(tag, value);
                }
                int tags_amount = dict.Count;
                if (tags_amount > max_tags)
                {
                    max_tags = tags_amount;
                    standart = item;
                }
                if(item.ContainsKey("category") && item["category"].Contains("Политика")) dict.Add(item);
            }
            for(int i=0;i<dict.Count;i++) {
                var element = dict[i];
                foreach (KeyValuePair<string, string> pair in standart) {
                    if (!element.ContainsKey(pair.Key)) {
                        element.Add(pair.Key, "");
                    }
                }
            }           
            return dict;
        }

        private IEnumerable<Dictionary<string, string>> read_model(string text)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            XmlSerializer serializer = new XmlSerializer(typeof(channel));
            using (StringReader reader = new StringReader(text))
            { 
                channel root = (channel)serializer.Deserialize(reader);
                foreach (item item1 in root.items){
                    var dict = (Dictionary<string, string>)item1;
                    if(dict.ContainsKey("category") && dict["category"].Contains("Политика")) list.Add(dict);
                }
            }
            return list;
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            Dictionary<string, method> readers = new Dictionary<string, method>()
            {
                { "Regex", read_regexp },
                { "Model", read_model },
            };
            Dictionary<string, method1> writers = new Dictionary<string, method1>()
            {
                { "excel", write_excel },
                { "word", write_word },
                { "json", write_json },
            };
            //if (read_file == "" || conf.reading_mode == "" || conf.writing_mode == "") return;
            FileInfo info = new FileInfo(read_file);
            //if (info.Extension != ".xml") return;
            using (StreamReader reader = new StreamReader(read_file)) {
                string text = reader.ReadToEnd();
                var items = readers[conf.reading_mode](text).ToList();
                var sorted_items = items.OrderBy(temp =>
                {
                    DateTime date;
                    if (DateTime.TryParseExact(temp["pubDate"], "ddd, dd MMM yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                    {
                        return date;
                    }
                    return DateTime.MinValue;
                }).ToList();
                writers[conf.writing_mode](sorted_items);
            }
        }

        private void write_json(IEnumerable<dynamic> items)
        {
            string res = JsonConvert.SerializeObject(items);
            if (!File.Exists(write_file_json)) File.Create(write_file_json);
            File.WriteAllText(write_file_json, String.Empty);
            using (StreamWriter writer = new StreamWriter(write_file_json))
            {
                writer.Write(res);
            }
        }

        private void write_word(IEnumerable<dynamic> items)
        {
            if (!File.Exists(write_file_word)) File.Create(write_file_word);
            Application wordApp = new Application();
            Document doc = wordApp.Documents.Open(write_file_word);
            doc.Content.Delete();
            Dictionary<string, string> temp = null;
            if (items.Count() > 0) temp = (Dictionary<string, string>)items.ElementAt(0); else return;
            int row = 1, col, row_amount= items.Count()+1, col_amount = temp.Count;
            Table table = doc.Tables.Add(doc.Range(), row_amount, col_amount);
            var brd = table.Borders[WdBorderType.wdBorderLeft];
            table.Borders.Enable = 1;
            bool write_headers = true;
            for (int i = 0; i < items.Count(); i++)
            {
                col = 1;
                Dictionary<string, string> item = (Dictionary<string, string>)items.ElementAt(i);
                if (write_headers)
                {
                    int l = 1;
                    var headers = item.Keys;
                    foreach (string head in headers)
                    {
                        Cell cell = table.Cell(row, l);
                        cell.Range.Text = head;
                        l++;
                    }
                    row++;
                    write_headers = false;
                }
                foreach (string value in item.Values)
                {
                    Cell cell = table.Cell(row, col);
                    cell.Range.Text = value;
                    col++;
                }
                row++;
            }
            doc.Save();
            wordApp.Quit();
        }

        private void write_excel(IEnumerable<dynamic> items)
        {
            if (!File.Exists(write_file_excel)) File.Create(write_file_excel);
            using (ExcelPackage package = new ExcelPackage(write_file_excel))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Лист1"]; ;
                worksheet.Cells.Clear();
                int row = 1, col;
                bool write_headers = true;
                for (int i = 0; i < items.Count(); i++) {
                    col = 1;
                    Dictionary<string, string> item = (Dictionary<string, string>)items.ElementAt(i);
                    if (write_headers)
                    {
                        int l = 1;
                        var headers = item.Keys;
                        foreach (string head in headers) {
                            worksheet.Cells[row, l].Value = head;
                            l++;
                        }
                        row++;
                        write_headers = false;
                    }
                    foreach (string value in item.Values)
                    {
                        worksheet.Cells[row, col].Value = value;
                        col++;
                    }
                    row++;
                }
                package.Save();
            }
        }
    }
}
