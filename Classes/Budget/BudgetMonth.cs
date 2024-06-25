public class BudgetMonth
{
     public int Month { get; private set; }
     public Dictionary<string, float> CategoryBudgets { get; private set; } = new();
     public BudgetMonth(int month)
     {
          //Subscribes to the CategoryCreated event to a new category when this one is added
          Category.CategoryCreated += OnCategoryCreated;
          Category.CategoryRenamed += OnCategoryRenamed;
          Category.CategoryDeleted += OnCategoryDeleted;
          //Checks if the month is valid
          if (month < 1 || month > 12)
          {
               throw new Exception("Month not valid");
          }
          this.Month = month;

          //Adds all the current categories
          foreach (KeyValuePair<string, Category> category in Category.Categories)
          {
               CategoryBudgets.Add(category.Value.Name, 0);
          }
     }

     private void OnCategoryCreated(Category createdCategory)
     {
          CategoryBudgets.Add(createdCategory.Name, 0);
     }
     private void OnCategoryRenamed(Category category, string previousName)
     {
          float currentBudget = CategoryBudgets[previousName];
          CategoryBudgets.Remove(previousName);
          CategoryBudgets.Add(category.Name, currentBudget);
     }
    private void OnCategoryDeleted(Category category)
    {
        CategoryBudgets.Remove(category.Name);
    }
     public void ChangeBudget(Category category, float newValue)
     {
          CategoryBudgets[category.Name] = newValue;
     }
}