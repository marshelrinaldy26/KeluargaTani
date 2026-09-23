    using System.Linq.Dynamic.Core;
    using System.Reflection;

    namespace KeluargaTani.Helper
    {
        public static class SearchHelper
        {
            public static IQueryable<T> Search<T>(this IQueryable<T> query, string columnName, string searchValue)
            {
                // Handle nested property
                PropertyInfo propertyInfo;
                Type propOwnerType = typeof(T);

                if (columnName.Contains("."))
                {
                    var parts = columnName.Split(".");
                    PropertyInfo currentProp = null;

                    foreach (var part in parts)
                    {
                        currentProp = propOwnerType.GetProperty(
                            part,
                            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                        );

                        if (currentProp == null) return query; // property tidak ditemukan
                        propOwnerType = currentProp.PropertyType;
                    }

                    propertyInfo = currentProp;
                }
                else
                {
                    propertyInfo = typeof(T).GetProperty(
                        columnName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                    );
                }

                if (propertyInfo == null)
                    return query;

                var propType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                try
                {
                    if (propType == typeof(DateTime))
                    {
                        if (DateTime.TryParse(searchValue, out var dt))
                        {
                            var start = dt.Date;
                            var end = dt.Date.AddDays(1);
                            query = query.Where($"{columnName} >= @0 && {columnName} < @1", start, end);
                        }
                    }
                    else if (propType == typeof(DateOnly))
                    {
                        if (DateOnly.TryParse(searchValue, out var dateOnly))
                        {
                            query = query.Where($"{columnName} == @0", dateOnly);
                        }
                    }
                    //else if (propType == typeof(int) || propType == typeof(long) || propType == typeof(decimal) || propType == typeof(double))
                    else if (propType.IsEnum || (
                            Type.GetTypeCode(propType) >= TypeCode.SByte &&
                            Type.GetTypeCode(propType) <= TypeCode.Decimal))
                    {
                        if (long.TryParse(searchValue, out var longval))
                            query = query.Where($"{columnName} != null && {columnName} == @0", longval);
                    }
                    else if (propType == typeof(bool))
                    {
                        if (bool.TryParse(searchValue, out var boolVal))
                            query = query.Where($"{columnName} == @0", boolVal);
                    }
                    else
                    {
                        // Untuk nested property gunakan format Dynamic LINQ yang benar
                        query = query.Where($"{columnName} != null && {columnName}.ToLower().Contains(@0)", searchValue.ToLower());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ApplySearch] Error on column {columnName}: {ex.Message}");
                }

                return query;
            }

            public static IQueryable<T> GlobalSearch<T>(this IQueryable<T> query, Microsoft.AspNetCore.Http.IFormCollection form, int columnCount, string searchValue)
            {
                var conditions = new List<string>();
                var propOwnerType = typeof(T);

                for (int i = 0; i < columnCount; i++)
                {
                    var isSearchable = form[$"columns[{i}][searchable]"].FirstOrDefault();
                    if (!string.Equals(isSearchable, "true", StringComparison.OrdinalIgnoreCase)) continue;

                    var columnName = form[$"columns[{i}][data]"].FirstOrDefault();
                    if (string.IsNullOrEmpty(columnName) || columnName == "null") continue;

                    PropertyInfo propertyInfo = null;
                    Type currentType = propOwnerType;

                    if (columnName.Contains("."))
                    {
                        var parts = columnName.Split(".");
                        foreach (var part in parts)
                        {
                            propertyInfo = currentType.GetProperty(
                                part,
                                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                            );
                            if (propertyInfo == null) break;
                            currentType = propertyInfo.PropertyType;
                        }
                    }
                    else
                    {
                        propertyInfo = currentType.GetProperty(
                            columnName,
                            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
                        );
                    }

                    if (propertyInfo == null) continue;

                    var propType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                    string exactPropName = propertyInfo.Name;
                    if (columnName.Contains(".")) {
                        exactPropName = char.ToUpper(columnName[0]) + columnName.Substring(1);
                        if (exactPropName.Contains(".")) {
                            var parts = exactPropName.Split(".");
                            exactPropName = parts[0] + "." + propertyInfo.Name;
                        }
                    }
                    if (propType == typeof(string))
                    {
                        conditions.Add($"{exactPropName} != null && {exactPropName}.ToLower().Contains(@0)");
                    }
                }

                if (conditions.Any())
                {
                    string whereClause = string.Join(" || ", conditions);
                    query = query.Where(whereClause, searchValue.ToLower());
                }

                return query;
            }
        }
    }