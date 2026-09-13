using Minicraft.Applications.Client;

try
{
    var app = new ClientApplication();
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"[ERROR] {ex.Message}\n{ex.StackTrace}");
}
