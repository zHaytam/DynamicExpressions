using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DynamicExpressions.UnitTests
{
    public class PropertyGetterTests
    {
        [Fact]
        public void GetPropertyGetter_ShouldThrow_WhenPropertyIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                DynamicExpressions.GetPropertyGetter<PropertyGetterTests>(null);
            });

            Assert.Equal("Value cannot be null. (Parameter 'property')", ex.Message);
        }

        [Fact]
        public void GetPropertyGetter_ShouldReturnCorrectGetter()
        {
            var entry = new Entry<string>(2);

            var getter = DynamicExpressions.GetPropertyGetter<Entry<string>>("Id").Compile();
            var id = getter(entry);

            Assert.Equal(entry.Id, id);
        }

        [Fact]
        public void GetPropertyGetter_ShouldBeUsableInQueryableOrderBy()
        {
            var entries = new List<Entry<string>>
            {
                new Entry<string>(1),
                new Entry<string>(2),
            };

            var getter = DynamicExpressions.GetPropertyGetter<Entry<string>>("Id");
            var sortedEntries = entries.AsQueryable().OrderByDescending(getter).ToList();

            Assert.Equal(entries[0], sortedEntries[1]);
            Assert.Equal(entries[1], sortedEntries[0]);
        }

        [Fact]
        public void GetPropertyGetter_ShouldThrow_WhenPropertyDoesntExist()
        {
            var entry = new Entry<string>(2);

            var ex = Assert.Throws<ArgumentException>(() => DynamicExpressions.GetPropertyGetter<Entry<string>>("Test"));

            Assert.Equal("Instance property 'Test' is not defined for type 'DynamicExpressions.UnitTests.Entry`1[System.String]' (Parameter 'propertyName')", ex.Message);
        }

        [Fact]
        public void GetPropertyGetter_ShouldHandleNestedProperty()
        {
            var entry = new Entry<string>(1, new SubEntry<string>("Title"));

            var getter = DynamicExpressions.GetPropertyGetter<Entry<string>>("SubEntry.Title").Compile();
            var value = getter(entry);

            Assert.Equal(entry.SubEntry.Title, value);
        }
    }
}