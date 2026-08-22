namespace Tests;

public class ObjectWithSimpleProperties
{
    public string? Text { get; set; }

    public int Value { get; set; }
}

public class ObjectWithSimpleStringArray
{
    public string[] Texts { get; set; } = null!;
}

public class ObjectWithSimpleIntArray
{
    public int[] Values { get; set; } = null!;
}

public class ObjectWithComplexArray
{
    public ObjectWithSimpleProperties[] Items { get; set; } = null!;
}

public class ObjectWithInnerObject
{
    public ObjectWithSimpleProperties InnerObject { get; set; } = null!;
}

public class ObjectWithNullableValues
{
    public string? Text { get; set; } = "Initial";

    public int? Number { get; set; } = 123;

    public string?[]? Values { get; set; }

    public string[]? EmptyValues { get; set; }
}
