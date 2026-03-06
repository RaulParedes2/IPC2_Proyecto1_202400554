using System.Data.SqlTypes;

namespace IPC2_Proyecto1
{
    public class Paciente
    {
        private string nombre;
        private int edad;
        private int periodo;
        private int m;
        public Rejilla Rejilla;
        public ResultadoSimulacion Resultado;

        public string Nombre
        {
            set
            {
                nombre = value;
            }
            get
            {
                return nombre;
            }
        }

        public int Edad
        {
            set
            {
                edad = value;
            }
            get
            {
                return edad;
            }
        }


        public int Periodos {
            set
            {
                 periodo= value;
            }
            get
            {
                return periodo;
            }
        }

        public int M
        {
            set
            {
                m = value;
            }
            get
            {
                return m;
            }
        }
        
        
        public Rejilla CopiarRejilla()
        {
            Rejilla copia = new Rejilla(this.M);

            NodoCelda original = this.Rejilla.Celdas.Cabeza;
            while (original != null)
            {
                copia.Celdas.Insertar(original.Fila, original.Columna);
                original = original.Siguiente;
            }

            return copia;
        }
    }
}