app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions("Auth0")
{
    ClaimsIssuer = "Auth0"

    Authority = "https://YOUR_AUTH0_DOMAIN",
    ClientId = "APP_CLIENT_ID",
    ClientSecret = "APP_CLIENT_SECRET",

    AutomaticAuthenticate = true, 
    AutomaticChallenge = true,

    ResponseType = "code",

    CallbackPath = new PathString("/signin-auth0"),
    
    SaveTokens = true,
    
    Events = new OpenIdConnectEvents
    {
        OnTicketReceived = context =>
        {
            var identity = context.Principal.Identity as ClaimsIdentity;
            if (identity != null)
            {
                if (context.Properties.Items.ContainsKey(".TokenNames"))
                {
                    string[] tokenNames = context.Properties.Items[".TokenNames"].Split(';');
                    
                    foreach(string tokenName in tokenNames)
                    {
                        string tokenValue = context.Properties.Items[$".Token.{tokenName}"];
                        
                        if (!identity.HasClaim(c => c.Type == tokenName))
                            identity.AddClaim(new Claim(tokenName, tokenValue));
                    }
                }
            }

            return Task.FromResult(0);
        }
    }
});