namespace Infrastructure;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class NamedEntityConfiguration<T> : ActiveEntityConfiguration<T>
     where T : NamedEntity
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(it => it.Name).IsRequired();

        base.Configure(builder);
    }
}
