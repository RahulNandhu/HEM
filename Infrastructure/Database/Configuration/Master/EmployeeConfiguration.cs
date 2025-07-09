namespace Infrastructure;

using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class EmployeeConfiguration: NamedEntityConfiguration<Employee>
{
    public override void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasIndex(e => e.EmployeeID)
            .IsUnique();
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.LastName)
            .HasMaxLength(100);
        builder.Property(e => e.Email)
            .HasMaxLength(100);
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20);
        builder.Property(e => e.Role)
            .IsRequired();
        base.Configure(builder);
    }
}
