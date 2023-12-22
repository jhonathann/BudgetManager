public static class ManagerDeMovimentos
{
    private static List<Movimiento> movimientos = new List<Movimiento>();
    public static void AddMovement(Movimiento movimiento)
    {
        movimientos.Add(movimiento);
    }
    public static void MostrarMovimientos()
    {
        foreach (Movimiento movimiento in movimientos)
        {
            movimiento.ShowInfo();
        }
    }
    public static IEnumerable<Movimiento> ObtenerMOvimientos()
    {
        return movimientos;
    }
}