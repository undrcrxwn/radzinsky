# ![](https://i.imgur.com/jM8yQCt.png)  Radzinsky
That one single Telegram bot to replace all the others

### Upcoming updates
- Group management
- Group games
- Group vs group chatting sessions
- Personal and group statistics
- Personal and group bios
- More flexible interaction process

### Solution structure
- [`Radzinsy.Host`](src/Radzinsky.Host) — web app to host Telegram webhooks
- [`Radzinsy.Application`](src/Radzinsky.Application) — application layer with everything needed to handle a Telegram update
- [`Radzinsy.Domain`](src/Radzinsky.Domain) — domain models and services that do not depent on anything including Telegram APIs
- [`Radzinsy.Persistence`](src/Radzinsky.Persistence) — persistence layer containing database contexts and migrations

### Useful links
- [Radzinsky](t.me/radzinsky_bot) — the bot
- [gгёzы](t.me/undrcrxwn) — developer
