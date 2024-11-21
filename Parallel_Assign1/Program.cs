using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ParallelBubbleSortExample
{
    public class ParallelBubbleSort
    {
       
        public static int[] GenerateRandomArray(int size, int maxValue = 1000000)
        {
            Random random = new Random();
            return Enumerable.Range(0, size).Select(_ => random.Next(maxValue)).ToArray();
        }

        public static int[] ParallelBubbleSortArray(int[] array, int numberOfThreads)
        {
            int partitionSize = array.Length / numberOfThreads;
            int[][] partitions = new int[numberOfThreads][];

            for (int i = 0; i < numberOfThreads; i++)
            {
                int startIdx = i * partitionSize;
                int endIdx = (i == numberOfThreads - 1) ? array.Length : startIdx + partitionSize;

                partitions[i] = array[startIdx..endIdx];
            }

            
            List<Task> tasks = new List<Task>();

            foreach (var partition in partitions)
            {
                tasks.Add(Task.Run(() => BubbleSort(partition)));
            }

            Task.WaitAll(tasks.ToArray());
         
            return MergeSortedPartitions(partitions);
        }
       
        private static void BubbleSort(int[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
        }        
        private static int[] MergeSortedPartitions(int[][] partitions)
        {
            List<int> mergedList = new List<int>();
            var pointers = new int[partitions.Length];

            while (mergedList.Count < partitions.Sum(p => p.Length))
            {
                int minValue = int.MaxValue;
                int minIndex = -1;

                // Find the smallest current element among all partitions
                for (int i = 0; i < partitions.Length; i++)
                {
                    if (pointers[i] < partitions[i].Length && partitions[i][pointers[i]] < minValue)
                    {
                        minValue = partitions[i][pointers[i]];
                        minIndex = i;
                    }
                }

                mergedList.Add(minValue);
                pointers[minIndex]++;
            }

            return mergedList.ToArray();
        }

        // Main method to test and measure execution
        public static void Main(string[] args)
        {
            int arraySize = 100000; // Size of the array
            int[] threadCounts = { 2, 3, 4, 6 }; // Number of threads to test

            int[] originalArray = GenerateRandomArray(arraySize);

            Console.WriteLine("This will take some time. Thank you for your patience!");
            foreach (int threadCount in threadCounts)
            {
                int[] arrayCopy = new int[arraySize];
                Array.Copy(originalArray, arrayCopy, arraySize);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                ParallelBubbleSortArray(arrayCopy, threadCount);

                stopwatch.Stop();

                Console.WriteLine($"The task is done. {threadCount} threads executed the task in time{stopwatch.Elapsed.TotalMilliseconds:F2} ms.");
            }

            Console.ReadLine();
        }
    }
}
