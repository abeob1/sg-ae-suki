<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="GRPOList.aspx.cs" Inherits="Suki.GRPOList" %>

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
        Good Receipt PO Listing
    </h2>
    <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div style="margin-left: 5px; width: 99%;">
                <hr />
                <table style="width: 100%; background-color: #D1D4D8;">
                    <tr>
                        <td width="170px" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                            Goods Receipt Date From :
                        </td>
                        <td width="160px">
                            <asp:TextBox ID="txtDateFrom" runat="server" Width="120px"></asp:TextBox>
                            <cc1:CalendarExtender ID="txtDeliDateFrom_CalendarExtender" runat="server" PopupButtonID="Image1"
                                TargetControlID="txtDateFrom" Format="dd/MM/yyyy">
                            </cc1:CalendarExtender>
                            <asp:ImageButton ID="Image1" runat="Server" AlternateText="Click to show calendar"
                                ImageUrl="~/Images/Calendar_scheduleHS.png" />
                        </td>
                        <td style="font-weight: bold; text-transform: capitalize; font-variant: normal; width:100px">
                            GRN Number :
                            </td>
                        <td align="left">
                            <asp:TextBox ID="txtGNRNo" runat="server" Width="207px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td width="170px" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                            Goods Receipt Date To :
                        </td>
                        <td width="150px">
                            <asp:TextBox ID="txtDateTo" runat="server" Width="120px"></asp:TextBox>
                            <cc1:CalendarExtender ID="txtDeliDateTo_CalendarExtender" runat="server" PopupButtonID="ImageButton1"
                                TargetControlID="txtDateTo" Format="dd/MM/yyyy">
                            </cc1:CalendarExtender>
                            <asp:ImageButton ID="ImageButton1" runat="Server" AlternateText="Click to show calendar"
                                ImageUrl="~/Images/Calendar_scheduleHS.png" />
                        </td>
                        <td style="font-weight: bold; text-transform: capitalize; font-variant: normal;"  >
                            Supplier :</td>
                        <td align="left">
                         
                            <asp:TextBox ID="txtVendorCode" runat="server" AutoPostBack="True" 
                                OnTextChanged="txtVendorCode_TextChanged" Width="70px"></asp:TextBox>
                            <asp:TextBox ID="txtVendorName" runat="server" Width="130px"></asp:TextBox>
                            <asp:Button ID="btnSelectVendor" runat="server" onclick="btnSelectVendor_Click" 
                                Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;" 
                                Text="..." />
                         
                            </td>
                    </tr>
                    <tr>
                    <td style="font-weight: bold; text-transform: capitalize; font-variant: normal; width: 120px;"
                            valign="middle"> Company :
                    </td>
                    <td> 
                        <asp:DropDownList ID="ddlCompany" runat="server" AutoPostBack="true" 
                            onselectedindexchanged="ddlCompany_SelectedIndexChanged">
                        </asp:DropDownList>
                        </td>
                            <td>  Outlet :
                            </td>
                            <td> <asp:DropDownList ID="drpOrderWareHouse" runat="server" Width="125px" 
                                    AutoPostBack="True" 
                                    onselectedindexchanged="drpOrderWareHouse_SelectedIndexChanged">
                            </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" 
                                Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;" 
                                Text="Search" Width="80px"/>
                            </td>
                    </tr>
                </table>
                <hr />
                <asp:GridView ID="grvSearchResult" runat="server" Width="100%" CssClass="GridInner"
                    BorderColor="#CCCCCC" BackColor="White" AllowSorting="true" AutoGenerateColumns="False"
                    CellPadding="0" HeaderStyle-Height="27px" CellSpacing="2" OnRowCommand="grvSearchResult_RowCommand" AllowPaging="True" 
                    onpageindexchanging="grvSearchResult_PageIndexChanging" PageSize="20"  OnRowDataBound="grvSearchResult_RowDataBound">
                    <PagerSettings Mode="NumericFirstLast" />
                    <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" 
                        VerticalAlign="Middle" />
                    <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                    <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                    <Columns>
                        <asp:TemplateField>
                            <ItemStyle HorizontalAlign="Center" Width="20px" />
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkheader" runat="server" onclick="CheckAll(this)" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkChild" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Action" SortExpression="DocNum" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="Center" Width="10px" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkView" CommandName="ViewPO" runat="server" Text='View'
                                    OnClick="lnkPONo_Click"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Action" HeaderStyle-VerticalAlign="Middle" Visible="false">
                            <ItemStyle HorizontalAlign="Center" Width="60px" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkCopy" runat="server" CommandName="Copy" BorderStyle="none"
                                    Text='Copy To GRPO'>
                                </asp:LinkButton>
                                <asp:HiddenField ID="hdnEmail" runat="server" Value='<%# Bind("E_Mail") %>'></asp:HiddenField>
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
                            <ItemStyle HorizontalAlign="Center" Width="20px" />
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
                        <asp:TemplateField HeaderText="Good Receipt Date" SortExpression="DocDate" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                            <ItemTemplate>
                                <asp:Label ID="lblPODate" runat="server" Text='<%# Bind("DocDate","{0:dd/MM/yyyy}") %>'
                                    BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="GRN No." SortExpression="DocNum" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkPONo" CommandName="ViewPO" runat="server" Text='<%# Bind("DocNum") %>'
                                    OnClick="lnkPONo_Click" Enabled="false"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Name" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="left" Width="200px" />
                            <ItemTemplate>
                                <asp:Label ID="lblItemDesc" runat="server" Text='<%# Bind("CardName") %>' BorderStyle="none"
                                    Style="margin-left: 5px">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Name" HeaderStyle-VerticalAlign="Middle" Visible="false">
                            <ItemStyle HorizontalAlign="left" Width="200px" />
                            <ItemTemplate>
                                <asp:Label ID="lblCompany" runat="server" Text='<%# Bind("CompnyName") %>' BorderStyle="none"
                                    Style="margin-left: 5px">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Delivery Date" SortExpression="DocDueDate" HeaderStyle-VerticalAlign="Middle" Visible="false">
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDelDate" runat="server" Text='<%# Bind("DocDueDate","{0:dd/MM/yyyy}")  %>'
                                    BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Outlet ID" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                            <ItemTemplate>
                                 <asp:Label ID="lblOutletID" runat="server" Text='<%# Bind("U_AB_POWhsCode") %>' BorderStyle="none"
                                    Style="margin-left: 5px">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataFormatString="{0,-15:#,##0.0000}" DataField="DocTotal" HeaderText="Total"
                            ReadOnly="true" SortExpression="Total">
                            <ItemStyle HorizontalAlign="Right" Width="60px" />
                            <HeaderStyle VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="GRPO Status" HeaderStyle-VerticalAlign="Middle">
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDocStatus" runat="server" Text='<%# Bind("DocStatus") %>' BorderStyle="none"></asp:Label>
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
                                <td align="center" style="width: 60px;">
                                    Action
                                </td>
                                <td align="center" style="width: 40px;">
                                    #
                                </td>
                                <td align="center" style="width: 80px;">
                                    Good Receipt Date
                                </td>
                                <td align="center" style="width: 50px;">
                                    GRPO No.
                                </td>
                                <td align="center" style="width: 250px;">
                                    Supplier Name
                                </td>
                                <td align="center" style="width: 100px;">
                                    Outlet ID
                                </td>
                                <td align="center" style="width: 60px;">
                                    Total
                                </td>
                                <td align="center" style="width: 70px;">
                                    GRPO Status
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
                <table>
                    <tr>
                        <td>
                            &nbsp;</td>
                        <td>
                            <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                </table>
                <asp:Timer ID="Timer1" runat="server" Interval="1" OnTick="Timer1_Tick" />
            </div>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
