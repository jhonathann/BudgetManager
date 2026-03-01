using BudgetManager.Components.Layout;
using System.Text.Json.Serialization;

public class Category
{
    public static event Action<Category>? CategoryCreated;
    public static event Action<Category>? CategoryRenamed;
    public static event Action<Category>? CategoryDeleted;
    // Used exclusively for O(1) name uniqueness checks
    public static Dictionary<string, Category> CategoriesByName { get; private set; } = new();
    public static Dictionary<Guid, Category> CategoriesById { get; private set; } = new();
    public Guid Id { get; private set; }
    public string Name { get; private set; } = "";
    public bool IsDeleted { get; private set; } = false;
    // Used exclusively for O(1) concept name uniqueness checks within this category
    [JsonIgnore]
    public Dictionary<string, Concept> ConceptsByName { get; private set; } = new();
    public Dictionary<Guid, Concept> ConceptsById { get; private set; } = new();
    public Category(string name, Guid categoryId = default, bool isDeleted = false, bool uploadData = true)
    {
        if (categoryId == default)
        {
            // New category
            if (CategoriesByName.ContainsKey(name))
            {
                MainLayout.DisplayInformationWindow("Error", "Ya existe una categoría con este nombre", IsErrorMessage: true);
                return;
            }
            Id = Guid.CreateVersion7();
            IsDeleted = false;
            Name = name;
            CategoriesByName.Add(Name, this);
            CategoriesById.Add(Id, this);
            CategoryCreated?.Invoke(this);
            if (uploadData) DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
            return;
        }
        if (!isDeleted)
        {
            // Loading active category from DB
            if (CategoriesByName.ContainsKey(name))
            {
                MainLayout.DisplayInformationWindow("Error", "Ya existe una categoría con este nombre", IsErrorMessage: true);
                return;
            }
            Id = categoryId;
            IsDeleted = false;
            Name = name;
            CategoriesByName.Add(Name, this);
            CategoriesById.Add(Id, this);
            return;
        }
        // Loading soft-deleted category from DB — only in CategoriesById for movement resolution
        Id = categoryId;
        IsDeleted = true;
        Name = name;
        CategoriesById.Add(Id, this);
    }
    public void Rename(string newName)
    {
        if (CategoriesByName.ContainsKey(newName))
        {
            MainLayout.DisplayInformationWindow("Error", "Ya existe una categoría con este nombre", IsErrorMessage: true);
            return;
        }
        string previousName = Name;
        CategoriesByName.Remove(previousName);
        Name = newName;
        CategoriesByName.Add(Name, this);
        CategoryRenamed?.Invoke(this);
        DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
    }
    public void SoftDelete()
    {
        IsDeleted = true;
        CategoriesByName.Remove(Name);
        foreach (Concept concept in ConceptsById.Values)
            concept.SoftDelete(writeToDb: false);
        CategoryDeleted?.Invoke(this);
        DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
    }
}