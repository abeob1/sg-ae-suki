<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="POSearch.aspx.cs" Inherits="Suki.POSearch" %>

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
        function OpenSOList(docEntry, vendorCode, companyCode) {
            var url = "SOList.aspx?PODocEntry=" + docEntry;
            url += "&POVendorCode=" + vendorCode;
            url += "&CompanyCode=" + companyCode;
            Main.openCustomDialog(url, 800, 610);
        }
        function OpenVendor(companyCode,whsCode) {
            var url = "VendorPopup.aspx?CompanyCode=" + companyCode + "&WareHouse=" + whsCode;
            Main.openCustomDialog(url, 600, 610);
        }
    </script>
    <h2>
        PO Listing
    </h2>
    <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div style="margin-left: 5px; width: 99%;">
                <hr />
                <table style="width: 100%; background-color: #D1D4D8;">
                    <tr>
                        <td width="120" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                            Issued Date From :
                        </td>
                        <td width="160px">
                            <asp:TextBox ID="txtDateFrom" runat="server" Width="120px"></asp:TextBox>
                            <cc1:CalendarExtender ID="txtDeliDateFrom_CalendarExtender" runat="server" PopupButtonID="Image1"
                                TargetControlID="txtDateFrom" Format="dd/MM/yyyy">
                            </cc1:CalendarExtender>
                            <asp:ImageButton ID="Image1" runat="Server" AlternateText="Click to show calendar"
                                ImageUrl="~/Images/Calendar_scheduleHS.png" />
                        </td>
                        <td style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                            Delivery Date From :
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtDeliDateFrom" runat="server" Width="120px"></asp:TextBox>
                            <cc1:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtDeliDateFrom"
                                PopupButtonID="Image2" Format="dd/MM/yyyy">
                            </cc1:CalendarExtender>
                            <asp:ImageButton ID="Image2" runat="Server" AlternateText="Click to show calendar"
                                ImageUrl="~/Images/Calendar_scheduleHS.png" />
                        </td>
                        <td style="font-weight: bold; text-transform: capitalize; font-variant: normalwidth:100px;">
                            <asp:Label ID="Label1" runat="server" Text="Change to Status:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlChangeStatus" runat="server">
                                <asp:ListItem Text="All" Value="A"></asp:ListItem>
                                <asp:ListItem Text="Sent to HQ" Value="N"></asp:ListItem>
                                <asp:ListItem Text="Sent to Supplier" Value="Y"></asp:ListItem>
                                <asp:ListItem Text="Sent to Supplier*" Value="S"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td width="120px" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                            Issued Date To :
                        </td>
                        <td width="150px">
                            <asp:TextBox ID="txtDateTo" runat="server" Width="120px"></asp:TextBox>
                            <cc1:CalendarExtender ID="txtDeliDateTo_CalendarExtender" runat="server" PopupButtonID="ImageButton1"
                                TargetControlID="txtDateTo" Format="dd/MM/yyyy">
                            </cc1:CalendarExtender>
                            <asp:ImageButton ID="ImageButton1" runat="Server" AlternateText="Click to show calendar"
                                ImageUrl="~/Images/Calendar_scheduleHS.png" />
                        </td>
                        <td style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                            Delivery Date To :
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtDeliDateTo" runat="server" Width="120px"></asp:TextBox>
                            <cc1:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDeliDateTo"
                                PopupButtonID="ImageButton2" Format="dd/MM/yyyy">
                            </cc1:CalendarExtender>
                            <asp:ImageButton ID="ImageButton2" runat="Server" AlternateText="Click to show calendar"
                                ImageUrl="~/Images/Calendar_scheduleHS.png" />
                            </td>
                         <td  width="120px" style="font-weight: bold; text-transform: capitalize; font-variant: normal">Supplier :</td>
                        <td>
                            <asp:TextBox ID="txtVendorCode" runat="server" AutoPostBack="True" 
                                OnTextChanged="txtVendorCode_TextChanged" Width="80px"></asp:TextBox>
                            <asp:TextBox ID="txtVendorName" runat="server" Width="277px"></asp:TextBox>
                            <asp:Button ID="btnSelectVendor" runat="server" OnClick="btnSelectVendor_Click" 
                                Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;" 
                                Text="..." />
                        </td>
                    </tr>
                    <tr>
                        <td width="120px" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                            PO.Status :
                        </td>
                        <td width="150px">
                            <asp:DropDownList ID="ddlStatus" runat="server" Width="125px">
                                <asp:ListItem Text="All" Value="A"></asp:ListItem>
                                <asp:ListItem Text="Closed" Value="C"></asp:ListItem>
                                <asp:ListItem Text="Open" Value="O"></asp:ListItem>
                                  <asp:ListItem Text="Canceled" Value="Y"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="font-weight: bold; text-transform: capitalize; font-variant: normal; width: 130px;"
                            valign="middle">
                            PO Number :
                            </td>
                        <td align="left" style="font-weight: bold;width:170px;" >
                            <asp:TextBox ID="txtPONo" runat="server" Width="121px"></asp:TextBox>
                            &nbsp;</td>
                         <td>
                             </td>
                        <td> </td>
                    </tr>
                   <%-- <%if (Session[Suki.Utils.AppConstants.IsSupperUser].ToString().ToUpper() == "Y" && Session[Suki.Utils.AppConstants.CompanyCode].ToString().ToUpper() == System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString().ToUpper())
                      { %>--%>
                    <tr>
                        <td style="font-weight: bold;">
                            <asp:Label ID="lblCompany" runat="server" Text="Company:"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlCompany" runat="server"   AutoPostBack="true"
                                onselectedindexchanged="ddlCompany_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                         <td style="font-weight: bold;">
                            <asp:Label ID="Label2" runat="server" Text="Outlet:"></asp:Label>
                        </td>
                        <td style="font-weight: bold;">   
                            <asp:DropDownList ID="drpOrderWareHouse" 
                                runat="server" 
                                onselectedindexchanged="drpOrderWareHouse_SelectedIndexChanged" 
                                AutoPostBack="True">
                            </asp:DropDownList>
                           </td>
                           <td  colspan="2">
                               <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" 
                                   Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;" 
                                   Text="Search" Width="80px" />
                        </td>
                    </tr>
                   <%-- <%} %>--%>
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
                        <asp:TemplateField>
                            <ItemStyle HorizontalAlign="Center" Width="2%" />
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkheader" runat="server" onclick="CheckAll(this)" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkChild" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Action" SortExpression="DocNum" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="40px" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkView" CommandName="ViewPO" runat="server" Text='View' OnClick="lnkPONo_Click"></asp:LinkButton>
                                <asp:LinkButton ID="lnkSO" CommandName="ViewSO" runat="server" Visible="false" Text='View SO'
                                    OnClick="lnkSO_Click"></asp:LinkButton>
                                <asp:HiddenField ID="hdnEmail" runat="server" Value='<%# Bind("E_Mail") %>'></asp:HiddenField>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Action" HeaderStyle-VerticalAlign="Middle" Visible="false">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="60px" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkCopy" runat="server" CommandName="Copy" BorderStyle="none"
                                    Text='Copy To GRPO'>
                                </asp:LinkButton>
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
                           <asp:TemplateField HeaderText="Delivery Date" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDelDate" runat="server" Text='<%# Bind("DocDueDate","{0:dd/MM/yyyy}")  %>'
                                    BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Issued Date" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                            <ItemTemplate>
                                <asp:Label ID="lblPODate" runat="server" Text='<%# Bind("DocDate","{0:dd/MM/yyyy}") %>'
                                    BorderStyle="none">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="PO No." HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkPONo" CommandName="ViewPO" runat="server" Text='<%# Bind("DocNum") %>'
                                    OnClick="lnkPONo_Click" Enabled="false"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Supplier Name" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="left" Width="200px" />
                            <ItemTemplate>
                                <asp:Label ID="lblItemDesc" runat="server" Text='<%# Bind("CardName") %>' BorderStyle="none"
                                    Style="margin-left: 5px">
                                </asp:Label>
                                <asp:HiddenField ID="hdnCardCode" runat="server" Value='<%# Bind("CardCode") %>'>
                                </asp:HiddenField>
                                <asp:HiddenField ID="hdnWhsCode" runat="server" Value='<%# Bind("U_AB_POWhsCode") %>'>
                                </asp:HiddenField>
                                <asp:HiddenField ID="hdnComment" runat="server" Value='<%# Bind("Comments") %>'>
                                </asp:HiddenField>
                                <asp:HiddenField ID="hdnUrgent" runat="server" Value='<%# Bind("U_AB_Urgent") %>'>
                                </asp:HiddenField>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Outlet ID" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                            <ItemTemplate>
                                 <asp:Label ID="lblOutletID" runat="server" Text='<%# Bind("U_AB_POWhsCode") %>' BorderStyle="none"
                                   >
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Remarks" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Left" Width="60px" />
                            <ItemTemplate>
                                 <asp:Label ID="lblRemarks" runat="server" Text='<%# Bind("U_AB_PORemarks") %>' BorderStyle="none"
                                   >
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="User ID & Name" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="80px" />
                            <ItemTemplate>
                                <asp:Label ID="lblUserID" runat="server" Text='<%# Bind("U_AB_UserCode") %>' BorderStyle="none"
                                    Style="margin-left: 5px">
                                </asp:Label>
                                -
                                <asp:Label ID="lblUserName" runat="server" Text='<%# Bind("UserName") %>' BorderStyle="none"
                                    Style="margin-left: 5px">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataFormatString="{0,-15:#,##0.0000}" DataField="DocTotal" HeaderText="Total"
                            ReadOnly="true">
                            <ItemStyle HorizontalAlign="Right" Width="60px" />
                            <HeaderStyle VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="PO.Status" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="40px" />
                            <ItemTemplate>
                                <asp:Label ID="lblDocStatus" runat="server" Text='<%# Bind("DocStatus") %>' BorderStyle="none"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="SO.Status" HeaderStyle-VerticalAlign="Middle" Visible = "false">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="40px" />
                            <ItemTemplate>
                                <asp:Label ID="lblSOStatus" runat="server" Text='<%# Bind("SOStatus") %>' BorderStyle="none"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="60px" />
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("U_AB_SentSupplier") %>'
                                    BorderStyle="none"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Converted to SO" HeaderStyle-VerticalAlign="Middle"
                            Visible="false">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="20px" />
                            <ItemTemplate>
                                <asp:Label ID="lblConvertSO" runat="server" Text='<%# Bind("ConvertedToSO") %>' BorderStyle="none"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField HeaderText="Time" HeaderStyle-VerticalAlign="Middle">
                            <HeaderStyle VerticalAlign="Middle" />
                            <ItemStyle HorizontalAlign="Center" Width="40px" />
                            <ItemTemplate>
                                <asp:Label ID="lblTime" runat="server" Text='<%# Bind("U_AB_Time") %>' BorderStyle="none"></asp:Label>
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
                                 <td align="center" style="width: 250px;">
                                    Delivery Date
                                </td>
                                <td align="center" style="width: 80px;">
                                    Issued Date
                                </td>
                                <td align="center" style="width: 50px;">
                                    PO No.
                                </td>
                                <td align="center" style="width: 250px;">
                                    Supplier Name
                                </td>
                                  <td align="center" style="width: 80px;">
                                    Outlet ID
                                </td>
                                <td align="center" style="width: 100px;">
                                    User ID & Name
                                </td>
                                <td align="center" style="width: 60px;">
                                    Total
                                </td>
                                <td align="center" style="width: 70px;">
                                    PO.Status
                                </td>
                                <td align="center" style="width: 70px;">
                                    Status
                                </td>
                                <td align="center" style="width: 70px;">
                                    Time
                                </td>
                                <%-- <td align="center" style="width: 70px;">
                                    Converted to SO
                                </td>--%>
                            </tr>
                            <tr>
                                <td colspan="12">
                                    No Data
                                </td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                </asp:GridView>
                <hr />
                <table style=" width: 100%; font-weight:bold; color:Red">
                <tr>
                <td align="right">
                Grand Total : <asp:Label ID="lblTotal" runat="server"></asp:Label>
                </td>
                </tr>
                </table>
                <hr />
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnSendPO" runat="server" Text="Process" Style="background-image: url('/Images/bgButton.png');"
                                Visible="false" OnClick="btnSendPO_Click" Width="80px" />
                            <asp:Button ID="btnExportPDF" runat="server" Text="Export To PDF" Style="background-image: url('/Images/bgButton.png');"
                                Visible="false" OnClick="btnExportPDF_Click" />
                            <asp:Button ID="btnExportExcel" runat="server" Text="Export To Excel" Style="background-image: url('/Images/bgButton.png');"
                                Visible="false" OnClick="btnExportExcel_Click" />
                        </td>
                        <td>
                            <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblMissingPoError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                </table>
                <asp:Timer ID="Timer1" runat="server" Interval="1" OnTick="Timer1_Tick" />
            </div>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
