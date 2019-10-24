using Remapper.Test.TestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Remapper.Test
{
    public class TransformationTest
    {
        [Fact]
        public void TestMapping()
        {
            var data = new List<ContentItem>();
            for(int i = 0; i < 100; i++)
            {
                data.Add(new ContentItem(new Dictionary<string, object>() { { "blog-content", "text" + i } }) { Title = "Blog Post " + i, Id = Guid.NewGuid() });
            }

            var provider = new TransformQueryProvider<ContentItem, BlogPost>(
                ci => new BlogPost() { Id = ci.Id, Title = ci.Title, Content = (string)ci.Attributes["blog-content"] },
                data.AsQueryable()
            );

            provider.AddMapping(b => b.Content, ci => (string)ci.Attributes["blog-content"]);

            var transformed = provider.CreateEmptyQuery();
            
            //Tests translation of a property and a method call
            Assert.Equal("Blog Post 50", transformed.Single(bp => !string.IsNullOrEmpty(bp.Content) && bp.Content.Equals("text50")).Title);
            Assert.Equal(100, transformed.Count());
        }
    }
}
