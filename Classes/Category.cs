
public class Category
{
    public static event Action<Category>? CategoryCreated;
    public static Dictionary<string, Category> Categories { get; private set; } = new();
    public string Name { get; private set; }
    public Category(string name)
    {
        Name = name;
        Categories.Add(Name, this);
        CategoryCreated?.Invoke(this);
    }
}