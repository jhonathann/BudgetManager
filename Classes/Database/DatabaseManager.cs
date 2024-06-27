using System.Diagnostics;
using BudgetManager;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

public static class DatabaseManager
{
     public static Func<string, Task<string?>> LoadDatabaseData { get; private set; }
     public static Action<string, string> UploadToDatabase { get; private set; }
     private static CosmosClient client;
     private static Database database;
     private static Container container;
     static DatabaseManager()
     {

          client = new CosmosClient(MauiProgram.Services.GetService<IConfiguration>().GetValue<string>("Database:Connection_String"));
          database = client.GetDatabase("BudgetManager");
          container = database.GetContainer("Main");
          UploadToDatabase = OnUploadToDatabase;
          LoadDatabaseData = OnLoadDatabaseData;
     }
     private static async void OnUploadToDatabase(string databaseId, string jsonString)
     {
          JsonStringDatabaseWrapper jsonStringWrapper = new(databaseId, jsonString);
          ItemResponse<JsonStringDatabaseWrapper> response;
          try
          {
               response = await container.CreateItemAsync(jsonStringWrapper);
          }
          catch
          {
               response = await container.ReplaceItemAsync(jsonStringWrapper, databaseId);
          }
          DisplayOperationResult(response);
     }
     private static async Task<string?> OnLoadDatabaseData(string databaseId)
     {
          ItemResponse<JsonStringDatabaseWrapper> response;
          try
          {
               response = await container.ReadItemAsync<JsonStringDatabaseWrapper>(databaseId, PartitionKey.None);
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
          Debug.Print("JsonString: " + itemResponse.Resource.JsonString);
     }

}