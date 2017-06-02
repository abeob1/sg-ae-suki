Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.Collections

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Mobile
    Inherits System.Web.Services.WebService
    Dim connect As New Connection()
    Dim js As New JavaScriptSerializer()
    Dim oLog As New clsLog()
    Dim sFuncName As String = String.Empty

    Public Shared Sub WriteLog(ByVal Str As String)
        Try
            Dim oWrite As IO.StreamWriter
            Dim FilePath As String
            FilePath = System.Configuration.ConfigurationManager.AppSettings("LogPath") & Date.Now.ToString("yyyyMMdd")

            If IO.File.Exists(FilePath) Then
                oWrite = IO.File.AppendText(FilePath)
            Else
                oWrite = IO.File.CreateText(FilePath)
            End If

            oWrite.Write(Now.ToString() + ":" + Str + vbCrLf)
            oWrite.Close()
        Catch ex As Exception
        End Try
    End Sub
    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Sub DriverLogin(ByVal userId As String, ByVal password As String)
        Try
            sFuncName = "DriverLogin()"
            oLog.WriteToLogFile_Debug("Starting Function ", sFuncName)
            Dim strSQL As String = "select Code  from [@AB_OnlineUser] where Code = @Param1 and U_PWD = @Param2"
            connect.ConnectDB()
            Dim dtUser As DataTable = connect.ObjectGetAll_Query_DB(strSQL, New Object() {userId, password}).Tables(0)
            If dtUser.Rows.Count > 0 Then
                oLog.WriteToLogFile_Debug("Login Successfull", sFuncName)
                Dim strJSON As String = js.Serialize(dtUser(0)("Code"))
                Context.Response.Clear()
                Context.Response.ContentType = "application/json"
                Context.Response.Flush()
                Context.Response.Write(strJSON)
                'Return dtUser(0)("Code")
            Else
                oLog.WriteToLogFile_Debug("No Records Found", sFuncName)
                Dim strJSON As String = js.Serialize("No Records Found!")
                Context.Response.Clear()
                Context.Response.ContentType = "application/json"
                Context.Response.Flush()
                Context.Response.Write(strJSON)
                'Return ""
            End If
            oLog.WriteToLogFile_Debug("Ending Function ", sFuncName)
        Catch ex As Exception
            Dim strJSON As String = js.Serialize(ex)
            oLog.WriteToLogFile(strJSON.ToString(), sFuncName)
            Context.Response.Clear()
            Context.Response.ContentType = "application/json"
            Context.Response.Flush()
            Context.Response.Write(strJSON)
            'Throw ex
        End Try
    End Sub

    '<WebMethod()> _
    '<ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    'Public Function Login(ByVal userId As String, ByVal password As String, ByVal dbName As String) As Integer
    '    Try
    '        Dim dt As New DataSet("Users")
    '        Dim errMsg As String = connect.setSQLDB(dbName)
    '        If errMsg.Length = 0 Then

    '            Dim strSQL As String = "select USERID from OUSR where USER_CODE = '" + userId + "'"
    '            connect.ConnectDB()
    '            Dim dtUser As DataTable = connect.ObjectGetAll_Query_DB(strSQL).Tables(0)
    '            Return dtUser(0)("USERID")
    '            'Return New JavaScriptSerializer().Serialize(4)
    '            'Return True
    '        Else
    '            Return -1 'New JavaScriptSerializer().Serialize(False)
    '            'Return False
    '        End If
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    '    'Dim loginInfo As New OUSR("thaonmt", "password", "Nguyen Mai Thanh Thao")

    'End Function

    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Sub Login(ByVal userId As String, ByVal password As String, ByVal dbName As String)
        Try
            Dim dt As New DataSet("Users")
            Dim errMsg As String = connect.setSQLDB(dbName)
            If errMsg.Length = 0 Then

                Dim strSQL As String = "select USERID from OUSR where USER_CODE = '" + userId + "'"
                connect.ConnectDB()
                Dim dtUser As DataTable = connect.ObjectGetAll_Query_DB(strSQL).Tables(0)
                'Return dtUser(0)("USERID")
                'Return New JavaScriptSerializer().Serialize(4)
                'Return True

                If dtUser.Rows.Count > 0 Then
                    Dim strJSON As String = js.Serialize(dtUser(0)("USERID"))
                    Context.Response.Clear()
                    Context.Response.ContentType = "application/json"
                    Context.Response.Flush()
                    Context.Response.Write(strJSON)
                Else
                    Dim strJSON As String = js.Serialize("No Records Found.")
                    'dtmkrs.Dispose()
                    Context.Response.Clear()
                    Context.Response.ContentType = "application/json"
                    Context.Response.Flush()
                    Context.Response.Write(strJSON)

                End If
            Else
                'Return -1 'New JavaScriptSerializer().Serialize(False)
                'Return False
                Context.Response.Clear()
                Context.Response.ContentType = "application/json"
                Context.Response.Flush()
                Context.Response.Write(-1)
            End If
        Catch ex As Exception
            'Throw ex
            Dim strJSON As String = js.Serialize(ex)
            Context.Response.Clear()
            Context.Response.ContentType = "application/json"
            Context.Response.Flush()
            Context.Response.Write(strJSON)
        End Try
    End Sub

    <WebMethod()> _
 <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function GetNumberOpenDO(ByVal driverNo As String) As Integer

        Dim list As New List(Of OpenDO)()
        Dim dtDBName As DataTable = getDBList()
        Dim sqlStr As String = "select SUM(OpenDOQty) OpenDOQty from ( "
        For Each dr As DataRow In dtDBName.Rows
            Dim dbName As String = dr("U_DBName")

            sqlStr += " select COUNT(od.DocEntry) OpenDOQty "
            sqlStr += " from " + dbName + "..ODLN od"
            sqlStr += " join [@AB_OUTLET] ol on ol.Code = od.U_AB_POWhsCode "
            sqlStr += " where od.U_AB_DriverNo = @Param1 and od.U_AB_DeliveryStatus = 0  and od.DocStatus = 'O' "
            sqlStr += " UNION ALL "
        Next
        sqlStr = sqlStr.Substring(0, sqlStr.Length - 11)
        sqlStr += " ) T "

        connect.ConnectDB()
        Dim dt As New DataTable
        dt = connect.ObjectGetAll_Query_DB(sqlStr, New Object() {driverNo}).Tables(0)
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0)("OpenDOQty")
        Else
            Return 0
        End If

    End Function

    Private Function getDBList() As DataTable
        Dim arr As New ArrayList
        connect.ConnectDB()
        Dim dt As New DataTable
        dt = connect.ObjectGetAll_Query_DB("SELECT U_DBName FROM [@AB_CompanySetup] T0 join sys.Databases T1 on T0.U_DBName COLLATE SQL_Latin1_General_CP1_CS_AS =  T1.name COLLATE SQL_Latin1_General_CP1_CS_AS  where U_DBName is not null and Code <> 'SS'").Tables(0)
        Return dt
    End Function

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Sub GetSummaryOrder(ByVal driverNo As String)

        sFuncName = "GetSummaryOrder()"
        oLog.WriteToLogFile_Debug("Starting Function ", sFuncName)

        Dim list As New List(Of OpenDO)()
        Dim dtDBName As DataTable = getDBList()

        'Dim sqlStr As String = " select Code, Name, Address, SUM(OpenDOQty) OpenDOQty from ("
        'For Each dr As DataRow In dtDBName.Rows
        '    Dim dbName As String = dr("U_DBName")
        '    sqlStr += " select ol.Code, ol.Name, od.Address2 Address, COUNT(od.DocEntry) OpenDOQty "
        '    sqlStr += " from " + dbName + "..ODLN od"
        '    sqlStr += " join [@AB_OUTLET] ol on ol.Code = od.U_AB_POWhsCode "
        '    sqlStr += " where od.U_AB_DriverNo = @Param1 and od.U_AB_DeliveryStatus = 0 and od.DocStatus = 'O' "
        '    sqlStr += " group by ol.Code, ol.Name, od.Address2 "
        '    sqlStr += " UNION ALL "
        'Next
        'sqlStr = sqlStr.Substring(0, sqlStr.Length - 11)
        'sqlStr += " ) T group by Code, Name, Address "

        Dim sqlStr As String = "select Code, Name, SUM(OpenDOQty) OpenDOQty, TotalAddress Address from ("
        For Each dr As DataRow In dtDBName.Rows
            Dim dbName As String = dr("U_DBName")
            sqlStr += " select ol.Code, ol.Name, COUNT(od.DocEntry) OpenDOQty, "
            sqlStr += " isnull(rd.Block, '') + '-' + isnull(rd.Street, '') + '-' + isnull(rd.StreetNo, '') + '-' + "
            sqlStr += " isnull(rd.City, '') + '-' + isnull(rd.Country, '') TotalAddress  "
            sqlStr += " from " + dbName + "..ODLN od "
            sqlStr += " join [@AB_OUTLET] ol on ol.Code = od.U_AB_POWhsCode "
            sqlStr += " join " + dbName + "..CRD1 rd on rd.Address = ol.Code and od.CardCode = rd.CardCode "
            sqlStr += " where od.U_AB_DriverNo = @Param1 and od.U_AB_DeliveryStatus = 0 and od.DocStatus = 'O' "
            sqlStr += " group by ol.Code, ol.Name, rd.Block, rd.Street, rd.StreetNo, rd.City, rd.Country "
            sqlStr += " UNION ALL "
        Next
        sqlStr = sqlStr.Substring(0, sqlStr.Length - 11)
        sqlStr += ") T group by Code, Name, TotalAddress"

        connect.ConnectDB()
        Dim dt As New DataTable
        dt = connect.ObjectGetAll_Query_DB(sqlStr, New Object() {driverNo}).Tables(0)

        For Each row As DataRow In dt.Rows
            Dim order As New OpenDO
            order.U_OutletID = ""
            order.Code = row("Code")
            order.Name = IIf(IsDBNull(row("Name")), "", row("Name"))
            order.OpenDOQty = IIf(IsDBNull(row("OpenDOQty")), 0, row("OpenDOQty"))
            order.Address = row("Address") 'IIf(IsDBNull(row("Address")), "", row("Address"))
            order.U_DBName = ""
            list.Add(order)
        Next
        'Return New JavaScriptSerializer().Serialize(list)
        Dim strJSON As String = js.Serialize(list)
        Context.Response.Clear()
        Context.Response.ContentType = "application/json"
        Context.Response.Flush()
        Context.Response.Write(strJSON)
        oLog.WriteToLogFile_Debug("Ending Function ", sFuncName)
    End Sub

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Sub GetWorkingOrder(ByVal numRecord As Integer, ByVal OutletCode As String, ByVal driverNo As String)

        sFuncName = "GetWorkingOrder()"
        oLog.WriteToLogFile_Debug("Starting Function ", sFuncName)

        Dim list As New List(Of ODLN)()
        Dim dtDBName As DataTable = getDBList()
        Dim sqlStr As String = ""
        For Each dr As DataRow In dtDBName.Rows
            Dim dbName As String = dr("U_DBName")
            sqlStr += " select distinct top (@Param1) do.DocEntry, isnull(m1.BeginStr, '') + CAST(do.DocNum as nvarchar(50)) DocNum, do.CardCode, do.Address2, convert(varchar(10),do.DocDueDate,103) DocDueDate, convert(varchar(10),do.DocDate,103) DocDate, fd.Descr, U_AB_DriverNo, do.NumAtCard, '" + dbName + "' U_DBName "
            sqlStr += ", ol.Code OutletCode, ol.Name OutletName from " + dbName + "..ODLN do join [@AB_OUTLET] ol on ol.Code = do.U_AB_POWhsCode "
            sqlStr += "  left join " + dbName + "..UFD1 fd on isnull(do.U_AB_DeliveryStatus, 0) = fd.FldValue and fd.TableID = 'ODLN' "
            sqlStr += "  join " + dbName + "..CUFD cu on cu.TableID = fd.TableID and cu.AliasID = 'AB_DeliveryStatus' "
            sqlStr += " left join " + dbName + "..NNM1 m1 on do.Series = m1.Series "
            sqlStr += "  where isnull(do.U_AB_DeliveryStatus, 0) =  0 and ol.Code = @Param2 and U_AB_DriverNo = @Param3  and do.DocStatus = 'O' "
            sqlStr += "  UNION ALL "
        Next

        sqlStr = sqlStr.Substring(0, sqlStr.Length - 11)

        Dim dt As New DataTable
        connect.ConnectDB()
        dt = connect.ObjectGetAll_Query_DB(sqlStr, New Object() {numRecord, OutletCode, driverNo}).Tables(0)
        'do.DocEntry, do.DocNum, do.CardCode, do.Address2, do.DocDueDate, fd.Descr, do.U_Driver, do.NumAtCard
        For Each row As DataRow In dt.Rows
            Dim order As New ODLN
            order.DocEntry = row("DocEntry")
            order.DocNum = row("DocNum")
            order.CardCode = IIf(IsDBNull(row("CardCode")), "", row("CardCode"))
            order.Address2 = IIf(IsDBNull(row("Address2")), "", row("Address2"))
            order.DocDueDate = row("DocDueDate")
            order.DocDate = row("DocDate")
            order.Descr = row("Descr")
            order.U_AB_DriverNo = IIf(IsDBNull(row("U_AB_DriverNo")), "", row("U_AB_DriverNo"))

            order.NumAtCard = IIf(IsDBNull(row("NumAtCard")), "", row("NumAtCard"))
            order.U_DBName = row("U_DBName")
            order.OutletCode = row("OutletCode")
            order.OutletName = row("OutletName")

            list.Add(order)
        Next
        'Return New JavaScriptSerializer().Serialize(list)
        Dim strJSON As String = js.Serialize(list)
        Context.Response.Clear()
        Context.Response.ContentType = "application/json"
        Context.Response.Flush()
        Context.Response.Write(strJSON)
        oLog.WriteToLogFile_Debug("Ending Function ", sFuncName)
    End Sub

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Sub GetAllOrder(ByVal OutletCode As String, ByVal driverNo As String)
        sFuncName = "GetAllOrder()"
        oLog.WriteToLogFile_Debug("Starting Function ", sFuncName)


        Dim _date As Date = Date.Now
        Dim strDate = _date.ToString("yyyy-MM-dd")
        Dim prev3Days As Date = _date.AddDays(-7)
        Dim strPrev3Day = prev3Days.ToString("yyyy-MM-dd")

        Dim dtDBName As DataTable = getDBList()
        Dim sqlStr As String = ""

        For Each dr As DataRow In dtDBName.Rows
            Dim dbName As String = dr("U_DBName")
            sqlStr += " select do.DocEntry,  isnull(m1.BeginStr, '') + CAST(do.DocNum as nvarchar(50)) DocNum, do.CardCode, do.Address2, do.DocDueDate, do.DocDate, 'Completed' as Descr, U_AB_DriverNo, do.NumAtCard, '" + dbName + "' U_DBName "
            sqlStr += " , ol.Code OutletCode, ol.Name OutletName from " + dbName + "..OPDN do join [@AB_OUTLET] ol on ol.Code = do.U_AB_POWhsCode "
            'sqlStr += "  left join " + dbName + "..UFD1 fd on isnull(do.U_AB_DeliveryStatus, 0) = fd.FldValue and fd.TableID = 'OPDN' "
            'sqlStr += "  join " + dbName + "..CUFD cu on cu.TableID = fd.TableID and cu.AliasID = 'AB_DeliveryStatus' "
            sqlStr += " left join " + dbName + "..NNM1 m1 on do.Series = m1.Series "
            sqlStr += "  where ol.Code = @Param1 and U_AB_DriverNo = @Param2 and do.DocDueDate between @Param3 and @Param4  and do.DocStatus = 'O' "
            sqlStr += "  UNION ALL "
        Next

        sqlStr = sqlStr.Substring(0, sqlStr.Length - 11)

        Dim list As New List(Of ODLN)()
        connect.ConnectDB()
        Dim dt As New DataTable
        dt = connect.ObjectGetAll_Query_DB(sqlStr, New Object() {OutletCode, driverNo, prev3Days, _date}).Tables(0)

        'do.DocEntry, do.DocNum, do.CardCode, do.Address2, do.DocDueDate, fd.Descr, do.U_Driver, do.NumAtCard
        For Each row As DataRow In dt.Rows
            Dim order As New ODLN
            order.DocEntry = row("DocEntry")
            order.DocNum = row("DocNum")
            order.CardCode = IIf(IsDBNull(row("CardCode")), "", row("CardCode"))
            order.Address2 = IIf(IsDBNull(row("Address2")), "", row("Address2"))
            order.DocDueDate = row("DocDueDate")
            order.DocDate = row("DocDate")

            order.Descr = row("Descr")
            order.U_AB_DriverNo = IIf(IsDBNull(row("U_AB_DriverNo")), "", row("U_AB_DriverNo"))
            ' order.U_Driver =
            order.NumAtCard = IIf(IsDBNull(row("NumAtCard")), "", row("NumAtCard"))
            order.U_DBName = row("U_DBName")
            order.OutletCode = row("OutletCode")
            order.OutletName = row("OutletName")
            list.Add(order)
        Next
        'Return New JavaScriptSerializer().Serialize(list)
        Dim strJSON As String = js.Serialize(list)
        Context.Response.Clear()
        Context.Response.ContentType = "application/json"
        Context.Response.Flush()
        Context.Response.Write(strJSON)
        oLog.WriteToLogFile_Debug("Ending Function ", sFuncName)

    End Sub

    <WebMethod()> _
  <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Sub GetCompletedOrder(ByVal driverNo As String)

        sFuncName = "GetCompletedOrder()"
        oLog.WriteToLogFile_Debug("Starting Function ", sFuncName)

        Dim _date As Date = Date.Now
        Dim strDate = _date.ToString("yyyy-MM-dd")
        Dim prev3Days As Date = _date.AddDays(-7)
        Dim strPrev3Day = prev3Days.ToString("yyyy-MM-dd")

        Dim dtDBName As DataTable = getDBList()
        Dim sqlStr As String = ""

        For Each dr As DataRow In dtDBName.Rows
            Dim dbName As String = dr("U_DBName")
            sqlStr += " select do.DocEntry,  isnull(m1.BeginStr, '') + CAST(do.DocNum as nvarchar(50)) DocNum, do.CardCode, do.Address2, do.DocDueDate, do.DocDate, 'Completed' as Descr, U_AB_DriverNo, do.NumAtCard, '" + dbName + "' U_DBName "
            sqlStr += "  ,ol.Code OutletCode, ol.Name OutletName from " + dbName + "..OPDN do join [@AB_OUTLET] ol on ol.Code = do.U_AB_POWhsCode "
            'sqlStr += "  left join " + dbName + "..UFD1 fd on isnull(do.U_AB_DeliveryStatus, 0) = fd.FldValue and fd.TableID = 'OPDN' "
            'sqlStr += "  join " + dbName + "..CUFD cu on cu.TableID = fd.TableID and cu.AliasID = 'AB_DeliveryStatus' "
            sqlStr += " left join " + dbName + "..NNM1 m1 on do.Series = m1.Series "
            sqlStr += "  where U_AB_DriverNo = @Param1 and do.DocDueDate between @Param2 and @Param3  and do.DocStatus = 'O' "
            sqlStr += "  UNION ALL "
        Next

        sqlStr = sqlStr.Substring(0, sqlStr.Length - 11)

        Dim list As New List(Of ODLN)()
        connect.ConnectDB()
        Dim dt As New DataTable
        oLog.WriteToLogFile_Debug("Query: " & sqlStr, sFuncName)
        oLog.WriteToLogFile_Debug("Parameters: " & driverNo & "," & prev3Days & "," & _date, sFuncName)
        dt = connect.ObjectGetAll_Query_DB(sqlStr, New Object() {driverNo, prev3Days, _date}).Tables(0)

        'do.DocEntry, do.DocNum, do.CardCode, do.Address2, do.DocDueDate, fd.Descr, do.U_Driver, do.NumAtCard
        For Each row As DataRow In dt.Rows
            Dim order As New ODLN
            order.DocEntry = row("DocEntry")
            order.DocNum = row("DocNum")
            order.CardCode = IIf(IsDBNull(row("CardCode")), "", row("CardCode"))
            order.Address2 = IIf(IsDBNull(row("Address2")), "", row("Address2"))
            order.DocDueDate = row("DocDueDate")
            order.DocDate = row("DocDate")

            order.Descr = row("Descr")
            order.U_AB_DriverNo = IIf(IsDBNull(row("U_AB_DriverNo")), "", row("U_AB_DriverNo"))
            ' order.U_Driver =
            order.NumAtCard = IIf(IsDBNull(row("NumAtCard")), "", row("NumAtCard"))
            order.U_DBName = row("U_DBName")
            order.OutletCode = row("OutletCode")
            order.OutletName = row("OutletName")
            list.Add(order)
        Next
        'Return New JavaScriptSerializer().Serialize(list)
        Dim strJSON As String = js.Serialize(list)
        Context.Response.Clear()
        Context.Response.ContentType = "application/json"
        Context.Response.Flush()
        Context.Response.Write(strJSON)
        oLog.WriteToLogFile_Debug("Ending Function ", sFuncName)
    End Sub

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Sub GetOrderDetail(ByVal DocEntry As Integer, ByVal DBName As String)
        'get connect from outlet code
        sFuncName = "GetOrderDetail()"
        oLog.WriteToLogFile_Debug("Starting Function ", sFuncName)

        Dim sisConnect As Connection = getConnectionByDBName(DBName)
        'connect.ConnectDB()
        Dim list As New List(Of DLN1)()
        Dim sqlStr = "select dt.DocEntry, dt.LineNum, dt.ItemCode, dt.Dscription, dt.Quantity, case when IsNull(do.RecQty,0) = 0 then dt.Quantity "
        sqlStr += " else IsNull(do.RecQty,0) end U_RecQty, dt.WhsCode, it.ManbtchNum, dt.UnitMsr, dt.U_AB_POQty from DLN1 dt "
        sqlStr += " join ODLN mas on dt.DocEntry = mas.DocEntry "
        sqlStr += " left join AB_DeliveryOrder do on mas.DocEntry = do.DocEntry and dt.LineNum = do.LineNum "
        sqlStr += " join OITM it on it.ItemCode = dt.ItemCode "
        sqlStr += " where dt.DocEntry = @Param1 "

        Dim dt As New DataTable

        dt = sisConnect.ObjectGetAll_Query_DB(sqlStr, New Object() {DocEntry}).Tables(0)

        For Each row As DataRow In dt.Rows
            Dim detail As New DLN1
            'dt.DocEntry, dt.LineNum, dt.ItemCode, dt.Dscription, dt.Quantity, dt.U_RecQty 
            detail.DocEntry = row("DocEntry")
            detail.LineNum = row("LineNum")
            detail.ItemCode = IIf(IsDBNull(row("ItemCode")), "", row("ItemCode"))
            detail.Dscription = IIf(IsDBNull(row("Dscription")), "", row("Dscription"))
            detail.Quantity = row("Quantity")
            detail.U_RecQty = IIf(IsDBNull(row("U_RecQty")), 0, row("U_RecQty"))
            detail.WhsCode = IIf(IsDBNull(row("WhsCode")), "", row("WhsCode"))
            detail.UnitMsr = IIf(IsDBNull(row("UnitMsr")), "", row("UnitMsr"))
            detail.U_AB_POQty = IIf(IsDBNull(row("U_AB_POQty")), 0, row("U_AB_POQty"))
            list.Add(detail)
        Next
        'Return New JavaScriptSerializer().Serialize(list)
        Dim strJSON As String = js.Serialize(list)
        Context.Response.Clear()
        Context.Response.ContentType = "application/json"
        Context.Response.Flush()
        Context.Response.Write(strJSON)
        oLog.WriteToLogFile_Debug("Ending Function ", sFuncName)
    End Sub

    <WebMethod()> _
  <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function GetOrderDetailComplete(ByVal DocEntry As Integer, ByVal DBName As String) As String
        'get connect from outlet code

        Dim sisConnect As Connection = getConnectionByDBName(DBName)
        'connect.ConnectDB()
        Dim list As New List(Of DLN1)()
        Dim sqlStr = "select dt.DocEntry, dt.LineNum, dt.ItemCode, dt.Dscription, dt.Quantity as U_RecQty, dt.WhsCode, it.ManbtchNum, dt.UnitMsr, "
        sqlStr += " isnull(dt.U_AB_POQty, 0) U_AB_POQty, isnull(U_AB_RDOQty, 0) Quantity from PDN1 dt "
        sqlStr += " join OPDN mas on dt.DocEntry = mas.DocEntry "
        sqlStr += " join OITM it on it.ItemCode = dt.ItemCode "
        sqlStr += " where dt.DocEntry = @Param1 "

        Dim dt As New DataTable

        dt = sisConnect.ObjectGetAll_Query_DB(sqlStr, New Object() {DocEntry}).Tables(0)

        For Each row As DataRow In dt.Rows
            Dim detail As New DLN1
            'dt.DocEntry, dt.LineNum, dt.ItemCode, dt.Dscription, dt.Quantity, dt.U_RecQty 
            detail.DocEntry = row("DocEntry")
            detail.LineNum = row("LineNum")
            detail.ItemCode = IIf(IsDBNull(row("ItemCode")), "", row("ItemCode"))
            detail.Dscription = IIf(IsDBNull(row("Dscription")), "", row("Dscription"))
            detail.Quantity = row("Quantity")
            detail.U_RecQty = IIf(IsDBNull(row("U_RecQty")), 0, row("U_RecQty"))
            detail.WhsCode = IIf(IsDBNull(row("WhsCode")), "", row("WhsCode"))
            detail.UnitMsr = IIf(IsDBNull(row("UnitMsr")), "", row("UnitMsr"))
            detail.U_AB_POQty = IIf(IsDBNull(row("U_AB_POQty")), 0, row("U_AB_POQty"))
            list.Add(detail)
        Next
        Return New JavaScriptSerializer().Serialize(list)
    End Function

    <WebMethod()> _
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Sub InsertABDeliveryOrder(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, ByVal data As String)
        Dim strJSON As String = Nothing
        Dim errMsg As String = Nothing
        Dim strSQL As String = Nothing

        sFuncName = "InsertABDeliveryOrder()"
        Dim i As Integer = 1
        oLog.WriteToLogFile_Debug("Starting Function ", sFuncName)

        oLog.WriteToLogFile_Debug("Log for JSON data " & data, sFuncName)

        Try
            Dim connect As New Connection()

            Dim jss As New JavaScriptSerializer()
            Dim list As List(Of DeliveryOrder) = jss.Deserialize(Of List(Of DeliveryOrder))(data)
            oLog.WriteToLogFile_Debug("Json Count " & list.Count(), sFuncName)

            For Each row As DeliveryOrder In list
                oLog.WriteToLogFile_Debug("Before Connect ", sFuncName)
                connect.ConnectDB()
                oLog.WriteToLogFile_Debug("After Connect ", sFuncName)
                'check whether the Delivery Order is already exists or not, If there, we will update
                oLog.WriteToLogFile_Debug("Before SET ", sFuncName)
                errMsg = connect.setSQLDB(databaseName)
                oLog.WriteToLogFile_Debug("AFTER SET " & databaseName, sFuncName)
                oLog.WriteToLogFile_Debug("Before Query ", sFuncName)

                strSQL = "select COUNT(DocEntry) TotalCount from [" & databaseName & "]..[AB_DeliveryOrder] where DocEntry = " & CInt(row.DocEntry.ToString) & " and LineNum = " & CInt(row.LineNum.ToString) & ""

                oLog.WriteToLogFile_Debug("After Query " & strSQL, sFuncName)
                Dim dtCount As DataTable = connect.ObjectGetAll_Query_DB(strSQL).Tables(0)
                oLog.WriteToLogFile_Debug("Count " & dtCount.Rows.Count, sFuncName)
                If (dtCount.Rows.Count > 0) Then
                    oLog.WriteToLogFile_Debug("Total Count " & dtCount(0)("TotalCount"), sFuncName)
                    If (dtCount(0)("TotalCount") > 0) Then
                        Dim sisConnect As Connection = getConnectionByDBName(databaseName)
                        oLog.WriteToLogFile_Debug("Input " & row.RecQty & row.DocEntry & row.LineNum & databaseName, sFuncName)
                        Dim sqlStr As String = "update AB_DeliveryOrder set RecQty = @Param1 where DocEntry = @Param2 and LineNum = @Param3"
                        sisConnect.ObjectUpateByQuery(sqlStr, New Object() {row.RecQty, row.DocEntry, row.LineNum})
                    Else
                        oLog.WriteToLogFile_Debug("Before Connecting FOR SO and DO Number", sFuncName)
                        errMsg = connect.setSQLDB(databaseName)
                        connect.ConnectDB()
                        Dim strSQL1 As String = "select TOP 1 T1.DocNum [DoNo], BaseEntry [SoNo] from [" & databaseName & "]..DLN1 T0 join [" & databaseName & "]..ODLN T1 on T0.DocEntry = T1.DocEntry where T0.DocEntry = '" & row.DocEntry & "' and T0.LineNum = '" & row.LineNum & "'"
                        oLog.WriteToLogFile_Debug("Before Selecting FOR SO and DO Number", sFuncName)
                        Dim dtDocNum As DataTable = connect.ObjectGetAll_Query_DB(strSQL1).Tables(0)
                        oLog.WriteToLogFile_Debug("After Selecting FOR SO and DO Number", sFuncName)
                        oLog.WriteToLogFile_Debug("docnum Count " & dtDocNum.Rows.Count, sFuncName)
                        For Each row1 As DataRow In dtDocNum.Rows
                            oLog.WriteToLogFile_Debug("loopCount " & 1, sFuncName)
                            oLog.WriteToLogFile_Debug("DO Value " & row1("DoNo"), sFuncName)
                            oLog.WriteToLogFile_Debug("SO Value " & row1("SoNo"), sFuncName)
                        Next

                        oLog.WriteToLogFile_Debug("Input Value " & row.DocEntry & row.LineNum & dtDocNum(0)("DoNo") & dtDocNum(0)("SoNo") & row.ItemCode & row.Dscription & row.Quantity & row.RecQty & row.UnitMsr, sFuncName)
                        Dim params = New Object() {row.DocEntry, row.LineNum, dtDocNum(0)("DoNo"), dtDocNum(0)("SoNo"), row.ItemCode, _
                                                   row.Dscription, row.Quantity, row.RecQty, row.UnitMsr}
                        Dim query As String = String.Empty
                        oLog.WriteToLogFile_Debug("Before Inserting Delivery Order", sFuncName)
                        query = "INSERT INTO [" & databaseName & "]..AB_DeliveryOrder(DocEntry,LineNum,DONo,SONo,ItemCode,Dscription,Quantity,RecQty,UnitMsr)"
                        query += " VALUES(@Param1,@Param2,@Param3,@Param4,@Param5,@Param6,@Param7,@Param8,@Param9)"
                        oLog.WriteToLogFile_Debug("Before COnnecting DB", sFuncName)
                        errMsg = connect.setSQLDB(databaseName)
                        connect.ConnectDB()
                        oLog.WriteToLogFile_Debug("After COnnecting DB", sFuncName)
                        If errMsg.Length = 0 Then
                            oLog.WriteToLogFile_Debug("Before Executing DB", sFuncName)
                            Dim count As Integer = connect.Object_Execute_SAP(query, params)
                            oLog.WriteToLogFile_Debug("After COnnecting DB", sFuncName)
                            oLog.WriteToLogFile_Debug("After Inserting Delivery Order", sFuncName)
                        Else
                            strJSON = js.Serialize(errMsg)
                            Context.Response.Clear()
                            Context.Response.ContentType = "application/json"
                            Context.Response.Flush()
                            Context.Response.Write(strJSON)
                        End If
                    End If
                End If

            Next
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            strJSON = js.Serialize("Server Error - " & ex.Message)
            Context.Response.Clear()
            Context.Response.ContentType = "application/json"
            Context.Response.Flush()
            Context.Response.Write(strJSON)
        End Try
        strJSON = js.Serialize("SUCCESS")
        Context.Response.Clear()
        Context.Response.ContentType = "application/json"
        Context.Response.Flush()
        Context.Response.Write(strJSON)
        oLog.WriteToLogFile_Debug("Ending Function", sFuncName)
    End Sub

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function GetBatch(ByVal DocEntry As Integer, ByVal ItemCode As String, ByVal DBName As String) As String
        Dim sisConnect As Connection = getConnectionByDBName(DBName)

        Dim list As New List(Of BATCHGET)()
        Dim sqlStr = " Select T0.DocEntry, T0.ItemCode, T5.DistNumber, T4.Quantity From DLN1 T0 WITH(NOLOCK)  "
        sqlStr += " Left join OITL T3 with (nolock) on (T3.DocEntry = T0.DocEntry and T3.DocLine = T0.LineNum and T3.DocType = 15 and T3.DefinedQty <> 0)          "
        sqlStr += " Left join ITL1 T4 with (nolock) on T3.LogEntry = T4.LogEntry"
        sqlStr += " Left join OBTN T5 with (nolock) on T5.AbsEntry = T4.MdAbsEntry and T5.ItemCode = T4.ItemCode"
        sqlStr += " Where T0.DocEntry = @Param1 and T0.ItemCode = @Param2"

        Dim dt As New DataTable

        dt = sisConnect.ObjectGetAll_Query_DB(sqlStr, New Object() {DocEntry, ItemCode}).Tables(0)

        For Each row As DataRow In dt.Rows
            Dim detail As New BATCHGET

            detail.DocEntry = row("DocEntry")
            detail.DistNumber = row("DistNumber")
            detail.ItemCode = row("ItemCode")
            detail.Quantity = row("Quantity")
            list.Add(detail)
        Next
        Return New JavaScriptSerializer().Serialize(list)
    End Function

    Private Function createPDFPCell(ByVal _text As String, ByVal _colSpan As Integer) As PdfPCell
        Dim cell As New PdfPCell(New Phrase(New Chunk(_text, FontFactory.GetFont(FontFactory.HELVETICA, 12, Font.BOLD))))
        cell.BorderWidth = 0
        cell.Colspan = _colSpan
        Return cell
    End Function

    Private Function createDetailTableHeader(ByVal _text As String, ByVal alignment As Integer) As PdfPCell
        Dim cell As New PdfPCell(New Phrase(New Chunk(_text, FontFactory.GetFont(FontFactory.HELVETICA, 11))))
        cell.BorderWidth = 0.5
        cell.BackgroundColor = Color.LIGHT_GRAY
        cell.HorizontalAlignment = alignment
        Return cell
    End Function

    Private Function createDetailTableDetail(ByVal _text As String, ByVal alignment As Integer) As PdfPCell
        Dim cell As New PdfPCell(New Phrase(New Chunk(_text, FontFactory.GetFont(FontFactory.HELVETICA, 11))))
        cell.BorderWidth = 0.5
        cell.HorizontalAlignment = alignment
        Return cell
    End Function

    Dim mStream As New System.IO.MemoryStream()
    Private Function createPdfDocument(ByVal dt As DataTable) As String


        Dim pdfDoc As New Document(PageSize.A4, 40, 20, 20, 20)
        Dim writer As PdfWriter = PdfWriter.GetInstance(pdfDoc, mStream)
        Try

            pdfDoc.Open()
            Dim logo As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(Server.MapPath("images") + "\\SukiS.png")
            logo.ScalePercent(30)
            pdfDoc.Add(logo)


            If dt.Rows.Count = 0 Then
                Dim noData As New Paragraph("EMPTY DATA", FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD))
                noData.Alignment = 1
                pdfDoc.Add(noData)
            Else
                Dim pTitle As New Paragraph("Good Receipt PO", FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD))
                pTitle.Alignment = 1
                pdfDoc.Add(pTitle)

                pdfDoc.Add(New Paragraph(" "))
                pdfDoc.Add(New Paragraph(" "))
                Dim tblInfo As New PdfPTable(4)
                'tblInfo.WidthPercentage = 100
                Dim widths As Integer() = {70, 100, 100, 100}
                tblInfo.SetWidths(widths)

                tblInfo.AddCell(createPDFPCell("Order No.: ", 1))
                tblInfo.AddCell(createPDFPCell(IIf(IsDBNull(dt.Rows(0)("DocNum")), "", dt.Rows(0)("DocNum").ToString()), 1))
                tblInfo.AddCell(createPDFPCell("Delivery Date: ", 1))
                Dim duedate As DateTime = DateTime.Parse(dt.Rows(0)("DocDueDate"))
                tblInfo.AddCell(createPDFPCell(duedate.ToString("dd/MM/yyyy"), 1))

                tblInfo.AddCell(createPDFPCell("P/O No.: ", 1))
                tblInfo.AddCell(createPDFPCell(IIf(IsDBNull(dt.Rows(0)("NumAtCard")), "", dt.Rows(0)("NumAtCard")), 1))
                tblInfo.AddCell(createPDFPCell("A/C No.: ", 1))
                tblInfo.AddCell(createPDFPCell(IIf(IsDBNull(dt.Rows(0)("ACNo")), "", dt.Rows(0)("ACNo")), 1))

                tblInfo.AddCell(createPDFPCell("Address", 1))
                tblInfo.AddCell(createPDFPCell(IIf(IsDBNull(dt.Rows(0)("Address2")), "", dt.Rows(0)("Address2")), 3))

                pdfDoc.Add(tblInfo)
                pdfDoc.Add(New Paragraph(" "))


                Dim detailHdr As New PdfPTable(6)
                Dim hdrWidth As Integer() = {40, 70, 200, 60, 60, 70}
                detailHdr.SetWidths(hdrWidth)
                detailHdr.AddCell(createDetailTableHeader("No", 0))
                detailHdr.AddCell(createDetailTableHeader("Item ID", 0))
                detailHdr.AddCell(createDetailTableHeader("Description", 0))
                detailHdr.AddCell(createDetailTableHeader("Qty", 2))
                detailHdr.AddCell(createDetailTableHeader("Rec. Qty", 2))
                detailHdr.AddCell(createDetailTableHeader("Unit", 1))

                'DETAIL of TABLE
                For Each row As DataRow In dt.Rows
                    detailHdr.AddCell(createDetailTableDetail(IIf(IsDBNull(row("LineNum")), 0, row("LineNum")) + 1, 0))
                    detailHdr.AddCell(createDetailTableDetail(IIf(IsDBNull(row("ItemCode")), "", row("ItemCode")), 0))
                    detailHdr.AddCell(createDetailTableDetail(IIf(IsDBNull(row("Dscription")), "", row("Dscription")), 0))

                    Dim qty As Double = IIf(IsDBNull(row("Quantity")), 0, row("Quantity"))
                    detailHdr.AddCell(createDetailTableDetail(qty, 2))
                    detailHdr.AddCell(createDetailTableDetail(IIf(IsDBNull(row("RecQty")), 0, row("RecQty")) + 0, 2))
                    detailHdr.AddCell(createDetailTableDetail(IIf(IsDBNull(row("unitMsr")), "", row("unitMsr")), 1))
                Next
                pdfDoc.Add(detailHdr)
            End If
            pdfDoc.Close()
            mStream.Flush()
            Return ""
        Catch ex As Exception
            Return ex.Message
        Finally
            mStream.Close()
        End Try
    End Function

    <WebMethod()> _
    Public Sub Acknowledgment(ByVal DocEntry As Integer, ByVal _username As String, ByVal _password As String, ByVal _outlet As String, ByVal DBName As String, ByVal GRPODate As String)
        Dim sFunName As String = String.Empty
        Try
            sFunName = "Acknowledgment()"
            oLog.WriteToLogFile_Debug("Starting Function ", sFuncName)
            WriteLog("Starting Function " & sFunName)

            'Start - This part of code is written for the coverting the date format from dd/mm/yy to mm/dd/yy
            Dim sDateArray As String() = GRPODate.Split("/"c)
            Dim newFormatedDate As String = sDateArray(1) + "/" + sDateArray(0) + "/" + sDateArray(2)
            Dim docDate As DateTime = DateTime.Parse(newFormatedDate)
            'Dim docDate As DateTime = Convert.ToDateTime(newFormatedDate)
            'End - This part of code is written for the coverting the date format from dd/mm/yy to mm/dd/yy

            'Dim docDate As DateTime = DateTime.Parse(GRPODate)

          
            oLog.WriteToLogFile_Debug("GRPO Date  " & docDate, sFuncName)

            Dim strGRPODate As String = docDate.ToString("yyyyMMdd")

            Dim strSQL As String = "select Code  from [@AB_OnlineUser] where Code = @Param1 and U_PWD = @Param2  and CHARINDEX(',' + @Param3 + ',', ',' + U_Outlet + ',') > 0 "
            'Dim strSQL As String = "select Code  from [@AB_OnlineUser] where Code = @Param1 and U_PWD = @Param2  and U_Outlet = @Param3"
            'CHARINDEX(','+'CK-P'+',', ','+U_Outlet+',')>0
            oLog.WriteToLogFile_Debug("Query  " & strSQL, sFuncName)
            WriteLog("Query  " & strSQL & " " & sFunName)
            connect.ConnectDB()
            oLog.WriteToLogFile_Debug("After Connecting DB", sFuncName)
            Dim dtUser As DataTable = connect.ObjectGetAll_Query_DB(strSQL, New Object() {_username, _password, _outlet}).Tables(0)
            oLog.WriteToLogFile_Debug("After Fetching Records", sFuncName)
            If dtUser.Rows.Count > 0 Then
                oLog.WriteToLogFile_Debug("Before Getting Connection by WhsCode", sFuncName)
                Dim arrConn As Array = getConnectionInfoByWhsCode(getCurrWhsCode(DocEntry, DBName))
                oLog.WriteToLogFile_Debug("After Getting Connection by WhsCode", sFuncName)

                Dim strDatabase As String = arrConn(0).ToString()
                Dim strSAPID As String = arrConn(1).ToString()
                Dim strSAPPass As String = arrConn(2).ToString()
                Dim strSQLServer As String = arrConn(3).ToString()
                Dim strSQLUser As String = arrConn(4).ToString()
                Dim strSQLPass As String = arrConn(5).ToString()
                Dim strSAPServer As String = arrConn(6).ToString()
                Dim strServerType As String = arrConn(7).ToString()
                oLog.WriteToLogFile_Debug("Calling Function generateXML() ", sFuncName)
                WriteLog("Calling Function generateXML() " & sFunName)
                Dim requestXML As String = generateXML(DocEntry, _username, DBName, strGRPODate)
                If requestXML.Length = 0 Then
                    Dim strJSON As String = js.Serialize("NO DATA!")
                    Context.Response.Clear()
                    Context.Response.ContentType = "application/json"
                    Context.Response.Flush()
                    Context.Response.Write(strJSON)
                    'Return "NO DATA!"
                    WriteLog("No DATA " & sFunName)
                Else
                    Dim ts As New Transaction()
                    oLog.WriteToLogFile_Debug("Calling Function CreateMarketingDocumentDynamic()", sFuncName)
                    WriteLog("Calling Function CreateMarketingDocumentDynamic()" & sFunName)
                    Dim dsUpdate As DataSet = ts.CreateMarketingDocumentDynamic(requestXML, strDatabase, strSAPID, strSAPPass, _
                                                                            strSQLServer, strSQLUser, strSQLPass, strSAPServer, strServerType, "20", "", False)

                    If CInt(dsUpdate.Tables(0).Rows(0)("ErrCode")) <> 0 Then
                        Dim alert As String = "Error: " & dsUpdate.Tables(0).Rows(0)("ErrMsg").ToString().Replace("'"c, " "c)
                        WriteLog("Mobile START")
                        WriteLog(alert)
                        WriteLog("Mobile END")
                        oLog.WriteToLogFile_Debug(alert, sFuncName)
                        'Return alert
                        Dim strJSON As String = js.Serialize(alert)
                        Context.Response.Clear()
                        Context.Response.ContentType = "application/json"
                        Context.Response.Flush()
                        Context.Response.Write(strJSON)
                    Else
                        Dim pdfFileName As String = DocEntry.ToString() + "-" + "DeliveryOrder-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + ".pdf"

                        Try
                            'WriteLog("PDFFile Name " & pdfFileName & " " & sFunName)
                            'Dim fs As System.IO.FileStream = System.IO.File.Create("C:\\DeliveryOrder\\" + pdfFileName)
                            'Dim content As Byte() = mStream.ToArray()
                            'fs.Write(content, 0, content.Length)
                            'fs.Flush()
                            'fs.Close()
                            oLog.WriteToLogFile_Debug("Calling Function updateDeliveryOrderComplete()", sFuncName)
                            WriteLog("Calling Function updateDeliveryOrderComplete()" & sFunName)
                            Dim intRet As Integer = updateDeliveryOrderComplete(DocEntry, DBName)
                            WriteLog("Return of updateDeliveryOrderComplete  : " + CStr(intRet))
                            oLog.WriteToLogFile_Debug("Return of updateDeliveryOrderComplete  : ", sFuncName)

                            If intRet <> -1 Then
                                WriteLog("Operation complete successful! " & sFunName)
                                'Return "Operation complete successful!"
                                Dim strJSON As String = js.Serialize("Operation complete successful!")
                                Context.Response.Clear()
                                Context.Response.ContentType = "application/json"
                                Context.Response.Flush()
                                Context.Response.Write(strJSON)
                            Else
                                WriteLog("Failed to update DO status. " & sFunName)
                                'Return "Failed to update DO status."
                                Dim strJSON As String = js.Serialize("Failed to update DO status.")
                                Context.Response.Clear()
                                Context.Response.ContentType = "application/json"
                                Context.Response.Flush()
                                Context.Response.Write(strJSON)
                            End If

                        Catch ex As Exception
                            'Return "Error in create pdf file: " + ex.Message
                            Dim strJSON As String = js.Serialize("Error in create pdf file: " + ex.Message)
                            Context.Response.Clear()
                            Context.Response.ContentType = "application/json"
                            Context.Response.Flush()
                            Context.Response.Write(strJSON)
                        End Try

                    End If
                End If

            Else
                'Return "Wrong username password"
                Dim strJSON As String = js.Serialize("Wrong username password")
                Context.Response.Clear()
                Context.Response.ContentType = "application/json"
                Context.Response.Flush()
                Context.Response.Write(strJSON)
            End If
            WriteLog("Completed With SUCCESS " & sFunName)
        Catch ex As Exception
            oLog.WriteToLogFile_Debug(ex.Message, sFuncName)
            WriteLog("Mobile START")
            WriteLog(ex.StackTrace & ":" & ex.Message)
            WriteLog("Mobile END")
            WriteLog("Completed With ERROR " & sFunName)
            Dim strJSON As String = js.Serialize(ex.Message)
            Context.Response.Clear()
            Context.Response.ContentType = "application/json"
            Context.Response.Flush()
            Context.Response.Write(strJSON)
        End Try
    End Sub

    Private Function getCurrWhsCode(ByVal DocEntry As Integer, ByVal DBName As String) As String
        Dim sisConnect As Connection = getConnectionByDBName(DBName)

        Dim dt As New DataTable
        'od.LicTradNum
        Dim sqlStr As String = "select od.DocNum, od.DocDueDate, od.NumAtCard, od.CardCode, od.U_AB_DriverNo, od.CardName, "
        sqlStr += " od.DocDate, od.DocEntry, od.OwnerCode, od.NumAtCard, od.SlpCode, od.DocStatus, "
        sqlStr += " od.[Address], od.Footer, od.Address2, '' ACNo, od.Address2, od.U_AB_DeliveryStatus, od.U_AB_POWhsCode, "
        sqlStr += " dl.ItemCode, dl.LineNum, dl.Dscription, dl.Quantity, deli.RecQty, dl.unitMsr, dl.LineTotal, "
        sqlStr += " dl.Price, dl.WhsCode, PriceBefDi, od.NumAtCard BaseEntry, dl.U_AB_POLineNum BaseLine "
        sqlStr += " from ODLN od join DLN1 dl on od.DocEntry = dl.DocEntry "
        sqlStr += " left join AB_DeliveryOrder deli on od.DocEntry = deli.DocEntry and dl.LineNum = deli.LineNum "
        sqlStr += " where od.DocEntry = @Param1"
        oLog.WriteToLogFile_Debug("Query : " & sqlStr, sFuncName)
        dt = sisConnect.ObjectGetAll_Query_DB(sqlStr, New Object() {DocEntry}).Tables(0)
        oLog.WriteToLogFile_Debug("Success for DocEntry : " & DocEntry, sFuncName)
        Return IIf(IsDBNull(dt.Rows(0)("U_AB_POWhsCode")), "", dt.Rows(0)("U_AB_POWhsCode"))

    End Function

    Private Function updateDeliveryOrderComplete(ByVal DocEntry As Integer, ByVal DBName As String) As Integer
        Dim sqlStr As String = String.Empty
        Dim sFunName As String = String.Empty

        Try
            sFunName = "updateDeliveryOrderComplete()"

            WriteLog("Starting Function " & sFunName)
            'update Delivery Order Status
            Dim sisConn As New Connection()
            sisConn = getConnectionByDBName(DBName)
            sqlStr = "Update ODLN set U_AB_DeliveryStatus = 1 where DocEntry = @Param1"
            WriteLog("Mobile START")
            WriteLog(sqlStr & ":" & DocEntry & ":" & DBName)
            WriteLog("Mobile END")
            WriteLog("Completed with SUCCESS  " & sFunName)
            Return sisConn.ObjectUpateByQuery(sqlStr, New Object() {DocEntry})
        Catch ex As Exception
            WriteLog("Mobile START")
            WriteLog(ex.StackTrace & ":" & ex.Message)
            WriteLog(sqlStr & ":" & DocEntry & ":" & DBName)
            WriteLog("Mobile END")
            WriteLog("Completed with ERROR " & sFunName)
        End Try
    End Function

    <WebMethod()> _
 <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Sub PostOrderDetail(ByVal jsonText As String, ByVal DBName As String)
        sFuncName = "PostOrderDetail()"
        Dim i As Integer = 1
        oLog.WriteToLogFile_Debug("Starting Function ", sFuncName)
        Dim jss As New JavaScriptSerializer()
        Dim list As List(Of DLN1UP) = jss.Deserialize(Of List(Of DLN1UP))(jsonText)

        oLog.WriteToLogFile_Debug("starting Update loop ", sFuncName)

        For Each oDet As DLN1UP In list
            oLog.WriteToLogFile_Debug("Starting loop Count " & i, sFuncName)
            Dim sisConnect As Connection = getConnectionByDBName(DBName)
            oLog.WriteToLogFile_Debug("Input " & oDet.RecQty & oDet.DocEntry & oDet.LineNum & DBName, sFuncName)
            Dim sqlStr As String = "update AB_DeliveryOrder set RecQty = @Param1 where DocEntry = @Param2 and LineNum = @Param3"
            sisConnect.ObjectUpateByQuery(sqlStr, New Object() {oDet.RecQty, oDet.DocEntry, oDet.LineNum})
            oLog.WriteToLogFile_Debug("Ending loop Count " & i, sFuncName)
            i = i + 1
        Next
        oLog.WriteToLogFile_Debug("ending update loop ", sFuncName)
        Dim strJSON As String = js.Serialize("True")
        Context.Response.Clear()
        Context.Response.ContentType = "application/json"
        Context.Response.Flush()
        Context.Response.Write(strJSON)
        oLog.WriteToLogFile_Debug("Ending Function ", sFuncName)
    End Sub

    Dim currWhsCode As String

    <WebMethod()> _
    Public Function generateXML(ByVal DocEntry As Integer, ByVal UserAck As String, ByVal DBName As String, ByVal strGRPODate As String) As String
        Dim sFunName As String = String.Empty
        Try
            Dim sisConnect As Connection = getConnectionByDBName(DBName)

            sFunName = "generateXML"
            Dim dt As New DataTable
            'od.LicTradNum
            WriteLog("Starting Function " & sFunName)
            Dim sqlStr As String = "select isnull(m1.BeginStr, '') + CAST(od.DocNum as nvarchar(50)) DocNum, od.DocDueDate, od.NumAtCard, od.CardCode, od.U_AB_DriverNo, od.CardName, "
            sqlStr += " od.DocDate, od.DocEntry, od.OwnerCode, od.SlpCode, od.DocStatus, "
            sqlStr += " od.[Address], od.Footer, od.Address2, '' ACNo, od.Address2, od.U_AB_DeliveryStatus, od.U_AB_POWhsCode, "
            sqlStr += " dl.ItemCode, dl.LineNum, dl.Dscription, dl.Quantity, isnull(deli.RecQty, 0) RecQty, dl.unitMsr, dl.LineTotal, "
            sqlStr += " dl.Price, dl.WhsCode, PriceBefDi, dl.U_AB_PODocEntry BaseEntry, dl.U_AB_POLineNum BaseLine, od.LicTradNum VendorCode, "
            sqlStr += " dl.U_AB_POQty, isnull(dl.U_AB_RDOQty, 0) U_AB_RDOQty, isnull(itm.NumInBuy, 0) NumInBuy "
            sqlStr += " from ODLN od join DLN1 dl on od.DocEntry = dl.DocEntry "
            sqlStr += " left join AB_DeliveryOrder deli on od.DocEntry = deli.DocEntry and dl.LineNum = deli.LineNum "
            sqlStr += " left join NNM1 m1 on od.Series = m1.Series "
            sqlStr += " left join OITM itm on itm.ItemCode = dl.ItemCode "
            sqlStr += " where od.DocEntry = @Param1  and isnull(deli.RecQty, 0) <> 0"

            WriteLog("Query " & sqlStr & " " & sFunName)

            dt = sisConnect.ObjectGetAll_Query_DB(sqlStr, New Object() {DocEntry}).Tables(0)


            Dim ds As New DataSet()
            Dim tblHeader As New DataTable("OPDN")
            tblHeader.Columns.Add("CardCode")
            tblHeader.Columns.Add("CardName")
            tblHeader.Columns.Add("DocDate")
            tblHeader.Columns.Add("DocDueDate")
            tblHeader.Columns.Add("Comments")
            'tblHeader.Columns.Add("OwnerCode")
            tblHeader.Columns.Add("NumAtCard")
            'tblHeader.Columns.Add("SlpCode")
            'tblHeader.Columns.Add("DocStatus")
            tblHeader.Columns.Add("Address")
            ' tblHeader.Columns.Add("DocEntry")
            tblHeader.Columns.Add("U_AB_POWhsCode")
            tblHeader.Columns.Add("U_AB_DriverNo")
            tblHeader.Columns.Add("U_AB_UserCode")

            'Dim ackDate As Date = Date.Now

            WriteLog("Data table OPDN - Header column creation " & sFunName)

            currWhsCode = IIf(IsDBNull(dt.Rows(0)("U_AB_POWhsCode")), "", dt.Rows(0)("U_AB_POWhsCode"))
            Dim row As DataRow = tblHeader.NewRow()
            row("CardCode") = dt.Rows(0)("VendorCode")
            row("CardName") = "" 'dt.Rows(0)("CardName")
            row("DocDate") = strGRPODate 'ackDate.ToString("yyyyMMdd")
            'Convert.ToDateTime(dt.Rows(0)("DocDate")).ToString("yyyyMMdd")
            row("DocDueDate") = Convert.ToDateTime(dt.Rows(0)("DocDueDate")).ToString("yyyyMMdd")
            'row("DocEntry") = dt.Rows(0)("DocEntry")
            'row("OwnerCode") = dt.Rows(0)("OwnerCode")
            row("NumAtCard") = dt.Rows(0)("DocNum").ToString()
            'row("SlpCode") = dt.Rows(0)("SlpCode")
            'row("DocStatus") = dt.Rows(0)("DocStatus")
            row("Address") = dt.Rows(0)("Address")
            row("U_AB_POWhsCode") = dt.Rows(0)("U_AB_POWhsCode")
            row("U_AB_DriverNo") = dt.Rows(0)("U_AB_DriverNo")
            row("U_AB_UserCode") = UserAck

            row("Comments") = "Posted by MOBILE" '"Based On Purchase Orders " & Convert.ToString(Me.txtPONo.Text) & "."

            tblHeader.Rows.Add(row)

            WriteLog("Data table OPDN - Datas Assigned " & sFunName)

            Dim tblItem As New DataTable("PDN1")
            tblItem.Columns.Add("BaseEntry")
            tblItem.Columns.Add("LineNum")
            tblItem.Columns.Add("BaseType")
            tblItem.Columns.Add("BaseLine")
            tblItem.Columns.Add("ItemCode")
            tblItem.Columns.Add("Dscription")
            tblItem.Columns.Add("Quantity")
            tblItem.Columns.Add("LineTotal")
            tblItem.Columns.Add("PriceBefDi")
            tblItem.Columns.Add("WhsCode")
            tblItem.Columns.Add("U_AB_POQty")
            tblItem.Columns.Add("U_AB_RDOQty")

            tblItem.Columns.Add("U_AE_RefDocEntry")
            tblItem.Columns.Add("U_AE_RefBaseLine")

            WriteLog("Data table PDN1 -  Header column creation " & sFunName)


            'Dim tb As DataTable = DirectCast(ViewState("POTable"), DataTable)
            'Dim tb As New DataTable("POTable")
            For Each r As DataRow In dt.Rows

                Dim rowNew As DataRow = tblItem.NewRow()
                rowNew("ItemCode") = r("ItemCode")
                rowNew("Dscription") = r("Dscription")
                rowNew("BaseEntry") = r("BaseEntry")

                rowNew("LineNum") = r("LineNum")

                rowNew("BaseType") = 22
                rowNew("BaseLine") = r("BaseLine")
                'rowNew["BuyUnitMsr"] = r["BuyUnitMsr"];
                'rowNew["BalanceQty"] = r["BalanceQty"]; ;
                rowNew("LineTotal") = r("LineTotal")
                rowNew("Quantity") = r("RecQty")
                'rowNew["ReceivedQty"] = r["ReceivedQty"]; ;
                rowNew("PriceBefDi") = r("PriceBefDi")
                rowNew("WhsCode") = r("U_AB_POWhsCode")

                rowNew("U_AB_POQty") = r("U_AB_POQty")
                rowNew("U_AB_RDOQty") = r("Quantity")

                rowNew("U_AE_RefDocEntry") = r("DocEntry")
                rowNew("U_AE_RefBaseLine") = r("LineNum")

                tblItem.Rows.Add(rowNew)
            Next

            ds.Tables.Add(tblHeader)
            ds.Tables.Add(tblItem)

            WriteLog("Data table PDN1 - Datas  " & sFunName)

            'Batch control
            'Dim sqlBatch As String = "Select T0.DocEntry, T0.LineNum, T0.ItemCode, T5.DistNumber, T4.Quantity*it.NumInBuy Quantity From DLN1 T0 WITH(NOLOCK)  "
            'sqlBatch += "  join OITL T3 with (nolock) on (T3.DocEntry = T0.DocEntry and T3.DocLine = T0.LineNum and T3.DocType = 15 and T3.DefinedQty <> 0) "
            'sqlBatch += "  join ITL1 T4 with (nolock) on T3.LogEntry = T4.LogEntry "
            'sqlBatch += "  join OBTN T5 with (nolock) on T5.AbsEntry = T4.MdAbsEntry and T5.ItemCode = T4.ItemCode "
            'sqlBatch += " join OITM it on it.ItemCode = T0.ItemCode "
            'sqlBatch += " Where T0.DocEntry = @Param1"

            Dim sqlBatch As String = "Select T0.DocEntry, T0.LineNum, T0.ItemCode, T2.DistNumber , IsNull(T1.Quantity,T0.Quantity)*T3.NumInBuy Quantity "
            sqlBatch += " From DLN1 T0"
            sqlBatch += " left join IBT1_LINK T1 on T1.ItemCode = T0.ItemCode and T1.BaseEntry = T0.DocEntry and T1.BaseType = 15 and T1.BaseLinNum = T0.LineNum "
            sqlBatch += " left join OBTN T2 on T2.ItemCode = T1.ItemCode and T2.DistNumber = T1.BatchNum "
            sqlBatch += " left join OITM T3 on T3.ItemCode = T0.ItemCode "
            sqlBatch += "  where(T0.DocEntry = @Param1) "

            WriteLog("SQL Batch  " & sqlBatch & " " & sFunName)

            sisConnect = getConnectionByDBName(DBName)
            Dim dtDOBatch As DataTable = sisConnect.ObjectGetAll_Query_DB(sqlBatch, New Object() {DocEntry}).Tables(0)
            If dtDOBatch.Rows.Count > 0 Then
                Dim tbBatch As New DataTable("BTNT")
                tbBatch.Columns.Add("DocLineNum")
                tbBatch.Columns.Add("DistNumber")
                tbBatch.Columns.Add("Quantity")

                For Each btn As DataRow In dtDOBatch.Rows
                    Dim bRow As DataRow = tbBatch.NewRow()
                    bRow("DocLineNum") = btn("LineNum")
                    bRow("DistNumber") = btn("DistNumber")
                    Dim qty = Math.Abs(Double.Parse(btn("Quantity")))
                    bRow("Quantity") = qty 'btn("Quantity")
                    tbBatch.Rows.Add(bRow)
                Next
                If dtDOBatch.Rows(0)("DistNumber").ToString() <> "" Then
                    ds.Tables.Add(tbBatch)
                End If

            End If


            'BIN ALLOCATION

            Dim whsBinCode As String = dt.Rows(0)("U_AB_POWhsCode")
            sisConnect = getConnectionByOutletCode(whsBinCode)

            Dim sqlBin As String = "select top(1) AbsEntry from OBIN where WhsCode='" + whsBinCode + "' and SysBin='N'"
            Dim dtBin As DataTable = sisConnect.ObjectGetAll_Query_DB(sqlBin).Tables(0)
            If dtBin.Rows.Count > 0 Then
                Dim binVal As String = dtBin.Rows(0)("AbsEntry").ToString()

                Dim tbBin As New DataTable("PDN19")
                tbBin.Columns.Add("DocEntry")
                tbBin.Columns.Add("BinAbs")
                tbBin.Columns.Add("Quantity")
                tbBin.Columns.Add("BinActTyp")
                tbBin.Columns.Add("SnBMDAbs")
                tbBin.Columns.Add("LineNum")

                For Each r As DataRow In dt.Rows
                    Dim rowNew As DataRow = tbBin.NewRow()
                    rowNew("DocEntry") = dt.Rows(0)("DocEntry")
                    rowNew("BinAbs") = binVal
                    rowNew("Quantity") = r("RecQty") * r("NumInBuy")
                    rowNew("BinActTyp") = 1
                    rowNew("SnBMDAbs") = 0
                    rowNew("LineNum") = r("LineNum")
                    tbBin.Rows.Add(rowNew)
                Next

                ds.Tables.Add(tbBin)

            End If

            WriteLog("Bin Location Completed   " & sFunName)
            '--------------------------------------------

            Dim objInfo As New DocumentXML()
            Dim requestXML As String = objInfo.ToXMLStringFromDS("20", ds)

            'WriteLog("Calling Function  createPdfDocument()  " & sFunName)
            'createPdfDocument(dt)

            WriteLog("Completed With SUCCESS  " & sFunName)
            Return requestXML
        Catch ex As Exception
            WriteLog("Completed With SUCCESS  " & ex.Message.ToString() & "  " & sFunName)
        End Try
    End Function

    Private Function getConnectionByDBName(ByVal DBName As String) As Connection
        connect.ConnectDB()

        Dim sqlStr As String = "select U_ConnString from [@AB_CompanySetup] where U_DBName = '" + DBName + "'"
        Dim dt As New DataTable

        dt = connect.ObjectGetAll_Query_DB(sqlStr).Tables(0)
        If dt.Rows.Count > 0 Then
            Dim connStr As String = dt.Rows(0)("U_ConnString").ToString()
            Dim arrConn As Array
            arrConn = connStr.Split(";")
            Dim conn As New Connection()
            conn.ConnectDynamicDB(arrConn(0).ToString(), arrConn(3).ToString(), arrConn(4).ToString(), arrConn(5).ToString())
            Return conn
        Else
            WriteLog("getConnectionByDBName return nothing ")
            Return Nothing
        End If
    End Function

    Private Function getConnectionByOutletCode(ByVal OutletCode As String) As Connection
        connect.ConnectDB()

        Dim sqlStr As String = "select U_ConnString from [@AB_COMPANYSETUP] cs join [@AB_OUTLET] ol on cs.U_CompanyCode = ol.U_CompanyCode "
        sqlStr += " where ol.U_OutletID = '" + OutletCode + "'"
        Dim dt As New DataTable

        dt = connect.ObjectGetAll_Query_DB(sqlStr).Tables(0)
        If dt.Rows.Count > 0 Then
            Dim connStr As String = dt.Rows(0)("U_ConnString").ToString()
            Dim arrConn As Array
            arrConn = connStr.Split(";")
            Dim conn As New Connection()
            conn.ConnectDynamicDB(arrConn(0).ToString(), arrConn(3).ToString(), arrConn(4).ToString(), arrConn(5).ToString())
            Return conn
        Else
            Return Nothing
        End If
    End Function

    Private Function getConnectionInfoByWhsCode(ByVal whsCode As String) As Array
        connect.ConnectDB()

        Dim sqlStr As String = "select U_ConnString from [@AB_COMPANYSETUP] cs join [@AB_OUTLET] ol "
        sqlStr += " on cs.Code = ol.U_CompanyCode where ol.U_WhseCode = '" + whsCode + "'"
        Dim dt As New DataTable
        oLog.WriteToLogFile_Debug("Query for ConnectionInfo WhsCode" & sqlStr, sFuncName)
        dt = connect.ObjectGetAll_Query_DB(sqlStr).Tables(0)
        If dt.Rows.Count > 0 Then
            Dim connStr As String = dt.Rows(0)("U_ConnString").ToString()
            Dim arrConn As Array
            arrConn = connStr.Split(";")
            Return arrConn
        Else
            Return Nothing
        End If
    End Function

    Class BATCHGET
        Property DocEntry As String
        Property ItemCode As String
        Property DistNumber As String
        Property Quantity As Double
    End Class

    Class BTNT
        Property DocLineNum As String
        Property DistNumber As String
        Property Quantity As String
        Public Sub New()

        End Sub
    End Class

    Class DLN1UP
        Property DocEntry As Integer
        Property LineNum As Integer
        Property RecQty As Double
        Public Sub New()

        End Sub
    End Class

    Class OUSR
        Public USER_CODE As String
        Public PASSWORD As String
        Public Fullname As String
        Public Sub New()

        End Sub
        Public Sub New(ByVal _username As String, ByVal _password As String, ByVal _fullname As String)
            USER_CODE = _username
            PASSWORD = _password
            Fullname = _fullname
        End Sub
    End Class
    'do.DocEntry, do.DocNum, do.CardCode, do.Address2, do.DocDueDate, do.DocNum, fd.Descr, do.U_Driver, do.NumAtCard 
    Class ODLN
        Property RowNum As Integer
        Property DocEntry As Integer
        Property DocNum As String
        Property CardCode As String
        Property Address2 As String
        Property DocDueDate As String
        Property DocDate As String

        Property NumAtCard As String

        Property Descr As String

        Property ACNo As String
        Property U_AB_DriverNo As String
        Property U_DBName As String

        Property OutletCode As String
        Property OutletName As String

        Public Sub New()

        End Sub

    End Class
    'dt.DocEntry, dt.LineNum, dt.ItemCode, dt.Dscription, dt.Quantity, dt.U_RecQty 
    Class DLN1
        Property LineNum As Integer
        Property DocEntry As Integer
        Property ItemCode As String
        Property Dscription As String
        Property Quantity As Double
        Property U_RecQty As Double
        Property UnitMsr As String
        Property WhsCode As String
        Property U_AB_POQty As Double
        Public Sub New()

        End Sub
    End Class

    Class OpenDO
        Property U_OutletID As String
        Property Code As String
        Property Name As String
        Property OpenDOQty As Integer
        Property Address As String
        Property U_DBName As String
        Public Sub New()

        End Sub
    End Class

    Class DeliveryOrder
        Property DocEntry As Integer
        Property LineNum As Integer
        Property DONo As Integer
        Property SONo As Integer
        Property ItemCode As String
        Property Dscription As String
        Property Quantity As Double
        Property RecQty As Double
        Property UnitMsr As String
    End Class
End Class