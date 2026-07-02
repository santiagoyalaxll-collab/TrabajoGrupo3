using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

class ProductoInfo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Peso { get; set; } = "";
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
    static string archivoBinario = "productos.dat";

    static void Main()
    {
        int opcion;

        do
        {
            Console.Clear();

            Console.WriteLine("===== INVENTARIO DE SNACKS MASCOTAS =====");
            Console.WriteLine("1. Agregar un snack manualmente");
            Console.WriteLine("2. Mostrar todos los snacks (CSV)");
            Console.WriteLine("3. Buscar snack por nombre");
            Console.WriteLine("4. Contar total de snacks");
            Console.WriteLine("5. Guardar todo en archivo binario (.dat)");
            Console.WriteLine("6. Leer desde archivo binario (.dat)");
            Console.WriteLine("7. Inicializar/Precargar lista oficial");
            Console.WriteLine("8. Modificar un snack por ID");
            Console.WriteLine("9. Eliminar un snack por ID");
            Console.WriteLine("10. Cambiar estado Disponible/No Disponible");
            Console.WriteLine("0. Salir");
            Console.WriteLine();

            Console.Write("Elige una opción: ");

            if (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.WriteLine("Opción inválida.");
                Console.ReadKey();
                continue;
            }

            Console.Clear();

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

                case 5:
                    GuardarBinario();
                    break;

                case 6:
                    LeerBinario();
                    break;

                case 7:
                    PrecargarSnacksEquipo();
                    break;

                case 8:
                    ModificarProducto();
                    break;

                case 9:
                    EliminarProducto();
                    break;

                case 10:
                    CambiarEstadoProducto();
                    break;

                case 0:
                    Console.WriteLine("Hasta luego.");
                    break;

                default:
                    Console.WriteLine("Opción incorrecta.");
                    break;
            }

            if (opcion != 0)
            {
                Console.WriteLine();
                Console.WriteLine("Presiona una tecla para continuar...");
                Console.ReadKey();
            }

        } while (opcion != 0);
    }
    static void AgregarProducto()
{
    Console.Write("Nombre del snack: ");
    string nombre = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Peso (ej. 45gr): ");
    string peso = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Stock: ");
    if (!int.TryParse(Console.ReadLine(), out int stock))
    {
        Console.WriteLine("Stock inválido.");
        return;
    }

    Console.Write("Precio: ");
    if (!double.TryParse(Console.ReadLine(), out double precio))
    {
        Console.WriteLine("Precio inválido.");
        return;
    }

    List<ProductoInfo> lista = LeerTodos();

    int nuevoId = lista.Count == 0 ? 1 : lista.Max(p => p.Id) + 1;

    lista.Add(new ProductoInfo(nuevoId, nombre, peso, stock, precio, true));

    using (StreamWriter sw = new StreamWriter(archivoTexto, false))
    {
        sw.WriteLine("ID;Producto;peso;stock;Precio;Disponible");

        foreach (var p in lista)
        {
            sw.WriteLine($"{p.Id};{p.Nombre};{p.Peso};{p.Stock};{p.Precio:F2};{p.Disponible}");
        }
    }

    Console.WriteLine("Snack agregado correctamente.");
}

static void MostrarProductos()
{
    List<ProductoInfo> lista = LeerTodos();

    if (lista.Count == 0)
    {
        Console.WriteLine("No hay productos.");
        return;
    }

    Console.WriteLine();

    Console.WriteLine("{0,-5}{1,-25}{2,-10}{3,-8}{4,-10}{5}",
        "ID", "PRODUCTO", "PESO", "STOCK", "PRECIO", "ESTADO");

    Console.WriteLine(new string('-', 75));

    foreach (var p in lista)
    {
        Console.WriteLine("{0,-5}{1,-25}{2,-10}{3,-8}S/.{4,-8:F2}{5}",
            p.Id,
            p.Nombre,
            p.Peso,
            p.Stock,
            p.Precio,
            p.Disponible ? "Disponible" : "No Disponible");
    }
}

static void BuscarProducto()
{
    List<ProductoInfo> lista = LeerTodos();

    if (lista.Count == 0)
    {
        Console.WriteLine("No existen productos.");
        return;
    }

    Console.Write("Ingrese el nombre del snack: ");

    string buscar = Console.ReadLine()?.Trim().ToLower() ?? "";

    var resultados = lista.Where(x =>
        x.Nombre.ToLower().Contains(buscar)).ToList();

    if (resultados.Count == 0)
    {
        Console.WriteLine("No se encontró ningún producto.");
        return;
    }

    foreach (var p in resultados)
    {
        Console.WriteLine($"{p.Id} - {p.Nombre} - {p.Peso} - Stock:{p.Stock} - S/.{p.Precio:F2}");
    }
}

static void ContarProductos()
{
    Console.WriteLine($"Total de snacks: {LeerTodos().Count}");
}
static void GuardarBinario()
{
    List<ProductoInfo> lista = LeerTodos();

    if (lista.Count == 0)
    {
        Console.WriteLine("No existen productos para guardar.");
        return;
    }

    using (BinaryWriter bw = new BinaryWriter(File.Open(archivoBinario, FileMode.Create)))
    {
        bw.Write(lista.Count);

        foreach (var p in lista)
        {
            bw.Write(p.Id);
            bw.Write(p.Nombre);
            bw.Write(p.Peso);
            bw.Write(p.Stock);
            bw.Write(p.Precio);
            bw.Write(p.Disponible);
        }
    }

    Console.WriteLine("Archivo binario guardado correctamente.");
}

static void LeerBinario()
{
    if (!File.Exists(archivoBinario))
    {
        Console.WriteLine("No existe el archivo binario.");
        return;
    }

    using (BinaryReader br = new BinaryReader(File.Open(archivoBinario, FileMode.Open)))
    {
        int total = br.ReadInt32();

        Console.WriteLine("\nARCHIVO BINARIO");
        Console.WriteLine(new string('-', 80));

        for (int i = 0; i < total; i++)
        {
            int id = br.ReadInt32();
            string nombre = br.ReadString();
            string peso = br.ReadString();
            int stock = br.ReadInt32();
            double precio = br.ReadDouble();
            bool disponible = br.ReadBoolean();

            Console.WriteLine($"{id} - {nombre} - {peso} - Stock:{stock} - S/.{precio:F2} - {(disponible ? "Disponible" : "No Disponible")}");
        }
    }
}

static ProductoInfo? ParsearLinea(string linea)
{
    if (string.IsNullOrWhiteSpace(linea))
        return null;

    string[] partes = linea.Split(';');

    if (partes.Length < 5)
        return null;

    ProductoInfo producto = new ProductoInfo();

    producto.Id = int.TryParse(partes[0], out int id) ? id : 0;
    producto.Nombre = partes[1];
    producto.Peso = partes[2];
    producto.Stock = int.TryParse(partes[3], out int stock) ? stock : 0;
    producto.Precio = double.TryParse(partes[4], out double precio) ? precio : 0;

    if (partes.Length >= 6)
        producto.Disponible = bool.TryParse(partes[5], out bool estado)
            ? estado
            : true;
    else
        producto.Disponible = true;

    return producto;
}

static int ContarLineas()
{
    if (!File.Exists(archivoTexto))
        return 0;

    return File.ReadAllLines(archivoTexto).Length;
}

static List<ProductoInfo> LeerTodos()
{
    List<ProductoInfo> lista = new List<ProductoInfo>();

    if (!File.Exists(archivoTexto))
        return lista;

    using (StreamReader sr = new StreamReader(archivoTexto))
    {
        sr.ReadLine();

        string? linea;

        while ((linea = sr.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(linea))
                continue;

            ProductoInfo? producto = ParsearLinea(linea);

            if (producto != null)
                lista.Add(producto);
        }
    }

    return lista;
}
static void ModificarProducto()
{
    List<ProductoInfo> lista = LeerTodos();

    if (lista.Count == 0)
    {
        Console.WriteLine("No existen productos.");
        return;
    }

    Console.Write("Ingrese el ID: ");

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }

    ProductoInfo? producto = lista.FirstOrDefault(p => p.Id == id);

    if (producto == null)
    {
        Console.WriteLine("Producto no encontrado.");
        return;
    }

    Console.WriteLine("\nPresiona ENTER para dejar el valor actual.\n");

    Console.Write($"Nombre ({producto.Nombre}): ");
    string nombre = Console.ReadLine() ?? "";
    if (!string.IsNullOrWhiteSpace(nombre))
        producto.Nombre = nombre;

    Console.Write($"Peso ({producto.Peso}): ");
    string peso = Console.ReadLine() ?? "";
    if (!string.IsNullOrWhiteSpace(peso))
        producto.Peso = peso;

    Console.Write($"Stock ({producto.Stock}): ");
    string stockTexto = Console.ReadLine() ?? "";
    if (int.TryParse(stockTexto, out int stock))
        producto.Stock = stock;

    Console.Write($"Precio ({producto.Precio:F2}): ");
    string precioTexto = Console.ReadLine() ?? "";
    if (double.TryParse(precioTexto, out double precio))
        producto.Precio = precio;

    GuardarCSV(lista);

    Console.WriteLine("\nProducto modificado correctamente.");
}

static void EliminarProducto()
{
    List<ProductoInfo> lista = LeerTodos();

    if (lista.Count == 0)
    {
        Console.WriteLine("No existen productos.");
        return;
    }

    Console.Write("Ingrese el ID del producto: ");

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }

    ProductoInfo? producto = lista.FirstOrDefault(p => p.Id == id);

    if (producto == null)
    {
        Console.WriteLine("Producto no encontrado.");
        return;
    }

    Console.Write($"¿Eliminar '{producto.Nombre}'? (S/N): ");

    string r = (Console.ReadLine() ?? "").ToUpper();

    if (r != "S")
    {
        Console.WriteLine("Operación cancelada.");
        return;
    }

    lista.Remove(producto);

    GuardarCSV(lista);

    Console.WriteLine("Producto eliminado correctamente.");
}

static void CambiarEstadoProducto()
{
    List<ProductoInfo> lista = LeerTodos();

    if (lista.Count == 0)
    {
        Console.WriteLine("No existen productos.");
        return;
    }

    Console.Write("Ingrese el ID: ");

    if (!int.TryParse(Console.ReadLine(), out int id))
    {
        Console.WriteLine("ID inválido.");
        return;
    }

    ProductoInfo? producto = lista.FirstOrDefault(p => p.Id == id);

    if (producto == null)
    {
        Console.WriteLine("Producto no encontrado.");
        return;
    }

    producto.Disponible = !producto.Disponible;

    GuardarCSV(lista);

    Console.WriteLine($"Nuevo estado: {(producto.Disponible ? "Disponible" : "No Disponible")}");
}

static void GuardarCSV(List<ProductoInfo> lista)
{
    using (StreamWriter sw = new StreamWriter(archivoTexto, false))
    {
        sw.WriteLine("ID;Producto;peso;stock;Precio;Disponible");

        foreach (var p in lista.OrderBy(x => x.Id))
        {
            sw.WriteLine($"{p.Id};{p.Nombre};{p.Peso};{p.Stock};{p.Precio:F2};{p.Disponible}");
        }
    }
}
static void PrecargarSnacksEquipo()
{
    List<ProductoInfo> lista = new List<ProductoInfo>()
    {
        new ProductoInfo(1,"Pollo","45gr",50,14.50,true),
        new ProductoInfo(2,"Pollo","90gr",0,19.80,false),
        new ProductoInfo(3,"Pollo","180gr",50,29.50,true),
        new ProductoInfo(4,"Pavo","45gr",50,15.50,true),
        new ProductoInfo(5,"Pavo","90gr",50,21.00,true),
        new ProductoInfo(6,"Pavo","180gr",50,31.50,true),
        new ProductoInfo(7,"Caballo","45gr",0,17.00,false),
        new ProductoInfo(8,"Caballo","90gr",50,24.50,true),
        new ProductoInfo(9,"Caballo","180gr",50,36.00,true),
        new ProductoInfo(10,"Llama","45gr",50,18.50,true),
        new ProductoInfo(11,"Llama","90gr",0,26.00,false),
        new ProductoInfo(12,"Llama","180gr",50,38.50,true),
        new ProductoInfo(13,"Pulmon-Res","45gr",50,13.50,true),
        new ProductoInfo(14,"Pulmon-Res","90gr",50,18.00,true),
        new ProductoInfo(15,"Pulmon-Res","180gr",50,27.00,true),
        new ProductoInfo(16,"Pulmon-Caballo","45gr",50,15.00,true),
        new ProductoInfo(17,"Pulmon-Caballo","90gr",0,21.00,false),
        new ProductoInfo(18,"Pulmon-Caballo","180gr",50,31.00,true),
        new ProductoInfo(19,"Bazo-Res","45gr",0,13.50,false),
        new ProductoInfo(20,"Bazo-Res","90gr",50,18.00,true),
        new ProductoInfo(21,"Bazo-Res","180gr",50,27.00,true),
        new ProductoInfo(22,"Higado-Res","45gr",50,14.00,true),
        new ProductoInfo(23,"Higado-Res","90gr",0,19.50,false),
        new ProductoInfo(24,"Higado-Res","180gr",50,28.50,true),
        new ProductoInfo(25,"Higado-Caballo","45gr",50,16.00,true),
        new ProductoInfo(26,"Higado-Caballo","90gr",0,23.00,false),
        new ProductoInfo(27,"Higado-Caballo","180gr",0,34.00,false),
        new ProductoInfo(28,"Corazon-Pollo","45gr",50,14.50,true),
        new ProductoInfo(29,"Corazon-Pollo","90gr",50,20.00,true),
        new ProductoInfo(30,"Corazon-Pollo","180gr",50,29.50,true),
        new ProductoInfo(31,"Corazon-Pavo","45gr",50,15.50,true),
        new ProductoInfo(32,"Corazon-Pavo","90gr",0,22.00,false),
        new ProductoInfo(33,"Corazon-Pavo","180gr",50,32.50,true),
        new ProductoInfo(34,"Molleja-Pollo","45gr",0,13.50,false),
        new ProductoInfo(35,"Molleja-Pollo","90gr",50,18.50,true),
        new ProductoInfo(36,"Molleja-Pollo","180gr",50,27.50,true),
        new ProductoInfo(37,"Patas-Pollo","100gr",50,19.00,true),
        new ProductoInfo(38,"Patas-Pollo","200gr",50,32.00,true),
        new ProductoInfo(39,"Patas-Pavo","100gr",50,21.00,true),
        new ProductoInfo(40,"Patas-Pavo","200gr",0,35.00,false),
        new ProductoInfo(41,"Mix-Pollo","45gr",50,16.50,true),
        new ProductoInfo(42,"Mix-Pollo","90gr",50,24.00,true),
        new ProductoInfo(43,"Mix-Pollo","180gr",50,35.50,true),
        new ProductoInfo(44,"Mix-Res","45gr",50,16.00,true),
        new ProductoInfo(45,"Mix-Res","90gr",50,23.50,true),
        new ProductoInfo(46,"Mix-Res","180gr",50,34.50,true),
        new ProductoInfo(47,"Mix-Pavo","180gr",0,37.00,false),
        new ProductoInfo(48,"Esofago-Res","100gr",50,20.00,true),
        new ProductoInfo(49,"Esofago-Res","200gr",50,33.50,true),
        new ProductoInfo(50,"Cuello-Pollo","100gr",50,18.00,true),
        new ProductoInfo(51,"Cuello-Pollo","200gr",50,30.00,true),
        new ProductoInfo(52,"Cuello-Pavo","100gr",0,20.50,false),
        new ProductoInfo(53,"Cuello-Pavo","200gr",50,33.00,true)
    };

    GuardarCSV(lista);

    Console.WriteLine("Lista de snacks cargada correctamente.");
}
}