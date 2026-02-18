using System;

namespace IPC2_Proyecto1
{
    class Program
    {
        public static void Main(string[] args)
        {
            Rejilla rejilla = new Rejilla(10);

            //Console.WriteLine("Celulas 1");
            rejilla.Celdas.Insertar( 5, 2);
            rejilla.Celdas.Insertar( 5, 3);
            rejilla.Celdas.Insertar( 5, 4);

           // Console.WriteLine("Celulas 2");
            rejilla.Celdas.Insertar( 7, 5);
            rejilla.Celdas.Insertar( 8, 5);
            rejilla.Celdas.Insertar( 9, 5);

           // Console.WriteLine("Celulas 3");
            rejilla.Celdas.Insertar( 1, 6);
            rejilla.Celdas.Insertar( 2, 6);
            rejilla.Celdas.Insertar( 3, 6);

          //  Console.WriteLine("Celulas 4");
            rejilla.Celdas.Insertar( 5, 7);
            rejilla.Celdas.Insertar( 5, 8);
            rejilla.Celdas.Insertar( 5, 9);

           /* Console.WriteLine("Periodo 0");
            Mostrar(rejilla);

           
            rejilla.EjecutarPeriodo();
            

            Console.WriteLine("Periodo 1");
            Mostrar(rejilla);

            rejilla.EjecutarPeriodo();

            Console.WriteLine("Periodo 2");
            Mostrar(rejilla);
*/
            string resultado = rejilla.Simular(100);
            Console.WriteLine("Resultado: " + resultado);


            Console.ReadLine();
    }

    static void Mostrar(Rejilla rejilla)
        {
            NodoCelda actual = rejilla.Celdas.Cabeza;

            while(actual !=null)
            {
                Console.WriteLine("Celda viva en: (" + actual.Fila + "," + actual.Columna + ")");
                actual = actual.Siguiente;
            }
            Console.WriteLine("------------------------");
        }
}
}