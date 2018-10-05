using System.Collections.Generic;
using System.Linq;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.DB.Mongo
{
    public class MongoQueryBuilder : IDBQueryBuilder
    {
        private FilterQuery _filter;
        private Dictionary<FilterOperator, string> _opratorMapping;

        public MongoQueryBuilder(FilterQuery filter)
        {
            _filter = filter;
            SetOperatorMapping();
        }

        private void SetOperatorMapping()
        {
            _opratorMapping = new Dictionary<FilterOperator, string>();
            _opratorMapping[FilterOperator.Equal] = "$eq";
            _opratorMapping[FilterOperator.NotEqual] = "$ne";
            _opratorMapping[FilterOperator.LessThan] = "$lt";
            _opratorMapping[FilterOperator.LessThanEquals] = "$lte";
            _opratorMapping[FilterOperator.GreaterThan] = "$gt";
            _opratorMapping[FilterOperator.GreaterThanEquals] = "$gte";
            _opratorMapping[FilterOperator.In] = "$in";
            _opratorMapping[FilterOperator.NotIn] = "$nin";
        }

        public string GetQuery()
        {
            List<string> filters = new List<string>();

            for (int filterCount = 0; filterCount < _filter.Count; filterCount++)
            {
                filters.Add(BuildFilterFiled(_filter[filterCount]));
            }

            return BuildFilter(filters);
        }

        private string BuildFilterFiled(Filter filter)
        {
            var resultFilter = string.Empty;
            resultFilter = string.Format("{{'{0}':{{ {1} : {2} }} }}", filter.Field.Name, GetOperator(filter.Operator), GetFilterData(filter));
            return resultFilter;
        }

        private string GetFilterData(Filter filter)
        {
            if (filter.Field.IgnoreCase)
            {
                ///^bar$/i

                switch (filter.Operator)
                {
                    case FilterOperator.In:
                    case FilterOperator.NotIn:
                        return string.Format("[/{0}/i]", string.Join("/i,/", filter.Field.Values));

                    default:
                        return string.Format("/{0}/i", filter.Field.Values.First());
                }
            }
            else
            {
                switch (filter.Operator)
                {
                    case FilterOperator.LessThan:
                    case FilterOperator.LessThanEquals:
                    case FilterOperator.GreaterThan:
                    case FilterOperator.GreaterThanEquals:
                        return filter.Field.Values.First();

                    case FilterOperator.In:
                    case FilterOperator.NotIn:
                        if (filter.Field.DataType == FieldDataType.Numeric)
                        {
                            return string.Format("[{0}]", string.Join(",", filter.Field.Values));
                        }
                        else
                        {
                            return string.Format("['{0}']", string.Join("','", filter.Field.Values));
                        }
                    default:
                        return string.Format("'{0}'", filter.Field.Values.First());
                }
            }
        }

        private string BuildFilter(List<string> filters)
        {
            if (_filter.Count == 0)
            {
                return "{}";
            }
            else
            {
                var filter = string.Join(",", filters);
                var condition = "$and";
                if (_filter.First().Condition == FilterCondition.OR)
                {
                    condition = "$or";
                }
                return string.Format("{{ {0} : [{1}] }}", condition, filter);
            }
        }

        private string GetOperator(FilterOperator foperator)
        {
            if (_opratorMapping.ContainsKey(foperator))
            {
                return _opratorMapping[foperator];
            }
            else
            {
                throw new KeyNotFoundException(string.Format("FilterOperator key not found {0}", foperator.ToString()));
            }
        }
    }
}