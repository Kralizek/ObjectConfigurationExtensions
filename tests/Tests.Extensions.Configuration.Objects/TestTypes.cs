namespace Tests
{
    public class ObjectWithSimpleProperties
    {
        public string Text { get; set; }

        public int Value { get; set; }
    }

    public class ObjectWithSimpleStringArray
    {
        public string[] Texts { get; set; }
    }

    public class ObjectWithSimpleIntArray
    {
        public int[] Values { get; set; }
    }

    public class ObjectWithComplexArray
    {
        public ObjectWithSimpleProperties[] Items { get; set; }
    }

    public class ObjectWithInnerObject
    {
        public ObjectWithSimpleProperties InnerObject { get; set; }
    }
}