using System;
using System.IO;

class Program
{
    static void Main()
    {
        // Nombre del archivo CSV para el grupo
        string rutaArchivo = "productos.csv";

        // Creamos el archivo asegurando codificación UTF-8 para las tildes
        using (StreamWriter archivo = new StreamWriter(rutaArchivo, false, System.Text.Encoding.UTF8))
        {
            // Encabezado exacto pedido por tu equipo
            archivo.WriteLine("Producto;peso;stock;Precio");

            // Filas de datos basadas en la lista del grupo
            archivo.WriteLine("Snack deshidratado de pollo;45g;50;12.00");
            archivo.WriteLine("Snack deshidratado de pollo;90g;50;24.00");
            archivo.WriteLine("Snack deshidratado de pavo;45g;40;13.00");
            archivo.WriteLine("Snack deshidratado de pavo;90g;40;26.00");
            archivo.WriteLine("Snack deshidratado de caballo;45g;30;12.00");
            archivo.WriteLine("Snack deshidratado de caballo;90g;30;24.00");
            archivo.WriteLine("Snack deshidratado de higado de res;45g;25;11.00");
            archivo.WriteLine("Snack deshidratado de higado de res;90g;25;22.00");
            archivo.WriteLine("Snack deshidratado de pulmon de res;45g;35;10.00");
            archivo.WriteLine("Snack deshidratado de pulmon de res;90g;35;20.00");
            archivo.WriteLine("Snack deshidratado de pulmon de caballo;45g;20;10.00");
            archivo.WriteLine("Snack deshidratado de pulmon de caballo;90g;20;20.00");
            archivo.WriteLine("Snack deshidratado de bazo de res;45g;15;10.00");
            archivo.WriteLine("Snack deshidratado de bazo de res;90g;15;20.00");
            archivo.WriteLine("Patas de pollo;100g;60;9.00");
            archivo.WriteLine("Patas de pollo;200g;60;18.00");
            archivo.WriteLine("Patas de pavo;100g;45;13.00");
            archivo.WriteLine("Patas de pavo;200g;45;26.00");
        }

        Console.WriteLine("¡Archivo productos.csv creado exitosamente con la estructura del equipo!");
    }
}