using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services.Mappings
{
    public static class IgnoreVirtualExtensions
    {
        public static TypeAdapterSetter<TSource, TDestination> IgnoreAllVirtual<TSource, TDestination>(this TypeAdapterSetter<TSource, TDestination> expression)
        {
            var desType = typeof(TDestination);
            foreach (var property in desType.GetProperties().Where(p => p.GetGetMethod().IsVirtual))
            {
                if (!property.Name.Equals("Id"))
                {
                    expression.Ignore(property.Name);
                }
            }

            return expression;
        }
    }
}
