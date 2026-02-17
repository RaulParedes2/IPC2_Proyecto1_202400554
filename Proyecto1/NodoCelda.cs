namespace IPC2_Proyecto1
{

    public class NodoCelda
    {

        private int fila;
        private int columna;

        private NodoCelda siguiente;
        public int Fila
        {
            set
            {
                fila = value;
            }
            get
            {
                return fila;
            }
        }

        public int Columna
        {
            set
            {
                columna = value;
            }

            get
            {
                return columna;
            }
        }

        public NodoCelda Siguiente
        {
            set
            {
                siguiente = value;
            }

            get
            {
                return siguiente;
            }
        }

        public NodoCelda(int fila, int columna)
        {
            this.fila = fila;
            this.columna = columna;
            this.siguiente = null;
        }
    }
}