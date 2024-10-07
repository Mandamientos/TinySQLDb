using System;
using System.Collections.Generic;

namespace QueryProcessing.Operations
{
    public class Nodo
    {
        public object Valor { get; set; }
        public List<string> Datos { get; set; }  // Campo actualizado a una lista de strings
        public Nodo Izquierda { get; set; }
        public Nodo Derecha { get; set; }

        // Constructor que acepta valor y lista de datos
        public Nodo(object valor, List<string> datos)
        {
            Valor = valor;
            Datos = datos ?? new List<string>();  // Almacena los datos adicionales en una lista
            Izquierda = null;
            Derecha = null;
        }
    }

    // Árbol Binario
    public class ArbolBinario
    {
        private Nodo raiz;
        private Type tipo; // To store the type (int or string)

        public ArbolBinario()
        {
            raiz = null;
            tipo = null; // Inicialmente no conocemos el tipo
        }

        // Insertar valores y datos
        public void Insertar(object valor, List<string> datos)
        {
            if (tipo == null) // Si el tipo no está configurado, determinarlo desde el primer valor
            {
                tipo = valor.GetType();
            }

            if (valor.GetType() != tipo)
            {
                throw new InvalidOperationException("All elements must be of the same type (either all int or all string).");
            }

            if (raiz == null)
            {
                raiz = new Nodo(valor, datos);  // Crea el nodo raíz con valor y lista de datos
            }
            else
            {
                InsertarRec(raiz, valor, datos);
            }
        }

        private void InsertarRec(Nodo nodo, object valor, List<string> datos)
        {
            if (Comparar(valor, nodo.Valor) < 0)
            {
                if (nodo.Izquierda == null)
                {
                    nodo.Izquierda = new Nodo(valor, datos);  // Inserta nodo a la izquierda con lista de datos
                }
                else
                {
                    InsertarRec(nodo.Izquierda, valor, datos);
                }
            }
            else
            {
                if (nodo.Derecha == null)
                {
                    nodo.Derecha = new Nodo(valor, datos);  // Inserta nodo a la derecha con lista de datos
                }
                else
                {
                    InsertarRec(nodo.Derecha, valor, datos);
                }
            }
        }

        // Método para comparar dos objetos del mismo tipo
        private int Comparar(object valor1, object valor2)
        {
            if (tipo == typeof(int))
            {
                return ((int)valor1).CompareTo((int)valor2);
            }
            else if (tipo == typeof(string))
            {
                return ((string)valor1).CompareTo((string)valor2);
            }

            return 0; // Este caso no debería suceder, ya que siempre tendremos el mismo tipo
        }

        // Recorrido inorden (muestra valor y lista de datos)
        public void RecorridoInorden()
        {
            RecorrerInordenRec(raiz);
            Console.WriteLine();
        }

        private void RecorrerInordenRec(Nodo nodo)
        {
            if (nodo != null)
            {
                RecorrerInordenRec(nodo.Izquierda);

                // Muestra el valor y cada dato de la lista
                Console.WriteLine($"Valor: {nodo.Valor}, Datos: {string.Join(", ", nodo.Datos)}");

                RecorrerInordenRec(nodo.Derecha);
            }
        }
    }
}
