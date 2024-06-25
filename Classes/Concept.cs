using BudgetManager.Components.Layout;
using System.Text.Json.Serialization;
public class Concept
{
    public string Name { get; private set; } = "";
    [JsonIgnore]
    public Category Category { get; private set; }
    public Dictionary<string, Rubro> Rubros { get; private set; } = new();

    public Concept(string name, Category category)
    {
        //First check if the name is already in the dictionary
        if (category.Concepts.ContainsKey(name))
        {
            MainLayout.DisplayInformationWindow("Error", "Ya existe un concepto con este nombre");
            return;
        }
        Name = name;
        Category = category;
        Category.Concepts.Add(Name, this);
        DatabaseManager.CategoryTreeDataChanged.Invoke();
    }
    public void Rename(string newName)
    {
        //First check if the name is already in the dictionary
        if (Category.Concepts.ContainsKey(newName))
        {
            MainLayout.DisplayInformationWindow("Error", "Ya existe un concepto con este nombre");
            return;
        }
        Category.Concepts.Remove(Name);
        Name = newName;
        Category.Concepts.Add(Name, this);
        DatabaseManager.CategoryTreeDataChanged.Invoke();
    }
    public void Delete()
    {
        Category.Concepts.Remove(Name);
        DatabaseManager.CategoryTreeDataChanged.Invoke();
    }
}