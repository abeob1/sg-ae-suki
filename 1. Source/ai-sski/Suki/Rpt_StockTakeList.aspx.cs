using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace Suki
{
    public partial class Rpt_StockTakeList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MasterData obj = new MasterData();
            Report.StockTakeList myreport = new Report.StockTakeList();
            DataSet ds = obj.ReportStockTakeList("NM-CL", "UAT_RNihonMura");
            if (ds != null && ds.Tables.Count > 0)
            {
                myreport.SetDataSource(ds.Tables[0]);
                CrystalReportViewer1.ReportSource = myreport;
            }
        }
    }
}