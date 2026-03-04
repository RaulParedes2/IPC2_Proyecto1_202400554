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
            historial.Insertar(CopiarEstado(), 0);

            Console.WriteLine("\n=== PATRÓN INICIAL (Período 0) ===");
            this.GraficarMatriz("Periodo_0", nombrePaciente);
            MostrarEstadisticas(0);

            for (int periodo = 1; periodo <= maxPeriodos; periodo++)
            {
                // Ejecuta siguiente generación
                EjecutarPeriodo();

                // (Opcional) Mostrar estadísticas
                MostrarEstadisticas(periodo);

                // Recorremos el historial para ver si el patrón ya existía
                NodoEstado actualEstado = historial.Cabeza;
                ListaCelda estadoActual = CopiarEstado(); //para copiar y comparar

                while (actualEstado != null)
                {
                    if (SonIguales(estadoActual, actualEstado.Estado))
                    {
                        int periodoAnterior = actualEstado.Periodo;
                        int diferencia = periodo - periodoAnterior;

                        Console.WriteLine($"\n=== PATRÓN FINAL (Período {periodo}) ===");
                        this.GraficarMatriz("Periodo_" + periodo, nombrePaciente);

                        // =========================
                        // CASO A: volvió al patrón inicial
                        // =========================
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

                        // =========================
                        // CASO B: patrón distinto al inicial
                        // =========================
                        else
                        {
                            int N1 = diferencia;
                            Console.WriteLine($" Nuevo patrón encontrado en período {periodoAnterior} que se repite cada {N1} períodos.");

                            return new ResultadoSimulacion
                            {
                                Tipo = (N1 == 1) ? "mortal" : "grave",
                                N = 0,
                                N1 = N1
                            };
                        }
                    }

                    actualEstado = actualEstado.Siguiente;
                }

                // Si no se repitió, lo agregamos al historial
                historial.Insertar(CopiarEstado(), periodo);
            }

            // =========================
            // CASO C: nunca se repitió
            // =========================
            Console.WriteLine($"\n=== LÍMITE ALCANZADO: {maxPeriodos} períodos ===");
            Console.WriteLine("No se encontraron patrones repetidos - ENFERMEDAD LEVE");
            this.GraficarMatriz("Periodo_" + maxPeriodos, nombrePaciente);
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
            historial.Insertar(CopiarEstado(), 0);

            Console.WriteLine("\n=== PATRÓN INICIAL (Período 0) ===");
            this.GraficarMatriz("Periodo_0", nombrePaciente);
            MostrarEstadisticas(0);

            /*
            Console.WriteLine("Presione Enter para continuar al siguiente período...");
            Console.ReadLine();
            */

            for (int periodo = 1; periodo <= maxPeriodos; periodo++)
            {
                Console.WriteLine($"\n=== PERÍODO {periodo} ===");
                EjecutarPeriodo();
                MostrarEstadisticas(periodo);
                this.GraficarMatriz("Periodo_" + periodo, nombrePaciente);


                NodoEstado actualEstado = historial.Cabeza;
                ListaCelda estadoActual = CopiarEstado();

                while (actualEstado != null)
                {
                    if (SonIguales(estadoActual, actualEstado.Estado))
                    {
                        int periodoAnterior = actualEstado.Periodo;
                        int diferencia = periodo - periodoAnterior;

                        Console.WriteLine($"\n Patrón repetido encontrado");

                        if (periodoAnterior == 0)
                        {
                            int N = periodo;
                            Console.WriteLine($"El patrón inicial se repitió después de {N} períodos.");

                            return new ResultadoSimulacion
                            {
                                Tipo = (N == 1) ? "mortal" : "grave",
                                N = N,
                                N1 = 0
                            };
                        }
                        else
                        {
                            int N1 = diferencia;
                            Console.WriteLine($"Nuevo patrón encontrado en período {periodoAnterior} que se repite cada {N1} períodos.");

                            return new ResultadoSimulacion
                            {
                                Tipo = (N1 == 1) ? "mortal" : "grave",
                                N = 0,
                                N1 = N1
                            };
                        }
                    }

                    actualEstado = actualEstado.Siguiente;
                }

                historial.Insertar(CopiarEstado(), periodo);
                /*
                if (periodo < maxPeriodos)
                {
                    Console.WriteLine("Presione Enter para continuar al siguiente período...");
                    Console.ReadLine();
                }
                */
            }

            Console.WriteLine($"\n=== LÍMITE ALCANZADO: {maxPeriodos} períodos ===");
            Console.WriteLine("No se encontraron patrones repetidos  - ENFERMEDAD LEVE.");

            return new ResultadoSimulacion
            {
                Tipo = "leve",
                N = 0,
                N1 = 0
            };
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