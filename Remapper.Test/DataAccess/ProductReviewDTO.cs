using Remapper.Test.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Remapper.Test.DataAccess
{
    public class ProductReviewDTO
    {
        public ProductReviewDTO(ProductReview review)
        {
            this.Id = review.Id;
            this.Rating = review.Rating;
            this.Review = review.Review;
        }

        public ProductReviewDTO() { }
        public Guid Id { get; set; }

        [ForeignKey("Id")]
        public ContentItemDTO ContentItem { get; set; }

        public int Rating { get; set; }

        [MaxLength(2000)]
        public string Review { get; set; }

    }
}
