namespace Catalog.Domain;

public class Attribute
{
    public Attribute() { }

    public Attribute(string name, List<AttributeValue> values)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Invalid name");

        Values = values;
    }

    public static Attribute Create(string name, List<AttributeValue> values)
        => new Attribute(name, values);

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<AttributeValue> Values { get; set; } = [];

    public AttributeNameEdited EditName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Invalid name");
        Name = name;
        return new AttributeNameEdited(Id, name);
    }

    public AttributeValueAdded AddValue(string text, string bgStyleClass, string textStyleClass, string borderStyleClass)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("Invalid value text");

        text = text.Trim();

        if (Values.Any(v => v.Text == text))
            throw new ArgumentException("Duplicate value");

        var value = new AttributeValue
            {
                Text = text,
                BgStyleClass = bgStyleClass ?? string.Empty,
                TextStyleClass = textStyleClass ?? string.Empty,
                BorderStyleClass = borderStyleClass ?? string.Empty
            };
        Values.Add(value);

        return new AttributeValueAdded(Id, value);
    }

    public AttributeValueRemoved RemoveValue(string valueText)
    {
        var value = Values.FirstOrDefault(x => x.Text == valueText)
            ?? throw new ArgumentException($"Value {valueText} does not exists");
        Values.Remove(value);
        return new AttributeValueRemoved(Id, valueText);
    }
}

public class AttributeValue
{
    public string Text { get; set; } = string.Empty;
    public string BgStyleClass { get; set; } = string.Empty;
    public string TextStyleClass { get; set; } = string.Empty;
    public string BorderStyleClass { get; set; } = string.Empty;
}
