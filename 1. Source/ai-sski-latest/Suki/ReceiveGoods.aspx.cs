using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Suki
{
    public partial class ReceiveGoods : System.Web.UI.Page
    {
        private string _docEntry = string.Empty;
        private string _grpoDocEntry = string.Empty;
        private string _isNew = string.Empty;
        private string _GRPONum = string.Empty;
        private string _company = string.Empty;
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
            //Check Valid Parameter
            _isNew = Request.QueryString["IsNew"];
            _GRPONum = Request.QueryString["GRNNo"];
            _company = Request.QueryString["Company"];
            if (_isNew == null || _isNew.Length == 0)
            {
                Response.Redirect("ErrorPage.aspx");
            }
            _docEntry = Request.QueryString["docEntry"];
            _grpoDocEntry = Request.QueryString["GRPODocEntry"];
            if (_isNew.ToUpper() == "N")
            {
                if (_docEntry == null || _docEntry.Length == 0)
                {
                    Response.Redirect("ErrorPage.aspx");
                }
                if (!Utils.AppConstants.isInteger(_docEntry))
                {
                    Response.Redirect("ErrorPage.aspx");
                }
            }
            else if (_isNew.ToUpper() == "E")
            {
                this.btnSave.Text = "Update";
                if (_grpoDocEntry == null || _grpoDocEntry.Length == 0)
                {
                    Response.Redirect("ErrorPage.aspx");
                }
                if (!Utils.AppConstants.isInteger(_grpoDocEntry))
                {
                    Response.Redirect("ErrorPage.aspx");
                }
                this.grvPO.Columns[7].Visible = false;
                this.grvPO.Columns[9].Visible = false;
                this.txtGRDate.Enabled = false;
                this.Image1.Visible = false;
            }
            else
            {
                Response.Redirect("ErrorPage.aspx");
            }
            if (!IsPostBack)
            {
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
                        && numSession != "CompanyHistoryPOGRPO"
                        && numSession != "OutletPOGRPO"
                         && numSession != "CompanyHistoryPOOut"
                        && numSession != "OutletPOOut"
                         && numSession != "PageIndex")
                    {
                        Session[numSession] = null;
                    }
                }
                if (_isNew.ToUpper() == "N")
                {
                    LoadData(_docEntry, _isNew.ToUpper());
                }
                else
                {
                    this.grvPO.Columns[0].Visible = false;
                    LoadData(_grpoDocEntry, _isNew.ToUpper());
                   
                }
            }
         
            this.txtIssuedBy.Enabled = false;
            if (_isNew.ToUpper() == "E")
            {
                for (int i = 0; i < this.grvPO.Rows.Count; i++)
                {
                    TextBox txtReceiQty = (TextBox)this.grvPO.Rows[i].FindControl("txtReceivedQty");
                    TextBox txtRemarks = (TextBox)this.grvPO.Rows[i].FindControl("txtRemarks");
                    txtReceiQty.Enabled = false;
                    txtRemarks.Enabled = false;
                }
            }
            if (Request.QueryString["Mess"] != null && Request.QueryString["Mess"].ToString() == "T")
            {
                this.lblError.Text = "Operation complete successful!";
                this.lblError.Visible = true;
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
                TextBox ddl1 = e.Row.FindControl("txtReceivedQty") as TextBox;
                ddl1.Attributes.Add("onfocus", "JavaScript:document.getElementById('" + ddl1.ClientID + "').select();");
                int index = 0;
                if (ViewState["rowindex"] != null)
                {
                    index = Convert.ToInt32(ViewState["rowindex"].ToString());
                    if ((index + 1) <= this.grvPO.Rows.Count)
                    {
                        if (e.Row.RowIndex == index + 1)
                        {
                            ddl1.Focus();
                            //ScriptManager.GetCurrent(this).SetFocus(ddl1.ClientID);
                        }
                    }
                }
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
                GridViewRow row = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                Label lblItemCode = (Label)row.FindControl("lblItemCode");
                DataTable tb = (DataTable)Session["POTable"];
                if (tb != null)
                {
                    DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                    if (rupdate.Length > 0)
                    {
                        Dictionary<string, string> dicItem = (Dictionary<string, string>)Session["ChosenItem"];
                        if (dicItem != null)
                        {
                            dicItem.Remove(rupdate[0]["ItemCode"].ToString());
                        }
                        tb.Rows.Remove(rupdate[0]);
                    }
                }
                this.grvPO.EditIndex = -1;

                BindData((DataTable)Session["POTable"]);
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
        protected void EditItem(object sender, GridViewEditEventArgs e)
        {
            this.grvPO.EditIndex = e.NewEditIndex;
            BindData((DataTable)Session["POTable"]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelEdit(object sender, GridViewCancelEditEventArgs e)
        {
            e.Cancel = true;
            this.grvPO.EditIndex = -1;
            BindData((DataTable)Session["POTable"]);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DeleteItem(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
            GridViewRow row = (GridViewRow)this.grvPO.Rows[e.RowIndex];
            Label lblItemCode = (Label)row.FindControl("lblItemCode");
            DataTable tb = (DataTable)Session["POTable"];
            if (tb != null)
            {
                DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                if (rupdate.Length > 0)
                {
                    Dictionary<string, string> dicItem = (Dictionary<string, string>)Session["ChosenItem"];
                    if (dicItem != null)
                    {
                        dicItem.Remove(rupdate[0]["ItemCode"].ToString());
                    }
                    tb.Rows.Remove(rupdate[0]);
                }
            }
            this.grvPO.EditIndex = -1;

            BindData((DataTable)Session["POTable"]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UpdateItem(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridViewRow row = (GridViewRow)this.grvPO.Rows[e.RowIndex];
                TextBox txtReceivedQty = (TextBox)row.FindControl("txtReceivedQty");
                if (txtReceivedQty.Text.Trim().Length == 0 || int.Parse(txtReceivedQty.Text.Trim()) == 0)
                {
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert('In Recevied Quantity column, enter whole number greater than 0.');", true);
                    txtReceivedQty.Focus();
                }
                Label lblItemCode = (Label)row.FindControl("lblItemCode");
                Label lblQuantity = (Label)row.FindControl("lblQuantity");
                TextBox txtRemarks = (TextBox)row.FindControl("txtRemarks");
                Label lblPrice = (Label)row.FindControl("lblPrice");
                DataTable tb = (DataTable)Session["POTable"];
                if (tb != null)
                {
                    DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                    if (rupdate.Length > 0)
                    {
                        rupdate[0]["ReceivedQty"] = txtReceivedQty.Text;
                        rupdate[0]["BalanceQty"] = double.Parse(lblQuantity.Text) - double.Parse(txtReceivedQty.Text);
                        rupdate[0]["LineTotal"] = double.Parse(txtReceivedQty.Text) * double.Parse(lblPrice.Text);
                        rupdate[0]["Remarks"] = txtRemarks.Text;
                    }
                    this.grvPO.EditIndex = -1;
                    BindData(tb);
                    CalcTotal(tb);
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
        protected void lblReceivedQty_OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow row = (GridViewRow)((DataControlFieldCell)((TextBox)sender).Parent).Parent;
                ViewState["rowindex"] = row.RowIndex;
                TextBox txtReceivedQty = (TextBox)row.FindControl("txtReceivedQty");
                if (txtReceivedQty.Text.Trim().Length == 0 || double.Parse(txtReceivedQty.Text.Trim()) == 0)
                {
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", "alert('In Recevied Quantity column, enter whole number greater than 0.');", true);
                    txtReceivedQty.Focus();
                }
                Label lblItemCode = (Label)row.FindControl("lblItemCode");
                Label lblQuantity = (Label)row.FindControl("lblQuantity");
                TextBox txtRemarks = (TextBox)row.FindControl("txtRemarks");
                Label lblPrice = (Label)row.FindControl("lblPrice");
                DataTable tb = (DataTable)Session["POTable"];
                if (tb != null)
                {
                    DataRow[] rupdate = tb.Select("ItemCode='" + lblItemCode.Text + "'");
                    if (rupdate.Length > 0)
                    {
                        rupdate[0]["ReceivedQty"] = txtReceivedQty.Text;
                         decimal banalce =  Math.Round(decimal.Parse(lblQuantity.Text) - decimal.Parse(txtReceivedQty.Text), 2) ;
                         if (banalce < 0)
                         {
                             rupdate[0]["BalanceQty"] = "0.0000";
                         }
                         else
                         {
                             rupdate[0]["BalanceQty"] = banalce.ToString(Utils.AppConstants.NUMBER_FORMAT);
                         }
                        rupdate[0]["LineTotal"] = Math.Round(decimal.Parse(txtReceivedQty.Text) * decimal.Parse(lblPrice.Text), 2);
                        rupdate[0]["Remarks"] = txtRemarks.Text;
                    }
                    this.grvPO.EditIndex = -1;
                    BindData(tb);
                    CalcTotal(tb);
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
        /// <param name="tb"></param>
        private void CalcTotal(DataTable tb)
        {
            try
            {
                double documentTotal = 0;
                double gstTotal = 0;
                double subTotal = 0;
                foreach (DataRow row in tb.Rows)
                {
                    subTotal += double.Parse(row["LineTotal"].ToString());
                    if (row["Rate"] != null && row["Rate"].ToString().Length > 0)
                    {
                        gstTotal += (double.Parse(row["LineTotal"].ToString()) * double.Parse(row["Rate"].ToString())) / 100;
                    }
                    else
                    {
                        gstTotal += (double.Parse(row["LineTotal"].ToString()) * 8.25) / 100;
                    }
                }
                documentTotal += subTotal + gstTotal;
                this.lblDocumentTotal.Text = string.Format("${0}", documentTotal.ToString(Utils.AppConstants.NUMBER_FORMAT));
                this.lblGSTAmount.Text = string.Format("${0}", gstTotal.ToString(Utils.AppConstants.NUMBER_FORMAT));
                this.lblSubTotal.Text = string.Format("${0}", subTotal.ToString(Utils.AppConstants.NUMBER_FORMAT));
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
        private DataTable CreateTabelFormat()
        {
            DataTable tbTemp = new DataTable();
            tbTemp.Columns.Add("No");
            tbTemp.Columns.Add("ItemCode");
            tbTemp.Columns.Add("Dscription");
            tbTemp.Columns.Add("BalanceQty");
            tbTemp.Columns.Add("BuyUnitMsr");
            tbTemp.Columns.Add("Quantity");
            tbTemp.Columns.Add("ReceivedQty");
            tbTemp.Columns.Add("Price");
            tbTemp.Columns.Add("LineTotal");
            tbTemp.Columns.Add("LineNum");
            tbTemp.Columns.Add("Remarks");
            tbTemp.Columns.Add("WhsCode");
            tbTemp.Columns.Add("VatgroupPu");
            tbTemp.Columns.Add("Rate");
            tbTemp.Columns.Add("ManBtchNum");
            tbTemp.Columns.Add("DocNum");

            return tbTemp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="docEntry"></param>
        private void LoadData(string docEntry, string isNew)
        {
            try
            {
                MasterData obj = new MasterData();
                DataSet ds = new DataSet();
                if (isNew == "N")
                {
                    ds = obj.CopyToGRPO(Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                           _company, docEntry);
                }
                else
                {
                    ds = obj.GetGRPOUpdate(_company, _grpoDocEntry);
                }
                if (ds != null && ds.Tables.Count > 1)
                {
                    string docNum = string.Empty;
                    foreach (DataRow row in ds.Tables["OPOR"].Rows)
                    {
                        docNum = row["DocNum"].ToString();
                        this.txtCardName.Text = row["CardName"].ToString();
                        this.lblCardCode.Value = row["CardCode"].ToString();
                        this.lblCardName.Value = row["CardName"].ToString();
                        this.lblDocEntry.Value = row["DocEntry"].ToString();
                        if (row["U_AB_UserCode"].ToString().Length > 0 && row["U_AB_UserCode"].ToString() != "-1" && isNew == "E")
                        {
                            this.txtIssuedBy.Text = row["U_AB_UserCode"].ToString().ToUpper();
                        }
                        else
                        {
                            this.txtIssuedBy.Text = Session[Utils.AppConstants.UserCode].ToString().ToUpper();
                        }
                        if (isNew == "E")
                        {
                            this.txtGRDate.Text = Convert.ToDateTime(row["DocDate"].ToString()).ToString(Utils.AppConstants.DATE);
                            string[] NumAtCard = row["NumAtCard"].ToString().Split('|');
                            if (NumAtCard.Length >= 1)
                            {
                                this.txtSuppDONo.Text = NumAtCard[0].ToString().Trim();
                            }
                            if (NumAtCard.Length > 1)
                            {
                                this.txtSuppInvNo.Text = NumAtCard[1].ToString().Trim();
                            }
                            this.lblGRNNo.Text = _GRPONum;
                        }
                        else
                        {
                            this.txtSuppDONo.Text = row["NumAtCard"].ToString();
                            this.txtSuppInvNo.Text = row["NumAtCard"].ToString();
                        }

                        this.txtRemarks.Text = row["Footer"].ToString();
                        this.txtAddress.Text = row["Address"].ToString();
                        if (row["DocStatus"].ToString().ToUpper() == "O")
                        {
                            this.txtStatus.Text = "Open";
                        }
                        else if (row["DocStatus"].ToString().ToUpper() == "C")
                        {
                            this.txtStatus.Text = "Closed";
                        }
                        this.lblDocumentTotal.Text = string.Format("${0}", decimal.Parse(row["DocTotal"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT));
                        this.lblGSTAmount.Text = string.Format("${0}", decimal.Parse(row["VATSum"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT));
                        this.lblSubTotal.Text = string.Format("${0}", decimal.Parse(row["SubTotal"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT));
                    }
                    DataTable tbItem = CreateTabelFormat();
                    int i = 1;
                    foreach (DataRow row in ds.Tables["POR1"].Rows)
                    {
                        DataRow rowNew = tbItem.NewRow();
                        rowNew["No"] = i;
                        rowNew["ItemCode"] = row["ItemCode"];
                        rowNew["Dscription"] = row["Dscription"];
                        rowNew["Price"] = decimal.Parse(row["Price"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                        if (isNew == "N")
                        {
                            rowNew["Quantity"] = decimal.Parse(row["Quantity"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                            rowNew["BalanceQty"] = "0.0000";
                        }
                        else
                        {
                            rowNew["Quantity"] = decimal.Parse(row["POQty"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                            rowNew["BalanceQty"] = (decimal.Parse(row["POQty"].ToString()) - decimal.Parse(row["Quantity"].ToString())).ToString(Utils.AppConstants.NUMBER_FORMAT);
                        }

                        rowNew["BuyUnitMsr"] = row["BuyUnitMsr"];
                        rowNew["LineTotal"] = row["LineTotal"].ToString().Length > 0 ? decimal.Parse(row["LineTotal"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT) : "0.0000";
                        if (isNew == "N")
                        {
                            if (row["LineStatus"].ToString().ToUpper() == "O")
                            {
                                rowNew["Quantity"] = decimal.Parse(row["OpenQty"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                rowNew["ReceivedQty"] = decimal.Parse(row["OpenQty"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                                rowNew["BalanceQty"] = (decimal.Parse(rowNew["Quantity"].ToString()) - decimal.Parse(rowNew["ReceivedQty"].ToString())).ToString(Utils.AppConstants.NUMBER_FORMAT);
                            }
                        }
                        else
                        {
                            rowNew["ReceivedQty"] = decimal.Parse(row["Quantity"].ToString()).ToString(Utils.AppConstants.NUMBER_FORMAT);
                        }
                        rowNew["LineNum"] = row["LineNum"];
                        rowNew["WhsCode"] = row["WhsCode"];
                        rowNew["ManBtchNum"] = row["ManBtchNum"];
                        //rowNew["DocNum"] = docNum;
                        if (isNew == "N")
                        {
                            rowNew["DocNum"] = Request.QueryString["PoNo"].ToString();
                        }
                        else
                        {
                            this.grvPO.Columns[5].Visible = false;
                        }
                        tbItem.Rows.Add(rowNew);
                        i++;
                        Session[row["ItemCode"] + "_" + row["LineNum"] + "_BatchData"] = null;
                    }
                    BindData(tbItem);
                }
                else
                {
                    DataTable tbItem = CreateTabelFormat();
                    BindData(tbItem);
                    this.lblError.Text = "Can not get data. System is processing another request. Pls try again later.";
                    this.lblError.Visible = true;
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
        protected void lnkBatch_Click(object sender, EventArgs e)
        {
            try
            {
                GridViewRow gr = (GridViewRow)((DataControlFieldCell)((LinkButton)sender).Parent).Parent;
                Label lblItemCode = (Label)gr.FindControl("lblItemCode");
                Label lblDscription = (Label)gr.FindControl("txtItemName");
                TextBox txtReceivedQty = (TextBox)gr.FindControl("txtReceivedQty");
                HiddenField lineNum = (HiddenField)gr.FindControl("hdnLineNum");
                HiddenField ManBtchNum = (HiddenField)gr.FindControl("hdnManBtchNum");
                if (ManBtchNum.Value.ToUpper() == "Y")
                {
                    string popup = "OpenBatch('" + lblItemCode.Text + "','" + lblDscription.Text + "','" + lineNum.Value + "','" + txtReceivedQty.Text + "','" + _isNew + "')";
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "CallJS", popup, true);
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
        /// <param name="tblData"></param>
        private void BindData(DataTable tblData)
        {
            Session["POTable"] = tblData;
            this.grvPO.DataSource = tblData;
            this.grvPO.DataBind();
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
                if(this.txtGRDate.Text.Trim().Length == 0)
                {
                    this.lblError.Text = "Please enter Goods Receipt Date";
                    this.lblError.Visible = true;
                    this.txtGRDate.Focus();
                    return;
                }
                if (this.txtSuppDONo.Text.Trim().Length == 0)
                {
                    this.lblError.Text = "Please enter Supplier DO No";
                    this.lblError.Visible = true;
                    this.txtSuppDONo.Focus();
                    return;
                }
                this.lblError.Visible = false;
                DataSet ds = new DataSet();
                DataTable tblHeader = new DataTable("OPDN");
                tblHeader.Columns.Add("CardCode");
                tblHeader.Columns.Add("CardName");
                tblHeader.Columns.Add("DocDate");
                tblHeader.Columns.Add("DocDueDate");
                tblHeader.Columns.Add("Comments");
                tblHeader.Columns.Add("NumAtCard");
                tblHeader.Columns.Add("SlpCode");
                tblHeader.Columns.Add("Address");
                //tblHeader.Columns.Add("DocEntry");
                tblHeader.Columns.Add("Footer");

                tblHeader.Columns.Add("U_AB_UserCode");

                DataRow row = tblHeader.NewRow();
                row["CardCode"] = this.lblCardCode.Value;
                row["CardName"] = this.lblCardName.Value;
                //row["DocEntry"] = this.lblDocEntry.Value;
                row["NumAtCard"] = this.txtSuppDONo.Text + " | " + this.txtSuppInvNo.Text;
                MasterData obj = new MasterData();
                DataSet dsCheck = obj.CheckSlpName(Session[Utils.AppConstants.CompanyCode].ToString(), this.txtIssuedBy.Text);
                if (dsCheck != null && dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0)
                {
                    row["SlpCode"] = dsCheck.Tables[0].Rows[0]["SlpCode"].ToString();
                }
                else
                {
                    row["SlpCode"] = "";
                }
                row["Address"] = this.txtAddress.Text;
                row["Footer"] = this.txtRemarks.Text;
                if (_isNew.ToUpper() == "N")
                {
                    row["DocDueDate"] = Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd");
                    row["DocDate"] = Utils.AppConstants.ConvertToDate(this.txtGRDate.Text.Trim()).ToString("yyyyMMdd");
                }
                row["Comments"] = "Based On Purchase Orders " + _docEntry + ".";

                row["U_AB_UserCode"] = this.txtIssuedBy.Text;

                tblHeader.Rows.Add(row);

                DataTable tblDetail = new DataTable("PDN1");
                tblDetail.Columns.Add("BaseEntry");

                tblDetail.Columns.Add("BaseType");
                tblDetail.Columns.Add("BaseLine");
                tblDetail.Columns.Add("ItemCode");
                tblDetail.Columns.Add("Dscription");
                tblDetail.Columns.Add("Quantity");
                //tblDetail.Columns.Add("LineTotal");
                tblDetail.Columns.Add("PriceBefDi");
                tblDetail.Columns.Add("WhsCode");
                tblDetail.Columns.Add("VatgroupPu");
                //tblDetail.Columns.Add("BaseRef");
                //tblDetail.Columns.Add("LineNum");

                DataTable tbBatch = new DataTable("BTNT");
                tbBatch.Columns.Add("DocLineNum");
                tbBatch.Columns.Add("DistNumber");
                tbBatch.Columns.Add("Quantity");

                DataTable tb = ((DataTable)Session["POTable"]);

                foreach (DataRow r in tb.Rows)
                {
                    DataTable tbSessBatch = (DataTable)Session[r["ItemCode"] + "_" + r["LineNum"] + "_BatchData"];
                    double RecQty = 0;
                    if (r["ManBtchNum"].ToString().ToUpper() == "Y")
                    {
                        if (tbSessBatch != null)
                        {
                            foreach (DataRow rS in tbSessBatch.Rows)
                            {
                                DataRow rBatch = tbBatch.NewRow();
                                rBatch["DocLineNum"] = rS["LineNum"];
                                rBatch["DistNumber"] = rS["BatchNo"];
                                rBatch["Quantity"] = rS["Quantity"];
                                RecQty += double.Parse(rBatch["Quantity"].ToString());
                                tbBatch.Rows.Add(rBatch);
                            }
                        }
                        else
                        {
                            this.lblError.Text = r["ItemCode"] + "- Please input batch.";
                            this.lblError.Visible = true;
                            return;
                        }
                        if (RecQty != double.Parse(r["ReceivedQty"].ToString()))
                        {
                            this.lblError.Text = "Item :" + r["ItemCode"] + "-Batch Quantity doesn't match Received Qty";
                            this.lblError.Visible = true;
                            return;
                        }
                    }

                    DataRow rowNew = tblDetail.NewRow();
                    rowNew["ItemCode"] = r["ItemCode"];
                    rowNew["Quantity"] = r["ReceivedQty"];
                    rowNew["Dscription"] = r["Dscription"];

                    rowNew["BaseEntry"] = this.lblDocEntry.Value;
                    rowNew["BaseType"] = 22;
                    rowNew["BaseLine"] = r["LineNum"];
                    //rowNew["LineNum"] = r["LineNum"];
                    rowNew["PriceBefDi"] = r["Price"];
                    rowNew["WhsCode"] = r["WhsCode"];
                    rowNew["VatgroupPu"] = r["VatgroupPu"];

                    tblDetail.Rows.Add(rowNew);
                }

                ds.Tables.Add(tblHeader);
                if (_isNew.ToUpper() == "N")
                {
                    ds.Tables.Add(tblDetail);
                    ds.Tables.Add(tbBatch);
                }

                DocumentXML objInfo = new DocumentXML();
                string requestXML = objInfo.ToXMLStringFromDS("20", ds);
                if (requestXML.Length == 0)
                {
                    this.lblError.Text = "XML Error. Pls try again.";
                }
                else
                {
                    Transaction ts = new Transaction();
                    DataSet dsUpdate = new DataSet();
                    if (_isNew.ToUpper() == "N")
                    {
                        dsUpdate = ts.CreateMarketingDocument(requestXML, Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                           _company, "20", "", false);
                    }
                    else
                    {
                        dsUpdate = ts.CreateMarketingDocument(requestXML, Session[Utils.AppConstants.UserCode].ToString(), Session[Utils.AppConstants.Pwd].ToString(),
                                           _company, "20", _grpoDocEntry, true);
                    }
                    if (dsUpdate != null && dsUpdate.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        if ((int)dsUpdate.Tables[0].Rows[0]["ErrCode"] != 0)
                        {
                            this.lblError.Text = dsUpdate.Tables[0].Rows[0]["ErrMsg"].ToString();
                            Utils.AppConstants.WriteLog(dsUpdate.Tables[0].Rows[0]["ErrMsg"].ToString(), true);
                            Utils.AppConstants.WriteLog(requestXML, true);
                            Utils.AppConstants.WriteLog(Session[Utils.AppConstants.UserCode].ToString(), true);
                            Utils.AppConstants.WriteLog(Session[Utils.AppConstants.CompanyCode].ToString(), true);
                        }
                        else
                        {
                            this.lblError.Text = "Operation complete successful!";
                            if (_isNew.ToUpper() == "N")
                            {
                                string[] docEntry = dsUpdate.Tables[0].Rows[0]["ErrMsg"].ToString().Split('-');
                                DataSet dsGRPO = obj.GetGRPO(_company, docEntry[1]);
                                if (dsGRPO != null && dsGRPO.Tables.Count > 0 && dsGRPO.Tables[0].Rows.Count > 0)
                                {
                                    Response.Redirect("ReceiveGoods.aspx?IsNew=E&GRPODocEntry=" + docEntry[1] + "&GRNNo=" + dsGRPO.Tables[0].Rows[0]["DocNum"].ToString() + "&Company=" + _company + "&Mess=T");
                                }
                            }
                        }
                      
                    }
                    else
                    {
                        this.lblError.Text = "System is processing another request. Pls try again later.";
                        this.lblError.Visible = true;
                        return;
                    }
                    for (int i = 0; i < this.grvPO.Rows.Count; i++)
                    {
                        TextBox txtReceiQty = (TextBox)this.grvPO.Rows[i].FindControl("txtReceivedQty");
                        TextBox txtRemarks = (TextBox)this.grvPO.Rows[i].FindControl("txtRemarks");
                        txtReceiQty.Enabled = false;
                        txtRemarks.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                this.lblError.Text = ex.Message;
            }
            this.lblError.Visible = true;
        }
        /// </summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvPO_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (_isNew.ToUpper() == "E")
                {
                    TextBox txtReceiQty = (TextBox)e.Row.FindControl("txtReceivedQty");
                    TextBox txtRemarks = (TextBox)e.Row.FindControl("txtRemarks");
                    txtReceiQty.Enabled = false;
                    txtRemarks.Enabled = false;
                }
            }
        }
    }
}