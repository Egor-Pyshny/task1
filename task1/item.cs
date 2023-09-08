using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace task1
{
    [Serializable]
    public class item
    {
        [Column("title")]

        public string title { get; set; } = "";

        [Column("link")]
        
        public string link { get; set; } = "";

        [Column("description")]
      
        public string description { get; set; } = "";

        [Column("category")]
        
        public string category { get; set; } = "";

        [Column("pubDate")]
        
        public string pubDate { get; set; } = "";

        public static explicit operator Dictionary<string, string>(item v)
        {
            return new Dictionary<string, string>{
                { "title", v.title },
                { "link", v.link },
                { "description", v.description },
                { "category", v.category },
                { "pubDate", v.pubDate },
            };
        }
    }
}
