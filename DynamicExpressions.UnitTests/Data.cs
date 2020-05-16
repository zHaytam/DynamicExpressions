namespace DynamicExpressions.UnitTests
{
    internal class Entry
    {
        public Entry(int id, SubEntry subEntry = null)
        {
            Id = id;
            SubEntry = subEntry;
        }

        public int Id { get; }
        public SubEntry SubEntry { get; }
    }

    internal class SubEntry
    {
        public SubEntry(string title)
        {
            Title = title;
        }

        public string Title { get; }
    }
}
