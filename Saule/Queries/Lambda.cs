﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace Saule.Queries
{
    internal static class Lambda
    {
        public static Expression SelectProperty(Type type, string property)
        {
            var returnType = type.GetProperty(property)?.PropertyType;
            if (returnType == null)
            {
                throw new ArgumentException(
                    $"Property {property} does not exist.",
                    nameof(property));
            }

            var funcType = typeof(Func<,>).MakeGenericType(type, returnType);
            var param = Expression.Parameter(type, "i");
            var property1 = Expression.Property(param, property);

            var expressionFactory = typeof(Expression).GetMethods()
                .Where(m => m.Name == "Lambda")
                .Select(m => new
                {
                    Method = m,
                    Params = m.GetParameters(),
                    Args = m.GetGenericArguments()
                })
                .Where(x => x.Params.Length == 2 && x.Args.Length == 1)
                .Select(x => x.Method)
                .First()
                .MakeGenericMethod(funcType);

            return expressionFactory.Invoke(null, new object[] { property1, new[] { param } }) as Expression;
        }
    }
}