using BudgetManager.Components.Layout;
public class Concept
{
    public static Dictionary<int, Concept> Concepts { get; private set; } = new();
    public static int IdCount { get; private set; } = 0;
    public int Id { get; private set; }
    public string Name { get; private set; } = "";
    public Category Category { get; private set; }
    public Dictionary<string, Rubro> Rubros { get; private set; } = new();

    public Concept(string name, Category category)
    {
        //First check if the name is already in the dictionary
        if (category.Concepts.ContainsKey(name))
        {
            MainLayout.DisplayInformation("Error", "Ya existe un concepto con este nombre");
            return;
        }
        Name = name;
        Category = category;
        IdCount++;
        Id = IdCount;
        Concepts.Add(Id, this);
        Category.Concepts.Add(Name, this);
        DatabaseManager.CategoryTreeDataChanged.Invoke();
    }

    //Constructor for deserialization
    public Concept(string name, Category category, int id)
    {
        Name = name;
        Category = category;
        Id = id;
        //If im adding a higher id, the idcount will start higher
        if (Id > IdCount)
        {
            IdCount = Id;
        }
        Concepts.Add(Id, this);
        Category.Concepts.Add(Name, this);
    }
}