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
     // Carry over money from previous month (per category)
     public Dictionary<Guid, float> CarryOver { get; private set; } = new();
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
          CarryOver = new Dictionary<Guid, float>();
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
     public BudgetMonth(int year, int month, float income, Dictionary<Guid, float> categoryBudgets, List<Movement> movements, Dictionary<Guid, float> carryOver)
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
          // Null guard for documents saved before CarryOver was added
          CarryOver = carryOver ?? new Dictionary<Guid, float>();
          //Adds the BudgetMonth to the static dictionary
          BudgetMonths.Add((Year, Month), this);
          CalculateAnalytics();

     }
     private void CalculateAnalytics()
     {
          //Updates the remaining money dictionary (total available = monthly budget + carry over amount from previous month)
          foreach (KeyValuePair<Guid, float> catBudgetKvp in CategoryBudgets)
          {
               float monthlyBudget = catBudgetKvp.Value * Income / 100;
               float carryOverAmount = CarryOver.TryGetValue(catBudgetKvp.Key, out float co) ? co : 0f;
               float totalAvailable = monthlyBudget + carryOverAmount;
               if (RemainingMoney.ContainsKey(catBudgetKvp.Key))
                    RemainingMoney[catBudgetKvp.Key] = totalAvailable;
               else
                    RemainingMoney.Add(catBudgetKvp.Key, totalAvailable);
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
          //Update the SpendedMoney Dictionary (total spent = total available - remaining)
          foreach (KeyValuePair<Guid, float> remainingMoneyKvp in RemainingMoney)
          {
               float monthlyBudget = CategoryBudgets.TryGetValue(remainingMoneyKvp.Key, out float pct) ? pct * Income / 100 : 0f;
               float carryOverAmount = CarryOver.TryGetValue(remainingMoneyKvp.Key, out float co) ? co : 0f;
               float totalSpent = monthlyBudget + carryOverAmount - remainingMoneyKvp.Value;
               if (SpendedMoney.ContainsKey(remainingMoneyKvp.Key))
                    SpendedMoney[remainingMoneyKvp.Key] = totalSpent;
               else
                    SpendedMoney.Add(remainingMoneyKvp.Key, totalSpent);
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
          //Create a new month, inheriting carry-over from the previous month if it exists
          int previousMonth = month == 1 ? 12 : month - 1;
          int previousYear = month == 1 ? year - 1 : year;
          Dictionary<Guid, float> initialCarryOver = new();

          if (BudgetMonths.TryGetValue((previousYear, previousMonth), out BudgetMonth? previousMonthCached))
          {
               initialCarryOver = new Dictionary<Guid, float>(previousMonthCached.RemainingMoney);
          }
          else
          {
               string? previousMonthJson = await DatabaseManager.LoadDatabaseData.Invoke($"BudgetMonth:({previousYear},{previousMonth})");
               if (previousMonthJson is not null)
               {
                    BudgetMonth? previousMonthDeserialized = JsonSerializer.Deserialize<BudgetMonth>(previousMonthJson);
                    if (previousMonthDeserialized is not null)
                         initialCarryOver = new Dictionary<Guid, float>(previousMonthDeserialized.RemainingMoney);
               }
          }

          BudgetMonth newBudgetMonth = new(year, month);
          newBudgetMonth.ApplyCarryOver(initialCarryOver);
          newBudgetMonth.CalculateAnalytics();
          return newBudgetMonth;
     }
     public void ModifyIncome(float newIncomeValue)
     {
          Income = newIncomeValue;
          SaveToDatabase();
          CalculateAnalytics();
          PropagateCarryOverAsync();
     }
     public void ModifyBudget(Guid categoryId, float newValue)
     {
          CategoryBudgets[categoryId] = newValue;
          SaveToDatabase();
          CalculateAnalytics();
          PropagateCarryOverAsync();
     }
     public void AddMovement(Movement movement)
     {
          Movements.Add(movement);
          SaveToDatabase();
          CalculateAnalytics();
          PropagateCarryOverAsync();
     }
     public void RemoveMovement(Movement movement)
     {
          Movements.Remove(movement);
          SaveToDatabase();
          CalculateAnalytics();
          PropagateCarryOverAsync();
     }
     private void OnCategoryCreated(Category createdCategory)
     {
          CategoryBudgets.Add(createdCategory.Id, 0);
          SaveToDatabase();
          CalculateAnalytics();
          PropagateCarryOverAsync();
     }
     private void OnCategoryDeleted(Category category)
     {
          CategoryBudgets.Remove(category.Id);
          SaveToDatabase();
          RemainingMoney.Remove(category.Id);
          CalculateAnalytics();
          PropagateCarryOverAsync();
     }

     private async void PropagateCarryOverAsync()
     {
          int nextMonth = Month == 12 ? 1 : Month + 1;
          int nextYear = Month == 12 ? Year + 1 : Year;

          BudgetMonth? next = null;
          if (BudgetMonths.TryGetValue((nextYear, nextMonth), out BudgetMonth? cached))
          {
               next = cached;
          }
          else
          {
               // Do NOT use GetBudgetMonth() — that creates new months. Only propagate to months that already exist.
               string? json = await DatabaseManager.LoadDatabaseData.Invoke($"BudgetMonth:({nextYear},{nextMonth})");
               if (json is null) return;
               next = JsonSerializer.Deserialize<BudgetMonth>(json);
               if (next is null) return;
          }

          // Short-circuit: if carry-over hasn't changed, no downstream months are affected
          if (CarryOverUnchanged(next.CarryOver, RemainingMoney)) return;

          next.ApplyCarryOver(RemainingMoney);
          next.CalculateAnalytics();
          next.SaveToDatabase();
          next.PropagateCarryOverAsync();
     }
     private void ApplyCarryOver(Dictionary<Guid, float> newCarryOver)
     {
          CarryOver = new Dictionary<Guid, float>(newCarryOver);
     }
     private static bool CarryOverUnchanged(Dictionary<Guid, float> existing, Dictionary<Guid, float> incoming)
     {
          if (existing.Count != incoming.Count) return false;
          foreach (KeyValuePair<Guid, float> kvp in incoming)
          {
               if (!existing.TryGetValue(kvp.Key, out float existingValue)) return false;
               if (MathF.Abs(existingValue - kvp.Value) > 0.01f) return false;
          }
          return true;
     }
     private void SaveToDatabase()
     {
          DatabaseManager.UploadToDatabase($"BudgetMonth:({Year},{Month})", JsonSerializer.Serialize(this));
     }
}