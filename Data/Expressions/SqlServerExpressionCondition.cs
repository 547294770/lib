using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Common.Data.Expressions
{
    internal class SqlServerExpressionCondition : ExpressionCondition
    {
        public SqlServerExpressionCondition(Expression expression) : base(expression)
        {

        }
    }
}
