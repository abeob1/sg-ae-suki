using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class ClosingStock : System.Web.UI.Page
    {
        private string _docEntry = string.Empty;
        private string _stockNo = string.Empty;
        private string _docTotal = string.Empty;
        private string _comments = string.Empty;
        private string _outlet = string.Empty;
        private string _dateTime = string.Empty;
        private string _isDraft = string.Empty;
        GridView grvClosingStock;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.txtStockCheckBy.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
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

                grvClosingStock = this.gvParentGrid.FindControl("grvClosingStock") as GridView;
                if (Request.QueryString["Create"] == null)
                {
                    _docEntry = Request.QueryString["DocEntry"];
                    _stockNo = Request.QueryString["StockNo"];
                    _docTotal = Request.QueryString["DocTotal"];
                    _comments = Request.QueryString["Comment"];
                    _outlet = Request.QueryString["Whs"];
                    _dateTime = Request.QueryString["Date"];
                    _isDraft = Request.QueryString["isDraft"];
                    if (_docEntry == null || _docEntry.Length == 0)
                    {
                        Response.Redirect("ErrorPage.aspx");
                    }
                    if (!Utils.AppConstants.isInteger(_docEntry))
                    {
                        Response.Redirect("ErrorPage.aspx");
                    }
                    if (!Utils.AppConstants.isDouble(_docTotal))
                    {
                        Response.Redirect("ErrorPage.aspx");
                    }

                    if (_isDraft != "1")
                    {
                        this.btnUpdate.Visible = false;
                        this.btnSaveAsDraft.Visible = false;
                        if (this.grvClosingStock != null)
                        {
                            this.grvClosingStock.Enabled = false;
                        }
                        this.txtRemarks.Enabled = false;
                        this.txtStockCheckBy.Enabled = false;
                    }
                    else
                    {
                        if (this.grvClosingStock != null)
                        {
                            this.grvClosingStock.Enabled = false;
                        }
                        this.txtRemarks.Enabled = true;
                        this.txtStockCheckBy.Enabled = true;
                    }
                }

                if (!IsPostBack)
                {
                    this.lblStockNo.Text = _stockNo;
                    if (_docTotal.Length > 0 && Utils.AppConstants.isDouble(_docTotal))
                    {
                        this.txtGrandTotal.Text = double.Parse(_docTotal).ToString(Utils.AppConstants.NUMBER_FORMAT_STOCKTAKE);
                    }
                    this.txtRemarks.Text = _comments;

                    Session["ItemStockTake"] = null;
                    if (Session[Utils.AppConstants.UserCode] == null)
                    {
                        Response.Redirect("Login.aspx");
                    }
                    this.gvParentGrid.DataSource = null;
                    this.gvParentGrid.DataBind();
                    this.txtSubmittedBy.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
                    LoadMonth();
                    LoadYear();
                    LoadOutlet();
                    if (this.grvClosingStock != null)
                    {
                        this.grvClosingStock.RowCreated += new GridViewRowEventHandler(grvClosingStock_RowCreated);
                        this.grvClosingStock.DataSource = null;
                        this.grvClosingStock.DataBind();
                    }
                    if (Request.QueryString["Create"] == null)
                    {
                        if (_isDraft != "1")
                        {
                            this.btnUpdate.Visible = false;
                            this.btnDeleteDraft.Visible = false;
                        }
                        if (this.grvClosingStock != null)
                        {
                            this.grvClosingStock.Columns[6].Visible = false;
                            this.grvClosingStock.Columns[7].Visible = false;
                            this.grvClosingStock.Columns[8].Visible = false;
                            this.grvClosingStock.Columns[9].Visible = false;
                        }
                        this.ddlOutlet.Enabled = false;
                        this.ddlMonth.Enabled = false;
                        this.ddlYear.Enabled = false;
                        this.ddlOutlet.SelectedValue = _outlet;
                    }
                    else
                    {
                        this.btnDeleteDraft.Visible = false;
                        if (this.grvClosingStock != null)
                        {
                            this.grvClosingStock.Columns[5].Visible = false;
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
        protected void grvSearchResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
                TextBox ddl1 = e.Row.FindControl("txtReportQty") as TextBox;
                TextBox ddl2 = e.Row.FindControl("txtRemarks") as TextBox;
                ddl1.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + ddl1.ClientID + "').select();");
                ddl1.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
                ddl2.Attributes.Add("onkeydown", "return (event.keyCode!=13);");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            GetData(this.ddlOutlet.SelectedValue.ToString());
            Timer1.Enabled = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="qty"></param>
        /// <param name="columnName"></param>
        private void UpdateQty(string itemCode, string qty, string columnName)
        {
            try
            {
                DataTable tb = (DataTable)Session["ItemStockTake"];
                if (tb != null)
                {
                    DataRow[] r = tb.Select("ItemCode='" + itemCode + "'");
                    if (r.Length > 0)
                    {
                        r[0][columnName] = qty;
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
        /// <param name="itemCode"></param>
        /// <param name="qty"></param>
        /// <param name="columnName"></param>
        /// <param name="index"></param>
        private void UpdateQty(string itemCode, string qty, string columnName, string cardCode)
        {
            try
            {
                DataTable tb = (DataTable)Session["ItemStockTake"];
                if (tb != null)
                {
                    DataRow[] r = tb.Select("ItemCode='" + itemCode + "' AND CardCode = '" + cardCode + "'");
                    if (r.Length > 0)
                    {
                        r[0][columnName] = qty;
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
        private void LoadOutlet()
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet ds = obj.GetABOutletByCompany(Session[Utils.AppConstants.CompanyCode].ToString());
                if (ds != null && ds.Tables.Count > 0)
                {
                    Session["OrderWareHouse"] = ds.Tables[0];
                    if (Session[Utils.AppConstants.IsSupperUser] != null
                        && Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "N"
                        && Session[Utils.AppConstants.IsCompanySuperUser].ToString().ToUpper() == "N")
                    {
                        Dictionary<string, string> dicOutlet = (Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet];
                        foreach (KeyValuePair<string, string> item in dicOutlet)
                        {
                            DataRow[] rOutlet = ds.Tables[0].Select("U_WhseCode='" + item.Key + "'");
                            if (rOutlet.Length > 0)
                            {
                                this.ddlOutlet.Items.Add(new ListItem(rOutlet[0]["U_WhseName"].ToString(), rOutlet[0]["U_WhseCode"].ToString()));
                            }
                        }
                        if (this.ddlOutlet.Items.Count > 0)
                        {
                            this.ddlOutlet.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        this.ddlOutlet.DataSource = ds.Tables[0];
                        this.ddlOutlet.DataTextField = "U_WhseName";
                        this.ddlOutlet.DataValueField = "U_WhseCode";
                        this.ddlOutlet.DataBind();
                        this.ddlOutlet.SelectedIndex = 0;
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
        private DataTable CreateFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("ItemName");
            tbTemp.Columns.Add("UOM");
            tbTemp.Columns.Add("ReportQty");
            tbTemp.Columns.Add("Quantity");
            tbTemp.Columns.Add("TotalReportQty");
            tbTemp.Columns.Add("OnHand");
            tbTemp.Columns.Add("avgPrice");
            tbTemp.Columns.Add("LineTotal");
            tbTemp.Columns.Add("Remarks");
            tbTemp.Columns.Add("frgnName");

            tbTemp.Columns.Add("CardCode");
            tbTemp.Columns.Add("Cumulative");

            return tbTemp;
        }
        /// <summary>
        /// 
        /// </summary>
        private void BindData(DataTable tb)
        {
            Session["ItemStockTake"] = tb;
            //this.grvClosingStock.DataSource = tb;
            //this.grvClosingStock.DataBind();
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadMonth()
        {
            for (int i = 0; i <= 12; i++)
            {
                if (i == 0)
                {
                    this.ddlMonth.Items.Add("MM");
                }
                else
                {
                    this.ddlMonth.Items.Add(i.ToString().PadLeft(2, '0'));
                }
            }
            if (this.ddlMonth.Items.Count > 0)
            {
                this.ddlMonth.SelectedIndex = 0;
                //this.ddlMonth.SelectedValue = DateTime.Now.Month.ToString().PadLeft(2, '0');
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadYear()
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet dsYear = obj.GetStockYear(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                                  Session[Utils.AppConstants.CompanyCode].ToString());
                if (dsYear != null)
                {
                    this.ddlYear.DataSource = dsYear;
                    this.ddlYear.DataTextField = "Year";
                    this.ddlYear.DataValueField = "Year";
                    this.ddlYear.DataBind();
                    if (this.ddlYear.Items.Count > 0)
                    {
                        this.ddlYear.SelectedIndex = -1;
                        //this.ddlYear.SelectedValue = DateTime.Now.Year.ToString();
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
        protected void ddlOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlMonth.SelectedItem.Text != "MM" && this.ddlYear.SelectedItem.Text != "YYYY")
            {
                lblError.Text = string.Empty;
                GetData(this.ddlOutlet.SelectedValue.ToString());
            }
            else
            {
                gvParentGrid.DataBind();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvUserInfo_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                MasterData obj = new MasterData();
                DataSet ds = new DataSet();
                if (Session["TempStockData"] != null)
                {
                    ds = (DataSet)Session["TempStockData"];
                }
                if (ds != null && ds.Tables.Count > 0)
                {
                    GridView gv = (GridView)e.Row.FindControl("grvClosingStock");
                    if (_docEntry.Length > 0)
                    {
                        gv.Enabled = false;
                        gv.Columns[6].Visible = false;
                        gv.Columns[7].Visible = false;
                        gv.Columns[8].Visible = false;
                        gv.Columns[9].Visible = false;
                    }
                    if (Request.QueryString["Create"] != null)
                    {
                        gv.Columns[5].Visible = false;
                        gv.Columns[10].Visible = false;
                    }

                    string cardCode = e.Row.Cells[1].Text;
                    DataTable tb = CreateFormat();
                    double grandTotal = 0;
                    DateTime dte = new DateTime();
                    if (this.ddlMonth.SelectedItem.Text != "MM" && this.ddlYear.SelectedItem.Text != "YYYY")
                    {
                        dte = new DateTime(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text), DateTime.DaysInMonth(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text)));
                    }
                    else
                    {
                        dte = new DateTime(int.Parse("1786"), int.Parse("01"), int.Parse("01"));
                    }
                    DataSet dsItem = obj.GetSupplierItemSetup(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                     Session[Utils.AppConstants.CompanyCode].ToString(), cardCode, this.ddlOutlet.SelectedValue.ToString(), dte);
                    // code to remove the duplicate items
                    DataTable tblTemp = (DataTable)Session["ItemStockTake"];
                    DataSet dsTmp = new DataSet();
                    if (dsItem != null)
                    {
                        dsTmp = dsItem.Clone();
                    }
                    if (tblTemp != null && dsItem != null)
                    {
                        foreach (DataRow rTmp in dsItem.Tables[0].Rows)
                        {
                            DataRow[] rck = tblTemp.Select("ItemCode='" + rTmp["ItemCode"] + "'");
                            if (rck.Length == 0)
                            {
                                dsTmp.Tables[0].ImportRow(rTmp);
                            }
                        }
                        dsItem = dsTmp.Copy();
                    }

                    foreach (DataRow setupRow in dsItem.Tables[0].Rows)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            if (row["ItemCode"].ToString() == setupRow["ItemCode"].ToString())
                            {
                                DataRow rowNew = tb.NewRow();
                                if (_docEntry.Length > 0)
                                {
                                    double Qty = double.Parse(row["U_AB_CountedQty"].ToString());
                                    rowNew["ItemCode"] = row["ItemCode"];
                                    rowNew["ItemName"] = row["Dscription"];
                                    rowNew["UOM"] = row["UOM"];
                                    rowNew["Quantity"] = double.Parse(row["Quantity"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT_STOCKTAKE);
                                    rowNew["Remarks"] = row["U_AB_LineRemarks"];
                                    this.txtRemarks.Text = row["Comments"].ToString().ToUpper();
                                    this.txtStockCheckBy.Text = row["U_AB_StockChkBy"].ToString().ToUpper();
                                    this.txtSubmittedBy.Text = row["U_AB_UserCode"].ToString().ToUpper();
                                    if (_dateTime.Length > 0 && Request.QueryString["Create"] == null)
                                    {
                                        string[] date = _dateTime.Split('/');
                                        if (date.Length >= 1)
                                        {
                                            this.ddlMonth.SelectedValue = date[0];
                                            this.ddlYear.SelectedValue = date[1];
                                        }
                                    }
                                    rowNew["frgnName"] = row["frgnName"];
                                    rowNew["Cumulative"] = double.Parse(row["Cumulative"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT_STOCKTAKE);
                                    rowNew["TotalReportQty"] = Qty.ToString(Utils.AppConstants.NUMBER_FORMAT_STOCKTAKE); ;
                                    if (_isDraft == "1")
                                    {

                                        rowNew["ReportQty"] = Qty > 0 ? Qty.ToString(Utils.AppConstants.NUMBER_FORMAT_STOCKTAKE) : "";
                                        gv.Columns[6].Visible = true;
                                        gv.Columns[8].Visible = true;
                                        gv.Columns[9].Visible = true;
                                        gv.Enabled = true;
                                        gv.Columns[5].Visible = false;
                                        gv.Columns[10].Visible = false;
                                        rowNew["avgPrice"] = double.Parse(row["LastPurPrc"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT_STOCKTAKE);
                                        double total = double.Parse(row["Cumulative"].ToString()) * double.Parse(row["LastPurPrc"].ToString());
                                        rowNew["TotalReportQty"] = total.ToString(Utils.AppConstants.NUMBER_FORMAT_STOCKTAKE);
                                    }
                                    else
                                    {
                                        gv.Columns[11].Visible = false;
                                    }
                                }
                                else
                                {
                                    rowNew["ItemCode"] = row["ItemCode"];
                                    rowNew["ItemName"] = row["ItemName"];
                                    rowNew["UOM"] = row["UOM"];
                                    rowNew["avgPrice"] = double.Parse(row["LastPurPrc"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT_STOCKTAKE);
                                    rowNew["Quantity"] = 0;
                                    double total = double.Parse(row["Cumulative"].ToString()) * double.Parse(row["LastPurPrc"].ToString());
                                    rowNew["TotalReportQty"] = total.ToString(Utils.AppConstants.NUMBER_FORMAT_STOCKTAKE);
                                    rowNew["frgnName"] = row["frgnName"];
                                    rowNew["Cumulative"] = double.Parse(row["Cumulative"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT_STOCKTAKE);

                                    grandTotal += total;
                                }
                                rowNew["CardCode"] = cardCode;
                                DataRow[] rCheck = tb.Select("ItemCode='" + row["ItemCode"].ToString() + "' AND CardCode='" + cardCode + "'");
                                if (rCheck.Length == 0)
                                {
                                    tb.Rows.Add(rowNew);
                                }
                            }
                        }
                    }
                    if (_docEntry.Length == 0)
                    {
                        this.txtGrandTotal.Text = grandTotal.ToString(Utils.AppConstants.NUMBER_FORMAT_STOCKTAKE);
                    }
                    if (Session["ItemStockTake"] != null)
                    {
                        if (((DataTable)Session["ItemStockTake"]).Rows.Count == 0)
                        {
                            Session["ItemStockTake"] = tb;
                        }
                        else
                        {
                            DataTable tbE = (DataTable)Session["ItemStockTake"];
                            foreach (DataRow r in tb.Rows)
                            {
                                DataRow[] rCheck = tbE.Select("ItemCode='" + r["ItemCode"].ToString() + "' AND CardCode='" + r["CardCode"].ToString() + "'");
                                if (rCheck.Length == 0)
                                {
                                    tbE.ImportRow(r);
                                }
                            }
                        }
                    }
                    else
                    {
                        Session["ItemStockTake"] = tb;
                    }
                    if (tb.Rows.Count == 0)
                    {
                        gv.Visible = false;
                        e.Row.Visible = false;
                    }
                    DataView dv = tb.DefaultView;
                    dv.Sort = "ItemCode ASC";
                    gv.DataSource = dv.ToTable();
                    gv.DataBind();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outletCode"></param>
        private void GetData(string outletCode)
        {
            try
            {
                Session["ItemStockTake"] = null;
                MasterData obj = new MasterData();
                DataSet ds = null;
                DataSet dsCumulative = new DataSet();
                DateTime dStockDate = new DateTime();
                if (_docEntry.Length > 0)
                {
                    if (_isDraft == "1")
                    {
                        ds = obj.GetStockTakeDraftDetail(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                                     Session[Utils.AppConstants.CompanyCode].ToString(), _outlet, _docEntry);
                    }
                    else
                    {
                        ds = obj.GetStockTakeDetail(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                                  Session[Utils.AppConstants.CompanyCode].ToString(), _outlet, _docEntry);
                    }
                }
                else
                {
                    if (this.ddlMonth.SelectedItem.Text != "MM" && this.ddlYear.SelectedItem.Text != "YYYY")
                    {
                        dStockDate = new DateTime(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text), DateTime.DaysInMonth(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text)));
                        //string st = dStockDate.ToString("dd/MM/yyyy");
                        ////dStockDate = new DateTime(2015, 05, 31);
                        //dStockDate = DateTime.ParseExact(st, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        dStockDate = new DateTime(int.Parse("1786"), int.Parse("01"), int.Parse("01"));
                    }
                    ds = obj.GetItemStockTake(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                                        Session[Utils.AppConstants.CompanyCode].ToString(), this.ddlOutlet.SelectedValue.ToString(), dStockDate);
                }
                if (_dateTime.Length > 0 && Request.QueryString["Create"] == null)
                {
                    string[] date = _dateTime.Split('/');
                    if (date.Length >= 1)
                    {
                        this.ddlMonth.SelectedValue = date[0];
                        this.ddlYear.SelectedValue = date[1];
                    }
                }
                if (Request.QueryString["Create"] != null)
                {
                    DataTable tbImport = new DataTable();
                    DateTime dte = new DateTime();
                    if (this.ddlMonth.SelectedItem.Text != "MM" && this.ddlYear.SelectedItem.Text != "YYYY")
                    {
                        dte = new DateTime(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text), DateTime.DaysInMonth(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text)));
                    }
                    else
                    {
                        dte = new DateTime(int.Parse("1786"), int.Parse("01"), int.Parse("01"));
                    }
                    dsCumulative = obj.GetItemCumulative(Session[Utils.AppConstants.CompanyCode].ToString(), this.ddlOutlet.SelectedValue.ToString(), dte);
                    if (ds != null && ds.Tables.Count > 0
                        && dsCumulative != null && dsCumulative.Tables.Count > 0)
                    {
                        tbImport = ds.Tables[0].Clone();
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            DataRow[] rItem = dsCumulative.Tables[0].Select("ItemCode='" + r["ItemCode"].ToString() + "'");
                            if (rItem.Length > 0)
                            {
                                if (double.Parse(rItem[0]["Cumulative"].ToString()) > 0)
                                {
                                    r["Cumulative"] = double.Parse(rItem[0]["Cumulative"].ToString());
                                    tbImport.ImportRow(r);
                                }
                            }
                        }
                        ds.Tables.Clear();
                        tbImport.TableName = "Item";
                        ds.Tables.Add(tbImport);
                    }

                }
                else
                {
                    if (_isDraft == "1")
                    {
                        DateTime dte = new DateTime();
                        if (this.ddlMonth.SelectedItem.Text != "MM" && this.ddlYear.SelectedItem.Text != "YYYY")
                        {
                            dte = new DateTime(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text), DateTime.DaysInMonth(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text)));
                        }
                        else
                        {
                            dte = DateTime.MinValue;
                        }
                        dsCumulative = obj.GetItemCumulative(Session[Utils.AppConstants.CompanyCode].ToString(), this.ddlOutlet.SelectedValue.ToString(), dte);
                        if (ds != null && ds.Tables.Count > 0
                            && dsCumulative != null && dsCumulative.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Columns.Contains("Cumulative"))
                            {
                                ds.Tables[0].Columns.Remove("Cumulative");
                                ds.Tables[0].Columns.Add("Cumulative");

                            }
                            foreach (DataRow r in ds.Tables[0].Rows)
                            {
                                DataRow[] rItem = dsCumulative.Tables[0].Select("ItemCode='" + r["ItemCode"].ToString() + "'");
                                if (rItem.Length > 0)
                                {
                                    if (Utils.AppConstants.isDouble(rItem[0]["Cumulative"].ToString()))
                                    {
                                        r["Cumulative"] = double.Parse(rItem[0]["Cumulative"].ToString());
                                    }
                                }
                            }
                        }
                    }
                }

                Session["TempStockData"] = ds;
                if (ds != null)
                {
                    DataSet dsSupplier = obj.GetSupplierOutletSetup(Session[Utils.AppConstants.CompanyCode].ToString(), this.ddlOutlet.SelectedValue.ToString());
                    DataTable tblVendor = null;
                    if (dsSupplier != null && dsSupplier.Tables.Count > 0)
                    {
                        tblVendor = dsSupplier.Tables[0];
                        this.gvParentGrid.DataSource = tblVendor;
                        this.gvParentGrid.DataBind();
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
        /// <param name="isDraft"></param>
        private void Update(bool isDraft)
        {
            if (ddlMonth.SelectedItem.Text == "MM")
            {
                lblError.Visible = true;
                lblError.Text = "Kindly Select the Month.";
                return;
            }
            else if (ddlYear.SelectedItem.Text == "YYYY")
            {
                lblError.Visible = true;
                lblError.Text = "Kindly Select the Year.";
                return;
            }
            else
            {
                lblError.Text = string.Empty;
                this.lblError.Visible = false;
                string objType = string.Empty;
                DataSet ds = new DataSet();
                DataTable tbHeader = new DataTable();
                tbHeader.Columns.Add("DocDate");
                tbHeader.Columns.Add("Comments");
                //tbHeader.Columns.Add("DocTotal");
                tbHeader.Columns.Add("U_AB_UserCode");
                tbHeader.Columns.Add("U_AB_POWhsCode");
                tbHeader.Columns.Add("U_AB_StockChkBy");

                if (isDraft)
                {
                    tbHeader.Columns.Add("ObjType");
                }
                MasterData obj = new MasterData();
                DataSet dsOnHand = obj.CheckOnHand("", "", Session[Utils.AppConstants.CompanyCode].ToString(), this.ddlOutlet.SelectedValue.ToString(), "");

                DataRow rHeader = tbHeader.NewRow();
                DateTime dte = new DateTime(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text), DateTime.DaysInMonth(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text)));
                rHeader["DocDate"] = dte.ToString("yyyyMMdd");
                rHeader["comments"] = this.txtRemarks.Text;
                rHeader["U_AB_UserCode"] = this.txtSubmittedBy.Text;
                rHeader["U_AB_POWhsCode"] = this.ddlOutlet.SelectedValue;
                rHeader["U_AB_StockChkBy"] = this.txtStockCheckBy.Text;

                if (isDraft)
                {
                    rHeader["ObjType"] = "60";
                }

                tbHeader.Rows.Add(rHeader);

                DataTable tbDetail = new DataTable();
                tbDetail.Columns.Add("ItemCode");
                tbDetail.Columns.Add("Dscription");
                tbDetail.Columns.Add("Quantity");
                tbDetail.Columns.Add("WhsCode");
                tbDetail.Columns.Add("U_AB_LineRemarks");
                tbDetail.Columns.Add("Price");
                tbDetail.Columns.Add("OcrCode");
                tbDetail.Columns.Add("U_AB_CountedQty");
                tbDetail.Columns.Add("LineStatus");
                //New Request 24/6/2014
                tbDetail.Columns.Add("CogsOcrCod");

                DataTable tb = (DataTable)Session["ItemStockTake"];
                if (tb != null)
                {
                    for (int j = 0; j < this.gvParentGrid.Rows.Count; j++)
                    {
                        this.grvClosingStock = (GridView)this.gvParentGrid.Rows[j].FindControl("grvClosingStock");
                        string cardCode = this.gvParentGrid.Rows[j].Cells[1].Text;
                        for (int i = 0; i < this.grvClosingStock.Rows.Count; i++)
                        {
                            Label lblItemCode = (Label)this.grvClosingStock.Rows[i].FindControl("lblItemCode");
                            TextBox txtReportQty = (TextBox)this.grvClosingStock.Rows[i].FindControl("txtReportQty");
                            TextBox txtQtyCtn = (TextBox)this.grvClosingStock.Rows[i].FindControl("txtQtyCtn");
                            TextBox txtAvgWt = (TextBox)this.grvClosingStock.Rows[i].FindControl("txtAvgWt");
                            TextBox txtRemarks = (TextBox)this.grvClosingStock.Rows[i].FindControl("txtRemarks");
                            DataRow[] rowNew = tb.Select("ItemCode='" + lblItemCode.Text + "' AND CardCode = '" + cardCode + "'");
                            if (rowNew.Length > 0)
                            {
                                UpdateQty(lblItemCode.Text, txtReportQty.Text, "ReportQty", cardCode);
                                UpdateQty(lblItemCode.Text, txtRemarks.Text, "Remarks", cardCode);
                            }
                        }
                    }

                    foreach (DataRow r in tb.Rows)
                    {
                        double reportQty = 0;
                        if (r["Cumulative"].ToString().Length > 0)
                        {
                            double cumulative = double.Parse(r["Cumulative"].ToString());
                            if (isDraft == false)
                            {
                                if (r["ReportQty"].ToString().Trim().Length > 0 && !Utils.AppConstants.isDouble(r["ReportQty"].ToString()))
                                {
                                    this.lblError.Text = r["ItemCode"].ToString() + " - In Counted Qty column, Qty is invalid format.";
                                    this.lblError.Visible = true;
                                    return;
                                }
                            }
                            if (Utils.AppConstants.isDouble(r["ReportQty"].ToString()))
                            {
                                reportQty = double.Parse(r["ReportQty"].ToString());
                            }
                            if (cumulative < reportQty)
                            {
                                this.lblError.Text = r["ItemCode"].ToString() + " - The Counted Qty cannot more than Cumulated Qty.";
                                this.lblError.Visible = true;
                                return;
                            }
                            double onHand = 0;
                            if (isDraft == false)
                            {
                                if (dsOnHand != null && dsOnHand.Tables.Count > 0 && dsOnHand.Tables[0].Rows.Count > 0)
                                {
                                    DataRow[] rOnHand = dsOnHand.Tables[0].Select("ItemCode='" + r["ItemCode"].ToString() + "'");
                                    if (rOnHand.Length > 0)
                                    {
                                        onHand = double.Parse(rOnHand[0]["OnHand"].ToString());
                                    }
                                }

                                DataRow[] rItemGroup = tb.Select("ItemCode='" + r["ItemCode"].ToString() + "'");
                                if (rItemGroup.Length > 1)
                                {
                                    double cumuGrp = 0, repQtyGrp = 0;
                                    foreach (DataRow rGroup in rItemGroup)
                                    {
                                        if (Utils.AppConstants.isDouble(r["Cumulative"].ToString()))
                                        {
                                            cumuGrp += double.Parse(rGroup["Cumulative"].ToString());
                                        }
                                        if (Utils.AppConstants.isDouble(r["ReportQty"].ToString()))
                                        {
                                            repQtyGrp += double.Parse(rGroup["ReportQty"].ToString());
                                        }
                                    }
                                    if (onHand < Math.Abs(cumuGrp - repQtyGrp))
                                    {
                                        this.lblError.Text = r["ItemCode"].ToString() + " - Quantity falls into negative inventory.";
                                        this.lblError.Visible = true;
                                        return;
                                    }
                                }
                                else
                                {
                                    if (onHand < Math.Abs(cumulative - reportQty))
                                    {
                                        this.lblError.Text = r["ItemCode"].ToString() + " - Quantity falls into negative inventory.";
                                        this.lblError.Visible = true;
                                        return;
                                    }
                                }
                            }

                            if (!isDraft)
                            {
                                tbHeader.TableName = "OIGE";
                                tbDetail.TableName = "IGE1";
                                objType = "60";
                            }
                            else
                            {
                                tbHeader.TableName = "ODRF";
                                tbDetail.TableName = "DRF1";
                                objType = "112";
                            }
                            DataRow rowNew = tbDetail.NewRow();
                            rowNew["ItemCode"] = r["ItemCode"];
                            rowNew["Dscription"] = r["ItemName"];
                            rowNew["WhsCode"] = this.ddlOutlet.SelectedValue.ToString();
                            //New change
                            //rowNew["Quantity"] = Math.Abs(onHand - reportQty);
                            if (isDraft == false)
                            {
                                rowNew["Quantity"] = Math.Abs(cumulative - reportQty);
                            }
                            else
                            {
                                rowNew["Quantity"] = 1;
                            }
                            rowNew["U_AB_LineRemarks"] = r["Remarks"];
                            rowNew["OcrCode"] = this.ddlOutlet.SelectedValue.ToString();
                            //New Request 24/6/2014
                            rowNew["CogsOcrCod"] = this.ddlOutlet.SelectedValue.ToString();
                            rowNew["LineStatus"] = "O";
                            rowNew["Price"] = r["avgPrice"];
                            rowNew["U_AB_CountedQty"] = reportQty;
                            if (isDraft == false)
                            {
                                if (cumulative != reportQty)
                                {
                                    tbDetail.Rows.Add(rowNew);
                                }
                            }
                            else
                            {
                                tbDetail.Rows.Add(rowNew);
                            }
                        }
                    }
                    ds.Tables.Add(tbHeader);
                    ds.Tables.Add(tbDetail);
                    if (tbDetail.Rows.Count > 0)
                    {
                        DocumentXML objInfo = new DocumentXML();
                        string requestXML = objInfo.ToXMLStringFromDS(objType, ds);
                        if (requestXML.Length > 0)
                        {
                            Transaction ts = new Transaction();
                            DataSet dsResult = new DataSet();
                            if (_isDraft == "1" && _docEntry.Length > 0 && isDraft)
                            {
                                dsResult = ts.CreateMarketingDocument(requestXML, Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                                     Session[Utils.AppConstants.CompanyCode].ToString(), objType, _docEntry, true);

                            }
                            else
                            {
                                dsResult = ts.CreateMarketingDocument(requestXML, Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                                    Session[Utils.AppConstants.CompanyCode].ToString(), objType, "", false);
                            }
                            if (dsResult != null && dsResult.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                if ((int)dsResult.Tables[0].Rows[0]["ErrCode"] != 0)
                                {
                                    Utils.AppConstants.WriteLog(dsResult.Tables[0].Rows[0]["ErrMsg"].ToString(), true);
                                    Utils.AppConstants.WriteLog(requestXML, true);
                                    Utils.AppConstants.WriteLog(Session[Utils.AppConstants.UserCode].ToString(), true);
                                    Utils.AppConstants.WriteLog(Session[Utils.AppConstants.CompanyCode].ToString(), true);
                                    this.lblError.Text = dsResult.Tables[0].Rows[0]["ErrMsg"].ToString();
                                }
                                else
                                {
                                    this.lblError.Text = "Operation complete successful!";
                                    if (_isDraft == "1" && isDraft == false)
                                    {
                                        Transaction trans = new Transaction();
                                        string error = string.Empty;
                                        error = trans.CloseCancelPO(int.Parse(_docEntry), Session[Utils.AppConstants.CompanyCode].ToString(), true, true);
                                        if (error.Length > 0)
                                        {
                                            this.lblError.Text = error;
                                        }
                                    }
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
                        //this.lblError.Text = "Stock take can not create because it do not changes any item.";
                    }
                    this.lblError.Visible = true;
                }
            }
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
                Update(false);
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
        protected void grvSearchResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.grvClosingStock.PageIndex = e.NewPageIndex;
                DataTable tb = (DataTable)Session["ItemStockTake"];
                if (tb != null)
                {
                    for (int i = 0; i < this.grvClosingStock.Rows.Count; i++)
                    {
                        Label lblItemCode = (Label)this.grvClosingStock.Rows[i].FindControl("lblItemCode");
                        TextBox txtReportQty = (TextBox)this.grvClosingStock.Rows[i].FindControl("txtReportQty");
                        TextBox txtQtyCtn = (TextBox)this.grvClosingStock.Rows[i].FindControl("txtQtyCtn");
                        TextBox txtAvgWt = (TextBox)this.grvClosingStock.Rows[i].FindControl("txtAvgWt");
                        TextBox txtRemarks = (TextBox)this.grvClosingStock.Rows[i].FindControl("txtRemarks");
                        DataRow[] rowNew = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                        if (rowNew.Length > 0)
                        {
                            UpdateQty(lblItemCode.Text, txtReportQty.Text, "ReportQty");
                            UpdateQty(lblItemCode.Text, txtRemarks.Text, "Remarks");
                        }

                    }
                    BindData(tb);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button1_Click(object sender, EventArgs e)
        {
            Update(true);
        }

        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlMonth.SelectedItem.Text != "MM" && this.ddlYear.SelectedItem.Text != "YYYY")
            {
                lblError.Text = string.Empty;
                GetData(this.ddlOutlet.SelectedValue.ToString());
            }
            else
            {
                this.gvParentGrid.DataBind();
            }
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddlMonth.SelectedItem.Text != "MM" && this.ddlYear.SelectedItem.Text != "YYYY")
            {
                lblError.Text = string.Empty;
                GetData(this.ddlOutlet.SelectedValue.ToString());
            }
            else
            {
                this.gvParentGrid.DataBind();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDeleteDraft_Click(object sender, EventArgs e)
        {
            try
            {
                Transaction obj = new Transaction();
                string error = string.Empty;
                error = obj.CloseCancelPO(int.Parse(_docEntry), Session[Utils.AppConstants.CompanyCode].ToString(), true, true);
                if (error.Length == 0)
                {
                    this.lblError.Text = "Operation complete successful!";
                    this.gvParentGrid.Enabled = false;
                    this.btnDeleteDraft.Visible = false;
                    this.btnSaveAsDraft.Visible = false;
                    this.btnUpdate.Visible = false;
                    this.txtStockCheckBy.Enabled = false;
                    this.txtSubmittedBy.Enabled = false;
                    this.txtRemarks.Enabled = false;
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