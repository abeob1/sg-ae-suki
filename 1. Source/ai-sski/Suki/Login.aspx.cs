using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class Login : System.Web.UI.Page
    {
        MasterData obj = new MasterData();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCompany();
                if (Request.QueryString["Logout"] != null)
                {
                   
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadOutlet()
        {
            try
            {
                DataSet ds = obj.GetAllCompany();
                if (ds != null)
                {
                    this.drdOutlet.DataSource = ds;
                    this.drdOutlet.DataTextField = "cmpName";
                    this.drdOutlet.DataValueField = "dbName";
                    this.drdOutlet.DataBind();
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadCompany()
        {
            try
            {

                DataSet ds = obj.GetAllABCompany();
                if (ds != null && ds.Tables.Count > 0)
                {
                    this.drdOutlet.DataSource = ds;
                    this.drdOutlet.DataTextField = "U_CompanyName";
                    this.drdOutlet.DataValueField = "U_DBName";
                    this.drdOutlet.DataBind();
                }
                else
                {
                    this.lblMessage.Text = "Cannot connect to SUKI web service. Pls try again.";
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = "Cannot connect to SUKI web service. Pls try again.";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet dsUser = obj.GetLogin(this.txtUserName.Text.Trim(), this.txtPassword.Text.Trim(), this.drdOutlet.SelectedValue.ToString());
                if (dsUser != null && dsUser.Tables.Count > 0 && dsUser.Tables[0].Rows.Count > 0)
                {
                    bool hasAccess = false;
                    string isSuperUser = dsUser.Tables[0].Rows[0]["U_SUPERUSER"].ToString();
                     string isCompanySuperUser = dsUser.Tables[0].Rows[0]["U_ComSS"].ToString();
                    string outlet = dsUser.Tables[0].Rows[0]["U_Outlet"].ToString();
                    string company = dsUser.Tables[0].Rows[0]["U_ComCode"].ToString();
                    string outletName = string.Empty;
                    string outletCode = string.Empty;
                    string[] lstOutlet = null;
                    string[] lstCompany = null;
                    Dictionary<string, string> ownOutlet = new Dictionary<string, string>();
                    Dictionary<string, string> ownCompany = new Dictionary<string, string>();
                    if (outlet.Length > 0)
                    {
                        lstOutlet = outlet.Split(',');
                    }
                    if (company.Length > 0)
                    {
                        lstCompany = company.Split(',');
                    }
                    if (isSuperUser.ToUpper() == "N")
                    {
                        if (isCompanySuperUser.ToUpper() == "N" || isCompanySuperUser.Length == 0)
                        {
                            if (lstCompany != null && lstOutlet != null)
                            {
                                foreach (string comp in lstCompany)
                                {
                                    foreach (string outl in lstOutlet)
                                    {
                                        DataSet dsData = obj.CheckPermissonDB(outl, comp, this.drdOutlet.SelectedValue.ToString());
                                        if (dsData != null && dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                                        {
                                            if (!ownOutlet.ContainsKey(dsData.Tables[0].Rows[0]["U_OutletID"].ToString()))
                                            {
                                                ownOutlet.Add(dsData.Tables[0].Rows[0]["U_OutletID"].ToString(), dsData.Tables[0].Rows[0]["U_WhseName"].ToString());
                                                hasAccess = true;
                                            }
                                        }
                                    }
                                    DataSet dsDataCom = obj.CheckPermissonDB(null, comp, this.drdOutlet.SelectedValue.ToString());
                                    if (dsDataCom != null && dsDataCom.Tables.Count > 0 && dsDataCom.Tables[0].Rows.Count > 0)
                                    {
                                        if (!ownCompany.ContainsKey(dsDataCom.Tables[0].Rows[0]["U_CompanyCode"].ToString()))
                                        {
                                            ownCompany.Add(dsDataCom.Tables[0].Rows[0]["U_CompanyCode"].ToString(), dsDataCom.Tables[0].Rows[0]["U_DBName"].ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (string outl in lstOutlet)
                                {
                                    DataSet dsData = obj.CheckPermissonDB(outl, string.Empty, this.drdOutlet.SelectedValue.ToString());
                                    if (dsData != null && dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                                    {
                                        if (!ownOutlet.ContainsKey(dsData.Tables[0].Rows[0]["U_OutletID"].ToString()))
                                        {
                                            ownOutlet.Add(dsData.Tables[0].Rows[0]["U_OutletID"].ToString(), dsData.Tables[0].Rows[0]["U_WhseName"].ToString());
                                            hasAccess = true;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (lstCompany != null)
                            {
                                foreach (string comp in lstCompany)
                                {
                                    DataSet dsData = obj.CheckPermissonDB(null, comp, this.drdOutlet.SelectedValue.ToString());
                                    if (dsData != null && dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                                    {
                                        if (!ownCompany.ContainsKey(dsData.Tables[0].Rows[0]["U_CompanyCode"].ToString()))
                                        {
                                            ownCompany.Add(dsData.Tables[0].Rows[0]["U_CompanyCode"].ToString(), dsData.Tables[0].Rows[0]["U_DBName"].ToString());
                                            hasAccess = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (lstCompany != null)
                        {
                            foreach (string comp in lstCompany)
                            {
                                DataSet ds = obj.GetCompanyInfo(comp);
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    if (!ownCompany.ContainsKey(ds.Tables[0].Rows[0]["U_CompanyCode"].ToString()))
                                    {
                                        ownCompany.Add(ds.Tables[0].Rows[0]["U_CompanyCode"].ToString(), ds.Tables[0].Rows[0]["U_DBName"].ToString());
                                    }
                                }
                            }
                        }
                        if (lstOutlet != null)
                        {
                            foreach (string outl in lstOutlet)
                            {
                                if (!ownOutlet.ContainsKey(outl))
                                {
                                    ownOutlet.Add(outl, outl);
                                }
                            }
                        }
                        if (this.drdOutlet.SelectedValue.ToString() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString())
                        {
                            hasAccess = true;
                        }
                    }
                    if (hasAccess == false)
                    {
                        this.lblMessage.Text = "You do not have permission to access " + this.drdOutlet.SelectedItem.Text + " database.";
                        this.lblMessage.Visible = true;
                        return;
                    }
                    Session[Utils.AppConstants.IsCompanySuperUser] = dsUser.Tables[0].Rows[0]["U_ComSS"];
                    Session[Utils.AppConstants.ListCompany] = ownCompany;
                    Session[Utils.AppConstants.ListOutlet] = ownOutlet;

                    Session[Utils.AppConstants.IsSupperUser] = dsUser.Tables[0].Rows[0]["U_SUPERUSER"];
                    Session[Utils.AppConstants.OutletCode] = outletCode;
                    Session[Utils.AppConstants.OutletName] = outletName;
                    Session[Utils.AppConstants.UserCode] = this.txtUserName.Text.Trim();
                    Session[Utils.AppConstants.CompanyCode] = this.drdOutlet.SelectedValue;
                    Session[Utils.AppConstants.CompanyName] = this.drdOutlet.SelectedItem.Text;
                    Session[Utils.AppConstants.Pwd] = this.txtPassword.Text.Trim();

                   Response.Redirect("Default.aspx");
                    //ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "OpenMain()", true);
                }
                else
                {
                    this.lblMessage.Text = "User ID or Password is incorrect.";
                }
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.Message;
            }
        }
    }
}