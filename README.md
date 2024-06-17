# ![](https://i.imgur.com/jM8yQCt.png)  Radzinsky
That one single Telegram bot to replace all the others.

### Solution structure
- [Radzinsy.Host](src/Radzinsky.Host) — ASP.NET Core web application that hosts a Telegram webhook or performs long polling
- [Radzinsy.Endpoints](src/Radzinsky.Endpoints) — crystal clean business logic; endpoint here basically means a use case
- [Radzinsy.Framework](src/Radzinsky.Framework) — boilerplate that makes business logic look crystal clean
- [Radzinsy.Persistence](src/Radzinsky.Persistence) — database contexts, entities, migration, and all that

### Flow
1. The Framework's discovery services scan the Endpoints assembly, collecting <kbd>IEndpoint</kbd> implementations with specific attributes.
1. Telegram update comes from Telegram through the Host's <kbd>WebhookController</kbd> or in response to a long polling request.
1. It reaches the Framework's <kbd>UpdateHandler</kbd> and then gets challenged by the Framework's routers.
1. Each router tries to classify the update using its discovery service and then form a <kbd>Route</kbd> based on it.
1. When a matching endpoint is finally found, it gets called. If there is no such endpoint, <kbd>NoMatchingEndpointException</kbd> is thrown.
1. Endpoint implementation handles the update in a separate DI scope, given both the <kbd>Update</kbd> and the <kbd>Route</kbd>.
1. If needed, route type is specified (for example, into <kbd>RegExRoute</kbd>) and routing details (such as command's alias and parameters) are obtained.

### When running locally
- Be humble and prefer <kbd>dotnet run</kbd> over Docker Compose to save time.
- Prefer using long polling over webhook. Otherwise, suffer with [ngrok](https://ngrok.com/docs/getting-started) or [localtunnel](https://loca.lt).
- If using [ngrok](https://ngrok.com/docs/getting-started), consider [signing in via auth token](https://dashboard.ngrok.com/get-started/your-authtoken) to get unlimited lifetime for your tunnel.
- To use long polling, just leave the <kbg>Telegram:WebhookHost</kbd> configuration null.
- Use [.NET user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to keep your configuration safe and JSON.
- Give this repo a star.

### When deploying to production
- Don't use [ngrok](https://ngrok.com/docs/getting-started) or [localtunnel](https://loca.lt). Get a VPS.
- If using [ngrok](https://ngrok.com/docs/getting-started), consider [signing in via auth token](https://dashboard.ngrok.com/get-started/your-authtoken) to get unlimited lifetime for your tunnel.
- Copy <kbd>.env.example</kbd> into <kbd>.env</kbd> (which is Git-ignored already), then modify the latter. Now run Docker Compose.
- Prefer webhook over long polling by setting <kbd>TELEGRAM__WEBHOOKHOST</kbd>.
- Give this repo a star.

### Configuration
Using double underscores in the environment variable names makes it possible to use both JSON configuration providers (for example, <kbd>appsettings.Production.json</kbd> or .NET user secrets) and environment variables seamlessly. What makes it work this way is Microsoft.Extensions.Configuration, that treats <kbd>TELEGRAM__TOKEN</kbd> and <kbd>Telegram:Token</kbd> as equivalent keys.

- `Telegram:Token` — Telegram Bot API token, issued at [BotFather](https://t.me/BotFather)
- `Telegram:WebhookHost` — null if you want to use long polling, otherwise the hostname of your webhook server; the final webhook URL is built by the following template: <kbd>{Telegram:WebhookHost}/bot/{Telegram:Token}</kbd>
- `Google:Token` — API key of your Google CSE, see the [tutorial](https://developers.google.com/custom-search/docs/tutorial/introduction)
- `Google:Cx` — Programmable Search Engine ID, get one from the [control panel](https://programmablesearchengine.google.com/controlpanel/all)

If using <kbd>.env</kbd>, see <kbd>.env.example</kbd>.

### Useful links
- [Radzinsky](https://t.me/radzinsky_bot) — its majesty
- [Степной ишак](https://t.me/undrcrxwn) — the one writing all this
