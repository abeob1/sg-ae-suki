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
    public partial class DriverAllocationSetting : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.txtBlock.Enabled = false;
            if (!IsPostBack)
            {
                LoadBlock();
                LoadData();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadBlock()
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet ds = obj.GetRouteHolding();
                if (ds != null && ds.Tables.Count > 0)
                {
                    this.dllBlock.DataSource = ds.Tables[0];
                    this.dllBlock.DataTextField = "Block";
                    this.dllBlock.DataValueField = "Address";
                    this.DataBind();
                    if (this.dllBlock.Items.Count > 0)
                    {
                        this.dllBlock.SelectedIndex = 0;
                        this.txtBlock.Text = this.dllBlock.SelectedItem.Text;
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
            tbTemp.Columns.Add("AB_TruckCat");
            tbTemp.Columns.Add("DriverNo");
            tbTemp.Columns.Add("VehicleNo");
            DataRow r = tbTemp.NewRow();
            tbTemp.Rows.Add();
            BindData(tbTemp);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tblData"></param>
        private void BindData(DataTable tblData)
        {
            Session["DriverAllocationTable"] = tblData;
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
        private void UpdatePrice()
        {
            try
            {
                //Update Price
                foreach (DataRow r in ((DataTable)ViewState["POTable"]).Rows)
                {
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

                DropDownList ddl = (DropDownList)myRow.FindControl("dllTrunkCategory");
                LoadTruckCategory(ddl);
                //ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(DataBinder.Eval(e.Row.DataItem, "Shiptype").ToString()));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void ResetPage()
        {
            this.Session["ChosenItem"] = null;
            this.Session["ChosenVendorCode"] = null;
            this.Session["ChosenVendorName"] = null;
            Session["SODATA"] = null;
            LoadBlock();
            LoadData();
            this.btnUpdate.Enabled = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dll"></param>
        private void LoadTruckCategory(DropDownList dll)
        {
            DataSet ds = new DataSet();
            DataTable tb = new DataTable();
            tb.Columns.Add("AB_TruckCat");
            tb.Columns.Add("ShipTypeName");
            DataRow r = tb.NewRow();
            r["AB_TruckCat"] = 3;
            r["ShiptypeName"] = "Dry";
            tb.Rows.Add(r);

            r = tb.NewRow();
            r["AB_TruckCat"] = 4;
            r["ShiptypeName"] = "Frozen";
            tb.Rows.Add(r);
            ds.Tables.Add(tb);
            dll.DataSource = ds;
            dll.DataTextField = "ShiptypeName";
            dll.DataValueField = "AB_TruckCat";
            dll.DataBind();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void dllBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtBlock.Text = this.dllBlock.SelectedItem.Text;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button1_Click(object sender, EventArgs e)
        {
            DataTable tbTemp = (DataTable)Session["DriverAllocationTable"];
            if (tbTemp != null)
            {
                tbTemp.Clear();
                for (int i = 0; i < this.grvPO.Rows.Count; i++)
                {
                    DropDownList dllTruck = (DropDownList)this.grvPO.Rows[i].FindControl("dllTrunkCategory");
                    Label lblNo = (Label)this.grvPO.Rows[i].FindControl("lblNo");
                    TextBox txtDriverNo = (TextBox)this.grvPO.Rows[i].FindControl("DriverNo");
                    TextBox txtVehicelNo = (TextBox)this.grvPO.Rows[i].FindControl("VehicleNo");
                    DataRow rnew = tbTemp.NewRow();

                    rnew["DriverNo"] = txtDriverNo.Text;
                    rnew["VehicleNo"] = txtVehicelNo.Text;
                    rnew["AB_TruckCat"] = dllTruck.SelectedValue.ToString();

                    tbTemp.Rows.Add(rnew);
                }
                DataRow r = tbTemp.NewRow();
                tbTemp.Rows.Add();
                BindData(tbTemp);
            }
        }
    }
}