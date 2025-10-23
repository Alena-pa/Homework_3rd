// <copyright file="ILazy.cs" company="Pakhnusheva Alena">
// Copyright (c) Pakhnusheva Alena. All rights reserved.
// </copyright>

using ILazy;
using System;

namespace LazyCalculation;

/// <summary>
/// Single threaded implementation of the ILazy interface
/// </summary>
/// <typeparam name="T">Type of the computed value</typeparam>
public class SingleThreaded<T> : ILazy<T>
{
    private Func<T>? supplier;
    private T value;
    private bool isEvaluared;

    public SingleThreaded(Func<T> supplier)
    {
        this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier)); 
    }

    /// <summary>
    /// Returns the lazily evaluated value
    /// </summary>
    /// <returns>The computed value</returns>
    public T Get()
    {
        if (isEvaluared)
        {
            return value;
        }

        if (supplier != null)
        {
            value = supplier();
            isEvaluared = true;
        }

        supplier = null;

        return value;
    }
}
