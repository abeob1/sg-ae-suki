<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="OutletItemList.aspx.cs" Inherits="Suki.OutletItemList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function OpenCalendar() {
            var companyCode = $("#<%= drdOutlet.ClientID %> option:selected").val();
            var companyName = $("#<%= drdOutlet.ClientID %> option:selected").text();
            var cardCode = $("#<%= txtVendorCode.ClientID %>").val();
            var cardName = $("#<%= txtVendorName.ClientID %>").val();
            var url = "CalendarOutlet.aspx?CompanyCode=" + companyCode;
            url += "&CompanyName=" + companyName;
            url += "&CardCode=" + cardCode;
            url += "&CardName=" + cardName;
            Main.openCustomDialog(url, 600, 610);
        }
        function OpenOrderAmount() {
            var companyCode = $("#<%= drdOutlet.ClientID %> option:selected").val();
            var companyName = $("#<%= drdOutlet.ClientID %> option:selected").text();
            var cardCode = $("#<%= txtVendorCode.ClientID %>").val();
            var cardName = $("#<%= txtVendorName.ClientID %>").val();
            var url = "AmountOutlet.aspx?CompanyCode=" + companyCode;
            url += "&CompanyName=" + companyName;
            url += "&CardCode=" + cardCode;
            url += "&CardName=" + cardName;
            Main.openCustomDialog(url, 600, 610);
        }
        function OpenOutlet(itemCode) {
            var companyCode = $("#<%= drdOutlet.ClientID %> option:selected").val();
            var companyName = $("#<%= drdOutlet.ClientID %> option:selected").text();
            var cardCode = $("#<%= txtVendorCode.ClientID %>").val();
            var cardName = $("#<%= txtVendorName.ClientID %>").val();
            var url = "OutletPopup.aspx?CompanyCode=" + companyCode;
            url += "&CompanyName=" + companyName;
            url += "&CardCode=" + cardCode;
            url += "&CardName=" + cardName;
            url += "&ItemCode=" + itemCode;
            Main.openCustomDialog(url, 800, 610);
        }
        function OpenSupplier() {
            var url = "VendorPopup.aspx?IsSetup=True";
            Main.openCustomDialog(url, 600, 610);
        }
    </script>
    <h2>
        Outlet Item List Setup</h2>
    <asp:HiddenField ID="hdnisUpdate" runat="server" />
    <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="margin-left: 5px; width: 99%;">
                <hr />
                <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                    <tr>
                        <td style="width: 60px">
                            Supplier :
                        </td>
                        <td style="width: 550px">
                            <asp:TextBox ID="txtVendorCode" runat="server" Width="63px" OnTextChanged="txtVendorCode_TextChanged"></asp:TextBox>
                            <asp:TextBox ID="txtVendorName" runat="server" Width="400px"></asp:TextBox>
                            <asp:Button ID="btnSelectVendor" runat="server" OnClientClick="OpenSupplier(); return false;"
                                Text="..." Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;
                                font-weight: bold" />
                        </td>
                        <td style="width: 80px">
                            Company :
                        </td>
                        <td>
                            <asp:DropDownList ID="drdOutlet" runat="server" OnSelectedIndexChanged="drdOutlet_SelectedIndexChanged"
                                AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <hr />
                <table style="width: 100%;" border="0">
                    <tr>
                        <td style="width: 600px">
                          <asp:Button ID="btnBack" runat="server" Text="Back" Style="background-image: url('/Images/bgButton.png');
                                background-repeat: no-repeat;"
                                onclick="btnBack_Click" />
                            <asp:Button ID="btnCalendar" runat="server" Text="Calendar" Style="background-image: url('/Images/bgButton.png');
                                background-repeat: no-repeat;" OnClientClick="OpenCalendar(); return false;"
                                Enabled="False" />
                            <asp:Button ID="btnOrderAmount" runat="server" Text="Order Amt" Style="background-image: url('/Images/bgButton.png');
                                background-repeat: no-repeat;" OnClientClick="OpenOrderAmount(); return false;"
                                Enabled="False" />
                            <asp:Button ID="btnAddNewItem" runat="server" OnClientClick="javascript:Main.openCustomDialog('ItemPopup.aspx',600,610); return false;"
                                Text="Add Item" Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;"
                                Enabled="False" />
                            <asp:Button ID="btnSave" runat="server" Text="Save Outlet Item List" Style="background-image: url('/Images/bgButton.png');
                                background-repeat: no-repeat;" Enabled="False" OnClick="btnSave_Click" />
                            <asp:Button ID="btnSetupNew" runat="server" Text="Setup New" Style="background-image: url('/Images/bgButton.png');
                                background-repeat: no-repeat;" Visible="False" OnClick="Button1_Click" />
                        </td>
                        <td style="width: 600px">
                            <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:GridView ID="grvItem" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                                BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                HeaderStyle-Height="27px" CellSpacing="2" OnRowEditing="EditItem" OnRowUpdating="UpdateItem"
                                HeaderStyle-VerticalAlign="Middle" OnRowCancelingEdit="CancelEdit" OnRowDeleting="DeleteItem"
                                OnRowDataBound="grvSearchResult_RowDataBound"   AllowPaging="True" OnPageIndexChanging="grvSearchResult_PageIndexChanging" PageSize="20">
                                  <PagerSettings Mode="NumericFirstLast" />
                    <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />
                                <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                                <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Outlet">
                                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkOutlet" runat="server" Text="Outlet" OnClick="lnkOutlet_Click"></asp:LinkButton>
                                            |
                                            <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" OnClick="lnkDelete_Click" OnClientClick='return confirm("Are you sure to delete?");'></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Item No">
                                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblID" runat="server" Visible="false" Text='<%# Bind("ID") %>' BorderStyle="none">
                                            </asp:Label>
                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none">
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Description">
                                        <ItemStyle HorizontalAlign="left" Width="40%" />
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:Label ID="txtDscription" runat="server" Text='<%# Bind("Dscription") %>' BorderStyle="none" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Inv. UoM">
                                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:Label ID="txtBuyUnitMsr" runat="server" Text='<%# Bind("InvntryUom") %>' BorderStyle="none" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Purchase UoM">
                                        <ItemStyle HorizontalAlign="Center" Width="10%" />
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:Label ID="txtPurchaseUoM" runat="server" Text='<%# Bind("BuyUnitMsr") %>' BorderStyle="none" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="L/B" Visible="false">
                                        <ItemStyle HorizontalAlign="left" Width="20%" />
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtLB" runat="server" Text='<%# Bind("LB") %>' BorderStyle="none"
                                                Width="98%" OnTextChanged="txtLB_OnTextChanged" />
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
                                                <span>Outlet</span>
                                            </th>
                                            <th>
                                                <span>Item No.</span>
                                            </th>
                                            <th>
                                                <span>Item Desc</span>
                                            </th>
                                            <th>
                                                <span>Inv. UoM</span>
                                            </th>
                                            <th>
                                                <span>Purchase UoM</span>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td colspan="7">
                                                <span>No Data</span>
                                            </td>
                                        </tr>
                                    </table>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
                <br />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
