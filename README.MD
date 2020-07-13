# Bot Framework Twitch Adapter

The TwitchAdapter is a C# component which allows you to add Twitch as an endpoint for a bots built with the [Microsoft Bot Framework SDK](https://github.com/Microsoft/botframework-sdk). 
All Twitch integration is achieved through the [Twitch IRC interface](https://dev.twitch.tv/docs/irc). The C# API support for the IRC interface is provided by the [TwitchLib Client](https://github.com/TwitchLib).

# Project Structure
**NegativeEddy.Bots.Twitch** - contains the Twitch adapter

**NegativeEddy.Bots.Twitch.AspNetCore** - contains support for ASP.NET Core integration

**NegativeEddy.Bots.Twitch.AspNetHost** - an ASP.NET Core sample which hosts the bot as a background service

**NegativeEddy.Bots.Twitch.BlazorHost** - an ASP.NET Core sample which hosts the bot as a background service along side a server side Blazor app. The Blazor app has a simple UI that allows some configuration of the TwitchAdapter

**NegativeEddy.Bots.Twitch.ConsoleHost** - a .NET Core Console sample which hosts the bot. The console accepts a few commands to configure the TwitchAdapter


# Requirements
To register a bot with Twitch

    1. create a twitch account for the bot's identity
       - make note of the account's ID
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
As part of the services configuration
 1. register your bot as an IBot inteface 
 2. call the AddTwitchBotAdapter() extension method with a populated TwitchAdapterSettings object.
 
```
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddTransient<IBot, TwitchBot>(); 
 
    var twitchSettings = new TwitchAdapterSettings();
    Configuration.GetSection("twitchBot").Bind(twitchSettings);
    services.AddTwitchBotAdapter(twitchSettings);
    ...
}
```

This will register the TwitchBotWorker background service which will set up the TwitchBotAdapter connected to your IBot implementation.

The TwitchBotAdapter has methods on it to manage the twitch client directly if needed. e.g. Joining/Leaving channels. It is registered in the services collection as a singleton. To access it, just inject it and call the methods on it.

# Optional Configuration
The twitchBot settings section has optional values that will override the adapter defaults
```
{
  "twitchBot": {
      "UserId": "[bot's user ID]",
      "ClientId": "[client ID]",
      "OAuthToken": "[auth Token]"
      "InitialChannels" : ["channel1", "channel2"],
      "ThrottlingMessagesAllowedInPeriod": "750",
      "ThrottlingPeriodInSeconds": "30"
  }
}
```
### InitialChannels
default value: none

This parameter is a string array of channel names that the twitch adapter will connect to after initialization. If left empty, no channels are connected by default.
### ThrottlingMessagesAllowedInPeriod 
default value: 750
### ThrottlingPeriodInSeconds
default value: 30
