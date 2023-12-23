/// <summary>
/// Clase movimiento encargada de definir el comportamiento de los movimientos
/// </summary>
public class Movement{
        public static List<Movement> movements = new();
        /// <summary>
        /// Cantidad del movimiento
        /// </summary>
        /// <value></value>
        public int Amount {get; set;}
        public Rubro rubro;

        /// <summary>
        /// Crea un nuevo movimiento
        /// </summary>
        /// <param name="amount">El valor del movimiento</param>
        /// <param name="rubro">Descripción del movimiento</param>
        
        public Movement(int amount, Rubro rubro){
            Amount = amount;
            this.rubro = rubro;
            movements.Add(this);
        }
}

