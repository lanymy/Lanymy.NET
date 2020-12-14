using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lanymy.Common.ExtensionFunctions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 将两个表达式用或连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">表达式1.</param>
        /// <param name="expr2">表达式2.</param>
        /// <returns>新的表达式</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null) return expr2;
            if (expr2 == null) return expr1;
            var secondBody = expr2.Body.Replace(expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, secondBody), expr1.Parameters);
        }

        /// <summary>
        /// 将两个表达式用并且连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">表达式1.</param>
        /// <param name="expr2">表达式2.</param>
        /// <returns>新的表达式</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null) return expr2;
            if (expr2 == null) return expr1;
            var secondBody = expr2.Body.Replace(expr2.Parameters[0], expr1.Parameters[0]);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, secondBody), expr1.Parameters);
        }

        /// <summary>
        /// 替换表达式
        /// </summary>
        /// <param name="expression">原表达式</param>
        /// <param name="searchEx">被替换的表达式</param>
        /// <param name="replaceEx">将替换成的表达式</param>
        /// <returns>新的表达式.</returns>
        public static Expression Replace(this Expression expression, Expression searchEx, Expression replaceEx)
        {
            return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
        }

        /// <summary>
        /// 将两个实体操作表达式合并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, T>> Mergin<T>(this Expression<Func<T, T>> expr1, Expression<Func<T, T>> expr2) where T : class
        {
            if (expr1 == null) return expr2;
            if (expr2 == null) return expr1;
            var firstBody = expr1.Body as MemberInitExpression;
            var secondBody = expr2.Body as MemberInitExpression;
            var merginBody = Expression.MemberInit(firstBody.NewExpression, firstBody.Bindings.Union(secondBody.Bindings));
            return Expression.Lambda<Func<T, T>>(merginBody, expr1.Parameters);
        }

        /// <summary>
        /// 将两个实体操作表达式合并
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T>> Mergin<T>(this Expression<Func<T>> expr1, Expression<Func<T>> expr2) where T : class
        {
            if (expr1 == null) return expr2;
            if (expr2 == null) return expr1;
            var firstBody = expr1.Body as MemberInitExpression;
            var secondBody = expr2.Body as MemberInitExpression;
            var merginBody = Expression.MemberInit(firstBody.NewExpression, firstBody.Bindings.Union(secondBody.Bindings));
            return Expression.Lambda<Func<T>>(merginBody, expr1.Parameters);
        }

        /// <summary>
        /// 表达式替换访问器.
        /// </summary>
        private class ReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression _from, _to;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            public ReplaceVisitor(Expression from, Expression to)
            {
                _from = from;
                _to = to;
            }

            /// <summary>
            /// 访问器
            /// </summary>
            /// <param name="node">节点.</param>
            public override Expression Visit(Expression node)
            {
                return node == _from ? _to : base.Visit(node);
            }
        }
    }
}
