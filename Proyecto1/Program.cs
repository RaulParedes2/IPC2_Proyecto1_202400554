using System;
using System.IO;
using System.Xml;
namespace IPC2_Proyecto1
{
    class Program
    {
        public static void Main(string[] args)
        {
            Rejilla rejilla = new Rejilla(10);

            //Console.WriteLine("Celulas 1");
            rejilla.Celdas.Insertar(5, 2);
            rejilla.Celdas.Insertar(5, 3);
            rejilla.Celdas.Insertar(5, 4);

            // Console.WriteLine("Celulas 2");
            rejilla.Celdas.Insertar(7, 5);
            rejilla.Celdas.Insertar(8, 5);
            rejilla.Celdas.Insertar(9, 5);

            // Console.WriteLine("Celulas 3");
            rejilla.Celdas.Insertar(1, 6);
            rejilla.Celdas.Insertar(2, 6);
            rejilla.Celdas.Insertar(3, 6);

            //  Console.WriteLine("Celulas 4");
            rejilla.Celdas.Insertar(5, 7);
            rejilla.Celdas.Insertar(5, 8);
            rejilla.Celdas.Insertar(5, 9);

            /* Console.WriteLine("Periodo 0");
             Mostrar(rejilla);


             rejilla.EjecutarPeriodo();


             Console.WriteLine("Periodo 1");
             Mostrar(rejilla);

             rejilla.EjecutarPeriodo();

             Console.WriteLine("Periodo 2");
             Mostrar(rejilla);
 */
            /*string resultado = rejilla.Simular(100);
            Console.WriteLine("Resultado: " + resultado);
            */
            ResultadoSimulacion resultado = rejilla.Simular(100);
            Console.WriteLine("Resultado: " + resultado);


            Console.ReadLine();
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

        static void GenerarXML(string nombre, int edad, int periodos, int m, ResultadoSimulacion resultado )
        {
            string carpeta ="Salidas";

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }
            //Limpieza de los nombre

            string nombreLimpio = nombre.Replace(" "," _" );
            string fechaHora = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string nombreArchivo = nombreLimpio + "_" + fechaHora + ".xml";
            string ruta = Path.Combine (carpeta, nombreArchivo);

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

                if(resultado.Tipo != "Leve")
                writer.WriteElementString("n", resultado.N.ToString());

                if(resultado.N1 > 0)
                writer.WriteElementString("n1", resultado.N1.ToString());

                writer.WriteEndElement(); //Paciente
                writer.WriteEndElement(); //pacientes
                writer.WriteEndDocument();

            }
            Console.WriteLine("XML generado en: "+ruta);
        
        }
    }
}