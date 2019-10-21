using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Remapper
{
    public class FlexVisitor : ExpressionVisitor
    {
        protected Func<Expression, bool> _finder = null;
        protected Func<Expression, Expression> _replacer = null;

        public FlexVisitor(Func<Expression, bool> finder, Func<Expression, Expression> replacer)
        {
            _finder = finder;
            _replacer = replacer;
        }

        public override Expression Visit(Expression node)
        {
            if (_finder(node))
            {
                return _replacer(node);
            }
            return base.Visit(node);
        }
    }
}
