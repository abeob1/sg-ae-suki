<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Popup.Master" CodeBehind="ItemPopup.aspx.cs"
    Inherits="Suki.ItemPopup" %>

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
                Filter
                <asp:TextBox runat="server" ID="txtSearch"></asp:TextBox>
                <asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click" Text="..." Style="background-image: url('/Images/bgButton.png');
                    background-repeat: no-repeat;" />
                <div style="height: 480px; overflow: scroll">
                    <table style="width: 97%; border: 1px;" cellpadding="3" cellspacing="0">
                        <tr>
                            <td colspan="2" style="text-align: center;">
                                <asp:GridView ID="grdItem" runat="server" CssClass="GridInner" Width="100%" BorderColor="White"
                                    BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                    HeaderStyle-Height="27px" OnSorting="grdItem_Sorting" OnRowDataBound="grdItem_RowDataBound">
                                    <RowStyle BackColor="#D9E0ED" ForeColor="Black" BorderColor="White" BorderWidth="2px" Height="25px" />
                                    <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" Height="25px"/>
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
                                        <asp:TemplateField HeaderText="Item Code" SortExpression="ItemCode">
                                            <ItemStyle HorizontalAlign="Center" Width="13%" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item Name" SortExpression="ItemName">
                                            <ItemStyle HorizontalAlign="left" Width="70%" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemName" runat="server" Text='<%# Bind("ItemName") %>' BorderStyle="none">
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
                <asp:Button ID="btnAccept" runat="server" Text="OK" OnClick="btnAccept_Click"
                    Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;" Width="80px" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
