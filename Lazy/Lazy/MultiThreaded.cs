// <copyright file="ILazy.cs" company="Pakhnusheva Alena">
// Copyright (c) Pakhnusheva Alena. All rights reserved.
// </copyright>

using System;
using ILazy;

namespace LazyCalculation;

public class MultiThreaded<T> : ILazy<T>
{
    private Func<T> _supplier;
    private T value;
    private bool isEvaluated;
    private readonly Lock _lock = new();

    public T Get()
    {
        if (isEvaluated)
        {
            return value;
        }

        lock (_lock)
        {
            if (!isEvaluated)
            {
                value = _supplier();
                isEvaluated = true;
                _supplier = null;
            }
        }

        return value;
    }
}