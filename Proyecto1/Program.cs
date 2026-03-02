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
                                seleccionado.Resultado =
                                    seleccionado.Rejilla.Simular(seleccionado.Periodos);

                                Console.WriteLine("Resultado: " + seleccionado.Resultado.Tipo);
                                Console.WriteLine("N: " + seleccionado.Resultado.N);
                                Console.WriteLine("N1: " + seleccionado.Resultado.N1);
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
                                seleccionado.Resultado =
                                    seleccionado.Rejilla.SimularPasoAPaso(seleccionado.Periodos, seleccionado.Nombre);

                                Console.WriteLine("Resultado Final: " + seleccionado.Resultado.Tipo);
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
            XmlDocument doc = new XmlDocument();

            XmlElement raiz = doc.CreateElement("resultados");
            doc.AppendChild(raiz);

            NodoPaciente actual = pacientes.Cabeza;

            while (actual != null)
            {
                Paciente p = actual.Datos;

                if (p.Resultado != null)
                {
                    XmlElement paciente = doc.CreateElement("paciente");

                    XmlElement nombre = doc.CreateElement("nombre");
                    nombre.InnerText = p.Nombre;
                    paciente.AppendChild(nombre);

                    XmlElement tipo = doc.CreateElement("tipo");
                    tipo.InnerText = p.Resultado.Tipo;
                    paciente.AppendChild(tipo);

                    XmlElement n = doc.CreateElement("N");
                    n.InnerText = p.Resultado.N.ToString();
                    paciente.AppendChild(n);

                    XmlElement n1 = doc.CreateElement("N1");
                    n1.InnerText = p.Resultado.N1.ToString();
                    paciente.AppendChild(n1);

                    raiz.AppendChild(paciente);
                }

                actual = actual.Siguiente;
            }

            doc.Save(ruta);

            Console.WriteLine("Archivo XML generado correctamente.");
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
                        if (fila >= 0 && fila < m && columna >= 0 && columna < m)
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


        static void GenerarDotMatriz(ListaCelda lista, int m, string nombrePaciente, int periodo)
        {
            string carpeta = "Graphviz";

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string nombreLimpio = nombrePaciente.Replace(" ", "_");
            string rutaDot = Path.Combine(carpeta, nombreLimpio + "_Periodo_" + periodo + ".dot");
            string rutaPng = Path.Combine(carpeta, nombreLimpio + "_Periodo_" + periodo + ".png");

            using (StreamWriter writer = new StreamWriter(rutaDot))
            {
                writer.WriteLine("digraph G {");
                writer.WriteLine("node [shape=plaintext];");
                writer.WriteLine("tabla [label=<");
                writer.WriteLine("<table border='1' cellborder='1' cellspacing='0'>");

                for (int i = 1; i <= m; i++)
                {
                    writer.WriteLine("<tr>");

                    for (int j = 1; j <= m; j++)
                    {
                        if (lista.Existe(i, j))
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

            // Convertir automáticamente a PNG
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "dot";
            psi.Arguments = $"-Tpng \"{rutaDot}\" -o \"{rutaPng}\"";
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            Process.Start(psi).WaitForExit();

            Console.WriteLine("Imagen generada en: " + rutaPng);
        }
    }
}