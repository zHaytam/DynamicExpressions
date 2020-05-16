namespace DynamicExpressions.Benchmarks
{
    public class Entry
    {

        public string Title { get; set; }
        public int Number { get; set; }
        public SubEntry SubEntry { get; set; }

        public Entry(string title, int number)
        {
            Title = title;
            Number = number;
        }

        public Entry(string title, int number, SubEntry subEntry) : this(title, number)
        {
            SubEntry = subEntry;
        }

    }
}
