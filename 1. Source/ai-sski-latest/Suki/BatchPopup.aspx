<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Popup.Master" CodeBehind="BatchPopup.aspx.cs"
    Inherits="Suki.BatchPopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField ID="hdnItemCode" runat="server" />
                <asp:HiddenField ID="hdnItemName" runat="server" />
                <asp:HiddenField ID="hdnLineNum" runat="server" />
                    <asp:HiddenField ID="hdnQuantity" runat="server" />
                <asp:HiddenField ID="hdnisUpdate" runat="server" />
                <div style="height: 500px; overflow: scroll">
                    <table style="width: 100%; border: 1px;" cellpadding="3" cellspacing="0">
                        <tr>
                            <td colspan="2" style="text-align: center;">
                                <asp:GridView ID="grdItem" runat="server" CssClass="GridInner" Width="100%" BorderColor="White"
                                    BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                    HeaderStyle-Height="27px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                                    OnRowDataBound="grdItem_RowDataBound">
                                    <RowStyle BackColor="#D9E0ED" ForeColor="Black" BorderColor="White" BorderWidth="2px"
                                        Height="25px" />
                                    <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" Height="25px" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Action">
                                            <ItemStyle HorizontalAlign="Center" Width="100px" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDelete" runat="server" Text='Delete' BorderStyle="none" OnClick="lnkDelete_Click">
                                                </asp:LinkButton>
                                                <asp:Label ID="lblNo" runat="server" Text='<%# Bind("No") %>' BorderStyle="none"
                                                    Visible="false">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item Code">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item Name">
                                            <ItemStyle HorizontalAlign="left" />
                                            <HeaderStyle VerticalAlign="Middle" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblOutletName" runat="server" Text='<%# Bind("Dscription") %>' BorderStyle="none">
                                                </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="LineNum">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblLineNum" runat="server" Text='<%# Bind("LineNum") %>' BorderStyle="none"
                                                    Width="70px" Style="text-align: center" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="BatchNo">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtBatchNo" runat="server" Text='<%# Bind("BatchNo") %>' BorderStyle="none"
                                                    Width="100px" Style="text-align: center" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Quantity">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtQuantity" runat="server" Text='<%# Bind("Quantity") %>' BorderStyle="none"
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
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnAddNewBatch" runat="server" Text="Add New Batch" Style="background-image: url(/Images/bgButton.png);
                                background-repeat: no-repeat;" OnClick="btnAddNewBatch_Click" />
                            <asp:Button ID="btnAccept" runat="server" Text="OK" OnClick="btnAccept_Click" Style="background-image: url(/Images/bgButton.png);
                                background-repeat: no-repeat; " Width="80px" />
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
