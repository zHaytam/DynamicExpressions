using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicExpressions.Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var entries = new List<Entry>
            {
                new Entry("Title 1", 1, new SubEntry("SubTitle 1", 1)),
                new Entry("Title 2", 2, new SubEntry("SubTitle 2", 2))
            };

            //Expression<Func<Entry, object>> expr = DynamicExpressions.GetPropertyGetter<Entry>("SubEntry.Title");
            //var result = entries.AsQueryable().OrderByDescending(expr).ToList();

            //var expr = DynamicExpressions.GetPredicate<Entry>("SubEntry.Title", FilterOperator.Equals, "SubTitle 2");
            //var entry = entries.AsQueryable().Where(expr).ToList();


            //var predicate = DynamicFilterBuilder<Entry>
            //    .StartWith("Title", FilterOperator.Equals, "Title 1")
            //    .And("Number", FilterOperator.Equals, 2)
            //    .Build();

            //var entry = entries.AsQueryable().FirstOrDefault(predicate);
           
            Func<Entry, bool> x = e => (e.Title.EndsWith("1") && e.Number == 1) || (e.Title.EndsWith("2") && e.Number == 2);
            var predicate = new DynamicFilterBuilder<Entry>()
                .And("Title", FilterOperator.Contains, "Title")
                .Or(b => b.And("Title", FilterOperator.EndsWith, "1").And("Number", FilterOperator.Equals, 1))
                .Or(b => b.And("Title", FilterOperator.EndsWith, "2").And("Number", FilterOperator.Equals, 2))
                .Build();

            var e = entries.AsQueryable().Where(predicate).ToList();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }

    }
}
