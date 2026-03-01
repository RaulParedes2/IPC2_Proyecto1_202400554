using System;
using System.IO;
using System.Xml;
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
        Console.Clear();
        Console.WriteLine("===== SISTEMA DE ANALISIS DE PACIENTES =====");
        Console.WriteLine("1. Cargar XML");
        Console.WriteLine("2. Mostrar pacientes");
        Console.WriteLine("3. Simular paciente");
        Console.WriteLine("4. Limpiar memoria");
        Console.WriteLine("5. Salir");
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
                pacientes.Mostrar();
                break;

            case "3":
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

                        GenerarXML(
                            seleccionado.Nombre,
                            seleccionado.Edad,
                            seleccionado.Periodos,
                            seleccionado.M,
                            seleccionado.Resultado
                        );
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
                pacientes.Limpiar();
                Console.WriteLine("Memoria limpiada.");
                break;

            case "5":
                return;

            default:
                Console.WriteLine("Opción inválida.");
                break;
        }

        Console.WriteLine("\nPresione una tecla para continuar...");
        Console.ReadKey();
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
                int edad = int.Parse(datos["edad"]?.InnerText ?? "0");

                int periodos = int.Parse(paciente["periodos"]?.InnerText ?? "0");
                int m = int.Parse(paciente["m"]?.InnerText ?? "0");

                Rejilla rejilla = new Rejilla(m);

                XmlNodeList? celdas = paciente.SelectNodes("rejilla/celda");

                if (celdas != null)
                {
                    foreach (XmlNode celda in celdas)
                    {
                        if (celda.Attributes == null) continue;

                        int fila = int.Parse(celda.Attributes["f"]?.Value ?? "0");
                        int columna = int.Parse(celda.Attributes["c"]?.Value ?? "0");

                        rejilla.Celdas.Insertar(fila, columna);
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


        static void GenerarDotListaCelda(ListaCelda lista, string nombrePaciente)
        {
            string carpeta = "Graphviz";

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string nombreLimpio = nombrePaciente.Replace(" ", "_");
            string ruta = Path.Combine(carpeta, nombreLimpio + "_ListaCelda.dot");

            using (StreamWriter writer = new StreamWriter(ruta))
            {
                writer.WriteLine("digraph ListaCelda {");
                writer.WriteLine("rankdir=LR;");
                writer.WriteLine("node [shape=box];");

                NodoCelda actual = lista.Cabeza;
                int contador = 0;

                while (actual != null)
                {
                    writer.WriteLine($"n{contador} [label=\"({actual.Fila},{actual.Columna})\"];");

                    if (actual.Siguiente != null)
                        writer.WriteLine($"n{contador} -> n{contador + 1};");

                    actual = actual.Siguiente;
                    contador++;
                }

                writer.WriteLine("}");
            }

            Console.WriteLine("Archivo DOT generado en: " + ruta);
        }
    }
}