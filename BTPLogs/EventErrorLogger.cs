using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTPLogs
{
    /// <summary>
    /// Log ostatnich 50 błędów
    /// </summary>
    public class EventErrorLogger
    {
        List<ErrorStruct> _errorList = new List<ErrorStruct>();

        /// <summary>
        /// Kasuje wszystkie logi
        /// </summary>
        public void ClearErrorLogger()
        {
            try
            {
                _errorList.Clear();
            }
            catch { }
        }

        /// <summary>
        /// Dodaje nowy log
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        public void AddLog(ErrorType type, string[] description)
        {
            try
            {
                if (_errorList.Count > 50)
                    _errorList.RemoveAt(0);

                _errorList.Add(new ErrorStruct
                {
                    Time = System.DateTime.Now,
                    Type = type,
                    Description = description
                });
            }
            catch { }
        }

        public string[][] GetErrorDescByErrorType(ErrorType type)
        {
            try
            {
                return _errorList
                    .Where(r => r.Type == type)
                    .Select(r => r.Description)
                    .ToArray();
            }
            catch
            {
                return null;
            }
        }

        public string[][] GetErrorDescByErrorTypes(ErrorType[] type)
        {
            try
            {
                return _errorList
                    .Where(r => type.Contains(r.Type))
                    .Select(r => r.Description)
                    .ToArray();
            }
            catch
            {
                return null;
            }
        }

        public ErrorStruct[] GetAllErrorData()
        {
            try
            {
                return _errorList.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public object GetErrorDescAndTimeByErrorType(ErrorType type)
        {
            try
            {
                return _errorList
                    .Where(r => r.Type == type)
                    .Select(r => new { r.Time, r.Description })
                    .ToArray();
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Dane błędów
    /// </summary>
    public struct ErrorStruct
    {
        public DateTime Time;
        public ErrorType Type;
        public String[] Description;
    }

    /// <summary>
    /// Typ błędu
    /// </summary>
    public enum ErrorType
    {
        InputInterface,
        PlcOnNotification
    }
}
