using Remapper.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remapper.Test.DataAccess
{
    class ProductReviewRepository
    {
        protected ContentDbContext _context;
        protected IQueryable<ProductReview> _transformedQueryable;
        public ProductReviewRepository(ContentDbContext context)
        {
            this._context = context;

            var provider = new TransformQueryProvider<ProductReviewDTO, ProductReview>()
            {
                InnerQueryable = context.ProductReviewDTOs,
                Transform = dto => new ProductReview() { Id = dto.Id, Rating = dto.Rating, Timestamp = dto.ContentItem.Timestamp, Author = dto.ContentItem.Author, Review = dto.Review }
            };
            provider.AddMapping(b => b.Id, dto => dto.Id);
            provider.AddMapping(b => b.Timestamp, dto => dto.ContentItem.Timestamp);
            provider.AddMapping(b => b.Author, dto => dto.ContentItem.Author);
            provider.AddMapping(b => b.Rating, dto => dto.Rating);
            provider.AddMapping(b => b.Review, dto => dto.Review);

            _transformedQueryable = provider.CreateEmptyQuery();
        }

        public void Add(ProductReview post)
        {
            var transferObj = new ContentItemDTO(post);
            this._context.ContentItemDTOs.Add(transferObj);
        }

        public IQueryable<ProductReview> Reviews
        {
            get
            { return _transformedQueryable; }
        }

        public void Update(ProductReview post)
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
