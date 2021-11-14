namespace DynamicExpressions.UnitTests
{
    internal class Entry<T>
    {
        public Entry(int id, SubEntry<T> subEntry = null)
        {
            Id = id;
            SubEntry = subEntry;
        }

        public int Id { get; }
        public SubEntry<T> SubEntry { get; }
    }

    internal class SubEntry<T>
    {
        public SubEntry(T title)
        {
            Title = title;
        }

        public T Title { get; }
    }
}
