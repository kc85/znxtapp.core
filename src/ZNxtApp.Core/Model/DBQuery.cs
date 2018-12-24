using System.Collections.Generic;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Model
{
    public class DBQuery
    {
        public FilterQuery Filters { get; set; }
        public List<Field> Fields { get; set; }
        public List<SortField> SortBy { get; set; }

        public DBQuery()
        {
            Filters = new FilterQuery();
            Fields = new List<Field>();
            SortBy = new List<SortField>();
        }
    }

    public class FilterQuery : List<Filter>
    {
    }

    public class Filter
    {
        public FilterField Field { get; set; }
        public FilterCondition Condition { get; set; }
        public FilterOperator Operator { get; set; }

        private Filter(FilterOperator filterOperator, FilterCondition condition)
        {
            this.Condition = condition;
            this.Operator = filterOperator;
        }

        public Filter(string field, string value, FilterOperator filterOperator = FilterOperator.Equal, FilterCondition condition = FilterCondition.AND) : this(filterOperator, condition)
        {
            this.Field = new FilterField(field, value);
        }

        public Filter(string field, List<string> values, FilterOperator filterOperator = FilterOperator.Equal, FilterCondition condition = FilterCondition.AND) : this(filterOperator, condition)
        {
            this.Field = new FilterField(field, values);
        }
    }

    public enum FilterCondition
    {
        AND,
        OR
    }

    public enum FilterOperator
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanEquals,
        GreaterThan,
        GreaterThanEquals,
        In,
        NotIn
    }

    public class Field
    {
        public string Name { get; set; }
        public FieldDataType DataType { get; set; }

        public Field(string name)
        {
            this.Name = name;
        }
    }

    public class FilterField : Field
    {
        public bool IgnoreCase { get; set; }

        public FilterField(string name, string value, bool ignoreCase = false) : base(name)
        {
            IgnoreCase = ignoreCase;
            this.Values = new List<string>();
            this.Values.Add(value);
        }

        public FilterField(string name, List<string> values, bool ignoreCase = false) : base(name)
        {
            this.Values = values;
            IgnoreCase = ignoreCase;
        }

        public List<string> Values { get; set; }
    }

    public class SortField : Field
    {
        public SortField(string name, SortType sort = SortType.ASC) : base(name)
        {
            Sort = sort;
        }

        public SortType Sort { get; set; }
    }

    public enum FieldDataType
    {
        String,
        Numeric
    }

    public enum SortType
    {
        ASC = 1,
        DESC = -1
    }

    public class RawQuery : IDBQueryBuilder
    {
        private string _filter;

        public RawQuery(string filter)
        {
            _filter = filter;
        }

        public string GetQuery()
        {
            return _filter;
        }
    }
}