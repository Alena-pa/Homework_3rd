// <copyright file="ILazy.cs" company="Pakhnusheva Alena">
// Copyright (c) Pakhnusheva Alena. All rights reserved.
// </copyright>

using System;
using ILazy;

namespace LazyCalculation;

/// <summary>
/// Multi threaded implementation of the ILazy interface.
/// </summary>
/// <typeparam name="T">Type of the computed value</typeparam>
public class MultiThreaded<T> : ILazy<T>
{
    private Func<T>?supplier;
    private T value;
    private bool isEvaluated;
    private readonly Lock locker = new();

    public MultiThreaded(Func<T> supplier)
    {
        this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
    }

    /// <summary>
    /// Returns the lazily evaluated value
    /// </summary>
    /// <returns>The computed value</returns>
    public T Get()
    {
        if (isEvaluated)
        {
            return value;
        }

        lock (locker)
        {
            if (!isEvaluated)
            {
                value = supplier();
                isEvaluated = true;
                supplier = null;
            }
        }

        return value;
    }
}