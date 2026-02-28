using System.Text.Json;
using System.Text.Json.Serialization;
using BudgetManager.Components.Layout;
/// <summary>
/// Handels the serialization and deserialization of the current Categories tree
/// </summary>
public static class CategoryTreeSerializer
{
     public static bool IsAlreadyInitialized { get; private set; } = false;
     /// <summary>
     /// The options for the serialization and deserialization
     /// </summary>
     private static JsonSerializerOptions options = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles };
     /// <summary>
     /// Serializes the current category-concepts-rubros tree
     /// </summary>
     /// <returns>the JSON serialized string</returns>
     public static string Serialize()
     {
          string serializedString = JsonSerializer.Serialize(Category.CategoriesById, options);
          return serializedString;
     }
     /// <summary>
     /// Takes a json string and creates all the category-concepts-rubros tree 
     /// </summary>
     /// <param name="jsonString">The JSON string containing the information</param>
     public static void Deserialize(string jsonString)
     {
          //Gets a dictionary of all the categories
          Dictionary<string, JsonElement>? categories = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString, options);
          //If deserialization was not successfull
          if (categories is null)
          {
               MainLayout.DisplayInformationWindow("Error", " Category Tree: categories. Deserialization failed", IsErrorMessage: true);
               return;
          }
          //Create a category for each category in the dictionary
          foreach (KeyValuePair<string, JsonElement> categorieKvp in categories)
          {
               Guid categoryId = Guid.Parse(categorieKvp.Key);
               if (Category.CategoriesById.ContainsKey(categoryId)) continue;
               string categoryName = categorieKvp.Value.GetProperty("Name").GetString()!;
               bool categoryIsDeleted = categorieKvp.Value.GetProperty("IsDeleted").GetBoolean();
               Category category = new(categoryName, categoryId, categoryIsDeleted);
               //Gets the concepts in the category as a dictionary (keyed by name)
               JsonElement conceptsJE = categorieKvp.Value.GetProperty("ConceptsById");
               Dictionary<string, JsonElement>? concepts = conceptsJE.Deserialize<Dictionary<string, JsonElement>>();
               if (concepts is null)
               {
                    MainLayout.DisplayInformationWindow("Error", " Category Tree: concepts.Deserialization failed", IsErrorMessage: true);
                    return;
               }
               //Creates the concept for each concept in the dictionary
               foreach (KeyValuePair<string, JsonElement> conceptKvp in concepts)
               {
                    Guid conceptId = Guid.Parse(conceptKvp.Key);
                    string conceptName = conceptKvp.Value.GetProperty("Name").GetString()!;
                    if (Concept.ConceptsById.ContainsKey(conceptId)) continue;
                    bool conceptIsDeleted = conceptKvp.Value.GetProperty("IsDeleted").GetBoolean();
                    Concept concept = new(conceptName, category, conceptId, conceptIsDeleted);
                    //Gets the Rubros as a dictionary (keyed by name)
                    JsonElement rubrosJE = conceptKvp.Value.GetProperty("RubrosById");
                    Dictionary<string, JsonElement>? rubros = rubrosJE.Deserialize<Dictionary<string, JsonElement>>();
                    if (rubros is null)
                    {
                         MainLayout.DisplayInformationWindow("Error", " Category Tree: rubros. Deserialization failed", IsErrorMessage: true);
                         return;
                    }
                    //Creates the rubro for each rubro in the dictionary
                    foreach (KeyValuePair<string, JsonElement> rubroKvp in rubros)
                    {
                         Guid rubroId = Guid.Parse(rubroKvp.Key);
                         string rubroName = rubroKvp.Value.GetProperty("Name").GetString()!;
                         if (Rubro.RubrosById.ContainsKey(rubroId)) continue;
                         bool rubroIsDeleted = rubroKvp.Value.GetProperty("IsDeleted").GetBoolean();
                         new Rubro(rubroName, concept, rubroId, rubroIsDeleted);
                    }
               }
          }
          //Flag the Category Tree as already Initialized
          IsAlreadyInitialized = true;
     }
}