using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ParallelInventorySearch
{
    class Program
    {
        static void Main(string[] args)
        {
            
            int inventorySize = 100000;
            var inventory = InventoryGenerator.GenerateInventory(inventorySize);
            
            var criteria = new Dictionary<int, int>
            {
                { 1, 30 }, 
                { 7, 15 }, 
                { 10, 8 }  
            };
          
            int[] threadCounts = { 2, 3, 4, 6 };
            foreach (int threadCount in threadCounts)
            {
                var stopwatch = Stopwatch.StartNew();

                var results = ParallelSearch.ParallelSearchInventory(inventory, threadCount, new Dictionary<int, int>(criteria));

                stopwatch.Stop();
                Console.WriteLine($"Execution time with {threadCount} threads: {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Items found: {results.Count}");
            }
        }
    }
}
