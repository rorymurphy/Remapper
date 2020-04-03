using Remapper.Test.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Remapper.Test.DataAccess
{
    public class ContentItemDTO
    {
        public ContentItemDTO(BlogPost blog) : this((ContentItem)blog)
        {
            this.BlogPost = new BlogPostDTO(blog);
        }

        public ContentItemDTO(ProductReview review) : this((ContentItem)review)
        {
            this.ProductReview = new ProductReviewDTO(review);
        }

        public ContentItemDTO(ContentItem item)
        {
            this.Id = item.Id;
            this.Timestamp = item.Timestamp;
            this.Author = item.Author;
        }

        public ContentItemDTO()
        {

        }

        [Key]
        public Guid Id { get; set; }

        [MaxLength(200)]
        public DateTime Timestamp { get; set; }

        [InverseProperty("ContentItem")]
        public BlogPostDTO BlogPost { get; set; }

        [MaxLength(200)]
        public string Author { get; set; }

        [InverseProperty("ContentItem")]
        public ProductReviewDTO ProductReview { get; set; }
    }
}
