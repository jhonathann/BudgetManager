/// <summary>
/// Clase movimiento encargada de definir el comportamiento de los movimientos
/// </summary>
public class Movement
{
    public static List<Movement> Movements { get; private set; } = new();
    public int Amount { get; private set; }
    public Rubro Rubro { get; private set; }
    public DateTime CreationDate { get; private set; }


    /// <summary>
    /// Crea un nuevo movimiento
    /// </summary>
    /// <param name="amount">El valor del movimiento</param>
    /// <param name="rubro">Descripción del movimiento</param>

    public Movement(int amount, Rubro rubro)
    {
        Amount = amount;
        Rubro = rubro;
        CreationDate = DateTime.Now;
        Movements.Add(this);
    }
}

