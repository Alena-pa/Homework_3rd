// <copyright file="ILazy.cs" company="Pakhnusheva Alena">
// Copyright (c) Pakhnusheva Alena. All rights reserved.
// </copyright>

using ILazy;
using System;

namespace LazyCalculation;

public class SingleThreaded<T> : ILazy<T>
{
    private Func<T> _supplier;
    private T value;
    private bool isEvaluared;

    public SingleThreaded(Func<T> supplier)
    {
        _supplier = supplier ?? throw new ArgumentNullException(nameof(supplier)); 
    }

    public T Get()
    {
        if (isEvaluared)
        {
            return value;
        }

        if (_supplier != null)
        {
            value = _supplier();
            isEvaluared = true;
        }

        _supplier = null;

        return value;
    }
}
