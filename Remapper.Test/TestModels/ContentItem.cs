using System;
using System.Collections.Generic;
using System.Text;

namespace Remapper.Test.TestModels
{
    public class ContentItem
    {
        public ContentItem() { }

        public ContentItem(IDictionary<string, object> attributes)
        {
            this.Attributes = new Dictionary<string, object>(attributes);
        }
        public Guid Id { get; set; }

        public String Title { get; set; }

        protected internal IDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
    }
}
