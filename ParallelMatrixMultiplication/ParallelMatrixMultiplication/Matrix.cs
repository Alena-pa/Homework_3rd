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
            if (rows <= 0 || columns <= 0)
            {
                throw new ArgumentException("Matrix dimensions must be positive");
            }
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
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be empty");
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("File is not found");
            }

            string[] lines = File.ReadAllLines(fileName);

            string[] dims = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (dims.Length != 2)
            {
                throw new FormatException("First line must contain two integers");
            }

            if (!int.TryParse(dims[0], out int rows) || !int.TryParse(dims[1], out int columns))
            {
                throw new FormatException("Matrix dimensions must be integers");
            }

            if (rows <= 0 || columns <= 0)
            {
                throw new ArgumentException("Matrix dimensions must be positive");
            }

            if (lines.Length - 1 < rows)
            {
                throw new FormatException("Not enough rows for matrix data");
            }

            Matrix result = new Matrix(rows, columns);

            for (int i = 0; i < rows; i++)
            {
                string[] parts = lines[i + 1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != columns)
                {
                    throw new FormatException($"Row {i + 1} must have {columns} values");
                }

                for (int j = 0; j < columns; j++)
                {
                    if (!int.TryParse(parts[j], out int value))
                    {
                        throw new FormatException($"Invalid number at row {i + 1}, column {j + 1}");
                    }

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
            if (rows <= 0 || columns <= 0)
            {
                throw new ArgumentException("Matrix dimensions must be positive");
            }
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
            if (first == null || second == null)
            {
                throw new ArgumentNullException("Matrices cannot be null");
            }

            if (first.Columns != second.Rows)
            {
                throw new ArgumentException("Invalid matrix dimensions for multiplication");
            }

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
            if (first == null || second == null)
            {
                throw new ArgumentNullException("Matrices cannot be null");
            }

            if (first.Columns != second.Rows)
            {
                throw new ArgumentException("Invalid matrix dimensions for multiplication");
            }

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

        public static void RunTests(int[] sizesOfMatrices, int numberOfTests)
        {
            if (numberOfTests <= 0)
            {
                throw new ArgumentException("Number of tests must be positive");
            }

            Console.WriteLine("Size   Mean seq (ms)     Std seq   Mean par (ms)     Std par");

            foreach (int size in sizesOfMatrices)
            {
                Matrix first = Matrix.GenerateRandomMatrix(size, size);
                Matrix second = Matrix.GenerateRandomMatrix(size, size);

                double[] timesSeq = new double[numberOfTests];
                double[] timesPar = new double[numberOfTests];

                for (int i = 0; i < numberOfTests; i++)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    Matrix resultSeq = Matrix.MultiplySequential(first, second);
                    sw.Stop();
                    timesSeq[i] = sw.Elapsed.TotalMilliseconds;

                    sw.Restart();
                    Matrix resultPar = Matrix.MultiplyParallel(first, second);
                    sw.Stop();
                    timesPar[i] = sw.Elapsed.TotalMilliseconds;

                    if (!CompareMatrices(resultSeq, resultPar))
                    {
                        throw new Exception("Results of sequential and parallel multiplication are different");
                    }
                }

                double meanSeq = timesSeq.Average();
                double varianceSeq = CalculateVariance(timesSeq, meanSeq);
                double stdSeq = Math.Sqrt(varianceSeq);

                double meanPar = timesPar.Average();
                double variancePar = CalculateVariance(timesPar, meanPar);
                double stdPar = Math.Sqrt(variancePar);

                Console.WriteLine($"{size}   {meanSeq,15:F2} {stdSeq,10:F2} {meanPar,15:F2} {stdPar,10:F2}");
            }
        }

        public static bool CompareMatrices(Matrix first, Matrix second)
        {
            if (first.Rows != second.Rows || first.Columns != second.Columns)
            {
                return false;
            }

            for (int i = 0; i < first.Rows; i++)
            {
                for (int j = 0; j < first.Columns; j++)
                {
                    if (first[i, j] != second[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static double CalculateVariance(double[] values, double mean)
        {
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                double diff = values[i] - mean;
                sum += diff * diff;
            }

            return sum / values.Length;
        }
    }
}