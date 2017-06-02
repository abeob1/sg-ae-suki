<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Popup.Master" CodeBehind="SOList.aspx.cs"
    Inherits="Suki.SOList" %>

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
  <h2>  SO Listing</h2>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div style="height: 450px; overflow: scroll">
                    <table style="width: 100%; border: 1px;" cellpadding="3" cellspacing="0">
                        <tr>
                            <td colspan="2" style="text-align: center;">
                                <asp:GridView ID="grdItem" runat="server" CssClass="GridInner" Width="100%" BorderColor="White"
                                    BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                    HeaderStyle-Height="27px" OnRowDataBound="grdItem_RowDataBound">
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
                                        <asp:TemplateField HeaderText="PO No" SortExpression="ItemCode">
                                            <ItemStyle HorizontalAlign="Center" Width="13%" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblNumAtCard" runat="server" Text='<%# Bind("NumAtCard") %>' BorderStyle="none">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Driver No" SortExpression="ItemName">
                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDriverNo" runat="server" Text='<%# Bind("U_AB_DriverNo") %>' BorderStyle="none">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Outlet ID" SortExpression="ItemName">
                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblWhsCode" runat="server" Text='<%# Bind("U_AB_POWhsCode") %>' BorderStyle="none">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="CardCode" SortExpression="ItemName">
                                            <ItemStyle HorizontalAlign="Center" Width="20%" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblCardCode" runat="server" Text='<%# Bind("CardCode") %>' BorderStyle="none">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status" SortExpression="ItemName">
                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("SOStatus") %>' BorderStyle="none">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Message" SortExpression="ItemName">
                                            <ItemStyle HorizontalAlign="left" Width="70%" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblMessage" runat="server" Text='<%# Bind("ErrMessage") %>' BorderStyle="none">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />
                                    <SelectedRowStyle BackColor="#9471DE" Font-Bold="true" ForeColor="White" />
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
                <asp:Button ID="btnAccept" runat="server" Text="Post SO" OnClick="btnAccept_Click"
                    Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;"
                    Width="80px" />
                <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false"></asp:Label>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
