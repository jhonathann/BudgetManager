using BudgetManager.Components.Layout;
using System.Text.Json.Serialization;
public class Concept
{
    public static Dictionary<Guid, Concept> ConceptsById { get; private set; } = new();
    public Guid Id { get; private set; }
    public string Name { get; private set; } = "";
    public bool IsDeleted { get; private set; } = false;
    [JsonIgnore]
    public Category Category { get; private set; } = default!;
    // Used exclusively for O(1) rubro name uniqueness checks within this concept
    [JsonIgnore]
    public Dictionary<string, Rubro> RubrosByName { get; private set; } = new();
    public Dictionary<Guid, Rubro> RubrosById { get; private set; } = new();

    public Concept(string name, Category category, Guid conceptId = default, bool isDeleted = false, bool uploadData = true)
    {
        if (category is null)
        {
            MainLayout.DisplayInformationWindow("Error", "No se ha asignado una categoria valida", IsErrorMessage: true);
            return;
        }
        if (conceptId == default)
        {
            // New concept
            if (category.ConceptsByName.ContainsKey(name))
            {
                MainLayout.DisplayInformationWindow("Error", "Ya existe un concepto con este nombre", IsErrorMessage: true);
                return;
            }
            Id = Guid.CreateVersion7();
            IsDeleted = false;
            Name = name;
            Category = category;
            Category.ConceptsByName.Add(Name, this);
            Category.ConceptsById.Add(Id, this);
            ConceptsById.Add(Id, this);
            if (uploadData) DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
        }
        else if (!isDeleted)
        {
            // Loading active concept from DB
            if (category.ConceptsByName.ContainsKey(name))
            {
                MainLayout.DisplayInformationWindow("Error", "Ya existe un concepto con este nombre", IsErrorMessage: true);
                return;
            }
            Id = conceptId;
            IsDeleted = false;
            Name = name;
            Category = category;
            Category.ConceptsByName.Add(Name, this);
            Category.ConceptsById.Add(Id, this);
            ConceptsById.Add(Id, this);
        }
        else
        {
            // Loading soft-deleted concept from DB — in ConceptsById and parent ConceptsById for serialization, not in ConceptsByName
            Id = conceptId;
            IsDeleted = true;
            Name = name;
            Category = category;
            Category.ConceptsById.Add(Id, this);
            ConceptsById.Add(Id, this);
        }
    }
    public void Rename(string newName)
    {
        if (Category.ConceptsByName.ContainsKey(newName))
        {
            MainLayout.DisplayInformationWindow("Error", "Ya existe un concepto con este nombre", IsErrorMessage: true);
            return;
        }
        Category.ConceptsByName.Remove(Name);
        Name = newName;
        Category.ConceptsByName.Add(Name, this);
        DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
    }
    public void SoftDelete(bool writeToDb = true)
    {
        IsDeleted = true;
        Category.ConceptsByName.Remove(Name); // free name for reuse; Guid stays in Category.ConceptsById
        foreach (Rubro rubro in RubrosById.Values)
            rubro.SoftDelete(writeToDb: false);
        if (writeToDb) DatabaseManager.UploadToDatabase.Invoke("CategoryTree", CategoryTreeSerializer.Serialize());
    }
}
