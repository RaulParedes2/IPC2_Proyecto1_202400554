namespace IPC2_Proyecto1
{
    public class Rejilla
    {
        private int tamanio;
        private ListaCelda celdas;

        public int Tamanio
        {
            set{
                tamanio = value;
            }

            get
            {
                return tamanio;
            }
        }

        public ListaCelda Celdas
        {
            set
            {
                celdas = value;
            }
            get
            {
                return celdas;
            }
        }

        public Rejilla(int tamanio)
        {
            this.tamanio = tamanio;
            Celdas = new ListaCelda();
        }

        public int ContarVecinos(int fila, int columna)
        {
            int contador = 0;

            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; i <= 1; i++)
                {
                    if(i==0 && j == 0)
                   continue;

                   int nuevaFila = fila+i;
                   int nuevaColumna = columna+j;

                   if(nuevaFila >=1 && nuevaFila<=Tamanio && 
                    nuevaColumna >=1 && nuevaColumna <= tamanio)
                    {
                        if(Celdas.Existe(nuevaFila, nuevaColumna))
                        {
                            contador++;
                        }
                    }
                }
            }
            return contador;
        }

        
         
    }
}