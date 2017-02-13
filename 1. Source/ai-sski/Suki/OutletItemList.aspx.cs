using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class OutletItemList : System.Web.UI.Page
    {
        string _companyCode = string.Empty;
        string _vendorCode = string.Empty;
        string _isNew = string.Empty;
        string _vendorName = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
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
                if (System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString() != Session[Suki.Utils.AppConstants.CompanyCode].ToString())
                {
                    Response.Redirect("Default.aspx");
                }
                _companyCode = Request.QueryString["CompanyCode"];
                if (Request.QueryString["IsNew"] != null && Request.QueryString["IsNew"].ToUpper() == "E")
                {
                    _isNew = Request.QueryString["IsNew"];
                    _vendorCode = Request.QueryString["VendorCode"];
                    _vendorName = Request.QueryString["VendorName"];
                }
                if (!IsPostBack)
                {
                    //Clear Session
                    Session["ChosenItem"] = null;
                    for (int i = 0; i < Session.Count; i++)
                    {
                        var numSession = Session.Keys[i];
                        if (numSession != Utils.AppConstants.CompanyCode
                            && numSession != Utils.AppConstants.CompanyName
                              && numSession != Utils.AppConstants.IsSupperUser
                              && numSession != Utils.AppConstants.OutletCode
                              && numSession != Utils.AppConstants.OutletName
                            && numSession != Utils.AppConstants.Pwd
                            && numSession != Utils.AppConstants.UserCode
                                 && numSession != Utils.AppConstants.IsCompanySuperUser
                             && numSession != Utils.AppConstants.ListCompany
                             && numSession != Utils.AppConstants.ListOutlet
                            && numSession != "PageIndex")
                        {
                            Session[numSession] = null;
                        }
                    }
                    LoadCompany();
                    LoadItem();
                    if (_isNew == "E")
                    {
                        this.drdOutlet.SelectedValue = _companyCode;
                        this.txtVendorCode.Text = _vendorCode;
                        this.txtVendorName.Text = _vendorName;
                        this.drdOutlet.Enabled = false;
                        this.txtVendorCode.Enabled = false;
                        this.txtVendorName.Enabled = false;
                        this.btnSelectVendor.Visible = false;
                        this.btnAddNewItem.Enabled = true;
                        this.btnCalendar.Enabled = true;
                        this.btnOrderAmount.Enabled = true;
                        this.btnSetupNew.Visible = true;

                        LoadOutletSupplier(this.txtVendorCode.Text, this.drdOutlet.SelectedValue.ToString());
                    }
                    else
                    {
                        this.btnSetupNew.Visible = false;
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
        protected void grvSearchResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.grvItem.PageIndex = e.NewPageIndex;
            DataTable tb = (DataTable) Session["OuletItemList"];
            BindItem(tb);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkOutlet_Click(object sender, EventArgs e)
        {
            try
            {
                GridViewRow gr = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                Label outletCode = (Label)gr.FindControl("lblItemCode");
                string popup = "OpenOutlet('" + outletCode.Text + "')";
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
        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            try
            {
                GridViewRow gr = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                Label lblItemCode = (Label)gr.FindControl("lblItemCode");
                Label lblID = (Label)gr.FindControl("lblID");
                DataTable tb = (DataTable)Session["OuletItemList"];
                if (tb != null)
                {
                    DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                    if (rupdate.Length > 0)
                    {
                        Dictionary<string, string> dicItem = (Dictionary<string, string>)Session["ChosenItem"];
                        Dictionary<string, string> dicRemove = (Dictionary<string, string>)Session["RemoveItem"];
                        if (dicItem != null)
                        {
                            dicItem.Remove(rupdate[0]["ItemCode"].ToString());
                            if (dicRemove == null)
                            {
                                dicRemove = new Dictionary<string, string>();
                            }
                            dicRemove.Add(lblID.Text, rupdate[0]["ItemCode"].ToString());
                        }
                        Session["RemoveItem"] = dicRemove;
                        tb.Rows.Remove(rupdate[0]);
                    }
                    BindItem(tb);
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
        /// <param name="cardCode"></param>
        /// <param name="companyCode"></param>
        private void LoadOutletSupplier(string cardCode, string companyCode)
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet ds = obj.GetOutletSupplier(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), cardCode,companyCode, Session[Utils.AppConstants.CompanyCode].ToString());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Dictionary<string, string> dicItem = new Dictionary<string, string>();

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        if (!dicItem.ContainsKey(r["ItemCode"].ToString()))
                        {
                            dicItem.Add(r["ItemCode"].ToString(), r["Dscription"].ToString());
                            Session[this.txtVendorCode.Text + "_" + r["ItemCode"].ToString() + "_OutletData"] = null;
                        }
                    }
                    Session["ChosenItem"] = dicItem;
                    this.btnSave.Enabled = true;
                    BindItem(ds.Tables[0]);
                    this.hdnisUpdate.Value = "1";
                }
                else
                {
                    LoadItem();
                    this.hdnisUpdate.Value = "0";
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
        private void LoadItem()
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("ID");
            tb.Columns.Add("ItemCode");
            tb.Columns.Add("Dscription");
            tb.Columns.Add("InvntryUom");
            tb.Columns.Add("BuyUnitMsr");
            tb.Columns.Add("LB");
            BindItem(tb);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb"></param>
        private void BindItem(DataTable tb)
        {
            Session["OuletItemList"] = tb;
            this.grvItem.DataSource = tb;
            this.grvItem.DataBind();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EditItem(object sender, GridViewEditEventArgs e)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelEdit(object sender, GridViewCancelEditEventArgs e)
        {
            e.Cancel = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DeleteItem(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UpdateItem(object sender, GridViewUpdateEventArgs e)
        {
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
                if (ds != null && ds.Tables.Count>0)
                {
                    if (Session[Utils.AppConstants.ListCompany] != null)
                    {
                        Dictionary<string, string> ownCompany = (Dictionary<string, string>)Session[Utils.AppConstants.ListCompany];
                        if (ownCompany.Count == 0)
                        {
                            this.drdOutlet.DataSource = ds.Tables[0];
                            this.drdOutlet.DataTextField = "U_CompanyName";
                            this.drdOutlet.DataValueField = "U_DBName";
                            this.drdOutlet.DataBind();
                            if (_companyCode != null && _companyCode.Length > 0)
                            {
                                if (this.drdOutlet.Items.Count > 0)
                                {
                                    this.drdOutlet.SelectedValue = _companyCode;
                                }
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<string, string> item in ownCompany)
                            {
                                DataRow[] r = ds.Tables[0].Select("U_DBName='" + item.Value + "'");
                                if (r.Length > 0)
                                {
                                    this.drdOutlet.Items.Add(new ListItem(r[0]["U_CompanyName"].ToString(), r[0]["U_DBName"].ToString()));
                                }
                                if (_companyCode != null && _companyCode.Length > 0)
                                {
                                    if (this.drdOutlet.Items.Count > 0)
                                    {
                                        this.drdOutlet.SelectedValue = _companyCode;
                                    }
                                }
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
        /// <param name="e"></param>
        protected override void OnLoadComplete(EventArgs e)
        {
            try
            {
                base.OnLoadComplete(e);
                if (this.Request["__EVENTARGUMENT"] != null && this.Request["__EVENTARGUMENT"].ToString() != "")
                {
                    this.lblError.Visible = false;
                    DataTable tblIPO = (DataTable)Session["OuletItemList"];
                    switch (this.Request["__EVENTARGUMENT"].ToString())
                    {
                        case "SelectVendor":
                            Session["ChosenItem"] = null;
                            this.txtVendorCode.Text = Session["ChosenVendorCode"] != null ? Session["ChosenVendorCode"].ToString() : "";
                            this.txtVendorName.Text = Session["ChosenVendorName"] != null ? Session["ChosenVendorName"].ToString() : "";
                            this.btnAddNewItem.Enabled = true;
                            this.btnCalendar.Enabled = true;
                            this.btnOrderAmount.Enabled = true;
                            Session["ChosenItem"] = null;
                            if (this.txtVendorCode.Text.Length > 0 && Session["OuletItemList"] != null && ((DataTable)Session["OuletItemList"]).Rows.Count > 0)
                            {
                                this.btnSave.Enabled = true;
                            }
                            LoadOutletSupplier(this.txtVendorCode.Text, this.drdOutlet.SelectedValue.ToString());
                            break;
                        case "SelectItem":
                            if (tblIPO != null)
                            {
                                Dictionary<string, string> dicItem = Session["ChosenItem"] != null ? (Dictionary<string, string>)Session["ChosenItem"] : null;
                                if (dicItem != null)
                                {
                                    foreach (KeyValuePair<string, string> item in dicItem)
                                    {
                                        DataRow[] existRow = tblIPO.Select("ItemCode = '" + item.Key + "'");
                                        if (existRow.Length == 0)
                                        {
                                            Session[this.txtVendorCode.Text + "_" + item.Key + " _OutletData"] = null;
                                            MasterData obj = new MasterData();
                                            DataRow rowNew = tblIPO.NewRow();
                                            rowNew["ID"] = "0";
                                            rowNew["ItemCode"] = item.Key;
                                            rowNew["Dscription"] = item.Value;
                                            DataSet ds = obj.GetItemOutletSetup(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                             this.drdOutlet.SelectedValue.ToString(), item.Key);
                                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                            {
                                                rowNew["InvntryUom"] = ds.Tables[0].Rows[0]["InvntryUom"];
                                                rowNew["BuyUnitMsr"] = ds.Tables[0].Rows[0]["BuyUnitMsr"];
                                                rowNew["LB"] = ds.Tables[0].Rows[0]["LB"];
                                            }
                                            tblIPO.Rows.Add(rowNew);
                                        }
                                    }
                                }
                                if (this.txtVendorCode.Text.Length > 0 && Session["OuletItemList"] != null && ((DataTable)Session["OuletItemList"]).Rows.Count > 0)
                                {
                                    this.btnSave.Enabled = true;
                                }
                                //ReLoad Data
                                BindItem(tblIPO);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Visible = true;
                this.lblError.Text = ex.Message;
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
                this.btnAddNewItem.Enabled = false;
                this.btnCalendar.Enabled = false;
                this.btnOrderAmount.Enabled = false;
                if (this.txtVendorCode.Text.Length == 0 || ((DataTable)Session["OuletItemList"]).Rows.Count == 0)
                {
                    this.btnSave.Enabled = false;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtLB_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = ((DataTable)Session["OuletItemList"]);
                if (tb != null)
                {
                    GridViewRow gr = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
                    Label lblItemCode = (Label)gr.FindControl("lblItemCode");
                    TextBox txtData = (TextBox)gr.FindControl("txtLB");
                    DataRow[] rowNew = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                    if (rowNew.Length > 0)
                    {
                        rowNew[0]["LB"] = txtData.Text.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Visible = true;
                this.lblError.Text = ex.Message;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.lblError.Visible = false;
                MasterData obj = new MasterData();
                DataTable tbMaster = (DataTable)Session["OuletItemList"];
                if (tbMaster != null)
                {
                    tbMaster.TableName = "Item_Supplier";
                    DataSet ds = new DataSet();
                    foreach (DataRow r in tbMaster.Rows)
                    {
                        DataTable tbTemp = (DataTable)Session[this.txtVendorCode.Text + "_" + r["ItemCode"].ToString() + "_OutletData"];
                        if (tbTemp != null)
                        {
                            tbTemp.TableName = r["ItemCode"].ToString();
                            DataTable tbDetail = tbTemp.Clone();
                            foreach (DataRow rDetail in tbTemp.Rows)
                            {
                                tbDetail.ImportRow(rDetail);
                            }
                            if (!ds.Tables.Contains(tbDetail.TableName))
                            {
                                ds.Tables.Add(tbDetail);
                            }
                        }
                    }
                    DataTable tbDelete = new DataTable();
                    tbDelete.TableName = "Delete";
                    tbDelete.Columns.Add("Delete");
                    tbDelete.Columns.Add("ItemCode");
                    Dictionary<string, string> dicRemove = (Dictionary<string, string>)Session["RemoveItem"];
                    if (dicRemove != null)
                    {
                        foreach (KeyValuePair<string, string> item in dicRemove)
                        {
                            DataRow rDelete = tbDelete.NewRow();
                            rDelete["Delete"] = item.Key;
                            rDelete["ItemCode"] = item.Value;
                            tbDelete.Rows.Add(rDelete);
                        }

                        ds.Tables.Add(tbDelete);
                    }
                    ds.Tables.Add(tbMaster.Copy());
                    string errMsg = obj.UpdateOutletItemList(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), 0, this.txtVendorCode.Text.Trim(),
                         this.txtVendorName.Text.Trim(), this.drdOutlet.SelectedValue.ToString(), this.drdOutlet.SelectedItem.Text, ds, "0", Session[Utils.AppConstants.CompanyCode].ToString());
                    if (errMsg.Length > 0)
                    {
                        this.lblError.Text = errMsg;
                    }
                    else
                    {
                        Session["RemoveItem"] = null;
                        this.lblError.Text = "Operation complete successful!";
                        LoadOutletSupplier(this.txtVendorCode.Text, this.drdOutlet.SelectedValue.ToString());
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
        protected void drdOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["ChosenItem"] = null;
            LoadOutletSupplier(this.txtVendorCode.Text, this.drdOutlet.SelectedValue.ToString());
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("OutletItemList.aspx");
            return;
            Session["ChosenItem"] = null;
            this.drdOutlet.Enabled = true;
            this.txtVendorCode.Enabled = false;
            this.txtVendorName.Enabled = true;
            this.btnSelectVendor.Visible = true;
            this.btnAddNewItem.Enabled = false;
            this.btnCalendar.Enabled = false;
            this.btnOrderAmount.Enabled = false;
            this.btnSetupNew.Visible = false;
            this.txtVendorCode.Text = "";
            this.txtVendorName.Text = "";
            this.drdOutlet.SelectedIndex = 0;
            this.btnSave.Enabled = false;
            LoadOutletSupplier(this.txtVendorCode.Text, this.drdOutlet.SelectedValue.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("CompanySetupList.aspx?Company=" + _companyCode);
        }
    }
}