using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace App.Services.Banking;

// Connections
public class UserConnections
{
    public UserConnection Payload { get; set; }
    public double ExecutionTime { get; set; }
}

public class UserConnection
{
    public List<UserConnectionsBank> Banks { get; set; }
    public string ClientUserId { get; set; }
}

public class UserConnectionsBank
{
    public string ConnectionId { get; set; }
    public string Country { get; set; }
    public string BankBIC { get; set; }
    public string DisplayName { get; set; }
    public string Status { get; set; }
    public DateTime ValidUntil { get; set; }
}
/*
 {"payload":
	{
	"connectionLink":"https://services.nbg.gr/apis/aggregator.connector/v1/Secure/ETHNGRAA/Token?state=0d9d88b55af34853be8e0fcb9f0dc5d55E420CF5-EB54-436C-9F6D-6BC555948F80",
	"connectionId":"0d9d88b5-5af3-4853-be8e-0fcb9f0dc5d5",
	"country":"GR",
	"bankBIC":"ETHNGRAA",
	"displayName":"NATIONAL BANK OF GREECE S.A."
	},
"executionTime":0.02
}
 */
// Add
public class UserConnectionsAdd
{
    public UserConnectionsAddPayload Payload { get; set; }

    public double ExecutionTime { get; set; }
}

public class UserConnectionsAddPayload
{
    public string ConnectionLink { get; set; }

    public string ConnectionId { get; set; }

    public string Country { get; set; }

    public string BankBIC { get; set; }

    public string DisplayName { get; set; }
}
