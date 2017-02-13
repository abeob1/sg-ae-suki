<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DebugPage.aspx.cs" Inherits="Suki.DebugPage" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <table border="0" width="100%">
        <tr>
            <td>
                ObjType
            </td>
            <td>
                <asp:TextBox ID="txtObjType" runat="server"></asp:TextBox>
            </td>
            <td>
                Company
            </td>
            <td>
                &nbsp;
                <asp:TextBox ID="txtCompany" runat="server"></asp:TextBox>
            </td>
            <td>
                IsUpdate
            </td>
            <td>
                <asp:CheckBox ID="chkIsUpdate" runat="server" />
            </td>
            <td>
                Key
            </td>
            <td>
                <asp:TextBox ID="txtKey" runat="server"></asp:TextBox>
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td>
                XML
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtXml" runat="server" TextMode="MultiLine" Height="521px" 
                    Width="100%"></asp:TextBox>
            </td>
        </tr>
    </table>
    <table width="100%"><tr><td style="width:60px;">
        <asp:Button ID="btnDebug" runat="server" onclick="btnDebug_Click" 
            Text="Debug" />
        </td><td>
            <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
        </td></tr></table>
    </form>
</body>
</html>
