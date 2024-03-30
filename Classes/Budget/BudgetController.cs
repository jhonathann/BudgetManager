public static class BudgetController
{
     public static Dictionary<int, BudgetYear> Years { get; private set; } = new();
     public static Dictionary<string, float> GetBudgetInfo(int year, int month)
     {
          if (!Years.ContainsKey(year))
          {
               Years.Add(year, new BudgetYear(year));
          }
          return Years[year].Months[month].CategoryBudgets;
     }
     public static void ModifyBudget(int year, int month, string name, float newValue)
     {
          Years[year].Months[month].ChangeBudget(Category.Categories[name], newValue);
     }
}