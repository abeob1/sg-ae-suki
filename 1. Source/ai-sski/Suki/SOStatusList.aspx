<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Main.Master" CodeBehind="SOStatusList.aspx.cs"
    Inherits="Suki.SOStatusList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function CheckAll(oCheckbox) {
            var GridView2 = document.getElementById("<%=grdItem.ClientID %>");
            for (i = 1; i < GridView2.rows.length; i++) {
                if (GridView2.rows[i].cells[0].getElementsByTagName("INPUT")[0]) {
                    GridView2.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked = oCheckbox.checked;
                }
            }
        }
    </script>
    <div>
        <h2>
            SO Listing</h2>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table style="width: 100%; background-color: #D1D4D8;">
                        <tr>
                            <td width="125px" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                                Approved Date From :
                            </td>
                            <td width="160px">
                                <asp:TextBox ID="txtDateFrom" runat="server" Width="120px"></asp:TextBox>
                                <cc1:CalendarExtender ID="txtDeliDateFrom_CalendarExtender" runat="server" PopupButtonID="Image1"
                                    TargetControlID="txtDateFrom" Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                                <asp:ImageButton ID="Image1" runat="Server" AlternateText="Click to show calendar"
                                    ImageUrl="~/Images/Calendar_scheduleHS.png" />
                            </td>
                            <td style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                                PO Number :
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtPONo" runat="server" Width="109px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td width="120px" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                                Approved Date To :
                            </td>
                            <td width="150px">
                                <asp:TextBox ID="txtDateTo" runat="server" Width="120px"></asp:TextBox>
                                <cc1:CalendarExtender ID="txtDeliDateTo_CalendarExtender" runat="server" PopupButtonID="ImageButton1"
                                    TargetControlID="txtDateTo" Format="dd/MM/yyyy">
                                </cc1:CalendarExtender>
                                <asp:ImageButton ID="ImageButton1" runat="Server" AlternateText="Click to show calendar"
                                    ImageUrl="~/Images/Calendar_scheduleHS.png" />
                            </td>
                            <td style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                                <asp:Label ID="lblCompany" runat="server" Text="Company:"></asp:Label>
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="ddlCompany" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td width="120px" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                                Status :
                            </td>
                            <td width="150px">
                                <asp:DropDownList ID="ddlStatus" runat="server" Width="125px">
                                    <asp:ListItem Text="All" Value="ALL"></asp:ListItem>
                                    <asp:ListItem Text="Pass" Value="Pass"></asp:ListItem>
                                    <asp:ListItem Text="Failed" Value="Failed"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td style="font-weight: bold; text-transform: capitalize; font-variant: normal; width: 80px;"
                                valign="middle">
                                &nbsp;
                            </td>
                            <td align="left" style="font-weight: bold;">
                                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search"
                                    Width="80px" Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;" />
                            </td>
                    </table>
                    <hr />
                    <asp:GridView ID="grdItem" runat="server" CssClass="GridInner" Width="100%" BorderColor="White"
                        BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                        AllowPaging="True" OnPageIndexChanging="grvSearchResult_PageIndexChanging" PageSize="20"
                        HeaderStyle-Height="27px" OnRowDataBound="grdItem_RowDataBound" OnRowCreated="grvClosingStock_RowCreated">
                        <PagerSettings Mode="NumericFirstLast" />
                        <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />
                        <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                        <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                        <Columns>
                            <asp:TemplateField>
                                <ItemStyle HorizontalAlign="Center" Width="10px" />
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkheader" runat="server" onclick="CheckAll(this)" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkChild" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="#" HeaderStyle-VerticalAlign="Middle">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Center" Width="40px" />
                                <ItemTemplate>
                                    <asp:Label ID="lblNo" runat="server" BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="PO No">
                                <ItemStyle HorizontalAlign="Center" Width="200px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblNumAtCard" runat="server" Text='<%# Bind("NumAtCard") %>' BorderStyle="none">
                                    </asp:Label>
                                    <asp:HiddenField ID="hdnPOEntry" runat="server" Value='<%# Bind("U_AB_PODocEntry") %>'>
                                    </asp:HiddenField>
                                    <asp:HiddenField ID="hdnID" runat="server" Value='<%# Bind("ID") %>'></asp:HiddenField>
                                    <asp:HiddenField ID="hdnVendorCode" runat="server" Value='<%# Bind("LicTradNum") %>'>
                                    </asp:HiddenField>
                                </ItemTemplate>
                            </asp:TemplateField>
                               <asp:TemplateField HeaderText="Approved Date">
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblDocDate" runat="server" Text='<%# Bind("DocDate") %>' BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="Delivery Date">
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblDocDueDate" runat="server" Text='<%# Bind("DocDueDate") %>' BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Supplier Code">
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblSupplierCode" runat="server" Text='<%# Bind("LicTradNum") %>' BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Driver No">
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblDriverNo" runat="server" Text='<%# Bind("U_AB_DriverNo") %>' BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Outlet ID">
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblWhsCode" runat="server" Text='<%# Bind("U_AB_POWhsCode") %>' BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CardCode">
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblCardCode" runat="server" Text='<%# Bind("CardCode") %>' BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Status">
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("SOStatus") %>' BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Message">
                                <ItemStyle HorizontalAlign="left" Width="1000px" />
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemTemplate>
                                    <asp:Label ID="lblMessage" runat="server" Text='<%# Bind("ErrMessage") %>' BorderStyle="none">
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <SelectedRowStyle BackColor="#9471DE" Font-Bold="true" ForeColor="White" />
                        <HeaderStyle BackColor="#6095C9" Font-Bold="true" ForeColor="#ffffff" Font-Overline="False"
                            Height="27px" VerticalAlign="Bottom" />
                        <EmptyDataTemplate>
                            <table class="GridInner" style="width: 100%; border-color: White;" border="1" rules="all"
                                cellspacing="2" cellpadding="2">
                                <tr valign="middle" style="height: 27px; color: white; font-weight: bold; text-decoration: none;
                                    background-color: rgb(96, 149, 201);">
                                    <td align="center" style="width: 10px;">
                                    </td>
                                    <td align="center" style="width: 20px;">
                                        #
                                    </td>
                                    <td align="center" style="width: 40px;">
                                        PO No
                                    </td>
                                      <td align="center" style="width: 60px;">
                                        Approved Date
                                    </td>
                                      <td align="center" style="width: 60px;">
                                        Delivery Date
                                    </td>
                                    <td align="center" style="width: 80px;">
                                        Driver No
                                    </td>
                                    <td align="center" style="width: 250px;">
                                        Outlet ID
                                    </td>
                                    <td align="center" style="width: 50px;">
                                        CardCode
                                    </td>
                                    <td align="center" style="width: 250px;">
                                        Status
                                    </td>
                                    <td align="center" style="width: 100px;">
                                        Message
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="10" align="left">
                                        No Data
                                    </td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <hr />
                    <asp:Timer ID="Timer1" runat="server" Interval="1" OnTick="Timer1_Tick" />
                    <asp:Button ID="btnAccept" runat="server" Text="Post SO" OnClick="btnAccept_Click"
                        Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;"
                        Width="80px" />
                    <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
