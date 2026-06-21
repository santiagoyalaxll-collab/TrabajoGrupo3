using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
    static string archivoBinario = "productos.dat";

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
            Console.WriteLine("5. Guardar todo en archivo binario (.dat)");
            Console.WriteLine("6. Leer desde archivo binario (.dat)");
            Console.WriteLine("7. Inicializar/Precargar lista oficial de snacks de tu equipo");
            Console.WriteLine("8. Modificar/Editar un snack por ID");
            Console.WriteLine("9. Eliminar Snack por ID");
            Console.WriteLine("0. Salir");
            Console.Write("Elige una opción: ");
            
            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Por favor, ingresa un número válido.");
                opcion = -1;
                continue;
            }

            switch (opcion)
            {
                case 1: AgregarProducto(); break;
                case 2: MostrarProductos(); break;
                case 3: BuscarProducto(); break;
                case 4: ContarProductos(); break;
                case 5: GuardarBinario(); break;
                case 6: LeerBinario(); break;
                case 7: PrecargarSnacksEquipo(); break;
                case 8: ModificarProducto(); break; 
                case 9: EliminarProducto();break;              
                case 0: Console.WriteLine("¡Hasta luego!"); break;
                default: Console.WriteLine("Opción no válida."); break;
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
                sw.WriteLine("ID;Producto;peso;stock;Precio");
            }
            sw.WriteLine($"{id};{nombre};{peso};{stock};{precio:F2}");
        }

        Console.WriteLine($"✔ Snack '{nombre} ({peso})' guardado con ID {id}.");
    }

    // CONTROL 2: Mostrar contenido del CSV ordenado en tabla
    static void MostrarProductos()
    {
        if (!File.Exists(archivoTexto))
        {
            Console.WriteLine("No hay snacks registrados aún. Prueba la opción 7 para cargar la lista oficial.");
            return;
        }

        Console.WriteLine("\n{0,-5} {1,-45} {2,-10} {3,-10} {4,-10}", "ID", "Producto / Descripción", "Peso", "Stock", "Precio");
        Console.WriteLine(new string('-', 85));

        using (StreamReader sr = new StreamReader(archivoTexto, System.Text.Encoding.UTF8))
        {
            sr.ReadLine(); // Saltar encabezado
            string linea;
            while ((linea = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                ProductoInfo p = ParsearLinea(linea);
                Console.WriteLine("{0,-5} {1,-45} {2,-10} {3,-10} S/.{4,-10:F2}", p.Id, p.Nombre, p.Peso, p.Stock, p.Precio);
            }
        }
    } 

    // CONTROL 3: Buscar por nombre
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
            sr.ReadLine(); // Saltar encabezado
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
        if (!encontrado) Console.WriteLine("No se encontró ningún snack con ese nombre.");
    }

    // CONTROL 4: Contar registros reales
    static void ContarProductos()
    {
        int total = ContarLineas();
        int totalSnacks = total > 0 ? total - 1 : 0;
        Console.WriteLine($"Total de snacks registrados en el CSV: {totalSnacks}");
    }

    // CONTROL 5: Exportar CSV a Binario (.dat)
    static void GuardarBinario()
    {
        List<ProductoInfo> lista = LeerTodos();
        if (lista.Count == 0)
        {
            Console.WriteLine("No hay datos en el archivo CSV para exportar.");
            return;
        }

        using (BinaryWriter bw = new BinaryWriter(File.Open(archivoBinario, FileMode.Create)))
        {
            bw.Write(lista.Count);
            foreach (ProductoInfo p in lista)
            {
                bw.Write(p.Id);
                bw.Write(p.Nombre);
                bw.Write(p.Peso);
                bw.Write(p.Stock);
                bw.Write(p.Precio);
            }
        }
        Console.WriteLine($"✔ {lista.Count} snack(s) exportado(s) a archivo binario '{archivoBinario}'.");
    }

    // CONTROL 6: Leer Binario (.dat)
    static void LeerBinario()
    {
        if (!File.Exists(archivoBinario))
        {
            Console.WriteLine("No existe el archivo binario. Ejecuta primero la opción 5.");
            return;
        }

        using (BinaryReader br = new BinaryReader(File.Open(archivoBinario, FileMode.Open)))
        {
            int total = br.ReadInt32();
            Console.WriteLine($"\nSnacks leídos desde archivo binario ({total}):");
            Console.WriteLine(new string('-', 85));

            for (int i = 0; i < total; i++)
            {
                int id = br.ReadInt32();
                string nombre = br.ReadString();
                string peso = br.ReadString();
                int stock = br.ReadInt32();
                double precio = br.ReadDouble();
                
                Console.WriteLine($"ID:{id} | {nombre,-40} | {peso,-5} | Stock:{stock,-4} | S/.{precio:F2}");
            }
        }
    }

    // CONTROL 7: BOTÓN AUTOMÁTICO - Carga tu lista original del CSV y la ordena por Nombre
    static void PrecargarSnacksEquipo()
    {
        Console.WriteLine("Cargando la lista de productos de tu equipo...");

        List<ProductoInfo> snacksIniciales = new List<ProductoInfo>()
        {
            new ProductoInfo(0, "Snack deshidratado de pollo", "45g", 50, 12.00),
            new ProductoInfo(0, "Snack deshidratado de pollo", "90g", 50, 24.00),
            new ProductoInfo(0, "Snack deshidratado de pavo", "45g", 40, 13.00),
            new ProductoInfo(0, "Snack deshidratado de pavo", "90g", 40, 26.00),
            new ProductoInfo(0, "Snack deshidratado de caballo", "45g", 30, 12.00),
            new ProductoInfo(0, "Snack deshidratado de caballo", "90g", 30, 24.00),
            new ProductoInfo(0, "Snack deshidratado de higado de res", "45g", 25, 11.00),
            new ProductoInfo(0, "Snack deshidratado de higado de res", "90g", 25, 22.00),
            new ProductoInfo(0, "Snack deshidratado de pulmon de res", "45g", 35, 10.00),
            new ProductoInfo(0, "Snack deshidratado de pulmon de res", "90g", 35, 20.00),
            new ProductoInfo(0, "Snack deshidratado de pulmon de caballo", "45g", 20, 10.00),
            new ProductoInfo(0, "Snack deshidratado de pulmon de caballo", "90g", 20, 20.00),
            new ProductoInfo(0, "Snack deshidratado de bazo de res", "45g", 15, 10.00),
            new ProductoInfo(0, "Snack deshidratado de bazo de res", "90g", 15, 20.00),
            new ProductoInfo(0, "Patas de pollo", "100g", 60, 9.00),
            new ProductoInfo(0, "Patas de pollo", "200g", 60, 18.00),
            new ProductoInfo(0, "Patas de pavo", "100g", 45, 13.00),
            new ProductoInfo(0, "Patas de pavo", "200g", 45, 26.00)
        };

        var listaOrdenada = snacksIniciales.OrderBy(p => p.Nombre).ToList();

        for (int i = 0; i < listaOrdenada.Count; i++)
        {
            listaOrdenada[i].Id = i + 1;
        }

        using (StreamWriter archivo = new StreamWriter(archivoTexto, false, System.Text.Encoding.UTF8))
        {
            archivo.WriteLine("ID;Producto;peso;stock;Precio");
            foreach (var prod in listaOrdenada)
            {
                archivo.WriteLine($"{prod.Id};{prod.Nombre};{prod.Peso};{prod.Stock};{prod.Precio:F2}");
            }
        }

        Console.WriteLine("✔ ¡El archivo 'productos.csv' ha sido creado, rellenado y ORDENADO automáticamente!");
    }

    // Funciones auxiliares de parseo y conteo
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

    static int ContarLineas()
    {
        if (!File.Exists(archivoTexto)) return 0;
        return File.ReadAllLines(archivoTexto).Length;
    }

    static List<ProductoInfo> LeerTodos()
    {
        var lista = new List<ProductoInfo>();
        if (!File.Exists(archivoTexto)) return lista;

        using (StreamReader sr = new StreamReader(archivoTexto, System.Text.Encoding.UTF8))
        {
            sr.ReadLine(); // Saltar cabecera
            string linea;
            while ((linea = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                lista.Add(ParsearLinea(linea));
            }
        }
        return lista;
    }

    // CONTROL 8: Modificar/Editar un snack por ID
    static void ModificarProducto()
    {
        if (!File.Exists(archivoTexto))
        {
            Console.WriteLine("No hay archivo CSV para modificar productos.");
            return;
        }

        Console.Write("Ingresa el ID del snack que deseas modificar: ");
        if (!int.TryParse(Console.ReadLine(), out int idBuscar))
        {
            Console.WriteLine("ID no válido.");
            return;
        }

        List<ProductoInfo> lista = LeerTodos();
        ProductoInfo productoAEditar = lista.Find(p => p.Id == idBuscar);

        if (productoAEditar == null)
        {
            Console.WriteLine($"❌ No se encontró ningún snack con el ID {idBuscar}.");
            return;
        }

        Console.WriteLine($"\nSnack seleccionado: {productoAEditar.Nombre} ({productoAEditar.Peso})");
        Console.WriteLine("--- Ingresa los nuevos datos ---");

        Console.Write("Nuevo Nombre (presiona Enter para mantener el actual): ");
        string nuevoNombre = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nuevoNombre)) productoAEditar.Nombre = nuevoNombre;

        Console.Write("Nuevo Peso (presiona Enter para mantener el actual): ");
        string nuevoPeso = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nuevoPeso)) productoAEditar.Peso = nuevoPeso;

        Console.Write("Nuevo Stock (o escribe -1 para mantener el actual): ");
        if (int.TryParse(Console.ReadLine(), out int nuevoStock) && nuevoStock != -1) 
            productoAEditar.Stock = nuevoStock;

        Console.Write("Nuevo Precio (o escribe -1 para mantener el actual): ");
        if (double.TryParse(Console.ReadLine(), out double nuevoPrecio) && nuevoPrecio != -1) 
            productoAEditar.Precio = nuevoPrecio;

        using (StreamWriter archivo = new StreamWriter(archivoTexto, false, System.Text.Encoding.UTF8))
        {
            archivo.WriteLine("ID;Producto;peso;stock;Precio");
            foreach (var prod in lista)
            {
                archivo.WriteLine($"{prod.Id};{prod.Nombre};{prod.Peso};{prod.Stock};{prod.Precio:F2}");
            }
        }

        Console.WriteLine("✔ ¡Snack modified y guardado en el CSV con éxito!");
    }

    // CONTROL 9: Eliminar un snack existente por ID
    static void EliminarProducto()
    {
        if (!File.Exists(archivoTexto))
        {
            Console.WriteLine("No hay archivo CSV para eliminar productos.");
            return;
        }

        Console.Write("Ingresa el ID del snack que deseas ELIMINAR: ");
        if (!int.TryParse(Console.ReadLine(), out int idBuscar))
        {
            Console.WriteLine("ID no válido.");
            return;
        }

        List<ProductoInfo> lista = LeerTodos();
        ProductoInfo productoAEliminar = lista.Find(p => p.Id == idBuscar);

        if (productoAEliminar == null)
        {
            Console.WriteLine($"❌ No se encontró ningún snack con el ID {idBuscar}.");
            return;
        }

        Console.WriteLine($"\n⚠ ¿Estás seguro de que deseas eliminar el snack: {productoAEliminar.Nombre} ({productoAEliminar.Peso})?");
        Console.Write("Escribe 'S' para confirmar o cualquier otra tecla para cancelar: ");
        string confirmacion = Console.ReadLine()?.Trim().ToUpper();

        if (confirmacion != "S")
        {
            Console.WriteLine("❌ Operación cancelada. El snack NO fue eliminado.");
            return;
        }

        lista.Remove(productoAEliminar);

        using (StreamWriter archivo = new StreamWriter(archivoTexto, false, System.Text.Encoding.UTF8))
        {
            archivo.WriteLine("ID;Producto;peso;stock;Precio");
            foreach (var prod in lista)
            {
                archivo.WriteLine($"{prod.Id};{prod.Nombre};{prod.Peso};{prod.Stock};{prod.Precio:F2}");
            }
        }

        Console.WriteLine("✔ ¡Snack eliminado del archivo CSV con éxito!");
    }
}