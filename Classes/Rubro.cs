using BudgetManager.Components.Layout;
using System.Text.Json.Serialization;
public class Rubro
{
    public string Name { get; private set; } = "";
    [JsonIgnore]
    public Concept Concept { get; private set; }
    public Rubro(string name, Concept concept)
    {
        //First check if the name is already in the dictionary
        if (concept.Rubros.ContainsKey(name))
        {
            MainLayout.DisplayInformationWindow("Error", "Ya existe un rubro con este nombre");
            return;
        }
        Name = name;
        Concept = concept;
        Concept.Rubros.Add(Name, this);
        DatabaseManager.CategoryTreeDataChanged.Invoke();
    }
    public void Rename(string newName)
    {
        //First check if the name is already in the dictionary
        if (Concept.Rubros.ContainsKey(newName))
        {
            MainLayout.DisplayInformationWindow("Error", "Ya existe un concepto con este nombre");
            return;
        }
        Concept.Rubros.Remove(Name);
        Name = newName;
        Concept.Rubros.Add(Name, this);
        DatabaseManager.CategoryTreeDataChanged.Invoke();
    }
    public void Delete()
    {
        Concept.Rubros.Remove(Name);
        DatabaseManager.CategoryTreeDataChanged.Invoke();
    }
}

