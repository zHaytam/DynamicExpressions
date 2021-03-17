using System.Collections.Generic;
using Xunit;

namespace DynamicExpressions.UnitTests
{
    public class PredicateTests
    {
        [Theory]
        [InlineData(1, "Title 1", "Id", FilterOperator.Equals, 1)]
        [InlineData(2, "Title 2", "Id", FilterOperator.DoesntEqual, 0)]
        [InlineData(6, "Title 6", "Id", FilterOperator.GreaterThan, 5)]
        [InlineData(7, "Title 7", "Id", FilterOperator.GreaterThanOrEqual, 6)]
        [InlineData(8, "Title 8", "Id", FilterOperator.GreaterThanOrEqual, 8)]
        [InlineData(9, "Title 9", "Id", FilterOperator.LessThan, 10)]
        [InlineData(10, "Title 10", "Id", FilterOperator.LessThanOrEqual, 12)]
        [InlineData(11, "Title 11", "Id", FilterOperator.LessThanOrEqual, 11)]
        public void GetPredicate_ShouldHandleNumericalOperators(int id, string title,
            string property, FilterOperator op, object value)
        {
            var entry = new Entry<string>(id, new SubEntry<string>(title));
            var predicate = DynamicExpressions.GetPredicate<Entry<string>>(property, op, value).Compile();
            Assert.True(predicate(entry));
        }

        [Theory]
        [InlineData(3, "Title 3", FilterOperator.Contains, "3")]
        [InlineData(3, "Title 3", FilterOperator.NotContains, "5")]
        [InlineData(4, "Title 4", FilterOperator.StartsWith, "Title")]
        [InlineData(5, "Title 5", FilterOperator.EndsWith, "5")]
        public void GetPredicate_ShouldHandleNestedStringOperators(int id, string title, FilterOperator op, object value)
        {
            var entry = new Entry<string>(id, new SubEntry<string>(title));
            var predicate = DynamicExpressions.GetPredicate<Entry<string>>("SubEntry.Title", op, value).Compile();
            Assert.True(predicate(entry));
        }

        [Theory]
        [InlineData(3, new string[1] { "Title 3" }, FilterOperator.Contains, "Title 3")]
        [InlineData(3, new string[1] { "Title 3" }, FilterOperator.NotContains, "Title 5")]
        public void GetPredicate_ShouldHandleEnumerableStringOperators(int id, string[] title, FilterOperator op, object value)
        {
            var entry = new Entry<string[]>(id, new SubEntry<string[]>(title));
            var predicate = DynamicExpressions.GetPredicate<Entry<string[]>>("SubEntry.Title", op, value).Compile();
            Assert.True(predicate(entry));
        }

        [Theory]
        [InlineData(1, "Title 1", FilterOperator.Contains, 1)]
        [InlineData(3, "Title 3", FilterOperator.NotContains, 5)]
        [InlineData(1, "1 Title", FilterOperator.StartsWith, 1)]
        [InlineData(1, "Title 1", FilterOperator.EndsWith, 1)]
        public void GetPredicate_ShouldWork_WhenValueIsNotStringAndOperatorIsStringBased(int id, string title,
            FilterOperator op, object value)
        {
            var entry = new Entry<string>(id, new SubEntry<string>(title));
            var predicate = DynamicExpressions.GetPredicate<Entry<string>>("SubEntry.Title", op, value).Compile();
            Assert.True(predicate(entry));
        }
    }
}
