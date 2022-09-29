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
1. Install [`ngrok`](https://ngrok.com/docs/getting-started)
1. Start a forwarding tunnel using `ngrok http 8443`
1. Copy the public ngrok URL you've got
1. Paste it into the `BotConfiguration:HostAddress` field of [`appsettings.Production.json`](src/Radzinsky.Host/appsettings.Production.json)
1. Fill the rest of configuration (e.g. `GoogleSearch:ApiKey` and so on)
1. Run `dotnet run --project src/Radzinsky.Host/Radzinsky.Host.csproj --configuration Release --environment Production`
1. Give this repo a star

### If using PM2
- `pm start ngrok -- 8443` to start a tunnel
- `curl http://localhost:4040/api/tunnels` to get the ngrok public URL

### Useful links
- [Radzinsky](https://t.me/radzinsky_bot) — the bot
- [gгёzы](https://t.me/undrcrxwn) — developer
