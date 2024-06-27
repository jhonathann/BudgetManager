using System.Text.Json.Serialization;

/// <summary>
/// Clase movimiento encargada de definir el comportamiento de los movimientos
/// </summary>
public class Movement
{
    public int Amount { get; private set; }
    public Guid RubroId { get; private set; }
    public DateTime CreationDate { get; private set; }
    [JsonIgnore]
    public Rubro Rubro { get; private set; }
    /// <summary>
    /// Crea un nuevo movimiento
    /// </summary>
    /// <param name="amount">El valor del movimiento</param>
    /// <param name="rubro">Descripción del movimiento</param>

    public Movement(int amount, Guid rubroId)
    {
        Amount = amount;
        RubroId = rubroId;
        CreationDate = DateTime.Now;
        Rubro = Rubro.Rubros[rubroId];
    }
}

