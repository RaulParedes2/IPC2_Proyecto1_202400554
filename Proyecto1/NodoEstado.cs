namespace IPC2_Proyecto1
{
    public class NodoEstado
    {

        private int periodo;
        public ListaCelda Estado;
        
        public NodoEstado Siguiente;

        public int Periodo
        {
            set
            {
                periodo = value;
            }
            get
            {
                return periodo;
            }
        }

        public NodoEstado(ListaCelda estado, int periodo){

            Estado=  estado;
            Periodo =  periodo;
            Siguiente = null;

        }
    }
}