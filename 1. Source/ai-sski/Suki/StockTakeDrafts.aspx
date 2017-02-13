<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="StockTakeDrafts.aspx.cs" Inherits="Suki.StockTakeDrafts" %>

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
        Stock Take Draft Listing
    </h2>
    <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div style="margin-left: 5px; width: 99%;">
                <hr />
             <table style="margin-left: 0px" class="MenuHeader" width="100%">
                    <tr>
                        <td style="width: 70px;">
                            Month :
                        </td>
                        <td style="width: 100px;">
                            <asp:DropDownList ID="ddlMonth" runat="server" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 50px;">
                            Year :
                        </td>
                        <td style="width: 100px;">
                            <asp:DropDownList ID="ddlYear" runat="server" AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 5%">
                            Outlet :
                        </td>
                        <td style="width: 363px">
                            <asp:DropDownList ID="ddlOutlet" runat="server" >
                            </asp:DropDownList>
                            <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" 
                                Style="background-image: url('/Images/bgButton.png');" Text="Search" 
                                Width="80px" />
                            <asp:TextBox ID="txtOutlet" runat="server" Enabled="False" Width="205px" 
                                Visible="false"></asp:TextBox>
                        </td>
                        <td style="width: 10%">
                            &nbsp;</td>
                        <td style="width: 15%">
                            &nbsp;</td>
                        <td style="width: 5%">
                        </td>
                        <td style="width: 25%">
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
                        <asp:TemplateField HeaderText="Action" SortExpression="DocNum" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="10px" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkView" CommandName="ViewPO" runat="server" Text='View' OnClick="lnkPONo_Click"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="" HeaderStyle-VerticalAlign="Middle" Visible="false">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="40px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDocEntry" runat="server" Visible="false" BorderStyle="none" Text='<%# Bind("DocEntry") %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="#" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="10px" />
                            <ItemTemplate>
                                <asp:Label ID="lblNo" runat="server" BorderStyle="none" Text='<%# Bind("No") %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Draft No" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="10px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDocNum" runat="server" Text='<%# Bind("DocEntry") %>' BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Period" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="30px" />
                            <ItemTemplate>
                                <asp:Label ID="lblPODate" runat="server" Text='<%# Bind("DocDate","{0:MM/yyyy}") %>'
                                    BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Right" Width="30px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDocTotal" runat="server"  Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("Quantity"))%>' BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Outlet" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                            <ItemTemplate>
                                <asp:Label ID="lblOutlet" runat="server" Text='<%# Bind("U_AB_POWhsCode")  %>' BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Remarks" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDelDate" runat="server" Text='<%# Bind("Comments")  %>' BorderStyle="none">
                                </asp:Label>
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
                                <td align="center" style="width: 40px;">
                                    #
                                </td>
                                <td align="center" style="width: 80px;">
                                    Stock No
                                </td>
                                <td align="center" style="width: 250px;">
                                    Period
                                </td>
                                <td align="center" style="width: 250px;">
                                    Total
                                </td>
                                <td align="center" style="width: 250px;">
                                    Outlet
                                </td>
                                <td align="center" style="width: 50px;">
                                    Remarks
                                </td>
                            </tr>
                            <tr>
                                <td colspan="7">
                                    No Data
                                </td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                </asp:GridView>
                <hr />
                <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                <asp:Timer ID="Timer1" runat="server" Interval="1" OnTick="Timer1_Tick" />
            </div>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
