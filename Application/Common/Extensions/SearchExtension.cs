using System.Linq.Expressions;
using Tournament.Application.Common.Models;

namespace Tournament.Application.Common.Extensions
{
	public static class SearchExtension {
		public static IQueryable<T> WhereRangeSearch<T,Tfield>(this IQueryable<T> source, Expression<Func<T, Tfield>> selector, RangeSearch<Tfield> rangeSearch)
		{
			if (rangeSearch==null || !rangeSearch.HasValue)
				return source;

			var parameter = selector.Parameters.Single();
			BinaryExpression expression;

			switch (rangeSearch.SearchOperator)
			{
				case SearchOperator.EqualTo:
					expression = Expression.Equal(selector.Body, Expression.Constant(rangeSearch.Value, typeof(Tfield)));
					break;

				case SearchOperator.GreaterThanOrEqualTo:
					expression = Expression.GreaterThanOrEqual(selector.Body, Expression.Constant(rangeSearch.Value, typeof(Tfield)));
					break;

				case SearchOperator.LessThanOrEqualTo:
					expression = Expression.LessThanOrEqual(selector.Body, Expression.Constant(rangeSearch.Value, typeof(Tfield)));
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return source.Where(Expression.Lambda<Func<T, bool>>(expression, parameter));
		}
	}
}
