using System;
using System.Collections.Generic;
using System.Text;

namespace AutoComplete
{
    class Program
    {
        static void Main(string[] args)
        {

            AutoCompleter completer = new();
            completer.AddToSearch(new()
            {
                new() { Name = "Анастасия" },
                new() { Name = "Анна" },
                new() { Name = "Аннб" },
                new() { Name = "Богдан" },
                new() { Name = "Борис" }
            });
            foreach (var str in completer.Search("Анна"))
                Console.WriteLine(str);
        }
    }
}
