using System.ComponentModel;

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
                for(int j = -1; j <= 1; j++)
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

        public void EjecutarPeriodo()
        {
            ListaCelda nuevaLista = new ListaCelda();
            ListaCelda evaluadas = new ListaCelda();

            NodoCelda actual = Celdas.Cabeza;

            while(actual != null)
            {
                EvaluarCelda(actual.Fila, actual.Columna, nuevaLista, evaluadas);

                for(int i = -1; i <= 1; i++)
                {
                    for(int j = -1; j <=1; j++)
                    {
                        if(i==0 && j==0)
                        continue;

                        int nuevaFila = actual.Fila + i;
                        int nuevaColumna = actual.Columna + j;

                        if(nuevaFila >= 1 && nuevaFila <=Tamanio
                        && nuevaColumna >= 1 && nuevaColumna <=tamanio)
                        {
                            EvaluarCelda(nuevaFila, nuevaColumna, nuevaLista, evaluadas);
                        }
                    }
                }
                actual = actual.Siguiente;
            }
            Celdas = nuevaLista;
        }

        public void EvaluarCelda(int fila, int columna, ListaCelda nuevaLista, ListaCelda evaluadas)
        {
            if(evaluadas.Existe(fila,columna))
            return;

            evaluadas.Insertar(fila, columna);

            int vecinos = ContarVecinos(fila, columna);
            bool estaContagiada = Celdas.Existe(fila, columna);

            if (estaContagiada)
            {
                if(vecinos == 2 || vecinos == 3)
                {
                    nuevaLista.Insertar(fila, columna);
                }
            }
            else
            {
                if (vecinos == 3)
                {
                    nuevaLista.Insertar(fila, columna);
                }
            }
        }
         
    }
}