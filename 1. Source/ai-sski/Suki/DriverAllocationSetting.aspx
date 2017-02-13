<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="DriverAllocationSetting.aspx.cs" Inherits="Suki.DriverAllocationSetting" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function isNumberKey(sender, evt) {
            var txt = sender.value;
            var dotcontainer = txt.split('.');
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (!(dotcontainer.length == 1 && charCode == 46) && charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }
    </script>
    <div>
        <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <h2>
                    Driver Allocation Setup</h2>
                <div style="margin-left: 5px; width: 99%;">
                    <hr />
                    <table border="0" width="100%" style="background-color: #D1D4D8; font-weight: bold;">
                        <tr>
                            <td style="width: 50px">
                                Route: </td>
                            <td style="width: 210px">
                                <asp:DropDownList ID="dllBlock" runat="server" AutoPostBack="True" onselectedindexchanged="dllBlock_SelectedIndexChanged" Width="200px" >
                                </asp:DropDownList>
                            </td>
                            <td style="width: 50px">
                                Code: 
                            </td>
                            <td>
                                <asp:TextBox ID="txtBlock" runat="server" 
                                   ></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <hr />
                    <div style="width: 50%">
                        <asp:GridView ID="grvPO" CssClass="GridInner" runat="server" Width="100%" BorderColor="White"
                            BackColor="White" AllowSorting="True" AutoGenerateColumns="False" CellPadding="2"
                            HeaderStyle-Height="27px" OnRowCreated="grvPO_RowCreated" CellSpacing="2" OnRowEditing="EditItem"
                            OnRowUpdating="UpdateItem" HeaderStyle-VerticalAlign="Middle" OnRowCancelingEdit="CancelEdit"
                            OnRowDeleting="DeleteItem">
                            <RowStyle BackColor="#D9E0ED" BorderColor="White" BorderWidth="2px" Height="25px" />
                            <AlternatingRowStyle BackColor="#EEF1F7" BorderColor="White" BorderWidth="2px" />
                            <Columns>
                                <asp:CommandField HeaderText="Action" ShowDeleteButton="True" ShowEditButton="False"
                                    HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField HeaderText="#">
                                    <ItemStyle HorizontalAlign="Center" Width="2%" />
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblNo" runat="server" Text="" BorderStyle="none">
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Truck Category">
                                    <ItemStyle HorizontalAlign="Center" Width="200px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:DropDownList ID="dllTrunkCategory" runat="server" SelectedValue='<%=3 %>' BorderStyle="none" Width="150px">
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Driver No">
                                    <ItemStyle HorizontalAlign="Center" Width="200px" />
                                    <HeaderStyle VerticalAlign="Middle" Font-Bold="true" />
                                    <ItemTemplate>
                                        <asp:TextBox ID="DriverNo" runat="server" Text='<%# Bind("DriverNo") %>' BorderStyle="none" Style="text-align:center"/>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Vehicle No">
                                    <HeaderStyle VerticalAlign="Middle" />
                                    <ItemStyle HorizontalAlign="Center" Width="200px"/>
                                    <ItemTemplate>
                                        <asp:TextBox ID="VehicleNo" runat="server" Text='<%# Bind("VehicleNo") %>' BorderStyle="none"  Style="text-align:center"/>
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
                                        <th>
                                            <span>#</span>
                                        </th>
                                        <th>
                                            <span>Truck Category</span>
                                        </th>
                                        <th>
                                            <span>Driver No</span>
                                        </th>
                                        <th>
                                            <span>Vehicle No</span>
                                        </th>
                                    </tr>
                                    <tr>
                                        <td colspan="12">
                                            <span>No Data</span>
                                        </td>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                    <hr width="100%" />
                </div>
                <table width="100%" border="0">
                    <tr>
                        <td>
                            <div style="margin-left: 5px; width: 99%;">
                                <hr />
                            </div>
                            <table style="width: 100%">
                                <tr>
                                    <td style="width: 220px;">
                                     <asp:Button ID="btnAddNew" runat="server" Text="Add New" 
                                            Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;
                                            color: White;" BorderStyle="Solid" Width="100px" onclick="Button1_Click" />
                                        <asp:Button ID="btnUpdate" runat="server" Text="OK" OnClick="btnUpdate_Click"
                                            Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;
                                            color: White;" BorderStyle="Solid" Width="100px" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
