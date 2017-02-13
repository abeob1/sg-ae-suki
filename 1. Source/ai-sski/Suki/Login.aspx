<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Suki.Login"
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%= txtUserName.ClientID %>").focus();
        });
        function OpenMain() {
            var dimensions = 'directories=no,titlebar=yes,toolbar=no,location=1,status=yes,menubar=no,scrollbars=yes,resizable=yes,height=' + screen.height + ',width=' + screen.width + '';
            if (navigator.appName != "Microsoft Internet Explorer") {
                window.opener = self;
            }
            else {
                window.opener = true;
            }
            var newWindow = window.open('Default.aspx', '_blank', dimensions);
            if (isIE() == 8) {
                newWindow.location.href = "Default.aspx"
            }
            var browser = get_browser();
            if (browser == "Chrome") {
                window.open("about:blank", "_self");
                window.close();
            }
            else
                if (browser == "Firefox") {
                    window.open("about:blank", "_self");
                    window.close();
                }
                else {
                    window.open('', '_self', '');
                    window.close();
                    window.moveTo(0, 0);
                    window.resizeTo(screen.width, screen.height - 200);
                }
        }
        function isIE() {
            var myNav = navigator.userAgent.toLowerCase();
            return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
        }
        function get_browser() {
            var ua = navigator.userAgent, tem, M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
            if (/trident/i.test(M[1])) {
                tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
                return 'IE ' + (tem[1] || '');
            }
            if (M[1] === 'Chrome') {
                tem = ua.match(/\bOPR\/(\d+)/)
                if (tem != null) { return 'Opera ' + tem[1]; }
            }
            M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
            if ((tem = ua.match(/version\/(\d+)/i)) != null) { M.splice(1, 1, tem[1]); }
            return M[0];
        }

        function get_browser_version() {
            var ua = navigator.userAgent, tem, M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
            if (/trident/i.test(M[1])) {
                tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
                return 'IE ' + (tem[1] || '');
            }
            if (M[1] === 'Chrome') {
                tem = ua.match(/\bOPR\/(\d+)/)
                if (tem != null) { return 'Opera ' + tem[1]; }
            }
            M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
            if ((tem = ua.match(/version\/(\d+)/i)) != null) { M.splice(1, 1, tem[1]); }
            return M[1];
        }
    </script>
    <div style="font-family: Trebuchet MS; margin-top: 70px;">
        <table border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td align="center">
                    <img src="Images/logo.jpg" alt="logo" width="400px" />
                    <table border="1" cellpadding="0" cellspacing="0" style="width: 400px; margin-top: 40px;
                        border-color: #A5A5A5; border-collapse: collapse">
                        <tr>
                            <td style="height: 30px; border: 0px solid #A5A5A5; background-color: #660000; color: #fff;
                                font-weight: bold; font-size: 16px;" align="center">
                                <strong>Login</strong>
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 144; border: 1px solid #A5A5A5;">
                                <asp:UpdatePanel ID="updatePanel" runat="server">
                                    <ContentTemplate>
                                        <table border="0" cellpadding="5" cellspacing="1" align="center" style="border-color: #A5A5A5;
                                            border-collapse: collapse; margin-top: 10px">
                                            <tr>
                                                <td align="left">
                                                    <span style="font-weight: bolder; text-transform: capitalize; font-variant: normal;
                                                        height: 144;">User ID:</span>
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtUserName" runat="server" Width="196px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtUserName"
                                                        Display="Dynamic" ErrorMessage="Please enter User Name">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                                                    Password:
                                                </td>
                                                <td align="left">
                                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" ValidationGroup="validateLogin"
                                                        Width="196px"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="rfvPasword" runat="server" ControlToValidate="txtPassword"
                                                        Display="Dynamic" ErrorMessage="Please enter password">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left" style="font-weight: bold; text-transform: capitalize; font-variant: normal">
                                                    Company:
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="drdOutlet" runat="server" Height="20px" Width="200px">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="drdOutlet"
                                                        Display="Dynamic" ErrorMessage="Please enter password">*</asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2" align="right">
                                                    <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" Style="background-image: url('/Images/bgButton.png');
                                                        background-repeat: no-repeat;" Width="80px" Height="30px" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" align="center">
                                                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
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
