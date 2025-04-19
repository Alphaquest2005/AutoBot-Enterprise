﻿
namespace System.Linq.Expressions
{
    public class ExpressionCSharp60
    {
        public static NullPropagationExpression NullPropagation(Expression receiver, ParameterExpression accessParameter,
            Expression accessExpression)
        {
            return new NullPropagationExpression(receiver, accessParameter, accessExpression);
        }
    }

    internal static class NullableExtensions
    {
        public static Type Nullify(this Type type)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
                return typeof (Nullable<>).MakeGenericType(type);

            return type;
        }

        public static Type Unnullify(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }
    }

    public class NullPropagationExpression : Expression
    {
        public Expression Receiver { get; private set; }
        public ParameterExpression AccessParameter { get; private set; }
        public Expression AccessExpression { get; private set; }
        public Type type;

        public NullPropagationExpression(Expression receiver, ParameterExpression accessParameter,
            Expression accessExpression)
        {
            this.Receiver = receiver;
            this.AccessParameter = accessParameter;
            this.AccessExpression = accessExpression;
            this.type = AccessExpression.Type.Nullify();
        }

        public override ExpressionType NodeType
        {
            get { return ExpressionType.Extension; }
        }

        public override Type Type
        {
            get { return type; }
        }

        public override bool CanReduce
        {
            get { return true; }
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var receiver = visitor.Visit(Receiver);
            var accessParameter = visitor.Visit(AccessParameter);
            var accessExpression = visitor.Visit(AccessExpression);

            return this.Update(receiver, accessParameter, accessExpression);
        }

        private Expression Update(Expression receiver, Expression accessParameter, Expression accessExpression)
        {
            if (receiver != this.Receiver || accessParameter != this.AccessParameter ||
                accessExpression != this.AccessExpression)
                return new NullPropagationExpression(receiver, AccessParameter, accessExpression);

            return this;
        }

        public override Expression Reduce()
        {
            if (this.Receiver.Type == this.AccessParameter.Type) //reference type
            {
                return Expression.Block(new[] {this.AccessParameter},
                    Expression.Assign(this.AccessParameter, this.Receiver),
                    Expression.Condition(
                        Expression.Equal(this.AccessParameter, Expression.Constant(null, this.AccessParameter.Type)),
                        Expression.Constant(null, this.Type),
                        this.AccessExpression));
            }
            else //nullable type
            {
                var nullableParam = Expression.Parameter(this.Receiver.Type);
                return Expression.Block(new[] {nullableParam},
                    Expression.Assign(nullableParam, this.Receiver),
                    Expression.Condition(
                        Expression.Equal(nullableParam, Expression.Constant(null, nullableParam.Type)),
                        Expression.Constant(null, this.Type),
                        Expression.Block(new[] {this.AccessParameter},
                            Expression.Assign(this.AccessParameter,
                                Expression.Convert(nullableParam, this.AccessParameter.Type)),
                            this.AccessExpression)));
            }
        }

        public override string ToString()
        {
            string receiver = Receiver.ToString();

            if (!(Receiver.NodeType == ExpressionType.MemberAccess ||
                  Receiver.NodeType == ExpressionType.Call ||
                  Receiver.NodeType == ExpressionType.Parameter))
                receiver = "(" + receiver + ")";

            var access = AccessExpression.ToString();
            var start = this.AccessParameter.ToString();
            if (access.StartsWith(start + ".") || access.StartsWith(start + "["))
                return receiver + "?" + access.Substring(start.Length);

            return receiver + "?(" + this.AccessParameter.ToString() + " => " + access + ")";
        }
    }
}
