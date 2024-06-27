using System.Text.Json;
using System.Text.Json.Serialization;

public class BudgetMonth
{
     private static Dictionary<(int year, int month), BudgetMonth> BudgetMonths = new();
     public int Year { get; private set; }
     public int Month { get; private set; }
     public float Income { get; set; }
     public Dictionary<string, float> CategoryBudgets { get; private set; } = new();
     public List<Movement> Movements { get; private set; } = new();
     public BudgetMonth(int year, int month)
     {
          //Subscribes to the Category events 
          Category.CategoryCreated += OnCategoryCreated;
          Category.CategoryRenamed += OnCategoryRenamed;
          Category.CategoryDeleted += OnCategoryDeleted;
          //Checks if the month is valid
          if (month < 1 || month > 12)
          {
               throw new Exception("Month not valid");
          }
          Year = year;
          Month = month;
          float startingPercentage = Category.Categories.Count > 0 ? 100 / Category.Categories.Count : 0;
          //Adds all the current categories
          foreach (KeyValuePair<string, Category> categoryKvp in Category.Categories)
          {
               CategoryBudgets.Add(categoryKvp.Value.Name, 0);
          }
          //Adds the BudgetMonth to the static dictionary
          BudgetMonths.Add((Year, Month), this);
          DatabaseManager.UploadToDatabase($"BudgetMonth:({Year},{Month})", JsonSerializer.Serialize(this));
     }
     [JsonConstructor]
     public BudgetMonth(int year, int month, float income, Dictionary<string, float> categoryBudgets, List<Movement> movements)
     {
          //Subscribes to the Category events 
          Category.CategoryCreated += OnCategoryCreated;
          Category.CategoryRenamed += OnCategoryRenamed;
          Category.CategoryDeleted += OnCategoryDeleted;
          //Checks if the month is valid
          if (month < 1 || month > 12)
          {
               throw new Exception("Month not valid");
          }
          Year = year;
          Month = month;
          Income = income;
          CategoryBudgets = categoryBudgets;
          Movements = movements;
          //Adds the BudgetMonth to the static dictionary
          if (BudgetMonths.ContainsKey((Year, Month)))
          {
               BudgetMonths.Remove((Year, Month));
          }
          BudgetMonths.Add((Year, Month), this);
     }
     public static async Task<BudgetMonth> GetBudgetMonth(int year, int month)
     {
          //See if the month has already been downloaded and is in the dictionary
          if (BudgetMonths.ContainsKey((year, month)))
          {
               return BudgetMonths[(year, month)];
          }
          //Try to get the month from the database
          string? jsonString = await DatabaseManager.LoadDatabaseData.Invoke($"BudgetMonth:({year},{month})");
          if (jsonString is not null)
          {
               return JsonSerializer.Deserialize<BudgetMonth>(jsonString);
          }
          //Create a new month
          return new BudgetMonth(year, month);
     }
     public void ModifyBudget(string categoryName, float newValue)
     {
          CategoryBudgets[categoryName] = newValue;
          DatabaseManager.UploadToDatabase($"BudgetMonth:({Year},{Month})", JsonSerializer.Serialize(this));
     }
     public void AddMovement(Movement movement)
     {
          Movements.Add(movement);
          DatabaseManager.UploadToDatabase($"BudgetMonth:({Year},{Month})", JsonSerializer.Serialize(this));
     }
     private void OnCategoryCreated(Category createdCategory)
     {
          CategoryBudgets.Add(createdCategory.Name, 0);
          DatabaseManager.UploadToDatabase($"BudgetMonth:({Year},{Month})", JsonSerializer.Serialize(this));
     }
     private void OnCategoryRenamed(Category category, string previousName)
     {
          float currentBudget = CategoryBudgets[previousName];
          CategoryBudgets.Remove(previousName);
          CategoryBudgets.Add(category.Name, currentBudget);
          DatabaseManager.UploadToDatabase($"BudgetMonth:({Year},{Month})", JsonSerializer.Serialize(this));
     }
     private void OnCategoryDeleted(Category category)
     {
          CategoryBudgets.Remove(category.Name);
          DatabaseManager.UploadToDatabase($"BudgetMonth:({Year},{Month})", JsonSerializer.Serialize(this));
     }
}