using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Suki
{
    public class GeneralFunctions
    {


        private static MasterData MD;
        private static DataSet ds;
        private CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
        
        public GeneralFunctions()
        {

        }

        public GeneralFunctions(string asUserName)
        {
            MD = new MasterData();
            //ds = MD.GetDisplaySetting(asUserName);
           // ds = MD.GetDisplaySetting("ELECTRA");
        }

        #region Properties
        public string DateFormatString
        {
            get { return "MM/dd/yyyy"; }
        }
        public string DecSep
        {
            get { return (ds.Tables[0].Rows[0]["DecSep"] == null ? "." : ds.Tables[0].Rows[0]["DecSep"].ToString()); }
        }

        public string ThousSep
        {
            get { return (ds.Tables[0].Rows[0]["ThousSep"] == null ? "," : ds.Tables[0].Rows[0]["ThousSep"].ToString()); }
        }

        public int SumDec
        {
            get { return (ds.Tables[0].Rows[0]["SumDec"] == null ? 0 : int.Parse(ds.Tables[0].Rows[0]["SumDec"].ToString())); }
        }

        public int QtyDec
        {
            get { return (ds.Tables[0].Rows[0]["QtyDec"] == null ? 0 : int.Parse(ds.Tables[0].Rows[0]["QtyDec"].ToString())); }
        }

        public int PriceDec
        {
            get { return (ds.Tables[0].Rows[0]["PriceDec"] == null ? 0 : int.Parse(ds.Tables[0].Rows[0]["PriceDec"].ToString())); }
        }

        public int PercentDec
        {
            get { return (ds.Tables[0].Rows[0]["PercentDec"] == null ? 2 : int.Parse(ds.Tables[0].Rows[0]["PercentDec"].ToString())); }
        }

        public int RateDec
        {
            get { return (ds.Tables[0].Rows[0]["RateDec"] == null ? 2 : int.Parse(ds.Tables[0].Rows[0]["RateDec"].ToString())); }
        }
        #endregion

        
        public string BuildKeepColumnStr(DataTable dt)
        {
            string str = "";
            foreach (DataColumn column in dt.Columns)
            {
                str = str + column.ColumnName + ";";
            }
            return str;
        }
        public DataTable ConvertDataTable_RemoveCols(DataTable dt, string KeepColumns)
        {
            DataTable dt1 = dt;
            Array arr = KeepColumns.Split(';');
            CultureInfo ivC = new System.Globalization.CultureInfo("es-US");
            string strcolumnremove = "";
            foreach (var column in dt1.Columns.Cast<DataColumn>().ToArray())
            {
                if (KeepColumns!="")
                {
                    if (Array.IndexOf(arr, column.ColumnName) < 0)
                    {//-------neu ko nam trong danh sach column giu lai, thi delete-----------------
                        dt1.Columns.Remove(column);
                    }
                }
                if (Array.IndexOf(arr, column.ColumnName) >= 0 || KeepColumns=="")
                {
                    if (column.DataType != typeof(string)) //chuyen doi tat ca kieu du lieu, ngoai tru kieu string
                    {
                        strcolumnremove = strcolumnremove + column.ColumnName + ";";
                        dt1.Columns.Add(column.ColumnName + "_1xxx", typeof(string)); //them cot
                        
                        foreach (DataRow row in dt1.Rows)
                        {
                            if (column.DataType == typeof(DateTime) & row[column].ToString()!="")
                            {
                                DateTime d = Convert.ToDateTime(row[column].ToString(), ivC);
                                row[column.ColumnName + "_1xxx"] = String.Format("{0:yyyyMMdd}", d);
                            }
                            else
                                row[column.ColumnName + "_1xxx"] = row[column].ToString();
                        }
                    }
                }
            }
            //Xoa cac cot co kieu du lieu khac string, doi ten cac cot du lieu moi.
            if (strcolumnremove != "")
            {
                Array arr1= strcolumnremove.Split(';');
                for (int i = 0; i < arr1.Length; i++)
                {
                    if (arr1.GetValue(i).ToString() != "")
                    {
                        dt1.Columns.Remove(arr1.GetValue(i).ToString());
                        dt1.Columns[arr1.GetValue(i).ToString() + "_1xxx"].ColumnName = arr1.GetValue(i).ToString();
                    }
                }
            }
            return dt1;
        }

        #region Transfer from Obj to Double
        public Double Object2Double(Object Obj)
        {
            double result = 0.0;
            try
            {
                if (Obj != null)
                {
                    if (Obj.ToString().IndexOf(ci.NumberFormat.NumberGroupSeparator) > 0)
                        Obj = Obj.ToString().Replace(ci.NumberFormat.NumberGroupSeparator, ci.NumberFormat.NumberDecimalSeparator);
                    result = Double.Parse(Obj.ToString(), ci);
                }
            }
            catch 
            {
                result = 0.0;
            }
            return result;        
        }
        #endregion

        #region Transfer from Obj & Display Setting Name
        public Double Object2Double(Object Obj, string attName)
        {
            double result=0.0;
            try
            {
                if (Obj != null)
                {
                    if (Obj.ToString().IndexOf(ci.NumberFormat.NumberGroupSeparator) > 0)
                        Obj = Obj.ToString().Replace(ci.NumberFormat.NumberGroupSeparator, ci.NumberFormat.NumberDecimalSeparator);
                    return result = Math.Round(Double.Parse(Obj.ToString(), ci), GetNumDecimals(attName));
                }
            }
            catch 
            {
                result = 0.0;
            }
            return 0.0;
        }
        #endregion

        #region Get Number Decimal of Display Setting Name
        public int GetNumDecimals(string attName)
        {
            switch (attName)
            {
                case "SumDec":
                    return SumDec;
                case "PriceDec":
                    return PriceDec;
                case "QtyDec":
                    return QtyDec;
                case "PercentDec":
                    return PercentDec;
                case "RateDec":
                    return RateDec;
                default: return 0;
            }
        }
        #endregion

        #region FormatNumeric
        public string FormatNumeric(string strValor, string asNumDecimales)
        {
            if (strValor == "") return "0";

            int intNumDecimales = GetNumDecimals(asNumDecimales);

            string NumberGroupSeparator = this.ThousSep, NegativeSign = "-", NegativeSignOrg = "";
            string PercentDecimalSeparator = this.DecSep, NumberDecimalSeparator = this.DecSep;

            strValor = (Math.Round(Double.Parse(strValor), intNumDecimales)).ToString();

            string strAux = null;
            string strComas = string.Empty;
            string strPuntos = null;

            if (strValor.Length == 0) return "";
            strValor = strValor.Replace(NumberGroupSeparator, NumberDecimalSeparator);
            if (strValor.Contains(NumberDecimalSeparator))
            {
                strAux = strValor.Substring(0, strValor.LastIndexOf(NumberDecimalSeparator));
                strComas = strValor.Substring(strValor.LastIndexOf(NumberDecimalSeparator) + 1);
            }
            else
            {
                strAux = strValor;
            }

            if (strAux.Substring(0, 1) == NegativeSign)
            {
                strAux = strAux.Substring(1);
                NegativeSignOrg = "-";
            }

            strPuntos = strAux;
            strAux = "";
            while (strPuntos.Length > 3)
            {
                strAux = NumberGroupSeparator + strPuntos.Substring(strPuntos.Length - 3, 3) + strAux;
                strPuntos = strPuntos.Substring(0, strPuntos.Length - 3);
            }
            if (intNumDecimales > 0)
            {
                if (strValor.Contains(PercentDecimalSeparator))
                {
                    strComas = PercentDecimalSeparator + strValor.Substring(strValor.LastIndexOf(PercentDecimalSeparator) + 1);
                    if (strComas.Length > intNumDecimales)
                    {
                        strComas = strComas.Substring(0, intNumDecimales + 1);
                    }

                }
            }
            strAux = NegativeSignOrg + strPuntos + strAux + strComas;

            return strAux;
        }
        #endregion

        #region Reset format numeric functions
        public string ResetFormatNumeric(string strValor)
        {
            return strValor.Replace(ThousSep, "");
        }

        public DataTable ResetFormatNumeric(DataTable dt, Array arr)
        {
            foreach (DataRow row in dt.Rows)
            {
                foreach(string ls in arr)
                {
                    row[ls] = ResetFormatNumeric(row[ls].ToString());
                }
            }
            return dt;
        }
        #endregion


        public DataTable RunQuery(String Query)
        {
            DataTable results = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ToString()))
            {
                SqlCommand command = new SqlCommand(Query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                conn.Open();
                adapter.Fill(results);
            }
            return results;
        }

        public static string UrlFullEncode(string strUrl)
        {
            if (strUrl == null)
                return "";
            strUrl = System.Web.HttpUtility.UrlEncode(strUrl);
            return strUrl.Replace("'", _strApostropheEncoding);
        }
        private const string _strApostropheEncoding = "%27";

        public int ObjectUpdate(string sp_Name, ArrayList pVarName, ArrayList pValList)
        {
            SqlConnection myConn = null;
            try
            {
                String conStr = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ToString();
                myConn = new SqlConnection(conStr);
                myConn.Open();
                SqlCommand myCommand = new SqlCommand(sp_Name, myConn);
                myCommand.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i <= pVarName.Count - 1; i++)
                {
                    myCommand.Parameters.AddWithValue(pVarName[i].ToString(), pValList[i]);
                }

                SqlParameter myParameter = new SqlParameter("@Result", SqlDbType.Int);
                myParameter.Direction = ParameterDirection.Output;
                myCommand.Parameters.Add(myParameter);
                myCommand.ExecuteNonQuery();
                myConn.Close();
                int re = 0;
                re = Convert.ToInt32(myCommand.Parameters[myCommand.Parameters.Count - 1].Value);
                return re;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if ((myConn != null))
                {
                    myConn.Close();
                }
            }

        }

        public DataTable ObjectSelect(string sp_Name, ArrayList pVarName, ArrayList pValList)
        {
            SqlConnection myConn = null;
            try
            {
                String conStr = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ToString();
                myConn = new SqlConnection(conStr);
                myConn.Open();
                SqlCommand myCommand = new SqlCommand(sp_Name, myConn);
                myCommand.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i <= pVarName.Count - 1; i++)
                {
                    myCommand.Parameters.AddWithValue(pVarName[i].ToString(), pValList[i]);
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = myCommand;
                DataSet ds = new DataSet();
                da.Fill(ds);
                //myCommand.ExecuteReader()
                myConn.Close();

                return ds.Tables[0];

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if ((myConn != null))
                {
                    myConn.Close();
                }
            }

        }
    }
}