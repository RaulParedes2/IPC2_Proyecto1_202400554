namespace IPC2_Proyecto1
{
    public class NodoPaciente
    {
        public Paciente Datos;
        public NodoPaciente Siguiente;

        public NodoPaciente(Paciente datos)
        {
            Datos = datos;
            Siguiente = null;
        }
    }
}