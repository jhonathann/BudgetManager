using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BudgetManager;
using BudgetManager.Components.Layout;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

public static class DatabaseManager
{
     public static Func<string, Task<string?>> LoadDatabaseData { get; private set; } = default!;
     public static Action<string, string> UploadToDatabase { get; private set; } = default!;
     private static CosmosClient Client { get; set; } = default!;
     private static Database Database { get; set; } = default!;
     private static Container Container { get; set; } = default!;
     static DatabaseManager()
     {
          IConfiguration? config = MauiProgram.Services.GetService<IConfiguration>();
          if (config is null)
          {
               MainLayout.DisplayInformationWindow("Error", "No se pudo acceder a la configuración", IsErrorMessage: true);
               return;
          }
          Client = new CosmosClient(config.GetValue<string>("Database:Connection_String"));
          Database = Client.GetDatabase("BudgetManager");
          Container = Database.GetContainer("Main");
          UploadToDatabase = OnUploadToDatabase;
          LoadDatabaseData = OnLoadDatabaseData;
     }
     private static async void OnUploadToDatabase(string databaseId, string jsonString)
     {
          JsonStringDatabaseWrapper jsonStringWrapper = new(databaseId, jsonString);
          ItemResponse<JsonStringDatabaseWrapper> response;
          try
          {
               response = await Container.CreateItemAsync(jsonStringWrapper);
          }
          catch
          {
               response = await Container.ReplaceItemAsync(jsonStringWrapper, databaseId);
          }
          DisplayOperationResult(response);
     }
     /// <summary>
     /// Tries to get data from the database
     /// </summary>
     /// <param name="databaseId">The id of the database</param>
     /// <returns>A json string if the retrieval was succesfull or null otherwise</returns>
     private static async Task<string?> OnLoadDatabaseData(string databaseId)
     {
          ItemResponse<JsonStringDatabaseWrapper> response;
          try
          {
               response = await Container.ReadItemAsync<JsonStringDatabaseWrapper>(databaseId, PartitionKey.None);
               DisplayOperationResult(response);
               return response.Resource.JsonString;
          }
          catch { return null; }
     }
     [Conditional("DEBUG")]
     private static void DisplayOperationResult(ItemResponse<JsonStringDatabaseWrapper> itemResponse)
     {
          Debug.Print("Status Code: " + itemResponse.StatusCode.ToString());
          Debug.Print("Request Charge: " + itemResponse.RequestCharge.ToString());
          Debug.Print("JsonString: " + JToken.Parse(itemResponse.Resource.JsonString).ToString(Formatting.Indented));
     }

}