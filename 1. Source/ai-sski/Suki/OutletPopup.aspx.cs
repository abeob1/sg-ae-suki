using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class OutletPopup : System.Web.UI.Page
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
            if (!IsPostBack)
            {
                this.hdnCompanyCode.Value = Request.QueryString["CompanyCode"];
                this.hdnCompanyName.Text = Request.QueryString["CompanyName"];
                this.hdnCardCode.Value = Request.QueryString["CardCode"];
                this.hdnCardName.Text = Request.QueryString["CardName"];
                this.lblItemCode.Text = Request.QueryString["ItemCode"];
                LoadItem();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private DataTable CreateFormat()
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("isCheck");
            tb.Columns.Add("HeaderID");
            tb.Columns.Add("ItemCode");
            tb.Columns.Add("CardCode");
            tb.Columns.Add("CompanyCode");
            tb.Columns.Add("OutletCode");
            tb.Columns.Add("OutletName");
            tb.Columns.Add("MinQty");
            tb.Columns.Add("MaxQty");
            tb.Columns.Add("MinAmt");
            tb.Columns.Add("MaxAmt");
            return tb;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBaseName"></param>
        private void LoadItem()
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet dsOutlet = obj.GetABOutletByCompany(this.hdnCompanyCode.Value);
                if (dsOutlet != null && dsOutlet.Tables.Count > 0)
                {
                    DataTable tblOutlet = dsOutlet.Tables[0];
                    if (tblOutlet != null)
                    {
                        DataTable tb = CreateFormat();
                        DataTable tbChosen = (DataTable)Session[this.hdnCardCode.Value + "_" + this.lblItemCode.Text + "_OutletData"];
                        foreach (DataRow row in tblOutlet.Rows)
                        {
                            DataRow rowNew = tb.NewRow();
                            rowNew["OutletCode"] = row["U_WhseCode"];
                            rowNew["OutletName"] = row["U_WhseName"];
                            if (tbChosen != null)
                            {
                                DataRow[] rS = tbChosen.Select("OutletCode = '" + row["U_WhseCode"].ToString() + "'");
                                if (rS.Length > 0)
                                {
                                    rowNew["HeaderID"] = rS[0]["HeaderID"];
                                    rowNew["isCheck"] = rS[0]["isCheck"];
                                    rowNew["MinQty"] = rS[0]["MinQty"];
                                    rowNew["MaxQty"] = rS[0]["MaxQty"];
                                    rowNew["MinAmt"] = rS[0]["MinAmt"];
                                    rowNew["MaxAmt"] = rS[0]["MaxAmt"];
                                }
                            }
                            else
                            {
                                DataSet ds = obj.GetOutletItemList(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), this.hdnCardCode.Value, this.hdnCompanyCode.Value, this.lblItemCode.Text);
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow[] rCheck = ds.Tables[0].Select("OutletCode='" + row["U_WhseCode"].ToString() + "'");
                                    if (rCheck.Length > 0)
                                    {
                                        rowNew["HeaderID"] = rCheck[0]["HeaderID"];
                                        rowNew["isCheck"] = "1";
                                        rowNew["MinQty"] = rCheck[0]["MinQty"];
                                        rowNew["MaxQty"] = rCheck[0]["MaxQty"];
                                        rowNew["MinAmt"] = rCheck[0]["MinAmt"];
                                        rowNew["MaxAmt"] = rCheck[0]["MaxAmt"];
                                    }
                                    else
                                    {
                                        rowNew["HeaderID"] = 0;
                                        rowNew["isCheck"] = 0;
                                        rowNew["MinQty"] = 0;
                                        rowNew["MaxQty"] = 0;
                                        rowNew["MinAmt"] = 0;
                                        rowNew["MaxAmt"] = 0;
                                    }
                                }
                                else
                                {
                                    rowNew["HeaderID"] = 0;
                                    rowNew["isCheck"] = 0;
                                    rowNew["MinQty"] = 0;
                                    rowNew["MaxQty"] = 0;
                                    rowNew["MinAmt"] = 0;
                                    rowNew["MaxAmt"] = 0;
                                }
                            }
                            rowNew["ItemCode"] = this.lblItemCode.Text;
                            rowNew["CardCode"] = this.hdnCardCode.Value;
                            rowNew["CompanyCode"] = this.hdnCompanyCode.Value;
                            if (Session[Utils.AppConstants.IsSupperUser] != null
                                  && Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y"
                                  && Session[Utils.AppConstants.ListOutlet] != null
                                   && ((Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet]).Count > 0)
                            {
                                Dictionary<string, string> dicOutlet = (Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet];
                                if (dicOutlet.ContainsKey(row["U_WhseCode"].ToString()))
                                {
                                    tb.Rows.Add(rowNew);
                                }
                            }
                            else
                            {
                                tb.Rows.Add(rowNew);
                            }
                          
                        }
                        BindOutlet(tb);
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
        /// <param name="tb"></param>
        private void BindOutlet(DataTable tb)
        {
            Session[this.hdnCardCode.Value + "_" + this.lblItemCode.Text + "_OutletData"] = tb;
            _rowCount = tb.Rows.Count;
            this.grdItem.DataSource = tb;
            this.grdItem.DataBind();
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadOutlet()
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet ds = obj.GetOutletItemList(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), this.hdnCardCode.Value, this.hdnCompanyCode.Value, this.lblItemCode.Text);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    BindOutlet(ds.Tables[0]);
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Timer1.Enabled = false;
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
                DataTable tb = ((DataTable)Session[this.hdnCardCode.Value + "_" + this.lblItemCode.Text + "_OutletData"]);
                if (tb != null)
                {
                    for (int i = 0; i < this.grdItem.Rows.Count; i++)
                    {
                        CheckBox checkBox = (CheckBox)this.grdItem.Rows[i].FindControl("chkChild");

                        if (checkBox != null)
                        {
                            Label lblOutletCode = (Label)this.grdItem.Rows[i].FindControl("lblOutletCode");
                            Label lblOutletName = (Label)this.grdItem.Rows[i].FindControl("lblOutletName");
                            TextBox txtMinQty = (TextBox)this.grdItem.Rows[i].FindControl("txtMinQty");
                            TextBox txtMaxQty = (TextBox)this.grdItem.Rows[i].FindControl("txtMaxQty");

                            DataRow[] rowNew = tb.Select("OutletCode='" + lblOutletCode.Text + "'");
                            if (checkBox.Checked)
                            {
                                if (txtMinQty.Text.Trim().Length == 0
                               || txtMaxQty.Text.Trim().Length == 0)
                                {
                                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert('In Quantity,Amount column, enter whole number greater or equal 0');", true);
                                    return;
                                }
                                if (rowNew.Length > 0)
                                {
                                    rowNew[0]["isCheck"] = "1";
                                    rowNew[0]["MinQty"] = txtMinQty.Text;
                                    rowNew[0]["MaxQty"] = txtMaxQty.Text;
                                    rowNew[0]["MinAmt"] = 0;
                                    rowNew[0]["MaxAmt"] = 0;
                                }
                            }
                            else
                            {
                                rowNew[0]["isCheck"] = "0";
                            }
                        }
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OK", "Main.okDialogClick('Outlet');", true);
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
        protected void grdItem_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                DataTable dt = Session[this.hdnCardCode.Value + "_" + this.lblItemCode.Text + "_OutletData"] as DataTable;
                if (dt != null)
                {
                    dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                    this.grdItem.DataSource = dt;
                    this.grdItem.DataBind();
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
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetSortDirection(string column)
        {
            try
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
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
            return string.Empty;
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
                DataTable tb = (DataTable)Session[this.hdnCardCode.Value + "_" + this.lblItemCode.Text + "_OutletData"];
                if (tb != null)
                {
                    try
                    {
                        Label outletCode = (Label)e.Row.FindControl("lblOutletCode");
                        DataRow[] rCheck = tb.Select("OutletCode ='" + outletCode.Text + "'");
                        if (rCheck.Length > 0)
                        {
                            CheckBox checkBox = (CheckBox)e.Row.FindControl("chkChild");
                            if (checkBox != null && rCheck[0]["isCheck"].ToString() == "1")
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
                        this.lblError.Text = ex.Message;
                        this.lblError.Visible = true;
                    }
                }
            }
        }
    }
}
