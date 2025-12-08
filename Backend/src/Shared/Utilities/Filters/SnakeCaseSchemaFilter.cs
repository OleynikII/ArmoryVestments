namespace Utilities.Filters;

public class SnakeCaseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null) return;

        var newProps = new Dictionary<string, OpenApiSchema>();
        foreach (var kvp in schema.Properties)
        {
            string snakeKey = ToSnakeCase(kvp.Key);
            newProps[snakeKey] = kvp.Value;
        }

        schema.Properties.Clear();
        foreach (var kvp in newProps)
            schema.Properties.Add(kvp.Key, kvp.Value);
    }
    
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        var sb = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (char.IsUpper(c))
            {
                if (i > 0) sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}