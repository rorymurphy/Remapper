using Remapper.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remapper.Test.DataAccess
{
    class BlogPostRepository
    {
        protected ContentDbContext _context;
        protected IQueryable<BlogPost> _transformedQueryable;
        public BlogPostRepository(ContentDbContext context)
        {
            this._context = context;

            var provider = new TransformQueryProvider<BlogPostDTO, BlogPost>()
            {
                InnerQueryable = context.BlogPostDTOs,
                Transform = dto => new BlogPost() { Id = dto.Id, Title = dto.Title, Timestamp = dto.ContentItem.Timestamp, Author = dto.ContentItem.Author, Content = dto.Content }
            };
            provider.AddMapping(b => b.Id, dto => dto.Id);
            provider.AddMapping(b => b.Content, dto => dto.Content);
            provider.AddMapping(b => b.Title, dto => dto.Title);
            provider.AddMapping(b => b.Author, dto => dto.ContentItem.Author);
            provider.AddMapping(b => b.Timestamp, dto => dto.ContentItem.Timestamp);

            _transformedQueryable = provider.CreateEmptyQuery();
        }

        public void Add(BlogPost post)
        {
            var transferObj = new ContentItemDTO(post);
            this._context.ContentItemDTOs.Add(transferObj);
        }

        public IQueryable<BlogPost> Posts
        {
            get
            { return _transformedQueryable; }
        }

        public void Update(BlogPost post)
        {
            var transferObj = new ContentItemDTO(post);
            this._context.ContentItemDTOs.Update(transferObj);
        }

        public void Delete(Guid id)
        {
            this._context.ContentItemDTOs.Remove(this._context.ContentItemDTOs.Find(id));
        }
    }
}
