using System.Text.Json;
using System.Text.Json.Serialization;
/// <summary>
/// Handels the serialization and deserialization of the current Categories tree
/// </summary>
public static class CategoryTreeSerializer
{
     /// <summary>
     /// The oprions for the serialization and deserialization
     /// </summary>
     private static JsonSerializerOptions options = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles };
     /// <summary>
     /// Serializes the current category-concepts-rubros tree
     /// </summary>
     /// <returns>the JSON serialized string</returns>
     public static string Serialize()
     {
          string serializedString = JsonSerializer.Serialize(Category.Categories, options);
          return serializedString;
     }
     /// <summary>
     /// Takes a json string and creates all the category-concepts-rubros tree 
     /// </summary>
     /// <param name="jsonString">The JSON string containing the information</param>
     public static void Deserialize(string jsonString)
     {
          //Gets a dictionary of all the categories
          Dictionary<string, JsonElement> categories = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString, options);
          //Create a category for each category in the dictionary
          foreach (KeyValuePair<string, JsonElement> categorieKvp in categories)
          {
               //Checks if the category has already been added
               if (Category.Categories.ContainsKey(categorieKvp.Key)) continue;
               //Creates the category but doesnt update the data
               Category category = new(categorieKvp.Key, uploadData: false);
               //Gets the concepts in the category as a dictionary
               JsonElement conceptsJE = categorieKvp.Value.GetProperty("Concepts");
               Dictionary<string, JsonElement> concepts = conceptsJE.Deserialize<Dictionary<string, JsonElement>>();
               //Creates the concept for each concept in the dictionary
               foreach (KeyValuePair<string, JsonElement> conceptKvp in concepts)
               {
                    //Checks if the concept has already been added
                    if (category.Concepts.ContainsKey(conceptKvp.Key)) continue;
                    //Creates the concept
                    Concept concept = new(conceptKvp.Key, category);
                    //Gets the Rubros as a dictionary
                    JsonElement rubrosJE = conceptKvp.Value.GetProperty("Rubros");
                    Dictionary<string, JsonElement> rubros = rubrosJE.Deserialize<Dictionary<string, JsonElement>>();
                    //Creates the rubro for each rubro in the dictionary
                    foreach (KeyValuePair<string, JsonElement> rubroKvp in rubros)
                    {
                         //Checks if the rubro has already been added
                         if (concept.Rubros.ContainsKey(rubroKvp.Key)) continue;
                         //Creates the rubro
                         new Rubro(rubroKvp.Key, concept);
                    }
               }
          }
     }
}