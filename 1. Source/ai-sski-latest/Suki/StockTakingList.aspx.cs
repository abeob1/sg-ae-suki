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

namespace Suki
{
    public partial class StockTakingList : System.Web.UI.Page
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
        protected void grvSearchResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
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
                Label lnkPONo = (Label)gr.FindControl("lblDocNum");
                Label lblDocTotal = (Label)gr.FindControl("lblDocTotal");
                Label lblComments = (Label)gr.FindControl("lblDelDate");
                Label lblOutlet = (Label)gr.FindControl("lblOutlet");
                Label lblPODate = (Label)gr.FindControl("lblPODate");
                Response.Redirect("ClosingStock.aspx?DocEntry=" + lblDocNum.Text + "&StockNo=" + lnkPONo.Text + "&DocTotal=" + lblDocTotal.Text + "&Comment=" + lblComments.Text + "&Whs=" + lblOutlet.Text + "&Date=" + lblPODate.Text);
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
            tbTemp.Columns.Add("DocDate");
            tbTemp.Columns.Add("Comments");
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
                if (Session[Utils.AppConstants.CompanyCode].ToString() != System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString())
                {
                    MasterData obj = new MasterData();
                    DataSet ds = new DataSet();

                    ds = obj.GetStockTakeList(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                        Session[Utils.AppConstants.CompanyCode].ToString());

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
            grvSearchResult.PageIndex = e.NewPageIndex;
            Search();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            Response.Redirect("Rpt_StockTakeList.aspx");
        }
    }
}