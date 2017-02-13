<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs" Inherits="Suki.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>
        Change Password
    </h2>
    <hr />
    <table width="100%">
        <tr>
            <td style="font-weight: bold; text-transform: capitalize; font-variant: normal;text-align:right; width:40%;">
                Old Password:
            </td>
            <td style="text-align:left; width:60%;">
                <asp:TextBox ID="txtOldPass" runat="server" Width="200px" TextMode="Password"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="font-weight: bold; text-transform: capitalize; font-variant: normal;text-align:right;">
                New Password:
            </td>
            <td style="text-align:left;">
                <asp:TextBox ID="txtNewPass" runat="server" Width="200px" TextMode="Password"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="font-weight: bold; text-transform: capitalize; font-variant: normal;text-align:right;">
                Confirm Password:
            </td>
            <td style="text-align:left;">
                <asp:TextBox ID="txtConfirmPass" runat="server" Width="200px" 
                    TextMode="Password"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Style="background-image: url('/Images/bgButton.png');
                    background-repeat: no-repeat;" Text="Change" Width="80px" />
            </td>
            <td style="text-align:left;">
                <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
            </td>
        </tr>
    </table>
    <hr />
</asp:Content>
