# ![](https://i.imgur.com/jM8yQCt.png)  Radzinsky
That one single Telegram bot to replace all the others

### Upcoming updates
- Group management
- Group games
- Group vs group chatting sessions
- Personal and group statistics

### Solution structure
- [Radzinsy.Host](src/Radzinsky.Host) — web app to host Telegram webhooks
- [Radzinsy.Application](src/Radzinsky.Application) — application layer with everything needed to handle a Telegram update
- [Radzinsy.Domain](src/Radzinsky.Domain) — domain models and services that do not depent on anything including Telegram APIs
- [Radzinsy.Persistence](src/Radzinsky.Persistence) — persistence layer containing database contexts and migrations

### Launching
1. Install [ngrok](https://ngrok.com/docs/getting-started) (or use [localtunnel](https://loca.lt) instead)
1. If using ngrok, consider [signing in via auth token](https://dashboard.ngrok.com/get-started/your-authtoken) to get unlimited lifetime for your tunnel
1. Run a forwarding HTTP tunnel using `ngrok http 8443`
1. Copy the public ngrok URL you've got
1. Initialize [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for [Radzinsky.Host](src/Radzinsky.Host)
1. Paste the copied ngrok public URL into the `Telegram:WebhookHost` field of the user secrets configuration
1. Fill the rest of user secrets (see [appsettings.json](src/Radzinsky.Host/appsettings.json))
1. Run `dotnet run --project src/Radzinsky.Host/Radzinsky.Host.csproj --configuration Release --environment Production`
1. Give this repo a star

### If using [PM2](https://pm2.keymetrics.io)
- `pm2 start ngrok -- http 8443` to start a tunnel
- `curl http://localhost:4040/api/tunnels` to get the ngrok public URL
- Running webhook host process: `pm2 start dotnet -- run --project src/Radzinsky.Host/Radzinsky.Host.csproj --configuration Release --environment Production`

### Useful links
- [Radzinsky](https://t.me/radzinsky_bot) — the bot
- [Radzinsky Community](https://t.me/radzinsky_chat) — community
- [gгёzы](https://t.me/undrcrxwn) — developer
