using System.Text.Json.Serialization;

namespace Tournament.Application.Common.Models

{
	public record RangeSearch<TField>
	{
		public SearchOperator? SearchOperator { get;set;  }
		public TField Value { get;set; }

		public RangeSearch()
		{
		}

		public RangeSearch(SearchOperator searchOperator, TField value)
		{
			SearchOperator=searchOperator;
			Value=value;
		}

		internal bool HasValue => SearchOperator != null && Value!=null;
	}
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum SearchOperator
	{
		EqualTo=0,
		GreaterThanOrEqualTo=1,
		LessThanOrEqualTo=2
	}

}
