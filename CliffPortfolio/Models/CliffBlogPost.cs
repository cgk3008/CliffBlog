using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CliffPortfolio.Models
{
    public class CliffBlogPost
    {
        public int Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; } //<Nullable>  or ?  to set value as null-able
        public string Title { get; set; }
        public string Body { get; set; }
        public string MediaUrl { get; set; }
        public bool Published { get; set; }
        public string Slug { get; set; }  //can type "prop" then tab tab to setup these, a little faster

        public virtual ICollection<BlogComment> Comments { get; set; }  //navigation property, expects a class that defines objects, "list of T" name for collections.

        public CliffBlogPost()   //this is a constructor that creates a class.......
        {
            Comments = new HashSet<BlogComment>();

        }


    }
}