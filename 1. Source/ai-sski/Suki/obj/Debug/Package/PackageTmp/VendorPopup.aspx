<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Popup.Master" CodeBehind="VendorPopup.aspx.cs"
    Inherits="Suki.VendorPopup" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                Filter
                <asp:TextBox runat="server" ID="txtSearch"></asp:TextBox>
                <asp:Button runat="server" ID="btnSearch" OnClick="btnSearch_Click" Text="..." Style="background-image: url('/Images/bgButton.png');
                    background-repeat: no-repeat;" />
                <table style="width: 100%; border: 1px;" cellpadding="3" cellspacing="0">
                    <tr>
                        <td colspan="2" style="text-align: center;">
                            <asp:GridView ID="grdVendor" runat="server" CssClass="GridInner" Width="100%" BorderColor="White"
                                BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                HeaderStyle-Height="27px" OnSorting="grdVendor_Sorting" OnSelectedIndexChanged="grdVendor_SelectedIndexChanged"
                                OnRowDataBound="grdVendor_RowDataBound">
                              <RowStyle BackColor="#D9E0ED" ForeColor="Black" BorderColor="White" BorderWidth="2px" Height="25px" />
                                    <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" Height="25px"/>
                                <Columns>
                                    <asp:TemplateField HeaderText="Card Code" SortExpression="CardCode">
                                        <ItemStyle HorizontalAlign="Center" Width="13%" />
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblItemDesc" runat="server" Text='<%# Bind("CardCode") %>' BorderStyle="none">
                                            </asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Card Name" SortExpression="CardName">
                                        <ItemStyle HorizontalAlign="left" Width="70%" />
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblFrgnItem" runat="server" Text='<%# Bind("CardName") %>' BorderStyle="none">
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
                <asp:Timer ID="Timer1" runat="server" Interval="1" OnTick="Timer1_Tick" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
