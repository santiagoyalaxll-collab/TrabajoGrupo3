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
    public bool Disponible { get; set; } 

    public ProductoInfo() { }

    public ProductoInfo(int id, string nombre, string peso, int stock, double precio, bool disponible = true)
    {
        Id = id;
        Nombre = nombre;
        Peso = peso;
        Stock = stock;
        Precio = precio;
        Disponible = disponible;
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
            Console.WriteLine("8. Modificar/Editar un snack por ID");
            Console.WriteLine("9. Eliminar un snack por ID");
            Console.WriteLine("10. Cambiar estado (Disponible / No disponible) de un snack");
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
                case 1: AgregarProducto(); break;
                case 2: MostrarProductos(); break;
                case 3: BuscarProducto(); break;
                case 4: ContarProductos(); break;
                case 8: ModificarProducto(); break;
                case 9: EliminarProducto(); break;
                case 10: CambiarEstadoProducto(); break; // Llama al módulo definitivo
                case 0: Console.WriteLine("Hasta luego!"); break;
                default: Console.WriteLine("Opcion no valida."); break;
            }
        } while (opcion != 0);
    }

    
   // CONTROL 1: Agregar snack manual
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
                sw.WriteLine("ID;Producto;peso;stock;Precio;Disponible");
            }
            sw.WriteLine($"{id};{nombre};{peso};{stock};{precio:F2};True");
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

        Console.WriteLine("\n{0,-5} {1,-35} {2,-10} {3,-10} {4,-12} {5,-15}", "ID", "Producto / Descripcion", "Peso", "Stock", "Precio", "Estado");
        Console.WriteLine(new string('-', 95));

        using (StreamReader sr = new StreamReader(archivoTexto, System.Text.Encoding.UTF8))
        {
            sr.ReadLine(); // Saltar encabezado
            string linea;
            while ((linea = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                ProductoInfo p = ParsearLinea(linea);
                string estadoStr = p.Disponible ? "Disponible" : "No disponible";
                Console.WriteLine("{0,-5} {1,-35} {2,-10} {3,-10} S/.{4,-10:F2} {5,-15}", p.Id, p.Nombre, p.Peso, p.Stock, p.Precio, estadoStr);
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
                    string estadoStr = p.Disponible ? "Disponible" : "No disponible";
                    Console.WriteLine($"✔ Encontrado → ID:{p.Id} | {p.Nombre} ({p.Peso}) | Stock:{p.Stock} | S/.{p.Precio:F2} | {estadoStr}");
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

    // CONTROL 8: Modificar un snack por ID
    static void ModificarProducto()
    {
        if (!File.Exists(archivoTexto))
        {
            Console.WriteLine("No hay archivo CSV para modificar productos.");
            return;
        }

        Console.Write("Ingresa el ID del snack que deseas editar: ");
        if (!int.TryParse(Console.ReadLine(), out int idBuscar))
        {
            Console.WriteLine("ID no valido.");
            return;
        }

        List<ProductoInfo> lista = LeerTodos();
        ProductoInfo producto = lista.Find(p => p.Id == idBuscar);

        if (producto == null)
        {
            Console.WriteLine($"No se encontro ningún snack con el ID {idBuscar}.");
            return;
        }

        Console.WriteLine($"Editando: {producto.Nombre} ({producto.Peso})");
        
        Console.Write("Nuevo Nombre (deja vacio para mantener actual): ");
        string nuevoNombre = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nuevoNombre)) producto.Nombre = nuevoNombre;

        Console.Write("Nuevo Stock: ");
        producto.Stock = int.Parse(Console.ReadLine());

        Console.Write("Nuevo Precio (S/.): ");
        producto.Precio = double.Parse(Console.ReadLine());

        GuardarLista(lista);
        Console.WriteLine("✔ El producto fue modificado correctamente en el archivo CSV.");
    }

    
    static void EliminarProducto()
    {
        if (!File.Exists(archivoTexto))
        {
            Console.WriteLine("No hay archivo CSV para eliminar productos.");
            return;
        }

        Console.Write("Ingresa el ID del snack que deseas eliminar: ");
        if (!int.TryParse(Console.ReadLine(), out int idBuscar))
        {
            Console.WriteLine("ID no valido.");
            return;
        }

        List<ProductoInfo> lista = LeerTodos();
        ProductoInfo producto = lista.Find(p => p.Id == idBuscar);

        if (producto == null)
        {
            Console.WriteLine($"No se encontro ningún snack con el ID {idBuscar}.");
            return;
        }

        lista.Remove(producto);
        GuardarLista(lista);
        Console.WriteLine($"✔ El producto '{producto.Nombre}' fue eliminado y el archivo CSV se ha actualizado.");
    }

    
    static void CambiarEstadoProducto()
    {
        if (!File.Exists(archivoTexto))
        {
            Console.WriteLine("No hay archivo CSV para cambiar el estado de productos.");
            return;
        }

        Console.Write("Ingresa el ID del snack al que deseas cambiar el estado: ");
        if (!int.TryParse(Console.ReadLine(), out int idBuscar))
        {
            Console.WriteLine("ID no valido.");
            return;
        }

        List<ProductoInfo> lista = LeerTodos();
        ProductoInfo producto = lista.Find(p => p.Id == idBuscar);

        if (producto == null)
        {
            Console.WriteLine($"No se encontro ningún snack con el ID {idBuscar}.");
            return;
        }

        string estadoAnterior = producto.Disponible ? "Disponible" : "No disponible";
        producto.Disponible = !producto.Disponible; // Operación de inversión lógica (Toggle)
        string estadoNuevo = producto.Disponible ? "Disponible" : "No disponible";

        GuardarLista(lista);
        Console.WriteLine($"✔ Estado de '{producto.Nombre}' cambiado de '{estadoAnterior}' a '{estadoNuevo}'.");
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
            Precio = double.Parse(partes[4]),
            Disponible = partes.Length > 5 ? bool.Parse(partes[5]) : true
        };
    }

    static List<ProductoInfo> LeerTodos()
    {
        List<ProductoInfo> lista = new List<ProductoInfo>();
        if (!File.Exists(archivoTexto)) return lista;

        using (StreamReader sr = new StreamReader(archivoTexto, System.Text.Encoding.UTF8))
        {
            sr.ReadLine(); 
            string linea;
            while ((linea = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                lista.Add(ParsearLinea(linea));
            }
        }
        return lista;
    }

    static void GuardarLista(List<ProductoInfo> lista)
    {
        using (StreamWriter sw = new StreamWriter(archivoTexto, append: false, System.Text.Encoding.UTF8))
        {
            sw.WriteLine("ID;Producto;peso;stock;Precio;Disponible");
            foreach (var p in lista)
            {
                sw.WriteLine($"{p.Id};{p.Nombre};{p.Peso};{p.Stock};{p.Precio:F2};{p.Disponible}");
            }
        }
    }
}