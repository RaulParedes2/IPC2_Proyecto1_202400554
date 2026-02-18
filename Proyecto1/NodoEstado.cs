namespace IPC2_Proyecto1
{
    public class NodoEstado
    {
        public ListaCelda Estado;
        public int Periodo;
        public NodoEstado Siguiente;

        public NodoEstado(ListaCelda estado, int periodo){

            Estado=  estado;
            Periodo =  periodo;
            Siguiente = null;

        }
    }
}