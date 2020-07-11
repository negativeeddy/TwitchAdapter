# To register the bot with Twitch
    1. create a new twitch account (optional but the bot's name will be whatever account is used to register it)
    2. enable 2-factor auth (required to manage twitch apps)
    3. create an auth token at https://twitchapps.com/
        - make note of the token
    4. register an app at https://dev.twitch.tv/console
        - make note of the client ID
    5. update appsettings.json with the new values

AppSettings fields required
``` 
{
  "twitchBot": {
      "UserId": "[bot's user ID]",
      "ClientId": "[client ID]",
      "OAuthToken": "[auth Token]"
  }
}
```

## Usage in an ASP.NET app
Register your bot as an IBot inteface and call the AddTwitchBotAdapter() extension method.
 
```
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddScoped<IBot, TwitchBot>(); 
    services.AddTwitchBotAdapter(Configuration);
}
```

This will register the TwitchBotWorker background service which will set up the TwitchBotAdapter connected to your IBot implementation.

The TwitchBotAdapter has methods on it to manage the twitch client directly if needed. e.g. Joining/Leaving channels. It is registered in the services collection as a singleton. To access it, just inject it and call the methods on it.

# Optional Configuration
The twitchBot settings section has optional values that will override the adapter defaults

### ThrottlingMessagesAllowedInPeriod 
default value: 750
### ThrottlingPeriodInSeconds
default value: 30
