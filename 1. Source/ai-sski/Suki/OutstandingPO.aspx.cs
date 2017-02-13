using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class OutstandingPO : System.Web.UI.Page
    {
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
            if (!IsPostBack)
            {
                LoadCompany();
                LoadOrderWareHouse(Session[Utils.AppConstants.CompanyCode].ToString());
                LoadData();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvSearchResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Search();
            Timer1.Enabled = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadData()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("DocNum");
            tbTemp.Columns.Add("DocEntry");
            tbTemp.Columns.Add("DocDate");
            tbTemp.Columns.Add("CardCode");
            tbTemp.Columns.Add("CardName");
            tbTemp.Columns.Add("DocDueDate");
            tbTemp.Columns.Add("DocStatus");
            tbTemp.Columns.Add("DocTotal");
            this.grvSearchResult.DataSource = tbTemp;
            this.grvSearchResult.DataBind();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvSearchResult_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.ToUpper() == "COPY")
                {
                    GridViewRow gvr = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                    int index = gvr.RowIndex;

                    Label lblStatus = (Label)this.grvSearchResult.Rows[index].FindControl("lblDocStatus");
                    LinkButton lnkPONo = (LinkButton)this.grvSearchResult.Rows[index].FindControl("lnkPONo");
                    if (lblStatus.Text.ToUpper() == "CLOSE" || lblStatus.Text.ToUpper() == "CANCELED")
                    {
                        ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert(This item does not allow to copy');", true);
                        return;
                    }
                    Label lblDocNum = (Label)this.grvSearchResult.Rows[index].FindControl("lblDocEntry");
                    Response.Redirect("ReceiveGoods.aspx?isNew=N&docEntry=" + lblDocNum.Text.Trim() + "&PoNo=" + lnkPONo.Text + "&Company=" + this.ddlCompany.SelectedValue.ToString()
                        + "&Outlet=" + this.drpOrderWareHouse.SelectedValue.ToString());
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Session["CompanyHistoryPOOut"] = this.ddlCompany.SelectedValue.ToString();
                Session["OutletPOOut"] = this.drpOrderWareHouse.SelectedValue.ToString();
                this.grvSearchResult.DataSource = null;
                this.grvSearchResult.DataBind();
                Search();
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkPONo_Click(object sender, EventArgs e)
        {
            try
            {
                GridViewRow gr = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                Label lblDocNum = (Label)gr.FindControl("lblDocEntry");
                Label lblDocStatus = (Label)gr.FindControl("lblDocStatus");
                LinkButton lnkPONo = (LinkButton)gr.FindControl("lnkPONo");
                Label lblConvertStatus = (Label)gr.FindControl("lblCStatus");
                Response.Redirect("CreatePO.aspx?IsPO=1&DocEntry=" + lblDocNum.Text + "&PONo=" + lnkPONo.Text + "&Status=" + lblDocStatus.Text
                    + "&CompanyCode=" + this.ddlCompany.SelectedValue.ToString()
                        + "&Outlet=" + this.drpOrderWareHouse.SelectedValue.ToString() + "&CStatus=" + lblConvertStatus.Text);
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
        private void Search()
        {
            try
            {
                //if (Session[Utils.AppConstants.CompanyCode].ToString() != System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString())
                {
                    MasterData obj = new MasterData();
                    DataSet dsUser = obj.GetOnlineUser();
                    //Doc Due Date
                    DateTime delFromDate = DateTime.Now;
                    DateTime delToDate = DateTime.Now;
                    if (this.txtDeliDateFrom.Text.Length == 0)
                    {
                        //delFromDate = DateTime.ParseExact("12/12/1900", Utils.AppConstants.DATE, null, System.Globalization.DateTimeStyles.None);
                        delFromDate = DateTime.ParseExact("12/12/1900", "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        if (!Utils.AppConstants.isDateTime(this.txtDeliDateFrom.Text))
                        {
                            this.lblError.Text = "DataTime is incorrect format.";
                            this.lblError.Visible = true;
                            return;
                        }
                        //delFromDate = DateTime.ParseExact(this.txtDeliDateFrom.Text, Utils.AppConstants.DATE, null, System.Globalization.DateTimeStyles.None);
                        delFromDate = DateTime.ParseExact(this.txtDeliDateFrom.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    if (this.txtDeliDateTo.Text.Length == 0)
                    {
                        delToDate = DateTime.MaxValue.Date;
                    }
                    else
                    {
                        if (!Utils.AppConstants.isDateTime(this.txtDeliDateTo.Text))
                        {
                            this.lblError.Text = "DataTime is incorrect format.";
                            this.lblError.Visible = true;
                            return;
                        }
                        //delToDate = DateTime.ParseExact(this.txtDeliDateTo.Text, Utils.AppConstants.DATE, null, System.Globalization.DateTimeStyles.None);
                        delToDate = DateTime.ParseExact(this.txtDeliDateTo.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }

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
                        //fromDate = DateTime.ParseExact(this.txtDateFrom.Text, Utils.AppConstants.DATE, null, System.Globalization.DateTimeStyles.None);
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
                        //toDate = DateTime.ParseExact(this.txtDateTo.Text, Utils.AppConstants.DATE, null, System.Globalization.DateTimeStyles.None);
                        toDate = DateTime.ParseExact(this.txtDateTo.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    string company = string.Empty;
                    company = Session[Utils.AppConstants.CompanyCode].ToString();
                    if (Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
                    {
                        company = ddlCompany.SelectedValue.ToString();
                    }
                    if (Session["CompanyHistoryPOOut"] != null)
                    {
                        company = Session["CompanyHistoryPOOut"].ToString();
                        this.ddlCompany.SelectedValue = company;
                        LoadOrderWareHouse(company);
                        this.drpOrderWareHouse.SelectedValue = Session["OutletPOOut"].ToString();
                    }
                    DataSet ds = obj.GetOutStandingPO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                   company, fromDate, toDate,
                                     delFromDate, delToDate,
                                     this.txtVendorCode.Text, this.txtPoNo.Text.Trim(), this.drpOrderWareHouse.SelectedValue.ToString(), Session[Utils.AppConstants.IsSupperUser].ToString());

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        int i = 1;
                        ds.Tables[0].Columns.Add("No");
                        DataSet dsLoad = ds.Clone();
                        Dictionary<string, string> dicOutlet = (Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet];
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {

                            if (dsUser != null && dsUser.Tables.Count > 0)
                            {
                                DataRow[] rName = dsUser.Tables[0].Select("Code='" + r["U_AB_UserCode"].ToString() + "'");
                                if (rName.Length > 0)
                                {
                                    r["UserName"] = rName[0]["Name"];
                                }
                            }
                            r["U_AB_SentSupplier"] = r["ConvertedToSO"].ToString() != "" ? "Sent to Supplier*" : r["U_AB_SentSupplier"];
                            r["No"] = i;
                            i++;
                            if (dicOutlet.Count > 0 && dicOutlet.ContainsKey(r["U_AB_POWhsCode"].ToString()))
                            {
                                dsLoad.Tables[0].ImportRow(r);
                            }
                            else
                            {
                                dsLoad.Tables[0].ImportRow(r);
                            }
                        }
                        if (Session["PageIndex"] != null)
                        {
                            this.grvSearchResult.PageIndex = int.Parse(Session["PageIndex"].ToString());
                        }
                        this.grvSearchResult.DataSource = dsLoad;
                        this.grvSearchResult.DataBind();
                    }
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
        /// <param name="company"></param>
        private void LoadOrderWareHouse(string company)
        {
            try
            {
                MasterData obj = new MasterData();
                this.drpOrderWareHouse.Items.Clear();
                DataSet ds = obj.GetABOutletByCompany(company);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (Session[Utils.AppConstants.ListOutlet] != null
                        && ((Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet]).Count > 0)
                    {
                        Dictionary<string, string> dicOutlet = (Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet];
                        foreach (KeyValuePair<string, string> item in dicOutlet)
                        {
                            DataRow[] rOutlet = ds.Tables[0].Select("U_WhseCode='" + item.Key + "'");
                            if (rOutlet.Length > 0)
                            {
                                this.drpOrderWareHouse.Items.Add(new ListItem(rOutlet[0]["U_WhseName"].ToString(), rOutlet[0]["U_WhseCode"].ToString()));
                            }
                        }
                        if (this.drpOrderWareHouse.Items.Count > 1)
                        {
                            this.drpOrderWareHouse.Items.Insert(0, new ListItem("All", "A"));
                            if (this.drpOrderWareHouse.Items.Count > 0)
                            {
                                this.drpOrderWareHouse.SelectedIndex = 0;
                            }
                        }
                    }
                    else
                    {
                        this.drpOrderWareHouse.DataSource = ds.Tables[0];
                        this.drpOrderWareHouse.DataTextField = "U_WhseName";
                        this.drpOrderWareHouse.DataValueField = "U_WhseCode";
                        this.drpOrderWareHouse.DataBind();
                        this.drpOrderWareHouse.Items.Insert(0, new ListItem("All", "A"));
                        if (this.drpOrderWareHouse.Items.Count > 0)
                        {
                            this.drpOrderWareHouse.SelectedIndex = 0;
                        }
                    }
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
        /// <param name="e"></param>
        protected override void OnLoadComplete(EventArgs e)
        {
            try
            {
                base.OnLoadComplete(e);
                if (this.Request["__EVENTARGUMENT"] != null && this.Request["__EVENTARGUMENT"].ToString() != "")
                {
                    switch (this.Request["__EVENTARGUMENT"].ToString())
                    {
                        case "SelectVendor":
                            this.txtVendorCode.Text = Session["ChosenVendorCode"] != null ? Session["ChosenVendorCode"].ToString() : "";
                            this.txtVendorName.Text = Session["ChosenVendorName"] != null ? Session["ChosenVendorName"].ToString() : "";
                            break;

                        default:
                            break;
                    }
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
        protected void txtVendorCode_TextChanged(object sender, EventArgs e)
        {
            if (this.txtVendorCode.Text.Length == 0)
            {
                this.txtVendorName.Text = "";
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
            this.grvSearchResult.PageIndex = e.NewPageIndex;
            Search();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSelectVendor_Click(object sender, EventArgs e)
        {
            try
            {
                string companyCode = Session[Utils.AppConstants.CompanyCode].ToString();
                string popup = "OpenVendor('" + companyCode + "','" + Session[Utils.AppConstants.OutletCode].ToString() + "')";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", popup, true);
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
                                LoadOrderWareHouse(this.ddlCompany.SelectedValue.ToString());
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
                            } if (this.ddlCompany.Items.Count > 0)
                            {
                                this.ddlCompany.SelectedValue = Session[Utils.AppConstants.CompanyCode].ToString();
                                LoadOrderWareHouse(this.ddlCompany.SelectedValue.ToString());
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
        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["CompanyHistoryPOOut"] = this.ddlCompany.SelectedValue.ToString();
            LoadOrderWareHouse(this.ddlCompany.SelectedValue.ToString());
        }

        protected void drpOrderWareHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["OutletPOOut"] = this.drpOrderWareHouse.SelectedValue.ToString();
        }
    }
}