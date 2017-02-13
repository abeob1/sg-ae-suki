<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="CreatePO.aspx.cs" Inherits="Suki.CreatePO" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 
    <script type="text/javascript">
        function isNumberKey(sender, evt) {
            var txt = sender.value;
            var dotcontainer = txt.split('.');
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (!(dotcontainer.length == 1 && charCode == 46) && charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }
        function OpenItemPopup() {
            var supplier = $("#<%= txtVendorCode.ClientID %>").val();
            var wareHouse = $("#<%= drpOrderWareHouse.ClientID %> option:selected").val();
            var url = "ItemPopup.aspx?Supplier=" + supplier + "&WareHouse=" + wareHouse;
            Main.openCustomDialog(url, 600, 610);
        }
        function OpenVendor() {
            var wareHouse = $("#<%= drpOrderWareHouse.ClientID %> option:selected").val();
            var url = "VendorPopup.aspx?WareHouse=" + wareHouse;
            Main.openCustomDialog(url, 600, 610);
        }
    </script>
    <div>

        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField ID="hdnIsUpdate" runat="server" />
                <asp:HiddenField ID="hdnStatus" runat="server" />
                   <asp:HiddenField ID="hdnCreatedUser" runat="server" />
                <h2>
                    <asp:Label ID="lblTitle" runat="server" Text="Create Purchase Order Process"></asp:Label>
                </h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <td>
                                PO No :
                            </td>
                            <td>
                                <asp:Label ID="lblPoNo" runat="server"></asp:Label>
                            </td>
                            <td>
                                PO.Status :
                            </td>
                            <td>
                                <asp:Label ID="lblStatus" runat="server" Text="Open"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 150px">
                                PO Issue Date :
                            </td>
                            <td style="width: 250px">
                                <asp:TextBox ID="txtPODate" runat="server" AutoPostBack="True" OnTextChanged="txtPODate_TextChanged"></asp:TextBox>
                                <asp:ImageButton ID="Image1" runat="Server" AlternateText="Click to show calendar"
                                    ImageUrl="~/Images/Calendar_scheduleHS.png" />
                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtPODate"
                                    PopupButtonID="Image1" Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                            </td>
                            <td style="width: 110px">
                                Outlet Name :
                            </td>
                            <td>
                                <asp:DropDownList ID="drpOrderWareHouse" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpOrderWareHouse_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:TextBox ID="txtShipTo" runat="server" Enabled="False" Height="16px" TextMode="MultiLine"
                                    Visible="false" Width="300px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td valign="middle">
                                Expected Delivery Date :
                            </td>
                            <td valign="top">
                                <asp:TextBox ID="txtDelDate" runat="server"></asp:TextBox>
                                <asp:ImageButton ID="ImageButton1" runat="Server" AlternateText="Click to show calendar"
                                    ImageUrl="~/Images/Calendar_scheduleHS.png" />
                                <cc1:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDelDate"
                                    PopupButtonID="ImageButton1" Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                                <asp:CheckBox runat="server" Text="Urgent" ID="chkUrgent" />
                            </td>
                            <td valign="middle">
                                Daily Min Amount :
                            </td>
                            <td>
                                <asp:Label ID="lblMinAmount" runat="server" ForeColor="Red"></asp:Label>
                                &nbsp;&nbsp;&nbsp;&nbsp; Daily Max Amount :
                                <asp:Label ID="lblMaxAmount" runat="server" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                        <td > Status :</td>
                        <td>
                            <asp:Label ID="lblCStatus" runat="server" Font-Bold="True" Font-Size="10pt" 
                                ForeColor="Blue" Text="Sent to HQ"></asp:Label>
                            </td>
                          <td >Calendar : </td>
                        <td >
                            <asp:CheckBoxList ID="chkCalendar" runat="server" RepeatDirection="Horizontal" 
                                Enabled="False" Font-Bold="True">
                                <asp:ListItem>Mon</asp:ListItem>
                                <asp:ListItem>Tue</asp:ListItem>
                                <asp:ListItem>Wed</asp:ListItem>
                                <asp:ListItem>Thu</asp:ListItem>
                                <asp:ListItem>Fri</asp:ListItem>
                                <asp:ListItem>Sat</asp:ListItem>
                                <asp:ListItem>Sun</asp:ListItem>
                            </asp:CheckBoxList>
                            </td>
                        </tr>
                    </table>
                    <table width="100%" style="font-weight: bold;" >
                        <tr>
                            <td style="white-space: nowrap; width: 100%" valign="middle">
                                Vendor:&nbsp;
                                <asp:TextBox ID="txtVendorCode" runat="server" ReadOnly="True" Width="96px" BackColor="#E6E6E6"></asp:TextBox>
                                <asp:TextBox ID="txtVendorName" runat="server" Width="250px"></asp:TextBox>
                                <asp:Button ID="btnSelectVendor" runat="server" OnClientClick="OpenVendor(); return false;"
                                    Text="..." Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;
                                    font-weight: bold" Width="30px" />
                                <asp:Button ID="btnShowPopup" runat="server" Text="Add new item" OnClientClick="OpenItemPopup(); return false;"
                                    Enabled="false" Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;" />
                            </td>
                        </tr>
                    </table>
                    <hr />
                    <div style="width: 100%">
                        <asp:GridView ID="grvPO" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                            BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                            HeaderStyle-Height="27px" OnRowCreated="grvPO_RowCreated" CellSpacing="2" OnRowEditing="EditItem"
                            OnRowUpdating="UpdateItem" HeaderStyle-VerticalAlign="Middle" OnRowCancelingEdit="CancelEdit"
                            AllowPaging="True" OnPageIndexChanging="grvSearchResult_PageIndexChanging" PageSize="20"
                            OnRowDeleting="DeleteItem" OnRowDataBound="grvSearchResult_RowDataBound">
                            <PagerSettings Mode="NumericFirstLast" />
                            <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />
                            <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                            <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                            <Columns>
                                <asp:CommandField HeaderText="Action" ShowDeleteButton="True" ShowEditButton="False"
                                    Visible="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                                    ItemStyle-Width="70px" />
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
                                        <asp:Label ID="lblNo" runat="server" Text="" BorderStyle="none">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Item Code">
                                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none">
                                        </asp:Label>
                                        <asp:HiddenField ID="hdnVatGroup" runat="server" Value='<%# Bind("VatgroupPu") %>'>
                                        </asp:HiddenField>
                                           <asp:HiddenField ID="hdnLineNum" runat="server" Value='<%# Bind("LineNum") %>'>
                                        </asp:HiddenField>
                                        <asp:HiddenField ID="hdnRate" runat="server" Value='<%# Bind("Rate") %>'></asp:HiddenField>
                                        <asp:HiddenField ID="hdnIsGRPO" runat="server" Value='<%# Bind("IsGRPO") %>'></asp:HiddenField>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description">
                                    <ItemStyle HorizontalAlign="left" Width="500px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtItemName" runat="server" Text='<%# Bind("Dscription") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="L/B" Visible="false">
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtLB" runat="server" Text='<%# Bind("LB") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Min">
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtMin" runat="server" Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("MinStock"))%>'
                                            BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Max">
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemStyle HorizontalAlign="Right" Width="80px" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtMax" runat="server" Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("MaxStock"))%>'
                                            BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="UoM">
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtUoM" runat="server" Text='<%# Bind("BuyUnitMsr") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Quantity">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtOrderQuantity" runat="server" Width="97%" Text='<%# Eval("Quantity")%>'
                                            AutoPostBack="true" OnKeyPress="return isNumberKey(this, event);" Style="text-align: right"
                                            OnTextChanged="txtOrderQuantity_OnTextChanged"/>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Received Quantity" Visible="false">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle" Width="80px" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtReceivedQuantity" runat="server" Text='<%# Bind("ReceivedQty") %>'
                                            BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Balance Quantity" Visible="false">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle" Width="80px" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtBalanceQty" runat="server" Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("BalanceQty"))%>'
                                            BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Unit Price">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle" Width="120px" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtUnitPrice" runat="server" Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("Price"))%>'
                                            BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Line Total">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle" Width="120px" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtLineTotal" runat="server" Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("LineTotal"))%>'
                                            BorderStyle="none" />
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
                                            <span>Action</span>
                                        </th>
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
                                            <span>Min</span>
                                        </th>
                                        <th>
                                            <span>Max</span>
                                        </th>
                                        <th>
                                            <span>UoM</span>
                                        </th>
                                        <th>
                                            <span>Order Quantity</span>
                                        </th>
                                        <th>
                                            <span>Unit Price</span>
                                        </th>
                                        <th>
                                            <span>Line Total</span>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="12">
                                            <span>No Data</span>
                                        </td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                    <hr width="100%" />
                </div>
                <table width="100%" border="0">
                    <tr>
                        <td valign="top" style="width: 300px;">
                            <table>
                                <tr>
                                    <td>
                                        <strong>Remarks :</strong>
                                    </td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtRemarks" Height="80px" TextMode="MultiLine" Width="222px"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width:100px;"></td>
                        <td>
                            <div style="margin-left: 75%; margin-right: 5px;">
                                <table border="0">
                                    <tr valign="middle" style="height: 27px; color: black; font-weight: bold; text-decoration: none;
                                        background-color: #D9E0ED;">
                                        <td align="right">
                                            Sub Total:
                                        </td>
                                        <td style="width: 150px" align="right">
                                            <asp:Label ID="lblSubTotal" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr valign="middle" style="height: 27px; color: white; font-weight: bold; text-decoration: none;
                                        background-color: rgb(96, 149, 201);">
                                        <td align="right">
                                            GST Amount:
                                        </td>
                                        <td style="width: 150px" align="right">
                                            <asp:Label ID="lblGSTAmount" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr valign="middle" style="height: 27px; font-weight: bold; text-decoration: none;
                                        background-color: #D9E0ED;">
                                        <td style="width: 150px" align="right">
                                            Grand Total:
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="lblDocumentTotal" runat="server" Font-Bold="True" ForeColor="Red"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <div style="margin-left: 5px; width: 99%;">
                                <hr />
                            </div>
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 400px;">
                                        <asp:Button ID="btnSaveDraft" runat="server" Text="Save As Draft" Style="background-image: url('/Images/bgButton.png');
                                            background-repeat: no-repeat; color: White;" OnClick="btnSaveDraft_Click" BorderStyle="Solid" />
                                        <asp:Button ID="btnUpdate" runat="server" Text="Send To HQ" OnClick="btnUpdate_Click"
                                            Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;
                                            color: White;" BorderStyle="Solid" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel PO" Style="background-image: url('/Images/bgButton.png');
                                            background-repeat: no-repeat; color: White;" BorderStyle="Solid" OnClick="btnCancel_Click" />
                                        <asp:Button ID="btnClose" runat="server" Text="Close PO" Style="background-image: url('/Images/bgButton.png');
                                            background-repeat: no-repeat; color: White;" BorderStyle="Solid" OnClick="btnClose_Click" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
