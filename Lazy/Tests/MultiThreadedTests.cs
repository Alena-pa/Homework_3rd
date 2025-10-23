// <copyright file="ILazy.cs" company="Pakhnusheva Alena">
// Copyright (c) Pakhnusheva Alena. All rights reserved.
// </copyright>
using System;
using ILazy;
using LazyCalculation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class MultiThreadedTests
    {
        private int counter;

        private int SupplierValue()
        {
            Interlocked.Increment(ref counter);
            Thread.Sleep(10);
            return 100;
        }

        private object SupplierNull()
        {
            Interlocked.Increment(ref counter);
            Thread.Sleep(5);
            return null;
        }

        private int SupplierConstant()
        {
            Interlocked.Increment(ref counter);
            return 500;
        }

        [TestMethod]
        public void getIsThreadSafeWithMultipleThreads()
        {
            counter = 0;
            var lazy = new MultiThreaded<int>(SupplierValue);

            const int threadCount = 50;
            var tasks = new Task<int>[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() => lazy.Get());
            }

            Task.WaitAll(tasks);

            for (int i = 0; i < tasks.Length; i++)
            {
                Assert.AreEqual(100, tasks[i].Result);
            }

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void getIsThreadSafeWhenValueIsNull()
        {
            counter = 0;
            var lazy = new MultiThreaded<object?>(SupplierNull);

            const int threadCount = 30;
            var tasks = new Task<object?>[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() => lazy.Get());
            }

            Task.WaitAll(tasks);

            for (int i = 0; i < tasks.Length; i++)
            {
                Assert.IsNull(tasks[i].Result);
            }

            Assert.AreEqual(1, counter);
        }

        [TestMethod]
        public void getReturnsSameValueAfterAllThreads()
        {
            counter = 0;
            var lazy = new MultiThreaded<int>(SupplierConstant);

            const int threadCount = 20;
            var threads = new Thread[threadCount];
            int[] results = new int[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int index = i;
                threads[i] = new Thread(() => results[index] = lazy.Get());
                threads[i].Start();
            }

            foreach (var t in threads)
            {
                t.Join();
            }

            for (int i = 0; i < threadCount; i++)
            {
                Assert.AreEqual(500, results[i]);
            }

            Assert.AreEqual(1, counter);
        }
    }
}
