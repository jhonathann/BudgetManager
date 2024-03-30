public class Concept
{
    public static Dictionary<int, Concept> Concepts { get; private set; } = new();
    public static int IdCount { get; private set; } = 0;
    public int Id { get; private set; }
    public string Name { get; private set; }
    public Category Category { get; private set; }

    public Concept(string name, Category category)
    {
        Name = name;
        this.Category = category;
        IdCount++;
        Id = IdCount;
        Concepts.Add(Id, this);
    }
}