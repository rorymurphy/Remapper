using Microsoft.EntityFrameworkCore;
using Remapper.Test.DataAccess;
using Remapper.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Remapper.Test
{
    public class TablePerTypeTest
    {
        [Fact]
        public void TestTablePerType()
        {
            var context = new ContentDbContext();
            var blogRepository = new BlogPostRepository(context);
            var reviewRepository = new ProductReviewRepository(context);
            context.Database.OpenConnection();
            context.Database.Migrate();

            List<BlogPost> blogPosts = new List<BlogPost>();
            List<ProductReview> productReviews = new List<ProductReview>();
            for (int i = 0; i < 100; i++)
            {
                var post = new BlogPost()
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    Author = "Author" + i,
                    Title = "Title" + i,
                    Content = "Content" + i
                };
                blogPosts.Add(post);
                blogRepository.Add(post);

                var review = new ProductReview()
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    Author = "Author" + i,
                    Rating = i,
                    Review = "Review" + i
                };
                productReviews.Add(review);
                reviewRepository.Add(review);
            }

            context.SaveChanges();

            var reviewResult = reviewRepository.Reviews.Where(r => r.Rating >= 90).OrderBy(r => r.Rating).ToList();
            Assert.Equal(10, reviewResult.Count);
            for(int i = 0; i < reviewResult.Count; i ++)
            {
                Assert.Equal(productReviews[90 + i], reviewResult[i]);
            }
        }
        //[Fact]
        //public void TestMapping()
        //{
        //    var data = new List<ContentItem>();
        //    for(int i = 0; i < 100; i++)
        //    {
        //        data.Add(new ContentItem(new Dictionary<string, object>() { { "blog-content", "text" + i } }) { Title = "Blog Post " + i, Id = Guid.NewGuid() });
        //    }

        //    var provider = new TransformQueryProvider<ContentItem, BlogPost>()
        //    {
        //        InnerQueryable = data.AsQueryable(),
        //        Transform = ci => new BlogPost() { Id = ci.Id, Title = ci.Title, Content = (string)ci.Attributes["blog-content"] }
        //    };
        //    provider.AddMapping(b => b.Content, ci => (string)ci.Attributes["blog-content"]);

        //    var transformed = provider.CreateEmptyQuery();
            
        //    //Tests translation of a property and a method call
        //    Assert.Equal("Blog Post 50", transformed.Single(bp => !string.IsNullOrEmpty(bp.Content) && bp.Content.Equals("text50")).Title);
        //    Assert.Equal(100, transformed.Count());
        //}
    }
}
