namespace Inventory.API;

public class Warehouse
{
    public Warehouse() { }
    public Warehouse(string name, decimal lat, decimal lng)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Invalid name");
        Id = Guid.CreateVersion7();
        Name = name;
        Latitude = lat;
        Longitude = lng;
    }

    public Warehouse(Guid id, string name, decimal lat, decimal lng)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Invalid name");
        if (id == Guid.Empty)
            throw new ArgumentException("Invalid ID");

        Id = id;
        Name = name.Trim();
        Latitude = lat;
        Longitude = lng;
    }

    public static Warehouse Create(string name, decimal lat, decimal lng)
        => new Warehouse(name, lat, lng);

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsActive { get; set; } = true;

    public WarehouseNameEdited EditName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Invalid Name");

        Name = name.Trim();

        return new WarehouseNameEdited(Id, Name);
    }

    public WarehouseLocationChanged ChangeLocation(decimal lat, decimal lng)
    {
        Latitude = lat;
        Longitude = lng;

        return new WarehouseLocationChanged(Id, lat, lng);
    }

    public WarehouseDeactivated Deactive()
    {
        IsActive = false;
        return new WarehouseDeactivated(Id);
    }

    public WarehouseReactivated Reactive()
    {
        IsActive = true;
        return new WarehouseReactivated(Id);
    }
}