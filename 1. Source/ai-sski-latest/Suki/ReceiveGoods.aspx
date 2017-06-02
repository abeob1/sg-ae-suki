<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="ReceiveGoods.aspx.cs" Inherits="Suki.ReceiveGoods" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <script type="text/javascript">
     function OpenBatch(itemCode,itemName,lineNum,qty,update) {
         var url = "BatchPopup.aspx?LineNum=" + lineNum;
         url += "&ItemCode=" + itemCode;
         url += "&ItemName=" + itemName;
         url += "&Qty=" + qty;
         url += "&IsUpdate=" + update;
         Main.openCustomDialog(url, 800, 610);
     }
    </script>
    <div>
        <h2>
        <asp:Label runat="server" ID ="lblTitle" Text="Receive Goods"></asp:Label>
        <div style="margin-left: 5px; width: 99%;">
            <hr />
            <ajax:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <table border="0" style="width: 100%; background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <td style="width: 100px;">
                                <div style="float: left; margin-top: 3px">
                                    Supplier Name :
                                </div>
                                <div style="float: left;">
                                    &nbsp;</div>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCardName" runat="server" Width="85%" Enabled="False"></asp:TextBox>
                            </td>
                            <td style="width: 80px;">
                                <div style="float: left;">
                                    Goods Receipt Date :
                                </div>
                            </td>
                            <td style="width: 130px">
                                <asp:TextBox ID="txtGRDate" runat="server" Width="90px"></asp:TextBox>
                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd/MM/yyyy" 
                                    PopupButtonID="Image1" TargetControlID="txtGRDate">
                                </cc1:CalendarExtender>
                                <asp:ImageButton ID="Image1" runat="Server" 
                                    AlternateText="Click to show calendar" 
                                    ImageUrl="~/Images/Calendar_scheduleHS.png" />
                            </td>
                            <td style="width: 100px">
                                <div style="white-space: nowrap; float: left; margin-top: 3px">
                                    Received By :
                                </div>
                            </td>
                            <td style="width: 130px">
                                <asp:TextBox ID="txtIssuedBy" runat="server" Enabled="False" Width="90px"></asp:TextBox>
                            </td>
                            <td style="width: 70px">
                                Status :
                            </td>
                            <td>
                                <asp:TextBox ID="txtStatus" runat="server" Width="110px" Enabled="False"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div style="white-space: nowrap; float: left; margin-top: 3px;">
                                    Address :
                                </div>
                                <div style="white-space: nowrap; float: left;">
                                </div>
                            </td>
                            <td rowspan="3" style="width: 250px;" valign="top">
                                <asp:TextBox ID="txtAddress" runat="server" Width="84%" AutoPostBack="true" Height="75px"
                                    TextMode="MultiLine" Style="margin-right: 124px" Enabled="false"></asp:TextBox>
                            </td>
                            <td style="width: 150px">
                                Supplier DO No :
                            </td>
                            <td>
                                <asp:TextBox ID="txtSuppDONo" runat="server" Width="223px"></asp:TextBox>
                            </td>
                            <td>
                                <div style="white-space: nowrap; float: left;">
                                </div>
                                GRN No :</td>
                            <td valign="middle">
                                <asp:Label ID="lblGRNNo" runat="server"></asp:Label>
                            </td>
                            <td>
                                &nbsp;</td>
                            <td>
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                Supplier Invoice No :
                            </td>
                            <td>
                                <asp:TextBox ID="txtSuppInvNo" runat="server" Width="223px"></asp:TextBox>
                            </td>
                            <td>
                                &nbsp;</td>
                            <td>
                                &nbsp;</td>
                            <td>
                                &nbsp;</td>
                            <td>
                                &nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;</td>
                            <td>
                                &nbsp;</td>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                <asp:HiddenField ID="lblCardCode" runat="server"></asp:HiddenField>
                                <asp:HiddenField ID="lblCardName" runat="server"></asp:HiddenField>
                                <asp:HiddenField ID="lblDocEntry" runat="server" />
                                <asp:HiddenField ID="lblLineNum" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <hr />
                    <div style="width: 100%; height: 100%;">
                        <asp:GridView ID="grvPO" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                            BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                            HeaderStyle-Height="27px" CellSpacing="2" OnRowEditing="EditItem" OnRowUpdating="UpdateItem"
                            HeaderStyle-VerticalAlign="Middle" OnRowCancelingEdit="CancelEdit" OnRowDeleting="DeleteItem"
                             OnRowCreated="grvPO_RowCreated" OnRowDataBound="grvSearchResult_RowDataBound">
                            <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                            <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                            <Columns>
                                <asp:CommandField HeaderText="Action" ShowDeleteButton="True" ShowEditButton="false"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" Visible="false"/>
                                      <asp:TemplateField HeaderText="Action">
                                    <ItemStyle HorizontalAlign="Center" Width="20px" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                       <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" TabIndex="-1" OnClick="lnkDelete_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="#">
                                    <ItemStyle HorizontalAlign="Center" Width="20px" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblNo" runat="server" Text='<%# Bind("No") %>' BorderStyle="none">
                                        </asp:Label>
                                        <asp:HiddenField ID="hdnVatGroup" runat="server" Value='<%# Bind("VatgroupPu") %>'>
                                        </asp:HiddenField>
                                        <asp:HiddenField ID="hdnRate" runat="server" Value='<%# Bind("Rate") %>'></asp:HiddenField>
                                        <asp:HiddenField ID="hdnManBtchNum" runat="server" Value='<%# Bind("ManBtchNum") %>'>
                                        </asp:HiddenField>
                                        <asp:HiddenField ID="hdnLineNum" runat="server" Value='<%# Bind("LineNum") %>'></asp:HiddenField>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Item Code">
                                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description">
                                    <ItemStyle HorizontalAlign="left" Width="400px" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtItemName" runat="server" Text='<%# Bind("Dscription") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="PO.No">
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblDocNum" runat="server" Text='<%# Bind("DocNum") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="UoM">
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblBuyUnitMsr" runat="server" Text='<%# Bind("BuyUnitMsr") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="PO Qty">
                                    <HeaderStyle VerticalAlign="Middle"  Width="100px" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblQuantity" runat="server" Text='<%# Bind("Quantity") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Receive Qty">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle"  Width="100px" />
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtReceivedQty" runat="server" Width="96%" Text='<%# Eval("ReceivedQty")%>' AutoPostBack="true"
                                            OnKeyPress="return isNumberKey(this, event);" Style="text-align: right"  OnTextChanged="lblReceivedQty_OnTextChanged" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Balance Qty">
                                    <HeaderStyle VerticalAlign="Middle"   Width="100px"/>
                                    <ItemStyle HorizontalAlign="Right"  />
                                    <ItemTemplate>
                                        <asp:Label ID="lblBalanceQty" runat="server" Text='<%# Bind("BalanceQty") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Unit Price">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblPrice" runat="server" Text='<%# Bind("Price") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Line Total">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle"  Width="100px" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblLineTotal" runat="server" Text='<%# Bind("LineTotal") %> ' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Remarks" Visible="false">
                                    <ItemStyle HorizontalAlign="Left" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtRemarks" runat="server" Width="97%" Text='<%# Eval("Remarks")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                             
                                  <asp:TemplateField HeaderText="Batch">
                                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblBatch" runat="server" Text='<%# Bind("ManBtchNum") %>' BorderStyle="none" Style="text-align:center">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                   <asp:TemplateField HeaderText="">
                                    <ItemStyle HorizontalAlign="Center" Width="70px" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" ID="lnkBatch" Text="Batch" OnClick="lnkBatch_Click"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle BackColor="#6095C9" Font-Bold="true" ForeColor="#ffffff" Font-Overline="False"
                                Height="27px" VerticalAlign="Bottom" />
                            <EmptyDataTemplate>
                                <table class="GridInner" style="width: 100%; border-color: White;" border="1" rules="all"
                                    cellspacing="2" cellpadding="2">
                                    <tr valign="middle" style="height: 27px; color: white; font-weight: bold; text-decoration: none;
                                        background-color: rgb(96, 149, 201);">
                                        <th>
                                            <span>#</span>
                                        </th>
                                        <th>
                                            <span>Item Code</span>
                                        </th>
                                        <th>
                                            <span>Description</span>
                                        </th>
                                        <th>
                                            <span>PO Qty</span>
                                        </th>
                                        <th>
                                            <span>UoM</span>
                                        </th>
                                        <th>
                                            <span>Receive Qty</span>
                                        </th>
                                        <th>
                                            <span>Balance Qty</span>
                                        </th>
                                        <th>
                                            <span>Unit Price</span>
                                        </th>
                                        <th>
                                            <span>Line Total</span>
                                        </th>
                                        <%--<th>
                                            <span>Remarks</span>
                                        </th>--%>
                                        <th>
                                            <span>Batch</span>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="13">
                                            <span>No Data</span>
                                        </td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                    <hr />
                    <div style="width: 100%;">
                        <table style="width: 100%; background-color: #D1D4D8; font-weight: bold;">
                            <tr>
                                <td style="width: 450px;" valign="top">
                                    <div style="float: left">
                                        <table border="0">
                                            <tr>
                                                <td valign="middle">
                                                    Remarks:
                                                </td>
                                                <td valign="top">
                                                    <asp:TextBox ID="txtRemarks" runat="server" Width="300px" Height="80px" TextMode="MultiLine"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                                <td>
                                    <div style="margin-left: 70%; margin-right: 0px;">
                                        <table border="0">
                                            <tr valign="middle" style="height: 27px; font-weight: bold; text-decoration: none;
                                                background-color: #D9E0ED; text-align: right">
                                                <td style="width: 450px">
                                                    Sub Total :
                                                </td>
                                                <td style="width: 500px">
                                                    &nbsp;
                                                    <asp:Label ID="lblSubTotal" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr valign="middle" style="height: 27px; font-weight: bold; text-decoration: none;
                                                background-color: #D9E0ED; text-align: right">
                                                <td>
                                                    GST :
                                                </td>
                                                <td>
                                                    &nbsp;
                                                    <asp:Label ID="lblGSTAmount" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr valign="middle" style="height: 27px; font-weight: bold; text-decoration: none;
                                                background-color: #D9E0ED; text-align: right">
                                                <td>
                                                    Total :
                                                </td>
                                                <td>
                                                    &nbsp;
                                                    <asp:Label ID="lblDocumentTotal" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <br />
                    <table>
                        <tr>
                            <td align="left">
                                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" Style="background-image: url('/Images/bgButton.png');
                                   " Width="80px" />
                            </td>
                            <td align="left">
                                <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </ajax:UpdatePanel>
        </div>
    </div>
</asp:Content>
