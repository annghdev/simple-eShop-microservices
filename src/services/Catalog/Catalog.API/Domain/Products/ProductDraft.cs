namespace Catalog.API;

public class ProductDraft
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public decimal Price { get; set; }
    public string MainImage { get; set; } = string.Empty;
    public string SecondaryImage { get; set; } = string.Empty;
    public ProductDimensions Dimensions { get; set; } = new ProductDimensions(1, 1, 1, 1);
    public Guid? CategoryId { get; set; }
    public List<string> Images { get; set; } = [];
    public List<ProductAttribute> Attributes { get; set; } = [];
    public List<Variant> Variants { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public ProductDraftStep CurrentStep { get; set; } = ProductDraftStep.Step0_Init;

    public bool Validate()
    {
        // simulate

        return true;
    }

    public void Confirm()
    {
        if (!Validate())
            throw new InvalidOperationException("StepX invalid");
        CurrentStep = ProductDraftStep.Step6_Confirm;
    }

    public Product Publish()
    {
        if (CurrentStep != ProductDraftStep.Step6_Confirm)
            throw new InvalidOperationException("Can not publish product before confirm");
        return Product.Publish(this);
    }
}

public enum ProductDraftStep
{
    Step0_Init = 0,
    Step1_Basic_Information = 1,
    Step2_Dimensions = 2,
    Step3_Attributes = 3,
    Step4_Variants = 4,
    Step5_Images = 5,
    Step6_Confirm = 6,
}
