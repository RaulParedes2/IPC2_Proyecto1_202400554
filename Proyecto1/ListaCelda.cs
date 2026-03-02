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
        public bool Existe(int fila, int columna)
        {
            NodoCelda actual = cabeza;

            while (actual != null)
            {
                if (actual.Fila == fila && actual.Columna == columna)
                    return true;

                actual = actual.Siguiente;
            }
            return false;
        }

        public int Contar()
        {
            int contador = 0;
            NodoCelda actual = Cabeza;

            while (actual != null)
            {
                contador++;
                actual = actual.Siguiente;
            }

            return contador;
        }

        
        public void Limpiar()
        {
            cabeza = null;
        }



    }




}