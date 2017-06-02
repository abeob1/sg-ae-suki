using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class SOStatusList : System.Web.UI.Page
    {
        private int _rowCount = 0;
        private int _checkRow = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[Utils.AppConstants.UserCode] == null
          || Session[Utils.AppConstants.CompanyName] == null
          || Session[Utils.AppConstants.CompanyCode] == null
           || Session[Utils.AppConstants.OutletCode] == null
             || Session[Utils.AppConstants.OutletName] == null
            || Session[Utils.AppConstants.IsSupperUser] == null
           || Session[Utils.AppConstants.Pwd] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (Session[Utils.AppConstants.IsSupperUser] != null && Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y" && Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
            {
                this.btnAccept.Visible = true;

            }
            else
            {
                this.btnAccept.Visible = false;
                this.grdItem.Enabled = false;
            }
            if (!IsPostBack)
            {
                LoadCompany();
                this.grdItem.DataSource = null;
                this.grdItem.DataBind();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadCompany()
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet ds = obj.GetAllABCompany();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Session[Utils.AppConstants.ListCompany] != null)
                    {
                        Dictionary<string, string> ownCompany = (Dictionary<string, string>)Session[Utils.AppConstants.ListCompany];
                        if (ownCompany.Count == 0)
                        {
                            this.ddlCompany.DataSource = ds;
                            this.ddlCompany.DataTextField = "U_CompanyName";
                            this.ddlCompany.DataValueField = "U_DBName";
                            this.ddlCompany.DataBind();
                            if (this.ddlCompany.Items.Count > 0)
                            {
                                this.ddlCompany.SelectedValue = Session[Utils.AppConstants.CompanyCode].ToString();
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<string, string> item in ownCompany)
                            {
                                DataRow[] r = ds.Tables[0].Select("U_DBName='" + item.Value + "'");
                                if (r.Length > 0)
                                {
                                    this.ddlCompany.Items.Add(new ListItem(r[0]["U_CompanyName"].ToString(), r[0]["U_DBName"].ToString()));
                                }
                                if (this.ddlCompany.Items.Count > 0)
                                {
                                    this.ddlCompany.SelectedValue = Session[Utils.AppConstants.CompanyCode].ToString();
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.lblError.Text = "Can not get Company list. System is processing another request. Pls try again later.";
                    this.lblError.Visible = true;
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (this.ddlCompany.Items.Count > 0)
            {
                LoadItem(this.ddlCompany.SelectedValue.ToString());
            }
            Timer1.Enabled = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadItem(string company)
        {
            try
            {
                this.lblError.Visible = false;
                MasterData obj = new MasterData();
                DataSet tblItem = null;
                //Doc Date
                DateTime fromDate = DateTime.Now;
                DateTime toDate = DateTime.Now;
                if (this.txtDateFrom.Text.Length == 0)
                {
                    //fromDate = DateTime.ParseExact("12/12/1900", Utils.AppConstants.DATE, null, System.Globalization.DateTimeStyles.None);
                    fromDate = DateTime.ParseExact("12/12/1900", "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    if (!Utils.AppConstants.isDateTime(this.txtDateFrom.Text))
                    {
                        this.lblError.Text = "DataTime is incorrect format.";
                        this.lblError.Visible = true;
                        return;
                    }
                    //fromDate = DateTime.ParseExact(this.txtDateFrom.Text, Utils.AppConstants.DATE, null, System.Globalization.DateTimeStyles.None).AddSeconds(-1);
                    fromDate = DateTime.ParseExact(this.txtDateFrom.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                if (this.txtDateTo.Text.Length == 0)
                {
                    toDate = DateTime.MaxValue.Date;
                }
                else
                {
                    if (!Utils.AppConstants.isDateTime(this.txtDateTo.Text))
                    {
                        this.lblError.Text = "DataTime is incorrect format.";
                        this.lblError.Visible = true;
                        return;
                    }
                    //toDate = DateTime.ParseExact(this.txtDateTo.Text, Utils.AppConstants.DATE, null, System.Globalization.DateTimeStyles.None).AddSeconds(86399);
                    toDate = DateTime.ParseExact(this.txtDateTo.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                tblItem = obj.SearchSOData(company,this.ddlStatus.SelectedValue.ToString(),this.txtPONo.Text.Trim(),fromDate,toDate);
                if (Session["PageIndex"] != null)
                {
                    this.grdItem.PageIndex = int.Parse(Session["PageIndex"].ToString());
                }
                if (tblItem != null && tblItem.Tables.Count > 0)
                {
                    foreach (DataRow r in tblItem.Tables[0].Rows)
                    {
                        string docDate = DateTime.Parse(r["CreateDate"].ToString()).ToString(Utils.AppConstants.DATE);
                        r["DocDate"] = docDate;
                        string docDueDate = r["DocDueDate"].ToString().Substring(6, 2) + "/" + r["DocDueDate"].ToString().Substring(4, 2) + "/" + r["DocDueDate"].ToString().Substring(0, 4);
                        r["DocDueDate"] = docDueDate;
                    }
                    _rowCount = tblItem.Tables[0].Rows.Count;
                    this.grdItem.DataSource = tblItem;
                }
                else
                {
                    this.grdItem.DataSource = null;
                }
                this.grdItem.DataBind();
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvClosingStock_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow myRow = e.Row;
                Label lblNo = myRow.FindControl("lblNo") as Label;
                lblNo.Text = (myRow.RowIndex + 1).ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvSearchResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Session["PageIndex"] = e.NewPageIndex;
            this.grdItem.PageIndex = e.NewPageIndex;
            LoadItem(this.ddlCompany.SelectedValue.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                MasterData obj = new MasterData();

                for (int i = 0; i < this.grdItem.Rows.Count; i++)
                {
                    CheckBox checkBox = (CheckBox)this.grdItem.Rows[i].FindControl("chkChild");
                    Label lblStatus = (Label)this.grdItem.Rows[i].FindControl("lblStatus");
                    HiddenField hdnID = (HiddenField)this.grdItem.Rows[i].FindControl("hdnID");
                    HiddenField hdnVendorCode = (HiddenField)this.grdItem.Rows[i].FindControl("hdnVendorCode");
                    DataSet dsInternal = obj.GetInternalPO(hdnVendorCode.Value);
                    if (dsInternal != null && dsInternal.Tables.Count > 0 && dsInternal.Tables[0].Rows.Count > 0)
                    {
                        //Get sister database
                        string sisterDatabase = dsInternal.Tables[0].Rows[0]["U_DBName"].ToString();

                        if (checkBox != null)
                        {
                            if (checkBox.Checked
                                && lblStatus.Text != Utils.AppConstants.SOStatus.Done.ToString()
                                 && lblStatus.Text != Utils.AppConstants.SOStatus.Pass.ToString())
                            {
                                string companyCode = Session[Utils.AppConstants.CompanyCode].ToString();
                                if (Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y")
                                {
                                    companyCode = this.ddlCompany.SelectedValue.ToString();
                                }
                                string docNum = string.Empty;
                                string driverNo = string.Empty;
                                DataSet dsSO = obj.GetSODataById(companyCode, hdnID.Value);
                                if (dsSO != null && dsSO.Tables.Count > 1)
                                {

                                    dsSO.Tables["AB_SO_Header"].TableName = "ORDR";
                                    dsSO.Tables["AB_SO_Detail"].TableName = "RDR1";

                                    //New
                                    dsSO.Tables["RDR1"].Columns.Add("OcrCode");
                                    dsSO.Tables["RDR1"].Columns.Add("U_AB_Dept");
                                    //New Request 24/6/2014
                                    dsSO.Tables["RDR1"].Columns.Add("CogsOcrCod");

                                    //New requirement
                                    dsSO.Tables["RDR1"].Columns.Add("U_AB_PODocEntry");

                                    Transaction trans = new Transaction();
                                    DocumentXML docx = new DocumentXML();
                                    foreach (DataRow rSO in dsSO.Tables["ORDR"].Rows)
                                    {
                                        if (rSO["SOStatus"].ToString() != Utils.AppConstants.SOStatus.Done.ToString()
                                            && rSO["SOStatus"].ToString() != Utils.AppConstants.SOStatus.Pass.ToString())
                                        {
                                            docNum = rSO["NumAtCard"].ToString();
                                            driverNo = rSO["U_AB_DriverNo"].ToString();
                                            DataSet dsUpdate = new DataSet();

                                            DataTable tbH = dsSO.Tables["ORDR"].Clone();
                                            tbH.ImportRow(rSO);

                                            DataTable tbD = dsSO.Tables["RDR1"].Clone();
                                            foreach (DataRow r in dsSO.Tables["RDR1"].Rows)
                                            {
                                                if (r["HeaderID"].ToString() == rSO["ID"].ToString())
                                                {
                                                    //New
                                                    DataSet dsDefaultWS = obj.GetDefaultWareHouse(r["ItemCode"].ToString(), sisterDatabase);
                                                    if (dsDefaultWS != null && dsDefaultWS.Tables.Count > 0 && dsDefaultWS.Tables[0].Rows.Count > 0)
                                                    {
                                                        r["OcrCode"] = dsDefaultWS.Tables[0].Rows[0]["DfltWH"];
                                                        r["U_AB_Dept"] = dsDefaultWS.Tables[0].Rows[0]["U_AB_Dept"];
                                                        //New Request 24/6/2014
                                                        r["CogsOcrCod"] = dsDefaultWS.Tables[0].Rows[0]["DfltWH"];
                                                    }
                                                    //New requirement
                                                    r["U_AB_PODocEntry"] = rSO["U_AB_PODocEntry"];

                                                    tbD.ImportRow(r);
                                                }

                                            }

                                            tbH.Columns.Remove("SOStatus");
                                            tbH.Columns.Remove("ErrMessage");
                                            tbH.Columns.Remove("ID");
                                            tbH.Columns.Remove("CreateDate");
                                            tbD.Columns.Remove("ID");
                                            tbD.Columns.Remove("HeaderID");

                                            dsUpdate.Tables.Add(tbH);
                                            dsUpdate.Tables.Add(tbD);

                                            //Check WhsCode
                                            bool whsCodeErr = false;
                                            string itemCode = string.Empty;
                                            string strWhsCode = string.Empty;
                                            foreach (DataRow row in tbD.Rows)
                                            {
                                                bool isExist = obj.CheckWhsCode(sisterDatabase, row["OcrCode"].ToString());
                                                if (!isExist)
                                                {
                                                    whsCodeErr = true;
                                                    itemCode = row["ItemCode"].ToString();
                                                    strWhsCode = row["OcrCode"].ToString();
                                                    break;
                                                }
                                            }

                                            if (whsCodeErr == false)
                                            {
                                                DataSet dsCheckSAPSO = obj.CheckCreatedSOSAP(docNum, driverNo, sisterDatabase);
                                                if (dsCheckSAPSO != null && dsCheckSAPSO.Tables.Count > 0
                                                    && dsCheckSAPSO.Tables[0].Rows.Count == 0)
                                                {
                                                    string xmlRequest = docx.ToXMLStringFromDS("17", dsUpdate);
                                                    DataSet dsErr = trans.CreateMarketingDocument(xmlRequest, "",
                                                        "", sisterDatabase, "17", "", false);
                                                    if (dsErr != null && dsErr.Tables.Count > 0)
                                                    {
                                                        if ((int)dsErr.Tables[0].Rows[0]["ErrCode"] != 0)
                                                        {
                                                            rSO["SOStatus"] = Utils.AppConstants.SOStatus.Failed;
                                                            rSO["ErrMessage"] = dsErr.Tables[0].Rows[0]["ErrMsg"].ToString();
                                                            Utils.AppConstants.WriteLog(dsErr.Tables[0].Rows[0]["ErrMsg"].ToString(), true);
                                                            Utils.AppConstants.WriteLog(xmlRequest, true);
                                                            Utils.AppConstants.WriteLog(Session[Utils.AppConstants.UserCode].ToString(), true);
                                                            Utils.AppConstants.WriteLog(this.ddlCompany.SelectedValue.ToString(), true);
                                                        }
                                                        else
                                                        {
                                                            rSO["ErrMessage"] = "Operation complete successful!";
                                                            rSO["SOStatus"] = Utils.AppConstants.SOStatus.Pass;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    rSO["ErrMessage"] = "Operation complete successful!";
                                                    rSO["SOStatus"] = Utils.AppConstants.SOStatus.Pass;
                                                }
                                            }
                                            else
                                            {
                                                rSO["SOStatus"] = Utils.AppConstants.SOStatus.Failed;
                                                rSO["ErrMessage"] = "Item Code - " + itemCode + " - Linked value " + strWhsCode + " does not exist.";
                                            }
                                        }
                                    }
                                    string err = obj.UpdateSO("", "", dsSO, companyCode, false);
                                    if (err.Length > 0)
                                    {
                                        this.lblError.Text = err;
                                        this.lblError.Visible = true;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                LoadItem(this.ddlCompany.SelectedValue.ToString());
                this.lblError.Text = "Operation complete successful!";
                this.lblError.Visible = true;
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                try
                {
                    CheckBox checkBox = (CheckBox)e.Row.FindControl("chkChild");
                    if (checkBox != null)
                    {
                        if (lblStatus.Text == Utils.AppConstants.SOStatus.Done.ToString()
                            || lblStatus.Text == Utils.AppConstants.SOStatus.Pass.ToString())
                        {
                            checkBox.Checked = true;
                            checkBox.Enabled = false;
                            _checkRow++;
                        }
                        if (_checkRow == _rowCount)
                        {
                            //Check All
                            GridViewRow headerRow = this.grdItem.HeaderRow;
                            ((CheckBox)headerRow.FindControl("chkheader")).Checked = true;
                            ((CheckBox)headerRow.FindControl("chkheader")).Enabled = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.lblError.Text = ex.Message;
                    this.lblError.Visible = true;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItem(this.ddlCompany.SelectedValue.ToString());
        }
    }
}
