using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Suki.Utils
{
    public class AppConstants
    {
        #region Session Objects
        public const string CompanyName = "CompanyName";
        public const string CompanyCode = "CompanyCode";
        public const string UserCode = "UserCode";
        public const string Pwd = "Pwd";
        public const string UserID = "UserID";
        public const string ConString = "ConString";
        public const string IsSupperUser = "IsSupperUser";
        public const string IsCompanySuperUser = "IsCompanySuperUser";
        public const string ListCompany = "ListCompany";
        public const string ListOutlet = "ListOutlet";
        public const string OutletCode = "OutletCode";
        public const string OutletName = "OutletName";
        public const string DATE = "dd/MM/yyyy";
        public const string NUMBER_FORMAT = "#,##0.0000";
        public const string NUMBER_FORMAT_STOCKTAKE = "#,##0.0000";
        public const string EmailLogPath = "Email";
        public const string InternalLogPath = "Internal";
        public const string ExternalLogPath = "External";
        public const string SOStatus_OnSearch = "Failed";
        #endregion

        public static bool isInteger(string integer)
        {
            try
            {
                int.Parse(integer);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool isDateTime(string dateTime)
        {
            try
            {
                string DateTimeString = string.Empty;
                if (dateTime.Contains("-"))
                { DateTimeString = dateTime.Replace("-", "/").ToString(); }
                else
                {
                    DateTimeString = dateTime;
                }

                //DateTime.ParseExact(dateTime, Utils.AppConstants.DATE, null, System.Globalization.DateTimeStyles.None);
                DateTime.ParseExact(DateTimeString, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool isDouble(string strDouble)
        {
            try
            {
                double.Parse(strDouble);
            }
            catch
            {
                return false;
            }
            return true;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        public static DateTime ConvertToDate(string strDate)
        {
            string DateTimeString = string.Empty;
            if (strDate.Contains("-"))
            { DateTimeString = strDate.Replace("-", "/").ToString(); }
            else
            {
                DateTimeString = strDate;
            }
            //return DateTime.ParseExact(strDate, Utils.AppConstants.DATE, null, System.Globalization.DateTimeStyles.None);
            return DateTime.ParseExact(DateTimeString, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 
        /// </summary>
        public enum SOStatus
        {
            New,
            Failed,
            Done,
            Pass
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Str"></param>
        public static void WriteLog(string Str, bool xmlLogPath = false)
        {
            try
            {
                System.IO.StreamWriter oWrite = null;
                string FilePath = null;
                if (xmlLogPath == false)
                {
                    FilePath = System.Configuration.ConfigurationManager.AppSettings["LogPath"]; //+ DateTime.Now.ToString("yyyyMMdd");
                }
                else
                {
                    FilePath = System.Configuration.ConfigurationManager.AppSettings["XMLLogPath"]; //+DateTime.Now.ToString("yyyyMMdd");
                }

                if (System.IO.File.Exists(FilePath))
                {
                    oWrite = System.IO.File.AppendText(FilePath);
                }
                else
                {
                    oWrite = System.IO.File.CreateText(FilePath);
                }

                oWrite.WriteLine(DateTime.Now.ToString() + ":" + Str);
                oWrite.Close();
            }
            catch (Exception ex)
            {
            }
        }

        public static void DebugLog(string Str, string path, bool xmlLogPath = false)
        {
            try
            {
                System.IO.StreamWriter oWrite = null;
                string FilePath = null;
                if (xmlLogPath == false)
                {
                    if (path == "Email")
                    {
                        FilePath = System.Configuration.ConfigurationManager.AppSettings["EmailLogPath"]; //+ DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    }
                    else if (path == "Internal")
                    {
                        FilePath = System.Configuration.ConfigurationManager.AppSettings["InternalLogPath"];// +DateTime.Now.ToString("yyyyMMdd");
                    }
                    else if (path == "External")
                    {
                        FilePath = System.Configuration.ConfigurationManager.AppSettings["ExternalLogPath"];// +DateTime.Now.ToString("yyyyMMdd");
                    }

                }
                else
                {
                    FilePath = System.Configuration.ConfigurationManager.AppSettings["XMLLogPath"];// +DateTime.Now.ToString("yyyyMMdd");
                }

                if (System.IO.File.Exists(FilePath))
                {
                    oWrite = System.IO.File.AppendText(FilePath);
                }
                else
                {
                    oWrite = System.IO.File.CreateText(FilePath);
                }
                if (string.IsNullOrEmpty(Str)) //       (Str == string.Empty)
                {
                    oWrite.WriteLine(Str);
                }
                else
                {
                    oWrite.WriteLine(DateTime.Now.ToString() + ":" + Str);
                }
                oWrite.Close();
            }
            catch (Exception ex)
            {
            }
        }
    }
}