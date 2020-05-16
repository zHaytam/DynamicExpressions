using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicExpressions.Benchmarks
{
    [MemoryDiagnoser]
    public class Filter
    {
        private readonly List<Entry> _entries;

        public Filter()
        {
            _entries = new List<Entry>();
            for (int i = 0; i < 5000; i++)
            {
                _entries.Add(new Entry($"Title {i}", i, new SubEntry($"SubTitle {i}", i)));
            }
        }

        [Benchmark(Baseline = true)]
        public List<Entry> ManualLambda()
        {
            return _entries.AsQueryable().Where(e => 
                e.Title.Contains("Title")
                || (e.Title.EndsWith("1") && e.Number == 1)
                || (e.Title.EndsWith("2") && e.Number == 2)).ToList();
        }

        [Benchmark]
        public List<Entry> GeneratedLambda()
        {
            var predicate = new DynamicFilterBuilder<Entry>()
                .And("Title", FilterOperator.Contains, "Title")
                .Or(b => b.And("Title", FilterOperator.EndsWith, "1").And("Number", FilterOperator.Equals, 1))
                .Or(b => b.And("Title", FilterOperator.EndsWith, "2").And("Number", FilterOperator.Equals, 2))
                .Build();

            return _entries.AsQueryable().Where(predicate).ToList();
        }

    }
}
