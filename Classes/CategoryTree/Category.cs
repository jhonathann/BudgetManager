using BudgetManager.Components.Layout;

public class Category
{
    public static event Action<Category>? CategoryCreated;
    public static event Action<Category, string>? CategoryRenamed;
    public static event Action<Category>? CategoryDeleted;
    public static Dictionary<string, Category> Categories { get; private set; } = new();
    public string Name { get; private set; } = "";
    public Dictionary<string, Concept> Concepts { get; private set; } = new();
    public Category(string name, bool uploadData = true)
    {
        //First check if the name is already in the dictionary
        if (Categories.ContainsKey(name))
        {
            MainLayout.DisplayInformationWindow("Error", "Ya existe una categoría con este nombre", IsErrorMessage: true);
            return;
        }
        Name = name;
        Categories.Add(Name, this);
        CategoryCreated?.Invoke(this);
        //Uploads the data to the database
        if (uploadData) DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
    }
    public void Rename(string newName)
    {
        //First check if the name is already in the dictionary
        if (Categories.ContainsKey(newName))
        {
            MainLayout.DisplayInformationWindow("Error", "Ya existe una categoría con este nombre", IsErrorMessage: true);
            return;
        }
        string previousName = Name;
        Categories.Remove(previousName);
        Name = newName;
        Categories.Add(Name, this);
        CategoryRenamed?.Invoke(this, previousName);
        //Uploads the data to the database
        DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
    }
    public void Delete()
    {
        Categories.Remove(Name);
        CategoryDeleted?.Invoke(this);
        //Uploads the data to the database
        DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
    }
}