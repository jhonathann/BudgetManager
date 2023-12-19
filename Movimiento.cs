/// <summary>
/// Clase movimiento encargada de definir el comportamiento de los movimientos
/// </summary>
public class Movimiento{
        /// <summary>
        /// Cantidad del movimiento
        /// </summary>
        /// <value></value>
        public int Amount {get; set;}
        
        public  string Descripcion{get; set;}
        public Categoria Tipo{get; set;}

        /// <summary>
        /// Crea un nuevo movimiento
        /// </summary>
        /// <param name="amount">El valor del movimiento</param>
        /// <param name="descripcion">Descripción del movimiento</param>
        /// <param name="tipo">Categoria del movimiento</param>
        public Movimiento(int amount, string descripcion, Categoria tipo){
            Amount = amount;
            Descripcion = descripcion;
            Tipo = tipo;
        }

        public void ShowInfo(){
                Console.WriteLine($"Cantidad: {Amount}, Descripción: {Descripcion}, Categoría: {Tipo}" );
        }
}

