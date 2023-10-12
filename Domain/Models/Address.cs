using System.ComponentModel.DataAnnotations;
using Domain.Common;
using Domain.Events;
using Domain.Extensions;
using FluentResults;

namespace Domain.Models;

public class Address : ValueObject
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public Guid AuthorId { get; init; }
    
    public virtual Author? Author { get; private set; }
    
    private Address() { } // For EF Core
    
    private Address(Guid id, string street, string city, string country, string zipCode, string email, Author author)
    {
        Id = id;
        Street = street;
        City = city;
        Country = country;
        ZipCode = zipCode;
        Email = email;
        Author = author;
        AuthorId = author.Id;
        AddEvent(new AddressCreatedEvent {Id = Guid.NewGuid()});
    }
    
    public static Result<Address> Create(Guid id, string street, string city, string country, string zipCode, string email, Author author)
    {
        var errors = new List<Error>();
        if (string.IsNullOrWhiteSpace(street))
            errors.SetError<ArgumentNullException>(nameof(street));
        if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
            errors.SetError<ArgumentException>(nameof(email));

        if (errors.Count > 0)
            return errors;
        
        return new Address(id, street, city, country, zipCode, email, author);
    }

    public override IEnumerator<object> GetEnumerator()
    {
        yield return Street;
        yield return City;
        yield return Country;
        yield return ZipCode;
        yield return Email;
    }
}