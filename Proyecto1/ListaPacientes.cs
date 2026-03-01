namespace IPC2_Proyecto1
{
    public class ListaPaciente
    {
        public NodoPaciente Cabeza;

        public ListaPaciente()
        {
            Cabeza = null;
        }

        public void Insertar(Paciente p)
        {
            NodoPaciente nuevo = new NodoPaciente(p);

            if (Cabeza == null)
            {
                Cabeza = nuevo;
            }
            else
            {
                NodoPaciente actual = Cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
        }

        public void Mostrar()
        {
            NodoPaciente actual = Cabeza;
            int i = 1;

            while (actual != null)
            {
                Console.WriteLine(i + ". " + actual.Datos.Nombre);
                actual = actual.Siguiente;
                i++;
            }
        }

        public Paciente ObtenerPorIndice(int indice)
        {
            NodoPaciente actual = Cabeza;
            int i = 1;

            while (actual != null)
            {
                if (i == indice)
                    return actual.Datos;

                actual = actual.Siguiente;
                i++;
            }

            return null;
        }

        public void Limpiar()
        {
            Cabeza = null;
        }
    }
}