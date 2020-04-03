using System;
using System.Collections.Generic;
using System.Text;

namespace Remapper.Test.Models
{
    public class ProductReview : ContentItem
    {
        public int Rating { get; set; }
        public String Review { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ProductReview review &&
                   base.Equals(obj) &&
                   Rating == review.Rating &&
                   Review == review.Review;
        }
    }
}
