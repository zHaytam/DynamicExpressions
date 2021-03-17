using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

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
        [InlineData(3, "", FilterOperator.IsEmpty, "doesn't matter. ignored")]
        [InlineData(3, null, FilterOperator.IsEmpty, "")]
        [InlineData(3, "Title 3", FilterOperator.IsNotEmpty, "")]
        public void GetPredicate_ShouldHandleNullOrEmtpyStringOperators(int id, string title, FilterOperator op, object value)
        {
            var entry = new Entry<string>(id, new SubEntry<string>(title));
            var predicate = DynamicExpressions.GetPredicate<Entry<string>>("SubEntry.Title", op, value).Compile();
            Assert.True(predicate(entry));
        }

        [Theory]
        [InlineData(3, "I'm not empty", FilterOperator.IsEmpty, "doesn't matter. ignored")]
        [InlineData(3, "Neither am i", FilterOperator.IsEmpty, "")]
        [InlineData(3, "", FilterOperator.IsNotEmpty, "")]
        [InlineData(3, null, FilterOperator.IsNotEmpty, "")]
        public void GetPredicate_ShouldHandleNullOrEmtpyStringOperatorsFalse(int id, string title, FilterOperator op, object value)
        {
            var entry = new Entry<string>(id, new SubEntry<string>(title));
            var predicate = DynamicExpressions.GetPredicate<Entry<string>>("SubEntry.Title", op, value).Compile();
            Assert.False(predicate(entry));
        }

        [Theory]
        [MemberData(nameof(ListTestData))]
        public void GetPredicate_ShouldHandleEnumerableStringOperators<T>(int id, T title, FilterOperator op, object value)
        {
            var entry = new Entry<T>(id, new SubEntry<T>(title));
            var predicate = DynamicExpressions.GetPredicate<Entry<T>>("SubEntry.Title", op, value).Compile();
            Assert.True(predicate(entry));
        }

        public static IEnumerable<object[]> ListTestData
        {
            get
            {
                return new[]{
                    new object[]{3, new List<string>() { "Title 3" }, FilterOperator.Contains, "Title 3" },
                    new object[]{3, new List<string>() { "Title 3" }, FilterOperator.NotContains, "Title 5" },
                    new object[]{3, new Dictionary<string, string> { { "Key 3", "Value 1" } }, FilterOperator.Contains, "Key 3" },
                    new object[]{3, new Dictionary<string, string> { { "Key 3", "Value 1" } }, FilterOperator.Contains, "Value 1" },
                    new object[]{3, new Dictionary<string, string> { { "Key 3", "Value 1" } }, FilterOperator.NotContains, "Key 5" },
                    new object[]{3, new Dictionary<string, string> { { "Key 3", "Value 1" } }, FilterOperator.NotContains, "Value 5" },
                    new object[]{3, new Dictionary<string, string> { { "Key 3", "Value 1" } }, FilterOperator.ContainsKey, "Key 3" },
                    new object[]{3, new Dictionary<string, string> { { "Key 3", "Value 1" } }, FilterOperator.ContainsValue, "Value 1" },
                    new object[]{3, new Dictionary<string, string> { { "Key 3", "Value 1" } }, FilterOperator.NotContainsKey, "Key 5" },
                    new object[]{3, new Dictionary<string, string> { { "Key 3", "Value 1" } }, FilterOperator.NotContainsValue, "Value 5" }
                };
            }
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
