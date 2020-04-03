using System;
using System.Collections.Generic;
using System.Text;

namespace Remapper.Test.Models
{
    public class BlogPost : ContentItem
    {
        public string Title { get; set; }

        public string Content {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            return obj is BlogPost post &&
                   base.Equals(obj) &&
                   Title == post.Title &&
                   Content == post.Content;
        }
    }
}
