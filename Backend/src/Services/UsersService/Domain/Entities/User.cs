using UsersService.Domain.Entities.Base;

namespace UsersService.Domain.Entities;

public class User : BaseEntity, IEntity<Guid>
{
    public User() { }
    
    public User(
        Guid id,
        string lastName,
        string firstName,
        string? middleName,
        string email,
        string userName,
        string passwordHash,
        bool isEmailConfirmed,
        bool activateUser)
    {
        Id = id;
        LastName = lastName;
        FirstName = firstName;
        MiddleName = middleName;
        Email = email;
        UserName = userName;
        PasswordHash = passwordHash;
        IsEmailConfirmed = isEmailConfirmed;
        IsActive = activateUser;
    }

    public User(
        string lastName,
        string firstName,
        string? middleName,
        string userName,
        string email,
        string passwordHash)
    {
        LastName = lastName;
        FirstName = firstName;
        MiddleName = middleName;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
    }

    public User(
        string lastName,
        string firstName,
        string? middleName,
        string userName,
        string email,
        string passwordHash,
        bool isEmailConfirmed,
        bool activateUser)
    {
        LastName = lastName;
        FirstName = firstName;
        MiddleName = middleName;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
        IsEmailConfirmed = isEmailConfirmed;
        IsActive = activateUser;
    }
    
    public Guid Id { get; set; }
    
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }

    
    public string UserName { get; set; }
    
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }

    public bool IsEmailConfirmed { get; set; }
    
    public bool IsActive { get; set; } = true;
 
    public virtual ICollection<EmailChangeToken> EmailChangeTokens { get; set; }
    public virtual ICollection<EmailConfirmationToken> EmailConfirmationTokens { get; set; }
    public virtual ICollection<ResetPasswordCode> PasswordResetCodes { get; set; }
    
    public virtual ICollection<Session> Sessions { get; set; }

    public virtual ICollection<Role> Roles { get; set; }
    public virtual ICollection<UserRoles> UserRoles { get; set; }
}