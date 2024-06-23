using BudgetManager.Components.Layout;

public class Category
{
    public static event Action<Category>? CategoryCreated;
    public static Dictionary<string, Category> Categories { get; private set; } = new();
    public string Name { get; private set; } = "";
    public Dictionary<string, Concept> Concepts { get; private set; } = new();
    public Category(string name, bool uploadData = true)
    {
        //First check if the name is already in the dictionary
        if (Categories.ContainsKey(name))
        {
            MainLayout.DisplayInformation("Error", "Ya existe una categoría con este nombre");
            return;
        }
        Name = name;
        Categories.Add(Name, this);
        CategoryCreated?.Invoke(this);
        //Uploads the data to the database
        if (uploadData) DatabaseManager.CategoryTreeDataChanged.Invoke();
    }
}