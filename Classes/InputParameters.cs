namespace BudgetManager.Classes;

public struct InputParameters(string value, DateTime creationDate)
{
    public string Value { get; private set; } = value;
    public DateTime CreationDate { get; private set; } = creationDate;
}
