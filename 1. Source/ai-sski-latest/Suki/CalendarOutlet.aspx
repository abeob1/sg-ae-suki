<%@ Page Title="" Language="C#" MasterPageFile="~/Popup.Master" AutoEventWireup="true"
    CodeBehind="CalendarOutlet.aspx.cs" Inherits="Suki.CalendarOutlet" %>

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
                <asp:GridView ID="grvCalendar" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                    BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                    HeaderStyle-Height="27px" CellSpacing="2" HeaderStyle-VerticalAlign="Middle"
                    OnRowDataBound="grdItem_RowDataBound">
                    <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                    <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                    <Columns>
                        <asp:TemplateField HeaderText="Outlet Code">
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemTemplate>
                                <asp:Label ID="lblOutletCode" runat="server" BorderStyle="none" Text='<%# Bind("OutletCode") %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Outlet Name">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemTemplate>
                                <asp:Label ID="lblOutletName" runat="server" BorderStyle="none" Text='<%# Bind("OutletName") %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Mon.">
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemTemplate>
                                <asp:CheckBox ID="chkMon" runat="server" Checked='<%#Convert.ToBoolean(Eval("Mon")) %>'
                                    BorderStyle="none" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged">
                                </asp:CheckBox>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tue.">
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemTemplate>
                                <asp:CheckBox ID="chkTue" runat="server" Checked='<%#Convert.ToBoolean(Eval("Tue")) %>'
                                    BorderStyle="none" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Wed.">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:CheckBox ID="chkWed" runat="server" Checked='<%#Convert.ToBoolean(Eval("Wed")) %>'
                                    BorderStyle="none" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Thu.">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:CheckBox ID="chkThu" runat="server" Checked='<%#Convert.ToBoolean(Eval("Thu")) %>'
                                    BorderStyle="none" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Fri.">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:CheckBox ID="chkFri" runat="server" Checked='<%#Convert.ToBoolean(Eval("Fri")) %>'
                                    BorderStyle="none" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sat.">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSat" runat="server" Checked='<%#Convert.ToBoolean(Eval("Sat")) %>'
                                    BorderStyle="none" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sun.">
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSun" runat="server" Checked='<%#Convert.ToBoolean(Eval("Sun")) %>'
                                    BorderStyle="none" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle BackColor="#6095C9" Font-Bold="true" ForeColor="#ffffff" Font-Overline="False"
                        Height="27px" VerticalAlign="Bottom" />
                    <EmptyDataTemplate>
                        <table class="GridInner" style="width: 100%; border-color: White;" border="1" rules="all"
                            cellspacing="2" cellpadding="2">
                            <tr valign="middle" style="height: 27px; color: white; font-weight: bold; text-decoration: none;
                                background-color: rgb(96, 149, 201);">
                                <th>
                                    <span>Outlet Code</span>
                                </th>
                                <th>
                                    <span>Mon.</span>
                                </th>
                                <th>
                                    <span>Tue.</span>
                                </th>
                                <th>
                                    <span>Wed.</span>
                                </th>
                                <th>
                                    <span>Thu.</span>
                                </th>
                                <th>
                                    <span>Fri.</span>
                                </th>
                                <th>
                                    <span>Sat.</span>
                                </th>
                                <th>
                                    <span>Sun.</span>
                                </th>
                            </tr>
                            <tr>
                                <td colspan="8">
                                    <span>No Data</span>
                                </td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                </asp:GridView>
                <br />
                <table>
                    <tr>
                        <td align="left">
                            <asp:Button ID="btnSave" runat="server" Text="Save" Style="background-image: url('/Images/bgButton.png');
                                background-repeat: no-repeat;" OnClick="btnSave_Click" Width="88px" />
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
