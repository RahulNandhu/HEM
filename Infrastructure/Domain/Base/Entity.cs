namespace Infrastructure;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public abstract class Entity
{
    public Entity()
            : this(Guid.NewGuid())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public Entity(Guid id)
    {
        this.ReferenceId = id;
        this.IsActive = true;
        this.CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public virtual Guid ReferenceId
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the created on.
    /// </summary>
    /// <value>
    /// The created on.
    /// </value>
    public virtual DateTime CreatedOn
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is active.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
    /// </value>
    public bool IsActive
    {
        get;
        set;
    }
}
