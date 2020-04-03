using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace Remapper
{
    public class TransformQueryProvider<TFrom, TTo> : IQueryProvider
    {
        public TransformQueryProvider(Func<TFrom, TTo> transform, IQueryable<TFrom> innerQueryable) : this()
        {
            this.Transform = transform;
            this.InnerQueryable = innerQueryable;
        }

        protected TransformQueryProvider()
        {
            this.Visitor = new TypeMapVisitor<TTo, TFrom>();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TransformQueryable<TElement>()
            {
                Expression = expression,
                Provider = this
            };
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return this.CreateQuery<object>(expression);
        }

        protected static ConcurrentDictionary<Type, Delegate> _cachedInvokers = new ConcurrentDictionary<Type, Delegate>();
        protected TResult DynamicallyCreateQuery<TResult>(Type elementType, System.Linq.Expressions.Expression expression)
        {
            if (!_cachedInvokers.ContainsKey(elementType)) {
                var typeInfo = this.GetType().GetTypeInfo();
                var mInfo = typeInfo.GetMethods()
                    .Where(m => m.Name == nameof(CreateQuery) && m.IsGenericMethod)
                    .SingleOrDefault();
                mInfo = mInfo.MakeGenericMethod(elementType);
                var expr = Expression.Parameter(typeof(Expression), "expression");
                var call = Expression.Call(mInfo, expr);
                Func<Expression, TResult> lambda = Expression.Lambda<Func<Expression, TResult>>(call, expr).Compile();
                _cachedInvokers[elementType] = lambda;
            }

            return ((Func<Expression, TResult>)_cachedInvokers[elementType])(expression);
        }

        public IQueryable<TTo> CreateEmptyQuery()
        {
            return new TransformQueryable<TTo>()
            {
                Provider = this,
                Expression = Expression.Constant(this.ConstantExpression)
            };
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            if (this.Transform == null)
            { throw new InvalidOperationException("Must specify a transform before performing a query against the TransformQueryProvider"); }

            if (!this.Visitor.ConstantMappings.ContainsKey(this.ConstantExpression))
            {
                this.Visitor.ConstantMappings.Add(this.ConstantExpression, this.InnerQueryable.Expression);
            }

            TResult result = default(TResult);
            TypeInfo qType = typeof(IQueryable).GetTypeInfo();
            Type gqType = typeof(IQueryable<>);
            TypeInfo gqTypeInfo = gqType.GetTypeInfo();
            Type resType = typeof(TResult);
            TypeInfo resTypeInfo = resType.GetTypeInfo();

            // Check if the type being requested can accept an IQueryable<T>, in which case we should not evaluate the expression
            // but instead return a new query.  First check is to ensure the TResult type is an interface, otherwise the rest definitely
            // won't work, so returning null to short-circuit the logic.  Seems like the best way to do it for performance and compactness,
            // so lots of reflection operations don't needlessly occur.
            var query = !resTypeInfo.IsInterface
                ? null
                : (new TypeInfo[] { resTypeInfo })
                    .Union( resTypeInfo.GetInterfaces().Select(i => i.GetTypeInfo()) )
                    .Where(i => i.IsGenericType 
                        && qType.IsAssignableFrom(i)
                        && i.GetGenericTypeDefinition() == gqType)
                    .SingleOrDefault();
            if (query != null)
            {
                result = DynamicallyCreateQuery<TResult>(query.GetGenericArguments()[0], expression);
                //result = (TResult)this
                //    .CallMethod(
                //        new Type[] {
                //            query.GetGenericArguments()[0]
                //        },
                //        "CreateQuery",
                //        expression);
            }
            else if (resType == typeof(IEnumerable<TTo>))
            {
                expression = this.Visitor.Visit(expression);
                FlexVisitor vis = new FlexVisitor((exp) => exp == this.ConstantExpression, (exp) => this.InnerQueryable.Expression);
                expression = vis.Visit(expression);
                var temp = InnerQueryable.Provider.CreateQuery<TFrom>(expression).ToArray();
                if (this.Transform != null)
                {
                    result = (TResult)((object)temp.Select(o => Transform(o)));
                }
            }
            else if (resType == typeof(TTo))
            {
                expression = this.Visitor.Visit(expression);
                FlexVisitor vis = new FlexVisitor((exp) => exp == this.ConstantExpression, (exp) => this.InnerQueryable.Expression);
                expression = vis.Visit(expression);
                var temp = InnerQueryable.Provider.Execute<TFrom>(expression);
                if (this.Transform != null)
                {
                    result = (TResult)((object)Transform(temp));
                }
            }
            else
            {
                expression = this.Visitor.Visit(expression);
                FlexVisitor vis = new FlexVisitor((exp) => exp == this.ConstantExpression, (exp) => this.InnerQueryable.Expression);
                expression = vis.Visit(expression);
                result = InnerQueryable.Provider.Execute<TResult>(expression);
            }

            return result;
        }

        public object Execute(Expression expression)
        {
            return this.Execute<object>(expression);
        }

        public void AddMapping<TValue>(Expression<Func<TTo, TValue>> toExpression, Expression<Func<TFrom, TValue>> fromExpression)
        {
            this.Visitor.AddMapping<TValue>(toExpression, fromExpression);
        }

        public Func<TFrom, TTo> Transform { get; set; }

        public TypeMapVisitor<TTo, TFrom> Visitor { get; protected internal set; }

        public IQueryable InnerQueryable { get; set; }

        protected object _constantExpression;
        public object ConstantExpression
        {
            get
            {
                _constantExpression = _constantExpression ?? new TTo[0].AsQueryable();
                return _constantExpression;
            }
        }
    }
}
