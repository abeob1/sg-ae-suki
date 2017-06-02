Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Data.SqlClient

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class GetDefault
    Inherits System.Web.Services.WebService
    Dim connect As New Connection()
#Region "Default Infor"
    '- Goi ws de lay default vendor/customer
    '- default date = today
    '- Sau khi chon Item: hien thi ItemCode, Description, Quantity=1,  goi ws de lay gia tri default
    '- Doi so luong: goi ws de lay gia tri default
#End Region
   

   
#Region "Apply Promotion"
    '- Neu WS GetPromotion tra ve nhieu hon 1 record thi show page "Promotion Selection" de chon 1, sau do theo logic duoi.

    '- Neu WS GetPromotion Tra ve 1 record thi theo logic duoi.

    'Logic Apply Promotion:
    '1. Discount By Promotion = HeadDscAmt + HeadDscPer*UnitPrice/100 + ProValue
    '2. Unit Price = Unit Price - Discount By Promotion
    '3. So le = field Sole
    '4. ProCode = field ProCode
    '5. Neu ProQty>0: them 1 dong vao grid
    '    Item Code= Item Code cua dong apply
    '    Description = Description cua dong apply
    '    Quantity=ProQty
    '    Unit Price,Discount,Discount By Promotion  = 0
    '    Warehouse = WS GetPromotionWarehouse
    '    U_ProLine = Y
    '    ProCode = field ProCode
    '    So le = field Sole
#End Region
  
    <WebMethod()> _
    Public Function GetCopyFromTo(Type As Integer, ObjType As String) As DataSet
        Dim a As New SAP_Functions
        'Type=1: Copy To
        'Type=2: Copy From
        'ObjType=22: Purchase Order
        Return a.GetGopyFromTo(Type, ObjType)
    End Function
 
    <WebMethod()> _
    Public Function LogOut() As Integer
        Try
            Dim connect As New Connection()
            connect.bConnect = False
            'PublicVariable.oCompany.Disconnect()
        Catch ex As Exception

        End Try
    End Function
    <WebMethod()> _
    Public Function TestConnection(ConnectionString As String) As String
        Dim MyArr As Array
        Dim sErrMsg As String = ""
        Dim connectOk As Integer = 0
        MyArr = ConnectionString.Split(";")
        Dim sCon As String = "server= " + MyArr(3).ToString() + ";database=" + MyArr(0).ToString() + " ;uid=" + MyArr(4).ToString() + "; pwd=" + MyArr(5).ToString() + ";"
        Dim sConnSAP As SqlConnection = New SqlConnection(sCon)

        Try
            sConnSAP.Open()

            Dim newcompayne As New SAPbobsCOM.Company
            newcompayne.CompanyDB = MyArr(0).ToString()
            newcompayne.UserName = MyArr(1).ToString()
            newcompayne.Password = MyArr(2).ToString()
            newcompayne.Server = MyArr(3).ToString()
            newcompayne.DbUserName = MyArr(4).ToString()
            newcompayne.DbPassword = MyArr(5).ToString()
            newcompayne.LicenseServer = MyArr(6)
            If MyArr(7) = 2008 Then
                newcompayne.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008
            ElseIf MyArr(7) = 2012 Then
                newcompayne.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012
            End If

            If newcompayne.Connect() <> 0 Then
                newcompayne.GetLastError(connectOk, sErrMsg)
                Return sErrMsg
            Else
                Return "OK"
            End If

        Catch ex As Exception
            Return ex.ToString
        End Try

        
    End Function

End Class