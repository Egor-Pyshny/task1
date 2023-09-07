using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task1
{
    public class item
    {
        [Column("title")]
        public string title { get; set; }
        [Column("link")]
        public string link { get; set; }
        [Column("description")]
        public string description { get; set; }
        [Column("category")]
        public string category { get; set; }
        [Column("pubDate")]
        public string pubDate { get; set; }


    }
}
