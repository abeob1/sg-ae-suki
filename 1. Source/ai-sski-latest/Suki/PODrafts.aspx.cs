using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;

namespace Suki
{
    public partial class PODrafts : System.Web.UI.Page
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
                LoadOrderWareHouse(Session[Utils.AppConstants.CompanyCode].ToString());
                LoadData();
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Search();
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
                GridViewRow gr = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                Label lblDocNum = (Label)gr.FindControl("lblDocEntry");
                LinkButton lnkPONo = (LinkButton)gr.FindControl("lnkPONo");
                Label lblDocStatus = (Label)gr.FindControl("lblDocStatus");
                Response.Redirect("CreatePO.aspx?IsPO=0&DocEntry=" + lblDocNum.Text + "&PONo=" + lnkPONo.Text + "&Status=" + lblDocStatus.Text);
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
                    if (lblStatus.Text.ToUpper() == "CLOSE")
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
        private void LoadData(  )
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
                this.lblError.Visible = false;
                Session["SentToHQ"] = null;
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
                    //DataSet ds = obj.DraftPO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                    //                     Session[Utils.AppConstants.CompanyCode].ToString(), fromDate, toDate,
                    //                     delFromDate, delToDate,
                    //                      this.txtVendorCode.Text, this.ddlStatus.SelectedValue.ToString(),Session[Utils.AppConstants.OutletCode].ToString(),Session[Utils.AppConstants.IsSupperUser].ToString());
                    DataSet ds = obj.DraftPO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                      Session[Utils.AppConstants.CompanyCode].ToString(), fromDate, toDate,
                                      delFromDate, delToDate,
                                       this.txtVendorCode.Text, this.ddlStatus.SelectedValue.ToString(), this.drpOrderWareHouse.SelectedValue.ToString(), Session[Utils.AppConstants.IsSupperUser].ToString());
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        int i = 1;
                        ds.Tables[0].Columns.Add("No");
                        DataSet dsLoad = ds.Clone();
                        Dictionary<string, string> dicOutlet = (Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet];
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {

                            r["No"] = i;
                            i++;
                            if (dsUser != null && dsUser.Tables.Count > 0)
                            {
                                DataRow[] rName = dsUser.Tables[0].Select("Code='" + r["U_AB_UserCode"].ToString() + "'");
                                if (rName.Length > 0)
                                {
                                    r["UserName"] = rName[0]["Name"];
                                }
                            }
                            if (dicOutlet.Count > 0)
                            {
                                if (dicOutlet.ContainsKey(r["U_AB_POWhsCode"].ToString()))
                                {
                                    dsLoad.Tables[0].ImportRow(r);
                                }
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
                if (Session["ItemChecked"] != null)
                {
                    for (int i = 0; i < this.grvSearchResult.Rows.Count; i++)
                    {
                        CheckBox checkBox = (CheckBox)this.grvSearchResult.Rows[i].FindControl("chkChild");
                        LinkButton lnkPONo = (LinkButton)this.grvSearchResult.Rows[i].FindControl("lblDocNum");
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
                LinkButton lnkPONo = (LinkButton)this.grvSearchResult.Rows[i].FindControl("lblDocNum");
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
            grvSearchResult.PageIndex = e.NewPageIndex;
            Search();
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
        protected void btnSendToHQ_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblError.Visible = false;
                MasterData obj = new MasterData();
                for (int i = 0; i < this.grvSearchResult.Rows.Count; i++)
                {
                    Label lblDocEntry = (Label)this.grvSearchResult.Rows[i].FindControl("lblDocEntry");
                    Label lblPODate = (Label)this.grvSearchResult.Rows[i].FindControl("lblPODate");
                    Label lblDelDate = (Label)this.grvSearchResult.Rows[i].FindControl("lblDelDate");
                    Label lblNo = (Label)this.grvSearchResult.Rows[i].FindControl("lblNo");
                    Label lblDocNum = (Label)this.grvSearchResult.Rows[i].FindControl("lblDocNum");
                    CheckBox checkBox = (CheckBox)this.grvSearchResult.Rows[i].FindControl("chkChild");
                    Dictionary<string, string> dicSent = null;
                    if (checkBox != null)
                    {
                        if (checkBox.Checked)
                        {
                            if (Session["SentToHQ"] != null)
                            {
                                dicSent = (Dictionary<string, string>)Session["SentToHQ"];
                                if (dicSent.ContainsKey(lblDocEntry.Text))
                                {
                                    continue;
                                }
                            }
                            DataSet ds = obj.GetPODraftUpdate(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                                        Session[Utils.AppConstants.CompanyCode].ToString(), lblDocEntry.Text);
                            DataSet dsUpdate = new DataSet();
                            if (ds != null && ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataTable tblHeader = new DataTable("OPOR");
                                    tblHeader.Columns.Add("CardCode");
                                    tblHeader.Columns.Add("CardName");
                                    tblHeader.Columns.Add("DocDate");
                                    tblHeader.Columns.Add("DocDueDate");
                                    tblHeader.Columns.Add("Comments");
                                    tblHeader.Columns.Add("OutletCode");
                                    tblHeader.Columns.Add("U_AB_Urgent");
                                    tblHeader.Columns.Add("U_AB_UserCode");
                                    tblHeader.Columns.Add("U_AB_POWhsCode");
                                    tblHeader.Columns.Add("U_AB_SentSupplier");
                                    tblHeader.Columns.Add("U_AB_PORemarks");
                                    tblHeader.Columns.Add("U_AB_Time");

                                    DateTime delData = new DateTime();
                                    string whsCode = string.Empty;
                                    foreach (DataRow rDraft in ds.Tables[0].Rows)
                                    {
                                        DataRow row = tblHeader.NewRow();
                                        row["CardCode"] = rDraft["CardCode"];
                                        row["CardName"] = rDraft["CardName"];
                                        delData = DateTime.Parse(rDraft["DocDueDate"].ToString());
                                        row["DocDate"] = DateTime.Parse(rDraft["DocDate"].ToString()).ToString("yyyyMMdd");
                                        row["DocDueDate"] = delData.ToString("yyyyMMdd");
                                        row["Comments"] = rDraft["Comments"];
                                        row["U_AB_PORemarks"] = rDraft["U_AB_PORemarks"];
                                        row["OutletCode"] = rDraft["U_AB_POWhsCode"];
                                        row["U_AB_UserCode"] = rDraft["U_AB_UserCode"];
                                        row["U_AB_POWhsCode"] = rDraft["U_AB_POWhsCode"];
                                        row["U_AB_Urgent"] = rDraft["U_AB_Urgent"];
                                        row["U_AB_SentSupplier"] = "N";
                                        row["U_AB_Time"] = DateTime.Now.ToString("hh : mm tt");
                                        whsCode = rDraft["U_AB_POWhsCode"].ToString();

                                        tblHeader.Rows.Add(row);
                                    }

                                    DataTable tblItem = new DataTable();
                                    tblItem.TableName = "POR1";
                                    tblItem.Columns.Add("ItemCode");
                                    tblItem.Columns.Add("Dscription");
                                    tblItem.Columns.Add("UnitMsr");
                                    tblItem.Columns.Add("Quantity");
                                    tblItem.Columns.Add("Price");
                                    tblItem.Columns.Add("LineTotal");
                                    tblItem.Columns.Add("Vatgroup");
                                    tblItem.Columns.Add("ShipType");
                                    tblItem.Columns.Add("TrnspName");
                                    tblItem.Columns.Add("WhsCode");
                                    tblItem.Columns.Add("U_AB_POQty");

                                    tblItem.Columns.Add("OcrCode");
                                    //New Request 24/6/2014
                                    tblItem.Columns.Add("CogsOcrCod");
                                    double checkQuantity = 0;
                                    foreach (DataRow r in ds.Tables[1].Rows)
                                    {
                                        DataRow rowNew = tblItem.NewRow();
                                        rowNew["ItemCode"] = r["ItemCode"];
                                        rowNew["Quantity"] = r["Quantity"];
                                        rowNew["Dscription"] = r["Dscription"];
                                        rowNew["LineTotal"] = r["LineTotal"];
                                        rowNew["UnitMsr"] = r["BuyUnitMsr"];
                                        rowNew["Price"] = r["Price"];
                                        rowNew["Vatgroup"] = r["VatgroupPu"];
                                        rowNew["ShipType"] = r["ShipType"];
                                        rowNew["TrnspName"] = r["TrnspName"];
                                        rowNew["U_AB_POQty"] = r["Quantity"];
                                        rowNew["WhsCode"] = r["WhsCode"];
                                        rowNew["OcrCode"] = r["OcrCode"];
                                        //New Request 24/6/2014
                                        rowNew["CogsOcrCod"] = r["CogsOcrCod"];

                                        if (double.Parse(r["Quantity"].ToString()) != 0)
                                        {
                                            checkQuantity += double.Parse(r["Quantity"].ToString());
                                            tblItem.Rows.Add(rowNew);
                                        }
                                       
                                    }

                                    if (checkQuantity == 0)
                                    {
                                        this.lblError.Text = "#" + lblNo.Text + " - " + "- at least one line item must have quantity.";
                                        this.lblError.Visible = true;
                                        return;
                                    }

                                    dsUpdate.Tables.Add(tblHeader);
                                    dsUpdate.Tables.Add(tblItem);
                                    string errMsg = string.Empty;
                                    errMsg = obj.CheckAddPO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                                       Session[Utils.AppConstants.CompanyCode].ToString(), dsUpdate, delData, false, string.Empty);
                                    if (errMsg.Length > 0)
                                    {
                                        if (errMsg.Trim() == "The device is not ready.")
                                        {
                                            errMsg = "System is processing another request. Pls try again later.";
                                        }
                                        this.lblError.Text = "#" + lblNo.Text +  " - " + errMsg;
                                        this.lblError.Visible = true;
                                        return;
                                    }
                                    else
                                    {
                                        DataTable tbAddress = new DataTable();
                                        tbAddress.TableName = "POR12";
                                        tbAddress.Columns.Add("StreetS");
                                        tbAddress.Columns.Add("BlockS");
                                        tbAddress.Columns.Add("CityS");
                                        tbAddress.Columns.Add("CountryS");
                                        tbAddress.Columns.Add("ZipCodes");

                                        DataRow rAddress = FillShipToAddress(whsCode);
                                        if (rAddress != null)
                                        {
                                            DataRow rNew = tbAddress.NewRow();
                                            rNew["StreetS"] = rAddress["Street"];
                                            rNew["BlockS"] = rAddress["Block"];
                                            rNew["CountryS"] = rAddress["Country"];
                                            rNew["CityS"] = rAddress["City"];
                                            rNew["ZipCodes"] = rAddress["ZipCode"];
                                            tbAddress.Rows.Add(rNew);
                                            dsUpdate.Tables.Add(tbAddress);
                                        }
                                    }
                                    DataSet dsResult = obj.CreatePO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), Session[Utils.AppConstants.CompanyCode].ToString(), "1", "0", dsUpdate, false, lblDocEntry.Text);
                                    if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                                    {
                                        if ((int)dsResult.Tables[0].Rows[0]["ErrCode"] != 0)
                                        {
                                            string err = dsResult.Tables[0].Rows[0]["ErrMsg"].ToString();
                                            if (err.Trim() == "The device is not ready.")
                                            {
                                                err = "System is processing another request. Pls try again later.";
                                            }
                                            this.lblError.Text = err;
                                        }
                                        else
                                        {
                                            if (Session["SentToHQ"] == null)
                                            {
                                                dicSent = new Dictionary<string, string>();
                                            }
                                            else
                                            {
                                                dicSent = (Dictionary<string, string>)Session["SentToHQ"];
                                            }
                                            dicSent.Add(lblDocEntry.Text, "");
                                            Session["SentToHQ"] = dicSent;
                                            this.lblError.Text = "Operation complete successful!";
                                        }
                                    }
                                    else
                                    {
                                        this.lblError.Text = "System is processing another request. Pls try again later.";
                                    }
                                }
                            }
                            else
                            {
                                this.lblError.Text = "System is processing another request. Pls try again later.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
            }
            this.lblError.Visible = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dr"></param>
        private DataRow FillShipToAddress(string outletId)
        {
            try
            {
                if (outletId.Length > 0)
                {
                    MasterData obj = new MasterData();
                    DataSet tbWareHouse = obj.GetWareHouseAddress(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
               Session[Utils.AppConstants.CompanyCode].ToString());
                    if (tbWareHouse != null && tbWareHouse.Tables.Count > 0)
                    {
                        DataRow[] r = tbWareHouse.Tables[0].Select("WhsCode = '" + outletId + "'");
                        if (r.Length > 0)
                        {
                            return r[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
            return null;
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
    }
}