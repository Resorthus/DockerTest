using System.Net;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Serves the files in wwwroot (our two photos).
app.UseStaticFiles();

app.MapGet("/", (IConfiguration config) =>
{
    // Reads the "President" setting. Outside Docker it's missing, so we fall back to Trump.
    // In Docker, set it with:  docker run -e President=Obama ...
    var setting = config["President"];
    var isObama = string.Equals(setting, "Obama", StringComparison.OrdinalIgnoreCase);

    var name  = isObama ? "Obama" : "Trump";
    var image = isObama ? "obama.jpg" : "trump.jpg";
    var shown = WebUtility.HtmlEncode(setting ?? "(not set)");
    var tip   = isObama
        ? "Run the container without <code>-e President</code> to go back to the default."
        : "Run the container with <code>-e President=Obama</code> to switch.";

    var html = $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="utf-8">
          <meta name="viewport" content="width=device-width, initial-scale=1">
          <title>Greetings from the {{name}}</title>
          <style>
            :root { --sky: #D6E4EF; --paper: #FBFCFD; --ink: #1F2F4D; --soft: #55657D; }
            * { box-sizing: border-box; }
            body {
              margin: 0; min-height: 100vh; display: grid; place-items: center;
              background: var(--sky); color: var(--ink);
              font-family: Georgia, "Times New Roman", serif; padding: 24px;
            }
            .card {
              background: var(--paper); border-radius: 6px; padding: 48px 56px;
              display: flex; align-items: center; gap: 56px; flex-wrap: wrap;
              justify-content: center; max-width: 1000px;
              box-shadow: 0 18px 40px rgba(31, 47, 77, 0.18);
            }
            h1 { font-size: clamp(2.5rem, 6vw, 4.5rem); line-height: 1.05; margin: 0; max-width: 8ch; }
            figure { margin: 0; text-align: center; }
            img {
              width: min(360px, 70vw); aspect-ratio: 1; object-fit: cover; display: block;
              border: 14px solid #fff; transform: rotate(2.5deg);
              box-shadow: 0 10px 24px rgba(31, 47, 77, 0.25);
            }
            figcaption { margin-top: 20px; font-size: 1.4rem; font-style: italic; }
            .info { margin-top: 28px; font-family: Arial, sans-serif; color: var(--soft); font-size: 0.95rem; text-align: center; }
            code { background: #E9EEF3; padding: 2px 6px; border-radius: 4px; }
            .blessed { margin: 28px 0 0; font-size: 1.5rem; text-align: center; }
          </style>
        </head>
        <body>
          <div>
            <main class="card">
              <h1>Greetings from the</h1>
              <figure>
                <img src="/images/{{image}}" alt="Photo of {{name}}">
                <figcaption>{{name}}</figcaption>
              </figure>
            </main>
            <p class="info">President setting: <code>{{shown}}</code>. {{tip}}</p>
          </div>
        </body>
        </html>
        """;

    return Results.Content(html, "text/html");
});

app.Run();
