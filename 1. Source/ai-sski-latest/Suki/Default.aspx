<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Suki.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        var key = "";
        var isNS = (navigator.appName == "Netscape") ? 1 : 0;
        if (navigator.appName == "Netscape") document.captureEvents(Event.MOUSEDOWN || Event.MOUSEUP);
        function mischandler() {
            return false;
        }

        function keyhandler(en) {
            key = en.keyCode;
        } 

        function mousehandler(e) {
            var myevent = (isNS) ? e : event;
            var eventbutton = (isNS) ? myevent.which : myevent.button;
            if ((eventbutton == 3)) { alert("Right-click Not Allowed"); return false; }
            else if ((eventbutton == 2)) { alert("Mouse center-click Not Allowed"); return false; }
            else if ((key == 17) && (eventbutton == 1)) { alert("Control + Left-click Not Allowed"); key = 0; return false; }
        }
        document.oncontextmenu = mischandler;
        document.onmousedown = mousehandler;
        document.onmouseup = mousehandler;
        document.onkeydown = keyhandler;

    </script>
    <div style="font-family: Trebuchet MS; margin-top: 50px;" oncontextmenu="return false">
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td align="center" valign="top">
                    <asp:Image ID="ImgLogo" runat="server" AlternateText="SAP Logo" Width="300px" Height="250px" />
                    <table border="1" cellpadding="0" cellspacing="0" style="width: 100%; margin-top: 30px;
                        border-color: #DDDDDD">
                        <tr>
                            <td style="height: 25px; border: 0px solid #4297d7; background-color: #660000; color: #fff;
                                font-weight: bold;" align="left">
                                <strong>
                                    <asp:Label ID="lblCompany" runat="server" Font-Size="9pt"></asp:Label></strong>
                            </td>
                            <td style="height: 25px; border: 0px solid #4297d7; background-color: #660000; color: #fff;
                                font-weight: bold;" align="right">
                                <strong>
                                    <asp:Label ID="lblDate" runat="server" Font-Size="9pt"></asp:Label></strong>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table style="width: 100%;">
            <tr>
                <td>
                    <table style="width: 300px;" align="left" border="0" class="sukiMenu">
                        <%--   <tr style="height: 25px; border: 0px solid #4297d7; background: #2191c0 url('images/bar.png') 50% 50% repeat-x;
                color: #000; font-weight: bold;">
                <td>
                    Create Purchase Order From Full List
                </td>
            </tr>--%>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #660000; font-weight: bold;">
                            <td align="left">
                                <a href="CreatePO.aspx?Create=N&IsPO=1">Create Purchase Order Process</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #660000; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="OutstandingPO.aspx">Good Receipt Process</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #660000; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="ClosingStock.aspx?Create=N">Stock Taking Process</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #660000; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="PODrafts.aspx">Drafts PO Listing</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #660000; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="POSearch.aspx">All PO Listing</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #660000; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="GRPOList.aspx">Good Receipt PO Listing</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #660000; font-weight: bold;
                            color: White; ">
                            <td align="left">
                                <a href="StockTakingList.aspx">Stock Take Listing</a>
                            </td>
                        </tr>
                         <tr style="height: 25px; border: 0px solid #4297d7; background-color: #660000; font-weight: bold;
                            color: White; ">
                            <td align="left">
                                <a href="StockTakeDrafts.aspx">Stock Take Draft Listing</a>
                            </td>
                        </tr>
                        <%if (System.Configuration.ConfigurationManager.AppSettings["HoldingDatabase"].ToString() == Session[Suki.Utils.AppConstants.CompanyCode].ToString())
                          { %>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #660000; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="SOStatusList.aspx">SO Status</a>
                            </td>
                        </tr>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #660000; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="CompanySetupList.aspx">Outlet Item List Setup</a>
                            </td>
                        </tr>
                        <%} %>
                        <tr style="height: 25px; border: 0px solid #4297d7; background-color: #660000; font-weight: bold;
                            color: White;">
                            <td align="left">
                                <a href="ChangePassword.aspx">Change Password</a>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <p>
                    </p>
                    <table style="width: 100%;">
                        <tr style="height: 40px; border: 0px solid #4297d7; background-color: #D2D2D2; color: #000;
                            font-weight: bold;" align="center">
                            <td>
                                <asp:UpdatePanel ID="updatePanel" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Button ID="btnLogOut" runat="server" Text="Log Out" OnClick="btnLogOut_Click"
                                            Style="background-image: url('/Images/bgButton.png'); background-repeat: no-repeat;"
                                            BorderStyle="Solid" Width="100px" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
