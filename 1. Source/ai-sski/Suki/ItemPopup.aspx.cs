using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class ItemPopup : System.Web.UI.Page
    {
        private int _rowCount = 0;
        private int _checkRow = 0;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            LoadItem();
            Timer1.Enabled = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadItem()
        {
            try
            {
                MasterData obj = new MasterData();
                DataTable tblItem = null;
                if (Request.QueryString["Supplier"] != null)
                {
                    tblItem = obj.GetSupplierItemSetup(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), Session[Utils.AppConstants.CompanyCode].ToString(), Request.QueryString["Supplier"].ToString(), Request.QueryString["WareHouse"].ToString(),DateTime.Now).Tables[0];
                }
                else
                {
                    tblItem = obj.GetOITM(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), Session[Utils.AppConstants.CompanyCode].ToString()).Tables[0];

                }
                if (tblItem != null)
                {
                    _rowCount = tblItem.Rows.Count;
                    Session["ItemTable"] = tblItem;
                    this.grdItem.DataSource = tblItem;
                    this.grdItem.DataBind();
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
        protected void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> dicItem = new Dictionary<string, string>();
                for (int i = 0; i < this.grdItem.Rows.Count; i++)
                {
                    CheckBox checkBox = (CheckBox)this.grdItem.Rows[i].FindControl("chkChild");

                    if (checkBox != null)
                    {
                        if (checkBox.Checked)
                        {
                            Label code = (Label)this.grdItem.Rows[i].FindControl("lblItemCode");
                            Label Name = (Label)this.grdItem.Rows[i].FindControl("lblItemName");
                            dicItem.Add(code.Text, Name.Text);
                        }
                    }
                }
                Session["ChosenItem"] = dicItem;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OK", "Main.okDialogClick('SelectItem');", true);
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataTable tblItem = (DataTable)Session["ItemTable"];
            if (tblItem != null && this.txtSearch.Text.Trim().Length > 0)
            {
                DataView dv = tblItem.DefaultView;

                dv.RowFilter = "ItemCode like '%" + this.txtSearch.Text.Trim() + "%' Or ItemName like '%" + this.txtSearch.Text.Trim() + "%'";

                this.grdItem.DataSource = dv.ToTable();
                this.grdItem.DataBind();
            }
            else
            {
                LoadItem();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdItem_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dt = Session["ItemTable"] as DataTable;
            if (dt != null)
            {
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                this.grdItem.DataSource = Session["ItemTable"];
                this.grdItem.DataBind();
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
        protected void grdItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                e.Row.Attributes["style"] = "cursor:pointer";
                if (Session["ChosenItem"] != null)
                {
                    Dictionary<string, string> dicItem = (Dictionary<string, string>)Session["ChosenItem"];
                    Label code = (Label)e.Row.FindControl("lblItemCode");
                    try
                    {
                        if (code != null && dicItem[code.Text.Trim()].Length > 0)
                        {
                            CheckBox checkBox = (CheckBox)e.Row.FindControl("chkChild");
                            if (checkBox != null)
                            {
                                checkBox.Checked = true;
                                _checkRow++;
                                if (_checkRow == _rowCount)
                                {
                                    //Check All
                                    GridViewRow headerRow = this.grdItem.HeaderRow;
                                    ((CheckBox)headerRow.FindControl("chkheader")).Checked = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //
                    }
                }
            }
        }
    }
}
