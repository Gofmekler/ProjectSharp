using System;
using System.Linq;

class Program
{
    static void Main()
    {
        int[] numbers = Enumerable.Range(1, 10).ToArray();

        Console.WriteLine("Сумма квадратов четных чисел различными способами:");

        // 1. Использование цикла for
        int sumFor = 0;
        for (int i = 2; i <= 10; i += 2)
        {
            sumFor += i * i;
        }
        Console.WriteLine($"Цикл for: {sumFor}");

        // 2. Использование foreach
        int sumForeach = 0;
        foreach (int num in numbers)
        {
            if (num % 2 == 0)
            {
                sumForeach += num * num;
            }
        }
        Console.WriteLine($"Цикл foreach: {sumForeach}");

        // 3. Операторы запросов LINQ
        int sumLinqQuery = (from num in numbers
                            where num % 2 == 0
                            select num * num).Sum();
        Console.WriteLine($"Запрос LINQ: {sumLinqQuery}");

        // 4. Методы расширения LINQ
        int sumLinqMethod = numbers.Where(num => num % 2 == 0)
                                   .Select(num => num * num)
                                   .Sum();
        Console.WriteLine($"Методы расширения LINQ: {sumLinqMethod}");
    }
}
