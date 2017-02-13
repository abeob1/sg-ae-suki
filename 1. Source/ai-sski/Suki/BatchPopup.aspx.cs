using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class BatchPopup : System.Web.UI.Page
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
                this.hdnItemCode.Value = Request.QueryString["ItemCode"];
                this.hdnItemName.Value = Request.QueryString["ItemName"];
                this.hdnLineNum.Value = Request.QueryString["LineNum"];
                this.hdnQuantity.Value = Request.QueryString["Qty"];
                if (Request.QueryString["IsUpdate"].ToString() == "E")
                {
                    this.btnAccept.Visible = false;
                    this.btnAddNewBatch.Visible = false;
                }
                LoadItem();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private DataTable CreateFormat()
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("No");
            tb.Columns.Add("ItemCode");
            tb.Columns.Add("Dscription");
            tb.Columns.Add("LineNum");
            tb.Columns.Add("BatchNo");
            tb.Columns.Add("Quantity");
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
                DataTable tb = (DataTable)Session[this.hdnItemCode.Value + "_" + this.hdnLineNum.Value + "_BatchData"];
                if (tb == null || tb.Rows.Count == 0)
                {
                    tb = CreateFormat();
                    DataRow rowNew = tb.NewRow();
                    rowNew["No"] = 1;
                    rowNew["ItemCode"] = this.hdnItemCode.Value;
                    rowNew["Dscription"] = this.hdnItemName.Value;
                    rowNew["LineNum"] = this.hdnLineNum.Value;
                    rowNew["Quantity"] = this.hdnQuantity.Value;
                    tb.Rows.Add(rowNew);
                }
                BindBatch(tb);
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
        private void BindBatch(DataTable tb)
        {
            Session[this.hdnItemCode.Value + "_" + this.hdnLineNum.Value + "_BatchData_Temp"] = tb;
            this.grdItem.DataSource = tb;
            this.grdItem.DataBind();
        }
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
                Label lblNo = (Label)gr.FindControl("lblNo");
                DataTable tb = (DataTable)Session[this.hdnItemCode.Value + "_" + this.hdnLineNum.Value + "_BatchData_Temp"];
                if (tb != null)
                {
                    DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "' AND No='" + lblNo.Text + "'");
                    if (rupdate.Length > 0)
                    {
                        tb.Rows.Remove(rupdate[0]);
                    }
                    this.grdItem.EditIndex = -1;
                    BindBatch((DataTable)Session[this.hdnItemCode.Value + "_" + this.hdnLineNum.Value + "_BatchData_Temp"]);
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
                DataSet ds = obj.GetOutletItemList(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), this.hdnItemCode.Value, this.hdnItemCode.Value, "1");
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    BindBatch(ds.Tables[0]);
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
        protected void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)Session[this.hdnItemCode.Value + "_" + this.hdnLineNum.Value + "_BatchData_Temp"];
                if (tb != null)
                {
                    double qty = 0;
                    for (int i = 0; i < this.grdItem.Rows.Count; i++)
                    {
                        Label lblOutletCode = (Label)this.grdItem.Rows[i].FindControl("lblItemCode");
                        Label lblNo = (Label)this.grdItem.Rows[i].FindControl("lblNo");
                        TextBox txtBatchNo = (TextBox)this.grdItem.Rows[i].FindControl("txtBatchNo");
                        TextBox txtQuantity = (TextBox)this.grdItem.Rows[i].FindControl("txtQuantity");

                        DataRow[] rowNew = tb.Select("ItemCode='" + lblOutletCode.Text + "' AND No='" + lblNo.Text + "'");
                        if (txtBatchNo.Text.Trim().Length == 0)
                        {
                            this.lblError.Text = "Please input Batch No.";
                            this.lblError.Visible = true;
                            txtBatchNo.Focus();
                            return;
                        }
                        if (txtQuantity.Text.Trim().Length == 0)
                        {
                            this.lblError.Text = "In Quantity column, enter whole number greater than 0";
                            this.lblError.Visible = true;
                            txtQuantity.Focus();
                            return;
                        }
                        if (rowNew.Length > 0)
                        {
                            rowNew[0]["BatchNo"] = txtBatchNo.Text;
                            rowNew[0]["Quantity"] = txtQuantity.Text;
                        }
                        qty += double.Parse(txtQuantity.Text);
                    }
                    if (qty != double.Parse(this.hdnQuantity.Value))
                    {
                        this.lblError.Text = "Quantity does not match.";
                        this.lblError.Visible = true;
                        return;
                    }
                    Session[this.hdnItemCode.Value + "_" + this.hdnLineNum.Value + "_BatchData"] = tb;
                    Session[this.hdnItemCode.Value + "_" + this.hdnLineNum.Value + "_BatchData_Temp"] = null;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "OK", "Main.okDialogClick('BatchSetup');", true);
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
        protected void grdItem_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void btnAddNewBatch_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tb = (DataTable)Session[this.hdnItemCode.Value + "_" + this.hdnLineNum.Value + "_BatchData_Temp"];
               
                if (tb != null)
                {
                    //Update Grid Item
                    for (int i = 0; i < this.grdItem.Rows.Count; i++)
                    {
                        Label lblOutletCode = (Label)this.grdItem.Rows[i].FindControl("lblItemCode");
                        Label lblNo = (Label)this.grdItem.Rows[i].FindControl("lblNo");
                        TextBox txtBatchNo = (TextBox)this.grdItem.Rows[i].FindControl("txtBatchNo");
                        TextBox txtQuantity = (TextBox)this.grdItem.Rows[i].FindControl("txtQuantity");

                        DataRow[] row = tb.Select("ItemCode='" + lblOutletCode.Text + "' AND No='" + lblNo.Text + "'");
                        if (row.Length > 0)
                        {
                            row[0]["BatchNo"] = txtBatchNo.Text;
                            row[0]["Quantity"] = txtQuantity.Text;
                        }
                    }

                    DataRow rowNew = tb.NewRow();
                    if (tb.Rows.Count == 0)
                    {
                        rowNew["No"] = 1;
                    }
                    else
                    {
                        rowNew["No"] = int.Parse(tb.Rows[tb.Rows.Count - 1]["No"].ToString()) + 1;
                    }
                    rowNew["ItemCode"] = this.hdnItemCode.Value;
                    rowNew["Dscription"] = this.hdnItemName.Value;
                    rowNew["LineNum"] = this.hdnLineNum.Value;
                    tb.Rows.Add(rowNew);
                    BindBatch(tb);
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
