// <copyright file="ILazy.cs" company="Pakhnusheva Alena">
// Copyright (c) Pakhnusheva Alena. All rights reserved.
// </copyright>
using System;
using ILazy;
using LazyCalculation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    /// <summary>
    /// Unit tests for the SingleThreaded lazy implementation
    /// </summary>
    [TestClass]
    public class SingleThreadedTests
    {
        [TestMethod]
        public void getReturnsCorrectValue()
        {
            var lazy = new SingleThreaded<int>(() => 10);

            Assert.AreEqual(10, lazy.Get());
        }

        [TestMethod]
        public void getReturnsSameValueEveryTime()
        {
            var counter = 0;
            var lazy = new SingleThreaded<int>(() =>
            {
                counter++;
                return 42;
            });

            var first = lazy.Get();
            var second = lazy.Get();
            var third = lazy.Get();

            Assert.AreEqual(42, first);
            Assert.AreEqual(first, second);
            Assert.AreEqual(second, third);
            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void getReturnsNull()
        {
            var counter = 0;
            var lazy = new SingleThreaded<object?>(() =>
            {
                counter++;
                return null;
            });

            var first = lazy.Get();
            var second = lazy.Get();

            Assert.IsNull(first);
            Assert.IsNull(second);
            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void constructorThrowsIfSupplierIsNull()
        {
            var unused = new SingleThreaded<int>(null!);
        }
    }
}
