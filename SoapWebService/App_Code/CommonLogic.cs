using System.Web;

/// <summary>
/// Основная логика системы
/// </summary>
public abstract class CommonLogic
{
    /// <summary>
    /// Получение пути размещения на диске веб-приложения.
    /// </summary>
    internal static string GetApplicationStartupPath()
    {
        return HttpRuntime.AppDomainAppPath;
    }
}