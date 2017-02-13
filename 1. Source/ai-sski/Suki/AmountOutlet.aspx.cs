using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class AmountOutlet : System.Web.UI.Page
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
                this.hdnCompanyCode.Value = Request.QueryString["CompanyCode"];
                this.hdnCompanyName.Text = Request.QueryString["CompanyName"];
                this.hdnCardCode.Value = Request.QueryString["CardCode"];
                this.hdnCardName.Text = Request.QueryString["CardName"];
                LoadOrderAmount();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadOrderAmount()
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet ds = obj.GetOutletAmount(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), this.hdnCardCode.Value, this.hdnCompanyCode.Value, Session[Utils.AppConstants.CompanyCode].ToString());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable tblOutlet = obj.GetABOutletByCompany(this.hdnCompanyCode.Value).Tables[0];
                    if (tblOutlet != null)
                    {
                        foreach (DataRow r in tblOutlet.Rows)
                        {
                            DataRow[] rNew = ds.Tables[0].Select("OutletCode='" + r["U_OutletID"].ToString() + "'");
                            if (rNew.Length == 0)
                            {
                                DataRow rowNew = ds.Tables[0].NewRow();
                                rowNew["OutletCode"] = r["U_WhseCode"];
                                rowNew["OutletName"] = r["U_WhseName"];
                                rowNew["MinAmt"] = 0;
                                rowNew["MaxAmt"] = 0;
                                if (Session[Utils.AppConstants.IsSupperUser] != null
                                    && Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y"
                                    && Session[Utils.AppConstants.ListOutlet] != null
                                     && ((Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet]).Count > 0)
                                {
                                    Dictionary<string, string> dicOutlet = (Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet];
                                    if (dicOutlet.ContainsKey(r["U_WhseCode"].ToString()))
                                    {
                                        ds.Tables[0].Rows.Add(rowNew);
                                    }
                                }
                                else
                                {
                                    ds.Tables[0].Rows.Add(rowNew);
                                }
                            }
                        }
                    }
                    DataTable tbAccess = ds.Tables[0].Clone();
                    if (Session[Utils.AppConstants.IsSupperUser] != null
                               && Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y"
                               && Session[Utils.AppConstants.ListOutlet] != null
                                && ((Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet]).Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Dictionary<string, string> dicOutlet = (Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet];
                            if (dicOutlet.ContainsKey(row["OutletCode"].ToString()))
                            {
                                tbAccess.ImportRow(row);
                            }
                        }
                    }
                    else
                    {
                        tbAccess = ds.Tables[0];
                    }
                    BindCalendar(tbAccess);
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
        private DataTable CreateFormat()
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("ID");
            tb.Columns.Add("OutletCode");
            tb.Columns.Add("OutletName");
            tb.Columns.Add("MinAmt");
            tb.Columns.Add("MaxAmt");
            return tb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb"></param>
        private void BindCalendar(DataTable tb)
        {
            Session["OrderAmountData"] = tb;
            this.grdItem.DataSource = tb;
            this.grdItem.DataBind();
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
                DataTable tblOutlet = obj.GetABOutletByCompany(this.hdnCompanyCode.Value).Tables[0];
                if (tblOutlet != null)
                {
                    DataTable tb = CreateFormat();
                    foreach (DataRow row in tblOutlet.Rows)
                    {
                        DataRow rowNew = tb.NewRow();
                        rowNew["OutletCode"] = row["U_WhseCode"];
                        rowNew["OutletName"] = row["U_WhseName"];
                        rowNew["MinAmt"] = 0;
                        rowNew["MaxAmt"] = 0;
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
                    BindCalendar(tb);
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
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["OrderAmountData"] != null)
                {
                    for (int i = 0; i < this.grdItem.Rows.Count; i++)
                    {
                        Label outletCode = (Label)this.grdItem.Rows[i].FindControl("lblOutletCode");
                        TextBox txtMin = (TextBox)this.grdItem.Rows[i].FindControl("txtMinAmt");
                        TextBox txtMax = (TextBox)this.grdItem.Rows[i].FindControl("txtMaxAmt");
                        DataTable tb = (DataTable)Session["OrderAmountData"];
                        if (tb != null)
                        {
                            DataRow[] r = tb.Select("OutletCode = '" + outletCode.Text + "'");
                            if (r.Length > 0)
                            {
                                r[0]["MinAmt"] = txtMin.Text;
                                r[0]["MaxAmt"] = txtMax.Text;
                            }
                        }
                    }

                    this.lblError.Visible = false;
                    MasterData obj = new MasterData();
                    DataSet ds = new DataSet();
                    ds.Tables.Add(((DataTable)Session["OrderAmountData"]).Copy());
                    if (ds != null)
                    {
                        string errMsg = obj.UpdateOutletOrderAmount(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), this.hdnCardCode.Value, this.hdnCardName.Text,
                             this.hdnCompanyCode.Value, this.hdnCompanyName.Text, ds, this.hdnisUpdate.Value, Session[Utils.AppConstants.CompanyCode].ToString());
                        if (errMsg.Length == 0)
                        {
                            this.lblError.Text = "Operation complete successful!";
                            this.hdnisUpdate.Value = "1";
                            LoadOrderAmount();
                        }
                        else
                        {
                            this.lblError.Text = errMsg;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
            this.lblError.Visible = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdItem_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onmouseover", "MouseEvents(this, event)");
                    e.Row.Attributes.Add("onmouseout", "MouseEvents(this, event)");
                    e.Row.Attributes["style"] = "cursor:pointer";
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