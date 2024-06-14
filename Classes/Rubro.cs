using BudgetManager.Components.Layout;
public class Rubro
{
    public static Dictionary<int, Rubro> Rubros { get; private set; } = new();
    public static int IdCount { get; private set; } = 0;
    public int Id { get; private set; }
    public string Name { get; private set; } = "";

    public Concept Concept { get; private set; }

    public Rubro(string name, Concept concept)
    {
        //First check if the name is already in the dictionary
        if (concept.Rubros.ContainsKey(name))
        {
            MainLayout.DisplayInformation("Error", "Ya existe un rubro con este nombre");
            return;
        }
        Name = name;
        Concept = concept;
        IdCount++;
        Id = IdCount;
        Rubros.Add(Id, this);
        Concept.Rubros.Add(Name, this);
    }
}

