using Infrastructure.Enum;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure;
public class Employee: NamedEntity
{
    public int EmployeeID
    {
        get;
        set;
    }
    public string FirstName
    {
        get;
        set;
    }

    public string? LastName
    {
        get;
        set;
    }

    public UserRole Role
    {
        get;
        set;
    }

    public string Email
    {
        get;
        set;
    }

    public string? PhoneNumber
    {
        get;
        set;
    }

}
