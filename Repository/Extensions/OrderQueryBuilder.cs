using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Repository.Extensions
{
    public static class OrderQueryBuilder
    {
        public static string CreateOrderQuery<T>(string orderByQueryString)
        {
            var orderParams = orderByQueryString.Trim().Split(','); // prep sort params from query
            var properyInfos =  // use reflection to get Employee parameters for use later
                typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param)) continue;

                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = properyInfos
                    .FirstOrDefault(p => p.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase)); // check if included param has a coresponding 
                                                                     // property on the T class to use

                if (objectProperty is null) continue; // if unfound skip to next param


                var direction = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction},");
            }

            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' '); // remove unnecesery ',' from end of query
            
            return orderQuery;
        }
    }
}
