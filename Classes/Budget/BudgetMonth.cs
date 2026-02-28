using System.Text.Json;
using System.Text.Json.Serialization;
using BudgetManager.Components.Layout;

public class BudgetMonth
{
     private static Dictionary<(int year, int month), BudgetMonth> BudgetMonths = new();
     public int Year { get; private set; }
     public int Month { get; private set; }
     public float Income { get; private set; }
     public Dictionary<Guid, float> CategoryBudgets { get; private set; } = new();
     public List<Movement> Movements { get; private set; } = new();
     [JsonIgnore]
     public Dictionary<Guid, float> RemainingMoney { get; private set; } = new();
     [JsonIgnore]
     public Dictionary<Guid, float> SpendedMoney { get; private set; } = new();
     public BudgetMonth(int year, int month)
     {
          //Only Modifies present and future months
          if (year >= DateTime.Now.Year && month >= DateTime.Now.Month)
          {
               Category.CategoryCreated += OnCategoryCreated;
               Category.CategoryDeleted += OnCategoryDeleted;
          }
          if (month < 1 || month > 12)
          {
               throw new Exception("Month not valid");
          }
          Year = year;
          Month = month;
          //Adds all active categories
          foreach (Category category in Category.CategoriesById.Values.Where(c => !c.IsDeleted))
          {
               CategoryBudgets.Add(category.Id, 0);
          }
          //Adds the BudgetMonth to the static dictionary
          BudgetMonths.Add((Year, Month), this);
          CalculateAnalytics();
     }
     [JsonConstructor]
     public BudgetMonth(int year, int month, float income, Dictionary<Guid, float> categoryBudgets, List<Movement> movements)
     {
          //Only Modifies present and future months
          if (year >= DateTime.Now.Year && month >= DateTime.Now.Month)
          {
               Category.CategoryCreated += OnCategoryCreated;
               Category.CategoryDeleted += OnCategoryDeleted;
          }
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
          foreach (KeyValuePair<Guid, float> catBudgetKvp in CategoryBudgets)
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
               if (movement.Rubro is null) continue;
               Guid categoryId = movement.Rubro.Concept.Category.Id;
               if (RemainingMoney.ContainsKey(categoryId))
               {
                    RemainingMoney[categoryId] -= movement.Amount;
               }
          }
          //Update the SpendedMoney Dictionary
          foreach (KeyValuePair<Guid, float> remainingMoneyKvp in RemainingMoney)
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
     public void ModifyBudget(Guid categoryId, float newValue)
     {
          CategoryBudgets[categoryId] = newValue;
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
          CategoryBudgets.Add(createdCategory.Id, 0);
          SaveToDatabase();
          CalculateAnalytics();
     }
     private void OnCategoryDeleted(Category category)
     {
          CategoryBudgets.Remove(category.Id);
          SaveToDatabase();
          RemainingMoney.Remove(category.Id);
          CalculateAnalytics();
     }

     private void SaveToDatabase()
     {
          DatabaseManager.UploadToDatabase($"BudgetMonth:({Year},{Month})", JsonSerializer.Serialize(this));
     }
}