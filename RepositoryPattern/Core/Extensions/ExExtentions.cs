using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class ExExtentions
    {
        private static string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Not:
                    return "NOT";

                case ExpressionType.AndAlso:
                    return "AND";

                case ExpressionType.OrElse:
                    return "OR";

                case ExpressionType.LessThan:
                    return "<";

                case ExpressionType.LessThanOrEqual:
                    return "<=";

                case ExpressionType.GreaterThan:
                    return ">";

                case ExpressionType.GreaterThanOrEqual:
                    return ">=";

                case ExpressionType.Equal:
                    return "=";

                case ExpressionType.NotEqual:
                    return "<>";

                default:
                    return string.Empty;
            }
        }
        public static string ToSQL<T>(this Expression<Func<T, bool>> expression)
        {
            dynamic operation = expression.Body;
            var result = WriteSql(operation);
            return result;
        }
        private static string BinaryExperssionWriter(BinaryExpression operation, string operators)
        {
            var left = operation.Left;
            var right = operation.Right;
            var result = string.Empty;

            switch (operators)
            {
                case "AND":
                case "OR":
                    result = $" ({WriteSql(left, operators)} {operators} {WriteSql(right, operators, true)}) ";
                    break;

                default:
                    result = $" {WriteSql(left, operators)} {operators} {WriteSql(right, operators, true)} ";
                    break;
            }

            return result;
        }

        private static string WriteSql(Expression expression, string opr = "", bool right = false)
        {
            var nodeType = expression.NodeType;
            var operators = GetOperator(nodeType);
            var result = string.Empty;


            switch (nodeType)
            {
                case ExpressionType.AndAlso:
                    {
                        result = BinaryExperssionWriter((BinaryExpression)expression, operators);
                    }
                    break;

                case ExpressionType.Constant:
                    {
                        var operation = (ConstantExpression)expression;
                        var opType = operation.Type;

                        if (opType == typeof(string))
                        {
                            result = $"N'{operation.Value}'";
                        }
                        else if (opType == typeof(bool))
                        {
                            if ((bool)operation.Value)
                            {
                                result = "1";
                            }
                            else
                            {
                                result = "0";
                            }
                        }
                        else
                        {
                            result = operation.Value.ToString();
                        }
                    }
                    break;

                case ExpressionType.Equal:
                    {
                        result = BinaryExperssionWriter((BinaryExpression)expression, operators);
                    }
                    break;

                case ExpressionType.GreaterThan:
                    {
                        result = BinaryExperssionWriter((BinaryExpression)expression, operators);
                    }

                    break;

                case ExpressionType.GreaterThanOrEqual:
                    {
                        result = BinaryExperssionWriter((BinaryExpression)expression, operators);
                    }
                    break;

                case ExpressionType.Lambda:
                    {
                        var operation = ((LambdaExpression)expression).Body;

                        result = WriteSql(operation);
                    }
                    break;

                case ExpressionType.LessThan:
                    {
                        result = BinaryExperssionWriter((BinaryExpression)expression, operators);
                    }
                    break;

                case ExpressionType.LessThanOrEqual:
                    {
                        result = BinaryExperssionWriter((BinaryExpression)expression, operators);
                    }
                    break;

                case ExpressionType.MemberAccess:
                    {
                        var operation = ((MemberExpression)expression);
                        result = BinaryExperssionWriter((BinaryExpression)expression, operators);
                    }
                    break;

                case ExpressionType.Not:
                    {
                        var operation = ((UnaryExpression)expression).Operand;
                        result = WriteSql(operation, "!");
                    }
                    break;

                case ExpressionType.NotEqual:
                    {
                        result = BinaryExperssionWriter((BinaryExpression)expression, operators);
                    }
                    break;

                case ExpressionType.OrElse:
                    {
                        result = BinaryExperssionWriter((BinaryExpression)expression, operators);
                    }
                    break;

                case ExpressionType.Parameter:
                    {
                        var operation = ((ParameterExpression)expression);
                        result = WriteSql(operation, operators);
                    }

                    break;

                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

    }
}