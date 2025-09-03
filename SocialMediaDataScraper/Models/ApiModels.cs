#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaDataScraper.Models
{
    public class GetPagesModel
    {
        public bool Status { get; set; }
        public List<Page> Data { get; set; }
    }

    public class Page
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
