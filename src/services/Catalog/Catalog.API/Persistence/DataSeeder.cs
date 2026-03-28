using Catalog.Domain;
using Marten;

namespace Catalog.Persistence;

public class DataSeeder(IDocumentSession session)
{
    private static readonly Guid ColorAttributeId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid StorageAttributeId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static readonly Guid ElectronicsCategoryId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid SmartPhoneCategoryId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private static readonly Guid IPhone15ProductId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid IPhone15Black128VariantId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid IPhone15Blue256VariantId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public async Task SeedAsync()
    {
        await SeedAttributes();
        await SeedCategories();
        await SeedProducts();

        await session.SaveChangesAsync();
    }

    private async Task SeedAttributes()
    {
        if (await session.LoadAsync<Catalog.Domain.Attribute>(ColorAttributeId) is null)
        {
            session.Store(new Catalog.Domain.Attribute
            {
                Id = ColorAttributeId,
                Name = "Color",
                Values =
                [
                    new AttributeValue
                    {
                        Text = "Black",
                        BgStyleClass = "bg-black",
                        TextStyleClass = "text-white",
                        BorderStyleClass = "border-neutral-800"
                    },
                    new AttributeValue
                    {
                        Text = "Blue",
                        BgStyleClass = "bg-blue-500",
                        TextStyleClass = "text-white",
                        BorderStyleClass = "border-blue-600"
                    }
                ]
            });
        }

        if (await session.LoadAsync<Catalog.Domain.Attribute>(StorageAttributeId) is null)
        {
            session.Store(new Catalog.Domain.Attribute
            {
                Id = StorageAttributeId,
                Name = "Storage",
                Values =
                [
                    new AttributeValue { Text = "128GB" },
                    new AttributeValue { Text = "256GB" }
                ]
            });
        }
    }

    private async Task SeedCategories()
    {
        if (await session.LoadAsync<Category>(ElectronicsCategoryId) is null)
        {
            session.Store(new Category
            {
                Id = ElectronicsCategoryId,
                Name = "Electronics",
                Icon = "ph:devices"
            });
        }

        if (await session.LoadAsync<Category>(SmartPhoneCategoryId) is null)
        {
            session.Store(new Category
            {
                Id = SmartPhoneCategoryId,
                ParentId = ElectronicsCategoryId,
                Name = "Smart Phones",
                Icon = "ph:device-mobile"
            });
        }
    }

    private async Task SeedProducts()
    {
        if (await session.LoadAsync<Product>(IPhone15ProductId) is not null)
        {
            return;
        }

        session.Store(new Product
        {
            Id = IPhone15ProductId,
            Name = "iPhone 15",
            ShortDescription = "Flagship smartphone with A16 Bionic chip",
            Description = "iPhone 15 with dynamic island, advanced camera system, and all-day battery life.",
            Sku = "APPLE-IPHONE15",
            Cost = 650m,
            Price = 799m,
            MainImage = "https://images.example.com/iphone15-main.jpg",
            SecondaryImage = "https://images.example.com/iphone15-secondary.jpg",
            Dimensions = new ProductDimensions(7.16, 14.76, 0.78, 0.171),
            CategoryId = SmartPhoneCategoryId,
            Images =
            [
                "https://images.example.com/iphone15-1.jpg",
                "https://images.example.com/iphone15-2.jpg"
            ],
            Attributes =
            [
                new ProductAttribute
                {
                    AttributeId = ColorAttributeId,
                    DisplayOrder = 1,
                    GroupVariants = true
                },
                new ProductAttribute
                {
                    AttributeId = StorageAttributeId,
                    DisplayOrder = 2,
                    GroupVariants = true
                }
            ],
            Variants =
            [
                new Variant
                {
                    Id = IPhone15Black128VariantId,
                    Name = "Black / 128GB",
                    Sku = "APPLE-IPHONE15-BLK-128",
                    Cost = 650m,
                    Price = 799m,
                    MainImage = "https://images.example.com/iphone15-black-128.jpg",
                    Attribtues = new Dictionary<Guid, AttributeValue>
                    {
                        {
                            ColorAttributeId,
                            new AttributeValue
                            {
                                Text = "Black",
                                BgStyleClass = "bg-black",
                                TextStyleClass = "text-white",
                                BorderStyleClass = "border-neutral-800"
                            }
                        },
                        {
                            StorageAttributeId,
                            new AttributeValue { Text = "128GB" }
                        }
                    }
                },
                new Variant
                {
                    Id = IPhone15Blue256VariantId,
                    Name = "Blue / 256GB",
                    Sku = "APPLE-IPHONE15-BLU-256",
                    Cost = 720m,
                    Price = 899m,
                    MainImage = "https://images.example.com/iphone15-blue-256.jpg",
                    Attribtues = new Dictionary<Guid, AttributeValue>
                    {
                        {
                            ColorAttributeId,
                            new AttributeValue
                            {
                                Text = "Blue",
                                BgStyleClass = "bg-blue-500",
                                TextStyleClass = "text-white",
                                BorderStyleClass = "border-blue-600"
                            }
                        },
                        {
                            StorageAttributeId,
                            new AttributeValue { Text = "256GB" }
                        }
                    }
                }
            ],
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        });
    }
}
