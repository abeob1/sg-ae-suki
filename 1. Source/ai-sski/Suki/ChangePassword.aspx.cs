using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            MasterData obj = new MasterData();
            if (this.txtOldPass.Text.Trim() == string.Empty)
            {
                this.lblError.Text = "Input old password.";
                this.lblError.Visible = true;
                return;
            }
            if (this.txtNewPass.Text.Trim() == string.Empty)
            {
                this.lblError.Text = "Input new password.";
                this.lblError.Visible = true;
                return;
            }
            if (this.txtConfirmPass.Text.Trim() == string.Empty)
            {
                this.lblError.Text = "Input comfirm password.";
                this.lblError.Visible = true;
                return;
            }
            if (this.txtConfirmPass.Text.Trim() != this.txtNewPass.Text.Trim())
            {
                this.lblError.Text = "Password does not match the confirm password.";
                this.lblError.Visible = true;
                return;
            }
            DataSet dsUser = obj.GetLogin(Session[Utils.AppConstants.UserCode].ToString(), this.txtOldPass.Text, "");
            if (dsUser != null && dsUser.Tables.Count > 0 && dsUser.Tables[0].Rows.Count == 0)
            {
                this.lblError.Text = "Old Password is incorrect.";
                this.lblError.Visible = true;
                return;
            }
            else
            {
                string count = obj.ChangePassword(Session[Utils.AppConstants.UserCode].ToString(), this.txtConfirmPass.Text);
                if (count != string.Empty)
                {
                    this.txtOldPass.Text = "";
                    this.txtNewPass.Text = "";
                    this.txtConfirmPass.Text = "";
                    this.lblError.Text = "Operation complete successful!";
                    this.lblError.Visible = true;
                }
            }
        }
    }
}