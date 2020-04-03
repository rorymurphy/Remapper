using System;
using System.Collections.Generic;
using System.Text;

namespace Remapper.Test.Models
{
    public abstract class ContentItem
    {
        public ContentItem() { }

        public Guid Id { get; set; }

        public string Author { get; set; }

        public DateTime Timestamp { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ContentItem item &&
                   Id.Equals(item.Id) &&
                   Author == item.Author &&
                   Timestamp == item.Timestamp;
        }
    }
}
