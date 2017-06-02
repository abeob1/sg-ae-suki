<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="PODrafts.aspx.cs" Inherits="Suki.PODrafts" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function CheckAll(oCheckbox) {
            var GridView2 = document.getElementById("<%=grvSearchResult.ClientID %>");
            for (i = 1; i < GridView2.rows.length; i++) {
                if (GridView2.rows[i].cells[0].getElementsByTagName("INPUT")[0]) {
                    GridView2.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked = oCheckbox.checked;
                }
            }
        }
        function OpenVendor(companyCode, whsCode) {
            var url = "VendorPopup.aspx?CompanyCode=" + companyCode + "&WareHouse=" + whsCode;
            Main.openCustomDialog(url, 600, 610);
        }
    </script>
    <h2>
        PO Draft Listing
    </h2>
    <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div style="margin-left: 5px; width: 99%;">
                <hr />
                <table style="width: 100%; background-color: #D1D4D8;">
                    <tr>
                        <td width="120px" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                            Issued Date From :
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
                            Delivery Date From :
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtDeliDateFrom" runat="server" Width="120px"></asp:TextBox>
                            <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtDeliDateFrom"
                                PopupButtonID="Image2" Format="dd/MM/yyyy">
                            </cc1:CalendarExtender>
                            <asp:ImageButton ID="Image2" runat="Server" AlternateText="Click to show calendar"
                                ImageUrl="~/Images/Calendar_scheduleHS.png" />
                        </td>
                    </tr>
                    <tr>
                        <td width="100px" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                            Issued Date To :
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
                            Delivery Date To :
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtDeliDateTo" runat="server" Width="120px"></asp:TextBox>
                            <cc1:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDeliDateTo"
                                PopupButtonID="ImageButton2" Format="dd/MM/yyyy">
                            </cc1:CalendarExtender>
                            <asp:ImageButton ID="ImageButton2" runat="Server" AlternateText="Click to show calendar"
                                ImageUrl="~/Images/Calendar_scheduleHS.png" />
                        </td>
                    </tr>
                    <tr>
                        <td style="font-weight: bold; text-transform: capitalize; font-variant: normal; width: 80px;"
                            valign="middle">
                            Supplier :
                        </td>
                        <td align="left" style="width: 250px">
                            <asp:TextBox ID="txtVendorCode" runat="server" AutoPostBack="True" OnTextChanged="txtVendorCode_TextChanged"
                                Width="70px"></asp:TextBox>
                            <asp:TextBox ID="txtVendorName" runat="server" Width="130px"></asp:TextBox>
                            <asp:Button ID="btnSelectVendor" runat="server" Style="background-image: url('/Images/bgButton.png');
                                background-repeat: no-repeat; font-weight: bold" Text="..." OnClick="btnSelectVendor_Click" />
                        </td>
                        <td width="160px" style="font-weight: bold; text-transform: capitalize; font-variant: normal;"
                            nowrap>
                            Outlet :&nbsp;
                            <asp:DropDownList ID="ddlStatus" runat="server" Width="125px" Enabled="false" 
                                Visible="False">
                                <asp:ListItem Text="All" Value="A"></asp:ListItem>
                                <asp:ListItem Text="Closed" Value="C"></asp:ListItem>
                                <asp:ListItem Text="Open" Value="O" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="drpOrderWareHouse" runat="server" Width="125px">
                            </asp:DropDownList>
                            <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" 
                                Style="background-image: url('/Images/bgButton.png');" Text="Search" 
                                Width="80px" />
                        </td>
                    </tr>
                </table>
                <hr />
                <asp:GridView ID="grvSearchResult" runat="server" Width="100%" CssClass="GridInner"
                    BorderColor="#CCCCCC" BackColor="White" AllowSorting="true" AutoGenerateColumns="False"
                    CellPadding="0" HeaderStyle-Height="27px" CellSpacing="2" OnRowCommand="grvSearchResult_RowCommand"
                    AllowPaging="True" OnPageIndexChanging="grvSearchResult_PageIndexChanging" PageSize="20"
                    OnRowDataBound="grvSearchResult_RowDataBound">
                    <PagerSettings Mode="NumericFirstLast" />
                    <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />
                    <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                    <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemStyle HorizontalAlign="Center" Width="2%" />
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkheader" runat="server" onclick="CheckAll(this)" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkChild" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Action" SortExpression="DocNum" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="Center" Width="30px" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkView" CommandName="ViewPO" runat="server" Text='View' OnClick="lnkPONo_Click"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="" HeaderStyle-VerticalAlign="Middle" Visible="false">
                            <ItemStyle HorizontalAlign="Center" Width="40px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDocEntry" runat="server" Visible="false" BorderStyle="none" Text='<%# Bind("DocEntry") %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="#" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="Center" Width="40px" />
                            <ItemTemplate>
                                <asp:Label ID="lblNo" runat="server" BorderStyle="none" Text='<%# Bind("No") %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="DocNum" Visible="false">
                            <ItemStyle HorizontalAlign="Center" Width="80px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDocNum" runat="server" Text='<%# Bind("DocNum") %>' BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Issued Date" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="Center" Width="30px" />
                            <ItemTemplate>
                                <asp:Label ID="lblPODate" runat="server" Text='<%# Bind("DocDate","{0:dd/MM/yyyy}") %>'
                                    BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Delivery Date" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="Center" Width="30px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDelDate" runat="server" Text='<%# Bind("DocDueDate","{0:dd/MM/yyyy}")  %>'
                                    BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Draft No." HeaderStyle-VerticalAlign="Middle" Visible="false">
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkPONo" CommandName="ViewPO" runat="server" Text='<%# Bind("DocNum") %>'
                                    OnClick="lnkPONo_Click"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Name" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="left" Width="200px" />
                            <ItemTemplate>
                                <asp:Label ID="lblItemDesc" runat="server" Text='<%# Bind("CardName") %>' BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="WhsCode" HeaderText="Outlet ID" ReadOnly="true" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="User ID & Name" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="Center" Width="150px" />
                            <ItemTemplate>
                                <asp:Label ID="lblUserCode" runat="server" Text='<%# Bind("U_AB_UserCode")%>' BorderStyle="none">
                                </asp:Label>
                                -
                                <asp:Label ID="lblUserName" runat="server" Text='<%# Bind("UserName") %>' BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataFormatString="{0,-15:#,##0.0000}" DataField="DocTotal" HeaderText="Total"
                            ReadOnly="true">
                            <ItemStyle HorizontalAlign="Right" Width="60px" />
                            <HeaderStyle VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="PO.Status" HeaderStyle-VerticalAlign="Middle" Visible="false">
                            <ItemStyle HorizontalAlign="Center" Width="70px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDocStatus" runat="server" Text='<%# Bind("DocStatus") %>' BorderStyle="none"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                          <asp:TemplateField HeaderText="Remarks" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="Left" Width="60px" />
                            <ItemTemplate>
                                <asp:Label ID="lblRemarks" runat="server" Text='<%# Bind("U_AB_PORemarks") %>' BorderStyle="none"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <SelectedRowStyle BackColor="LightCyan" ForeColor="DarkBlue" Font-Bold="true" />
                    <HeaderStyle BackColor="#6095C9" Font-Bold="true" ForeColor="#ffffff" Font-Overline="False"
                        Height="27px" VerticalAlign="Bottom" />
                    <EmptyDataTemplate>
                        <table class="GridInner" style="width: 100%; border-color: White;" border="1" rules="all"
                            cellspacing="2" cellpadding="2">
                            <tr valign="middle" style="height: 27px; color: white; font-weight: bold; text-decoration: none;
                                background-color: rgb(96, 149, 201);">
                                <td align="center" style="width: 30px;">
                                    Action
                                </td>
                                <td align="center" style="width: 10px;">
                                </td>
                                <td align="center" style="width: 40px;">
                                    #
                                </td>
                                <td align="center" style="width: 50px;">
                                    Issued Date
                                </td>
                                <td align="center" style="width: 50px;">
                                    Delivery Date
                                </td>
                                <td align="center" style="width: 250px;">
                                    Supplier Name
                                </td>
                                <td align="center" style="width: 30px;">
                                    Outlet ID
                                </td>
                                <td align="center" style="width: 60px;">
                                    User ID & Name
                                </td>
                                <td align="center" style="width: 60px;">
                                    Total
                                </td>
                            </tr>
                            <tr>
                                <td colspan="9">
                                    No Data
                                </td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                </asp:GridView>
                <hr />
                <asp:Button ID="btnSendToHQ" runat="server" Text="Send to HQ" Style="background-image: url(/Images/bgButton.png);"
                    Width="80px" OnClick="btnSendToHQ_Click" />
                <asp:Button ID="Button2" runat="server" Text="Export To Excel" Style="background-image: url(/Images/bgButton.png);"
                    Visible="false" />
                <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Timer ID="Timer1" runat="server" Interval="1" OnTick="Timer1_Tick" />
            </div>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
