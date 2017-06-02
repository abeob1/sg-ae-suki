using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class VendorPopup : System.Web.UI.Page
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
        }
        /// <summary>
        /// 
        /// </summary>
        private void BindVendor()
        {
            try
            {
                DataTable tblVendor = LoadVendor();
                if (tblVendor != null)
                {
                    Session["VendorTable"] = tblVendor;
                    this.grdVendor.DataSource = tblVendor;
                    this.grdVendor.DataBind();
                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DataTable LoadVendor()
        {
            try
            {
                MasterData obj = new MasterData();
                DataTable tblVendor = null;
                if (Request.QueryString["IsSetup"] != null)
                {
                    tblVendor = obj.GetOCRD(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), Session[Utils.AppConstants.CompanyCode].ToString()).Tables[0];
                }
                else
                {
                    string companyCode = Session[Utils.AppConstants.CompanyCode].ToString();
                    if (Request.QueryString["CompanyCode"] != null)
                    {
                        companyCode = Request.QueryString["CompanyCode"].ToString();
                    }
                    string outlet = string.Empty;
                    if (Request.QueryString["WareHouse"] != null)
                    {
                        outlet = Request.QueryString["WareHouse"].ToString();
                    }
                    DataSet ds = obj.GetSupplierOutletSetup(companyCode, outlet);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        tblVendor = ds.Tables[0];
                    }
                }
                return tblVendor;
            }
            catch
            {
            }
            return null;
        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            BindVendor();
            Timer1.Enabled = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataTable tblItem = (DataTable)Session["VendorTable"];
            if (tblItem != null && this.txtSearch.Text.Trim().Length > 0)
            {
                if (tblItem.Rows.Count == 0)
                {
                    tblItem = LoadVendor();
                }
                if (tblItem != null)
                {
                    DataView dv = tblItem.DefaultView;

                    dv.RowFilter = "CardCode like '%" + this.txtSearch.Text.Trim() + "%' Or CardName like '%" + this.txtSearch.Text.Trim() + "%'";

                    Session["VendorTable"] = dv.ToTable();
                    this.grdVendor.DataSource = dv.ToTable();
                    this.grdVendor.DataBind();
                }
            }
            else
            {
                BindVendor();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdVendor_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                DataTable dt = Session["VendorTable"] as DataTable;
                if (dt != null)
                {
                    dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                    Session["VendorTable"] = dt.DefaultView.ToTable();
                    this.grdVendor.DataSource = dt.DefaultView.ToTable();
                    this.grdVendor.DataBind();
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetSortDirection(string column)
        {

            string sortDirection = "ASC";
            string sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow row = this.grdVendor.SelectedRow;
                DataTable dt = (DataTable)Session["VendorTable"];
                if (dt != null)
                {
                    int index = row.DataItemIndex;
                    var data = dt.Rows[index];
                    if (data != null)
                    {
                        Session["ChosenVendorCode"] = data["CardCode"];
                        Session["ChosenVendorName"] = data["CardName"];
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "OK", "Main.okDialogClick('SelectVendor');", true);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdVendor_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                    e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)"); 

                    e.Row.Attributes["style"] = "cursor:pointer";
                    e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.grdVendor, "Select$" + e.Row.RowIndex);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}