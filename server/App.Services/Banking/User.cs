using System.Collections.Generic;

namespace App.Services.Banking;

// User
public class User
{
    public UserPayload Payload { get; set; }
    public double ExecutionTime { get; set; }
}

public class UserPayload
{
    public string ClientUserId { get; set; }
    public List<ConsentUserDetails> ConsentUserDetails { get; set; }
}

public class ConsentUserDetails
{
    public string BankBIC { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullLegalName { get; set; }
    public string TaxId { get; set; }
}