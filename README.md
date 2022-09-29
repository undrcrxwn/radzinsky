# ![](https://i.imgur.com/jM8yQCt.png)  Radzinsky
That one single Telegram bot to replace all the others

### Upcoming updates
- Group management
- Group games
- Group vs group chatting sessions
- Personal and group statistics

### Solution structure
- [`Radzinsy.Host`](src/Radzinsky.Host) — web app to host Telegram webhooks
- [`Radzinsy.Application`](src/Radzinsky.Application) — application layer with everything needed to handle a Telegram update
- [`Radzinsy.Domain`](src/Radzinsky.Domain) — domain models and services that do not depent on anything including Telegram APIs
- [`Radzinsy.Persistence`](src/Radzinsky.Persistence) — persistence layer containing database contexts and migrations

### Launching
1. Install [`ngrok`](https://ngrok.com/docs/getting-started) or [`localtunnel`](https://localtunnel.github.io/www)
1. Open a tunnel using `ngrok http 8443` or `lt --port 8443`
1. Copy the ngrok or localtunnel URL you've got
1. Paste it into the `BotConfiguration:HostAddress` configuration variable of [`appsettings.Production.json`](src/Radzinsky.Host/appsettings.Production.json)
1. Fill the rest of configuration variables (e.g. `GoogleSearch:ApiKey` and so on)
1. Run `dotnet run --project src/Radzinsky.Host/Radzinsky.Host.csproj --configuration Release --environment Production`
1. Give this repo a star

### Useful links
- [Radzinsky](https://t.me/radzinsky_bot) — the bot
- [gгёzы](https://t.me/undrcrxwn) — developer
