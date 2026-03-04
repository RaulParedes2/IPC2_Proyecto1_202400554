using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
namespace IPC2_Proyecto1
{
    class Program
    {
        static ListaPaciente pacientes = new ListaPaciente();

        public static void Main(string[] args)
        {
            string carpetaEntrada = "Entradas";
            string rutaEntrada = Path.Combine(carpetaEntrada, "entrada.xml");

            while (true)
            {
                if (Console.IsOutputRedirected == false)
                {
                    Console.Clear();
                }
                Console.WriteLine("===== SISTEMA DE ANALISIS DE PACIENTES =====");
                Console.WriteLine("1. Cargar XML");
                Console.WriteLine("2. Mostrar pacientes");
                Console.WriteLine("3. Simular paciente");
                Console.WriteLine("4. Simular paso a paso");
                Console.WriteLine("5. Limpiar memoria");
                Console.WriteLine("6. Generar XML Final");
                Console.WriteLine("7. Salir");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        if (!Directory.Exists(carpetaEntrada))
                        {
                            Console.WriteLine("La carpeta Entradas no existe.");
                        }
                        else if (!File.Exists(rutaEntrada))
                        {
                            Console.WriteLine("No se encontró el archivo entrada.xml");
                        }
                        else
                        {
                            LeerEntrada(rutaEntrada);
                        }
                        break;

                    case "2":
                        if (pacientes.EstaVacia())
                        {
                            Console.WriteLine("No hay pacientes cargados.");
                            break;
                        }
                        pacientes.Mostrar();
                        break;

                    case "3":
                        if (pacientes.EstaVacia())
                        {
                            Console.WriteLine("No hay pacientes cargados.");
                            break;
                        }
                        pacientes.Mostrar();
                        Console.Write("Seleccione el número del paciente: ");

                        if (int.TryParse(Console.ReadLine(), out int indice))
                        {
                            Paciente seleccionado = pacientes.ObtenerPorIndice(indice);

                            if (seleccionado != null)
                            {
                                //CREAR UNA COPIA DE LA REJILLA PARA SIMULAR
                                Rejilla rejillaSimulacion = new Rejilla(seleccionado.M);

                                // Copiar las celdas contagiadas del original
                                NodoCelda original = seleccionado.Rejilla.Celdas.Cabeza;
                                while (original != null)
                                {
                                    rejillaSimulacion.Celdas.Insertar(original.Fila, original.Columna);
                                    original = original.Siguiente;
                                }

                                // MOSTRAR PATRÓN INICIAL
                                Console.WriteLine($"\n=== PATRÓN INICIAL del paciente {seleccionado.Nombre} ===");
                                rejillaSimulacion.MostrarEstadisticas(0);
                                rejillaSimulacion.GraficarMatriz("Periodo_0", seleccionado.Nombre);

                                Console.WriteLine("\n--- Simulando... ---\n");

                                // SIMULAR usando la copia
                                seleccionado.Resultado =
                                    rejillaSimulacion.Simular(seleccionado.Periodos, seleccionado.Nombre);

                                Console.WriteLine("\n=== RESULTADO FINAL ===");
                                Console.WriteLine("Resultado: " + seleccionado.Resultado.Tipo);

                                if (seleccionado.Resultado.Tipo != "leve")
                                {
                                    Console.WriteLine("N: " + seleccionado.Resultado.N);
                                    if (seleccionado.Resultado.N1 > 0)
                                        Console.WriteLine("N1: " + seleccionado.Resultado.N1);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Paciente no válido.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Entrada inválida.");
                        }
                        break;

                    case "4":
                        if (pacientes.EstaVacia())
                        {
                            Console.WriteLine("No hay pacientes cargados.");
                            break;
                        }
                        pacientes.Mostrar();
                        Console.Write("Seleccione el número del paciente: ");

                        if (int.TryParse(Console.ReadLine(), out int indicePaso))
                        {
                            Paciente seleccionado = pacientes.ObtenerPorIndice(indicePaso);

                            if (seleccionado != null)
                            {
                                // CREAR UNA COPIA DE LA REJILLA PARA SIMULAR PASO A PASO
                                Rejilla rejillaSimulacion = new Rejilla(seleccionado.M);

                                // Copiar las celdas contagiadas del original
                                NodoCelda original = seleccionado.Rejilla.Celdas.Cabeza;
                                while (original != null)
                                {
                                    rejillaSimulacion.Celdas.Insertar(original.Fila, original.Columna);
                                    original = original.Siguiente;
                                }

                                Console.WriteLine($"\n=== Simulación PASO A PASO ===");
                                Console.WriteLine($"Paciente: {seleccionado.Nombre}");
                                Console.WriteLine($"Períodos a simular: {seleccionado.Periodos}");
                                Console.WriteLine("Generando imágenes para CADA período...\n");

                                // SIMULAR PASO A PASO usando la copia
                                seleccionado.Resultado =
                                    rejillaSimulacion.SimularPasoAPaso(seleccionado.Periodos, seleccionado.Nombre);

                                Console.WriteLine("\n Simulación completada.");
                                Console.WriteLine($"Resultado final: {seleccionado.Resultado.Tipo}");

                                if (seleccionado.Resultado.Tipo != "leve")
                                {
                                    Console.WriteLine($"N: {seleccionado.Resultado.N}");
                                    if (seleccionado.Resultado.N1 > 0)
                                        Console.WriteLine($"N1: {seleccionado.Resultado.N1}");
                                }

                                Console.WriteLine($"\n Revisa la carpeta 'Graphviz' para ver las imágenes generadas.");
                            }
                            else
                            {
                                Console.WriteLine("Paciente no válido.");
                            }
                        }
                        break;

                    case "5":
                        pacientes.Limpiar();
                        Console.WriteLine("Memoria limpiada.");
                        break;

                    case "6":
                        if (pacientes.EstaVacia())
                        {
                            Console.WriteLine("No hay datos para generar XML.");
                            break;
                        }
                        GenerarXMLFinal("Salidas/SalidaFinal.xml");
                        break;

                    case "7":
                        return;

                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }


            }
        }


        static void Mostrar(Rejilla rejilla)
        {
            NodoCelda actual = rejilla.Celdas.Cabeza;

            while (actual != null)
            {
                Console.WriteLine("Celda activa en: (" + actual.Fila + "," + actual.Columna + ")");
                actual = actual.Siguiente;
            }
            Console.WriteLine("------------------------");
        }
        /*
                static void GenerarXML(string nombre, int edad, int periodos, int m, ResultadoSimulacion resultado)
                {
                    string carpeta = "Salidas";

                    if (!Directory.Exists(carpeta))
                    {
                        Directory.CreateDirectory(carpeta);
                    }
                    //Limpieza de los nombre

                    string nombreLimpio = nombre.Replace(" ", " _");
                    string fechaHora = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                    string nombreArchivo = nombreLimpio + "_" + fechaHora + ".xml";
                    string ruta = Path.Combine(carpeta, nombreArchivo);

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;

                    using (XmlWriter writer = XmlWriter.Create(ruta, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Pacinetes");

                        writer.WriteStartElement("paciente");

                        writer.WriteStartElement("datospersonales");
                        writer.WriteElementString("nombre", nombre);
                        writer.WriteElementString("edad", edad.ToString());
                        writer.WriteEndElement();

                        writer.WriteElementString("Periodos", periodos.ToString());
                        writer.WriteElementString("m", m.ToString());
                        writer.WriteElementString("resultado", resultado.Tipo);

                        if (resultado.Tipo != "Leve")
                            writer.WriteElementString("n", resultado.N.ToString());

                        if (resultado.N1 > 0)
                            writer.WriteElementString("n1", resultado.N1.ToString());

                        writer.WriteEndElement(); //Paciente
                        writer.WriteEndElement(); //pacientes
                        writer.WriteEndDocument();

                    }
                    Console.WriteLine("XML generado en: " + ruta);

                }*/

        static void GenerarXMLFinal(string ruta)
        {
            // Asegurar que la carpeta Salidas existe
            string carpeta = Path.GetDirectoryName(ruta);
            if (!string.IsNullOrEmpty(carpeta) && !Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            XmlDocument doc = new XmlDocument();

            /*
            XmlElement raiz = doc.CreateElement("resultados");
            doc.AppendChild(raiz);
            */

            XmlElement pacientesElem = doc.CreateElement("pacientes");
            doc.AppendChild(pacientesElem);


            NodoPaciente actual = pacientes.Cabeza;

            while (actual != null)
            {
                Paciente p = actual.Datos;

                if (p.Resultado != null)
                {
                    XmlElement pacienteElem = doc.CreateElement("paciente");

                    // Datos personales
                    XmlElement datosPersonales = doc.CreateElement("datospersonales");

                    XmlElement nombre = doc.CreateElement("nombre");
                    nombre.InnerText = p.Nombre;
                    datosPersonales.AppendChild(nombre);

                    XmlElement edad = doc.CreateElement("edad");
                    edad.InnerText = p.Edad.ToString();
                    datosPersonales.AppendChild(edad);

                    pacienteElem.AppendChild(datosPersonales);

                    // PERIODOS (faltaba)
                    XmlElement periodos = doc.CreateElement("periodos");
                    periodos.InnerText = p.Periodos.ToString();
                    pacienteElem.AppendChild(periodos);

                    // M (faltaba)
                    XmlElement m = doc.CreateElement("m");
                    m.InnerText = p.M.ToString();
                    pacienteElem.AppendChild(m);

                    // Resultado
                    XmlElement resultado = doc.CreateElement("resultado");
                    resultado.InnerText = p.Resultado.Tipo.ToUpper(); // Convierte a mayúsculas
                    pacienteElem.AppendChild(resultado);

                    // N (si existe y es > 0)
                    if (p.Resultado.N > 0)
                    {
                        XmlElement n = doc.CreateElement("n");
                        n.InnerText = p.Resultado.N.ToString();
                        pacienteElem.AppendChild(n);
                    }

                    // N1 (si existe y es > 0)
                    if (p.Resultado.N1 > 0)
                    {
                        XmlElement n1 = doc.CreateElement("n1");
                        n1.InnerText = p.Resultado.N1.ToString();
                        pacienteElem.AppendChild(n1);
                    }

                    pacientesElem.AppendChild(pacienteElem);
                }

                actual = actual.Siguiente;
            }
            // Guardar con formato bonito
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    "; // 4 espacios para indentación
            settings.Encoding = System.Text.Encoding.UTF8;

            using (XmlWriter writer = XmlWriter.Create(ruta, settings))
            {
                doc.Save(writer);
            }

            /*doc.Save(ruta);*/
            Console.WriteLine($" Archivo XML generado correctamente en: {ruta}");
        }
        static void LeerEntrada(string rutaArchivo)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(rutaArchivo);

            XmlNodeList? listaPacientes = doc.SelectNodes("//paciente");

            if (listaPacientes == null)
            {
                Console.WriteLine("No se encontraron pacientes.");
                return;
            }

            foreach (XmlNode paciente in listaPacientes)
            {
                XmlNode? datos = paciente.SelectSingleNode("datospersonales");
                if (datos == null) continue;

                string nombre = datos["nombre"]?.InnerText ?? "Desconocido";

                if (!int.TryParse(datos["edad"]?.InnerText, out int edad))
                    edad = 0;

                if (!int.TryParse(paciente["periodos"]?.InnerText, out int periodos))
                    periodos = 0;

                if (!int.TryParse(paciente["m"]?.InnerText, out int m))
                    m = 0;

                // VALIDACIÓN DE LÍMITES
                if (m <= 0 || m > 10000)
                {
                    Console.WriteLine($"El tamaño de rejilla del paciente {nombre} es inválido.");
                    continue;
                }

                if (periodos <= 0 || periodos > 10000)
                {
                    Console.WriteLine($"Los períodos del paciente {nombre} son inválidos.");
                    continue;
                }

                Rejilla rejilla = new Rejilla(m);

                XmlNodeList? celdas = paciente.SelectNodes("rejilla/celda");

                if (celdas != null)
                {
                    foreach (XmlNode celda in celdas)
                    {
                        if (celda.Attributes == null) continue;

                        if (!int.TryParse(celda.Attributes["f"]?.Value, out int fila))
                            continue;

                        if (!int.TryParse(celda.Attributes["c"]?.Value, out int columna))
                            continue;

                        // VALIDAR QUE ESTÉ DENTRO DE LA MATRIZ
                        if (fila >= 1 && fila <= m && columna >= 1 && columna <= m)
                        {
                            rejilla.Celdas.Insertar(fila, columna);
                        }
                    }
                }

                Paciente nuevo = new Paciente();
                nuevo.Nombre = nombre;
                nuevo.Edad = edad;
                nuevo.Periodos = periodos;
                nuevo.M = m;
                nuevo.Rejilla = rejilla;

                pacientes.Insertar(nuevo);
            }

            Console.WriteLine("Pacientes cargados correctamente.");
        }

        /*
        static void GenerarDotMatriz(string nombreArchivo, string nombrePaciente)
        {
            string carpeta = "Graphviz";

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string nombreLimpio = nombrePaciente.Replace(" ", "_");
            string rutaDot = Path.Combine(carpeta, nombreLimpio + "_" + nombreArchivo + ".dot");
            string rutaPng = Path.Combine(carpeta, nombreLimpio + "_" + nombreArchivo + ".png");

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

            Console.WriteLine("Generando imagen PNG...");

            // Intentar convertir a PNG si Graphviz está instalado
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

                            // OPCIONAL: Eliminar el archivo .dot después de generar el PNG
                            try
                            {
                                File.Delete(rutaDot);
                                // Console.WriteLine("Archivo .dot eliminado temporal.");
                            }
                            catch
                            {
                                // Si no se puede eliminar, no importa
                            }
                        }
                        else
                        {
                            string error = process.StandardError.ReadToEnd();
                            Console.WriteLine($" Error al generar PNG: {error}");
                            Console.WriteLine($"   Archivo DOT disponible en: {rutaDot}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("  No se pudo generar la imagen PNG. Asegúrate de tener Graphviz instalado.");
                Console.WriteLine("   Puedes instalar Graphviz desde: https://graphviz.org/download/");
                Console.WriteLine($"   Archivo DOT disponible en: {rutaDot}");
            }

            // Convertir automáticamente a PNG
             ProcessStartInfo psi = new ProcessStartInfo();
             psi.FileName = "dot";
             psi.Arguments = $"-Tpng \"{rutaDot}\" -o \"{rutaPng}\"";
             psi.UseShellExecute = false;
             psi.CreateNoWindow = true;

             Process.Start(psi).WaitForExit();

             Console.WriteLine("Imagen generada en: " + rutaPng);
             
        }*/
    }
}