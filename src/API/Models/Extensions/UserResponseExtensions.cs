﻿namespace Users.API.Models.Extensions;

public static class UserResponseExtensions
{
    public static ExpandoObject ShapeData(this Users.API.Models.Response.v1.UserResponse source, string? fields = null)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (string.IsNullOrEmpty(fields)) fields = GetAllFields();

        fields = AddIdToFieldsIfMissing(fields);

        var propertiesToInclude = new List<PropertyInfo>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            var propertyInfos = typeof(Users.API.Models.Response.v1.UserResponse).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            propertiesToInclude.AddRange(propertyInfos);
        }
        else
        {
            var splitFields = fields.Split(',');

            foreach(var field in splitFields)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(Users.API.Models.Response.v1.UserResponse).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null) throw new ArgumentOutOfRangeException(nameof(fields), $"Field '{propertyName}' was not found.");
                propertiesToInclude.Add(propertyInfo);
            }
        }

        var shaped = new ExpandoObject();

        foreach (var propertyToInclude in propertiesToInclude)
        {
            var name = propertyToInclude.Name;
            var value = propertyToInclude.GetValue(source);
            shaped.TryAdd(name, value);
        }

        return shaped;
    }

    public static IEnumerable<ExpandoObject> ShapeData(this IEnumerable<Users.API.Models.Response.v1.UserResponse> source, string fields)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (string.IsNullOrEmpty(fields)) fields = GetAllFields();

        fields = AddIdToFieldsIfMissing(fields);

        var propertiesToInclude = new List<PropertyInfo>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            var propertyInfos = typeof(Users.API.Models.Response.v1.UserResponse).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            propertiesToInclude.AddRange(propertyInfos);
        }
        else
        {
            var splitFields = fields.Split(',');

            foreach (var field in splitFields)
            {
                var propertyName = field.Trim();
                var propertyInfo = typeof(Users.API.Models.Response.v1.UserResponse).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null) throw new ArgumentOutOfRangeException(nameof(fields), $"Field '{propertyName}' was not found.");
                propertiesToInclude.Add(propertyInfo);
            }
        }

        var shapeds = new List<ExpandoObject>();

        foreach (var user in source)
        {
            var shaped = new ExpandoObject();
            foreach (var propertyToInclude in propertiesToInclude)
            {                
                var name = propertyToInclude.Name;
                var value = propertyToInclude.GetValue(user);
                shaped.TryAdd(name, value);                
            }
            shapeds.Add(shaped);
        }

        return shapeds;
    }

    private static string AddIdToFieldsIfMissing(string fields)
    {
        var listOfFields = new List<string>();
        var hasId = false;
        foreach (var field in fields.Split(','))
        {
            if (field.ToLower() == "id") hasId = true;
            listOfFields.Add(field);
        }
        if (!hasId) listOfFields.Add("Id");
        return string.Join(",", listOfFields);
    }

    private static string GetAllFields()
    {
        var fields = new List<string>();

        var propertyInfos = typeof(Users.API.Models.Response.v1.UserResponse).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        foreach (var propertyInfo in propertyInfos)
        {
            fields.Add(propertyInfo.Name);
        }

        return string.Join(",", fields);
    }
}
