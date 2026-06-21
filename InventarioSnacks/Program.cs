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
                    MostrarProductos(); 
                    break;
                case 3: 
                    BuscarProducto(); 
                    break;
                case 4: 
                    ContarProductos(); 
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

    
    static void MostrarProductos()
    {
        if (!File.Exists(archivoTexto))
        {
            Console.WriteLine("No hay snacks registrados aun.");
            return;
        }

        Console.WriteLine("\n{0,-5} {1,-35} {2,-10} {3,-10} {4,-12}", "ID", "Producto / Descripcion", "Peso", "Stock", "Precio");
        Console.WriteLine(new string('-', 75));

        using (StreamReader sr = new StreamReader(archivoTexto, System.Text.Encoding.UTF8))
        {
            sr.ReadLine(); // Saltar encabezado
            string linea;
            while ((linea = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                ProductoInfo p = ParsearLinea(linea);
                Console.WriteLine("{0,-5} {1,-35} {2,-10} {3,-10} S/.{4,-10:F2}", p.Id, p.Nombre, p.Peso, p.Stock, p.Precio);
            }
        }
    }

    
    static void BuscarProducto()
    {
        if (!File.Exists(archivoTexto))
        {
            Console.WriteLine("No hay snacks registrados para buscar.");
            return;
        }

        Console.Write("Nombre del snack a buscar: ");
        string buscar = Console.ReadLine().ToLower();
        bool encontrado = false;

        using (StreamReader sr = new StreamReader(archivoTexto, System.Text.Encoding.UTF8))
        {
            sr.ReadLine(); 
            string linea;
            while ((linea = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                ProductoInfo p = ParsearLinea(linea);
                
               
                if (p.Nombre.ToLower().Contains(buscar))
                {
                    Console.WriteLine($"✔ Encontrado → ID:{p.Id} | {p.Nombre} ({p.Peso}) | Stock:{p.Stock} | S/.{p.Precio:F2}");
                    encontrado = true;
                }
            }
        }
        if (!encontrado) Console.WriteLine("No se encontro ningun snack con ese nombre.");
    }

    
    static void ContarProductos()
    {
        int total = ContarLineas();
        
        int totalSnacks = total > 0 ? total - 1 : 0;
        Console.WriteLine($"Total de snacks registrados en el CSV: {totalSnacks}");
    }

   
    static ProductoInfo ParsearLinea(string linea)
    {
        string[] partes = linea.Split(';');
        return new ProductoInfo
        {
            Id = int.Parse(partes[0]),
            Nombre = partes[1],
            Peso = partes[2],
            Stock = int.Parse(partes[3]),
            Precio = double.Parse(partes[4])
        };
    }
}