namespace IPC2_Proyecto1
{
    public class ListaEstado
    {
        public NodoEstado Cabeza;

        public ListaEstado()
        {
            Cabeza = null;
        }

        public void Insertar(ListaCelda estado, int periodo)
        {
            NodoEstado nuevo = new NodoEstado(estado, periodo);

            if (Cabeza == null)
            {
                Cabeza = nuevo;
            }
            else
            {
                NodoEstado actual = Cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
        }
    }
}