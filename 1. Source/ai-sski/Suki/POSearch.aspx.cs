using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using Suki.Utils;
using System.IO;
using System.Text;
using CrystalDecisions.Shared;

namespace Suki
{
    public partial class POSearch : System.Web.UI.Page
    {
        /// <summary>
        /// 
        public string message = string.Empty;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.Private);
            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.btnExportExcel);
            scriptManager.RegisterPostBackControl(this.btnExportPDF);
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
                LoadData();
                if (Session[Utils.AppConstants.IsSupperUser] != null && Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y" && Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
                {
                    this.btnSendPO.Visible = true;
                    this.btnExportExcel.Visible = true;
                    this.btnExportPDF.Visible = true;
                    this.lblTotal.Text = "0.0000";
                }
                else
                {
                    this.ddlCompany.Enabled = false;
                }
                LoadCompany();
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
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            //Search();
            Timer1.Enabled = false;
        }
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkPONo_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session[Utils.AppConstants.CompanyCode] != null)
                {
                    string companyCode = Session[Utils.AppConstants.CompanyCode].ToString();
                    if (Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y" && Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
                    {
                        companyCode = this.ddlCompany.SelectedValue.ToString();
                    }
                    GridViewRow gr = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                    Label lblDocNum = (Label)gr.FindControl("lblDocEntry");
                    Label lblDocStatus = (Label)gr.FindControl("lblDocStatus");
                    LinkButton lnkPONo = (LinkButton)gr.FindControl("lnkPONo");
                    Label lblConvertStatus = (Label)gr.FindControl("lblStatus");
                    Response.Redirect("CreatePO.aspx?IsPO=1&DocEntry=" + lblDocNum.Text + "&PONo=" + lnkPONo.Text + "&CompanyCode=" + companyCode + "&Status=" + lblDocStatus.Text + "&CStatus=" + lblConvertStatus.Text);
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
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
        protected void lnkSO_Click(object sender, EventArgs e)
        {
            try
            {
                GridViewRow gr = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                Label lblDocEntry = (Label)gr.FindControl("lblDocEntry");
                HiddenField hdnCardCode = (HiddenField)gr.FindControl("hdnCardCode");
                string companyCode = Session[Utils.AppConstants.CompanyCode].ToString();
                if (Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y" && Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
                {
                    companyCode = this.ddlCompany.SelectedValue.ToString();
                }
                string popup = "OpenSOList('" + lblDocEntry.Text + "','" + hdnCardCode.Value + "','" + companyCode + "')";
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
                    if (lblStatus.Text.ToUpper() == "CLOSE" || lblStatus.Text.ToUpper() == "CANCELED")
                    {
                        ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert(This item does not allow to copy');", true);
                        return;
                    }
                    Label lblDocNum = (Label)this.grvSearchResult.Rows[index].FindControl("lblDocEntry");
                    Response.Redirect("ReceiveGoods.aspx?docEntry=" + lblDocNum.Text.Trim());
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Session["CompanyHistoryPOSearch"] = this.ddlCompany.SelectedValue.ToString();

                Session["OutletPOSearch"] = this.drpOrderWareHouse.SelectedValue.ToString();

                Session["ItemChecked"] = null;
                Search();
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
                    }
                    if (this.drpOrderWareHouse.Items.Count > 1)
                    {
                        this.drpOrderWareHouse.SelectedIndex = 0;
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
        private void Search()
        {
            try
            {

                this.grvSearchResult.DataSource = null;
                this.grvSearchResult.DataBind();
                this.lblError.Visible = false;
                this.lblMissingPoError.Visible = false;
                MasterData obj = new MasterData();
                DataSet dsUser = obj.GetOnlineUser();
                DataSet ds = new DataSet();

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
                    fromDate = DateTime.ParseExact(this.txtDateFrom.Text, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    //fromDate = DateTime.ParseExact(this.txtDateFrom.Text, Utils.AppConstants.DATE, null, System.Globalization.DateTimeStyles.None);
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

                string company = string.Empty;
                company = Session[Utils.AppConstants.CompanyCode].ToString();
                if (Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
                {
                    company = ddlCompany.SelectedValue.ToString();
                }
                if (Session["CompanyHistoryPOSearch"] != null)
                {
                    company = Session["CompanyHistoryPOSearch"].ToString();
                    this.ddlCompany.SelectedValue = company;
                    LoadOrderWareHouse(company);
                    this.drpOrderWareHouse.SelectedValue = Session["OutletPOSearch"].ToString();
                }
                decimal grandTotal = 0;
                string outlet = this.drpOrderWareHouse.SelectedValue.ToString();
                ds = obj.SearchPO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                    company, fromDate, toDate,
                                     delFromDate, delToDate, this.txtVendorCode.Text,
                                     this.ddlStatus.SelectedValue.ToString(), this.txtPONo.Text.Trim(), this.ddlChangeStatus.SelectedValue.ToString(),
                                     outlet, Session[Utils.AppConstants.IsSupperUser].ToString());
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataSet dsContact = obj.GetPeronalContact();
                    int i = 1;
                    ds.Tables[0].Columns.Add("No");
                    DataSet dsNew = ds.Clone();
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
                        if (dsContact != null && ds.Tables.Count > 0)
                        {
                            DataRow[] rEmail = dsContact.Tables[0].Select("CardCode='" + r["CardCode"].ToString() + "'");
                            if (rEmail.Length > 0)
                            {
                                r["E_Mail"] = rEmail[0]["E_Maill"];
                            }
                        }
                        r["U_AB_SentSupplier"] = r["ConvertedToSO"].ToString() != "" ? "Sent to Supplier*" : r["U_AB_SentSupplier"];
                        r["No"] = i;
                        i++;
                        if (Utils.AppConstants.isDouble(r["DocTotal"].ToString()))
                        {
                            grandTotal += decimal.Parse(r["DocTotal"].ToString());
                        }
                        if (dicOutlet.Count > 0)
                        {
                            if (dicOutlet.ContainsKey(r["U_AB_POWhsCode"].ToString()))
                            {
                                dsNew.Tables[0].ImportRow(r);
                            }
                        }
                        else
                        {
                            dsNew.Tables[0].ImportRow(r);
                        }

                    }
                    if (Session["PageIndex"] != null)
                    {
                        this.grvSearchResult.PageIndex = int.Parse(Session["PageIndex"].ToString());
                    }
                    if (dicOutlet.Count > 0)
                    {
                        Session["HasData"] = dsNew.Tables[0];
                        this.grvSearchResult.DataSource = dsNew;
                    }
                    else
                    {
                        Session["HasData"] = ds.Tables[0];
                        this.grvSearchResult.DataSource = ds;
                    }
                    this.grvSearchResult.DataBind();

                    this.lblTotal.Text = grandTotal.ToString(Utils.AppConstants.NUMBER_FORMAT);
                }
                else
                {
                    this.lblError.Text = "System is processing another request. Pls try again later.";
                    this.lblError.Visible = true;
                }
                if (Session["ItemChecked"] != null)
                {
                    for (int i = 0; i < this.grvSearchResult.Rows.Count; i++)
                    {
                        CheckBox checkBox = (CheckBox)this.grvSearchResult.Rows[i].FindControl("chkChild");
                        LinkButton lnkPONo = (LinkButton)this.grvSearchResult.Rows[i].FindControl("lnkPONo");
                        Dictionary<string, string> dicCheck = new Dictionary<string, string>();
                        dicCheck = (Dictionary<string, string>)Session["ItemChecked"];
                        if (dicCheck.ContainsKey(lnkPONo.Text))
                        {
                            checkBox.Checked = true;
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
        protected void btnSendPO_Click(object sender, EventArgs e)
        {
            //long POCount = 0;
            //long SOCount = 0;
            var ponumber = string.Empty;
            string id = string.Empty;
            message = string.Empty;
            MasterData obj = new MasterData();
            try
            {
                bool isUpdate = false;
                bool isProcess = false;
                bool bHasSOCreated = false;
                for (int i = 0; i < this.grvSearchResult.Rows.Count; i++)
                {
                    ponumber = string.Empty;
                    id = string.Empty;
                    CheckBox checkBox = (CheckBox)this.grvSearchResult.Rows[i].FindControl("chkChild");
                    HiddenField vendorCode = (HiddenField)this.grvSearchResult.Rows[i].FindControl("hdnCardCode");
                    HiddenField whsCode = (HiddenField)this.grvSearchResult.Rows[i].FindControl("hdnWhsCode");
                    LinkButton lnkPONo = (LinkButton)this.grvSearchResult.Rows[i].FindControl("lnkPONo");
                    HiddenField hdnComment = (HiddenField)this.grvSearchResult.Rows[i].FindControl("hdnComment");
                    Label lblUserID = (Label)this.grvSearchResult.Rows[i].FindControl("lblUserID");
                    Label lblPODate = (Label)this.grvSearchResult.Rows[i].FindControl("lblPODate");
                    Label lblDelDate = (Label)this.grvSearchResult.Rows[i].FindControl("lblDelDate");
                    HiddenField hdnUrgent = (HiddenField)this.grvSearchResult.Rows[i].FindControl("hdnUrgent");
                    Label lblStatus = (Label)this.grvSearchResult.Rows[i].FindControl("lblStatus");
                    Label lblDocEntry = (Label)this.grvSearchResult.Rows[i].FindControl("lblDocEntry");
                    Label lblDocStatus = (Label)this.grvSearchResult.Rows[i].FindControl("lblDocStatus");
                    ponumber = lnkPONo.Text;
                    bool createSO = false;
                    if (checkBox != null && checkBox.Checked && lblDocStatus.Text.ToUpper() == "OPEN")
                    {
                        string company = string.Empty;
                        company = Session[Utils.AppConstants.CompanyCode].ToString();
                        if (Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
                        {
                            company = ddlCompany.SelectedValue.ToString();
                        }
                      
                        //Get Internal PO
                        DataSet dsInternal = obj.GetInternalPO(vendorCode.Value);
                        if (dsInternal != null && dsInternal.Tables.Count > 0
                            && dsInternal.Tables[0].Rows.Count > 0
                            && lblStatus.Text.Trim() != "Sent to Supplier*"
                            && lblStatus.Text.Trim() != "Sent to Supplier")
                        {
                            //Check converted PO
                            bool dsCheck = obj.CheckProcessPO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                             company, lblDocEntry.Text, lnkPONo.Text, true);
                            if (dsCheck == false)
                            {
                                bHasSOCreated = true;
                                continue;
                            }
                            //Get sister database
                            string sisterDatabase = dsInternal.Tables[0].Rows[0]["U_DBName"].ToString();
                            string companyCode = Session[Utils.AppConstants.CompanyCode].ToString();
                            if (Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y"
                                && Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
                            {
                                companyCode = this.ddlCompany.SelectedValue.ToString();
                            }
                            //Check created SO
                            DataSet dsSO = obj.GetSOData(companyCode, lnkPONo.Text);
                            if (dsSO.Tables.Count > 0 && dsSO.Tables["AB_SO_Header"].Rows.Count == 0)
                            {
                                DataSet dsData = new DataSet();
                                DataTable tbHeader = new DataTable();
                                DataTable tbDetail = new DataTable();

                                //Header
                                tbHeader.TableName = "ORDR";
                                tbHeader.Columns.Add("LicTradNum");
                                tbHeader.Columns.Add("ShipToCode");
                                tbHeader.Columns.Add("U_AB_DriverNo");
                                tbHeader.Columns.Add("NumAtCard");
                                tbHeader.Columns.Add("U_AB_PORemarks");
                                tbHeader.Columns.Add("CardCode");
                                tbHeader.Columns.Add("DocDate");
                                tbHeader.Columns.Add("DocDueDate");
                                tbHeader.Columns.Add("Comments");
                                tbHeader.Columns.Add("U_AB_Urgent");
                                tbHeader.Columns.Add("U_AB_UserCode");
                                tbHeader.Columns.Add("U_AB_POWhsCode");
                                tbHeader.Columns.Add("SOStatus");
                                tbHeader.Columns.Add("ErrMessage");
                                tbHeader.Columns.Add("U_AB_PODocEntry");
                                tbHeader.Columns.Add("ID");
                                tbHeader.Columns.Add("CreateDate");
                                tbHeader.Columns.Add("U_AE_IFlag");

                                //Detail
                                tbDetail.TableName = "RDR1";
                                tbDetail.Columns.Add("U_AB_POLineNum");
                                tbDetail.Columns.Add("LineTotal");
                                tbDetail.Columns.Add("U_AB_POQty");
                                tbDetail.Columns.Add("ItemCode");
                                tbDetail.Columns.Add("Dscription");
                                tbDetail.Columns.Add("Quantity");
                                tbDetail.Columns.Add("Price");

                                //New Requirement
                                tbDetail.Columns.Add("U_AB_PODocEntry");

                                //Get CustomerCode
                                string customerCode = string.Empty;
                                DataSet dsBlock = new DataSet();

                                DataSet dsCustomer = obj.GetCustomerCode(companyCode);
                                if (dsCustomer != null & dsCustomer.Tables.Count > 0 & dsCustomer.Tables[0].Rows.Count > 0)
                                {
                                    customerCode = dsCustomer.Tables[0].Rows[0]["U_CustomerCode"].ToString();
                                    //Get Block 
                                    dsBlock = obj.GetBlock("", "", sisterDatabase, customerCode, whsCode.Value);
                                    if (dsBlock == null || dsBlock.Tables.Count == 0 || dsBlock.Tables[0].Rows.Count == 0)
                                    {
                                        this.lblError.Text = lnkPONo.Text + " - Block is blank. Pls try again.";
                                        this.lblError.Visible = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    this.lblError.Text = lnkPONo.Text + " - Can not get Customer. Pls Try again.";
                                    this.lblError.Visible = true;
                                    return;
                                }
                                //Set data Header
                                DataRow rHeader = tbHeader.NewRow();
                                rHeader["ShipToCode"] = whsCode.Value;
                                rHeader["Comments"] = hdnComment.Value;
                                rHeader["CardCode"] = customerCode;
                                rHeader["NumAtCard"] = lnkPONo.Text;
                                rHeader["LicTradNum"] = vendorCode.Value;
                                rHeader["DocDate"] = Utils.AppConstants.ConvertToDate(lblPODate.Text).ToString("yyyyMMdd");
                                rHeader["DocDueDate"] = Utils.AppConstants.ConvertToDate(lblDelDate.Text).ToString("yyyyMMdd");
                                rHeader["U_AB_PORemarks"] = hdnComment.Value;
                                rHeader["U_AB_Urgent"] = hdnUrgent.Value;
                                rHeader["U_AB_UserCode"] = lblUserID.Text;
                                rHeader["U_AB_POWhsCode"] = whsCode.Value;
                                rHeader["SOStatus"] = Utils.AppConstants.SOStatus.New;
                                rHeader["ErrMessage"] = "";
                                rHeader["U_AB_PODocEntry"] = lblDocEntry.Text;
                                rHeader["ID"] = "0";
                                rHeader["CreateDate"] = DateTime.Now.Date;
                                rHeader["U_AE_IFlag"] = "Y";
                                tbHeader.Rows.Add(rHeader);

                                //Set data Detail
                                DataSet dsDetail = obj.GetPODataUpdate("", "", companyCode, lblDocEntry.Text);
                                //POCount = dsDetail.Tables["POR1"].Rows.Count;
                                if (dsDetail != null && dsDetail.Tables.Count > 1 && dsDetail.Tables.Contains("POR1"))
                                {
                                    //Split SO
                                    DataSet dsShip = new DataSet();
                                    foreach (DataRow r in dsDetail.Tables["POR1"].Rows)
                                    {
                                        //Check DriverNo
                                        DataSet dsDriver = obj.GetDriverNo(dsBlock.Tables[0].Rows[0]["Block"].ToString(), r["TrnspName"].ToString());
                                        if (dsDriver != null && dsDriver.Tables.Count > 0 && dsDriver.Tables[0].Rows.Count == 0)
                                        {
                                            this.lblError.Text = lnkPONo.Text + " - " + r["ItemCode"].ToString() + " - " + r["TrnspName"].ToString() + " - Truck Category is blank in item master data.";
                                            this.lblError.Visible = true;
                                            return;
                                        }
                                        string driverNo = dsDriver.Tables[0].Rows[0]["U_DriverNo"].ToString();
                                        DataTable tbTemp = tbDetail.Clone();

                                        tbTemp.Columns.Add("ShipType");
                                        DataRow rDetail = tbTemp.NewRow();
                                        rDetail["U_AB_POLineNum"] = r["LineNum"].ToString();
                                        rDetail["ItemCode"] = r["ItemCode"].ToString();
                                        rDetail["LineTotal"] = r["LineTotal"].ToString();
                                        rDetail["U_AB_POQty"] = r["Quantity"].ToString();
                                        rDetail["Dscription"] = r["Dscription"].ToString();
                                        rDetail["Quantity"] = r["Quantity"].ToString();
                                        rDetail["Price"] = r["Price"].ToString();
                                        rDetail["ShipType"] = r["ShipType"].ToString();

                                        //New Requirement
                                        rDetail["U_AB_PODocEntry"] = lblDocEntry.Text;
                                        tbTemp.Rows.Add(rDetail);
                                        //Group By Driver No
                                        if (dsShip.Tables.Contains(driverNo))
                                        {
                                            dsShip.Tables[driverNo].ImportRow(rDetail);
                                        }
                                        else
                                        {
                                            DataTable tbSO = tbTemp.Copy();
                                            tbSO.TableName = driverNo;
                                            dsShip.Tables.Add(tbSO.Copy());
                                        }
                                    }
                                    if (dsShip.Tables.Count == 0)
                                    {
                                        this.lblError.Text = lnkPONo.Text + " - Can not Split SO. Pls Try again.";
                                        this.lblError.Visible = true;
                                        return;
                                    }
                                    //Process data for Updating
                                    foreach (DataTable tb in dsShip.Tables)
                                    {
                                        tb.Columns.Remove("ShipType");
                                        DataSet dsUpdateSO = new DataSet();
                                        DataTable tbDetailUpdate = tb.Copy();
                                        //SOCount = SOCount + tb.Rows.Count;
                                        if (dsBlock != null && dsBlock.Tables.Count > 0 && dsBlock.Tables[0].Rows.Count > 0)
                                        {
                                            //Header
                                            if (tbHeader != null && tbHeader.Rows.Count > 0)
                                            {
                                                tbHeader.Rows[0]["U_AB_DriverNo"] = tb.TableName;
                                            }
                                            //Detail
                                            tbDetailUpdate.TableName = "RDR1";
                                            dsUpdateSO.Tables.Add(tbDetailUpdate.Copy());
                                            dsUpdateSO.Tables.Add(tbHeader.Copy());

                                            //Update UDT data.
                                            string err = obj.UpdateSO("", "", dsUpdateSO, companyCode, true);
                                            if (err.Length > 0)
                                            {
                                                this.lblError.Text = err;
                                                this.lblError.Visible = true;
                                                createSO = false;
                                                return;
                                            }
                                            createSO = true;
                                        }
                                        else
                                        {
                                            this.lblError.Text = lnkPONo.Text + " - Block is blank. Pls try again.";
                                            this.lblError.Visible = true;
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    this.lblError.Text = lnkPONo.Text + " - Can not get PO Detail. Pls Try again.";
                                    this.lblError.Visible = true;
                                    return;
                                }
                                isProcess = true;
                            }
                            if (createSO)
                            {
                                isProcess = true;
                                bool isErr = false;

                                //Get UDT data
                                dsSO = obj.GetSOData(companyCode, lnkPONo.Text);
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
                                    string docEntry = string.Empty;
                                    string driverNo = string.Empty;
                                    foreach (DataRow rSO in dsSO.Tables["ORDR"].Rows)
                                    {
                                        if (rSO["SOStatus"].ToString() != Utils.AppConstants.SOStatus.Done.ToString()
                                            && rSO["SOStatus"].ToString() != Utils.AppConstants.SOStatus.Pass.ToString())
                                        {
                                            docEntry = rSO["U_AB_PODocEntry"].ToString();
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
                                                        //New Request 24/6/2014
                                                        r["CogsOcrCod"] = dsDefaultWS.Tables[0].Rows[0]["DfltWH"];
                                                        r["U_AB_Dept"] = dsDefaultWS.Tables[0].Rows[0]["U_AB_Dept"];
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
                                                //Create SO
                                                if (docEntry.Length > 0 && driverNo.Length>0)
                                                {
                                                    DataSet dsCheckSAPSO = obj.CheckCreatedSOSAP(lnkPONo.Text, driverNo, sisterDatabase);
                                                    if (dsCheckSAPSO != null && dsCheckSAPSO.Tables.Count > 0
                                                        && dsCheckSAPSO.Tables[0].Rows.Count == 0)
                                                    {
                                                        string xmlRequest = docx.ToXMLStringFromDS("17", dsUpdate);
                                                        DataSet dsErr = trans.CreateMarketingDocument(xmlRequest, "",
                                                            "", sisterDatabase, "17", "", false);
                                                        if (dsErr != null && dsErr.Tables.Count > 0 && dsErr.Tables[0].Rows.Count > 0)
                                                        {
                                                            if ((int)dsErr.Tables[0].Rows[0]["ErrCode"] != 0)
                                                            {
                                                                isErr = true;
                                                                rSO["SOStatus"] = Utils.AppConstants.SOStatus.Failed;
                                                                rSO["ErrMessage"] = dsErr.Tables[0].Rows[0]["ErrMsg"].ToString();
                                                                Utils.AppConstants.WriteLog(dsErr.Tables[0].Rows[0]["ErrMsg"].ToString(), true);
                                                                Utils.AppConstants.WriteLog(xmlRequest, true);
                                                                Utils.AppConstants.WriteLog(Session[Utils.AppConstants.UserCode].ToString(), true);
                                                                Utils.AppConstants.WriteLog(this.ddlCompany.SelectedValue.ToString(), true);
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                //if (POCount == SOCount)
                                                                //{
                                                                    rSO["ErrMessage"] = "Operation complete successful!";
                                                                    rSO["SOStatus"] = Utils.AppConstants.SOStatus.Pass;
                                                                //}
                                                                //else
                                                                //{
                                                                //    rSO["ErrMessage"] = "Operation failed!";
                                                                //    rSO["SOStatus"] = Utils.AppConstants.SOStatus.Failed;
                                                                //}
                                                            }
                                                        }
                                                        else
                                                        {
                                                            this.lblError.Text = "System is processing another request. Pls try again later.";
                                                            this.lblCompany.Visible = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //if (POCount == SOCount)
                                                        //{
                                                            rSO["ErrMessage"] = "Operation complete successful!";
                                                            rSO["SOStatus"] = Utils.AppConstants.SOStatus.Pass;
                                                        //}
                                                        //else
                                                        //{
                                                        //    rSO["ErrMessage"] = "Operation failed!";
                                                        //    rSO["SOStatus"] = Utils.AppConstants.SOStatus.Failed;
                                                        //}
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                isErr = true;
                                                rSO["SOStatus"] = Utils.AppConstants.SOStatus.Failed;
                                                rSO["ErrMessage"] = "Item Code - " + itemCode + " - Linked value " + strWhsCode + " does not exist.";
                                            }
                                        }
                                    }
                                    //Update SO UDT Log
                                    string err = obj.UpdateSO("", "", dsSO, companyCode, false);
                                    if (err.Length > 0)
                                    {
                                        this.lblError.Text = err;
                                        this.lblError.Visible = true;
                                        continue;
                                    }
                                    if (isErr == false)
                                    {
                                        lblStatus.Text = "Sent to Supplier*";
                                        lblStatus.ForeColor = Color.Blue;
                                    }
                                    else
                                    {
                                        if (isErr == true)
                                        {
                                            //lblStatus.Text = "Sent to Supplier*";
                                            lblStatus.Text = "Sent to HQ";
                                            lblStatus.ForeColor = Color.Red;
                                            lblStatus.Font.Bold = true;
                                            this.lblError.Text = "Have errors when created SO. Pls go to SO Status menu and try again.";
                                            this.lblError.Visible = true;
                                            bHasSOCreated = true;
                                            continue;
                                        }
                                    }
                                }
                            }

                            if (bHasSOCreated == true)
                            {
                                this.lblError.Text = "Operation complete successful, errors when creating SO. Pls go to SO Status menu and try again.";
                                this.lblError.Visible = true;
                            }
                            else
                            {
                                this.lblError.Text = "Operation complete successful!";
                                this.lblError.Visible = true;
                            }

                        }
                        else //External PO
                        {
                            //Check converted PO
                            if (checkBox.Checked
                                && lblStatus.Text.Trim() != "Sent to Supplier"
                                && lblStatus.Text.Trim() != "Sent to Supplier*")
                            {
                                
                                bool dsCheck = obj.CheckProcessPO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                          company, lblDocEntry.Text, lnkPONo.Text, false);
                                if (dsCheck == false)
                                {
                                    continue;
                                }
                                var emailAddress = (HiddenField)this.grvSearchResult.Rows[i].FindControl("hdnEmail");
                                var cardcode = (HiddenField)this.grvSearchResult.Rows[i].FindControl("hdnCardCode");
                                var cardName = this.grvSearchResult.Rows[i].FindControl("lblItemDesc");
                                AppConstants.DebugLog("PO no : " + ponumber, AppConstants.EmailLogPath);
                                AppConstants.DebugLog("Initiating the entry in email log table for the PO no " + ponumber + " Cardcode " + Convert.ToString(cardcode.Value) + " EmailId " + Convert.ToString(emailAddress.Value), AppConstants.EmailLogPath);
                                id = obj.InsertPODetailsforEmailCheck(ponumber, Convert.ToString(cardcode.Value), ((System.Web.UI.WebControls.Label)(cardName)).Text, "External", Convert.ToString(emailAddress.Value));
                                if (id.Substring(0, 1).ToString() == "I")
                                {
                                    AppConstants.DebugLog("After inserting purchase order in email log table:", AppConstants.EmailLogPath);
                                    AppConstants.DebugLog("Error on POno " + ponumber + " Cardcode " + Convert.ToString(cardcode.Value) + " EmailId " + Convert.ToString(emailAddress.Value) + " with error message " + id, AppConstants.EmailLogPath);
                                }
                                else
                                {
                                    AppConstants.DebugLog("After inserting purchase order in email log table:", AppConstants.EmailLogPath);
                                    AppConstants.DebugLog("Success on POno " + ponumber + " Cardcode " + Convert.ToString(cardcode.Value) + " EmailId " + Convert.ToString(emailAddress.Value) + " With Identity row no " + id, AppConstants.EmailLogPath);
                                    
                                }
                                //obj.SelectMaxIdofPO(poNo.Text);
                                isProcess = true;
                                HiddenField hdnCardCode = (HiddenField)this.grvSearchResult.Rows[i].FindControl("hdnCardCode");
                                HiddenField hdnWhsCode = (HiddenField)this.grvSearchResult.Rows[i].FindControl("hdnWhsCode");
                                //Update data
                                DataTable tblHeader = new DataTable("OPOR");
                                tblHeader.Columns.Add("OutletCode");
                                tblHeader.Columns.Add("U_AB_SentSupplier");
                                DataRow r = tblHeader.NewRow();
                                r["U_AB_SentSupplier"] = "Y";
                                tblHeader.Rows.Add(r);

                                DataTable tblItem = new DataTable();
                                tblItem.TableName = "POR1";
                                tblItem.Columns.Add("ShipType");
                                tblItem.Columns.Add("TrnspName");
                                DataSet dsData = new DataSet();
                                dsData.Tables.Add(tblHeader);
                                dsData.Tables.Add(tblItem);
                                string companyCode = Session[Utils.AppConstants.CompanyCode].ToString();
                                if (Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y"
                                    && Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
                                {
                                    companyCode = this.ddlCompany.SelectedValue.ToString();
                                }
                                //Create PDF file and Send Email
                                HiddenField email = (HiddenField)this.grvSearchResult.Rows[i].FindControl("hdnEmail");
                                bool result = true;
                                if (email.Value.Length > 0)
                                {
                                    AppConstants.DebugLog("Before Creating PDF file and Send Email for PO No: " + lnkPONo.Text + " with Id: " + id, AppConstants.EmailLogPath);
                                    result = ProcessExternalPO(obj, lblDelDate, lblDocEntry, email.Value, lnkPONo.Text, id);
                                }
                                if (!result)
                                {
                                    isUpdate = false;
                                    AppConstants.DebugLog("", AppConstants.EmailLogPath);
                                    //obj.UpdatePOStatusforEmailnotSent(lnkPONo.Text);
                                }
                                else //If send email successful
                                {
                                    lblStatus.Text = "Sent to Supplier";
                                    lblStatus.ForeColor = Color.Blue;
                                    isUpdate = true;
                                    //Update Sent Suppler field
                                    DataSet dsResult = obj.CreatePO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), companyCode, "1", "1", dsData, false, lblDocEntry.Text);
                                    if (dsData != null && dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                                    {
                                        if ((int)dsResult.Tables[0].Rows[0]["ErrCode"] != 0)
                                        {
                                            string err = dsResult.Tables[0].Rows[0]["ErrMsg"].ToString();
                                            if (err.Trim() == "The device is not ready.")
                                            {
                                                err = "System is processing another request. Pls try again later.";
                                            }
                                            //obj.UpdateStatusforEmailCheck(poNo.Text, "Failure", err, id);
                                            this.lblError.Text = err;
                                            isUpdate = false;
                                            this.lblError.Visible = true;
                                            AppConstants.DebugLog("", AppConstants.EmailLogPath);
                                        }
                                    }
                                    else
                                    {
                                        AppConstants.DebugLog("", AppConstants.EmailLogPath);
                                        this.lblError.Text = "System is processing another request. Pls try again later.";
                                        this.lblCompany.Visible = true;
                                    }
                                }
                            }
                            if (isUpdate)
                            {
                                this.lblError.Text = "Operation complete successful!";
                                AppConstants.DebugLog("", AppConstants.EmailLogPath);
                                this.lblError.Visible = true;
                            }
                            else {
                                if (message.ToString() != string.Empty)
                                {
                                    lblMissingPoError.Text = string.Empty;
                                    string err = message + "The above PO's are Unable to process due to some error";
                                    lblMissingPoError.Text = err.Replace(Environment.NewLine, "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
                                    lblMissingPoError.Visible = true;
                                }
                            }
                        }
                    }
                }
                if (!isProcess)
                {
                    if (message.ToString() != string.Empty)
                    {
                        string err = message + "The above PO's are Unable to process due to some error";
                        lblMissingPoError.Text = err.Replace(Environment.NewLine, "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
                        lblMissingPoError.Visible = true;
                    }
                    else
                    {
                        if (bHasSOCreated == true)
                        {
                            this.lblError.Text = "Operation complete successful, errors when creating SO. Pls go to SO Status menu and try again.";
                            this.lblError.Visible = true;
                        }
                        else
                        {
                            this.lblError.Text = "Operation complete successful!";
                            this.lblError.Visible = true;
                        }
                        //this.lblError.Text = "Operation complete successful!";
                        //this.lblError.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //obj.UpdateStatusforEmailCheck(ponumber, "Failure", ex.Message, id);
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="lblDelDate"></param>
        /// <param name="lblDocEntry"></param>
        /// <param name="email"></param>
        private bool ProcessExternalPO(MasterData obj, Label lblDelDate, Label lblDocEntry, string email, string poNo, string id)
        {
            //Create PDF file
            string directory = Server.MapPath("TEMP") + "/PDF/PO";
            string fileName = Server.MapPath("TEMP") + "/PDF/PO/" + poNo + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + DateTime.Now.Millisecond + ".pdf";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            try
            {
                Report.PurchaseOrder myreport = new Report.PurchaseOrder();
                DataSet ds = obj.ReportPurchaseOrder(lblDocEntry.Text, this.ddlCompany.SelectedValue.ToString());
                if (ds != null && ds.Tables.Count > 0)
                {
                    myreport.SetDataSource(ds.Tables[0]);
                }
                ExportOptions CrExportOptions;
                DiskFileDestinationOptions CrDiskFileDestinationOptions = new DiskFileDestinationOptions();
                PdfRtfWordFormatOptions CrFormatTypeOptions = new PdfRtfWordFormatOptions();
                CrDiskFileDestinationOptions.DiskFileName = fileName;
                CrExportOptions = myreport.ExportOptions;
                {
                    CrExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    CrExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    CrExportOptions.DestinationOptions = CrDiskFileDestinationOptions;
                    CrExportOptions.FormatOptions = CrFormatTypeOptions;
                }
                myreport.Export();
                myreport.Dispose();
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
                return false;
            }
            AppConstants.DebugLog("PDF Created successfully for PO no: " + poNo + " with Id: " + id, AppConstants.EmailLogPath);
            //Send Email
            if (File.Exists(fileName))
            {
                Attachment att = new Attachment(fileName);
                att.Name = poNo + ".pdf";
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Images/Email/EmailTemplate.htm")))
                {
                    body = reader.ReadToEnd();
                    body = body.Replace("{DeliDate}", lblDelDate.Text);
                }
                string subject = "Suki Group of Restaurants – Vendor’s PO [" + this.ddlCompany.SelectedValue.ToString() + ":" + poNo + "]";
                Utils.AppConstants.WriteLog(Session[Utils.AppConstants.UserCode].ToString(), true);
                Utils.AppConstants.WriteLog(this.ddlCompany.SelectedValue.ToString(), true);
                AppConstants.WriteLog(email);
                AppConstants.DebugLog("Ready to send Email for PO no: " + poNo + " in the Email Id: " + email, AppConstants.EmailLogPath);
                string errMsg = Utils.Emails.SendEmail(System.Configuration.ConfigurationManager.AppSettings["FromEmail"].ToString(), new string[] { email }, subject, body, new Attachment[] { att }, null, poNo , id);
                if (errMsg.Length > 0)
                {
                    AppConstants.WriteLog(errMsg);
                    obj.UpdateStatusforEmailCheck(poNo, "Failure", errMsg, id);
                    AppConstants.DebugLog("Email sending Failure for the PO no: " + poNo + " in the Email Id: " + email + " with the error message as " + errMsg, AppConstants.EmailLogPath);
                    this.lblError.Text = errMsg;
                    this.lblError.Visible = true;
                    message = message + poNo + "\r\n";
                    return false;
                }
                else
                {
                    AppConstants.DebugLog("Email Sent successfully for the PO no: " + poNo + " in the Email Id: " + email, AppConstants.EmailLogPath);
                    AppConstants.WriteLog("Email Sent successfuly");
                }
            }
            else
            {
                this.lblError.Text = "Can not create attached file and send email.";
                this.lblError.Visible = true;
                return false;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvSearchResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            Dictionary<string, string> dicCheck;
            if (Session["ItemChecked"] == null)
            {
                dicCheck = new Dictionary<string, string>();
            }
            else
            {
                dicCheck = (Dictionary<string, string>)Session["ItemChecked"];
            }
            for (int i = 0; i < this.grvSearchResult.Rows.Count; i++)
            {
                CheckBox checkBox = (CheckBox)this.grvSearchResult.Rows[i].FindControl("chkChild");
                LinkButton lnkPONo = (LinkButton)this.grvSearchResult.Rows[i].FindControl("lnkPONo");
                if (checkBox.Checked)
                {
                    if (!dicCheck.ContainsKey(lnkPONo.Text))
                    {
                        dicCheck.Add(lnkPONo.Text, lnkPONo.Text);
                    }
                }
                else
                {
                    if (dicCheck.ContainsKey(lnkPONo.Text))
                    {
                        dicCheck.Remove(lnkPONo.Text);
                    }
                }
            }
            Session["PageIndex"] = e.NewPageIndex;
            Session["ItemChecked"] = dicCheck;
            this.grvSearchResult.PageIndex = e.NewPageIndex;
            Search();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DataTable CreatedTableExport()
        {
            DataTable tblExport = new DataTable();
            try
            {
                tblExport.Columns.Add("#");
                tblExport.Columns.Add("Issued Date");
                tblExport.Columns.Add("Delivery Date");
                tblExport.Columns.Add("PO No.");
                tblExport.Columns.Add("Supplier Name");
                tblExport.Columns.Add("User ID & Name");
                tblExport.Columns.Add("Total");
                tblExport.Columns.Add("PO.Status");
                tblExport.Columns.Add("Status");
                DataTable tblReportData = (DataTable)Session["HasData"];
                if (tblReportData != null)
                {
                    foreach (DataRow r in tblReportData.Rows)
                    {
                        DataRow row = tblExport.NewRow();
                        row[0] = r["No"];
                        row[1] = DateTime.Parse(r["DocDate"].ToString()).ToString(Utils.AppConstants.DATE);
                        row[2] = DateTime.Parse(r["DocDueDate"].ToString()).ToString(Utils.AppConstants.DATE);
                        row[3] = r["DocNum"];
                        row[4] = r["CardName"];
                        row[5] = r["U_AB_UserCode"] + "-" + r["UserName"];
                        row[6] = decimal.Parse(r["DocTotal"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                        row[7] = r["DocStatus"];
                        row[8] = r["U_AB_SentSupplier"];
                        tblExport.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
            return tblExport;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            using (var exporter = new NpoiExport())
            {
                if (Session["HasData"] != null && ((DataTable)Session["HasData"]).Rows.Count > 0)
                {
                    try
                    {
                        string excelFileName = System.Configuration.ConfigurationManager.AppSettings["ExcelFileName"].ToString();
                        DataTable tblExport = CreatedTableExport();
                        exporter.ExportDataTableToWorkbook(tblExport, excelFileName);
                        string saveAsFileName = string.Format(excelFileName + "_{0:d}.xls", DateTime.Now.Ticks);
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", saveAsFileName));
                        Response.Clear();
                        Response.BinaryWrite(exporter.GetBytes());
                        Response.End();
                    }
                    catch (Exception ex)
                    {
                        this.lblError.Text = ex.Message;
                        this.lblError.Visible = true;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            string directory = Server.MapPath("TEMP") + "/PDF/";
            try
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
                if (Session["HasData"] != null && ((DataTable)Session["HasData"]).Rows.Count > 0)
                {

                    string fileName = string.Format("{0}_{1}", System.Configuration.ConfigurationManager.AppSettings["PDFFileName"].ToString(), DateTime.Now.Ticks);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    string pdfFileName = string.Format("{0}{1}.pdf", directory, fileName);
                    DataTable tblExport = CreatedTableExport();
                    float[] relativeWidths = { 20, 50, 50, 50, 200, 100, 50, 50, 70 };
                    PdfExport.DataSourceToPdf(tblExport, pdfFileName, relativeWidths);
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", string.Format("{0}.{1}", fileName, "pdf")));
                    Response.ContentType = "application/pdf";
                    Response.Clear();
                    Response.WriteFile(pdfFileName);
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }
        protected void grvSearchResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";

                Label lblStatus = e.Row.FindControl("lblStatus") as Label;
                if (Convert.ToString(DataBinder.Eval(e.Row.DataItem, "SOStatus")) == AppConstants.SOStatus_OnSearch)
                {
                    lblStatus.Font.Bold = true;
                    lblStatus.ForeColor = Color.Red;
                }
            }
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
                if (Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y" && Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
                {
                    companyCode = this.ddlCompany.SelectedValue.ToString();
                }
                string popup = "OpenVendor('" + companyCode + "','" + string.Empty + "')";
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["CompanyHistoryPOSearch"] = this.ddlCompany.SelectedValue.ToString();

            this.grvSearchResult.DataSource = null;
            this.grvSearchResult.DataBind();
            this.lblTotal.Text = "0.0000";
            LoadOrderWareHouse(this.ddlCompany.SelectedValue.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void drpOrderWareHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["OutletPOSearch"] = this.drpOrderWareHouse.SelectedValue.ToString();
        }
    }
}