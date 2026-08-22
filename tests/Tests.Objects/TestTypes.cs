using System;
using System.Collections.Generic;

namespace Tests;

public class ObjectWithScalarInt
{
    public int Value { get; set; }
}

public class ObjectWithScalarNullableInt
{
    public int? Value { get; set; }
}

public class ObjectWithScalarString
{
    public string? Value { get; set; }
}

public class ObjectWithScalarTimeSpan
{
    public TimeSpan Value { get; set; }
}

public class ObjectWithIntList
{
    public List<int>? Values { get; set; }
}

public class ObjectWithNullableIntList
{
    public List<int?>? Values { get; set; }
}

public class ObjectWithStringList
{
    public List<string?>? Values { get; set; }
}

public class ObjectWithIntMap
{
    public Dictionary<string, int>? Values { get; set; }
}

public class ObjectWithNullableIntMap
{
    public Dictionary<string, int?>? Values { get; set; }
}

public class ObjectWithStringMap
{
    public Dictionary<string, string?>? Values { get; set; }
}

public class ObjectWithTwoScalars
{
    public int Count { get; set; }

    public string? Name { get; set; }
}

public class ObjectWithThreeScalars
{
    public int Count { get; set; }

    public string? Name { get; set; }

    public TimeSpan Timeout { get; set; }
}

public class ObjectWithTwoCollections
{
    public List<int>? Numbers { get; set; }

    public List<string?>? Names { get; set; }
}

public class ObjectWithCollectionAndMap
{
    public List<string?>? Names { get; set; }

    public Dictionary<string, int>? Scores { get; set; }
}

public class ObjectWithNestedObject
{
    public ObjectWithThreeScalars? Nested { get; set; }
}
