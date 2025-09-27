using System.Text.Json;
using System.Text.Json.Serialization;
using BudgetManager.Components.Layout;

public class BudgetMonth
{
     private static Dictionary<(int year, int month), BudgetMonth> BudgetMonths = new();
     public int Year { get; private set; }
     public int Month { get; private set; }
     public float Income { get; private set; }
     public Dictionary<string, float> CategoryBudgets { get; private set; } = new();
     public List<Movement> Movements { get; private set; } = new();
     [JsonIgnore]
     public Dictionary<string, float> RemainingMoney { get; private set; } = new();
     [JsonIgnore]
     public Dictionary<string, float> SpendedMoney { get; private set; } = new();
     public BudgetMonth(int year, int month)
     {
          //Only Modifies present and future months
          if (year >= DateTime.Now.Year && month >= DateTime.Now.Month)
          {
               //Subscribes to the Category events
               Category.CategoryCreated += OnCategoryCreated;
               Category.CategoryRenamed += OnCategoryRenamed;
               Category.CategoryDeleted += OnCategoryDeleted;
          }
          //Checks if the month is valid
          if (month < 1 || month > 12)
          {
               throw new Exception("Month not valid");
          }
          Year = year;
          Month = month;
          //Adds all the current categories
          foreach (KeyValuePair<string, Category> categoryKvp in Category.Categories)
          {
               CategoryBudgets.Add(categoryKvp.Value.Name, 0);
          }
          //Adds the BudgetMonth to the static dictionary
          BudgetMonths.Add((Year, Month), this);
          CalculateAnalytics();
     }
     [JsonConstructor]
     public BudgetMonth(int year, int month, float income, Dictionary<string, float> categoryBudgets, List<Movement> movements)
     {
          //Only Modifies present and future months
          if (year >= DateTime.Now.Year && month >= DateTime.Now.Month)
          {
               //Subscribes to the Category events
               Category.CategoryCreated += OnCategoryCreated;
               Category.CategoryRenamed += OnCategoryRenamed;
               Category.CategoryDeleted += OnCategoryDeleted;
          }
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
          BudgetMonths.Add((Year, Month), this);
          CalculateAnalytics();

     }
     private void CalculateAnalytics()
     {
          //Updates the remaining money dictionary
          foreach (KeyValuePair<string, float> catBudgetKvp in CategoryBudgets)
          {
               if (RemainingMoney.ContainsKey(catBudgetKvp.Key))
               {
                    RemainingMoney[catBudgetKvp.Key] = catBudgetKvp.Value * Income / 100;
               }
               else
               {
                    RemainingMoney.Add(catBudgetKvp.Key, catBudgetKvp.Value * Income / 100);
               }
          }
          foreach (Movement movement in Movements)
          {
               string categoryName = movement.Rubro.Concept.Category.Name;
               if (RemainingMoney.ContainsKey(categoryName))
               {
                    RemainingMoney[categoryName] -= movement.Amount;
               }
          }
          //Update the SpendedMoney Dictionary
          foreach (KeyValuePair<string, float> remainingMoneyKvp in RemainingMoney)
          {
               if (SpendedMoney.ContainsKey(remainingMoneyKvp.Key))
               {
                    SpendedMoney[remainingMoneyKvp.Key] = (CategoryBudgets[remainingMoneyKvp.Key] * Income / 100) - remainingMoneyKvp.Value;
               }
               else
               {
                    SpendedMoney.Add(remainingMoneyKvp.Key, (CategoryBudgets[remainingMoneyKvp.Key] * Income / 100) - remainingMoneyKvp.Value);
               }
          }
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
               BudgetMonth? budgetMonth = JsonSerializer.Deserialize<BudgetMonth>(jsonString);
               if (budgetMonth is not null)
               {
                    return budgetMonth;
               }
               else
               {
                    MainLayout.DisplayInformationWindow("Error: ", $"Fail to deserialize the BudgetMonth:({year},{month})", IsErrorMessage: true);
               }
          }
          //Create a new month
          return new BudgetMonth(year, month);
     }
     public void ModifyIncome(float newIncomeValue)
     {
          Income = newIncomeValue;
          SaveToDatabase();
          CalculateAnalytics();
     }
     public void ModifyBudget(string categoryName, float newValue)
     {
          CategoryBudgets[categoryName] = newValue;
          SaveToDatabase();
          CalculateAnalytics();
     }
     public void AddMovement(Movement movement)
     {
          Movements.Add(movement);
          SaveToDatabase();
          CalculateAnalytics();
     }
     public void RemoveMovement(Movement movement)
     {
          Movements.Remove(movement);
          SaveToDatabase();
          CalculateAnalytics();
     }
     private void OnCategoryCreated(Category createdCategory)
     {
          CategoryBudgets.Add(createdCategory.Name, 0);
          SaveToDatabase();
          CalculateAnalytics();
     }
     private void OnCategoryRenamed(Category category, string previousName)
     {
          float currentBudget = CategoryBudgets[previousName];
          CategoryBudgets.Remove(previousName);
          CategoryBudgets.Add(category.Name, currentBudget);
          SaveToDatabase();
          RemainingMoney.Remove(previousName);
          CalculateAnalytics();
     }
     private void OnCategoryDeleted(Category category)
     {
          CategoryBudgets.Remove(category.Name);
          SaveToDatabase();
          RemainingMoney.Remove(category.Name);
          CalculateAnalytics();
     }

     private void SaveToDatabase()
     {
          DatabaseManager.UploadToDatabase($"BudgetMonth:({Year},{Month})", JsonSerializer.Serialize(this));
     }
}