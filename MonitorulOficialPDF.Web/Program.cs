using MonitorulOficialPDF.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient("MonitorScraper", client =>
{
    client.BaseAddress = new Uri("https://monitoruloficial.ro");
    client.DefaultRequestHeaders.Add("accept", "*/*");
    client.DefaultRequestHeaders.Add("accept-language", "ro-RO,ro;q=0.9,en-US;q=0.8,en;q=0.7,it;q=0.6");
    client.DefaultRequestHeaders.Add("origin", "https://monitoruloficial.ro");
    client.DefaultRequestHeaders.Add("priority", "u=1, i");
    client.DefaultRequestHeaders.Add("referer", "https://monitoruloficial.ro/e-monitor/");
    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
    client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
    client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
    client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
    client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
    client.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
    client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
});

builder.Services.AddHttpClient("PdfDownloader", client =>
{
    client.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");
    client.DefaultRequestHeaders.Add("cache-control", "no-cache");
    client.DefaultRequestHeaders.Add("pragma", "no-cache");
    client.DefaultRequestHeaders.Add("priority", "u=0, i");
    client.DefaultRequestHeaders.Referrer = new Uri("https://monitoruloficial.ro/e-monitor/");
    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
    client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
    client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
    client.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
    client.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
    client.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
    client.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
    client.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
});

builder.Services.AddScoped<MonitorScraperService>();
builder.Services.AddScoped<PdfDownloadService>();
builder.Services.AddScoped<OcrService>();
builder.Services.AddScoped<AzureOpenAiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
