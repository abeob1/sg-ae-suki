using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class CalendarOutlet : System.Web.UI.Page
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
                LoadOutletCalendar();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow gr = (GridViewRow)((DataControlFieldCell)((CheckBox)sender).Parent).Parent;
                Label outletCode = (Label)gr.FindControl("lblOutletCode");
                CheckBox mon = (CheckBox)gr.FindControl("chkMon");
                CheckBox tue = (CheckBox)gr.FindControl("chkTue");
                CheckBox wed = (CheckBox)gr.FindControl("chkWed");
                CheckBox thu = (CheckBox)gr.FindControl("chkThu");
                CheckBox fri = (CheckBox)gr.FindControl("chkFri");
                CheckBox sat = (CheckBox)gr.FindControl("chkSat");
                CheckBox sun = (CheckBox)gr.FindControl("chkSun");
                DataTable tb = (DataTable)Session["CalandarData"];
                if (tb != null)
                {
                    DataRow[] r = tb.Select("OutletCode = '" + outletCode.Text + "'");
                    if (r.Length > 0)
                    {
                        r[0]["Mon"] = mon.Checked;
                        r[0]["Tue"] = tue.Checked;
                        r[0]["Wed"] = wed.Checked;
                        r[0]["Thu"] = thu.Checked;
                        r[0]["Fri"] = fri.Checked;
                        r[0]["Sat"] = sat.Checked;
                        r[0]["Sun"] = sun.Checked;
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
        private void LoadOutletCalendar()
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet ds = obj.GetOutletCalendar(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), this.hdnCardCode.Value, this.hdnCompanyCode.Value, Session[Utils.AppConstants.CompanyCode].ToString());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable tblOutlet = obj.GetABOutletByCompany(this.hdnCompanyCode.Value).Tables[0];
                    if (tblOutlet != null)
                    {
                        foreach (DataRow row in tblOutlet.Rows)
                        {
                            DataRow[] rNew = ds.Tables[0].Select("OutletCode='" + row["U_OutletID"].ToString() + "'");
                            if (rNew.Length == 0)
                            {
                                DataRow rowNew = ds.Tables[0].NewRow();
                                rowNew["OutletCode"] = row["U_WhseCode"];
                                rowNew["OutletName"] = row["U_WhseName"];
                                rowNew["Mon"] = "false";
                                rowNew["Tue"] = "false";
                                rowNew["Wed"] = "false";
                                rowNew["Thu"] = "false";
                                rowNew["Fri"] = "false";
                                rowNew["Sat"] = "false";
                                rowNew["Sun"] = "false";
                                if (Session[Utils.AppConstants.IsSupperUser] != null
                                    && Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y"
                                    && Session[Utils.AppConstants.ListOutlet] != null
                                     && ((Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet]).Count > 0)
                                {
                                    Dictionary<string, string> dicOutlet = (Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet];
                                    if (dicOutlet.ContainsKey(row["U_WhseCode"].ToString()))
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
            tb.Columns.Add("Mon");
            tb.Columns.Add("Tue");
            tb.Columns.Add("Wed");
            tb.Columns.Add("Thu");
            tb.Columns.Add("Fri");
            tb.Columns.Add("Sat");
            tb.Columns.Add("Sun");
            return tb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tb"></param>
        private void BindCalendar(DataTable tb)
        {
            Session["CalandarData"] = tb;
            this.grvCalendar.DataSource = tb;
            this.grvCalendar.DataBind();
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
                        rowNew["Mon"] = "false";
                        rowNew["Tue"] = "false";
                        rowNew["Wed"] = "false";
                        rowNew["Thu"] = "false";
                        rowNew["Fri"] = "false";
                        rowNew["Sat"] = "false";
                        rowNew["Sun"] = "false";
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
                if (Session["CalandarData"] != null)
                {
                    this.lblError.Visible = false;
                    MasterData obj = new MasterData();
                    DataSet ds = new DataSet();
                    ds.Tables.Add(((DataTable)Session["CalandarData"]).Copy());
                    if (ds != null)
                    {
                        string errMsg = obj.UpdateOutletCalendar(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), this.hdnCardCode.Value, this.hdnCardName.Text,
                             this.hdnCompanyCode.Value, this.hdnCompanyName.Text, ds, this.hdnisUpdate.Value, Session[Utils.AppConstants.CompanyCode].ToString());
                        if (errMsg.Length == 0)
                        {
                            this.lblError.Text = "Operation complete successful!";
                            this.hdnisUpdate.Value = "1";
                            LoadOutletCalendar();
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