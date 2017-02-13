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
    public partial class StockTakeDrafts : System.Web.UI.Page
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
                LoadMonth();
                LoadYear();
                LoadOutlet();
                LoadData();
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
                Label lnkPONo = (Label)gr.FindControl("lblDocNum");
                Label lblDocTotal = (Label)gr.FindControl("lblDocTotal");
                Label lblComments = (Label)gr.FindControl("lblDelDate");
                Label lblOutlet = (Label)gr.FindControl("lblOutlet");
                Label lblPODate = (Label)gr.FindControl("lblPODate");
                Session["stMonth"] = this.ddlMonth.SelectedValue.ToString();
                Session["stYear"] = this.ddlYear.SelectedValue.ToString();
                Response.Redirect("ClosingStock.aspx?isDraft=1&DocEntry=" + lblDocNum.Text + "&StockNo=" + lnkPONo.Text + "&DocTotal=" + lblDocTotal.Text + "&Comment=" + lblComments.Text + "&Whs=" + lblOutlet.Text + "&Date=" + lblPODate.Text);
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
                Session["stMonth"] = null;
                Session["stYear"] = null;
                Search();
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }
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
        private void LoadMonth()
        {
            for (int i = 1; i <= 12; i++)
            {
                this.ddlMonth.Items.Add(i.ToString().PadLeft(2, '0'));
            }
            if (this.ddlMonth.Items.Count > 0)
            {
                if (Session["stMonth"] != null)
                {
                    this.ddlMonth.SelectedValue = Session["stMonth"].ToString();
                }
                else
                {
                    this.ddlMonth.SelectedValue = DateTime.Now.Month.ToString().PadLeft(2, '0');
                }
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
                        if (Session["stYear"] != null)
                        {
                            this.ddlYear.SelectedValue = Session["stYear"].ToString();
                        }
                        else
                        {
                            this.ddlYear.SelectedValue = DateTime.Now.Year.ToString();
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
        private void Search()
        {
            try
            {
                if (Session[Utils.AppConstants.CompanyCode].ToString() != System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString())
                {
                    MasterData obj = new MasterData();
                    DataSet dsUser = obj.GetOnlineUser();
                    DateTime fromDate = DateTime.Now;
                    DateTime toDate = DateTime.Now;
                    if (Session["stYear"] != null)
                    {
                        this.ddlYear.SelectedValue = Session["stYear"].ToString();
                    }
                    if (Session["stMonth"] != null)
                    {
                        this.ddlMonth.SelectedValue = Session["stMonth"].ToString();
                    }
                    DateTime dte = new DateTime(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text), DateTime.DaysInMonth(int.Parse(this.ddlYear.SelectedItem.Text), int.Parse(this.ddlMonth.SelectedItem.Text)));
                 
                    DataSet ds = obj.DraftStockTake(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                      Session[Utils.AppConstants.CompanyCode].ToString(), dte,
                                      this.ddlOutlet.SelectedValue.ToString(), Session[Utils.AppConstants.IsSupperUser].ToString());

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        int i = 1;
                        ds.Tables[0].Columns.Add("No");
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            r["DocDate"] = DateTime.Parse(r["DocDate"].ToString()).Date.ToString("MM/yyyy");
                            r["DocTotal"] = double.Parse(r["DocTotal"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                            r["Quantity"] = double.Parse(r["Quantity"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                            r["No"] = i;
                            i++;
                        }
                        this.grvSearchResult.DataSource = ds;
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