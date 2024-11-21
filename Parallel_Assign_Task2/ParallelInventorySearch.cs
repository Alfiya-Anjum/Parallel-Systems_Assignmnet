using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ParallelInventorySearch
{
    public class ParallelSearch
    {
        private static readonly object Locker = new object();

        public static List<InventoryItem> ParallelSearchInventory(
            List<InventoryItem> inventory,
            int threadCount,
            Dictionary<int, int> criteria)
        {            
            int partitionSize = inventory.Count / threadCount;
            var partitions = new List<List<InventoryItem>>();

            for (int i = 0; i < threadCount; i++)
            {
                int startIdx = i * partitionSize;
                int endIdx = (i == threadCount - 1) ? inventory.Count : startIdx + partitionSize;
                partitions.Add(inventory.GetRange(startIdx, endIdx - startIdx));
            }            
            var results = new List<InventoryItem>();
            var remainingCriteria = new Dictionary<int, int>(criteria);

            var threads = new List<Thread>();
            foreach (var partition in partitions)
            {
                var thread = new Thread(() =>
                {
                    var localResults = SearchPartition(partition, remainingCriteria);
                    lock (Locker)
                    {
                        results.AddRange(localResults);
                    }
                });
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return results;
        }

        private static List<InventoryItem> SearchPartition(
            List<InventoryItem> partition,
            Dictionary<int, int> criteria)
        {
            var localResults = new List<InventoryItem>();
            foreach (var item in partition)
            {
                lock (Locker)
                {
                    if (criteria.All(kv => kv.Value <= 0))
                    {
                        break;
                    }

                    if (criteria.ContainsKey(item.Type) && criteria[item.Type] > 0)
                    {
                        localResults.Add(item);
                        criteria[item.Type]--;
                    }
                }
            }
            return localResults;
        }
    }
}
