<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Popup.Master" CodeBehind="OutletPopup.aspx.cs"
    Inherits="Suki.OutletPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function CheckAll(oCheckbox) {
            var GridView2 = document.getElementById("<%=grdItem.ClientID %>");
            for (i = 1; i < GridView2.rows.length; i++) {
                GridView2.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked = oCheckbox.checked;
            }
        }
    </script>
    <div>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField ID="hdnCompanyCode" runat="server" />
                <asp:HiddenField ID="hdnCardCode" runat="server" />
                <asp:HiddenField ID="hdnisUpdate" runat="server" />
                <table id="Table1" width="100%" style="font-weight: bold; background-color: #D1D4D8">
                    <tr>
                        <td style="width: 100px;">
                            Item Code :
                        </td>
                        <td align="left">
                            <asp:Label ID="lblItemCode" runat="server" />
                        </td>
                        <td style="width: 80px;">
                            Supplier :
                        </td>
                        <td align="left">
                            <asp:Label ID="hdnCardName" runat="server" />
                        </td>
                        <td style="width: 80px;">
                            Company :
                        </td>
                        <td align="left">
                            <asp:Label ID="hdnCompanyName" runat="server" />
                        </td>
                    </tr>
                </table>
                <div style="height: 500px; overflow: scroll">
                    <hr />
                    <table style="width: 100%; border: 1px;" cellpadding="3" cellspacing="0">
                        <tr>
                            <td colspan="2" style="text-align: center;">
                                <asp:GridView ID="grdItem" runat="server" CssClass="GridInner" Width="100%" BorderColor="White"
                                    BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                    HeaderStyle-Height="27px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                                    OnSorting="grdItem_Sorting" OnRowDataBound="grdItem_RowDataBound">
                                    <RowStyle BackColor="#D9E0ED" ForeColor="Black" BorderColor="White" BorderWidth="2px"
                                        Height="25px" />
                                    <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" Height="25px" />
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
                                        <asp:TemplateField HeaderText="Outlet Code">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblOutletCode" runat="server" Text='<%# Bind("OutletCode") %>' BorderStyle="none">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Outlet Name">
                                            <ItemStyle HorizontalAlign="left" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblOutletName" runat="server" Text='<%# Bind("OutletName") %>' BorderStyle="none">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Min. Qty">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtMinQty" runat="server" Text='<%# Bind("MinQty") %>' BorderStyle="none"
                                                    Width="70px" OnKeyPress="return isNumberKey(this, event);" Style="text-align: right" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Max. Qty" >
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtMaxQty" runat="server" Text='<%# Bind("MaxQty") %>' BorderStyle="none"
                                                    Width="70px" OnKeyPress="return isNumberKey(this, event);" Style="text-align: right" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Min. Order Amt" Visible="false">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtMinAmt" runat="server" Text='<%# Bind("MinAmt") %>' BorderStyle="none"
                                                    Width="70px" OnKeyPress="return isNumberKey(this, event);" Style="text-align: right" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Max. Order Amt" Visible="false">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtMaxAmt" runat="server" Text='<%# Bind("MaxAmt") %>' BorderStyle="none"
                                                    Width="70px" OnKeyPress="return isNumberKey(this, event);" Style="text-align: right" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle BackColor="#6095C9" Font-Bold="true" ForeColor="#ffffff" Font-Overline="False"
                                        Height="27px" VerticalAlign="Bottom" />
                                    <EmptyDataTemplate>
                                        No Data Found.
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: center;">
                            </td>
                        </tr>
                    </table>
                </div>
                <asp:Timer ID="Timer1" runat="server" Interval="1" OnTick="Timer1_Tick" />
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnAccept" runat="server" Text="OK" OnClick="btnAccept_Click" Style="background-image: url(/Images/bgButton.png);
                                background-repeat: no-repeat;font-weight:bold;"  Width="80px"/>
                        </td>
                        <td>
                            <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
