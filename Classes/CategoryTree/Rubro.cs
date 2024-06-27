using BudgetManager.Components.Layout;
using System.Text.Json.Serialization;
public class Rubro
{
    public static Dictionary<Guid, Rubro> Rubros { get; private set; } = new();
    public Guid Id { get; private set; }
    public string Name { get; private set; } = "";
    [JsonIgnore]
    public Concept Concept { get; private set; }
    public Rubro(string name, Concept concept, Guid rubroId = default)
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
        if (rubroId == default)
        {
            Id = Guid.NewGuid();
            Rubros.Add(Id, this);
            DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
        }
        else
        {
            Id = rubroId;
            Rubros.Add(rubroId, this);
        }
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
        DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
    }
    public void Delete()
    {
        Concept.Rubros.Remove(Name);
        Rubros.Remove(Id);
        DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
    }
}

