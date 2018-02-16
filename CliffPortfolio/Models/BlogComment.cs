using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CliffPortfolio.Models
{
    public class BlogComment
    {

        public int Id { get; set; }
        public int PostId { get; set; }
        public string AuthorId { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Updated { get; set; }
        public string UpdateReason { get; set; }

//The next two lines receive information from the user and the blog post being commented on.
        public virtual ApplicationUser Author { get; set; }  
        public virtual CliffBlogPost Post { get; set; }

    }
}