using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remapper
{
    public class TransformQueryable<T> : IOrderedQueryable<T>
    {
        protected IEnumerable<T> _executedQuery = null;
        public IEnumerator<T> GetEnumerator()
        {
            return this.Provider.Execute<IEnumerable<T>>(this.Expression).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get;
            internal set;
        }

        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get { return this.Expression; }
        }

        public IQueryProvider Provider
        {
            get;
            internal set;
        }

        IQueryProvider IQueryable.Provider
        {
            get
            {
                return this.Provider;
            }
        }
    }
}
