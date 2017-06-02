using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class CompanySetupList : System.Web.UI.Page
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
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString() != Session[Suki.Utils.AppConstants.CompanyCode].ToString())
                {
                    Response.Redirect("Default.aspx");
                }
                if (!IsPostBack)
                {
                    LoadCompany();

                    if (Request.QueryString["Company"] == null)
                    {
                        LoadItem();
                    }
                    else
                    {
                        this.drdOutlet.SelectedValue = Request.QueryString["Company"].ToString();
                        LoadOutletSupplier(this.drdOutlet.SelectedValue.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                string alert = "alert('Error:" + ex.Message.Replace('\'', ' ') + " ');";
                ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", alert, true);
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
                LinkButton lblItemCode = (LinkButton)gr.FindControl("lblItemCode");
                Label lblID = (Label)gr.FindControl("txtDscription");
                MasterData obj = new MasterData();
                string err = obj.DeleteVendorSetup(this.drdOutlet.SelectedValue.ToString(), lblItemCode.Text);
                if (err.Length > 0)
                {
                    this.lblError.Text = err;
                    return;
                }
                else
                {
                    this.lblError.Text = "Operation complete successful!";
                    LoadOutletSupplier(this.drdOutlet.SelectedValue.ToString());
                }
                this.lblError.Visible = true;
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
            Session["PageIndex"] = e.NewPageIndex;
            this.grvItem.PageIndex = e.NewPageIndex;
            LoadOutletSupplier(this.drdOutlet.SelectedValue.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardCode"></param>
        /// <param name="companyCode"></param>
        private void LoadOutletSupplier(string companyCode)
        {
            try
            {
                if (companyCode.Length > 0)
                {
                    MasterData obj = new MasterData();
                    DataSet ds = obj.GetSupplierSetupList(companyCode);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable tb = ds.Tables[0].Clone();
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            DataRow[] rCardCode = tb.Select("CardCode='" + r["CardCode"].ToString() + "'");
                            if (rCardCode.Length == 0)
                            {
                                tb.ImportRow(r);
                            }
                        }
                        BindItem(tb);
                    }
                    else
                    {
                        BindItem(null);
                    }
                }
                else
                {
                    BindItem(null);
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
            tb.Columns.Add("CardCode");
            tb.Columns.Add("CardName");
            BindItem(tb);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb"></param>
        private void BindItem(DataTable tb)
        {
            if (Session["PageIndex"] != null)
            {
                this.grvItem.PageIndex = int.Parse(Session["PageIndex"].ToString());
            }
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
                            this.drdOutlet.Items.Insert(0, new ListItem("", ""));
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
                            }
                            this.drdOutlet.Items.Insert(0, new ListItem("", ""));
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lblItemCode_Click(object sender, EventArgs e)
        {
            try
            {
                GridViewRow gr = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                LinkButton lnkPONo = (LinkButton)gr.FindControl("lblItemCode");
                Label lnkName = (Label)gr.FindControl("txtDscription");
                Response.Redirect("OutletItemList.aspx?IsNew=E&CompanyCode=" + this.drdOutlet.SelectedValue.ToString() + "&VendorCode=" + lnkPONo.Text + "&VendorName=" + lnkName.Text);
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
        protected void drdOutlet_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadOutletSupplier(this.drdOutlet.SelectedValue.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCalendar_Click(object sender, EventArgs e)
        {
            Response.Redirect("OutletItemList.aspx?CompanyCode=" + this.drdOutlet.SelectedValue.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCopyTemplate_Click(object sender, EventArgs e)
        {

        }
    }
}