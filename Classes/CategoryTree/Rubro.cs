using BudgetManager.Components.Layout;
using System.Text.Json.Serialization;
public class Rubro
{
    public static Dictionary<Guid, Rubro> RubrosById { get; private set; } = new();
    public Guid Id { get; private set; }
    public string Name { get; private set; } = "";
    public bool IsDeleted { get; private set; } = false;
    [JsonIgnore]
    public Concept Concept { get; private set; } = default!;
    public Rubro(string name, Concept concept, Guid rubroId = default, bool isDeleted = false)
    {
        if (concept is null)
        {
            MainLayout.DisplayInformationWindow("Error", "No se ha asignado un concepto valido", IsErrorMessage: true);
            return;
        }
        if (rubroId == default)
        {
            // New rubro
            if (concept.RubrosByName.ContainsKey(name))
            {
                MainLayout.DisplayInformationWindow("Error", "Ya existe un rubro con este nombre", IsErrorMessage: true);
                return;
            }
            Id = Guid.CreateVersion7();
            IsDeleted = false;
            Name = name;
            Concept = concept;
            Concept.RubrosByName.Add(Name, this);
            Concept.RubrosById.Add(Id, this);
            RubrosById.Add(Id, this);
            DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
        }
        else if (!isDeleted)
        {
            // Loading active rubro from DB
            if (concept.RubrosByName.ContainsKey(name))
            {
                MainLayout.DisplayInformationWindow("Error", "Ya existe un rubro con este nombre", IsErrorMessage: true);
                return;
            }
            Id = rubroId;
            IsDeleted = false;
            Name = name;
            Concept = concept;
            Concept.RubrosByName.Add(Name, this);
            Concept.RubrosById.Add(rubroId, this);
            RubrosById.Add(rubroId, this);
        }
        else
        {
            // Loading soft-deleted rubro from DB — in RubrosById and parent RubrosById for serialization, not in RubrosByName
            Id = rubroId;
            IsDeleted = true;
            Name = name;
            Concept = concept;
            Concept.RubrosById.Add(rubroId, this);
            RubrosById.Add(rubroId, this);
        }
    }
    public void Rename(string newName)
    {
        if (Concept.RubrosByName.ContainsKey(newName))
        {
            MainLayout.DisplayInformationWindow("Error", "Ya existe un rubro con este nombre", IsErrorMessage: true);
            return;
        }
        Concept.RubrosByName.Remove(Name);
        Name = newName;
        Concept.RubrosByName.Add(Name, this);
        DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
    }
    public void SoftDelete(bool writeToDb = true)
    {
        IsDeleted = true;
        Concept.RubrosByName.Remove(Name); // free name for reuse; Guid stays in Concept.RubrosById
        // Intentionally kept in Rubro.RubrosById so past movements can always resolve their reference
        if (writeToDb) DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
    }
}
