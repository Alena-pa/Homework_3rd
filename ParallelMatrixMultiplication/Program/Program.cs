using System;
using System.Diagnostics;
using ParallelMultiplication;

namespace ParallelMultiplication
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Matrix muliplication program");
            Console.WriteLine("Enter file path for first matrix: ");
            string fileFirst = Console.ReadLine();
            Console.WriteLine("Enter file path for second matrix: ");
            string fileSecond = Console.ReadLine();

            Console.WriteLine("Enter file path for result of sequential multiplication: ");
            string pathForSeqResult = Console.ReadLine();

            Console.WriteLine("Enter file path for result of parallel multiplication: ");
            string pathForParResult = Console.ReadLine();

            Matrix first, second;
            try
            {
                first = Matrix.ReadFromFile(fileFirst);
                second = Matrix.ReadFromFile(fileSecond);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at reading files: " + ex.Message);
                return;
            }

            Console.WriteLine($"First matrix: {first.Rows}x{first.Columns}, Second matrix: {second.Rows}x{second.Columns}");

            var (resultOfSequentialMultiply, sequentialTime) = Matrix.MultiplySequential(first, second);
            Console.WriteLine($"Sequential multiplication took {sequentialTime} ms");

            var (resultOfParallelMultiply, parallelTime) = Matrix.MultiplyParallel(first, second);
            Console.WriteLine($"Parallel multiplication took {parallelTime} ms");

            resultOfSequentialMultiply.WriteToFile(pathForSeqResult);
            resultOfParallelMultiply.WriteToFile(pathForParResult);
        }
    }
}