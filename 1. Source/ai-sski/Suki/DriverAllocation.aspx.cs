using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using System.Text;

namespace Suki
{
    public partial class DriverAllocation : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.txtDelDate.Text = DateTime.Now.Date.ToShortDateString();
                this.txtPODate.Text = DateTime.Now.Date.ToShortDateString();
                Session["ChosenItem"] = null;
                Session["OrderWareHouse"] = null;
                LoadOrderWareHouse();
                LoadData();
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
                GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
                TextBox txtQuantity = (TextBox)row.FindControl("txtOrderQuantity");
                if (txtQuantity.Text.Trim().Length == 0 || int.Parse(txtQuantity.Text.Trim()) == 0)
                {
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert('In Quantity column, enter whole number greater than 0');", true);
                    txtQuantity.Focus();
                    return;
                }
                Label lblItemCode = (Label)row.FindControl("lblItemCode");

                DataTable tb = (DataTable)ViewState["POTable"];
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
            catch (Exception ex)
            {
                string alert = "alert('Error: " + ex.Message + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
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
                DataSet ds = obj.GetABOutletByCompany(Session[Utils.AppConstants.CompanyCode].ToString());
                if (ds != null && ds.Tables.Count > 0)
                {
                    Session["OrderWareHouse"] = ds.Tables[0];
                    if (Session[Utils.AppConstants.IsSupperUser] != null && Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "N")
                    {
                        DataRow[] rOutlet = ds.Tables[0].Select("U_WhseCode='" + Session[Utils.AppConstants.OutletCode] + "'");
                        if (rOutlet.Length > 0)
                        {
                            this.drpOrderWareHouse.Items.Add(new ListItem(rOutlet[0]["U_WhseName"].ToString(), rOutlet[0]["U_WhseCode"].ToString()));
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
                string alert = "alert('Error: " + ex.Message + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
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
                    DataTable tbWareHouse = (DataTable)Session["OrderWareHouse"];
                    if (tbWareHouse != null)
                    {
                        DataRow[] r = tbWareHouse.Select("U_WhseCode = " + outletId);
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
                string alert = "alert('Error: " + ex.Message + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
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
            BindData((DataTable)ViewState["POTable"]);
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
            BindData((DataTable)ViewState["POTable"]);
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
                DataTable tb = (DataTable)ViewState["POTable"];
                DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                if (rupdate.Length > 0)
                {
                    Dictionary<string, string> dicItem = (Dictionary<string, string>)Session["ChosenItem"];
                    if (dicItem != null)
                    {
                        dicItem.Remove(rupdate[0]["ItemCode"].ToString());
                    }
                    tb.Rows.Remove(rupdate[0]);
                }
                this.grvPO.EditIndex = -1;
                BindData((DataTable)ViewState["POTable"]);
                CalcTotal(((DataTable)ViewState["POTable"]));
            }
            catch (Exception ex)
            {
                string alert = "alert('Error: " + ex.Message + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
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
                GridViewRow row = (GridViewRow)this.grvPO.Rows[e.RowIndex];
                TextBox txtQuantity = (TextBox)row.FindControl("txtOrderQuantity");
                if (txtQuantity.Text.Trim().Length == 0 || int.Parse(txtQuantity.Text.Trim()) == 0)
                {
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert('In Quantity column, enter whole number greater than 0');", true);
                    txtQuantity.Focus();
                    return;
                }
                Label lblItemCode = (Label)row.FindControl("lblItemCode");

                DataTable tb = (DataTable)ViewState["POTable"];
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
            catch (Exception ex)
            {
                string alert = "alert('Error: " + ex.Message + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadData()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Dscription");
            tbTemp.Columns.Add("MaxStock");
            tbTemp.Columns.Add("BuyUnitMsr");
            tbTemp.Columns.Add("Quantity");
            tbTemp.Columns.Add("Price");
            tbTemp.Columns.Add("LineTotal");
            BindData(tbTemp);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tblData"></param>
        private void BindData(DataTable tblData)
        {
            ViewState["POTable"] = tblData;
            this.grvPO.DataSource = tblData;
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
                    DateTime poDate = Convert.ToDateTime(this.txtPODate.Text.Trim());
                    DataTable tblIPO = (DataTable)ViewState["POTable"];
                    switch (this.Request["__EVENTARGUMENT"].ToString())
                    {
                        case "SelectVendor":
                            this.lblError.Visible = false;
                            this.txtVendorCode.Text = Session["ChosenVendorCode"] != null ? Session["ChosenVendorCode"].ToString() : "";
                            this.txtVendorName.Text = Session["ChosenVendorName"] != null ? Session["ChosenVendorName"].ToString() : "";
                            tblIPO.Rows.Clear();
                            Session["ChosenItem"] = null;
                            tblIPO = LoadSetupItem(poDate, tblIPO);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                string alert = "alert('Error: " + ex.Message + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
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
            MasterData obj = new MasterData();
            DataSet dsItem = obj.GetSupplierItemSetup(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                Session[Utils.AppConstants.CompanyCode].ToString(), this.txtVendorCode.Text, this.drpOrderWareHouse.SelectedValue.ToString(),DateTime.Now);
            if (dsItem != null && dsItem.Tables.Count > 0)
            {
                foreach (DataRow r in dsItem.Tables[0].Rows)
                {
                    tblIPO = AddNewItem(tblIPO, poDate, r["ItemCode"].ToString(), r["ItemName"].ToString(), r["LB"].ToString(), this.txtVendorCode.Text, 0);
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
            this.btnUpdate.Enabled = true;
            return tblIPO;
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdatePrice()
        {
            try
            {
                //Update Price
                foreach (DataRow r in ((DataTable)ViewState["POTable"]).Rows)
                {
                    GetPrice(r);
                }
                BindData((DataTable)ViewState["POTable"]);
            }
            catch (Exception ex)
            {
                string alert = "alert('Error: " + ex.Message + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
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
                DataSet ds = GetDataItem(Convert.ToDateTime(this.txtPODate.Text.Trim()), r["ItemCode"].ToString(), this.txtVendorCode.Text, double.Parse(r["Quantity"].ToString()));
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    r["Price"] = ds.Tables[0].Rows[0]["Price"];
                    r["LineTotal"] = double.Parse(ds.Tables[0].Rows[0]["Price"].ToString()) * double.Parse(r["Quantity"].ToString());
                }
            }
            catch (Exception ex)
            {
                string alert = "alert('Error: " + ex.Message + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tblIPO"></param>
        /// <param name="poDate"></param>
        /// <param name="itemCode"></param>
        /// <param name="itemName"></param>
        /// <param name="lb"></param>
        /// <param name="venderCode"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private DataTable AddNewItem(DataTable tblIPO, DateTime poDate, string itemCode, string itemName, string lb, string venderCode, double quantity)
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
                        rowNew["BuyUnitMsr"] = row["BuyUnitMsr"];
                        rowNew["Quantity"] = quantity;
                        rowNew["Price"] = row["Price"];
                        rowNew["LineTotal"] = row["LineTotal"];
                        tblIPO.Rows.Add(rowNew);
                    }
                }
            }
            catch (Exception ex)
            {
                string alert = "alert('Error: " + ex.Message + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
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
            }
            catch (Exception ex)
            {
                string alert = "alert('Error: " + ex.Message + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
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
                DataSet ds = obj.GetItem(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                         Session[Utils.AppConstants.CompanyCode].ToString(), poDate, itemCode, venderCode, quantity);
                return ds;
            }
            catch (Exception ex)
            {
                string alert = "alert('Error: " + ex.Message + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
            }
            return null;
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
                this.lblError.Visible = false;
                string requestXML = CollectData(false);
                if (requestXML == "") return;
                Transaction ts = new Transaction();
                DataSet ds = ts.CreateMarketingDocument(requestXML, Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                     Session[Utils.AppConstants.CompanyCode].ToString(), "22", "", false);
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
                    ResetPage();
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
            try
            {
                DataTable tb = ((DataTable)ViewState["POTable"]);
                if (tb != null && tb.Rows.Count > 0)
                {
                    DocumentXML objInfo = new DocumentXML();
                    DataSet ds = new DataSet("DS");

                    DataTable tblHeader = new DataTable("ORDR");
                    tblHeader.Columns.Add("CardCode");
                    tblHeader.Columns.Add("CardName");
                    tblHeader.Columns.Add("DocDate");
                    tblHeader.Columns.Add("DocDueDate");
                    tblHeader.Columns.Add("NumAtCard");
                    tblHeader.Columns.Add("Address");
                    tblHeader.Columns.Add("AB_DriverNo");
                    tblHeader.Columns.Add("AB_Urgent");

                    DataRow row = tblHeader.NewRow();
                    row["CardCode"] = this.txtVendorCode.Text;
                    row["CardName"] = this.txtVendorName.Text;
                    row["DocDate"] = Convert.ToDateTime(this.txtPODate.Text.Trim()).ToString("yyyyMMdd");
                    row["DocDueDate"] = Convert.ToDateTime(this.txtDelDate.Text.Trim()).ToString("yyyyMMdd");
                    row["NumAtCard"] = this.txtPONo.Text;
                    row["Address"] = this.txtShipTo.Text;
                    row["AB_DriverNo"] = this.drpDriverNo.SelectedValue.ToString();
                    row["AB_Urgent"] = this.chkUrent.Checked;

                    tblHeader.Rows.Add(row);

                    DataTable tblItem = new DataTable("RDR1");
                    tblItem.Columns.Add("ItemCode");
                    tblItem.Columns.Add("Dscription");
                    tblItem.Columns.Add("UnitMsr");
                    tblItem.Columns.Add("Quantity");
                    tblItem.Columns.Add("Price");
                    tblItem.Columns.Add("LineTotal");

                    foreach (DataRow r in tb.Rows)
                    {
                        DataRow rowNew = tblItem.NewRow();
                        rowNew["ItemCode"] = r["ItemCode"];
                        rowNew["Quantity"] = r["Quantity"];
                        rowNew["Dscription"] = r["Dscription"];
                        rowNew["LineTotal"] = r["LineTotal"];
                        rowNew["UnitMsr"] = r["Quantity"];
                        rowNew["Price"] = r["Price"];
                        tblItem.Rows.Add(rowNew);
                    }

                    ds.Tables.Add(tblHeader);
                    ds.Tables.Add(tblItem);
                    return objInfo.ToXMLStringFromDS("22", ds);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
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
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ResetPage()
        {
            this.txtVendorCode.Text = "";
            this.txtVendorName.Text = "";
            this.Session["ChosenItem"] = null;
            this.Session["ChosenVendorCode"] = null;
            this.Session["ChosenVendorName"] = null;
            Session["SODATA"] = null;
            LoadOrderWareHouse();
            LoadData();
            this.btnUpdate.Enabled = false;
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
                    DateTime poDate = Convert.ToDateTime(this.txtPODate.Text.Trim());
                    DataTable tblIPO = (DataTable)ViewState["POTable"];
                    tblIPO.Rows.Clear();
                    LoadSetupItem(poDate, tblIPO);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}