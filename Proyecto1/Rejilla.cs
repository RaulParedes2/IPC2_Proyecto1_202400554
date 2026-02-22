using System.ComponentModel;

namespace IPC2_Proyecto1
{
    public class Rejilla
    {
        private int tamanio;
        private ListaCelda celdas;

        public int Tamanio
        {
            set
            {
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

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    int nuevaFila = fila + i;
                    int nuevaColumna = columna + j;

                    if (nuevaFila >= 1 && nuevaFila <= Tamanio &&
                     nuevaColumna >= 1 && nuevaColumna <= tamanio)
                    {
                        if (Celdas.Existe(nuevaFila, nuevaColumna))
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

            while (actual != null)
            {
                EvaluarCelda(actual.Fila, actual.Columna, nuevaLista, evaluadas);

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                            continue;

                        int nuevaFila = actual.Fila + i;
                        int nuevaColumna = actual.Columna + j;

                        if (nuevaFila >= 1 && nuevaFila <= Tamanio
                        && nuevaColumna >= 1 && nuevaColumna <= tamanio)
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
            if (evaluadas.Existe(fila, columna))
                return;

            evaluadas.Insertar(fila, columna);

            int vecinos = ContarVecinos(fila, columna);
            bool estaContagiada = Celdas.Existe(fila, columna);

            if (estaContagiada)
            {
                if (vecinos == 2 || vecinos == 3)
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
        public ListaCelda CopiarEsatdo()
        {
            ListaCelda copia = new ListaCelda();
            NodoCelda actual = Celdas.Cabeza;

            while (actual != null)
            {
                copia.Insertar(actual.Fila, actual.Columna);
                actual = actual.Siguiente;
            }
            return copia;
        }

        private bool SonIguales(ListaCelda a, ListaCelda b)
        {
            NodoCelda actualA = a.Cabeza;

            while (actualA != null)
            {
                if (!b.Existe(actualA.Fila, actualA.Columna))
                    return false;

                actualA = actualA.Siguiente;
            }

            NodoCelda actualB = b.Cabeza;

            while (actualB != null)
            {
                if (!a.Existe(actualB.Fila, actualB.Columna))
                    return false;

                actualB = actualB.Siguiente;
            }

            return true;
        }

        public ResultadoSimulacion Simular(int maxPeriodos)
        {
            ListaEstado historial = new ListaEstado();
            historial.Insertar(CopiarEsatdo(), 0);

            for (int periodo = 1; periodo <= maxPeriodos; periodo++)
            {
                EjecutarPeriodo();

                NodoEstado actualEstado = historial.Cabeza;

                while (actualEstado != null)
                {
                    if (SonIguales(Celdas, actualEstado.Estado))
                    {
                        int N = actualEstado.Periodo;
                        int N1 = periodo - actualEstado.Periodo;

                        if (N == 1)
                        {
                            return new ResultadoSimulacion
                            {
                              Tipo = "mortal",
                              N = N,
                              N1 = 1  
                            };
                        }
                        else
                        {
                           return new ResultadoSimulacion
                           {
                               Tipo = "grave",
                               N = N,
                               N1 = N1
                           };
                        }
                    }
                    actualEstado = actualEstado.Siguiente;
                }
                historial.Insertar(CopiarEsatdo(), periodo);
            }
            return new ResultadoSimulacion
            {
              Tipo = "leve",
              N = 0,
              N1 = 0

            };
        }
    }
    
}