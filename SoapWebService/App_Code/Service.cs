using System;
using System.Web.Services;
using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>
/// Веб-служба для обслуживания программы VKrutka
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Service : WebService
{
    /// <summary>
    /// Проверка доступности веб-службы.
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public bool WebserverConnectionTest()
    {
        // Переменная для хранения результата проверки.
        bool blnResult = false;

        try
        {
            // Задержка требуется чтобы снизить нагрузку на сервер.
            // System.Threading.Thread.Sleep(100);
            LogHelper.Debug($"Запущен веб-метод {GetCurrentMethod()}.");

            //Возвращение строки об успешном тесте.
            blnResult = true;
        }
        catch (Exception currentException)
        {
            LogHelper.Warn($"При выполнении веб-метода {GetCurrentMethod()} произошла непредвиденная ошибка: {currentException}");
        }

        // Процедура возвращает результат проверки.
        return blnResult;
    }
    
    /// <summary>
    /// Получение имени текущего метода.
    /// По материалам https://stackoverflow.com/questions/2652460/how-to-get-the-name-of-the-current-method-from-code.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private string GetCurrentMethod()
    {
        var st = new StackTrace();
        var sf = st.GetFrame(1);

        return sf.GetMethod().Name;
    }
}
