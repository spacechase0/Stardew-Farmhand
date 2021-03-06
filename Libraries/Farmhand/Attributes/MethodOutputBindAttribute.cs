﻿using System;

namespace Farmhand.Attributes
{
    /// <summary>
    /// Gets the otherwise outputted variable from a HookReturnable marked exit method
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MethodOutputBindAttribute : ParameterBindAttribute
    {
    }
}
