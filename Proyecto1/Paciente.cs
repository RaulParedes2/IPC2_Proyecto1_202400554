namespace IPC2_Proyecto1
{
    public class Paciente
    {
        public string Nombre;
        public int Edad;
        public int Periodos;
        public int M;
        public Rejilla Rejilla;
        public ResultadoSimulacion Resultado;

        
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