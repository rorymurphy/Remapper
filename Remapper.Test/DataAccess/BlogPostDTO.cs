using Remapper.Test.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Remapper.Test.DataAccess
{
    public class BlogPostDTO
    {
        public BlogPostDTO(BlogPost blog)
        {
            this.Id = blog.Id;
            this.Content = blog.Content;
            this.Title = blog.Title;
        }

        public BlogPostDTO()
        {

        }

        public Guid Id { get; set; }

        [ForeignKey("Id")]
        public ContentItemDTO ContentItem { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(65000)]
        public string Content { get; set; }
    }
}
