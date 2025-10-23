// <copyright file="ILazy.cs" company="Pakhnusheva Alena">
// Copyright (c) Pakhnusheva Alena. All rights reserved.
// </copyright>

namespace ILazy;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    T Get();
}