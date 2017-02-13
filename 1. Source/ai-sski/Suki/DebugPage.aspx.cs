using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class DebugPage : System.Web.UI.Page
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
        protected void btnDebug_Click(object sender, EventArgs e)
        {
            Transaction ts = new Transaction();
            DataSet ds = ts.CreateMarketingDocumentDebug(this.txtXml.Text, this.txtCompany.Text, this.txtObjType.Text, this.txtKey.Text, this.chkIsUpdate.Checked);
            if ((int)ds.Tables[0].Rows[0]["ErrCode"] != 0)
            {
                this.lblError.Text = ds.Tables[0].Rows[0]["ErrMsg"].ToString();
            }
        }
    }
}