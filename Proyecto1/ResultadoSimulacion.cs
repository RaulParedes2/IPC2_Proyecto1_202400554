using System;

namespace IPC2_Proyecto1
{
    public class ResultadoSimulacion
        {
            private string tipo;
            private int n;
            private int n1;

            public string Tipo
        {
            set
            {
                tipo = value;
            }

            get
            {
               return tipo; 

            }
        }

        public int N
        {
            set
            {
                n = value;
            }

            get
            {
                return n;
            }
        }

        public int N1
        {
            set
            {
                n1=value;
            }
            get
            {
                return n1 ;
            }
        }

            
        }
}