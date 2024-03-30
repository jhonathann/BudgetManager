public class BudgetYear
{
     public int Year { get; private set; }
     public Dictionary<int, BudgetMonth> Months { get; private set; } = new();
     public BudgetYear(int year)
     {
          this.Year = year;
          for (int i = 1; i < 13; i++)
          {
               Months.Add(i, new BudgetMonth(i));
          }
     }
}