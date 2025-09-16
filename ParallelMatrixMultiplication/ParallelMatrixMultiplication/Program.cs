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
            Console.WriteLine("Choose an option:\n 1. Read two matrixes from files \n2. Generate random matrixed");
            Console.WriteLine("Enter an option: ");
            string option = Console.ReadLine();

            Matrix first = null, second = null;

            switch (option)
            {
                case "1":
                    Console.WriteLine("Enter file path for first matrix: ");
                    string fileFirst = Console.ReadLine();
                    Console.WriteLine("Enter file path for second matrix: ");
                    string fileSecond = Console.ReadLine();

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

                    break;

                case "2":
                    Console.WriteLine("Enter number of rows and columns for first matrix: ");
                    string[] rowsColumnsForFirst = Console.ReadLine().Split();

                    Console.WriteLine("Enter file path for first matrix or leave blank if you want to create a new file");
                    string filePath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(filePath))
                    {
                        File.Create("firstMatrix.txt").Close();
                        filePath = "firstMatrix.txt";
                    }

                    int rows = int.Parse(rowsColumnsForFirst[0]);
                    int columns = int.Parse(rowsColumnsForFirst[1]);
                    first = Matrix.GenerateRandomMatrix(rows, columns);
                    first.WriteToFile(filePath);
                    Console.WriteLine("Enter number of rows and columns for second matrix: ");
                    string[] rowsColumnsForSecond = Console.ReadLine().Split();

                    Console.WriteLine("Enter file path for second matrix or leave blank if you want to create a new file");
                    filePath = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(filePath))
                    {
                        File.Create("secondMatrix.txt").Close();
                        filePath = "secondMatrix.txt";
                    }

                    rows = int.Parse(rowsColumnsForSecond[0]);
                    columns = int.Parse(rowsColumnsForSecond[1]);
                    second = Matrix.GenerateRandomMatrix(rows, columns);
                    second.WriteToFile(filePath);
                    break;

                default:
                    Console.WriteLine("Incorrect number of option");
                    break;
            }

            Console.WriteLine("Enter file path for result of sequential multiplication: ");
            string pathForSeqResult = Console.ReadLine();

            Console.WriteLine("Enter file path for result of parallel multiplication: ");
            string pathForParResult = Console.ReadLine();

            if (!Matrix.IsMatrixEmpty(first) && !Matrix.IsMatrixEmpty(second))
            {
                var sequentialTime = Stopwatch.StartNew();
                Matrix resultOfSequentialMultiply = Matrix.MultiplySequential(first, second);
                sequentialTime.Stop();
                Console.WriteLine($"Sequential multiplication took {sequentialTime} ms");

                var parallelTime = Stopwatch.StartNew();
                Matrix resultOfParallelMultiply = Matrix.MultiplyParallel(first, second);
                parallelTime.Stop();

                Console.WriteLine($"Parallel multiplication took {parallelTime} ms");

                resultOfSequentialMultiply.WriteToFile(pathForSeqResult);
                resultOfParallelMultiply.WriteToFile(pathForParResult);
            }
        }
    }
}