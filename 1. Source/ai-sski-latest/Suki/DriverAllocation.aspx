<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="DriverAllocation.aspx.cs" Inherits="Suki.DriverAllocation" %>

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
    </script>
    <div>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <h2>
                    Driver Allocation</h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <td style="width: 150px">
                                PO Date :
                            </td>
                            <td style="width: 250px">
                                <asp:TextBox ID="txtPODate" runat="server" AutoPostBack="True" OnTextChanged="txtPODate_TextChanged"></asp:TextBox>
                                <asp:ImageButton ID="Image1" runat="Server" AlternateText="Click to show calendar"
                                    ImageUrl="~/Images/Calendar_scheduleHS.png" />
                                <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtPODate"
                                    PopupButtonID="Image1">
                                </cc1:CalendarExtender>
                            </td>
                            <td style="width: 110px">
                                Order WareHouse :
                            </td>
                            <td>
                                <asp:DropDownList ID="drpOrderWareHouse" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpOrderWareHouse_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                Expected Delivery Date :
                            </td>
                            <td valign="top">
                                <asp:TextBox ID="txtDelDate" runat="server"></asp:TextBox>
                                <asp:ImageButton ID="ImageButton1" runat="Server" AlternateText="Click to show calendar"
                                    ImageUrl="~/Images/Calendar_scheduleHS.png" />
                                <cc1:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDelDate"
                                    PopupButtonID="ImageButton1">
                                </cc1:CalendarExtender>
                                <asp:CheckBox ID="chkUrent" runat="server" Text="Ugrent" />
                            </td>
                            <td valign="top">
                                Ship To :
                            </td>
                            <td>
                                <asp:TextBox ID="txtShipTo" runat="server" TextMode="MultiLine" Enabled="False" Height="45px"
                                    Width="300px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                PO No:</td>
                            <td valign="top">
                                <asp:TextBox ID="txtPONo" runat="server"></asp:TextBox>
                                <cc1:CalendarExtender ID="txtPONo_CalendarExtender" runat="server" 
                                    PopupButtonID="ImageButton1" TargetControlID="txtPONo">
                                </cc1:CalendarExtender>
                            </td>
                            <td valign="top">
                                &nbsp; Driver No:</td>
                            <td>
                                <asp:DropDownList ID="drpDriverNo" runat="server" AutoPostBack="True" 
                                    OnSelectedIndexChanged="drpOrderWareHouse_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                    <table id="Table1" width="100%" style="font-weight: bold">
                        <tr>
                            <td style="white-space: nowrap; width: 280px" valign="middle">
                                Vendor:&nbsp;
                                <asp:TextBox ID="txtVendorCode" runat="server" ReadOnly="True" Width="96px" BackColor="#E6E6E6"></asp:TextBox>
                                <asp:TextBox ID="txtVendorName" runat="server"></asp:TextBox>
                                <asp:Button ID="btnSelectVendor" runat="server" OnClientClick="javascript:Main.openCustomDialog('VendorPopup.aspx',600,610); return false;"
                                    Text="..." Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;
                                    font-weight: bold" />
                            </td>
                        </tr>
                    </table>
                    <hr />
                    <div style="width: 100%">
                        <asp:GridView ID="grvPO" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                            BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                            HeaderStyle-Height="27px" OnRowCreated="grvPO_RowCreated" CellSpacing="2" OnRowEditing="EditItem"
                            OnRowUpdating="UpdateItem" HeaderStyle-VerticalAlign="Middle" OnRowCancelingEdit="CancelEdit"
                            OnRowDeleting="DeleteItem">
                            <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                            <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                            <Columns>
                                <asp:CommandField HeaderText="Action" ShowDeleteButton="True" ShowEditButton="False"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="#">
                                    <ItemStyle HorizontalAlign="Center" Width="2%" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblNo" runat="server" Text="" BorderStyle="none">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Item Code">
                                    <ItemStyle HorizontalAlign="left" Width="10%" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none">
                                        </asp:Label>
                                        <asp:HiddenField ID="hdnVatGroup" runat="server" Value='<%# Bind("VatgroupPu") %>'>
                                        </asp:HiddenField>
                                        <asp:HiddenField ID="hdnRate" runat="server" Value='<%# Bind("Rate") %>'></asp:HiddenField>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description">
                                    <ItemStyle HorizontalAlign="left" Width="20%" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtItemName" runat="server" Text='<%# Bind("Dscription") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="UoM">
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtUoM" runat="server" Text='<%# Bind("BuyUnitMsr") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Quantity">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblOrderQuantity" runat="server" Text='<%# Bind("Quantity") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Received Quantity">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtReceivedQuantity" runat="server" Text='<%# Bind("ReceivedQty") %>'
                                            BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Unit Price">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtUnitPrice" runat="server" Text='<%# Bind("Price") %>' BorderStyle="none" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Line Total">
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="txtLineTotal" runat="server" Text='<%# Bind("LineTotal") %>' BorderStyle="none" />
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
                                            <span>UoM</span>
                                        </th>
                                        <th>
                                            <span>Order Quantity</span>
                                        </th>
                                        <th>
                                            <span>Received Quantity</span>
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
                        <td>
                            <div style="margin-left: 5px; width: 99%;">
                                <hr />
                            </div>
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 220px;">
                                        <asp:Button ID="btnUpdate" runat="server" Text="Add PO" OnClick="btnUpdate_Click"
                                            Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;
                                            color: White;" BorderStyle="Solid" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
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
