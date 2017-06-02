using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class SOList : System.Web.UI.Page
    {
        private int _rowCount = 0;
        private int _checkRow = 0;
        private string _docEntry = string.Empty;
        private string _vendorCode = string.Empty;
        private string _companyCode = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _docEntry = Request.QueryString["PODocEntry"].ToString();
            _vendorCode = Request.QueryString["POVendorCode"].ToString();
            _companyCode = Request.QueryString["CompanyCode"].ToString();
            if (Session[Utils.AppConstants.IsSupperUser] != null && Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y" && Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
            {
                this.btnAccept.Visible = true;

            }
            else
            {
                this.btnAccept.Visible = false;
                this.grdItem.Enabled = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            LoadItem();
            Timer1.Enabled = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void LoadItem()
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet tblItem = null;
                string companyCode = Session[Utils.AppConstants.CompanyCode].ToString();
                if (Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y")
                {
                    companyCode = _companyCode;
                }
                tblItem = obj.GetSOData(companyCode, _docEntry);
                if (tblItem != null && tblItem.Tables.Count > 0)
                {
                    _rowCount = tblItem.Tables[0].Rows.Count;
                    this.grdItem.DataSource = tblItem;
                    this.grdItem.DataBind();
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
                MasterData obj = new MasterData();
                DataSet dsInternal = obj.GetInternalPO(_vendorCode);
                if (dsInternal != null && dsInternal.Tables.Count > 0 && dsInternal.Tables[0].Rows.Count > 0)
                {
                    //Get sister database
                    string sisterDatabase = dsInternal.Tables[0].Rows[0]["U_DBName"].ToString();
                    for (int i = 0; i < this.grdItem.Rows.Count; i++)
                    {
                        CheckBox checkBox = (CheckBox)this.grdItem.Rows[i].FindControl("chkChild");
                        Label lblStatus = (Label)this.grdItem.Rows[i].FindControl("lblStatus");
                        if (checkBox != null)
                        {
                            if (checkBox.Checked && lblStatus.Text != Utils.AppConstants.SOStatus.Done.ToString()
                                && lblStatus.Text != Utils.AppConstants.SOStatus.Pass.ToString())
                            {
                                string companyCode = Session[Utils.AppConstants.CompanyCode].ToString();
                                if (Session[Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y")
                                {
                                    companyCode = _companyCode;
                                }
                                DataSet dsSO = obj.GetSOData(companyCode, _docEntry);
                                if (dsSO != null && dsSO.Tables.Count > 1)
                                {

                                    dsSO.Tables["AB_SO_Header"].TableName = "ORDR";
                                    dsSO.Tables["AB_SO_Detail"].TableName = "RDR1";
                                    Transaction trans = new Transaction();
                                    DocumentXML docx = new DocumentXML();
                                    foreach (DataRow rSO in dsSO.Tables["ORDR"].Rows)
                                    {
                                        if (rSO["SOStatus"].ToString() != Utils.AppConstants.SOStatus.Done.ToString()
                                            && rSO["SOStatus"].ToString() != Utils.AppConstants.SOStatus.Pass.ToString())
                                        {
                                            DataSet dsUpdate = new DataSet();

                                            DataTable tbH = dsSO.Tables["ORDR"].Clone();
                                            tbH.ImportRow(rSO);

                                            DataTable tbD = dsSO.Tables["RDR1"].Clone();
                                            foreach (DataRow r in dsSO.Tables["RDR1"].Rows)
                                            {
                                                if (r["HeaderID"].ToString() == rSO["ID"].ToString())
                                                {
                                                    tbD.ImportRow(r);
                                                }
                                            }

                                            tbH.Columns.Remove("SOStatus");
                                            tbH.Columns.Remove("ErrMessage");
                                            tbH.Columns.Remove("ID");
                                            tbD.Columns.Remove("ID");
                                            tbD.Columns.Remove("HeaderID");

                                            dsUpdate.Tables.Add(tbH);
                                            dsUpdate.Tables.Add(tbD);

                                            string xmlRequest = docx.ToXMLStringFromDS("17", dsUpdate);
                                            DataSet dsErr = trans.CreateMarketingDocument(xmlRequest, "",
                                                "", sisterDatabase, "17", "", false);
                                            if ((int)dsErr.Tables[0].Rows[0]["ErrCode"] != 0)
                                            {
                                                rSO["SOStatus"] = Utils.AppConstants.SOStatus.Failed;
                                                rSO["ErrMessage"] = dsErr.Tables[0].Rows[0]["ErrMsg"].ToString();
                                                Utils.AppConstants.WriteLog(dsErr.Tables[0].Rows[0]["ErrMsg"].ToString(), true);
                                                Utils.AppConstants.WriteLog(xmlRequest, true);
                                                Utils.AppConstants.WriteLog(Session[Utils.AppConstants.UserCode].ToString(), true);
                                                Utils.AppConstants.WriteLog(Session[Utils.AppConstants.CompanyCode].ToString(), true);
                                            }
                                            else
                                            {
                                                rSO["ErrMessage"] = "Operation complete successful!";
                                                rSO["SOStatus"] = Utils.AppConstants.SOStatus.Pass;
                                            }
                                        }
                                    }
                                    string err = obj.UpdateSO("", "", dsSO, companyCode,false);
                                }
                            }
                        }
                    }
                    LoadItem();
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
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");

                try
                {
                    CheckBox checkBox = (CheckBox)e.Row.FindControl("chkChild");
                    if (checkBox != null)
                    {
                        if (lblStatus.Text == Utils.AppConstants.SOStatus.Done.ToString())
                        {
                            checkBox.Checked = true;
                            checkBox.Enabled = false;
                            _checkRow++;
                        }
                        if (_checkRow == _rowCount)
                        {
                            //Check All
                            GridViewRow headerRow = this.grdItem.HeaderRow;
                            ((CheckBox)headerRow.FindControl("chkheader")).Checked = true;
                            ((CheckBox)headerRow.FindControl("chkheader")).Enabled = false;
                        }
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
}
