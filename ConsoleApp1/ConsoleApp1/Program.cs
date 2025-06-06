using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

class Program
{
    static void Main()
    {
        const int count = 100000;
        var random = new Random();
        int firstElement;

        // Генерация списка случайных чисел
        var list = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            list.Add(random.Next(1, 1000));
        }
        firstElement = list[0];

        var linkedList = new LinkedList<int>(list);

        Console.WriteLine("Начало обработки...");

        // Измерение скорости для List
        Stopwatch sw = Stopwatch.StartNew();
        ProcessList(list, firstElement);
        sw.Stop();
        Console.WriteLine($"List обработан за {sw.ElapsedMilliseconds} мс");

        // Измерение скорости для LinkedList
        sw.Restart();
        ProcessLinkedList(linkedList, firstElement);
        sw.Stop();
        Console.WriteLine($"LinkedList обработан за {sw.ElapsedMilliseconds} мс");
    }

    static void ProcessList(List<int> list, int firstElement)
    {
        list.RemoveAll(x => x % firstElement == 0);

        for (int i = 0; i < list.Count - 1; i++)
        {
            if (list[i] % 2 == list[i + 1] % 2)
            {
                list.Insert(i + 1, 0);
                i++;
            }
        }
    }

    static void ProcessLinkedList(LinkedList<int> linkedList, int firstElement)
    {
        var node = linkedList.First;
        while (node != null)
        {
            var next = node.Next;
            if (node.Value % firstElement == 0)
            {
                linkedList.Remove(node);
            }
            node = next;
        }

        node = linkedList.First;
        while (node?.Next != null)
        {
            if (node.Value % 2 == node.Next.Value % 2)
            {
                linkedList.AddAfter(node, new LinkedListNode<int>(0));
                node = node.Next;
            }
            node = node.Next;
        }
    }
}
