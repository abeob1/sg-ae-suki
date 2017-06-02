<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="CompanySetupList.aspx.cs" Inherits="Suki.CompanySetupList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <script type="text/javascript">
     function OpenCalendar() {
         var companyCode = $("#<%= drdOutlet.ClientID %> option:selected").val();
         var companyName = $("#<%= drdOutlet.ClientID %> option:selected").text();
         var url = "CopyTemplate.aspx?CompanyCode=" + companyCode;
         url += "&CompanyName=" + companyName;
         Main.openCustomDialog(url, 600, 550);
     }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>
        Supplier List Setup</h2>
    <asp:HiddenField ID="hdnisUpdate" runat="server" />
    <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="margin-left: 5px; width: 99%;">
                <hr />
                <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                    <tr>
                        <td style="width: 60px">
                               Company :
                        </td>
                        <td style="width: 280px">
                            <asp:DropDownList ID="drdOutlet" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drdOutlet_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 80px">
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
                <hr />
                <table style="width: 100%;" border="0">
                    <tr>
                        <td style="width: 50px;">
                            <asp:Button ID="btnCalendar" runat="server" Text="Setup New" Style="background-image: url('/Images/bgButton.png');
                                background-repeat: no-repeat;" OnClick="btnCalendar_Click" />
                        </td>
                        <td >
                            <asp:Button ID="btnCopyTemplate" runat="server" Text="Copy Template" Style="background-image: url('/Images/bgButton.png');
                                background-repeat: no-repeat;" onclick="btnCopyTemplate_Click"  OnClientClick="OpenCalendar()" />
                        </td>
                        <td style="width: 65%">
                            <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:GridView ID="grvItem" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                                BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                                HeaderStyle-Height="27px" CellSpacing="2" OnRowEditing="EditItem" OnRowUpdating="UpdateItem"
                                HeaderStyle-VerticalAlign="Middle" OnRowCancelingEdit="CancelEdit" OnRowDeleting="DeleteItem"
                                OnRowDataBound="grvSearchResult_RowDataBound"   AllowPaging="True" OnPageIndexChanging="grvSearchResult_PageIndexChanging" PageSize="20">
                                  <PagerSettings Mode="NumericFirstLast" />
                    <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />
                                <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                                <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemStyle HorizontalAlign="Center"  Width="10%"/>
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" OnClick="lnkDelete_Click" OnClientClick='return confirm("Are you sure to delete?");'></asp:LinkButton> | 
                                            <asp:LinkButton ID="lnkView" runat="server" Text='View' BorderStyle="none" OnClick="lblItemCode_Click">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Supplier Code">
                                        <ItemStyle HorizontalAlign="Center" Width="20%" />
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lblItemCode" runat="server" Text='<%# Bind("CardCode") %>' BorderStyle="none"
                                                OnClick="lblItemCode_Click" Enabled="false">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Supplier Name">
                                        <ItemStyle HorizontalAlign="left" Width="80%" />
                                        <HeaderStyle VerticalAlign="Middle" />
                                        <ItemTemplate>
                                            <asp:Label ID="txtDscription" runat="server" Text='<%# Bind("CardName") %>' BorderStyle="none" />
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
                                                <span>Action</span>
                                            </th>
                                            <th style="width: 20%">
                                                <span>Supplier Code</span>
                                            </th>
                                            <th style="width: 80%">
                                                <span>Supplier Name</span>
                                            </th>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <span>No Data</span>
                                            </td>
                                        </tr>
                                    </table>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
                <br />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
