using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace DynamicExpressions.Benchmarks
{
    [MemoryDiagnoser]
    public class OrderByDescending
    {

        private readonly List<Entry> _entries;

        public OrderByDescending()
        {
            _entries = new List<Entry>();
            for (int i = 0; i < 5000; i++)
            {
                _entries.Add(new Entry($"Title {i}", i));
            }
        }

        [Benchmark(Baseline = true)]
        public List<List<Entry>> SearchTitle()
        {
            var result = new List<List<Entry>>();

            for (int i = 0; i <= 500; i++)
            {
                var orderedEntries = _entries.AsQueryable().OrderByDescending(p => p.Title).ToList();
                result.Add(orderedEntries);
            }

            return result;
        }

        [Benchmark]
        public List<List<Entry>> SearchTitleDynamicallyWithoutCache()
        {
            var result = new List<List<Entry>>();

            for (int i = 0; i <= 500; i++)
            {
                var expr = DynamicExpressions.GetPropertyGetter<Entry>("Title");
                var orderedEntries = _entries.AsQueryable().OrderByDescending(expr).ToList();
                result.Add(orderedEntries);
            }

            return result;
        }

    }
}
