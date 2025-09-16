using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ParallelMultiplication
{
    public class Matrix
    {
        public int Rows { get; }
        public int Columns { get; }
        private int[,] numbers { get; }
        public Matrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            numbers = new int[rows, columns];
        }

        public int this[int row, int column]
        {
            get => numbers[row, column];
            set => numbers[row, column] = value;
        }

        public static Matrix ReadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("File is not found");
            }

            string[] lines = File.ReadAllLines(fileName);

            string[] dims = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int rows = int.Parse(dims[0]);
            int columns = int.Parse(dims[1]);

            Matrix result = new Matrix(rows, columns);

            for (int i = 0; i < rows; i++)
            {
                string[] parts = lines[i + 1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < columns; j++)
                {
                    result.numbers[i, j] = int.Parse(parts[j]);
                }
            }

            return result;
        }

        public void WriteToFile(string fileName)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine($"{Rows} {Columns}");

                for (int i = 0; i < Rows; i++)
                {
                    string[] line = new string[Columns];
                    for (int j = 0; j < Columns; j++)
                    {
                        line[j] = numbers[i, j].ToString();
                    }

                    writer.WriteLine(string.Join(" ", line));
                }
            }
        }

        public static Matrix GenerateRandomMatrix(int rows, int columns)
        {
            Matrix matrix = new Matrix(rows, columns);

            Random random = new Random();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    matrix[i, j] = random.Next(0, 10000);
                }
            }

            return matrix;
        }

        public static bool IsMatrixEmpty(Matrix matrix)
        {
            if (matrix == null) return true;

            try
            {
                for (int i = 0; i < matrix.Rows; i++)
                {
                    for (int j = 0; j < matrix.Columns; j++)
                    {
                        if (matrix[i, j] != default)
                            return false;
                    }
                }

                return true;
            }
            catch
            {
                return true;
            }
        }

        public static Matrix MultiplySequential(Matrix first, Matrix second)
        {
            Matrix resultMatrix = new Matrix(first.Rows, second.Columns);

            for (int i = 0; i < first.Rows; i++)
            {
                for (int j = 0; j < second.Columns; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < first.Columns; k++)
                    {
                        sum += first[i, k] * second[k, j];
                    }

                    resultMatrix[i, j] = sum;
                }
            }

            return resultMatrix;
        }

        public static Matrix MultiplyParallel(Matrix first, Matrix second)
        {
            Matrix resultMatrix = new Matrix(first.Rows, second.Columns);

            var threads = Environment.ProcessorCount;
            Thread[] workers = new Thread[threads];

            int numberOfRows = (first.Rows + threads - 1) / threads;

            for (int t = 0; t < threads; t++)
            {
                int startRow = t * numberOfRows;
                int endRow = Math.Min(startRow + numberOfRows, first.Rows);

                workers[t] = new Thread(() =>
                {
                    for (int i = startRow; i < endRow; i++)
                    {
                        for (int j = 0; j < second.Columns; j++)
                        {
                            int sum = 0;
                            for (int k = 0; k < first.Columns; k++)
                            {
                                sum += first[i, k] * second[k, j];
                            }

                            resultMatrix[i, j] = sum;
                        }
                    }
                });
                workers[t].Start();
            }

            for (int t = 0; t < threads; t++)
            {
                workers[t].Join();
            }

            return resultMatrix;
        }
    }
}