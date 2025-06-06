using System;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.Write("Введите количество строк: ");
        int rows;
        while (!int.TryParse(Console.ReadLine(), out rows) || rows <= 0)
        {
            Console.Write("Некорректный ввод. Введите целое положительное число: ");
        }

        Console.Write("Введите количество столбцов: ");
        int cols;
        while (!int.TryParse(Console.ReadLine(), out cols) || cols <= 0)
        {
            Console.Write("Некорректный ввод. Введите целое положительное число: ");
        }

        int[,] array = new int[rows, cols];
        Random random = new Random();

        // Заполняем массив случайными числами и выводим его
        Console.WriteLine("\nСгенерированный массив:");
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                array[i, j] = random.Next(1, 100); // Диапазон 1-99
                Console.Write(array[i, j] + "\t");
            }
            Console.WriteLine();
        }

        // Преобразуем массив в одномерный для использования LINQ
        var flatArray = array.Cast<int>();

        // Подсчет четных и нечетных чисел с использованием лямбда-выражений
        int evenCount = flatArray.Count(n => n % 2 == 0);
        int oddCount = flatArray.Count(n => n % 2 != 0);

        Console.WriteLine($"\nКоличество четных чисел: {evenCount}");
        Console.WriteLine($"Количество нечетных чисел: {oddCount}");
    }
}
