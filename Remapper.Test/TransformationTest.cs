using Moq;
using Remapper.Test.DataAccess;
using Remapper.Test.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xunit;

namespace Remapper.Test
{
    public class TransformationTest
    {
        [Fact]
        public void TestMapping()
        {
            var data = new List<ContentItemDTO>();
            for (int i = 0; i < 100; i++)
            {
                var post = new ContentItemDTO(new BlogPost()
                {
                    Id = Guid.NewGuid(),
                    Timestamp = DateTime.Now,
                    Author = "Author" + i,
                    Title = "Title" + i,
                    Content = "Content" + i
                });
                data.Add(post);

            }

            var dataQueryable = data.AsQueryable();
            Expression exprResult = null;

            var mockProvider = new Mock<IQueryProvider>(MockBehavior.Strict);

            mockProvider.Setup(p => p.CreateQuery<It.IsAnyType>(It.IsAny<Expression>())).Callback((Action<Expression>)(expr =>
            {
                exprResult = expr;
            })
            ).Returns(new InvocationFunc(inv => dataQueryable.Provider.CreateQuery( (Expression)inv.Arguments[0] )));

            mockProvider.Setup(p => p.Execute<It.IsAnyType>(It.IsAny<Expression>())).Callback((Action<Expression>)(expr =>
            {
                exprResult = expr;
            })).Returns(new InvocationFunc(inv => dataQueryable.Provider.Execute((Expression)inv.Arguments[0])));
                
            var mockQueryable = new Mock<IQueryable<ContentItemDTO>>(MockBehavior.Strict);
            mockQueryable.Setup(q => q.Expression).Returns(dataQueryable.Expression);
            mockQueryable.As<IQueryable>().Setup(q => q.Expression).Returns(dataQueryable.Expression);
            mockQueryable.Setup(q => q.Provider).Returns(mockProvider.Object);
            mockQueryable.As<IQueryable>().Setup(q => q.Provider).Returns(mockProvider.Object);
            mockQueryable.Setup(q => q.ElementType).Returns(dataQueryable.ElementType);
            mockQueryable.Setup(q => q.GetEnumerator()).Returns(() => mockProvider.Object.Execute<IEnumerable<ContentItemDTO>>(mockQueryable.Object.Expression).GetEnumerator());
            mockQueryable.As<IEnumerable>().Setup(q => q.GetEnumerator()).Returns(() => mockProvider.Object.Execute<IEnumerable<ContentItemDTO>>(mockQueryable.Object.Expression).GetEnumerator());

            var provider = new TransformQueryProvider<ContentItemDTO, BlogPost>(
                dto => new BlogPost() { Id = dto.Id, Title = dto.BlogPost.Title, Timestamp = dto.Timestamp, Author = dto.Author, Content = dto.BlogPost.Content },
                mockQueryable.Object
            );
            provider.AddMapping(b => b.Id, dto => dto.Id);
            provider.AddMapping(b => b.Content, dto => dto.BlogPost.Content);
            provider.AddMapping(b => b.Title, dto => dto.BlogPost.Title);
            provider.AddMapping(b => b.Author, dto => dto.Author);
            provider.AddMapping(b => b.Timestamp, dto => dto.Timestamp);

            var transformedQueryable = provider.CreateEmptyQuery();

            Assert.Equal(100, mockQueryable.Object.Count());

            var result = transformedQueryable.Where(p => p.Title == "Title50").Single();
            Assert.Equal("Content50", result.Content);
            //Validates that the query is being translated into the original type
            Assert.Equal(typeof(ContentItemDTO), exprResult.Type);
            Assert.Equal(ExpressionType.Call, exprResult.NodeType);
            Assert.Equal(typeof(Queryable).GetMethods().Where(m => m.Name == nameof(Queryable.Single) && m.GetParameters().Length == 1).Single(),
                ((MethodCallExpression)exprResult).Method.GetGenericMethodDefinition());
            Assert.Collection(((MethodCallExpression)exprResult).Arguments, (nestedExpr) =>
            {
                Assert.Equal(ExpressionType.Call, nestedExpr.NodeType);
                MethodInfo whereMethod = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(Queryable.Where)
                    && m.GetParameters().Length == 2
                    && m.GetParameters().Last().ParameterType.GenericTypeArguments.Length == 1
                    && m.GetParameters().Last().ParameterType.GenericTypeArguments.Single().GenericTypeArguments.Length == 2
                    && m.GetParameters().Last().ParameterType.GenericTypeArguments.Single().GenericTypeArguments.First() != typeof(int)).Single();
                Assert.Equal(whereMethod, ((MethodCallExpression)nestedExpr).Method.GetGenericMethodDefinition());
            });
            
        }
    }
}
