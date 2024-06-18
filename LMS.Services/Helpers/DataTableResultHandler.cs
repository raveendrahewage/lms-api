using LMS.Data.Models;
using LMS.Services.Common;
using LMS.Services.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Services.Helpers
{
    public static class DataTableResultHandler<T>
    {
        private static DataTableResult<T> result { get; set; } = new DataTableResult<T>();
        public static DataTableResult<T> ResultToSsr(
            List<T> list,
            DataTableConfiguration dataTableConfiguration,
            DataTableConfigurationOptions dataTableConfigurationOptions)
        {
            if (dataTableConfiguration == null)
                throw new ArgumentNullException(nameof(dataTableConfiguration), "Table configurations cannot be null.");

            if (list == null)
                throw new ArgumentNullException(nameof(list), "Input list cannot be null.");

            result.TotalRecords = list.Count;

            switch (dataTableConfigurationOptions)
            {
                case DataTableConfigurationOptions.All:
                    list = ApplyAll(list, dataTableConfiguration);
                    break;
                case DataTableConfigurationOptions.Search:
                    list = ApplySearch(list, dataTableConfiguration);
                    break;
                case DataTableConfigurationOptions.Sorting:
                    list = ApplySorting(list, dataTableConfiguration);
                    break;
                case DataTableConfigurationOptions.Pagination:
                    list = ApplyPagination(list, dataTableConfiguration);
                    break;
                default:
                    throw new ArgumentException("Invalid table configuration option specified.", nameof(dataTableConfigurationOptions));
            }

            result.Data = list;
            return result;
        }

        private static List<T> ApplyAll(List<T> list, DataTableConfiguration dataTableConfiguration)
        {
            list = ApplySearch(list, dataTableConfiguration);
            list = ApplySorting(list, dataTableConfiguration);
            list = ApplyPagination(list, dataTableConfiguration);
            return list;
        }
        private static List<T> ApplySorting(List<T> list, DataTableConfiguration dataTableConfiguration)
        {
            switch (dataTableConfiguration.sortMode)
            {
                case SortMode.ASC:
                    list = list.OrderBy(x => x.GetType().GetProperty(dataTableConfiguration.sortBy)?.GetValue(x, null))
                        .ToList();
                    break;
                case SortMode.DESC:
                    list = list.OrderByDescending(x =>
                        x.GetType().GetProperty(dataTableConfiguration.sortBy)?.GetValue(x, null))
                        .ToList();
                    break;
            }
            return list;
        }
        private static List<T> ApplySearch(List<T> list, DataTableConfiguration dataTableConfiguration)
        {
            if (dataTableConfiguration.search == null || string.IsNullOrWhiteSpace(dataTableConfiguration.search))
                return list;
            var parameter = Expression.Parameter(typeof(T), "entity");
            foreach (var word in dataTableConfiguration.search.ToLower().Split(" "))
            {
                var orConditions = new List<Expression>();
                var properties = typeof(T).GetProperties()
                                  .Where(p => p.PropertyType == typeof(string))
                                  .Select(p => p.Name)
                                  .ToList();
                foreach (var column in properties)
                {
                    var property = Expression.Property(parameter, column);
                    var toStringMethod = typeof(object).GetMethod("ToString");
                    var propertyToString = Expression.Call(Expression.Convert(property, typeof(object)), toStringMethod);

                    var nullCheck = Expression.NotEqual(property, Expression.Constant(null));

                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var toLowerCall = Expression.Call(propertyToString, toLowerMethod);

                    var searchTermLower = Expression.Constant(word.ToLower(), typeof(string));

                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var containsCall = Expression.Call(toLowerCall, containsMethod, searchTermLower);

                    var condition = Expression.AndAlso(nullCheck, containsCall);
                    orConditions.Add(condition);
                }
                var searchOrCondition = orConditions.Any() ? orConditions.Aggregate(Expression.OrElse) : Expression.Constant(true);
                var searchPredicate = Expression.Lambda<Func<T, bool>>(searchOrCondition, parameter);
                list = list.AsQueryable().Where(searchPredicate).ToList();
            }
            result.TotalRecords = list.Count;
            return list;
        }

        private static List<T> ApplyPagination(List<T> list, DataTableConfiguration dataTableConfiguration)
        {
            var start = (dataTableConfiguration.page * dataTableConfiguration.pageSize);
            var end = start + dataTableConfiguration.pageSize;
            if (end <= list.Count)
                list = list.GetRange(start, dataTableConfiguration.pageSize);
            else list = list.GetRange(start, list.Count - start);

            return list;
        }
    }
}
