using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Suki
{
    public partial class Main : System.Web.UI.MasterPage
    {
        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            Page.EnableViewState = true;
            //this.lnkBack.Attributes.Add("onClick", "javascript:history.back(); return false;");
            if (Session[Utils.AppConstants.UserCode] != null
                && Session[Utils.AppConstants.CompanyName] != null
                && Session[Utils.AppConstants.CompanyCode] != null
                 && Session[Utils.AppConstants.OutletCode] != null
                   && Session[Utils.AppConstants.OutletName] != null
                  && Session[Utils.AppConstants.IsSupperUser] != null
                 && Session[Utils.AppConstants.Pwd] != null)
            {
                this.lblUser.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
                if (Session[Utils.AppConstants.IsSupperUser].ToString() == "N"
                    && Session[Utils.AppConstants.IsCompanySuperUser].ToString() == "N")
                {
                   // this.lblOutlet.Text = Session[Utils.AppConstants.CompanyName].ToString() + ", Outlet: " + Session[Utils.AppConstants.OutletName].ToString();
                    string outlet = string.Empty;
                    if (Session[Utils.AppConstants.ListOutlet] != null)
                    {
                        Dictionary<string, string> dicOutlet = (Dictionary<string, string>)Session[Utils.AppConstants.ListOutlet];
                        foreach (KeyValuePair<string, string> item in dicOutlet)
                        {
                            outlet += string.Format("{0} - {1} ,", item.Key, item.Value);
                        }
                    }
                    //this.lblCompany.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper() + " | " + Session[Utils.AppConstants.CompanyName].ToString() + " | " + Session[Utils.AppConstants.OutletCode].ToString() + " - " + Session[Utils.AppConstants.OutletName].ToString();
                    if (outlet.Length > 0)
                    {
                        this.lblOutlet.Text = Session[Utils.AppConstants.CompanyName].ToString() + ", Outlet: " + outlet.Substring(0, outlet.Length - 1);
                    }
                }
                else
                {
                    this.lblOutlet.Text = Session[Utils.AppConstants.CompanyName].ToString();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        /// <summary>
        /// Logout Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            try
            {
                GetDefault obj = new GetDefault();
                obj.LogOut();
                Session.Abandon();
                Response.Redirect("Login.aspx");
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Home
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void lnkBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
    }
}