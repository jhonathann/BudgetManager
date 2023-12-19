public class Movimiento{

        public int Amount {get; set;}
        
        public  string Descripcion{get; set;}
        public Categoria Tipo{get; set;}

        public Movimiento(int amount, string descripcion, Categoria tipo){
            Amount = amount;
            Descripcion = descripcion;
            Tipo = tipo;
        }

        public void ShowInfo(){
                Console.WriteLine($"Cantidad: {Amount}, Descripción: {Descripcion}, Categoría: {Tipo}" );
        }
}

