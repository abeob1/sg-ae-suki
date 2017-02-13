<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvalidPage.aspx.cs" Inherits="Suki.InvalidPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Access Denied</title>
    <script>
        function Close() {
            window.open('', '_self', ''); window.close();
        }
</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        The page opened already. Please close this page.
    </div>
    </form>
</body>
</html>
