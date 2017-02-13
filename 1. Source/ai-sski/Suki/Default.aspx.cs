using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;

namespace Suki
{
    public partial class Default : System.Web.UI.Page
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
                 || Session[Utils.AppConstants.Pwd] == null)
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                if (!IsPostBack)
                {
                    Session["PageIndex"] = null;
                    //Clear Session
                    for (int i = 0; i < Session.Count; i++)
                    {
                        var numSession = Session.Keys[i];
                        if (numSession != Utils.AppConstants.CompanyCode
                            && numSession != Utils.AppConstants.CompanyName
                              && numSession != Utils.AppConstants.IsSupperUser
                              && numSession != Utils.AppConstants.OutletCode
                              && numSession != Utils.AppConstants.OutletName
                            && numSession != Utils.AppConstants.Pwd
                            && numSession != Utils.AppConstants.UserCode
                             && numSession != Utils.AppConstants.IsCompanySuperUser
                             && numSession != Utils.AppConstants.ListCompany
                             && numSession != Utils.AppConstants.ListOutlet
                              && numSession != "WindowName")
                        {
                            Session[numSession] = null;
                        }
                    }
                    if (Session[Utils.AppConstants.IsSupperUser].ToString() == "N")
                    {
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
                            this.lblCompany.Text = string.Format("{0} | {1} | {2}", Session[Utils.AppConstants.UserCode].ToString().ToUpper(),
                                Session[Utils.AppConstants.CompanyName].ToString(),
                                outlet.Substring(0, outlet.Length - 1));
                        }
                        else
                        {
                            this.lblCompany.Text = string.Format("{0} | {1} ", Session[Utils.AppConstants.UserCode].ToString().ToUpper(),
                             Session[Utils.AppConstants.CompanyName].ToString());
                        }
                    }
                    else
                    {
                        this.lblCompany.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper() + " | " + Session[Utils.AppConstants.CompanyName].ToString();
                    }
                    this.lblDate.Text = DateTime.Now.ToString(Utils.AppConstants.DATE);
                    LoadLogo();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadLogo()
        {
            try
            {
                //MasterData obj = new MasterData();
                //DataSet dslogo = obj.GetLogo(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), Session[Utils.AppConstants.CompanyCode].ToString());
                //if (dslogo != null && dslogo.Tables.Count > 0 && dslogo.Tables[0].Rows.Count > 0)
                //{

                //    byte[] logo = (byte[])dslogo.Tables[0].Rows[0]["LogoImage"];

                //    MemoryStream ms = new MemoryStream(logo, 0,
                //   logo.Length);
                //    // Convert byte[] to Image
                //    ms.Write(logo, 0, logo.Length);
                //    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                //    //Save file
                //    string imgPath = "/Images/SAPLogo/Logo.png";
                //    image.Save(Server.MapPath(imgPath));

                //    this.ImgLogo.ImageUrl = imgPath;
                //}
                MasterData obj = new MasterData();
                string logo = obj.GetLogo(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(), Session[Utils.AppConstants.CompanyCode].ToString());
                if (logo.Length > 0)
                {
                    string imgPath = "/Images/SAP_Logo/" + logo;
                    this.ImgLogo.ImageUrl = imgPath;
                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public string Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);
            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            //Save file
            string imgPath = "/Images/SAPLogo/Logo.png";
            image.Save(Server.MapPath(imgPath));
            return imgPath;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLogOut_Click(object sender, EventArgs e)
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
    }
}