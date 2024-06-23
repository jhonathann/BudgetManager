using System.Diagnostics;
using BudgetManager;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

public static class DatabaseManager
{
     //Delegate definition for a LoadCategoryTreeDataAction
     public delegate Task LoadCategoryTreeDataAction();
     public static LoadCategoryTreeDataAction LoadCategoryTreeData { get; private set; }
     public static Action CategoryTreeDataChanged { get; private set; }
     private static CosmosClient client;
     private static Database database;
     private static Container container;
     static DatabaseManager()
     {

          client = new CosmosClient(MauiProgram.Services.GetService<IConfiguration>().GetValue<string>("Database:Connection_String"));
          database = client.GetDatabase("BudgetManager");
          container = database.GetContainer("Main");
          CategoryTreeDataChanged = OnCategoryTreeChanged;
          LoadCategoryTreeData = OnLoadTreeData;
     }
     private static async void OnCategoryTreeChanged()
     {
          await UploadCategoryTreeData();
     }

     private static async Task UploadCategoryTreeData()
     {
          JsonStringWrapper jsonStringWrapper = new("CategoryTree");
          ItemResponse<JsonStringWrapper> response;
          try
          {
               response = await container.CreateItemAsync(jsonStringWrapper);
          }
          catch (CosmosException e)
          {
               Debug.Print(e.StatusCode.ToString());
               response = await container.ReplaceItemAsync(jsonStringWrapper, "CategoryTree");
          }
          DisplayOperationResult(response);
     }
     private static async Task OnLoadTreeData()
     {
          ItemResponse<JsonStringWrapper> response;
          try
          {
               response = await container.ReadItemAsync<JsonStringWrapper>("CategoryTree", PartitionKey.None);
               CategoryTreeSerializer.Deserialize(response.Resource.JsonString);
               DisplayOperationResult(response);
          }
          catch { }
     }
     [Conditional("DEBUG")]
     private static void DisplayOperationResult(ItemResponse<JsonStringWrapper> itemResponse)
     {
          Debug.Print("Status Code: " + itemResponse.StatusCode.ToString());
          Debug.Print("Request Charge: " + itemResponse.RequestCharge.ToString());
          Debug.Print("JsonString: " + itemResponse.Resource.JsonString);
     }

}