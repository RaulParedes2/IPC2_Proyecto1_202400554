using System.ComponentModel;
using System.IO;
using System.Diagnostics;

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
                     nuevaColumna >= 1 && nuevaColumna <= Tamanio)
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
        public ListaCelda CopiarEstado()
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

        public ResultadoSimulacion Simular(int maxPeriodos, string nombrePaciente = "Paciente")
        {
            if (maxPeriodos > 10000)
                maxPeriodos = 10000;

            // Guardamos el estado inicial 
            ListaEstado historial = new ListaEstado();
            ListaCelda estadoInicial = CopiarEstado();
            historial.Insertar(estadoInicial, 0);

            for (int periodo = 1; periodo <= maxPeriodos; periodo++)
            {
                // Ejecuta siguiente generación
                EjecutarPeriodo();
                ListaCelda estadoActual = CopiarEstado();

                //Guardar en historial
                historial.Insertar(estadoActual, periodo);

                // Buscar patrones (excluyendo el período actual)
                NodoEstado actualEstado = historial.Cabeza;

                while (actualEstado != null)
                {
                    // No comparar con el mismo período
                    if (actualEstado.Periodo != periodo)
                    {
                        if (SonIguales(estadoActual, actualEstado.Estado))
                        {
                            int periodoAnterior = actualEstado.Periodo;
                            int diferencia = periodo - periodoAnterior;

                            Console.WriteLine($"\n=== PATRÓN ENCONTRADO en período {periodo} (igual al período {periodoAnterior}) ===");
                            this.GraficarMatriz("Periodo_" + periodo, nombrePaciente);
                            MostrarEstadisticas(periodo);

                            // CASO A: volvió al patrón inicial
                            if (periodoAnterior == 0)
                            {
                                int N = periodo;
                                Console.WriteLine($" El patrón inicial se repitió después de {N} períodos.");
                                return new ResultadoSimulacion
                                {
                                    Tipo = (N == 1) ? "mortal" : "grave",
                                    N = N,
                                    N1 = 0
                                };
                            }
                            // CASO B: patrón distinto al inicial
                            else
                            {
                                int n1Real = diferencia;
                                bool esConsistente = VerificarConsistencia(historial, periodoAnterior, diferencia, periodo, estadoActual, ref n1Real);

                                if (esConsistente)
                                {
                                    Console.WriteLine($" Nuevo patrón encontrado en período {periodoAnterior} que se repite cada {n1Real} períodos.");
                                    return new ResultadoSimulacion
                                    {
                                        Tipo = (n1Real == 1) ? "mortal" : "grave",
                                        N = periodoAnterior,
                                        N1 = n1Real
                                    };
                                }
                                else
                                {
                                    Console.WriteLine($"Patrón repetido pero no consistente, continuando búsqueda...");
                                }
                            }
                        }
                    }
                    actualEstado = actualEstado.Siguiente;
                }
            }

            Console.WriteLine($"\n=== PATRÓN FINAL (Período {maxPeriodos}) ===");
            this.GraficarMatriz("Periodo_" + maxPeriodos, nombrePaciente);
            MostrarEstadisticas(maxPeriodos);
            Console.WriteLine("No se encontraron patrones repetidos - ENFERMEDAD LEVE");

            return new ResultadoSimulacion
            {
                Tipo = "leve",
                N = 0,
                N1 = 0
            };
        }
        public ResultadoSimulacion SimularPasoAPaso(int maxPeriodos, string nombrePaciente = "Paciente")
        {
            if (maxPeriodos > 10000)
                maxPeriodos = 10000;

            ListaEstado historial = new ListaEstado();
            ListaCelda estadoInicial = CopiarEstado();
            historial.Insertar(estadoInicial, 0);

            Console.WriteLine("\n=== PATRÓN INICIAL (Período 0) ===");
            this.GraficarMatriz("Periodo_0", nombrePaciente);
            MostrarEstadisticas(0);
            Console.WriteLine("----------------------------------------");

            // Variables para almacenar el resultado final
            string tipoResultado = "leve";
            int nEncontrado = 0;
            int n1Encontrado = 0;

            for (int periodo = 1; periodo <= maxPeriodos; periodo++)
            {
                Console.WriteLine($"\n=== PERÍODO {periodo} ===");

                // Ejecutar siguiente generación
                EjecutarPeriodo();
                
                ListaCelda estadoActual = CopiarEstado();

                // PRIMERO: Guardar en historial
                historial.Insertar(estadoActual, periodo);

                // DESPUÉS: Buscar patrones
                NodoEstado actualEstado = historial.Cabeza;

                // Mostrar estadísticas
                MostrarEstadisticas(periodo);

                // Generar gráfica
                this.GraficarMatriz("Periodo_" + periodo, nombrePaciente);

                Console.WriteLine("----------------------------------------");

                // Verificar si hay patrones repetidos
                //NodoEstado actualEstado = historial.Cabeza;
                //ListaCelda estadoActual = CopiarEstado();

                while (actualEstado != null)
                {
                    if (SonIguales(estadoActual, actualEstado.Estado))
                    {
                        int periodoAnterior = actualEstado.Periodo;
                        int diferencia = periodo - periodoAnterior;

                        Console.WriteLine($"\n ANÁLISIS: Se encontró un patrón repetido!");

                        if (periodoAnterior == 0)
                        {
                            // Caso A: Repetición del patrón inicial
                            nEncontrado = periodo;
                            n1Encontrado = 0;

                            if (nEncontrado == 1)
                            {
                                tipoResultado = "mortal";
                                Console.WriteLine($" El patrón inicial se repite en cada período - ENFERMEDAD MORTAL");
                            }
                            else
                            {
                                tipoResultado = "grave";
                                Console.WriteLine($" El patrón inicial se repite cada {nEncontrado} períodos - ENFERMEDAD GRAVE");
                            }

                            // IMPORTANTE: Salir del while pero continuar la simulación
                            break;
                        }
                        else
                        {
                            // Caso B: Repetición de otro patrón
                            // Verificar si la repetición es consistente
                            int n1Real = diferencia;
                            bool esConsistente = VerificarConsistencia(historial, periodoAnterior, diferencia, periodo, estadoActual, ref n1Real);

                            if (esConsistente)
                            {
                                nEncontrado = periodoAnterior;
                                n1Encontrado = n1Real;

                                if (n1Encontrado == 1)
                                {
                                    tipoResultado = "mortal";
                                    Console.WriteLine($"Patrón del período {nEncontrado} se repite en cada período - ENFERMEDAD MORTAL");
                                }
                                else
                                {
                                    tipoResultado = "grave";
                                    Console.WriteLine($"Patrón del período {nEncontrado} se repite cada {n1Encontrado} períodos - ENFERMEDAD GRAVE");
                                }

                                break;
                            }
                            else
                            {
                                Console.WriteLine($"Patrón repetido pero no consistente, continuando búsqueda...");
                            }
                        }
                    }
                    actualEstado = actualEstado.Siguiente;
                }

                // Guardar este período en el historial
                historial.Insertar(CopiarEstado(), periodo);

                Console.WriteLine("----------------------------------------");
            }

            // Mostrar resumen final
            Console.WriteLine("\n========== RESUMEN FINAL ==========");
            Console.WriteLine($"Períodos simulados: {maxPeriodos}");
            Console.WriteLine($"Resultado: {tipoResultado}");

            if (tipoResultado != "leve")
            {
                if (n1Encontrado == 0)
                {
                    Console.WriteLine($"El patrón inicial se repite cada {nEncontrado} períodos");
                }
                else
                {
                    Console.WriteLine($"Patrón encontrado en período {nEncontrado} que se repite cada {n1Encontrado} períodos");
                }
            }
            else
            {
                Console.WriteLine("No se encontraron patrones repetidos - ENFERMEDAD LEVE");
            }
            Console.WriteLine("====================================\n");

            return new ResultadoSimulacion
            {
                Tipo = tipoResultado,
                N = nEncontrado,
                N1 = n1Encontrado
            };
        }

        // Método auxiliar para verificar consistencia - VERSIÓN CORREGIDA
        private bool VerificarConsistencia(ListaEstado historial, int periodoInicio, int intervalo, int periodoActual, ListaCelda patron, ref int n1Real)
        {
            // Primero, verificar si se repite CADA período (intervalo = 1)
            bool repiteCada1 = true;

            // Verificar que TODOS los períodos desde periodoInicio+1 hasta periodoActual
            // sean IGUALES al patrón
            for (int p = periodoInicio + 1; p <= periodoActual; p++)
            {
                bool encontrado = false;
                NodoEstado nodo = historial.Cabeza;

                while (nodo != null)
                {
                    if (nodo.Periodo == p)
                    {
                        // Si el período p NO es igual al patrón, ya no es consistente
                        if (!SonIguales(patron, nodo.Estado))
                        {
                            repiteCada1 = false;
                            break;
                        }
                        encontrado = true;
                        break;
                    }
                    nodo = nodo.Siguiente;
                }

                // Si no se encontró el período en el historial, no es consistente
                if (!encontrado)
                {
                    repiteCada1 = false;
                    break;
                }

                if (!repiteCada1) break;
            }

            if (repiteCada1)
            {
                n1Real = 1;
                return true;
            }

            // Si no se repite cada 1, verificar con el intervalo original
            for (int p = periodoInicio + intervalo; p <= periodoActual; p += intervalo)
            {
                bool encontrado = false;
                NodoEstado nodo = historial.Cabeza;

                while (nodo != null)
                {
                    if (nodo.Periodo == p)
                    {
                        if (!SonIguales(patron, nodo.Estado))
                        {
                            return false;
                        }
                        encontrado = true;
                        break;
                    }
                    nodo = nodo.Siguiente;
                }

                if (!encontrado)
                    return false;
            }

            n1Real = intervalo;
            return true;
        }

        // En Rejilla.cs - ÚNICO método de graficación
        public void GraficarMatriz(string nombreArchivo, string nombrePaciente)
        {
            string carpeta = "Graphviz";

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string nombreLimpio = nombrePaciente.Replace(" ", "_");
            string rutaDot = Path.Combine(carpeta, nombreLimpio + "_" + nombreArchivo + ".dot");
            string rutaPng = Path.Combine(carpeta, nombreLimpio + "_" + nombreArchivo + ".png");

            // Crear el archivo DOT
            using (StreamWriter writer = new StreamWriter(rutaDot))
            {
                writer.WriteLine("digraph G {");
                writer.WriteLine("node [shape=plaintext];");
                writer.WriteLine("tabla [label=<");
                writer.WriteLine("<table border='1' cellborder='1' cellspacing='0'>");

                for (int i = 1; i <= this.Tamanio; i++)
                {
                    writer.WriteLine("<tr>");

                    for (int j = 1; j <= this.Tamanio; j++)
                    {
                        if (this.Celdas.Existe(i, j))
                            writer.WriteLine("<td bgcolor='red' width='20' height='20'></td>");
                        else
                            writer.WriteLine("<td width='20' height='20'></td>");
                    }

                    writer.WriteLine("</tr>");
                }

                writer.WriteLine("</table>");
                writer.WriteLine(">];");
                writer.WriteLine("}");
            }

            // Convertir a PNG
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "dot";
                psi.Arguments = $"-Tpng \"{rutaDot}\" -o \"{rutaPng}\"";
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                using (Process process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        process.WaitForExit();

                        if (process.ExitCode == 0)
                        {
                            Console.WriteLine($" Imagen generada: {rutaPng}");
                            File.Delete(rutaDot); // Eliminar .dot
                        }
                        else
                        {
                            string error = process.StandardError.ReadToEnd();
                            Console.WriteLine($" Error al generar PNG: {error}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" No se pudo generar PNG. Archivo DOT: {rutaDot}");
            }
        }
        public void MostrarEstadisticas(int periodo)
        {
            int contagiadas = celdas.Contar();
            int total = Tamanio * Tamanio;
            int sanas = total - contagiadas;

            Console.WriteLine("Periodo: " + periodo);
            Console.WriteLine("Células contagiadas: " + contagiadas);
            Console.WriteLine("Células sanas: " + sanas);
            Console.WriteLine("---------------------------------");
        }
    }

}