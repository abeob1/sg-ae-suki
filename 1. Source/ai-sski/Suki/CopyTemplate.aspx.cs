using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class CopyTemplate : System.Web.UI.Page
    {
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
                LoadCompany();
                this.hdnCompanyCode.Value = Request.QueryString["CompanyCode"];
                this.hdnCompanyName.Text = Request.QueryString["CompanyName"];
                LoadItem();
            }
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
                if (ds != null && ds.Tables.Count > 0)
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
        private DataTable CreateFormat()
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("CompanyCode");
            tb.Columns.Add("OutletCode");
            tb.Columns.Add("OutletName");
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
                        foreach (DataRow row in tblOutlet.Rows)
                        {
                            DataRow rowNew = tb.NewRow();
                            rowNew["OutletCode"] = row["U_WhseCode"];
                            rowNew["OutletName"] = row["U_WhseName"];
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
                LoadItem();
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {

                DataTable tb = new DataTable();
                tb.Columns.Add("OutletCode");
                tb.Columns.Add("NewOutletCode");
                for (int i = 0; i < this.grdItem.Rows.Count; i++)
                {
                    CheckBox checkBox = (CheckBox)this.grdItem.Rows[i].FindControl("chkChild");

                    if (checkBox != null && checkBox.Checked)
                    {
                        TextBox txtNewOutletCode = (TextBox)this.grdItem.Rows[i].FindControl("txtNewCode");
                        Label lblOutletName = (Label)this.grdItem.Rows[i].FindControl("lblOutletName");
                        Label lblOutletCode = (Label)this.grdItem.Rows[i].FindControl("lblOutletCode");

                        DataRow rNew = tb.NewRow();
                        rNew["OutletCode"] = lblOutletCode.Text;
                        rNew["NewOutletCode"] = txtNewOutletCode.Text;
                        tb.Rows.Add(rNew);
                    }
                }
                MasterData obj = new MasterData();
                DataSet ds = new DataSet();
                ds.Tables.Add(tb);
                string errMess = obj.CopyTemplate(this.hdnCompanyCode.Value, this.drdOutlet.SelectedValue.ToString(), this.drdOutlet.SelectedItem.Text, ds);
                this.lblError.Text = errMess == string.Empty ? "Operation complete successful!" : errMess;
                this.lblError.Visible = true;
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
                this.lblError.Visible = true;
            }
        }
    }
}