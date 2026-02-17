namespace IPC2_Proyecto1
{

    public class ListaCelda
    {
        private NodoCelda cabeza;

        public NodoCelda Cabeza
        {
            set
            {
                cabeza = value;
            }
            get
            {
                return cabeza;
            }
        }
        public ListaCelda()
        {
            Cabeza = null;
        }

        public void Insertar(int fila, int columna)
        {
            if (!Existe(fila, columna))
            {
                NodoCelda nuevo = new NodoCelda(fila, columna);
                if (Cabeza == null)
                {
                    Cabeza = nuevo;
                }
                else
                {

                    NodoCelda actual = Cabeza;
                    while (actual.Siguiente != null)
                    {
                        actual = actual.Siguiente;
                    }
                    actual.Siguiente = nuevo;
                }
            }

        }
        public bool Existe(int fila, int columnna)
        {
            NodoCelda actual = cabeza;

            while (actual != null)
            {
                if (actual.Fila == fila && actual.Columna == columnna)
                {
                    return true;
                }
                actual = actual.Siguiente;
            }
            return false;
        }
        public void Limpiar()
        {
            cabeza = null;
        }
    }


}