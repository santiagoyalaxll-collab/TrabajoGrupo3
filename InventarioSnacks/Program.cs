using System;
using System.IO;
using System.Collections.Generic;

class ProductoInfo
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Peso { get; set; }
    public int Stock { get; set; }
    public double Precio { get; set; }

    public ProductoInfo() { }

    public ProductoInfo(int id, string nombre, string peso, int stock, double precio)
    {
        Id = id;
        Nombre = nombre;
        Peso = peso;
        Stock = stock;
        Precio = precio;
    }
}

class Program
{
    static string archivoTexto = "productos.csv";

    static void Main()
    {
        int opcion;
        do
        {
            Console.WriteLine("\n===== INVENTARIO DE SNACKS MASCOTAS =====");
            Console.WriteLine("1. Agregar un snack manualmente");
            Console.WriteLine("2. Mostrar todos los snacks (CSV)");
            Console.WriteLine("3. Buscar snack por nombre");
            Console.WriteLine("4. Contar total de snacks");
            Console.WriteLine("0. Salir");
            Console.Write("Elige una opcion: ");
            
            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Por favor, ingresa un numero valido.");
                opcion = -1;
                continue;
            }

            switch (opcion)
            {
                case 1: 
                    AgregarProducto(); 
                    break;
                case 2: 
                    Console.WriteLine("..."); 
                    break;
                case 3: 
                    Console.WriteLine("..."); 
                    break;
                case 4: 
                    Console.WriteLine("..."); 
                    break;
                case 0: 
                    Console.WriteLine("Hasta luego!"); 
                    break;
                default: 
                    Console.WriteLine("Opcion no valida."); 
                    break;
            }
        } while (opcion != 0);
    }

    
    static void AgregarProducto()
    {
        Console.Write("Nombre del snack: ");
        string nombre = Console.ReadLine();

        Console.Write("Peso (ej. 45g): ");
        string peso = Console.ReadLine();

        Console.Write("Stock disponible: ");
        int stock = int.Parse(Console.ReadLine());

        Console.Write("Precio (S/.): ");
        double precio = double.Parse(Console.ReadLine());

        int id = ContarLineas() == 0 ? 1 : ContarLineas();
        bool existeArchivo = File.Exists(archivoTexto);

        using (StreamWriter sw = new StreamWriter(archivoTexto, append: true, System.Text.Encoding.UTF8))
        {
            if (!existeArchivo)
            {
                sw.WriteLine("ID;Producto;peso;stock;Precio");
            }
            sw.WriteLine($"{id};{nombre};{peso};{stock};{precio:F2}");
        }

        Console.WriteLine($"✔ Snack '{nombre} ({peso})' guardado con ID {id}.");
    }

    static int ContarLineas()
    {
        if (!File.Exists(archivoTexto)) return 0;
        return File.ReadAllLines(archivoTexto).Length;
    }
}