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
                Console.WriteLine("----------------------------------------------");
                Console.Write("Seleccione una opción: " );

                string opcion = Console.ReadLine();

                switch (opcion)
                {

                    case "1":
                        Console.Write("Ingrese el nombre del archivo XML (ej: entrada.xml): ");
                        string? nombreArchivo = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(nombreArchivo))
                        {
                            Console.WriteLine(" Nombre de archivo inválido.");
                            break;
                        }

                        // Asegurar que tenga extensión .xml
                        if (!nombreArchivo.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                        {
                            nombreArchivo += ".xml";
                        }

                        string carpetaEntradaArchivos = "Entradas";
                        string rutaEntrada2 = Path.Combine(carpetaEntradaArchivos, nombreArchivo);

                        if (!Directory.Exists(carpetaEntradaArchivos))
                        {
                            Console.WriteLine($"La carpeta '{carpetaEntradaArchivos}' no existe.");
                            Console.WriteLine("   Creando carpeta...");
                            Directory.CreateDirectory(carpetaEntradaArchivos);
                            Console.WriteLine($"   Por favor, coloque el archivo '{nombreArchivo}' en la carpeta '{carpetaEntradaArchivos}'");
                        }
                        else if (!File.Exists(rutaEntrada2))
                        {
                            Console.WriteLine($" No se encontró el archivo '{nombreArchivo}' en la carpeta '{carpetaEntradaArchivos}'");
                            Console.WriteLine("   Archivos disponibles:");

                            string[] archivos = Directory.GetFiles(carpetaEntradaArchivos, "*.xml");
                            if (archivos.Length == 0)
                            {
                                Console.WriteLine("   No hay archivos XML en la carpeta.");
                            }
                            else
                            {
                                foreach (string archivo in archivos)
                                {
                                    Console.WriteLine($"   - {Path.GetFileName(archivo)}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"\n Cargando archivo: {nombreArchivo}");
                            LeerEntrada(rutaEntrada2);
                        }

                        /*Console.WriteLine("\nPresione Enter para continuar...");
                        Console.ReadLine();*/
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
                        Console.WriteLine("Saliendo...");
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
                Console.WriteLine($"\n Paciente: {nombre}");
                Console.WriteLine($"   Tamaño de matriz: {m} x {m}");
                Console.WriteLine($"   Total de celdas: {m * m}");

                Rejilla rejilla = new Rejilla(m);

                XmlNodeList? celdas = paciente.SelectNodes("rejilla/celda");
                //*
                int contadorCeldas = 0;

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
                            contadorCeldas++;
                        }
                    }
                }
                //*x
                Console.WriteLine($"   Células contagiadas iniciales: {contadorCeldas}");
                Console.WriteLine($"   Células sanas iniciales: {m * m - contadorCeldas}");
                Console.WriteLine("----------------------------------------");

                Paciente nuevo = new Paciente();
                nuevo.Nombre = nombre;
                nuevo.Edad = edad;
                nuevo.Periodos = periodos;
                nuevo.M = m;
                nuevo.Rejilla = rejilla;

                pacientes.Insertar(nuevo);
            }

            Console.WriteLine($"\n Pacientes cargados correctamente Total: {ContarPacientes()}.");
        }
        static int ContarPacientes()
        {
            int contador = 0;
            NodoPaciente actual = pacientes.Cabeza;
            while (actual != null)
            {
                contador++;
                actual = actual.Siguiente;
            }
            return contador;
        }
    }
}