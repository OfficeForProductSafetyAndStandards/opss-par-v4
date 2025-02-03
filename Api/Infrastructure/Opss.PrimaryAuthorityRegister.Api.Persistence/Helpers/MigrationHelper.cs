namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Helpers;

internal static class MigrationHelper
{
    internal static string ReadMigrationFile(string fileName)
    {
        var sqlFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations", fileName);
        return File.ReadAllText(sqlFile);
    }
}
