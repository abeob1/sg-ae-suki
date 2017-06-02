<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="ClosingStock.aspx.cs" Inherits="Suki.ClosingStock" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script language="javascript" type="text/javascript">

        function divexpandcollapse(divname) {

            var div = document.getElementById(divname);

            var img = document.getElementById('img' + divname);

            if (div.style.display == "none") {

                div.style.display = "inline";

                img.src = "minus.gif";

            } else {

                div.style.display = "none";

                img.src = "plus.gif";

            }
        }
        function keyPressListener(e) {
            if (e.keyCode == 13) {
                // do something
            }
        }
    </script>
    <h2>
        New Report Monthly Closing Stock</h2>
    <div style="margin-left: 5px; width: 99%;">
        <hr />
        <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table style="margin-left: 0px" class="MenuHeader" width="100%">
                    <tr>
                        <td style="width: 70px;">
                            Month :
                        </td>
                        <td style="width: 100px;">
                            <asp:DropDownList ID="ddlMonth" runat="server" AutoPostBack="True" 
                                onselectedindexchanged="ddlMonth_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 50px;">
                            Year :
                        </td>
                        <td style="width: 100px;">
                            <asp:DropDownList ID="ddlYear" runat="server" AutoPostBack="True" 
                                onselectedindexchanged="ddlYear_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 5%">
                            Outlet :
                        </td>
                        <td style="width: 363px">
                            <asp:DropDownList ID="ddlOutlet" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlOutlet_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:TextBox ID="txtOutlet" runat="server" Enabled="False" Width="363px" Visible="false"></asp:TextBox>
                        </td>
                        <td style="width: 10%">
                            Stock No :
                        </td>
                        <td style="width: 15%">
                            <asp:Label ID="lblStockNo" runat="server"></asp:Label>
                        </td>
                        <td style="width: 5%">
                        </td>
                        <td style="width: 25%">
                        </td>
                    </tr>
                </table>
                <hr />
                <div style="margin-left: 0px; width: 100%">
                    <asp:GridView ID="gvParentGrid" runat="server" DataKeyNames="CardCode" Width="100%"
                        AutoGenerateColumns="false" OnRowDataBound="gvUserInfo_RowDataBound" GridLines="None"
                        BorderStyle="Solid" BorderWidth="0px" BorderColor="#df5015">
                        <RowStyle BackColor="#E1E1E1" Height="24px"/>
                        <AlternatingRowStyle BackColor="#E1E1E1" Height="24px"/>
                        <HeaderStyle BackColor="#df5015" Font-Bold="true" ForeColor="White" Height="27px" />
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="20px">
                                <ItemTemplate>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="CardCode" HeaderText="Supplier Code" HeaderStyle-HorizontalAlign="Left"
                                ItemStyle-Width="150px" />
                            <asp:BoundField DataField="CardName" HeaderText="Supplier Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="600px" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <tr>
                                        <td colspan="100%">
                                            <asp:GridView ID="grvClosingStock" CssClass="GridInner" runat="server" Width="100%"
                                                BorderColor="White" BackColor="White" AllowSorting="True" AutoGenerateColumns="False"
                                                CellPadding="2" HeaderStyle-Height="27px" OnRowCreated="grvClosingStock_RowCreated"
                                                CellSpacing="2" HeaderStyle-VerticalAlign="Middle" OnPageIndexChanging="grvSearchResult_PageIndexChanging"
                                                 OnRowDataBound="grvSearchResult_RowDataBound">
                                                <PagerSettings Mode="NumericFirstLast" />
                                                <PagerStyle BackColor="#70A0D0" Font-Bold="True" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                                                <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="#">
                                                        <HeaderStyle BorderWidth="1px" Width="20px" VerticalAlign="Middle"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="center" Width="20px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblNo" runat="server" Text="" BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Item Code" >
                                                        <HeaderStyle BorderWidth="1px" Width="100px" VerticalAlign="Middle"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemCode" runat="server" Text='<%# Bind("ItemCode") %>' BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Suki Old Code" >
                                                        <HeaderStyle BorderWidth="1px" Width="150px" VerticalAlign="Middle"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="Center" Width="150px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSukiOldCode" runat="server" Text='<%# Bind("frgnName") %>' BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Description">
                                                        <HeaderStyle BorderWidth="1px" Width="400px" VerticalAlign="Middle"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="Left" Width="400px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblItemDesc" runat="server" Text='<%# Bind("[ItemName]") %>' BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Inventory UoM">
                                                        <HeaderStyle BorderWidth="1px" Width="50px" VerticalAlign="Middle"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="Center" Width="50px" VerticalAlign="Middle" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblUOM" runat="server" Text='<%# Bind("UOM") %>' BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Stock Issued Quantity">
                                                        <HeaderStyle BorderWidth="1px" Width="10px" VerticalAlign="Middle"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="Right" Width="10px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQuantity" runat="server" Text='<%# Bind("Quantity") %>' BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Counted Qty" HeaderStyle-VerticalAlign="Middle" >
                                                        <ItemStyle HorizontalAlign="Right" Width="100px" />
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtReportQty" runat="server" Width="92%" BorderStyle="None" Text='<%# String.Format("{0:N}", Eval("ReportQty")) %>'
                                                                Style="text-align: right" OnKeyPress="return isNumberKey(this, event);" ></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="OnHand Qty" HeaderStyle-VerticalAlign="Middle" Visible="false">
                                                        <HeaderStyle BorderWidth="1px" Width="80px"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblOnHand" runat="server" Text='<%# String.Format("{0:N}", Eval("OnHand")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Cumulative Qty" HeaderStyle-VerticalAlign="Middle">
                                                        <HeaderStyle BorderWidth="1px" Width="80px"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblCumulative" runat="server" Text='<%# String.Format("{0:N}", Eval("Cumulative")) %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Last Purchase Price" HeaderStyle-VerticalAlign="Middle">
                                                        <HeaderStyle BorderWidth="1px" Width="80px"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="Right" Width="80px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblClosingRate" runat="server" Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("avgPrice"))%>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Counted Quantity" HeaderStyle-VerticalAlign="Middle">
                                                        <HeaderStyle BorderWidth="1px" Width="100px"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="Right" Width="100px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTotal" runat="server" Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("TotalReportQty"))%>'
                                                                BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                      <asp:TemplateField HeaderText="Total" HeaderStyle-VerticalAlign="Middle">
                                                        <HeaderStyle BorderWidth="1px" Width="100px"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="Right" Width="100px" />
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTotal1" runat="server" Text='<%#String.Format("{0,-15:#,##0.0000}", Eval("TotalReportQty"))%>'
                                                                BorderStyle="none">
                                                            </asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Remarks" HeaderStyle-VerticalAlign="Middle">
                                                        <HeaderStyle BorderWidth="1px" Width="150px"></HeaderStyle>
                                                        <ItemStyle HorizontalAlign="left" Width="150px" />
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtRemarks" Text='<%# Bind("Remarks") %>' runat="server" Width="99%"
                                                                AutoCompleteType="Disabled" BorderStyle="None"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <HeaderStyle BackColor="#6095C9" Font-Bold="true" ForeColor="#ffffff" Font-Overline="False"
                                                    Height="27px" VerticalAlign="Bottom" />
                                                <EmptyDataTemplate>
                                                    <table class="GridInner" style="border-color: white; width: 100%; background-color: white;"
                                                        border="0" cellspacing="0" cellpadding="0">
                                                        <tr valign="bottom" style="height: 30px; color: white; font-weight: bold; text-decoration: none;
                                                            background-color: rgb(96, 149, 201);">
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                                                #
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 12%;" scope="col">
                                                                Item Code
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 6%;" scope="col">
                                                                Suki Old Code
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 15%;" scope="col">
                                                                Description
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 5%;" scope="col">
                                                                Inventory UOM
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 6%;" scope="col">
                                                                Counted Qty
                                                            </th>
                                                               <th valign="middle" style="border-width: 1px; border-style: solid; width: 6%;" scope="col">
                                                                Cumulative Qty
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 6%;" scope="col">
                                                                Last Purchase Price
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 6%;" scope="col">
                                                                Counted Quantity
                                                            </th>
                                                            <th valign="middle" style="border-width: 1px; border-style: solid; width: 22%;" scope="col">
                                                                Remarks
                                                            </th>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="13">
                                                                No Data
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <table class="GridInner" style="border-color: white; width: 100%; background-color: white;"
                                border="0" cellspacing="0" cellpadding="0">
                                <tr valign="bottom" style="height: 30px; color: white; font-weight: bold; text-decoration: none;
                                    background-color: rgb(96, 149, 201);">
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 3%;" scope="col">
                                        #
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 12%;" scope="col">
                                        Item Code
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 6%;" scope="col">
                                        Suki Old Code
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 15%;" scope="col">
                                        Description
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 5%;" scope="col">
                                        Inventory UOM
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 6%;" scope="col">
                                        Counted Qty
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 6%;" scope="col">
                                        Cumulative Qty
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 6%;" scope="col">
                                        Last Purchase Price
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 6%;" scope="col">
                                       Counted Quantity
                                    </th>
                                    <th valign="middle" style="border-width: 1px; border-style: solid; width: 22%;" scope="col">
                                        Remarks
                                    </th>
                                </tr>
                                <tr>
                                    <td colspan="13">
                                        No Data
                                    </td>
                                </tr>
                            </table>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
                <hr />
                <div style="margin-left: 5px; width: 99%; margin-top: 10px; display: none;">
                    <div style="float: right; font-weight: bold;">
                        <asp:Label ID="lblGrandTotal" runat="server" Text="Closing Stock Value: "></asp:Label>
                        <asp:TextBox ID="txtGrandTotal" runat="server" ReadOnly="True" ForeColor="Red"></asp:TextBox>
                    </div>
                </div>
                <br />
                <br />
                <div style="margin-left: 0px; width: 100%">
                    <table style="width: 100%" class="MenuHeader" border="0">
                        <tr>
                            <td style="height: 26px; width: 30%;" align="left">
                                <div style="float: left">
                                    <asp:Label ID="lblOrderby" runat="server" Height="12px" Font-Size="Larger" Font-Bold="True">Stock Check By : </asp:Label>
                                    &nbsp;
                                    <asp:TextBox ID="txtStockCheckBy" runat="server"></asp:TextBox>
                                </div>
                            </td>
                            <td style="height: 26px; width: 30%;" align="left">
                                <div style="float: left">
                                    <asp:Label ID="lblTotal" runat="server" Height="12px" Font-Size="Larger" Font-Bold="True">Submitted By : </asp:Label>&nbsp;
                                    &nbsp;<asp:TextBox ID="txtSubmittedBy" runat="server" Enabled="false"></asp:TextBox>
                                </div>
                            </td>
                            <td style="width: 40%" colspan="2" align="left">
                                <div style="float: left; margin-top: 2px">
                                    <div style="float: left; margin-top: 12px">
                                        <asp:Label ID="Label1" runat="server" Font-Size="Larger" Font-Bold="True">Remarks : </asp:Label>&nbsp;
                                    </div>
                                    <asp:TextBox ID="txtRemarks" runat="server" Height="34px" TextMode="MultiLine" Width="367px"></asp:TextBox>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="margin-left: 0px; margin-top: 5px;">
                    <hr />
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnUpdate"  runat="server" Text="Submit" Style="background-image: url('/Images/bgButton.png');
                                    background-repeat: no-repeat;" OnClick="btnUpdate_Click" Width="80px" OnClientClick='return confirm("You cannot change this document after you have submitted. Continue?");' />
                            </td>
                             <td>
                                <asp:Button ID="btnSaveAsDraft" runat="server" Text="Save As Draft" Style="background-image: url('/Images/bgButton.png');
                                    background-repeat: no-repeat;" Width="100px" onclick="Button1_Click" />
                            </td>
                             <td>
                                <asp:Button ID="btnDeleteDraft" runat="server" Text="Delete Draft" Style="background-image: url('/Images/bgButton.png');
                                    background-repeat: no-repeat;" Width="100px" 
                                     onclick="btnDeleteDraft_Click" />
                            </td>
                            <td>
                                <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
                <asp:Timer ID="Timer1" runat="server" Interval="1" OnTick="Timer1_Tick" />
            </ContentTemplate>
        </ajax:UpdatePanel>
    </div>
</asp:Content>
