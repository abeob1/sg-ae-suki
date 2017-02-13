using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Suki
{
    public partial class CreatePO : System.Web.UI.Page
    {
        private string _docEntry = string.Empty;
        private string _poNo = string.Empty;
        private string _isPO = string.Empty; //1: PO, 0: Draft
        private string _companyCode = string.Empty;
        private string _status = string.Empty;
        private string _Cstatus = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.Private);
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
            _isPO = Request.QueryString["IsPO"];
            _poNo = Request.QueryString["PONo"];
            _status = Request.QueryString["Status"];
            _Cstatus = Request.QueryString["CStatus"];
            if (Session[Utils.AppConstants.CompanyCode] != null)
            {
                _companyCode = Session[Utils.AppConstants.CompanyCode].ToString();
                if (Session[Utils.AppConstants.CompanyCode] != null && Request.QueryString["CompanyCode"] != null)
                {
                    string companyCode = Request.QueryString["CompanyCode"].ToString();
                    if (Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y")
                    {
                        _companyCode = companyCode;
                    }
                }
                this.lblPoNo.Text = _poNo;
                if (_isPO == null || _isPO.Length == 0)
                {
                    Response.Redirect("ErrorPage.aspx");
                }
                if (Request.QueryString["Create"] == null)
                {
                    _docEntry = Request.QueryString["DocEntry"];
                    if (_docEntry == null || _docEntry.Length == 0)
                    {
                        Response.Redirect("ErrorPage.aspx");
                    }
                    if (!Utils.AppConstants.isInteger(_docEntry))
                    {
                        Response.Redirect("ErrorPage.aspx");
                    }
                }
                else
                {
                    this.btnCancel.Visible = false;
                    this.btnClose.Visible = false;
                }
                if (_isPO == "1" && Request.QueryString["Create"] == null)
                {

                    this.btnSaveDraft.Visible = false;
                    this.btnUpdate.Text = "Update PO";
                    this.lblTitle.Text = "View Purchase Order";
                    this.grvPO.Columns[9].Visible = true;
                    this.grvPO.Columns[10].Visible = true;
                    this.btnShowPopup.Enabled = true;
                    this.lblStatus.Text = _status;
                    this.lblCStatus.Text = _Cstatus;

                }
                else if (_isPO == "0" && Request.QueryString["Create"] == null)
                {
                    this.btnCancel.Text = "Delete Draft";
                    this.btnClose.Visible = false;
                    this.btnSaveDraft.Text = "Update As Draft";
                    this.btnUpdate.Text = "Send to HQ";
                    this.lblTitle.Text = "Update Draft PO";
                    this.lblStatus.Text = _status;
                    this.lblCStatus.Text = _Cstatus;
                }
                if (!IsPostBack)
                {
                    LoadOrderWareHouse();
                    if (Request.QueryString["DocEntry"] != null && Request.QueryString["DocEntry"].ToString().Length > 0)
                    {
                        //Set Update Process
                        this.hdnIsUpdate.Value = "1";
                        this.btnSelectVendor.Visible = false;
                        this.txtVendorName.Enabled = false;

                        LoadData(_docEntry, _isPO);
                    }
                    else
                    {
                        //Set Create New Process
                        this.hdnIsUpdate.Value = "0";
                        this.btnSaveDraft.Enabled = false;
                        this.btnUpdate.Enabled = false;
                        this.txtDelDate.Text = DateTime.Now.Date.ToString(Utils.AppConstants.DATE);
                        this.txtPODate.Text = DateTime.Now.Date.ToString(Utils.AppConstants.DATE);
                        Session["ChosenItem"] = null;
                        Session["OrderWareHouse"] = null;
                        LoadData();
                    }
                    if ((this.hdnStatus.Value == "C" || Session[Utils.AppConstants.IsSupperUser].ToString() == "N") && Request.QueryString["Create"] == null && _isPO == "1")
                    {
                        DisableControl();
                    }
                }
                if (Session[Utils.AppConstants.IsSupperUser] != null && Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "N" && Request.QueryString["Create"] == null && _isPO == "1")
                {
                    this.btnUpdate.Visible = false;
                    this.btnCancel.Visible = false;
                    this.btnClose.Visible = false;
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvSearchResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.grvPO.PageIndex = e.NewPageIndex;
            DataTable tblIPO = (DataTable)Session["POTable"];
            BindData(tblIPO);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblError.Text = "";
                GridViewRow row = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                Label lblItemCode = (Label)row.FindControl("lblItemCode");
                HiddenField hdnLineNum = (HiddenField)row.FindControl("hdnLineNum");
                DataTable tb = (DataTable)Session["POTable"];
                if (tb != null)
                {
                    DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                    if (rupdate.Length > 0)
                    {
                        Dictionary<string, string> dicItem = (Dictionary<string, string>)Session["ChosenItem"];
                        if (dicItem != null)
                        {
                            if (dicItem.ContainsKey(rupdate[0]["ItemCode"].ToString()))
                            {
                                //Delete SAP
                                if (this.hdnIsUpdate.Value == "1")
                                {
                                    DataTable tbDelete = new DataTable();
                                    tbDelete.Columns.Add("LineNum");
                                    if (Session["LineDeleted"] == null)
                                    {
                                        Session["LineDeleted"] = tbDelete;
                                    }
                                    else
                                    {
                                        tbDelete = (DataTable)Session["LineDeleted"];
                                    }
                                    DataRow r = tbDelete.NewRow();
                                    r["LineNum"] = hdnLineNum.Value;
                                    tbDelete.Rows.Add(r);
                                    Session["LineDeleted"] = tbDelete;
                                }
                                dicItem.Remove(rupdate[0]["ItemCode"].ToString());
                            }
                        }
                        tb.Rows.Remove(rupdate[0]);
                    }
                    this.grvPO.EditIndex = -1;
                    BindData((DataTable)Session["POTable"]);
                    CalcTotal(((DataTable)Session["POTable"]));
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
        protected void grvSearchResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
                int index = 0;
                TextBox ddl1 = e.Row.FindControl("txtOrderQuantity") as TextBox;
                ddl1.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + ddl1.ClientID + "').select();");
                if (ViewState["rowindex"] != null && Session["POTable"] != null)
                {
                    index = Convert.ToInt32(ViewState["rowindex"].ToString());
                    int rowCount = ((DataTable)Session["POTable"]).Rows.Count;
                    if ((index + 1) <= this.grvPO.Rows.Count)
                    {
                        if (e.Row.RowIndex == index + 1)
                        {
                            ddl1.Focus();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void DisableControl()
        {
            this.btnUpdate.Visible = false;
            this.btnSaveDraft.Visible = false;
            this.btnShowPopup.Visible = false;
            this.drpOrderWareHouse.Enabled = false;
            this.txtPODate.Enabled = false;
            this.txtDelDate.Enabled = false;
            this.txtRemarks.Enabled = false;
            this.chkUrgent.Enabled = false;
            this.btnClose.Visible = false;
            this.btnCancel.Visible = false;
            this.grvPO.Columns[0].Visible = false;
            for (int i = 0; i < this.grvPO.Rows.Count; i++)
            {
                TextBox txtOrderQuantity = (TextBox)this.grvPO.Rows[i].FindControl("txtOrderQuantity");
                txtOrderQuantity.Enabled = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="docEntry"></param>
        /// <param name="type"></param>
        private void LoadData(string docEntry, string type)
        {
            try
            {
                if (docEntry != null && docEntry.Length > 0)
                {
                    MasterData obj = new MasterData();
                    DataTable tblIPO = CreateTabelFormat();
                    DataSet ds = new DataSet();
                    string headerName = string.Empty;
                    string detailName = string.Empty;
                    //Load PO
                    if (type == "1")
                    {
                        ds = obj.GetPODataUpdate(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                                _companyCode, docEntry);
                        headerName = "OPOR";
                        detailName = "POR1";
                    }
                    //Load Draft
                    else if (type == "0")
                    {
                        ds = obj.GetPODraftUpdate(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                                _companyCode, docEntry);
                        headerName = "ODRF";
                        detailName = "DRF1";
                    }
                    if (ds != null && ds.Tables.Count > 1)
                    {
                        //Header
                        if (ds.Tables.Contains(headerName))
                        {
                            foreach (DataRow row in ds.Tables[headerName].Rows)
                            {
                                this.txtDelDate.Text = Convert.ToDateTime(row["DocDueDate"].ToString()).ToString(Utils.AppConstants.DATE);
                                this.txtPODate.Text = Convert.ToDateTime(row["DocDate"].ToString()).ToString(Utils.AppConstants.DATE);
                                this.txtVendorCode.Text = row["CardCode"].ToString();
                                this.txtVendorName.Text = row["CardName"].ToString();
                                this.lblDocumentTotal.Text = string.Format("${0}", decimal.Parse(row["DocTotal"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT));
                                this.lblGSTAmount.Text = string.Format("${0}", decimal.Parse(row["VATSum"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT));
                                this.lblSubTotal.Text = string.Format("${0}", decimal.Parse(row["SubTotal"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT));
                                this.txtRemarks.Text = row["U_AB_PORemarks"].ToString();
                                this.hdnStatus.Value = row["DocStatus"].ToString();
                                this.chkUrgent.Checked = row["U_AB_Urgent"].ToString() == "Y" ? true : false;
                                this.hdnCreatedUser.Value = row["U_AB_UserCode"].ToString();
                                if (row["U_AB_SentSupplier"].ToString().ToUpper() == "Y")
                                {
                                    this.btnCancel.Visible = false;
                                }
                                if (this.drpOrderWareHouse.Items.Count > 0)
                                {
                                    this.drpOrderWareHouse.SelectedValue = row["U_AB_POWhsCode"].ToString();
                                    this.drpOrderWareHouse.Enabled = false;
                                }
                            }
                            DisplayCalendar();
                        }
                        //Detail
                        if (ds.Tables.Contains(detailName))
                        {
                            //Get Holding Setup
                            double documentTotal = 0;
                            double gstTotal = 0;
                            double subTotal = 0;
                            DataSet dsItem = obj.GetSupplierItemSetup(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                                _companyCode, this.txtVendorCode.Text, this.drpOrderWareHouse.SelectedValue.ToString(),DateTime.Now);
                            Dictionary<string, string> dicItem = new Dictionary<string, string>();
                            foreach (DataRow row in ds.Tables[detailName].Rows)
                            {
                                
                                DataRow rowNew = tblIPO.NewRow();
                                rowNew["ItemCode"] = row["ItemCode"];
                                rowNew["Dscription"] = row["Dscription"];

                                if (dsItem != null && dsItem.Tables.Count > 0)
                                {
                                    foreach (DataRow r in dsItem.Tables[0].Rows)
                                    {
                                        if (r["ItemCode"].ToString() == row["ItemCode"].ToString())
                                        {
                                            rowNew["LB"] = r["LB"];
                                            rowNew["MinStock"] = double.Parse(r["MinQty"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                            rowNew["MaxStock"] = double.Parse(r["MaxQty"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                        }
                                    }
                                }
                                rowNew["BuyUnitMsr"] = row["BuyUnitMsr"];
                                rowNew["Quantity"] = double.Parse(row["Quantity"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                rowNew["ReceivedQty"] = "0.0000";
                                rowNew["BalanceQty"] = "0.0000";
                                if (type == "1")
                                {
                                    rowNew["Price"] = decimal.Parse(row["Price"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                    rowNew["LineTotal"] = decimal.Parse(row["LineTotal"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                    rowNew["SAPPrice"] = row["Price"];
                                }
                                else if (type == "0")
                                {
                                    if (row["PriceBefDi"].ToString().Length > 0)
                                    {
                                        rowNew["Price"] = decimal.Parse(row["PriceBefDi"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);

                                        rowNew["SAPPrice"] = row["PriceBefDi"];
                                    }
                                    else
                                    {
                                        rowNew["Price"] = "0.0000";
                                        rowNew["SAPPrice"] = "0.0000";
                                    }
                                    decimal lineTotal = decimal.Parse(rowNew["Price"].ToString()) * decimal.Parse(rowNew["Quantity"].ToString());
                                    rowNew["LineTotal"] = lineTotal.ToString(Utils.AppConstants.NUMBER_FORMAT);
                                    subTotal += double.Parse(rowNew["LineTotal"].ToString());
                                    if (row["Rate"] != null && row["Rate"].ToString().Length > 0)
                                    {
                                        gstTotal += (double.Parse(rowNew["LineTotal"].ToString()) * double.Parse(row["Rate"].ToString())) / 100;
                                    }
                                    else
                                    {
                                        gstTotal += (double.Parse(rowNew["LineTotal"].ToString()) * 8.25) / 100;
                                    }
                                }
                              
                                rowNew["VatgroupPu"] = row["VatgroupPu"];
                                rowNew["Rate"] = row["Rate"];
                                rowNew["ShipType"] = row["ShipType"];
                                rowNew["TrnspName"] = row["TrnspName"];
                                rowNew["IsGRPO"] = row["IsGRPO"];
                                rowNew["LineNum"] = row["LineNum"];
                                if (row["IsGRPO"].ToString() != "")
                                {
                                    if (row["BalanceQty"].ToString() != "")
                                    {
                                        rowNew["ReceivedQty"] = Math.Round(decimal.Parse(row["BalanceQty"].ToString()), 2).ToString() == "0" ? "0.0000" : Math.Round(decimal.Parse(row["BalanceQty"].ToString()), 2).ToString();
                                        decimal bal = Math.Round(decimal.Parse(row["Quantity"].ToString()) - decimal.Parse(row["BalanceQty"].ToString()), 2);
                                        rowNew["BalanceQty"] = bal.ToString(Utils.AppConstants.NUMBER_FORMAT);
                                    }
                                }
                                else
                                {
                                    rowNew["BalanceQty"] = decimal.Parse(row["Quantity"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                }
                                if (!dicItem.ContainsKey(row["ItemCode"].ToString()))
                                {
                                    dicItem.Add(row["ItemCode"].ToString(), row["Dscription"].ToString());
                                    tblIPO.Rows.Add(rowNew);
                                }
                            }
                            if (type == "0")
                            {
                                documentTotal += subTotal + gstTotal;
                                this.lblDocumentTotal.Text = string.Format("${0}", documentTotal.ToString(Utils.AppConstants.NUMBER_FORMAT));
                                this.lblGSTAmount.Text = string.Format("${0}", gstTotal.ToString(Utils.AppConstants.NUMBER_FORMAT));
                                this.lblSubTotal.Text = string.Format("${0}", subTotal.ToString(Utils.AppConstants.NUMBER_FORMAT));
                            }
                            //ReLoad Data
                            BindData(tblIPO);
                            for (int i = 0; i < this.grvPO.Rows.Count; i++)
                            {
                                HiddenField isGRPO = this.grvPO.Rows[i].FindControl("hdnIsGRPO") as HiddenField;
                                TextBox txtOrderQuantity = this.grvPO.Rows[i].FindControl("txtOrderQuantity") as TextBox;
                                if (isGRPO.Value != "")
                                {
                                    txtOrderQuantity.Enabled = false;
                                }
                            }
                            Session["ChosenItem"] = dicItem;
                        }
                        else
                        {
                            this.lblError.Text = "This PO is invalid. Pls create orther PO.";
                            this.lblError.Visible = true;
                        }
                    }
                }
                LoadSupplierAmount();
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
        private void LoadSupplierAmount()
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet ds = obj.GetSupplierAmount(this.txtVendorCode.Text, _companyCode, this.drpOrderWareHouse.SelectedValue.ToString());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    this.lblMinAmount.Text =double.Parse( ds.Tables[0].Rows[0]["MinAmt"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                    this.lblMaxAmount.Text = double.Parse(ds.Tables[0].Rows[0]["MaxAmt"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
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
        protected void txtOrderQuantity_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblError.Visible = false;
                GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
                ViewState["rowindex"] = row.RowIndex;
                TextBox txtQuantity = (TextBox)row.FindControl("txtOrderQuantity");
                if (txtQuantity.Text.Trim().Length == 0)
                {
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert('Pls input quantity');", true);
                    txtQuantity.Focus();
                    return;
                }
                Label lblItemCode = (Label)row.FindControl("lblItemCode");

                DataTable tb = (DataTable)Session["POTable"];
                if (tb != null)
                {
                    DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                    if (rupdate.Length > 0)
                    {
                        rupdate[0]["Quantity"] = txtQuantity.Text;
                        GetPrice(rupdate[0]);
                    }
                }
                this.grvPO.EditIndex = -1;

                BindData(tb);
                CalcTotal(tb);
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
        private void LoadOrderWareHouse()
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet ds = obj.GetABOutletByCompany(_companyCode);
                this.drpOrderWareHouse.Items.Clear();
                if (ds != null && ds.Tables.Count > 0)
                {
                    Session["OrderWareHouse"] = ds.Tables[0];
                    if (Session[Utils.AppConstants.IsSupperUser] != null 
                        && Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "N"
                         && (Session[Utils.AppConstants.IsCompanySuperUser].ToString().ToUpper() == "N" 
                         || Session[Utils.AppConstants.IsCompanySuperUser].ToString()==string.Empty))
                    {
                        if (Session[Utils.AppConstants.ListOutlet] != null)
                        {
                            Dictionary<string, string> dicOutlet = (Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet];
                            foreach (KeyValuePair<string, string> item in dicOutlet)
                            {
                                DataRow[] rOutlet = ds.Tables[0].Select("U_WhseCode='" +item.Key + "'");
                                if (rOutlet.Length > 0)
                                {
                                    this.drpOrderWareHouse.Items.Add(new ListItem(rOutlet[0]["U_WhseName"].ToString(), rOutlet[0]["U_WhseCode"].ToString()));
                                }
                            }
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
                        if (this.drpOrderWareHouse.Items.Count > 0)
                        {
                            this.drpOrderWareHouse.SelectedIndex = 0;
                        }
                    }
                    FillShipToAddress(this.drpOrderWareHouse.SelectedValue.ToString(), true);
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
        /// <param name="dr"></param>
        private DataRow FillShipToAddress(string outletId, bool isFill)
        {
            try
            {
                if (outletId.Length > 0)
                {
                    MasterData obj = new MasterData();
                    DataSet tbWareHouse = obj.GetWareHouseAddress(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
               _companyCode);
                    if (tbWareHouse != null && tbWareHouse.Tables.Count > 0)
                    {

                        DataRow[] r = tbWareHouse.Tables[0].Select("WhsCode = '" + outletId + "'");
                        if (r.Length > 0)
                        {
                            if (isFill)
                            {
                                StringBuilder shipTo = new StringBuilder();
                                shipTo.AppendLine(r[0]["Block"].ToString());
                                shipTo.AppendLine(r[0]["Street"].ToString() + " " + r[0]["City"].ToString());
                                shipTo.AppendLine(r[0]["Country"].ToString() + " " + r[0]["ZipCode"].ToString());
                                this.txtShipTo.Text = shipTo.ToString();
                            }
                            else
                            {
                                return r[0];
                            }
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
        protected void EditItem(object sender, GridViewEditEventArgs e)
        {
            this.grvPO.EditIndex = e.NewEditIndex;
            BindData((DataTable)Session["POTable"]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelEdit(object sender, GridViewCancelEditEventArgs e)
        {
            e.Cancel = true;
            this.grvPO.EditIndex = -1;
            BindData((DataTable)Session["POTable"]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DeleteItem(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                e.Cancel = true;
                GridViewRow row = (GridViewRow)this.grvPO.Rows[e.RowIndex];
                Label lblItemCode = (Label)row.FindControl("lblItemCode");
                DataTable tb = (DataTable)Session["POTable"];
                if (tb != null)
                {
                    DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                    if (rupdate.Length > 0)
                    {
                        Dictionary<string, string> dicItem = (Dictionary<string, string>)Session["ChosenItem"];
                        if (dicItem != null)
                        {
                            if (dicItem.ContainsKey(rupdate[0]["ItemCode"].ToString()))
                            {
                                dicItem.Remove(rupdate[0]["ItemCode"].ToString());
                            }
                        }
                        tb.Rows.Remove(rupdate[0]);
                    }
                    this.grvPO.EditIndex = -1;
                    BindData((DataTable)Session["POTable"]);
                    CalcTotal(((DataTable)Session["POTable"]));
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
        protected void UpdateItem(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                ViewState["rowindex"] = e.RowIndex;
                GridViewRow row = (GridViewRow)this.grvPO.Rows[e.RowIndex];
                TextBox txtQuantity = (TextBox)row.FindControl("txtOrderQuantity");
                if (txtQuantity.Text.Trim().Length == 0 || int.Parse(txtQuantity.Text.Trim()) == 0)
                {
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert('In Quantity column, enter whole number greater than 0');", true);
                    txtQuantity.Focus();
                    return;
                }
                Label lblItemCode = (Label)row.FindControl("lblItemCode");

                DataTable tb = (DataTable)Session["POTable"];
                if (tb != null)
                {
                    DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                    if (rupdate.Length > 0)
                    {
                        rupdate[0]["Quantity"] = txtQuantity.Text;
                        GetPrice(rupdate[0]);
                    }

                    this.grvPO.EditIndex = -1;

                    BindData(tb);
                    CalcTotal(tb);
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }
        private DataTable CreateTabelFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Dscription");
            tbTemp.Columns.Add("LB");
            tbTemp.Columns.Add("MinStock");
            tbTemp.Columns.Add("MaxStock");
            tbTemp.Columns.Add("BuyUnitMsr");
            tbTemp.Columns.Add("Quantity");
            tbTemp.Columns.Add("ReceivedQty");
            tbTemp.Columns.Add("Price");
            tbTemp.Columns.Add("LineTotal");
            tbTemp.Columns.Add("VatgroupPu");
            tbTemp.Columns.Add("Rate");
            tbTemp.Columns.Add("ShipType");
            tbTemp.Columns.Add("TrnspName");
            tbTemp.Columns.Add("IsGRPO");
            tbTemp.Columns.Add("BalanceQty");
            tbTemp.Columns.Add("LineNum");
            tbTemp.Columns.Add("SAPPrice");
            return tbTemp;
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadData()
        {
            BindData(CreateTabelFormat());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tblData"></param>
        private void BindData(DataTable tblData)
        {
            Session["POTable"] = tblData;

            DataView dv = tblData.DefaultView;

            dv.Sort = "ItemCode  ASC";

            this.grvPO.DataSource = dv.ToTable();
            this.grvPO.DataBind();
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
                    DateTime poDate = Utils.AppConstants.ConvertToDate(this.txtPODate.Text.Trim());
                    DataTable tblIPO = (DataTable)Session["POTable"];
                    switch (this.Request["__EVENTARGUMENT"].ToString())
                    {
                        case "SelectItem":
                            this.lblError.Visible = false;
                            if (tblIPO != null)
                            {
                                Dictionary<string, string> dicItem = Session["ChosenItem"] != null ? (Dictionary<string, string>)Session["ChosenItem"] : null;
                                if (dicItem != null)
                                {
                                    MasterData obj = new MasterData();
                                    DataSet dsItem = obj.GetSupplierItemSetup(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                             _companyCode, this.txtVendorCode.Text, this.drpOrderWareHouse.SelectedValue.ToString(), DateTime.Now);
                                    foreach (KeyValuePair<string, string> item in dicItem)
                                    {
                                        DataRow[] existRow = tblIPO.Select("ItemCode = '" + item.Key + "'");
                                        if (existRow.Length == 0)
                                        {
                                         
                                            if (dsItem != null && dsItem.Tables.Count > 0)
                                            {
                                                foreach (DataRow r in dsItem.Tables[0].Rows)
                                                {
                                                    if (r["ItemCode"].ToString() == item.Key)
                                                    {
                                                        tblIPO = AddNewItem(tblIPO, poDate, r["ItemCode"].ToString(), r["ItemName"].ToString(), r["LB"].ToString(), decimal.Parse(r["MinQty"].ToString()), decimal.Parse(r["MaxQty"].ToString()), this.txtVendorCode.Text, 0);
                                                    }
                                                }
                                            }
                                            //ReLoad Data
                                            BindData(tblIPO);
                                            if (tblIPO.Rows.Count > 0)
                                            {
                                                //Update Price
                                                UpdatePrice();
                                                CalcTotal(tblIPO);
                                            }
                                        }
                                    }
                                }
                            }
                            //ReLoad Data
                            BindData(tblIPO);
                            break;
                        case "SelectVendor":
                            //Clear Data
                            if (tblIPO != null)
                            {
                                tblIPO.Rows.Clear();
                            }
                            BindData(tblIPO);
                            ViewState["rowindex"] = -1;
                            this.lblError.Visible = false;
                            this.txtVendorCode.Text = Session["ChosenVendorCode"] != null ? Session["ChosenVendorCode"].ToString() : "";
                            this.txtVendorName.Text = Session["ChosenVendorName"] != null ? Session["ChosenVendorName"].ToString() : "";
                            Session["ChosenItem"] = null;
                            tblIPO = LoadSetupItem(poDate, tblIPO);
                            LoadSupplierAmount();
                            DisplayCalendar();
                    
                            this.btnShowPopup.Enabled = true;
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

        private void DisplayCalendar()
        {
            try
            {
                //Load Calendar 
                MasterData objC = new MasterData();
                DataSet ds = objC.GetOutletCalendar("", "", this.txtVendorCode.Text, Session[Utils.AppConstants.CompanyCode].ToString(), "");
                if (ds != null && ds.Tables.Count > 0)
                {
                    this.chkCalendar.Items.Clear();
                    DataRow[] rCalendar = ds.Tables[0].Select("OutletCode='" + this.drpOrderWareHouse.SelectedValue.ToString() + "'");
                    DataTable tbNew = new DataTable();
                    tbNew.Columns.Add("Name");
                    tbNew.Columns.Add("Value");
                    if (rCalendar.Length > 0)
                    {
                        int colIndex = 7;
                        for (int i = colIndex; i < rCalendar[0].Table.Columns.Count; i++)
                        {
                            DataRow rNew = tbNew.NewRow();
                            rNew["Name"] = rCalendar[0].Table.Columns[i].ColumnName;
                            rNew["Value"] = rCalendar[0][rCalendar[0].Table.Columns[i].ColumnName].ToString();
                            tbNew.Rows.Add(rNew);
                        }
                    }

                    this.chkCalendar.DataSource = tbNew;
                    this.chkCalendar.DataTextField = "Name";
                    this.chkCalendar.DataValueField = "Value";
                    this.chkCalendar.DataBind();
                    if (this.chkCalendar.Items.Count > 0)
                    {
                        for (int i = 0; i < this.chkCalendar.Items.Count; i++)
                        {
                            if (Convert.ToBoolean(this.chkCalendar.Items[i].Value.ToString()) == true)
                            {
                                this.chkCalendar.Items[i].Selected = true;
                            }
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
        /// <param name="poDate"></param>
        /// <param name="tblIPO"></param>
        /// <returns></returns>
        private DataTable LoadSetupItem(DateTime poDate, DataTable tblIPO)
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet dsItem = obj.GetSupplierItemSetup(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                    _companyCode, this.txtVendorCode.Text, this.drpOrderWareHouse.SelectedValue.ToString(), DateTime.Now);
                Dictionary<string, string> dicItem = new Dictionary<string, string>();
                if (dsItem != null && dsItem.Tables.Count > 0)
                {
                    dsItem.Tables[0].Columns.Add("Quantity");
                    DataSet dsAllPrice = obj.GetAllItem(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                            _companyCode, poDate, this.txtVendorCode.Text, dsItem.Tables[0]);
                    foreach (DataRow r in dsItem.Tables[0].Rows)
                    {
                        if (!dicItem.ContainsKey(r["ItemCode"].ToString()))
                        {
                            dicItem.Add(r["ItemCode"].ToString(), r["ItemName"].ToString());
                        }
                        if (dsAllPrice != null && dsAllPrice.Tables.Count > 0)
                        {
                            foreach (DataRow row in dsAllPrice.Tables[0].Rows)
                            {
                                if (r["ItemCode"].ToString() == row["ItemCode"].ToString())
                                {
                                    DataRow rowNew = tblIPO.NewRow();
                                    rowNew["ItemCode"] = r["ItemCode"].ToString();
                                    rowNew["Dscription"] = r["ItemName"].ToString();
                                    rowNew["LB"] = r["LB"].ToString();
                                    rowNew["MinStock"] = Math.Round(decimal.Parse(r["MinQty"].ToString()), 4).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                    rowNew["MaxStock"] = Math.Round(decimal.Parse(r["MaxQty"].ToString()), 4).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                    rowNew["BuyUnitMsr"] = row["BuyUnitMsr"];
                                    rowNew["Quantity"] = decimal.Parse("0").ToString(Utils.AppConstants.NUMBER_FORMAT);
                                    rowNew["ReceivedQty"] = row["ReceivedQty"];
                                    rowNew["Price"] = (decimal.Parse(row["NumInBuy"].ToString()) * decimal.Parse(row["Price"].ToString())).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                    rowNew["SAPPrice"] = (decimal.Parse(row["NumInBuy"].ToString()) * decimal.Parse(row["Price"].ToString()));
                                    rowNew["LineTotal"] = decimal.Parse(row["LineTotal"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                    rowNew["VatgroupPu"] = row["VatgroupPu"];
                                    rowNew["Rate"] = row["Rate"];
                                    rowNew["ShipType"] = row["ShipType"];
                                    rowNew["TrnspName"] = row["TrnspName"];
                                    DataRow[] rCheck = tblIPO.Select("ItemCode='" + r["ItemCode"].ToString() + "'");
                                    if (rCheck.Length == 0)
                                    {
                                        tblIPO.Rows.Add(rowNew);
                                    }
                                }
                            }
                        }
                    }
                }
                Session["ChosenItem"] = dicItem;
                //ReLoad Data
                BindData(tblIPO);
                if (tblIPO.Rows.Count > 0)
                {
                    CalcTotal(tblIPO);
                }
                this.btnSaveDraft.Enabled = true;
                this.btnUpdate.Enabled = true;
                this.btnShowPopup.Enabled = true;
                return tblIPO;
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdatePrice()
        {
            try
            {
                //Update Price
                foreach (DataRow r in ((DataTable)Session["POTable"]).Rows)
                {
                    GetPrice(r);
                }
                BindData((DataTable)Session["POTable"]);
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }
        /// <summary>
        /// Update Price
        /// </summary>
        /// <param name="r"></param>
        private void GetPrice(DataRow r)
        {
            try
            {
                DataSet ds = GetDataItem(Utils.AppConstants.ConvertToDate(this.txtPODate.Text.Trim()), r["ItemCode"].ToString(), this.txtVendorCode.Text, double.Parse(r["Quantity"].ToString()));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    r["Price"] = (decimal.Parse(ds.Tables[0].Rows[0]["NumInBuy"].ToString()) * decimal.Parse(ds.Tables[0].Rows[0]["Price"].ToString())).ToString(Utils.AppConstants.NUMBER_FORMAT);

                    r["SAPPrice"] = (decimal.Parse(ds.Tables[0].Rows[0]["NumInBuy"].ToString()) * decimal.Parse(ds.Tables[0].Rows[0]["Price"].ToString()));

                    r["LineTotal"] = (double.Parse(r["Price"].ToString()) * double.Parse(r["Quantity"].ToString())).ToString(Utils.AppConstants.NUMBER_FORMAT);
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
        /// <param name="tblIPO"></param>
        /// <param name="poDate"></param>
        /// <param name="itemCode"></param>
        /// <param name="itemName"></param>
        /// <param name="venderCode"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private DataTable AddNewItem(DataTable tblIPO, DateTime poDate, string itemCode, string itemName, string lb, decimal min, decimal max, string venderCode, double quantity)
        {
            try
            {
                DataSet ds = GetDataItem(poDate, itemCode, venderCode, quantity);
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DataRow rowNew = tblIPO.NewRow();
                        rowNew["ItemCode"] = itemCode;
                        rowNew["Dscription"] = itemName;
                        rowNew["LB"] = lb;
                        rowNew["MinStock"] = Math.Round(min, 2);
                        rowNew["MaxStock"] = Math.Round(max, 2);
                        rowNew["BuyUnitMsr"] = row["BuyUnitMsr"];
                        rowNew["Quantity"] = quantity.ToString(Utils.AppConstants.NUMBER_FORMAT);
                        rowNew["ReceivedQty"] = row["ReceivedQty"];
                        rowNew["Price"] = decimal.Parse(row["Price"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                        rowNew["SAPPrice"] = row["Price"];
                        rowNew["LineTotal"] = decimal.Parse(row["LineTotal"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                        rowNew["VatgroupPu"] = row["VatgroupPu"];
                        rowNew["Rate"] = row["Rate"];
                        rowNew["ShipType"] = row["ShipType"];
                        rowNew["TrnspName"] = row["TrnspName"];
                        DataRow[] rCheck = tblIPO.Select("ItemCode='" + itemCode + "'");
                        if (rCheck.Length == 0)
                        {
                            tblIPO.Rows.Add(rowNew);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
            return tblIPO;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb"></param>
        private void CalcTotal(DataTable tb)
        {
            try
            {
                if (tb != null)
                {
                    double documentTotal = 0;
                    double gstTotal = 0;
                    double subTotal = 0;
                    foreach (DataRow row in tb.Rows)
                    {
                        subTotal += double.Parse(row["LineTotal"].ToString());
                        if (row["Rate"] != null && row["Rate"].ToString().Length > 0)
                        {
                            gstTotal += (double.Parse(row["LineTotal"].ToString()) * double.Parse(row["Rate"].ToString())) / 100;
                        }
                        else
                        {
                            gstTotal += (double.Parse(row["LineTotal"].ToString()) * 8.25) / 100;
                        }
                    }
                    documentTotal += subTotal + gstTotal;
                    this.lblDocumentTotal.Text = string.Format("${0}",documentTotal.ToString(Utils.AppConstants.NUMBER_FORMAT));
                    this.lblGSTAmount.Text = string.Format("${0}", gstTotal.ToString(Utils.AppConstants.NUMBER_FORMAT));
                    this.lblSubTotal.Text = string.Format("${0}", subTotal.ToString(Utils.AppConstants.NUMBER_FORMAT));
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
        /// <param name="poDate"></param>
        /// <param name="itemCode"></param>
        /// <param name="venderCode"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private DataSet GetDataItem(DateTime poDate, string itemCode, string venderCode, double quantity)
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet ds = new DataSet();
                ds = obj.GetItem(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                        _companyCode, poDate, itemCode, venderCode, quantity);
                return ds;
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
        private string CheckDate()
        {
            if (this.txtDelDate.Text.Trim().Length > 0 && this.txtPODate.Text.Trim().Length > 0)
            {
                if (Utils.AppConstants.isDateTime(this.txtPODate.Text) && Utils.AppConstants.isDateTime(this.txtDelDate.Text))
                {
                    if (Utils.AppConstants.ConvertToDate(this.txtPODate.Text).Date > Utils.AppConstants.ConvertToDate(this.txtDelDate.Text).Date)
                    {
                        return "Delivery Date cannot earliar than PO Issue Date.";
                    }
                }
                else
                {
                    return "PO Issue Date or Expected Delivery Date is incorrect format.";
                }
            }
            else
            {
                return "PO Issue Date or Expected Delivery Date is not Blank.";
            }
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isPO == "0")
                {
                    //Update Draft
                    this.lblError.Visible = false;
                    string requestXML = CollectData(true);
                    if (requestXML == "") return;
                    Transaction ts = new Transaction();
                    DataSet ds = new DataSet();
                    MasterData obj = new MasterData();
                    if (this.hdnIsUpdate.Value == "1")
                    {
                        ds = ts.CreateMarketingDocument(requestXML, Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                      _companyCode, "112", _docEntry, true);
                    }
                    if ((int)ds.Tables[0].Rows[0]["ErrCode"] != 0)
                    {
                        this.lblError.Text = ds.Tables[0].Rows[0]["ErrMsg"].ToString();
                        Utils.AppConstants.WriteLog(ds.Tables[0].Rows[0]["ErrMsg"].ToString(), true);
                        Utils.AppConstants.WriteLog(requestXML, true);
                        Utils.AppConstants.WriteLog(Session[Utils.AppConstants.UserCode].ToString(), true);
                        Utils.AppConstants.WriteLog(Session[Utils.AppConstants.CompanyCode].ToString(), true);


                        return;
                    }
                }
                //Sent to HQ
                string errMess = CheckDate();
                if (errMess.Length > 0)
                {
                    this.lblError.Text = errMess;
                    this.lblError.Visible = true;
                    return;
                }
                this.lblError.Visible = false;
                this.lblError.Text = "";
                DataSet dsData = new DataSet();
                dsData = CollectDataPO(false);
                if (dsData != null)
                {
                    DataTable tbDelete = (DataTable)Session["LineDeleted"];
                    if (tbDelete != null)
                    {
                        Transaction tns = new Transaction();
                        DocumentXML xml = new DocumentXML();
                        DataSet dsDelete = new DataSet();
                        dsDelete.Tables.Add(tbDelete.Copy());
                        if (tbDelete.Rows.Count > 0)
                        {
                            DataSet dss = tns.RemoveMarketingDocumentLine(dsDelete, Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), _companyCode,
                            "22", _docEntry);
                            Session["LineDeleted"] = null;
                        }
                    }

                    MasterData obj = new MasterData();
                    DataSet dsResult = obj.CreatePO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), _companyCode, this.hdnIsUpdate.Value, _isPO, dsData, false, _docEntry);
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

                            this.lblError.Text = "Operation complete successful!";

                            if (this.hdnIsUpdate.Value == "0")
                            {
                                ResetPage();
                            }
                            else if (this.hdnIsUpdate.Value == "1" && _isPO == "0")
                            {
                                DataSet dsPoNo = obj.GetPONo(int.Parse(dsResult.Tables[0].Rows[0]["ErrMsg"].ToString()), Session[Utils.AppConstants.CompanyCode].ToString());
                                if (dsPoNo != null && dsPoNo.Tables.Count > 0 && dsPoNo.Tables[0].Rows.Count > 0)
                                {
                                    string status = dsPoNo.Tables[0].Rows[0]["DocStatus"].ToString();
                                    if (status.ToUpper() == "O")
                                    {
                                        status = "Open";
                                    }
                                    if (status.ToUpper() == "C")
                                    {
                                        status = "Closed";
                                    }
                                    Response.Redirect("CreatePO.aspx?IsPO=1&DocEntry=" + dsResult.Tables[0].Rows[0]["ErrMsg"].ToString() +
                                        "&PONo=" + dsPoNo.Tables[0].Rows[0]["BeginStr"].ToString() + dsPoNo.Tables[0].Rows[0]["DocNum"].ToString()
                                        + "&Status=" + status);
                                }
                            }
                            else if (this.hdnIsUpdate.Value == "1" && _isPO == "1")
                            {
                                LoadData(_docEntry, _isPO);
                            }
                        }
                    }
                    else
                    {
                        this.lblError.Text = "System is processing another request. Pls try again later.";
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
        /// <returns></returns>
        public string CollectData(bool isDraft)
        {
            DocumentXML objInfo = new DocumentXML();
            DataSet ds = CreateData(isDraft);
            if (ds != null)
            {
                Session["SODATA"] = ds.Copy();
                if (isDraft)
                {
                    ds.Tables["ODRF"].Columns.Remove("OutletCode");
                    ds.Tables["DRF1"].Columns.Remove("ShipType");
                    ds.Tables["DRF1"].Columns.Remove("TrnspName");
                    return objInfo.ToXMLStringFromDS("112", ds);
                }
                else
                {
                    ds.Tables["OPOR"].Columns.Remove("OutletCode");
                    ds.Tables["POR1"].Columns.Remove("ShipType");
                    ds.Tables["POR1"].Columns.Remove("TrnspName");
                    return objInfo.ToXMLStringFromDS("22", ds);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDraft"></param>
        /// <returns></returns>
        public DataSet CollectDataPO(bool isDraft)
        {
            return CreateData(isDraft);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDraft"></param>
        /// <returns></returns>
        private DataSet CreateData(bool isDraft)
        {
            try
            {
                DataTable tb = ((DataTable)Session["POTable"]);
                if (tb != null && tb.Rows.Count > 0)
                {
                    DocumentXML objInfo = new DocumentXML();
                    DataSet ds = new DataSet("DS");

                    DataTable tblHeader = new DataTable("OPOR");
                    tblHeader.Columns.Add("CardCode");
                    tblHeader.Columns.Add("CardName");
                    tblHeader.Columns.Add("DocDate");
                    tblHeader.Columns.Add("DocDueDate");
                    tblHeader.Columns.Add("U_AB_PORemarks");
                    tblHeader.Columns.Add("OutletCode");
                    tblHeader.Columns.Add("U_AB_Urgent");
                    tblHeader.Columns.Add("U_AB_UserCode");
                    tblHeader.Columns.Add("U_AB_POWhsCode");
                    tblHeader.Columns.Add("U_AB_Time");
                    tblHeader.Columns.Add("U_AE_IFlag");
                    if (this.hdnIsUpdate.Value == "0")
                    {
                        tblHeader.Columns.Add("U_AB_SentSupplier");
                    }
                   
                    DataRow row = tblHeader.NewRow();
                    row["CardCode"] = this.txtVendorCode.Text;
                    row["CardName"] = this.txtVendorName.Text;
                    row["DocDate"] = Utils.AppConstants.ConvertToDate(this.txtPODate.Text.Trim()).ToString("yyyyMMdd");
                    row["DocDueDate"] = Utils.AppConstants.ConvertToDate(this.txtDelDate.Text.Trim()).ToString("yyyyMMdd");
                    row["U_AB_PORemarks"] = this.txtRemarks.Text;
                    row["OutletCode"] = this.drpOrderWareHouse.SelectedValue.ToString();
                    if (this.hdnCreatedUser.Value.ToString().Length == 0)
                    {
                        row["U_AB_UserCode"] = Session[Utils.AppConstants.UserCode] != null ? Session[Utils.AppConstants.UserCode].ToString() : "";
                    }
                    else
                    {
                        row["U_AB_UserCode"] = this.hdnCreatedUser.Value;
                    }
                    row["U_AB_POWhsCode"] = this.drpOrderWareHouse.SelectedValue.ToString();
                    row["U_AB_Urgent"] = this.chkUrgent.Checked ? "Y" : "N";
                    row["U_AB_Time"] = DateTime.Now.ToString("hh : mm tt");
                    row["U_AE_IFlag"] = "Y";
                    if (this.hdnIsUpdate.Value == "0")
                    {
                        row["U_AB_SentSupplier"] = "N";
                    }
                    //Save As Draft
                    if (isDraft)
                    {
                        tblHeader.Columns.Add("ObjType");
                        row["ObjType"] = "22";
                        tblHeader.TableName = "ODRF";
                    }

                    tblHeader.Rows.Add(row);

                    DataTable tblItem = new DataTable();
                    if (!isDraft)
                    {
                        tblItem.TableName = "POR1";
                    }
                    else
                    {
                        tblItem.TableName = "DRF1";
                    }
                    tblItem.Columns.Add("ItemCode");
                    tblItem.Columns.Add("Dscription");
                    tblItem.Columns.Add("UnitMsr");
                    tblItem.Columns.Add("Quantity");
                    tblItem.Columns.Add("PriceBefDi");
                    tblItem.Columns.Add("LineTotal");
                    tblItem.Columns.Add("Vatgroup");
                    tblItem.Columns.Add("ShipType");
                    tblItem.Columns.Add("TrnspName");
                    tblItem.Columns.Add("WhsCode");
                    tblItem.Columns.Add("U_AB_POQty");

                    tblItem.Columns.Add("OcrCode");
                    //New Request 24/6/2014
                    tblItem.Columns.Add("CogsOcrCod");

                    if (this.hdnIsUpdate.Value == "1")
                    {
                        tblItem.Columns.Add("LineNum");
                    }
                    int visOrder = 0;
                    foreach (DataRow r in tb.Rows)
                    {
                        DataRow rowNew = tblItem.NewRow();
                        rowNew["ItemCode"] = r["ItemCode"];
                          //Save As Draft
                        if (isDraft)
                        {
                            if (double.Parse(r["Quantity"].ToString()) == 0)
                            {
                                rowNew["Quantity"] = "0.0001";
                            }
                            else
                            {
                                rowNew["Quantity"] = r["Quantity"];
                            }
                            rowNew["LineTotal"] = decimal.Parse(rowNew["Quantity"].ToString()) * decimal.Parse(r["Price"].ToString());
                            rowNew["PriceBefDi"] = r["Price"];
                        }
                        else
                        {
                            rowNew["Quantity"] = r["Quantity"];
                            rowNew["LineTotal"] = r["LineTotal"];
                        }
                        rowNew["Dscription"] = r["Dscription"];
                        rowNew["UnitMsr"] = r["BuyUnitMsr"];
                        //rowNew["Price"] = r["Price"];
                        rowNew["PriceBefDi"] = r["SAPPrice"];
                        rowNew["ShipType"] = r["ShipType"];
                        rowNew["TrnspName"] = r["TrnspName"];
                        rowNew["U_AB_POQty"] = r["Quantity"];
                        rowNew["Vatgroup"] = r["VatgroupPu"];
                        rowNew["WhsCode"] = this.drpOrderWareHouse.SelectedValue.ToString();

                        rowNew["OcrCode"] = this.drpOrderWareHouse.SelectedValue.ToString();
                        //New Request 24/6/2014
                        rowNew["CogsOcrCod"] = this.drpOrderWareHouse.SelectedValue.ToString();
                        if (this.hdnIsUpdate.Value == "1")
                        {
                            rowNew["LineNum"] = visOrder;
                        }
                        //rowNew["DisCPrcnt"] = 0;
                        if (!isDraft)
                        {
                            if (double.Parse(r["Quantity"].ToString()) != 0)
                            {
                                tblItem.Rows.Add(rowNew);
                            }
                        }
                        else
                        {
                            tblItem.Rows.Add(rowNew);
                        }
                        visOrder++;
                    }

                    ds.Tables.Add(tblHeader);
                    ds.Tables.Add(tblItem);
                    MasterData obj = new MasterData();
                    string errMsg = string.Empty;
                    if (!isDraft)
                    {
                        if (this.hdnIsUpdate.Value == "0" || _isPO == "0")
                        {
                            errMsg = obj.CheckAddPO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                               _companyCode, ds, Utils.AppConstants.ConvertToDate(this.txtDelDate.Text.Trim()).Date, false, string.Empty);
                        }
                        else
                        {
                            errMsg = obj.CheckAddPO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                          _companyCode, ds, Utils.AppConstants.ConvertToDate(this.txtDelDate.Text.Trim()).Date, true, _docEntry);
                        }
                        if (errMsg.Length > 0)
                        {
                            if (errMsg.Trim() == "The device is not ready.")
                            {
                                errMsg = "System is processing another request. Pls try again later.";
                            }
                            this.lblError.Text = errMsg;
                            this.lblError.Visible = true;
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

                            DataRow rAddress = FillShipToAddress(this.drpOrderWareHouse.SelectedValue.ToString(), false);
                            if (rAddress != null)
                            {
                                DataRow rNew = tbAddress.NewRow();
                                rNew["StreetS"] = rAddress["Street"];
                                rNew["BlockS"] = rAddress["Block"];
                                rNew["CountryS"] = rAddress["Country"];
                                rNew["CityS"] = rAddress["City"];
                                rNew["ZipCodes"] = rAddress["ZipCode"];
                                tbAddress.Rows.Add(rNew);
                                ds.Tables.Add(tbAddress);
                            }
                            return ds;
                        }
                    }
                    else
                    {
                        return ds;
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvPO_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow myRow = e.Row;
                Label lblNo = myRow.FindControl("lblNo") as Label;
                lblNo.Text = (myRow.RowIndex + 1).ToString();
                if (this.hdnStatus.Value == "C")
                {
                    TextBox txtQuantity = myRow.FindControl("txtOrderQuantity") as TextBox;
                    txtQuantity.Enabled = false;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtPODate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //Check Valid Date
                Utils.AppConstants.ConvertToDate(this.txtPODate.Text.Trim());
                this.btnShowPopup.Enabled = true;
            }
            catch
            {
                this.btnShowPopup.Enabled = false;
            }
            UpdatePrice();
        }
        /// <summary>
        /// 
        /// </summary>
        private void ResetPage()
        {
            this.txtVendorCode.Text = "";
            this.txtVendorName.Text = "";
            this.txtRemarks.Text = "";
            this.lblMaxAmount.Text = "";
            this.lblMinAmount.Text = "";
            this.lblDocumentTotal.Text = "0.000";
            this.lblGSTAmount.Text = "0.000";
            this.lblSubTotal.Text = "0.000";
            this.Session["ChosenItem"] = null;
            this.Session["ChosenVendorCode"] = null;
            this.Session["ChosenVendorName"] = null;
            Session["SODATA"] = null;
            LoadOrderWareHouse();
            LoadData();
            this.btnSaveDraft.Enabled = false;
            this.btnUpdate.Enabled = false;
            if (this.chkCalendar.Items.Count > 0)
            {
                for (int i = 0; i < this.chkCalendar.Items.Count; i++)
                {
                    this.chkCalendar.Items[i].Selected = false;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void drpOrderWareHouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FillShipToAddress(this.drpOrderWareHouse.SelectedValue.ToString(), true);
                if (this.txtVendorCode.Text.Trim().Length > 0)
                {
                    DateTime poDate = Utils.AppConstants.ConvertToDate(this.txtPODate.Text.Trim());
                    DataTable tblIPO = (DataTable)Session["POTable"];
                    if (tblIPO != null)
                    {
                        tblIPO.Rows.Clear();
                        LoadSetupItem(poDate, tblIPO);
                        LoadSupplierAmount();
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
        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                CheckDate();
                this.lblError.Visible = false;
                string requestXML = CollectData(true);
                if (requestXML == "") return;
                Transaction ts = new Transaction();
                DataSet ds = new DataSet();
                //Create New Draft
                if (this.hdnIsUpdate.Value == "0")
                {
                    ds = ts.CreateMarketingDocument(requestXML, Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                       _companyCode, "112", "", false);
                }
                //Update Draft
                else if (this.hdnIsUpdate.Value == "1")
                {
                    ds = ts.CreateMarketingDocument(requestXML, Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                  _companyCode, "112", _docEntry, true);
                }
                if ((int)ds.Tables[0].Rows[0]["ErrCode"] != 0)
                {
                    this.lblError.Text = ds.Tables[0].Rows[0]["ErrMsg"].ToString();
                    Utils.AppConstants.WriteLog(ds.Tables[0].Rows[0]["ErrMsg"].ToString(), true);
                    Utils.AppConstants.WriteLog(requestXML, true);
                    Utils.AppConstants.WriteLog(Session[Utils.AppConstants.UserCode].ToString(), true);
                    Utils.AppConstants.WriteLog(Session[Utils.AppConstants.CompanyCode].ToString(), true);
                }
                else
                {
                    this.lblError.Text = "Operation complete successful!";
                    if (this.hdnIsUpdate.Value == "0")
                    {
                        ResetPage();
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            CloseCancel(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClose_Click(object sender, EventArgs e)
        {
            CloseCancel(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isClose"></param>
        private void CloseCancel(bool isClose)
        {
            try
            {
                Transaction obj = new Transaction();
                string error = string.Empty;
                if (_isPO == "1")
                {
                    error = obj.CloseCancelPO(int.Parse(_docEntry), _companyCode, isClose, false);
                }
                else
                {
                    error = obj.CloseCancelPO(int.Parse(_docEntry), _companyCode, isClose, true);
                }
                if (error.Length == 0)
                {
                    this.lblError.Text = "Operation complete successful!";
                    DisableControl();
                }
                else
                {
                    this.lblError.Text = error;
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
            }
            this.lblError.Visible = true;
        }
    }
}