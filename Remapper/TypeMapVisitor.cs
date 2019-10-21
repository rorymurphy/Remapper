using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Linq.Expressions;
namespace Remapper
{
    public class TypeMapVisitor<TFrom, TTo> : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _parameters = new Dictionary<ParameterExpression, ParameterExpression>();

        public TypeMapVisitor(Dictionary<MemberInfo, LambdaExpression> mappings = null, Dictionary<object, object> constantMappings = null)
        {
            this.Mappings = mappings ?? new Dictionary<MemberInfo, LambdaExpression>();
            this.ConstantMappings = constantMappings ?? new Dictionary<object, object>();
        }

        public Dictionary<MemberInfo, LambdaExpression> Mappings { get; protected internal set; }

        public Dictionary<object, object> ConstantMappings { get; protected internal set; }

        public void AddMapping<TValue>(Expression<Func<TFrom, TValue>> fromExpression, Expression<Func<TTo, TValue>> toExpression)
        {
            this.Mappings.Add(
                ((MemberExpression)fromExpression.Body).Member,
                (LambdaExpression)toExpression);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            Expression result = node;
            if (ConstantMappings.ContainsKey(node.Value) && ConstantMappings[node.Value] is Expression)
            {
                result = (Expression)ConstantMappings[node.Value];
            }
            else if (ConstantMappings.ContainsKey(node.Value))
            {
                result = base.Visit(Expression.Constant(ConstantMappings[node.Value]));
            }
            return result;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Expression result = node;
            Type from = typeof(TFrom);
            Type to = typeof(TTo);

            Type[] argTypes = null;
            if (node.Method.IsGenericMethod
                && (argTypes = node.Method.GetGenericArguments()).Contains(from))
            {
                argTypes = argTypes.Select(t => (t == from) ? to : t).ToArray();
                MethodInfo meth = node.Method.GetGenericMethodDefinition().MakeGenericMethod(argTypes);
                result = base.Visit(
                    Expression.Call(
                        node.Object,
                        meth,
                        node.Arguments.Select(a => this.Visit(a))
                ));

            }
            else
            {
                result = base.VisitMethodCall(node);
            }

            return result;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Expression result = node;
            if (node.Type == typeof(TFrom))
            {
                ParameterExpression param;
                if (!this._parameters.TryGetValue(node, out param))
                {
                    this._parameters.Add(node, param = Expression.Parameter(typeof(TTo), node.Name));
                }
                result = base.VisitParameter(param);
            }
            else
            {
                result = base.VisitParameter(node);
            }
            return result;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Expression result = null;
            if (node.Expression != null && node.Expression.Type == typeof(TFrom))
            {
                Expression expression = this.Visit(node.Expression);
                if (expression.Type != typeof(TTo))
                {
                    throw new Exception("An error occurred");
                }
                LambdaExpression lambdaExpression;
                if (this.Mappings.TryGetValue(node.Member, out lambdaExpression))
                {
                    Expression param = lambdaExpression.Parameters.Single();
                    result = new FlexVisitor(
                        (expr) => expr == param,
                        (expr) => expression
                    ).Visit(lambdaExpression.Body);
                }else{
                    result = Expression.Property(expression, node.Member.Name);
                }
            }
            else
            {
                result = base.VisitMember(node);
            }
            return result;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Expression.Lambda(
                this.Visit(node.Body),
                node.Parameters.Select(this.Visit).Cast<ParameterExpression>()
            );
        }
    }
}
