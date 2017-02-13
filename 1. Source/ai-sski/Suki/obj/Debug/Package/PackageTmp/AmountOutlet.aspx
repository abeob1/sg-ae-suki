<%@ Page Title="" Language="C#" MasterPageFile="~/Popup.Master" AutoEventWireup="true"
    CodeBehind="AmountOutlet.aspx.cs" Inherits="Suki.AmountOutlet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:HiddenField ID="hdnCompanyCode" runat="server" />
                <asp:HiddenField ID="hdnCardCode" runat="server" />
                <asp:HiddenField ID="hdnisUpdate" runat="server" />
                <table id="Table1" width="100%" style="font-weight: bold; background-color: #D1D4D8">
                    <tr>
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
                <hr />
              <asp:GridView ID="grdItem" runat="server" CssClass="GridInner" Width="100%" BorderColor="White"
                                    BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                    HeaderStyle-Height="27px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                                   OnRowDataBound="grdItem_RowDataBound">
                                    <RowStyle BackColor="#D9E0ED" ForeColor="Black" BorderColor="White" BorderWidth="2px"
                                        Height="25px" />
                                    <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" Height="25px" />
                                    <Columns>
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
                                        <asp:TemplateField HeaderText="Daily Min Amount">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle VerticalAlign="Middle" Width="100px" />
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtMinAmt" runat="server" Text='<%# Bind("MinAmt") %>' BorderStyle="none"
                                                    Width="70px" OnKeyPress="return isNumberKey(this, event);" Style="text-align: right" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Daily Max Amount">
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
                <br />
                <table>
                    <tr>
                        <td align="left">
                            <asp:Button ID="btnSave" runat="server" Text="Save" Style="background-image: url('/Images/bgButton.png');
                                background-repeat: no-repeat;" OnClick="btnSave_Click" Width="80px" />
                        </td>
                        <td align="left">
                            <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
