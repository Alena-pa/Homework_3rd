using System;
using System.IO;
using ParallelMultiplication;

namespace ParallelMultiplicationTests
{
    [TestClass]
    public class MatrixTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorInvalidDimensions()
        {
            var matrix = new Matrix(0, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RandomInvalidDimensions()
        {
            var matrix = Matrix.GenerateRandomMatrix(-2, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SequentialNullMatrices()
        {
            var matrix = Matrix.GenerateRandomMatrix(2, 2);
            Matrix.MultiplySequential(null, matrix);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SequentialIncompatibleSizes()
        {
            var firstMatrix = Matrix.GenerateRandomMatrix(2, 3);
            var secondMatrix = Matrix.GenerateRandomMatrix(4, 2);
            Matrix.MultiplySequential(firstMatrix, secondMatrix);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParallelIncompatibleSizes()
        {
            var firstMatrix = Matrix.GenerateRandomMatrix(2, 3);
            var secondMatrix = Matrix.GenerateRandomMatrix(4, 2);
            Matrix.MultiplyParallel(firstMatrix, secondMatrix);
        }

        [TestMethod]
        public void SequentialEqualsParallel()
        {
            var firstMatrix = Matrix.GenerateRandomMatrix(3, 3);
            var secondMatrix = Matrix.GenerateRandomMatrix(3, 3);

            var sequential = Matrix.MultiplySequential(firstMatrix, secondMatrix);
            var parallel = Matrix.MultiplyParallel(firstMatrix, secondMatrix);

            Assert.IsTrue(Matrix.CompareMatrices(sequential, parallel));
        }

        [TestMethod]
        public void EmptyMatrixCheck()
        {
            var matrix = new Matrix(2, 2);
            Assert.IsTrue(Matrix.IsMatrixEmpty(matrix));

            matrix[0, 0] = 1;
            Assert.IsFalse(Matrix.IsMatrixEmpty(matrix));

            Assert.IsTrue(Matrix.IsMatrixEmpty(null));
        }

        [TestMethod]
        public void CompareDifferentSizes()
        {
            var firstMatrix = Matrix.GenerateRandomMatrix(2, 2);
            var secondMatrix = Matrix.GenerateRandomMatrix(3, 3);

            Assert.IsFalse(Matrix.CompareMatrices(firstMatrix, secondMatrix));
        }

        [TestMethod]
        public void CompareIdenticalMatrices()
        {
            var firstMatrix = new Matrix(2, 2);
            var secondMatrix = new Matrix(2, 2);

            firstMatrix[0, 0] = 1; firstMatrix[0, 1] = 2;
            firstMatrix[1, 0] = 3; firstMatrix[1, 1] = 4;

            secondMatrix[0, 0] = 1; secondMatrix[0, 1] = 2;
            secondMatrix[1, 0] = 3; secondMatrix[1, 1] = 4;

            Assert.IsTrue(Matrix.CompareMatrices(firstMatrix, secondMatrix));
        }

        [TestMethod]
        public void ReadFromMissingFile()
        {
            Assert.ThrowsException<FileNotFoundException>(() =>
            {
                Matrix.ReadFromFile("nonExistent.txt");
            });
        }

        [TestMethod]
        public void ReadFromFileBadDimensions()
        {
            string fileName = "badDims.txt";
            File.WriteAllText(fileName, "abc def\n1 2\n3 4");

            try
            {
                Assert.ThrowsException<FormatException>(() =>
                {
                    Matrix.ReadFromFile(fileName);
                });
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [TestMethod]
        public void ReadFromFileNotEnoughRows()
        {
            string fileName = "notEnoughRows.txt";
            File.WriteAllText(fileName, "3 2\n1 2\n3 4");

            try
            {
                Assert.ThrowsException<FormatException>(() =>
                {
                    Matrix.ReadFromFile(fileName);
                });
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [TestMethod]
        public void ReadFromFileWrongColumnsCount()
        {
            string fileName = "wrongCols.txt";
            File.WriteAllText(fileName, "2 3\n1 2 3\n4 5");

            try
            {
                Assert.ThrowsException<FormatException>(() =>
                {
                    Matrix.ReadFromFile(fileName);
                });
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        [TestMethod]
        public void ReadFromFileInvalidNumber()
        {
            string fileName = "invalidNumber.txt";
            File.WriteAllText(fileName, "2 2\n1 x\n3 4");

            try
            {
                Assert.ThrowsException<FormatException>(() =>
                {
                    Matrix.ReadFromFile(fileName);
                });
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }
    }
}