using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

/// <summary>
///  Набор функций и типов данных для работы с логом событий программы
/// </summary>
static class LogHelper
{
    /// <summary>
    /// Имя файла лога. 
    /// </summary>
    private const string LOG_FILE_NAME = "webservice_log";
    /// <summary>
    /// Расширение файла лога.
    /// </summary>
    private const string LOG_FILE_EXTENTION = "txt";
    /// <summary>
    /// Размер в байтах файла лога, после превышения которого происходит его копирование с новым именем.
    /// </summary>
    private const int LOG_FILE_SIZE_THRESHOLD = 3145728;
    /// <summary>
    /// Максимально допустимое количество попыток выполнить операцию с log-файлом если отсутствует доступ к нему.
    /// </summary>
    private const int LOGFILE_WRITE_ATTEMPTS_THRESHOLD = 2;

    /// <summary>
    /// Переменная-объект для блокировки файла, в который пишется лог.
    /// </summary>
    private static readonly object _logFileLockObject = new object();

    /// <summary>
    /// Помещение переданной записи в лог программы. Запись момещается в лог только если включено логирование debug-сообщений в настройках.
    /// </summary>
    /// <param name="message"> Текст сообщения </param>
    /// <param name="sendToServer"> Следует ли отправлять сообщение на сервер </param>
    public static void Debug(string message, bool sendToServer = false)
    {
        // *** Отладочные сообщения не пишутся в лог для уменьшения его размера.
        WriteDbLine(message);
        message = $"{DateTime.Now} (debug) {message}";
        WriteFileLine(message);
    }

    /// <summary>
    /// Помещение переданной записи в лог программы
    /// </summary>
    /// <param name="message"> Текст сообщения </param>
    /// <param name="sendToServer"> Следует ли отправлять сообщение на сервер </param>
    public static void Info(string message, bool sendToServer = false)
    {
        // *** Информационные сообщения не пишутся в лог для уменьшения его размера.
        WriteDbLine(message);
        message = $"{DateTime.Now} {message}";
        WriteFileLine(message);
    }

    /// <summary>
    /// Помещение переданной записи в лог программы. Запись момещается в лог только если включено логирование warning-сообщений в настройках.
    /// </summary>
    /// <param name="message"> Текст сообщения </param>
    /// <param name="sendToServer"> Следует ли отправлять сообщение на сервер </param>
    public static void Warn(string message, bool sendToServer = false)
    {
        WriteDbLine(message);
        message = $"{DateTime.Now} (warning!) {message}";
        WriteFileLine(message);
    }

    /// <summary>
    /// Помещение переданной записи в файл
    /// </summary>
    /// <param name="messageText"></param>
    private static void WriteFileLine(string messageText)
    {
        try
        {
            //Составление полного пути файла лога.
            string strFileFullPath = Path.Combine(CommonLogic.GetApplicationStartupPath(), Path.ChangeExtension(LOG_FILE_NAME, LOG_FILE_EXTENTION));

            // Создание критической секции при записи в файл. Без этого возникают ошибки при многопоточной работе.
            lock (_logFileLockObject)
            {
                bool writeSuccessful = false;
                int writeAttempts = 0;

                // Производится несколько попыток выполнить операцию с log-файлом если с первой попытки записать строку в файл или скопировать его с другим именем не удалось.
                while (!writeSuccessful && writeAttempts < LOGFILE_WRITE_ATTEMPTS_THRESHOLD)
                {
                    try
                    {
                        // Если размер файла превышает заранее установленный порог, то файл переименовывается в формат <ИмяФайла>_<МеткаДатавремени>.txt.
                        if (File.Exists(strFileFullPath) && (new FileInfo(strFileFullPath)).Length > LOG_FILE_SIZE_THRESHOLD)
                        {
                            string datetimeStamp = $"{DateTime.Now.Year.ToString("D4", CultureInfo.InvariantCulture)}{DateTime.Now.Month.ToString("D2", CultureInfo.InvariantCulture)}{DateTime.Now.Day.ToString("D2", CultureInfo.InvariantCulture)}{DateTime.Now.Hour.ToString("D2", CultureInfo.InvariantCulture)}{DateTime.Now.Minute.ToString("D2", CultureInfo.InvariantCulture)}{DateTime.Now.Second.ToString("D2", CultureInfo.InvariantCulture)}{DateTime.Now.Millisecond.ToString("D4", CultureInfo.InvariantCulture)}";

                            string bigLogFileName = $"{CommonLogic.GetApplicationStartupPath()}\\{LOG_FILE_NAME}_{datetimeStamp}.{LOG_FILE_EXTENTION}";
                            File.Move(strFileFullPath, bigLogFileName);
                        }

                        // Создание потока для записи файла.
                        using (StreamWriter logFile = new StreamWriter(strFileFullPath, true, Encoding.Unicode))
                        {
                            // Помещение строки в файл.
                            logFile.WriteLine(messageText);
                            writeSuccessful = true;
                        }
                    }
                    catch (IOException)
                    {
                        Interlocked.Increment(ref writeAttempts);
                    }
                }
            }
        }
        catch (Exception)
        {
            // Логгирование не является критически важной операцией, ошибки в ней не должны влиять на работу приложения, поэтому они игнорируются.
        }
    }

    /// <summary>
    /// Помещение переданной записи в таблицу SQL-server.
    /// </summary>
    private static void WriteDbLine(string messageText)
    {
        try
        {
            //ParamNameValue prmErrorData = new ParamNameValue("errorData", messageText);
            //SqlDataAccessLogic.UpdateInsertDeleteFunction("[dbo].[WebserviceErrorDataWrite]", prmErrorData);
        }
        catch (Exception)
        {
            // Логгирование не является критически важной операцией, ошибки в ней не должны влиять на работу приложения, поэтому они игнорируются.
        }
    }
}