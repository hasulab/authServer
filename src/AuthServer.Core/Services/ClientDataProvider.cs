namespace AuthServer.Services;

public class ClientDataProvider
{
    public ClientDataProvider() 
    {
        
    }

    public virtual bool ValidateClient(string clientId)
    {
        var list = new List<string>()
        {
            "00000000-0000-0000-0000-000000000001",
            "00000001-0000-0000-a000-000000000001",
            "00000002-0000-0000-b000-000000000001"
        };
        return list.Any(x => x == clientId);
    }

    public virtual bool ValidateApp(string appId)
    {
        var list = new List<string>()
        {
            "00000000-0000-0000-0000-000000000002",
            "00000001-0000-0000-a000-000000000002",
            "00000002-0000-0000-b000-000000000002"
        };
        return list.Any(x => x == appId);
    }
    
    public virtual string[] GetResponseTypes(Guid tenantId, OAuthTokenRequest tokenRequest)
    {
        var response_type = string.Empty;
        switch (tokenRequest.grant_type)
        {
            case GrantType.client_credentials:
                response_type = tokenRequest.response_type ?? ResponseType.access_token;
                break;
            case GrantType.password:
                response_type = tokenRequest.response_type ?? ResponseType.id_token;
                break;
        }

        return response_type.Split('+',' ');
    }

    public virtual StoredUser ValidateSecret(Guid tenantId, OAuthTokenRequest tokenRequest)
    {
        return new StoredUser
        {
            Id = "10000000-0000-0000-0000-000000000001"
        };
    }

    public virtual StoredUser ValidateUserPassword(Guid tenantId, OAuthTokenRequest tokenRequest)
    {
        return new StoredUser
        {
            Id = "10000000-0000-0000-0000-000000000002",
            Name="Test",
            Email="test@test.com",
            UserName="Test",
            Roles= new string[] { "Admin", "Dev", "QA" }
        };
    }

    public class StoredUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string[] Roles { get; set; }
    }
}