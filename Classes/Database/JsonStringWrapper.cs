/// <summary>
/// Wrapper for the serialized string to be able to sent it to the cosmos Database
/// </summary>
public class JsonStringWrapper(string id)
{
     //The propperty name must be "id" to be compatrible with cosmoDB
     public string id { get; set; } = id;
     public string JsonString { get; set; } = CategoryTreeSerializer.Serialize();
}