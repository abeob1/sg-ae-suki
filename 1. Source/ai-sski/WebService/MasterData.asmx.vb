Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports SAPbobsCOM

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://electra-ai.com/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class MasterData
    Inherits System.Web.Services.WebService

#Region "SUKI"
    '<WebMethod()> _
    'Public Function TEST() As String
    '    Try
    '        Dim connect As New Connection()
    '        Dim oCompany As SAPbobsCOM.Company
    '        Dim errMsg As String = connect.setHoldingDB(oCompany)
    '        Dim abc As String = "CLING FILM 'LACY'S' PVC (313)(450MX300M)(6ROLL/CTN)"
    '        If errMsg.Length = 0 Then
    '            Dim params = New Object() {abc}
    '            Dim query As String = "Update [Item_Supplier] set ItemName = @Param1"
    '            query += " where [Item_Supplier].ItemCode = 'SG10001792'"
    '            Dim count As Integer = connect.Object_Execute_SAP(query, params)
    '        End If
    '    Catch ex As Exception
    '        WriteLog(ex.StackTrace & ":" & ex.Message)
    '    End Try
    '    Return 0
    'End Function
    <WebMethod()> _
    Public Function CreatePO(ByVal userCode As String, ByVal pwd As String, ByVal databaseName As String, ByVal isUpdate As String, ByVal isPO As String, _
                             ByVal dsData As DataSet, ByVal isDraft As Boolean, ByVal Key As String) As DataSet
        Dim connect As New Connection()
        Dim oCompanyPO As SAPbobsCOM.Company = New SAPbobsCOM.Company
        Dim dsDataSO = dsData.Copy()
        Dim lErrCode As Integer
        Dim objType As String = String.Empty
        Dim sErrMsg As String = String.Empty
        Dim closeDraft As Boolean = False
        Dim b As New SAP_Functions
        Dim PODocEntry As String = String.Empty
        Try
            If isDraft Then
                dsData.Tables("ODRF").Columns.Remove("OutletCode")
                dsData.Tables("DRF1").Columns.Remove("ShipType")
                dsData.Tables("DRF1").Columns.Remove("TrnspName")
                If dsData.Tables("DRF1").Columns.Contains("LineTotal") Then
                    dsData.Tables("DRF1").Columns.Remove("LineTotal")
                End If
            Else
                dsData.Tables("OPOR").Columns.Remove("OutletCode")
                dsData.Tables("POR1").Columns.Remove("ShipType")
                dsData.Tables("POR1").Columns.Remove("TrnspName")
                If dsData.Tables("POR1").Columns.Contains("LineTotal") Then
                    dsData.Tables("POR1").Columns.Remove("LineTotal")
                End If
            End If

            Dim transConnectOutlet As New TransConnection()
            Dim transConnectSister As New TransConnection()

            If isDraft Then
                objType = "112"
            Else
                objType = "22"
            End If

            Dim oDocment As SAPbobsCOM.Documents

            'Parse XML 
            Dim xmlDoc As DocumentXML = New DocumentXML()
            Dim xmlRequest = xmlDoc.ToXMLStringFromDS(objType, dsData)
            If xmlRequest.Length = 0 Then
                Return b.ReturnMessage(-1, "XmlRequest is incorrect format.")
            End If

            Dim constr As String
            Dim dt As New DataSet("Outlet")
            constr = connect.setHoldingDB(oCompanyPO)
            Dim conStrCompany As String = String.Empty
            If constr.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_CompanySetup] WHERE U_DBName = @Param1", New Object() {databaseName})
                If dt IsNot Nothing And dt.Tables.Count > 0 Then
                    If dt.Tables(0).Rows.Count > 0 Then
                        If dt.Tables(0).Rows(0)("U_ConnString") IsNot Nothing Then
                            conStrCompany = dt.Tables(0).Rows(0)("U_ConnString")
                            constr = transConnectOutlet.connectDB(dt.Tables(0).Rows(0)("U_ConnString"), oCompanyPO)
                        Else
                            Return b.ReturnMessage(lErrCode, "Can't connect database.")
                        End If
                    Else
                        Return b.ReturnMessage(lErrCode, "Can't connect database.")
                    End If
                Else
                    Return b.ReturnMessage(lErrCode, "Can't connect database.")
                End If
            End If
            If constr <> "" Then
                Return b.ReturnMessage(-1, constr)
            End If
            If oCompanyPO.Connected Then
                oCompanyPO.XMLAsString = True
                oDocment = oCompanyPO.GetBusinessObjectFromXML(xmlRequest, 0)
                If (isUpdate = "0" And isPO = "1") Or (isUpdate = "1" And isPO = "0") Then
                    'Create PO
                    lErrCode = oDocment.Add()
                    If lErrCode <> 0 Then
                        oCompanyPO.GetLastError(lErrCode, sErrMsg)
                        UpdateXMLLog(userCode, pwd, databaseName, objType, xmlRequest, sErrMsg, DateTime.Now, "0", "")
                        Return b.ReturnMessage(lErrCode, sErrMsg)
                    Else
                        Dim docEntry As String = oCompanyPO.GetNewObjectKey()
                        'Copy Draft PO to actual PO
                        If (isUpdate = "1" And isPO = "0") Then
                            Dim oDraft As SAPbobsCOM.Documents
                            oDraft = oCompanyPO.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)
                            Dim errcode As Integer
                            If oDraft.GetByKey(Key) Then
                                errcode = oDraft.Cancel()
                                If errcode <> 0 Then
                                    UpdateXMLLog(userCode, pwd, databaseName, objType, xmlRequest, sErrMsg, DateTime.Now, "0", "")
                                    Return b.ReturnMessage(lErrCode, sErrMsg)
                                End If
                                UpdateXMLLog(userCode, pwd, databaseName, objType, xmlRequest, "Operation successful!-" & docEntry, DateTime.Now, "0", "")
                                Return b.ReturnMessage(lErrCode, docEntry)
                            End If
                        Else
                            UpdateXMLLog(userCode, pwd, databaseName, objType, xmlRequest, "Operation successful!-" & docEntry, DateTime.Now, "0", "")
                            Return b.ReturnMessage(0, docEntry)
                        End If
                    End If
                ElseIf (isUpdate = "1" And isPO = "1") Then
                    ' Update(PO)

                    If oDocment.GetByKey(Key) Then
                        oDocment.Browser.ReadXml(xmlRequest, 0)
                        lErrCode = oDocment.Update()
                        If lErrCode <> 0 Then
                            oCompanyPO.GetLastError(lErrCode, sErrMsg)
                            UpdateXMLLog(userCode, pwd, databaseName, objType, xmlRequest, sErrMsg, DateTime.Now, isUpdate, Key)
                            Return b.ReturnMessage(lErrCode, sErrMsg)
                        End If
                        UpdateXMLLog(userCode, pwd, databaseName, objType, xmlRequest, "Operation successful!", DateTime.Now, isUpdate, Key)
                        Return b.ReturnMessage(lErrCode, "Operation successful!")
                    Else
                        UpdateXMLLog(userCode, pwd, databaseName, objType, xmlRequest, "Record not found!", DateTime.Now, isUpdate, Key)
                        Return b.ReturnMessage(-1, "Record not found!")
                    End If
                End If
            End If

        Catch ex As Exception
            WriteLog(ex.StackTrace & " " & ex.Message)
            Return b.ReturnMessage(-1, ex.Message)
        Finally
            If oCompanyPO.Connected Then
                oCompanyPO.Disconnect()
            End If
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function UpdateSO(ByVal userId As String, ByVal passWord As String, _
                                         ByVal data As DataSet, ByVal databaseName As String, ByVal isInsert As Boolean) As String
        Dim connect As New Connection()
        Try
            For Each row As DataRow In data.Tables("ORDR").Rows
                Dim dsCheck As DataSet = GetSOData(databaseName, row("NumAtCard"), row("U_AB_DriverNo"))
                Dim errMsg As String = connect.setSQLDB(databaseName)
                If errMsg.Length = 0 Then
                    Dim params = New Object() {row("CardCode"), row("DocDate"), row("DocDueDate"), row("U_AB_Urgent"), row("U_AB_UserCode"), row("U_AB_POWhsCode"), row("LicTradNum"), row("ShipToCode"), row("U_AB_DriverNo"), row("NumAtCard"), row("SOStatus"), row("ErrMessage"), row("U_AB_PODocEntry"), row("U_AB_PORemarks")}
                    Dim query As String = String.Empty
                    If isInsert = True Then
                        If dsCheck IsNot Nothing And
                            dsCheck.Tables.Count > 0 And
                            dsCheck.Tables(0).Rows.Count = 0 And
                            row("ID").ToString() = "0" Then
                            'Insert Header
                            query = "INSERT INTO AB_SO_Header(CardCode,DocDate,DocDueDate,U_AB_Urgent,U_AB_UserCode,U_AB_POWhsCode,LicTradNum,ShipToCode,U_AB_DriverNo,NumAtCard,SOStatus,ErrMessage,U_AB_PODocEntry,CreateDate,U_AB_PORemarks)"
                            query += " VALUES(@Param1,@Param2,@Param3,@Param4,@Param5,@Param6,@Param7,@Param8,@Param9,@Param10,@Param11,@Param12,@Param13,GETDATE(),@Param14)"
                            Dim count As Integer = connect.Object_Execute_SAP(query, params)
                            'Get Max Id
                            Dim ds As DataSet = connect.ObjectGetAll_Query_SAP("SELECT IDENT_CURRENT('AB_SO_Header')")
                            'Insert Detail
                            If ds IsNot Nothing And ds.Tables.Count > 0 Then
                                Dim tbDetail As DataTable = data.Tables("RDR1")
                                Dim masterId As Integer = Integer.Parse(ds.Tables(0).Rows(0)(0).ToString())
                                If tbDetail IsNot Nothing Then
                                    If tbDetail.Rows.Count > 0 Then
                                        For Each rowDetail As DataRow In tbDetail.Rows
                                            'Get Max ID
                                            query = "INSERT INTO AB_SO_Detail(HeaderID,ItemCode,Dscription,Quantity,Price,LineTotal,U_AB_POQty,U_AB_POLineNum)"
                                            query += " VALUES(@Param1,@Param2,@Param3,@Param4,@Param5,@Param6,@Param7,@Param8)"
                                            params = New Object() {masterId, rowDetail("ItemCode"), rowDetail("Dscription"), rowDetail("Quantity"), rowDetail("Price"), rowDetail("LineTotal"), rowDetail("U_AB_POQty"), rowDetail("U_AB_POLineNum")}
                                            count = connect.Object_Execute_SAP(query, params)
                                            If count = 0 Then
                                                'Delete Header if have any error
                                                query = "DELETE FROM AB_SO_Header "
                                                query += " WHERE ID=" + masterId
                                                connect.Object_Execute_SAP(query)
                                                Return "Can not update SO Detail."
                                            End If
                                        Next
                                    Else
                                        'Delete Header if have any error
                                        query = "DELETE FROM AB_SO_Header "
                                        query += " WHERE ID=" + masterId
                                        connect.Object_Execute_SAP(query)
                                        Return "Can not update SO Detail."
                                    End If
                                End If
                            End If
                        End If
                    Else
                        If row("ID").ToString() <> "0" Then
                            'Update Master
                            query = "UPDATE AB_SO_Header SET SOStatus = @Param1,ErrMessage = @Param2,NumAtCard = @Param3"
                            query += " WHERE ID = @Param4"
                            connect.Object_Execute_SAP(query, New Object() {row("SOStatus"), row("ErrMessage"), row("NumAtCard"), row("ID")})
                        End If
                    End If
                Else
                    Return errMsg
                End If
            Next
        Catch ex As Exception
            WriteLog(ex.StackTrace & " " & ex.Message)
            Return ex.Message
        End Try
        Return String.Empty
    End Function
#Region "Backup"
#End Region
    <WebMethod()> _
    Public Function GetSOData(ByVal databaseName As String, ByVal poDocEntry As String) As DataSet
        Dim ds As DataSet = New DataSet
        Dim connect As New Connection()
        Try
            Dim dsHeader As New DataSet("AB_SO_Header")
            Dim dsDetail As DataSet = Nothing
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                dsHeader = connect.ObjectGetAll_Query_SAP("SELECT * FROM AB_SO_Header WHERE NumAtCard = @Param1", New Object() {poDocEntry})
                dsHeader.Tables(0).TableName = "AB_SO_Header"
                ds.Tables.Add(dsHeader.Tables(0).Copy())
                If dsHeader IsNot Nothing And dsHeader.Tables.Count > 0 Then
                    If dsHeader.Tables(0).Rows.Count > 0 Then
                        For Each r As DataRow In dsHeader.Tables(0).Rows
                            Dim dsTemp As DataSet = connect.ObjectGetAll_Query_SAP("SELECT * FROM  AB_SO_Detail WHERE HeaderID = @Param1", New Object() {r("ID")})
                            If dsTemp IsNot Nothing And dsDetail Is Nothing Then
                                dsDetail = dsTemp.Copy()
                            Else
                                If dsTemp.Tables.Count > 0 Then
                                    If dsTemp.Tables(0).Rows.Count > 0 Then
                                        For Each rD As DataRow In dsTemp.Tables(0).Rows
                                            dsDetail.Tables(0).ImportRow(rD)
                                        Next
                                    End If
                                End If
                            End If
                        Next
                        dsDetail.Tables(0).TableName = "AB_SO_Detail"
                        ds.Tables.Add(dsDetail.Tables(0).Copy())
                    End If
                End If
            End If
            Return ds
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Public Function GetSOData(ByVal databaseName As String, ByVal poDocEntry As String, ByVal driverNo As String) As DataSet
        Dim connect As New Connection()
        Try
            Dim dsHeader As New DataSet("AB_SO_Header")
            Dim dsDetail As DataSet = Nothing
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                dsHeader = connect.ObjectGetAll_Query_SAP("SELECT * FROM AB_SO_Header WHERE NumAtCard = @Param1 AND U_AB_DriverNo = @Param2", New Object() {poDocEntry, driverNo})
            End If
            Return dsHeader
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetSODataById(ByVal databaseName As String, ByVal id As String) As DataSet
        Dim ds As DataSet = New DataSet
        Dim connect As New Connection()
        Try
            Dim dsHeader As New DataSet("AB_SO_Header")
            Dim dsDetail As New DataSet("AB_SO_Detail")
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                dsHeader = connect.ObjectGetAll_Query_SAP("SELECT * FROM AB_SO_Header WHERE ID = @Param1", New Object() {id})
                If dsHeader IsNot Nothing Then
                    If dsHeader.Tables.Count > 0 Then
                        dsHeader.Tables(0).TableName = "AB_SO_Header"
                        ds.Tables.Add(dsHeader.Tables(0).Copy())
                        If dsHeader.Tables(0).Rows.Count > 0 Then
                            dsDetail = connect.ObjectGetAll_Query_SAP("SELECT * FROM  AB_SO_Detail WHERE HeaderID = @Param1", New Object() {dsHeader.Tables(0).Rows(0)("ID")})
                            dsDetail.Tables(0).TableName = "AB_SO_Detail"
                            ds.Tables.Add(dsDetail.Tables(0).Copy())
                        End If
                    End If
                End If
            End If
            Return ds
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function SearchSOData(ByVal databaseName As String, ByVal status As String, ByVal poNo As String, _
                                 ByVal fromDate As DateTime, ByVal toDate As DateTime) As DataSet
        Dim connect As New Connection()
        Try
            Dim dsHeader As New DataSet("AB_SO_Header")
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                Dim query As String = "SELECT * FROM AB_SO_Header"
                Dim params As Object() = New Object() {fromDate, toDate}
                query += " WHERE CreateDate BETWEEN @Param1 AND @Param2 "
                If status.ToUpper() <> "ALL" Then
                    query += "  AND SOStatus = @Param3"
                    params = New Object() {fromDate, toDate, status}
                End If
                If poNo.Length > 0 Then
                    query += " AND NumAtCard LIKE '%" + poNo + "%'"
                End If
                query += " ORDER BY CreateDate DESC"
                dsHeader = connect.ObjectGetAll_Query_SAP(query, params)
                If dsHeader IsNot Nothing Then
                    If dsHeader.Tables.Count > 0 Then
                        dsHeader.Tables(0).TableName = "AB_SO_Header"
                    End If
                End If
                Return dsHeader
            End If
        Catch ex As Exception
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function CheckCreatedSOSAP(ByVal docEntry As String, ByVal driverNo As String, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("ORDR")
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM  ORDR WHERE NumAtCard = @Param1 AND U_AB_DriverNo = @Param2", New Object() {docEntry, driverNo})
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetPrefix(ByVal Series As Integer, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("NNM1")
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM NNM1 WHERE Series = @Param1", New Object() {Series})
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetPONo(ByVal docEntry As Integer, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("OPOR")
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT T0.DocStatus,T0.DocNum,T1.BeginStr FROM OPOR T0 JOIN NNM1 T1 ON T0.Series = T1.Series WHERE T0.DocEntry = @Param1", New Object() {docEntry})
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetStockYear(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("OACP")
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT 'YYYY' [Year] UNION ALL SELECT cast(Year as Varchar(10)) FROM OACP")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetAllABCompany() As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Outlet")
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_CompanySetup]")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetAllABOutlet() As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Outlet")
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_Outlet]")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetWareHouseAddress(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("OWHS")
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM OWHS")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetABOutletByCompany(ByVal companyDB As String) As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Outlet")
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_Outlet] WHERE U_DBName= @Param1", New Object() {companyDB})
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetABOutletByData(ByVal companyDB As String) As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Outlet")
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT U_ConnString FROM [@AB_COMPANYSETUP] WHERE U_DBName= @Param1", New Object() {companyDB})
                If (dt IsNot Nothing) Then
                    If dt.Tables.Count > 0 And dt.Tables(0).Rows.Count > 0 Then
                        Dim MyArr As Array
                        MyArr = dt.Tables(0).Rows(0)(0).ToString().Split(";")
                        If MyArr.Length > 0 Then
                            errMsg = connect.setSQLDB(MyArr(2))
                            If errMsg.Length = 0 Then
                                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM OWHS")
                                Return dt
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function GetInternalPO(ByVal vendorCode As String) As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Outlet")
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_CompanySetup] WHERE U_VendorCode = @Param1", New Object() {vendorCode})
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetCustomerCode(ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim dt As New DataSet("Outlet")
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_CompanySetup] WHERE U_DBName = @Param1", New Object() {databaseName})
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetOnlineUser() As DataSet
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim dt As New DataSet("AB_OnlineUser")
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_OnlineUser] ")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetAllOutlet(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Outlet")
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM OWHS")
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetAllCompany() As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Outlet")
            connect.ConnectDB()
            dt = connect.ObjectGetAll_Query_DB("SELECT * FROM SRGC")
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    '<WebMethod()> _
    'Public Function GetLogin(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As DataSet
    '    Try
    '        Dim connect As New Connection()
    '        Dim oCompany As SAPbobsCOM.Company
    '        Dim dt As New DataSet("AB_ONLINEUSER")
    '        Dim errMsg = connect.setHoldingDB(oCompany)
    '        If errMsg.Length = 0 Then
    '            dt = connect.ObjectGetAll_Query_SAP("SELECT  *,T0.U_WhseName FROM [@AB_ONLINEUSER] JOIN [@AB_OUTLET] T0 ON [@AB_ONLINEUSER].U_Outlet = T0.U_OutletID WHERE U_USerType='W' AND [@AB_ONLINEUSER].Code = @Param1 AND U_PWD = @Param2", New Object() {userId, passWord})
    '            If dt IsNot Nothing And dt.Tables.Count > 0 Then
    '                If dt.Tables(0).Rows.Count > 0 Then
    '                    Dim checkDB As DataSet = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_OUTLET] T0 JOIN [@AB_COMPANYSETUP] T1 ON T0.U_CompanyCode = T1.U_CompanyCode WHERE T0.U_OutletID = @Param1 AND T0.U_DBName = @Param2", New Object() {dt.Tables(0).Rows(0)("U_Outlet"), databaseName})
    '                    If checkDB IsNot Nothing And checkDB.Tables.Count > 0 Then
    '                        If checkDB.Tables(0).Rows.Count > 0 Then
    '                            Return dt
    '                        Else
    '                            errMsg = "You do not have permission to access " & databaseName & " database."
    '                        End If
    '                    Else
    '                        errMsg = "System is processing another request. Pls try again later."
    '                    End If
    '                Else
    '                    Return dt
    '                End If
    '            Else
    '                Return dt
    '            End If
    '        End If
    '        dt.Tables.Clear()
    '        Dim dtErr As DataTable = New DataTable()
    '        dtErr.Columns.Add("ErrMsg")
    '        Dim rErr = dtErr.NewRow()
    '        rErr("ErrMsg") = errMsg
    '        dtErr.Rows.Add(rErr)
    '        dt.Tables.Add(dtErr)
    '        Return dt
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    '    Return Nothing
    'End Function
    <WebMethod()> _
    Public Function GetCompanyInfo(ByVal companyCode As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim errMsg = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                Dim query = "SELECT * FROM [@AB_COMPANYSETUP]  WHERE U_CompanyCode = @Param1 "
                Dim checkDB As DataSet = connect.ObjectGetAll_Query_SAP(query, New Object() {companyCode})
                Return checkDB
            End If
        Catch ex As Exception
            Throw ex
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function CheckPermissonDB(ByVal outlet As String, ByVal companyCode As String, ByVal loginDB As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim errMsg = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                If companyCode.Length > 0 Then
                    If outlet IsNot Nothing Then
                        Dim query = "SELECT * FROM [@AB_OUTLET] T0 JOIN [@AB_COMPANYSETUP] T1 ON T0.U_CompanyCode = T1.U_CompanyCode WHERE T0.U_OutletID = @Param1 AND T0.U_CompanyCode = @Param2 AND T0.U_DBName = @Param3"
                        Dim checkDB As DataSet = connect.ObjectGetAll_Query_SAP(query, New Object() {outlet, companyCode, loginDB})
                        Return checkDB
                    Else
                        Dim query = "SELECT * FROM [@AB_COMPANYSETUP]  WHERE U_CompanyCode = @Param1 AND U_DBName = @Param2"
                        Dim checkDB As DataSet = connect.ObjectGetAll_Query_SAP(query, New Object() {companyCode, loginDB})
                        Return checkDB
                    End If
                Else
                    Dim query = "SELECT * FROM [@AB_OUTLET] T0 JOIN [@AB_COMPANYSETUP] T1 ON T0.U_CompanyCode = T1.U_CompanyCode WHERE T0.U_OutletID = @Param1 AND T0.U_DBName = @Param2"
                    Dim checkDB As DataSet = connect.ObjectGetAll_Query_SAP(query, New Object() {outlet, loginDB})
                    Return checkDB
                End If

            End If
        Catch ex As Exception
            Throw ex
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function GetLogin(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim dt As New DataSet("AB_ONLINEUSER")
            Dim errMsg = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_ONLINEUSER] WHERE U_USerType='W' AND [@AB_ONLINEUSER].Code = @Param1 AND U_PWD = @Param2", New Object() {userId, passWord})
            End If
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function GetUserInfo(ByVal databaseName As String, ByVal userCode As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Users")
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM OUSR WHERE User_Code = @Param1", New Object() {userCode})
                Return dt
            End If
        Catch ex As Exception
            Throw ex
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function Login(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As Boolean
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Users")
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetOITM(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("OITM")
            connect.setSQLDB(databaseName)
            dt = connect.ObjectGetAll_Query_SAP("SELECT ItemCode, ItemName FROM OITM WHERE validFor ='Y' AND  PrchseItem ='Y' AND SellItem In ('N','Y') AND  InvntItem In ('N','Y')")
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetOPOR(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("OPOR")
            connect.setSQLDB(databaseName)
            dt = connect.ObjectGetAll_Query_SAP("SELECT docnum, cardcode, cardname FROM OPOR")
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetOCRD(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("OITM")
            connect.setSQLDB(databaseName)
            dt = connect.ObjectGetAll_Query_SAP("SELECT cardcode, cardname FROM OCRD ORDER BY cardcode ASC ")
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetPeronalContact() As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("OCRD")
            Dim oCompany As SAPbobsCOM.Company
            connect.setHoldingDB(oCompany)
            dt = connect.ObjectGetAll_Query_SAP("SELECT CardCode,U_AB_Email AS E_Maill FROM OCRD")
            'dt = connect.ObjectGetAll_Query_SAP("SELECT CardCode,E_Maill FROM OCPR")
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function CheckWhsCode(ByVal databaseName As String _
                             , ByVal whsCode As String) As Boolean
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("PO")
            connect.setSQLDB(databaseName)
            Dim query As String = "SELECT PrcCode FROM OPRC WHERE PrcCode='" + whsCode + "'"
            dt = connect.ObjectGetAll_Query_SAP(query)
            If dt IsNot Nothing Then
                If dt.Tables(0).Rows.Count = 0 Then
                    Return False
                    'connect.setSQLDB(databaseName)
                    'query = "SELECT WhsCode FROM OWHS WHERE WhsCode='" + whsCode + "'"
                    'dt = connect.ObjectGetAll_Query_SAP(query)
                    'If dt IsNot Nothing Then
                    '    If dt.Tables(0).Rows.Count = 0 Then
                    '        Return False
                    '    Else
                    '        Return True
                    '    End If
                    'End If
                Else
                    Return True
                End If
            End If
            Return True
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
        End Try
        Return True
    End Function
    <WebMethod()> _
    Public Function CheckProcessPO(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String _
                             , ByVal docEntry As String, ByVal numAtCard As String, ByVal isInternal As Boolean) As Boolean
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("PO")
            connect.setSQLDB(databaseName)
            Dim query As String = String.Empty
            If isInternal Then
                query = "SELECT NumAtCard "
                query += " FROM AB_SO_Header  "
                query += " WHERE NumAtCard = '" + numAtCard + "'"
                dt = connect.ObjectGetAll_Query_SAP(query)
                If dt IsNot Nothing Then
                    If dt.Tables.Count > 0 Then
                        If dt.Tables(0).Rows.Count = 0 Then
                            Return True
                        Else
                            Return False
                        End If
                    End If
                End If
            Else
                connect.setSQLDB(databaseName)
                query = "SELECT T0.U_AB_SentSupplier "
                query += " FROM OPOR T0 "
                query += " WHERE U_AB_SentSupplier =  'Y' AND T0.DocEntry = " + docEntry
                dt = connect.ObjectGetAll_Query_SAP(query)
                If dt IsNot Nothing Then
                    If dt.Tables.Count > 0 Then
                        If dt.Tables(0).Rows.Count = 0 Then
                            Return True
                        Else
                            Return False
                        End If
                    End If
                End If
            End If
            Return True
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function SearchPO(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String _
                             , ByVal fromDate As Date, ByVal toDate As Date, ByVal fromDueDate As Date, ByVal toDueDate As Date, ByVal vendorCode As String _
                             , ByVal status As String, ByVal poNo As String, ByVal changeStatus As String _
                             , ByVal outlet As String, ByVal isSuperUser As String) As DataSet
        Try
            'T2.E_Maill AS E_Mail, 1=1 LEFT JOIN OCPR T2 ON T0.CardCode = T2.CardCode
            Dim connect As New Connection()
            Dim dt As New DataSet("PO")
            connect.setSQLDB(databaseName)
            Dim params = New Object() {fromDate, toDate, fromDueDate, toDueDate}
            Dim query As String = "SELECT DISTINCT T0.U_AB_PORemarks,T0.U_AB_Time, T4.SOStatus, Case when T4.SOStatus = 'Failed' Then '' else T4.NumAtCard end AS ConvertedToSO, T0.U_AB_Urgent,T0.U_AB_UserCode,T0.Comments,"
            query += " T0.U_AB_POWhsCode, T0.CardCode,CASE T0.U_AB_SentSupplier WHEN 'Y' THEN 'Sent to Supplier' WHEN 'N' THEN 'Sent to HQ' ELSE 'Sent to HQ' END AS U_AB_SentSupplier"
            query += " ,'' AS USerName, UPPER(T0.U_AB_UserCode) AS U_AB_UserCode, T3.BeginStr + CAST(T0.DocNum AS varchar(20)) AS DocNum,"
            query += " '' AS E_Mail, T0.DocEntry,Convert(date,T0.DocDate,103) [DocDate],T0.DocNum AS DocNumValue,DocTotal,Convert(date,T0.DocDueDate,103) [DocDueDate],T0.CardCode,T0.CardName,T1.CompnyName,"
            query += " CASE DocStatus WHEN 'O' THEN 'Open' WHEN 'C' THEN CASE CANCELED WHEN 'Y' THEN 'Canceled' ELSE  'Closed' END WHEN 'D' THEN 'Delivered' END AS DocStatus"
            query += " FROM OPOR T0 JOIN OADM T1 ON 1=1"
            query += " JOIN NNM1 T3 ON T0.Series = T3.Series"
            query += " LEFT JOIN (SELECT Distinct T11.NumAtCard,T11.U_AB_PODocEntry,CASE when T12.SOStatus='Failed' Then 'Failed' else T11.SOStatus end as SOStatus "
            query += " From AB_SO_Header T11 LEFT JOIN (Select * from AB_SO_Header where SOStatus='Failed') T12 ON T11.U_AB_PODocEntry = T12.U_AB_PODocEntry where YEAR(T11.CreateDate) >= YEAR(GETDATE())) "
            query += " T4 ON T0.DocEntry = T4.U_AB_PODocEntry "
            query += " WHERE (T0.DocDate BETWEEN @Param1 and @Param2 "
            query += " AND T0.DocDueDate BETWEEN @Param3 and @Param4) "
            If status <> "A" Then
                If status <> "Y" Then
                    query += " AND DocStatus = @Param5 AND CANCELED ='N'"
                    params = New Object() {fromDate, toDate, fromDueDate, toDueDate, status}
                    If vendorCode.Length > 0 Then
                        params = New Object() {fromDate, toDate, fromDueDate, toDueDate, status, vendorCode}
                        query += " AND T0.CardCode=@Param6"
                    End If
                Else
                    query += " AND CANCELED = @Param5"
                    params = New Object() {fromDate, toDate, fromDueDate, toDueDate, status}
                    If vendorCode.Length > 0 Then
                        params = New Object() {fromDate, toDate, fromDueDate, toDueDate, status, vendorCode}
                        query += " AND T0.CardCode=@Param6"
                    End If
                End If
            Else
                If vendorCode.Length > 0 Then
                    query += " AND T0.CardCode=@Param5"
                    params = New Object() {fromDate, toDate, fromDueDate, toDueDate, vendorCode}
                End If
            End If
            If poNo.Length > 0 Then
                query += " AND T0.DocNum like '%" + poNo + "%'"
            End If
            If changeStatus <> "A" Then
                If changeStatus <> "S" Then
                    If changeStatus <> "N" Then
                        query += " AND T0.U_AB_SentSupplier = '" + changeStatus + "' AND T4.NumAtCard IS NULL "
                    Else
                        'query += " AND (T0.U_AB_SentSupplier = '" + changeStatus + "' OR  T0.U_AB_SentSupplier IS NULL)  AND T4.NumAtCard IS NULL   "
                        query += " AND (T0.U_AB_SentSupplier = '" + changeStatus + "' OR  T0.U_AB_SentSupplier IS NULL) AND (T4.NumAtCard IS NULL OR (T4.NumAtCard is not NULL AND T4.SOStatus = 'Failed')) "
                    End If
                Else
                    query += " AND T4.NumAtCard IS NOT NULL AND not T4.SOStatus = 'Failed'"
                End If
            End If
            If isSuperUser.ToUpper() = "N" Then
                If outlet.ToUpper() <> "A" Then
                    query += " AND T0.U_AB_POWhsCode = '" + outlet + "'"
                End If
            Else
                If outlet.ToUpper() <> "A" Then
                    query += " AND T0.U_AB_POWhsCode = '" + outlet + "'"
                End If
            End If
            query += " ORDER BY convert(date,T0.DocDueDate,103) DESC,T0.DocNum DESC"
            dt = connect.ObjectGetAll_Query_SAP(query, params)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function GRPOSearch(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String _
                             , ByVal fromDate As Date, ByVal toDate As Date, ByVal vendorCode As String, _
                             ByVal poNo As String, ByVal outlet As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("PO")
            connect.setSQLDB(databaseName)
            Dim params = New Object() {fromDate, toDate}
            Dim query As String = "SELECT T0.U_AB_POWhsCode,T3.BeginStr + CAST(T0.DocNum AS varchar(20)) AS DocNum,T2.E_Mail, T0.DocEntry,T0.DocDate,T0.DocNum AS DocNumValue,DocTotal,T0.DocDueDate,T0.CardCode,T0.CardName,T1.CompnyName, CASE DocStatus WHEN 'O' THEN 'Open' WHEN 'C' THEN CASE CANCELED WHEN 'Y' THEN 'Canceled' ELSE  'Closed' END WHEN 'D' THEN 'Delivered' END AS DocStatus"
            query += " FROM OPDN T0 JOIN OADM T1 ON 1=1 JOIN OCRD T2 ON T0.CardCode = T2.CardCode"
            query += " JOIN NNM1 T3 ON T0.Series = T3.Series"
            query += " WHERE (T0.DocDate BETWEEN @Param1 and @Param2) "
            If vendorCode.Length > 0 Then
                query += " AND T0.CardCode=@Param3"
                params = New Object() {fromDate, toDate, vendorCode}
            End If
            If poNo.Length > 0 Then
                query += " AND T0.DocNum like '%" + poNo + "%'"
            End If
            If outlet <> "A" Then
                query += " AND T0.U_AB_POWhsCode ='" + outlet + "'"
            End If
            query += " ORDER BY  T0.CardName,T0.DocDueDate"
            dt = connect.ObjectGetAll_Query_SAP(query, params)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function DraftStockTake(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String _
                             , ByVal dtDate As Date, _
                             ByVal outlet As String, ByVal isSuperUser As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("PODraft")
            connect.setSQLDB(databaseName)
            Dim params = New Object() {dtDate, outlet}
            Dim query As String = "SELECT SUM(Quantity) AS Quantity, T0.DocEntry, DocNum,T0.DocDate,DocTotal,U_AB_POWhsCode,Comments "
            query += " FROM ODRF T0 JOIN DRF1 T1 ON T0.DocEntry = T1.DocEntry "
            query += " WHERE T0.ObjType = '60' AND T0.DocDate = @Param1  AND U_AB_POWhsCode = @Param2 AND T0.CANCELED ='N' AND T0.DocStatus='O' "
            query += " GROUP BY   T0.DocEntry,DocNum,T0.DocDate,DocTotal, U_AB_POWhsCode,Comments"
            dt = connect.ObjectGetAll_Query_SAP(query, params)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function DraftPO(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String _
                             , ByVal fromDate As Date, ByVal toDate As Date, ByVal fromDueDate As Date, ByVal toDueDate As Date, _
                             ByVal vendorCode As String, ByVal status As String, ByVal outlet As String, ByVal isSuperUser As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("PODraft")
            connect.setSQLDB(databaseName)
            Dim params = New Object() {fromDate, toDate, fromDueDate, toDueDate}
            Dim query As String = "SELECT DISTINCT T0.U_AB_PORemarks, '' AS UserName,T0.U_AB_POWhsCode,UPPER(T0.U_AB_UserCode) AS U_AB_UserCode,UPPER(T4.WhsCode) AS WhsCode, T3.BeginStr + CAST(T0.DocNum AS varchar(20)) AS DocNum,T0.DocEntry,T0.DocDate,T0.DocNum AS DocNumValue,DocTotal,T0.DocDueDate,T0.CardCode,T0.CardName,T1.CompnyName, CASE DocStatus WHEN 'O' THEN 'Open' WHEN 'C' THEN CASE CANCELED WHEN 'Y' THEN 'Canceled' ELSE  'Closed' END WHEN 'D' THEN 'Delivered' END AS DocStatus"
            query += " FROM ODRF T0 join OADM T1 ON 1=1"
            query += " JOIN NNM1 T3 ON T0.Series = T3.Series"
            query += " JOIN DRF1 T4 ON T0.DocEnTry = T4.DocEnTry"
            query += " WHERE T0.ObjType = '22' AND (T0.DocDate BETWEEN @Param1 and @Param2 "
            query += " AND T0.DocDueDate BETWEEN @Param3 and @Param4) "
            If status <> "A" Then
                params = New Object() {fromDate, toDate, fromDueDate, toDueDate, status}
                query += " AND DocStatus = @Param5"
                If vendorCode.Length > 0 Then
                    params = New Object() {fromDate, toDate, fromDueDate, toDueDate, status, vendorCode}
                    query += " AND T0.CardCode=@Param6"
                End If
            Else
                If vendorCode.Length > 0 Then
                    query += " AND CardCode=@Param5"
                    params = New Object() {fromDate, toDate, fromDueDate, toDueDate, vendorCode}
                End If
            End If
            If outlet <> "A" Then
                If isSuperUser.ToUpper() = "N" Then
                    query += " AND T0.U_AB_POWhsCode = '" + outlet + "'"
                End If
            End If
            query += " ORDER BY  T0.CardName,T0.DocDueDate"
            dt = connect.ObjectGetAll_Query_SAP(query, params)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetOutStandingPO(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, _
                                     ByVal fromDate As Date, ByVal toDate As Date, ByVal fromDueDate As Date, ByVal toDueDate As Date, ByVal vendorCode As String, ByVal poNo As String _
                                     , ByVal outlet As String, ByVal isSuperUser As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("PO")
            connect.setSQLDB(databaseName)
            Dim params = New Object() {fromDate, toDate, fromDueDate, toDueDate}
            Dim query As String = "SELECT DISTINCT CASE T0.U_AB_SentSupplier WHEN 'Y' THEN 'Sent to Supplier' WHEN 'N' THEN 'Sent to HQ' ELSE 'Sent to HQ' END AS U_AB_SentSupplier,  T4.NumAtCard AS ConvertedToSO,'' AS UserName,T0.U_AB_POWhsCode,T0.U_AB_UserCode, T3.BeginStr + CAST(T0.DocNum AS varchar(20)) AS DocNum,T2.E_Mail, T0.DocEntry,T0.DocDate,T0.DocNum AS DocNumValue,DocTotal,T0.DocDueDate,T0.CardCode,T0.CardName,T1.CompnyName, CASE DocStatus WHEN 'O' THEN 'Open' WHEN 'C' THEN CASE CANCELED WHEN 'Y' THEN 'Canceled' ELSE  'Closed' END WHEN 'D' THEN 'Delivered' END AS DocStatus "
            query += " FROM OPOR T0 JOIN OADM T1 ON 1=1 JOIN OCRD T2 ON T0.CardCode = T2.CardCode "
            query += " JOIN NNM1 T3 ON T0.Series = T3.Series"
            query += " LEFT JOIN AB_SO_Header T4 ON T0.DocEntry = T4.U_AB_PODocEntry"
            query += " WHERE (T0.U_AB_SentSupplier = 'Y' OR  T4.NumAtCard IS NOT NULL ) AND CANCELED ='N' AND DocStatus='O' AND (T0.DocDate BETWEEN @Param1 and @Param2 "
            query += " AND T0.DocDueDate BETWEEN @Param3 and @Param4) "
            If vendorCode.Length > 0 Then
                query += " AND T0.CardCode=@Param5"
                params = New Object() {fromDate, toDate, fromDueDate, toDueDate, vendorCode}
            End If
            If poNo.Length > 0 Then
                query += " AND T0.DocNum like '%" + poNo + "%'"
            End If
            If outlet <> "A" Then
                query += " AND T0.U_AB_POWhsCode = '" + outlet + "'"
            End If
            query += " Order By T0.DocDueDate DESC"
            'query += " AND T0.U_AB_UserCode = '" + userId + "'"
            dt = connect.ObjectGetAll_Query_SAP(query, params)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function CheckSlpName(ByVal databaseName As String, ByVal userCode As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Item")
            connect.setSQLDB(databaseName)
            Dim query As String = "SELECT * "
            query += "  FROM OSLP WHERE SlpName = @Param1"
            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {userCode})
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetGRPO(ByVal databaseName As String, ByVal docEntry As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Item")
            connect.setSQLDB(databaseName)
            Dim query As String = "SELECT  T3.BeginStr + CAST(T0.DocNum AS varchar(20)) AS DocNum "
            query += "  FROM OPDN T0 JOIN NNM1 T3 ON T0.Series = T3.Series WHERE DocEntry = @Param1"
            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {docEntry})
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function CheckOnHand(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, ByVal outletCode As String, ByVal itemCode As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Item")
            connect.setSQLDB(databaseName)
            Dim query As String = "SELECT ItemCode, OnHand FROM OITW "
            query += " WHERE WhsCode =@Param1"
            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {outletCode})
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetItemStockTake(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, ByVal outletCode As String, ByVal stockDate As Date) As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Item")
            'connect.setSQLDB(databaseName)
            connect.setHoldingDB(oCompany)

            'Dim sInputDate As String = stockDate.ToString()
            'Dim finalDate As String = String.Empty
            'Dim sStringArray As String() = sInputDate.Split("/"c)
            'If sStringArray(0).Length <= 1 Then
            '    sStringArray(0) = "0" + sStringArray(0)
            'End If
            'If sStringArray(1).Length <= 1 Then
            '    sStringArray(1) = "0" + sStringArray(1)
            'End If

            'finalDate = sStringArray(1) + "/" + sStringArray(0) + "/" + sStringArray(2)


            Dim query As String = "SELECT T3.OutletCode, Convert(int,'') AS Cumulative, T0.LastPurPrc, T0.avgPrice,T0.frgnName, T0.ShipType, T0.VatgroupPu, T0.InvntryUoM AS UOM,T0.itemcode,T0.ItemName,T2.Rate "
            query += "   FROM [" + databaseName + "].dbo.OITM T0 JOIN Item_Supplier T1 ON T0.itemcode = T1.itemcode  "
            query += "  LEFT JOIN OVTG T2 ON T0.VatgroupPu = T2.Code "
            query += "  JOIN Item_Outlet T3 ON T1.ID = T3.HeaderID "
            query += " WHERE T3.OutletCode = @Param1"

            query += " UNION ALL"
            query += " SELECT WareHouse AS OutletCode,sum(Cumulative) AS Cumulative, T1.LastPurPrc, T1.avgPrice,T1.frgnName, T1.ShipType, "
            query += " T1.VatgroupPu, T1.InvntryUoM AS UOM,T0.itemcode,T1.ItemName,T3.Rate FROM "
            query += " ( SELECT DocDate,ItemCode ,WareHouse, (SUM(InQty)-SUM(OutQty)) AS Cumulative  "
            query += " FROM [" + databaseName + "].dbo.OINM where DocDate <= '" + stockDate + "' "
            query += " GROUP BY DocDate,ItemCode,WareHouse,InQty,OutQty) AS T0  "
            query += " INNER JOIN  [" + databaseName + "].dbo.OITM T1 ON T0.ItemCode=T1.ItemCode "
            query += " LEFT JOIN OVTG T3 ON T1.VatgroupPu = T3.Code  "
            query += " WHERE T0.Warehouse =  @Param1  AND  T0.ItemCode not in "
            query += " (select T0.ItemCode from Item_Supplier T0"
            query += " INNER JOIN Item_Outlet T1 ON T1.HeaderID = T0.ID WHERE T1.OutletCode= @Param1)"
            query += " GROUP BY T0.ItemCode ,WareHouse, T1.LastPurPrc, T1.avgPrice,T1.frgnName, T1.ShipType, "
            query += " T1.VatgroupPu, T1.InvntryUoM, T0.itemcode, T1.ItemName, T3.Rate"
            query += " Having(SUM(Cumulative) <> 0)"
            query += " ORDER BY T0.ItemCode ASC"

            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {outletCode})
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetItemCumulative(ByVal databaseName As String, ByVal outletCode As String, ByVal strDate As DateTime) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Item")
            connect.setSQLDB(databaseName)
            Dim query As String = " SELECT ItemCode,WareHouse,SUM(Cumulative) AS Cumulative"
            query += "  FROM ( SELECT DocDate,ItemCode ,WareHouse, (SUM(InQty)-SUM(OutQty)) AS Cumulative  "
            query += "  FROM OINM    "
            query += "  GROUP BY DocDate,ItemCode,WareHouse,InQty,OutQty) AS T0  "
            query += " WHERE T0.Warehouse = @Param1 AND  T0.DocDate <= @Param2 "
            query += " GROUP BY ItemCode ,WareHouse"
            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {outletCode, strDate})
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetStockTakeList(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Item")
            connect.setSQLDB(databaseName)
            'Dim query As String = "SELECT SUM(Quantity) AS Quantity, T0.DocEntry, DocNum,T0.DocDate,DocTotal,U_AB_POWhsCode,Comments "
            'query += " FROM OIGE T0 JOIN IGE1 T1 ON T0.DocEntry = T1.DocEntry "
            'query += " GROUP BY T0.DocEntry,DocNum,T0.DocDate,DocTotal, U_AB_POWhsCode,Comments"
            'Dim query As String = "SELECT SUM(T2.Debit) AS Quantity, T0.DocEntry, DocNum,T0.DocDate,DocTotal,"
            'query += " U_AB_POWhsCode,Comments"
            'query += " FROM OIGE T0 JOIN IGE1 T1 ON T0.DocEntry = T1.DocEntry"
            'query += " LEFT JOIN JDT1 T2 ON T0.TransId=T2.TransId"
            'query += " GROUP BY T0.DocEntry,DocNum,T0.DocDate,DocTotal, U_AB_POWhsCode,Comments,T2.TransId"

            Dim query As String = " SELECT  (SELECT IsNull(SUM(Debit),0) from JDT1 WHERE TransId=T0.TransId) AS Quantity,"
            query += " T0.DocEntry, DocNum,T0.DocDate,IsNull(DocTotal,0) As DocTotal,"
            query += " U_AB_POWhsCode, Comments"
            query += " FROM OIGE T0 JOIN IGE1 T1 ON T0.DocEntry = T1.DocEntry"
            query += " GROUP BY T0.DocEntry,DocNum,T0.DocDate,DocTotal, U_AB_POWhsCode,Comments,t0.TransId"
            dt = connect.ObjectGetAll_Query_SAP(query)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetStockTakeDraftDetail(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, ByVal outletCode As String, ByVal docEntry As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Item")
            connect.setSQLDB(databaseName)

            Dim query As String = "SELECT *,0 AS Cumulative,T2.InvntryUoM AS UOM ,U_AB_CountedQty "
            query += " FROM ODRF T0 JOIN DRF1 T1 ON T0.DocEntry = T1.DocEntry JOIN OITM T2 ON T1.ItemCode =  T2.ItemCode "
            query += "WHERE T0.DocEntry = @Param1 AND T1.WhsCode = @Param2 "
            query += "ORDER BY T1.ItemCode ASC"

            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {docEntry, outletCode})
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetStockTakeDetail(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, ByVal outletCode As String, ByVal docEntry As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Item")
            connect.setSQLDB(databaseName)
            Dim query As String = "SELECT *,0 AS Cumulative,T2.InvntryUoM AS UOM  ,U_AB_CountedQty "
            query += "FROM OIGE T0 JOIN IGE1 T1 ON T0.DocEntry = T1.DocEntry JOIN OITM T2 ON T1.ItemCode =  T2.ItemCode "
            query += "WHERE T0.DocEntry = @Param1 AND T1.WhsCode = @Param2 "
            query += "ORDER BY T1.ItemCode ASC"
            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {docEntry, outletCode})
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetLastPurchasePrice(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Item")
            connect.setSQLDB(databaseName)
            Dim query As String = "SELECT T0.ItemCode, T0.ItemName ,T2.DocEnTry,T1.Price "
            query += " FROM OITM T0 JOIN POR1 T1 ON T0.ItemCode = T1.ItemCode JOIN OPOR T2 ON T1.DocEntry = T2.DocEntry "
            query += " ORDER BY T2.DocEntry DESC "
            dt = connect.ObjectGetAll_Query_SAP(query)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetLogo(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String) As String
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("PO")
            connect.setHoldingDB(oCompany)
            Dim query As String = "SELECT U_Logo FROM [@AB_COMPANYSETUP] WHERE U_DBName = @Param1"
            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {databaseName})
            If dt IsNot Nothing Then
                If dt.Tables.Count > 0 And dt.Tables(0).Rows.Count > 0 Then
                    If dt.Tables(0).Rows(0)(0).ToString().Length > 0 Then
                        Dim filename As String = dt.Tables(0).Rows(0)(0).ToString()
                        Return filename
                    End If
                End If
                Return String.Empty
            End If
            Return String.Empty
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetDefaultWareHouse(ByVal ItemCode As String, ByVal company As String) As DataSet
        Try
            Dim connect As New Connection()
            connect.setSQLDB(company)
            Dim dt As New DataSet("OITM")
            Dim query = "SELECT DfltWH,U_AB_Dept FROM OITM  WHERE ItemCode='" + ItemCode + "'"
            dt = connect.ObjectGetAll_Query_SAP(query)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
        Finally
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function GetItem(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String _
                            , ByVal PODate As Date, ByVal ItemCode As String, ByVal VendorCode As String _
                            , ByVal Quantity As Double) As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim vObj As SAPbobsCOM.SBObob
            Dim rs As SAPbobsCOM.Recordset
            connect.connectHoldingDB(oCompany)
            If oCompany.Connected Then
                Dim price As Decimal
                vObj = oCompany.GetBusinessObject(BoObjectTypes.BoBridge)
                rs = oCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                rs = vObj.GetItemPrice(VendorCode, ItemCode, Quantity, PODate)
                'price = Math.Round(rs.Fields.Item(0).Value, 2)
                price = rs.Fields.Item(0).Value
                Dim dt As New DataSet("PO")
                'connect.setSQLDB(databaseName)
                'connect.setHoldingDB(oCompany)
                Dim query = "SELECT T0.NumInBuy, T3.TrnspName,T0.ShipType,T0.VatgroupPu,T2.Rate,1 AS Quantity, " + CStr(price) + " AS Price, T0.ItemCode,ItemName,BuyUnitMsr,CAST(CAST(t1.MinStock AS varbinary(20)) AS decimal(10,2)) AS MinStock  , CAST(CAST(t1.MaxStock AS varbinary(20)) AS decimal(10,2)) AS MaxStock, '' LB, 0 ReceivedQty," + CStr(price * Quantity) + " AS LineTotal  FROM  OITM T0 left join oitw T1 on T0.ItemCode=T1.ItemCode and T1.WhsCode='01' LEFT JOIN OVTG T2 ON T0.VatgroupPu = T2.Code JOIN OSHP T3 ON T0.ShipType = T3.TrnspCode  WHERE T0.ItemCode='" + ItemCode + "'"
                dt = connect.ObjectGetAll_Query_SAP(query)
                Return dt
            Else
                Return Nothing
            End If
        Catch ex As Exception
            WriteLog("GetItem" & ex.StackTrace & ":" & ex.Message)
        Finally
            If oCompany.Connected Then
                oCompany.Disconnect()
            End If
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function GetAllItem(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String _
                            , ByVal PODate As Date, ByVal VendorCode As String _
                           , ByVal tb As DataTable) As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim vObj As SAPbobsCOM.SBObob
            Dim rs As SAPbobsCOM.Recordset
            'WriteLog("GetAllItem" + " Before  connectHoldingDB ")
            connect.connectHoldingDB(oCompany)
            'WriteLog("GetAllItem" + " After  connectHoldingDB ")
            If oCompany.Connected Then
                vObj = oCompany.GetBusinessObject(BoObjectTypes.BoBridge)
                rs = oCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                Dim dsResult As DataSet = Nothing
                For Each row As DataRow In tb.Rows
                    Dim price As Decimal
                    vObj = oCompany.GetBusinessObject(BoObjectTypes.BoBridge)
                    rs = oCompany.GetBusinessObject(BoObjectTypes.BoRecordset)
                    rs = vObj.GetItemPrice(VendorCode, row("ItemCode"), 0, PODate)
                    'price = Math.Round(rs.Fields.Item(0).Value, 2)
                    price = rs.Fields.Item(0).Value
                    Dim dt As New DataSet("PO")
                    'connect.setSQLDB(databaseName)
                    'connect.setHoldingDB(oCompany)
                    Dim query = "SELECT T0.NumInBuy, T3.TrnspName,T0.ShipType,T0.VatgroupPu,T2.Rate,1 AS Quantity, " + CStr(price) + " AS Price, T0.ItemCode,ItemName,BuyUnitMsr,CAST(CAST(t1.MinStock AS varbinary(20)) AS decimal(10,2)) AS MinStock  , CAST(CAST(t1.MaxStock AS varbinary(20)) AS decimal(10,2)) AS MaxStock, '' LB, 0 ReceivedQty," + CStr(price * 0) + " AS LineTotal  FROM  OITM T0 left join oitw T1 on T0.ItemCode=T1.ItemCode and T1.WhsCode='01' LEFT JOIN OVTG T2 ON T0.VatgroupPu = T2.Code JOIN OSHP T3 ON T0.ShipType = T3.TrnspCode  WHERE T0.ItemCode='" + row("ItemCode") + "'"
                    dt = connect.ObjectGetAll_Query_SAP(query)
                    If dt IsNot Nothing Then
                        If dt.Tables.Count > 0 Then
                            If dsResult Is Nothing Then
                                dsResult = dt.Clone()
                                dsResult.Tables(0).Columns("Price").DataType = System.Type.GetType("System.String")
                            End If
                            For Each rPrice As DataRow In dt.Tables(0).Rows
                                dsResult.Tables(0).ImportRow(rPrice)
                            Next
                        End If
                    End If
                Next
                Return dsResult
            End If
        Catch ex As Exception
            WriteLog("GetAllItem" & ex.StackTrace & ":" & ex.Message)
            Throw ex
        Finally
            If oCompany.Connected Then
                oCompany.Disconnect()
            End If
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function GetItemEdit(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, ByVal ItemCode As String, ByVal docEntry As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("PO")
            connect.setSQLDB(databaseName)
            Dim query = "SELECT T3.TrnspName,T4.ShipType,T4.VatgroupPu,T2.Rate,Quantity, Price, T0.ItemCode,ItemName,"
            query += " BuyUnitMsr,CAST(CAST(t1.MinStock AS varbinary(20)) AS decimal(10,2)) AS MinStock  ,"
            query += " CAST(CAST(t1.MaxStock AS varbinary(20)) AS decimal(10,2)) AS MaxStock, '' AS LB, 0 ReceivedQty, LineTotal  "
            query += " FROM  POR1 T0 LEFT JOIN OITW T1 on T0.ItemCode=T1.ItemCode AND T1.WhsCode='01 '"
            query += " JOIN OITM T4 ON T0.ItemCode =  T4.ItemCode "
            query += " LEFT JOIN OVTG T2 ON T4.VatgroupPu = T2.Code JOIN OSHP T3 ON T4.ShipType = T3.TrnspCode "
            query += " WHERE T0.ItemCode= @param1 AND T0.DocEntry=@Param2"
            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {ItemCode, docEntry})
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function CopyToGRPO(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, ByVal DocEntry As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("DS")
            Dim params = New Object() {DocEntry}
            Dim err As String = connect.setSQLDB(databaseName)
            If err.Length = 0 Then
                Dim query = "SELECT '' AS SlpName ,T0.DocNum, T0.Address,T0.U_AB_UserCode , T0.CardCode,T0.CardName, T0.DocDate, T0.DocDueDate,T0.DocNum,T0.DocEntry,T0.NumAtCard,T0. SlpCode,T0. Footer,T0.OwnerCode,T0.DocStatus,  "
                query += " CAST(CAST(T0.DocTotal AS varbinary(20)) AS decimal(10,2)) AS DocTotal, CAST(CAST(T0.VATSum AS varbinary(20)) AS decimal(10,2)) AS VATSum,CAST(CAST((T0.doctotal- T0.vatsum ) AS varbinary(20)) AS decimal(10,2)) AS SubTotal"
                query += " FROM OPOR T0 WHERE T0.DocEntry=@Param1"
                dt = connect.ObjectGetAll_Query_SAP(query, params)
                If dt IsNot Nothing Then
                    If dt.Tables.Count > 0 Then
                        dt.Tables(0).TableName = "OPOR"
                    End If
                End If
                query = "SELECT T0.LineStatus, T0.OpenQty,T2.ManBtchNum,T2.VatgroupPu, T3.Rate,T0.WhsCode,T0.ItemCode,T0.Dscription,T0.Quantity,T0.Price, T0.LineTotal,T0.LineNum , T1.PriceBefDi,T2.BuyUnitMsr"
                query += " FROM POR1 T0 LEFT JOIN PDN1 T1 ON (T0.DocEntry = T1.DocEntry AND T0.LineNum = T1.LineNum) LEFT JOIN OITM T2 ON T0.ItemCode = T2.ItemCode "
                query += " LEFT JOIN OVTG T3 ON T2.VatgroupPu = T3.Code "
                query += " WHERE T0.LineStatus ='O' AND T0.DocEntry=@Param1"
                Dim dsItem As DataSet = connect.ObjectGetAll_Query_SAP(query, params)
                If dsItem IsNot Nothing Then
                    If dsItem.Tables.Count > 0 Then
                        dsItem.Tables(0).TableName = "POR1"
                        dt.Tables.Add(dsItem.Tables(0).Copy())
                    End If
                End If
            End If
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetGRPOUpdate(ByVal databaseName As String, ByVal DocEntry As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("DS")
            Dim params = New Object() {DocEntry}
            Dim err As String = connect.setSQLDB(databaseName)
            If err.Length = 0 Then
                Dim query = "SELECT T1.SlpName, T0.Address,T0.U_AB_UserCode , T0.CardCode,T0.CardName, T0.DocDate, T0.DocDueDate,T0.DocNum,T0.DocEntry,T0.NumAtCard,T0. SlpCode,T0. Footer,T0.OwnerCode,T0.DocStatus,  "
                query += " CAST(CAST(T0.DocTotal AS varbinary(20)) AS decimal(10,2)) AS DocTotal, CAST(CAST(T0.VATSum AS varbinary(20)) AS decimal(10,2)) AS VATSum,CAST(CAST((T0.doctotal- T0.vatsum ) AS varbinary(20)) AS decimal(10,2)) AS SubTotal"
                query += " FROM OPDN T0 JOIN OSLP T1 ON T0.SlpCode = T1.SlpCode  WHERE T0.DocEntry=@Param1"
                dt = connect.ObjectGetAll_Query_SAP(query, params)
                If dt IsNot Nothing Then
                    If dt.Tables.Count > 0 Then
                        dt.Tables(0).TableName = "OPOR"
                    End If
                End If
                query = "SELECT T5.Quantity AS POQty,T0.BaseRef As DocNum, T2.ManBtchNum,T2.VatgroupPu, T3.Rate,T0.WhsCode,T0.ItemCode,T0.Dscription,T0.Quantity,T0.Price, T0.LineTotal,T0.LineNum , T1.PriceBefDi,T2.BuyUnitMsr"
                query += " FROM PDN1 T0 LEFT JOIN PDN1 T1 ON (T0.DocEntry = T1.DocEntry AND T0.LineNum = T1.LineNum) LEFT JOIN OITM T2 ON T0.ItemCode = T2.ItemCode "
                query += " LEFT JOIN OVTG T3 ON T2.VatgroupPu = T3.Code JOIN OPOR T4 ON T4.DocNum = T0.BaseRef JOIN POR1 T5 ON T4.DocEntry = T5.DocEntry AND T0.ItemCode = T5.ItemCode"
                query += " WHERE T0.DocEntry=@Param1"
                Dim dsItem As DataSet = connect.ObjectGetAll_Query_SAP(query, params)
                If dsItem IsNot Nothing Then
                    If dsItem.Tables.Count > 0 Then
                        dsItem.Tables(0).TableName = "POR1"
                        dt.Tables.Add(dsItem.Tables(0).Copy())
                    End If
                End If
            End If
            Return dt
        Catch ex As Exception
            WriteLog("GetGRPOUpdate" & ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetPODataUpdate(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, ByVal DocEntry As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("DS")
            Dim params = New Object() {DocEntry}
            Dim err As String = connect.setSQLDB(databaseName)
            If err.Length = 0 Then
                Dim query = "SELECT T0.U_AB_PORemarks, T0.U_AB_POWhsCode, U_AB_SentSupplier,U_AB_Urgent,U_AB_UserCode, DocStatus,T0.Comments, T1.User_Code , T0.CardCode,T0.CardName, T0.DocDate, T0.DocDueDate,T0.DocNum,T0.DocEntry,T0.DocStatus, "
                query += " CAST(CAST(T0.DocTotal AS varbinary(20)) AS decimal(10,2)) AS DocTotal, CAST(CAST(T0.VATSum AS varbinary(20)) AS decimal(10,2)) AS VATSum,CAST(CAST((T0.doctotal- T0.vatsum ) AS varbinary(20)) AS decimal(10,2)) AS SubTotal"
                query += " FROM OPOR T0 JOIN OUSR T1 ON T1.UserID = T0.UserSign WHERE T0.DocEntry=@Param1"
                dt = connect.ObjectGetAll_Query_SAP(query, params)
                If dt IsNot Nothing Then
                    If dt.Tables.Count > 0 Then
                        dt.Tables(0).TableName = "OPOR"
                    End If
                End If
                query = "SELECT T0.U_AB_POQty, CAST(T7.Quantity AS decimal(10,2)) AS BalanceQty ,T7.DocEntry AS IsGRPO, T5.TrnspName,T4.ShipType,T2.ManBtchNum,T2.VatgroupPu, T3.Rate,T0.WhsCode,T0.ItemCode,T0.Dscription,"
                query += " CAST(CAST( T0.Quantity AS varbinary(20)) AS decimal(10,2)) AS Quantity , CAST(CAST( T0.Price AS varbinary(20)) AS decimal(10,2)) AS Price,CAST(CAST( T0.LineTotal AS varbinary(20)) AS decimal(10,2)) AS LineTotal,T0.LineNum , T7.PriceBefDi,T2.BuyUnitMsr"
                query += " FROM POR1 T0  LEFT JOIN OITM T2 ON T0.ItemCode = T2.ItemCode "
                query += " LEFT JOIN OVTG T3 ON T2.VatgroupPu = T3.Code JOIN OITM T4 ON T4.ItemCode = T0.ItemCode JOIN OSHP T5 ON T4.ShipType = T5.TrnspCode "
                query += " JOIN OPOR T6 ON T0.DocEntry= T6.DocEntry LEFT JOIN PDN1 T7 ON T0.ItemCode = T7.ItemCode AND T6.DocNum = T7.BaseRef "
                query += " WHERE T0.DocEntry=@Param1"
                Dim dsItem As DataSet = connect.ObjectGetAll_Query_SAP(query, params)
                If dsItem IsNot Nothing Then
                    If dsItem.Tables.Count > 0 Then
                        dsItem.Tables(0).TableName = "POR1"
                        dt.Tables.Add(dsItem.Tables(0).Copy())
                    End If
                End If
            End If
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetPODraftUpdate(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, ByVal DocEntry As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("DS")
            Dim params = New Object() {DocEntry}
            Dim err As String = connect.setSQLDB(databaseName)
            If err.Length = 0 Then
                Dim query = "SELECT T0.U_AB_PORemarks, T0.U_AB_POWhsCode,U_AB_SentSupplier, U_AB_Urgent,U_AB_UserCode, DocStatus,T0.Comments, T1.User_Code , T0.CardCode,T0.CardName, T0.DocDate, T0.DocDueDate,T0.DocNum,T0.DocEntry,T0.DocStatus, "
                query += " CAST(CAST(T0.DocTotal AS varbinary(20)) AS decimal(10,2)) AS DocTotal, CAST(CAST(T0.VATSum AS varbinary(20)) AS decimal(10,2)) AS VATSum,CAST(CAST((T0.doctotal- T0.vatsum ) AS varbinary(20)) AS decimal(10,2)) AS SubTotal"
                query += " FROM ODRF T0 JOIN OUSR T1 ON T1.UserID = T0.UserSign WHERE T0.DocEntry=@Param1"
                dt = connect.ObjectGetAll_Query_SAP(query, params)
                If dt IsNot Nothing Then
                    If dt.Tables.Count > 0 Then
                        dt.Tables(0).TableName = "ODRF"
                    End If
                End If
                query = "SELECT '0' AS BalanceQty ,'' AS IsGRPO, T0.OcrCode,T5.TrnspName,T4.ShipType,T2.ManBtchNum,T2.VatgroupPu, T3.Rate,T0.WhsCode,T0.ItemCode,T0.Dscription,"
                query += " CAST(CAST( T0.Quantity AS varbinary(20)) AS decimal(10,2)) AS Quantity , CAST(CAST( T0.Price AS varbinary(20)) AS decimal(10,2)) AS Price,CAST(CAST( T0.LineTotal AS varbinary(20)) AS decimal(10,2)) AS LineTotal,T0.LineNum , T0.PriceBefDi,T2.BuyUnitMsr"
                query += " FROM DRF1 T0 LEFT JOIN PDN1 T1 ON (T0.DocEntry = T1.DocEntry AND T0.LineNum = T1.LineNum) LEFT JOIN OITM T2 ON T0.ItemCode = T2.ItemCode "
                query += " LEFT JOIN OVTG T3 ON T2.VatgroupPu = T3.Code JOIN OITM T4 ON T4.ItemCode = T0.ItemCode JOIN OSHP T5 ON T4.ShipType = T5.TrnspCode "
                query += " WHERE T0.DocEntry=@Param1"
                Dim dsItem As DataSet = connect.ObjectGetAll_Query_SAP(query, params)
                If dsItem IsNot Nothing Then
                    If dsItem.Tables.Count > 0 Then
                        dsItem.Tables(0).TableName = "DRF1"
                        dt.Tables.Add(dsItem.Tables(0).Copy())
                    End If
                End If
            End If
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetOutletCalendar(ByVal userId As String, ByVal passWord As String, ByVal cardCode As String, ByVal companyCode As String, ByVal databaseName As String) As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("DS")
            Dim params = New Object() {cardCode, companyCode}
            connect.setHoldingDB(oCompany)
            Dim query = "SELECT * FROM Outlet_Calendar WHERE CardCode=@Param1 AND CompanyCode = @Param2"
            dt = connect.ObjectGetAll_Query_SAP(query, params)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetOutletAmount(ByVal userId As String, ByVal passWord As String, ByVal cardCode As String, ByVal companyCode As String, ByVal databaseName As String) As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("DS")
            Dim params = New Object() {cardCode, companyCode}
            connect.setHoldingDB(oCompany)
            Dim query = "SELECT * FROM Outlet_OrderAmount WHERE CardCode=@Param1 AND CompanyCode = @Param2"
            dt = connect.ObjectGetAll_Query_SAP(query, params)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetOutletSupplier(ByVal userId As String, ByVal passWord As String, ByVal cardCode As String, ByVal companyCode As String, ByVal databaseName As String) As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("DS")
            Dim params = New Object() {cardCode, companyCode}
            connect.setHoldingDB(oCompany)
            'Edit By Hieu Ha 12/8/2014 - START
            'Dim query = "SELECT DISTINCT ID,T1.CardCode,T1.CardName,T1.CompanyCode,T1.CompanyName,T1.ItemCode,T1.ItemName AS Dscription, T0.InvntryUom,T0.BuyUnitMsr ,T1.LB "
            'query += " FROM  Item_Supplier T1 INNER JOIN OITM T0 ON (T1.ItemCode = T0.ItemCode ) WHERE T0.validFor = 'Y'  AND T1.CardCode=@Param1 AND T1.CompanyCode = @Param2"
            Dim query = "SELECT DISTINCT ID,T1.CardCode,T2.CardName,T1.CompanyCode,T1.CompanyName,T1.ItemCode,T0.ItemName AS Dscription, T0.InvntryUom,T0.BuyUnitMsr ,T1.LB "
            query += " FROM  Item_Supplier T1 INNER JOIN OITM T0 ON (T1.ItemCode = T0.ItemCode ) "
            query += " INNER JOIN OCRD T2 ON T1.CardCode = T2.CardCode"
            query += " WHERE T0.validFor = 'Y' AND T1.CardCode=@Param1 AND T1.CompanyCode = @Param2"
            'Edit By Hieu Ha 12/8/2014 - END
            dt = connect.ObjectGetAll_Query_SAP(query, params)

            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetItemOutletSetup(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String _
                            , ByVal ItemCode As String) As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("PO")
            connect.setHoldingDB(oCompany)
            Dim query = "SELECT InvntryUom, '' AS LB, BuyUnitMsr FROM  OITM  WHERE ItemCode='" + ItemCode + "'"
            dt = connect.ObjectGetAll_Query_SAP(query)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function

    <WebMethod()> _
    Public Function GetSupplierOutletSetup(ByVal companyCode As String, ByVal outletCode As String) As DataSet
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("OITM")
            connect.setHoldingDB(oCompany)
            Dim params = New Object() {companyCode}
            Dim query As String = "SELECT DISTINCT T0.CardCode, T0.CardName FROM OCRD T0 INNER JOIN Item_Supplier T1 ON T0.CardCode = T1.CardCode"
            query += " JOIN Item_Outlet T2 ON T2.HeaderID = T1.ID "
            query += " WHERE T1.CompanyCode = @Param1"
            If outletCode.Length > 0 Then
                query += "  AND OutletCode = @Param2"
                params = New Object() {companyCode, outletCode}
            End If
            query += " UNION ALL Select 'ZZZ','No Vendor'"
            query += "  ORDER BY T0.CardCode ASC"
            dt = connect.ObjectGetAll_Query_SAP(query, params)
            Return dt
        Catch ex As Exception

            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetSupplierItemSetup(ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, ByVal cardCode As String, ByVal WhsCode As String, ByVal DocDate As Date) As DataSet
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim dt As New DataSet("OITM")
            connect.setHoldingDB(oCompany)
            Dim params = New Object() {cardCode, databaseName, WhsCode, DocDate}
            'dt = connect.ObjectGetAll_Query_SAP("SELECT DISTINCT T2.U_AB_Dept,T1.ItemCode, T2.ItemName,T1.LB,T0.MinQty,T0.MaxQty FROM Item_Supplier T1 JOIN Item_Outlet T0 ON T1.ID = T0.HeaderID  JOIN OITM T2 ON T1.ItemCode = T2.ItemCode WHERE T2.validFor = 'Y' AND T1.CardCode = @Param1 AND T1.CompanyCode = @Param2 AND T0.OutletCode = @Param3 ORDER BY T2.U_AB_Dept,T1.ItemCode,T1.LB,T0.MinQty,T0.MaxQty ASC", params)
            If cardCode = "ZZZ" Then
                Dim sql As String

                sql = "select T3.U_AB_Dept,T3.ItemCode, T3.ItemName,'' LB,0 MinQty,0 MaxQty " & _
                "from (select ItemCode,warehouse, (SUM(InQty)-SUM(OutQty)) AS Cumulative " & _
                "from [" + databaseName + "].dbo.OINM where Warehouse = @Param3 AND  DocDate <= @Param4 " & _
                "group by ItemCode,Warehouse) T0 " & _
                "left join(select T0.ItemCode, T1.* from Item_Supplier T0 INNER JOIN Item_Outlet T1 ON T1.HeaderID = T0.ID WHERE T1.OutletCode= @Param3) T1 " & _
                "on T0.ItemCode=T1.ItemCode and T0.Warehouse = T1.OutletCode " & _
                "INNER JOIN  [" + databaseName + "].dbo.OITM T3 ON T0.ItemCode=T3.ItemCode " & _
                "LEFT JOIN OVTG T4 ON T3.VatgroupPu = T4.Code " & _
                "where(T0.Cumulative > 0 And T1.ItemCode Is null) " & _
                "ORDER BY T0.ItemCode ASC"
                dt = connect.ObjectGetAll_Query_SAP(sql, params)
            Else
                dt = connect.ObjectGetAll_Query_SAP("SELECT DISTINCT T2.U_AB_Dept,T1.ItemCode, T2.ItemName,T1.LB,T0.MinQty,T0.MaxQty FROM Item_Supplier T1 JOIN Item_Outlet T0 ON T1.ID = T0.HeaderID  JOIN OITM T2 ON T1.ItemCode = T2.ItemCode WHERE T2.validFor = 'Y' AND T1.CardCode = @Param1 AND T1.CompanyCode = @Param2 AND T0.OutletCode = @Param3 ORDER BY T2.U_AB_Dept,T1.ItemCode,T1.LB,T0.MinQty,T0.MaxQty ASC", params)
            End If

            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetOutletItemList(ByVal userId As String, ByVal passWord As String, ByVal cardCode As String, ByVal companyCode As String, ByVal itemCode As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim dt As New DataSet("DS")
            Dim params = New Object() {cardCode, companyCode, itemCode}
            connect.setHoldingDB(oCompany)
            Dim query = "SELECT *, 1 AS isCheck FROM Item_Outlet T0 "
            query += " INNER JOIN Item_Supplier T1 ON T0.HeaderID  = T1.ID  "
            query += " WHERE T1.CardCode=@Param1 AND T1.CompanyCode = @Param2 AND T1.ItemCode = @Param3"
            dt = connect.ObjectGetAll_Query_SAP(query, params)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function CheckAddPO(ByVal userId As String, ByVal passWord As String, ByVal companyCode As String, ByVal data As DataSet, ByVal docDueDate As DateTime, _
                               ByVal isUpdate As Boolean, ByVal docEntry As String) As String
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim dt As New DataSet("DS")
            If data IsNot Nothing And data.Tables.Count > 0 Then
                Dim tbHeader As DataTable = data.Tables(0)
                Dim cardCode As String = String.Empty
                Dim outletCode As String = String.Empty
                'Check Delivery Date
                If tbHeader.Rows.Count > 0 Then
                    cardCode = tbHeader.Rows(0)("CardCode")
                    outletCode = tbHeader.Rows(0)("OutletCode")
                    connect.setHoldingDB(oCompany)
                    Dim dayOfWeek = Weekday(docDueDate)
                    Dim day = WeekdayName(dayOfWeek).Substring(0, 3)
                    Dim query = "SELECT Mon,Tue,Wed,Thu,Fri,Sat,Sun FROM Outlet_Calendar WHERE CardCode = @Param1 AND CompanyCode = @Param2 AND OutletCode = @Param3"
                    Dim dsCalendar As DataSet = connect.ObjectGetAll_Query_SAP(query, New Object() {tbHeader.Rows(0)("CardCode"), companyCode, tbHeader.Rows(0)("OutletCode")})
                    If dsCalendar IsNot Nothing And dsCalendar.Tables.Count > 0 And dsCalendar.Tables(0).Rows.Count > 0 Then
                        If Boolean.Parse(dsCalendar.Tables(0).Rows(0)(day)) = False Then
                            Return "Invalid Date, pls. select expected delivery date again."
                        End If
                    End If
                End If
                'Check Quantity
                Dim tbMaster As DataTable = data.Tables(1)
                Dim totalQuantity As Double
                Dim totalLineTotal As Double
                Dim lineTotal As Double
                If tbMaster IsNot Nothing Then
                    For Each row As DataRow In tbMaster.Rows
                        totalQuantity = 0
                        'Get Quanity group by delivery date
                        connect.setSQLDB(companyCode)
                        Dim query = "SELECT SUM(T1.Quantity) AS TotalQuantity,SUM(T1.LineTotal) AS TotalLineTotal FROM OPOR T0  JOIN POR1 T1 ON T0.DocEntry =  T1.DocEntry "
                        If isUpdate And docEntry.Length > 0 Then
                            'query += " WHERE T0.CardCode = @Param1 AND T1.ItemCode=@Param2 AND T0.DocDueDate = @Param3 AND T0.DocEntry <> @Param4 AND T0.U_AB_POWhsCode = @Param4"
                            query += " WHERE T0.DocStatus = 'O' AND T0.CANCELED='N' AND T0.CardCode = @Param1 AND T1.ItemCode=@Param2 AND T0.DocDueDate = @Param3 AND T0.DocEntry <> @Param4 AND T0.U_AB_POWhsCode = @Param4"
                            query += " GROUP BY T0.DocDueDate"
                            Dim dsTotalQuantity As DataSet = connect.ObjectGetAll_Query_SAP(query, New Object() {cardCode, row("ItemCode").ToString(), docDueDate, docEntry, outletCode})
                            If dsTotalQuantity IsNot Nothing And dsTotalQuantity.Tables.Count > 0 Then
                                If dsTotalQuantity.Tables(0).Rows.Count > 0 Then
                                    totalQuantity = Double.Parse(dsTotalQuantity.Tables(0).Rows(0)("TotalQuantity").ToString())
                                End If
                            End If
                        Else
                            query += " WHERE T0.DocStatus = 'O' AND T0.CANCELED='N' AND T0.CardCode = @Param1 AND T1.ItemCode=@Param2 AND T0.DocDueDate = @Param3 AND T0.U_AB_POWhsCode = @Param4"
                            query += " GROUP BY T0.DocDueDate"
                            Dim dsTotalQuantity As DataSet = connect.ObjectGetAll_Query_SAP(query, New Object() {cardCode, row("ItemCode").ToString(), docDueDate, outletCode})
                            If dsTotalQuantity IsNot Nothing And dsTotalQuantity.Tables.Count > 0 Then
                                If dsTotalQuantity.Tables(0).Rows.Count > 0 Then
                                    totalQuantity = Double.Parse(dsTotalQuantity.Tables(0).Rows(0)("TotalQuantity").ToString())
                                End If
                            End If
                        End If
                        'Get Setup Quantity 
                        connect.setHoldingDB(oCompany)
                        query = "SELECT * FROM Item_Supplier T0 JOIN Item_Outlet T1  ON T0.ID =  T1.HeaderID"
                        query += " WHERE CardCode = @Param1  AND CompanyCode = @Param2 AND OutletCode =@Param3 AND ItemCode=@Param4"
                        Dim dsSetup As DataSet = connect.ObjectGetAll_Query_SAP(query, New Object() {cardCode, companyCode, outletCode, row("ItemCode").ToString()})
                        If dsSetup IsNot Nothing And dsSetup.Tables.Count > 0 And dsSetup.Tables(0).Rows.Count > 0 Then
                            If dsSetup.Tables(0).Rows.Count > 0 Then
                                Dim r As DataRow = dsSetup.Tables(0).Rows(0)
                                If Double.Parse(row("Quantity").ToString()) < Double.Parse(r("MinQty").ToString()) Then
                                    Return "[" + row("ItemCode").ToString() + "] - Order quantity less than Minimum Quantity, pls. check again."
                                ElseIf Double.Parse(row("Quantity").ToString()) + totalQuantity > Double.Parse(r("MaxQty").ToString()) Then
                                    Return "[" + row("ItemCode").ToString() + "] - Order quantity exceed Maximum Quantity, pls. check again."
                                End If
                            End If
                        End If
                        'Sum line total
                        lineTotal += Double.Parse(row("LineTotal").ToString())
                    Next
                    'Get Line Total group by delivery date
                    connect.setSQLDB(companyCode)
                    Dim QueryAmount = "SELECT SUM(T1.LineTotal) AS TotalLineTotal FROM OPOR T0  JOIN POR1 T1 ON T0.DocEntry =  T1.DocEntry "
                    If isUpdate And docEntry.Length > 0 Then
                        'QueryAmount += " WHERE T0.CardCode = @Param1 AND T1.WhsCode=@Param2 AND T0.DocDueDate = @Param3 AND T0.DocEntry <> @Param4"
                        QueryAmount += " WHERE T0.DocStatus = 'O' AND T0.CANCELED='N' AND T0.CardCode = @Param1 AND T1.WhsCode=@Param2 AND T0.DocDueDate = @Param3 AND T0.DocEntry <> @Param4"
                        QueryAmount += " GROUP BY T0.DocDueDate"
                        Dim dsTotalLine = connect.ObjectGetAll_Query_SAP(QueryAmount, New Object() {cardCode, outletCode, docDueDate, docEntry})
                        If dsTotalLine IsNot Nothing And dsTotalLine.Tables.Count > 0 Then
                            If dsTotalLine.Tables(0).Rows.Count > 0 Then
                                totalLineTotal = Double.Parse(dsTotalLine.Tables(0).Rows(0)("TotalLineTotal").ToString())
                            End If
                        End If
                    Else
                        QueryAmount += " WHERE T0.DocStatus = 'O' AND T0.CANCELED='N' AND T0.CardCode = @Param1 AND T1.WhsCode=@Param2 AND T0.DocDueDate = @Param3"
                        QueryAmount += " GROUP BY T0.DocDueDate"
                        Dim dsTotalLine = connect.ObjectGetAll_Query_SAP(QueryAmount, New Object() {cardCode, outletCode, docDueDate})
                        If dsTotalLine IsNot Nothing And dsTotalLine.Tables.Count > 0 Then
                            If dsTotalLine.Tables(0).Rows.Count > 0 Then
                                totalLineTotal = Double.Parse(dsTotalLine.Tables(0).Rows(0)("TotalLineTotal").ToString())
                            End If
                        End If
                    End If
                    'Get Setup  Amount
                    connect.setHoldingDB(oCompany)
                    QueryAmount = "SELECT * FROM Outlet_OrderAmount "
                    QueryAmount += " WHERE CardCode = @Param1  AND CompanyCode = @Param2 AND OutletCode =@Param3"
                    Dim dsSetupAmount = connect.ObjectGetAll_Query_SAP(QueryAmount, New Object() {cardCode, companyCode, outletCode})
                    If dsSetupAmount IsNot Nothing And dsSetupAmount.Tables.Count > 0 Then
                        If dsSetupAmount.Tables(0).Rows.Count > 0 Then
                            Dim r As DataRow = dsSetupAmount.Tables(0).Rows(0)
                            If lineTotal + totalLineTotal < Double.Parse(r("MinAmt").ToString()) Then
                                Return "Outlet : [" + outletCode + "] - Insufficient Amount, pls. check again."
                            ElseIf lineTotal + totalLineTotal > Double.Parse(r("MaxAmt").ToString()) Then
                                Return "Outlet : [" + outletCode + "] - Over Maximum PO Amount, pls. check again."
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Return ex.Message
        End Try
        Return String.Empty
    End Function
    <WebMethod()> _
    Public Function GetSupplierAmount(ByVal cardCode As String, ByVal companyCode As String, ByVal outletCode As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim dt As New DataSet("DS")
            Dim params = New Object() {cardCode, companyCode, outletCode}
            connect.setHoldingDB(oCompany)
            Dim query = "SELECT * FROM Outlet_OrderAmount "
            query += " WHERE CardCode = @Param1  AND CompanyCode = @Param2 AND OutletCode =@Param3"
            dt = connect.ObjectGetAll_Query_SAP(query, params)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function UpdateOutletCalendar(ByVal userId As String, ByVal passWord As String, _
                                          ByVal cardCode As String, ByVal cardName As String, _
                                         ByVal companyCode As String, ByVal companyName As String, _
                                         ByVal data As DataSet, ByVal isUpdate As String, ByVal databaseName As String) As String
        Try
            Dim connect As New Connection()

            For Each row As DataRow In data.Tables(0).Rows
                Dim count As Integer = ExcuteOutletCalendar(userId, passWord, cardCode, cardName, companyCode, companyName, row("OutletCode").ToString(), row("OutletName").ToString(), _
                                                            Boolean.Parse(row("Mon").ToString()), _
                                                            Boolean.Parse(row("Tue").ToString()), Boolean.Parse(row("Wed").ToString()), Boolean.Parse(row("Thu").ToString()), _
                                                            Boolean.Parse(row("Fri").ToString()), Boolean.Parse(row("Sat").ToString()), Boolean.Parse(row("Sun").ToString()), _
                                                            isUpdate, databaseName, row("ID").ToString())
            Next
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Return ex.Message
        End Try
        Return String.Empty
    End Function
    Public Function ExcuteOutletCalendar(ByVal userId As String, ByVal passWord As String, _
                                         ByVal cardCode As String, ByVal cardName As String, _
                                         ByVal companyCode As String, ByVal companyName As String, _
                                         ByVal outletCode As String, ByVal outletName As String, _
                                         ByVal Mon As Boolean, ByVal Tue As Boolean, ByVal Web As Boolean, _
                                         ByVal Thu As Boolean, ByVal Fri As Boolean, ByVal Sat As Boolean, ByVal Sun As Boolean, _
                                         ByVal isUpdate As String, ByVal databaseName As String, ByVal ID As String) As String
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                Dim params = New Object() {cardCode, cardName, companyCode, companyName, outletCode, outletName, Mon, Tue, Web, Thu, Fri, Sat, Sun}
                Dim query As String = String.Empty
                If isUpdate = "0" Or ID = "" Then
                    query = "IF NOT EXISTS (SELECT * FROM Outlet_Calendar WHERE CardCode ='" + cardCode + "' AND CompanyCode='" + companyCode + "' AND OutletCode ='" & outletCode & "') BEGIN"
                    query += " INSERT INTO Outlet_Calendar(CardCode,CardName,CompanyCode,CompanyName,OutletCode,OutletName,Mon,Tue,Wed,Thu,Fri,Sat,Sun)"
                    query += " VALUES(@Param1,@Param2,@Param3,@Param4,@Param5,@Param6,@Param7,@Param8,@Param9,@Param10,@Param11,@Param12,@Param13)"
                    query += " END"
                Else
                    params = New Object() {Mon, Tue, Web, Thu, Fri, Sat, Sun, outletCode, cardCode, companyCode}
                    query = "UPDATE Outlet_Calendar SET Mon =@Param1,Tue=@Param2,Wed=@Param3,Thu=@Param4,Fri=@Param5,Sat=@Param6,Sun=@Param7"
                    query += " WHERE OutletCode = @Param8 AND CardCode = @Param9 AND CompanyCode = @Param10"
                End If
                Dim count As Integer = connect.Object_Execute_SAP(query, params)
                Return count
            End If
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
        End Try
        Return 0
    End Function
    <WebMethod()> _
    Public Function UpdateOutletOrderAmount(ByVal userId As String, ByVal passWord As String, _
                                          ByVal cardCode As String, ByVal cardName As String, _
                                         ByVal companyCode As String, ByVal companyName As String, _
                                         ByVal data As DataSet, ByVal isUpdate As String, ByVal databaseName As String) As String
        Try

            Dim connect As New Connection()
            For Each row As DataRow In data.Tables(0).Rows
                Dim count As Integer = ExcuteOutletOrderAmount(userId, passWord, cardCode, cardName, companyCode, companyName, row("OutletCode").ToString(), row("OutletName").ToString(), _
                                                            Double.Parse(row("MinAmt").ToString()), _
                                                            Double.Parse(row("MaxAmt").ToString()), isUpdate, databaseName, row("ID").ToString())
            Next
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Return ex.Message
        End Try
        Return String.Empty
    End Function
    Public Function ExcuteOutletOrderAmount(ByVal userId As String, ByVal passWord As String, _
                                        ByVal cardCode As String, ByVal cardName As String, _
                                        ByVal companyCode As String, ByVal companyName As String, _
                                        ByVal outletCode As String, ByVal outletName As String, _
                                        ByVal minAmt As Double, ByVal maxAmt As Double, _
                                        ByVal isUpdate As String, ByVal databaseName As String, ByVal ID As String) As String
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                Dim params = New Object() {cardCode, cardName, companyCode, companyName, outletCode, outletName, minAmt, maxAmt}
                Dim query As String = String.Empty
                If isUpdate = "0" Or ID = "" Then
                    query = "IF NOT EXISTS (SELECT * FROM Outlet_OrderAmount WHERE CardCode ='" + cardCode + "' AND CompanyCode='" + companyCode + "' AND OutletCode ='" & outletCode & "') BEGIN"
                    query += " INSERT INTO Outlet_OrderAmount(CardCode,CardName,CompanyCode,CompanyName,OutletCode,OutletName,MinAmt,MaxAmt)"
                    query += " VALUES(@Param1,@Param2,@Param3,@Param4,@Param5,@Param6,@Param7,@Param8)"
                    query += " END"
                Else
                    params = New Object() {minAmt, maxAmt, outletCode, cardCode, companyCode}
                    query = "UPDATE Outlet_OrderAmount SET MinAmt =@Param1,MaxAmt=@Param2"
                    query += " WHERE OutletCode = @Param3 AND CardCode = @Param4 AND CompanyCode = @Param5"
                End If
                Dim count As Integer = connect.Object_Execute_SAP(query, params)
                Return count
            End If
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
        End Try
        Return 0
    End Function

    <WebMethod()> _
    Public Function UpdateOutletItemList(ByVal userId As String, ByVal passWord As String, ByVal Id As Integer, _
                                          ByVal cardCode As String, ByVal cardName As String, _
                                         ByVal companyCode As String, ByVal companyName As String, _
                                         ByVal data As DataSet, ByVal isUpdate As String, ByVal databaseName As String) As String
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length > 0 Then
                Return errMsg
            End If
            'Delete Data
            If data.Tables.Contains("Delete") Then
                For Each row As DataRow In data.Tables("Delete").Rows
                    Dim params = New Object() {row("Delete")}
                    Dim query = "DELETE Item_Supplier WHERE ID=@Param1"
                    Dim count As Integer = connect.Object_Execute_SAP(query, params)
                    If count > 0 Then
                        If data.Tables.Contains(row("ItemCode").ToString()) Then
                            data.Tables.Remove(row("ItemCode").ToString())
                        End If
                        query = "DELETE FROM Item_Outlet WHERE HeaderID = @Param1"
                        connect.Object_Execute_SAP(query, params)
                    End If
                Next
            End If
            'Excute Data
            For Each row As DataRow In data.Tables("Item_Supplier").Rows
                Dim querycheck = "SELECT * "
                querycheck += " FROM  Item_Supplier "
                querycheck += " WHERE CardCode = @Param1 AND CompanyCode = @Param2 AND ItemCode = @Param3"
                Dim dsCheck As DataSet = connect.ObjectGetAll_Query_SAP(querycheck, New Object() {cardCode, companyCode, row("ItemCode")})
                Dim params = New Object() {cardCode, cardName, companyCode, companyName, row("ItemCode"), row("Dscription"), row("LB")}
                Dim query As String = String.Empty
                If row("ID").ToString() = "0" Then
                    If dsCheck IsNot Nothing And dsCheck.Tables.Count > 0 And dsCheck.Tables(0).Rows.Count = 0 Then
                        'Insert Master
                        query = "INSERT INTO Item_Supplier(CardCode,CardName,CompanyCode,CompanyName,ItemCode,ItemName,LB)"
                        query += " VALUES(@Param1,@Param2,@Param3,@Param4,@Param5,@Param6,@Param7)"
                        Dim count As Integer = connect.Object_Execute_SAP(query, params)
                        'Get Max Id
                        Dim ds As DataSet = connect.ObjectGetAll_Query_SAP("SELECT IDENT_CURRENT('Item_Supplier')")
                        'Insert Detail
                        If ds IsNot Nothing And ds.Tables.Count > 0 Then
                            Dim tbDetail As DataTable = data.Tables(row("ItemCode"))
                            If tbDetail IsNot Nothing Then
                                For Each rowDetail As DataRow In tbDetail.Rows
                                    Dim isExcute As String
                                    isExcute = rowDetail("isCheck").ToString()
                                    Dim masterId As Integer = Integer.Parse(ds.Tables(0).Rows(0)(0).ToString())
                                    Dim headerId As Integer = 0
                                    errMsg = UpdateOutletItemListDetail(userId, passWord, companyCode, masterId, headerId, _
                                                              rowDetail("OutletCode"), rowDetail("OutletName"), _
                                                              Decimal.Parse(rowDetail("MinQty").ToString()), Decimal.Parse(rowDetail("MaxQty").ToString()), _
                                                              Decimal.Parse(rowDetail("MinAmt").ToString()), Decimal.Parse(rowDetail("MaxAmt").ToString()), isExcute, databaseName)
                                    If errMsg.Length > 0 Then
                                        Return errMsg
                                    End If
                                Next
                            End If
                        End If
                    End If
                Else
                    'Update Master
                    query = "UPDATE Item_Supplier SET LB = @Param1"
                    query += " WHERE ID = @Param2"
                    connect.Object_Execute_SAP(query, New Object() {row("LB"), row("ID")})
                    Dim tbDetail As DataTable = data.Tables(row("ItemCode"))
                    If tbDetail IsNot Nothing Then
                        For Each rowDetail As DataRow In tbDetail.Rows
                            Dim isExcute As String
                            isExcute = rowDetail("isCheck").ToString()
                            Dim headerId As Integer = Integer.Parse(rowDetail("HeaderID").ToString())
                            Dim masterId As Integer = Integer.Parse(row("ID").ToString())
                            errMsg = UpdateOutletItemListDetail(userId, passWord, companyCode, masterId, headerId, _
                                                                rowDetail("OutletCode"), rowDetail("OutletName"), _
                                                                Decimal.Parse(rowDetail("MinQty").ToString()), Decimal.Parse(rowDetail("MaxQty").ToString()), _
                                                                Decimal.Parse(rowDetail("MinAmt").ToString()), Decimal.Parse(rowDetail("MaxAmt").ToString()), isExcute, databaseName)
                            If errMsg.Length > 0 Then
                                Return errMsg
                            End If
                        Next
                    End If
                End If
            Next
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Return "Server Error - " & ex.Message
        End Try
        Return String.Empty
    End Function
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
    Public Function UpdateOutletItemListDetail(ByVal userId As String, ByVal passWord As String, ByVal companyCode As String, ByVal Id As Integer, ByVal headerId As Integer, _
                                          ByVal outletCode As String, ByVal outletName As String, _
                                         ByVal MinQty As Double, ByVal MaxQty As Double, _
                                         ByVal MinAmt As Double, ByVal MaxAmt As Double, ByVal isExcute As String, ByVal databaseName As String) As String
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                Dim query As String = String.Empty
                If isExcute = "1" Then
                    If headerId = "0" Then
                        Dim params = New Object() {Id, outletCode, outletName, MinQty, MaxQty, MinAmt, MaxAmt}
                        query = "INSERT INTO Item_Outlet(HeaderID,OutletCode,OutletName,MinQty,MaxQty,MinAmt,MaxAmt)"
                        query += " VALUES(@Param1,@Param2,@Param3,@Param4,@Param5,@Param6,@Param7)"
                        connect.Object_Execute_SAP(query, params)
                    Else
                        Dim params = New Object() {MinQty, MaxQty, MinAmt, MaxAmt, headerId, outletCode}
                        query = "UPDATE Item_Outlet SET MinQty = @Param1 ,MaxQty = @Param2,MinAmt = @Param3,MaxAmt = @Param4"
                        query += " WHERE HeaderID =@Param5 AND OutletCode =@Param6 "
                        connect.Object_Execute_SAP(query, params)
                    End If
                ElseIf isExcute = "0" Then
                    Dim params = New Object() {headerId, outletCode}
                    query = "DELETE FROM Item_Outlet WHERE HeaderID = @Param1 AND OutletCode = @Param2"
                    connect.Object_Execute_SAP(query, params)
                End If
            Else
                Return errMsg
            End If
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Return "Server Error - " & ex.Message
        End Try
        Return String.Empty
    End Function
    <WebMethod()> _
    Public Function InsertABDeliveryOrder(ByVal userId As String, ByVal passWord As String, _
                                        ByVal databaseName As String, ByVal data As DataSet) As String
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim connect As New Connection()
            For Each row As DataRow In data.Tables("AB_DeliveryOrder").Rows
                Dim params = New Object() {row("DocEntry"), row("LineNum"), row("DONo"), row("SONo"), row("ItemCode"), _
                                           row("Dscription"), row("Quantity"), row("RecQty"), row("UnitMsr")}
                Dim query As String = String.Empty
                query = "INSERT INTO AB_DeliveryOrder(DocEntry,LineNum,DONo,SONo,ItemCode,Dscription,Quantity,RecQty,UnitMsr)"
                query += " VALUES(@Param1,@Param2,@Param3,@Param4,@Param5,@Param6,@Param7,@Param8,@Param9)"
                Dim errMsg As String = connect.setSQLDB(databaseName)
                If errMsg.Length = 0 Then
                    Dim count As Integer = connect.Object_Execute_SAP(query, params)
                Else
                    Return errMsg
                End If
            Next
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Return "Server Error - " & ex.Message
        End Try
        Return String.Empty
    End Function
    <WebMethod()> _
    Public Function DeleteVendorSetup(ByVal companyCode As String, ByVal vendorCode As String) As String
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                Dim query As String = String.Empty
                Dim params = New Object() {vendorCode, companyCode}
                Dim dsSuplier As DataSet = GetOutletSupplier("", "", vendorCode, companyCode, companyCode)
                query = "DELETE FROM Item_Supplier WHERE CardCode = @Param1 AND CompanyCode = @Param2"
                Dim count As Integer = connect.Object_Execute_SAP(query, params)
                If count > 0 Then
                    Dim arr As String = String.Empty
                    If dsSuplier IsNot Nothing And dsSuplier.Tables.Count > 0 Then
                        For Each r As DataRow In dsSuplier.Tables(0).Rows
                            arr = arr + r("ID").ToString() + ","
                        Next
                    End If
                    arr = arr.Substring(0, arr.Length - 1)
                    query = "DELETE FROM Item_Outlet WHERE HeaderID In (" + arr + ")"
                    count = connect.Object_Execute_SAP(query)
                Else
                    Return "Can't delete"
                End If
            Else
                Return errMsg
            End If
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Return "Server Error - " & ex.Message
        End Try
        Return String.Empty
    End Function
    <WebMethod()> _
    Public Function GetPOByDocEntry(ByVal userId As String, ByVal passWord As String, _
                                       ByVal databaseName As String, ByVal docEntry As Integer) As DataSet
        Try
            Dim connect As New Connection()
            Dim errMsg As String = connect.setSQLDB(databaseName)
            If errMsg.Length = 0 Then
                Dim query As String = String.Empty
                query = "SELECT * FROM POR1 "
                query += " WHERE DocEntry = @Param1"
                Return connect.ObjectGetAll_Query_SAP(query, New Object() {docEntry})
            End If
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
        End Try
        Return Nothing
    End Function
    <WebMethod()> _
    Public Function GetRouteHolding() As DataSet
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim dt As New DataSet("CRD1")
            connect.setHoldingDB(oCompany)
            Dim query As String = "SELECT  Block,Address  FROM CRD1 WHERE Block IS NOT NULL GROUP BY Block,Address"
            dt = connect.ObjectGetAll_Query_SAP(query)
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetDriverNo(ByVal route As String, ByVal truckCategory As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim dt As New DataSet("CRD1")
            connect.setHoldingDB(oCompany)
            Dim query As String = "SELECT  T1.* FROM [@AB_DRIVERH] T0 JOIN [@AB_DRIVERD] T1 ON T0.Code = T1.Code WHERE T0.U_Route = @Param1 AND T1.U_AB_TruckCat = @Param2"
            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {route, truckCategory})
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function GetBlock(ByVal userId As String, ByVal passWord As String, _
                                       ByVal databaseName As String, ByVal cardCode As String, ByVal address As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("CRD1")
            connect.setSQLDB(databaseName)
            Dim query As String = "SELECT T1.Block FROM OCRD T0 JOIN CRD1 T1 ON T0.CardCode = T1.CardCode WHERE T0.CardType = 'C' AND T1.AdresType = 'S' AND T0.CardCode = @Param1 AND T1.Address = @Param2"
            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {cardCode, address})
            Return dt
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Throw ex
        End Try
    End Function
#End Region
#Region "Admin"
    <WebMethod()> _
    Public Function GetSupplierSetupList(ByVal companyCode As String) As DataSet
        Try
            Dim oCompany As SAPbobsCOM.Company
            Dim connect As New Connection()
            Dim dt As New DataSet("Item_Supplier")
            connect.setHoldingDB(oCompany)
            'Edit By Hieu 12/8/2014 - START
            'Dim query As String = "SELECT DISTINCT CardCode,CardName FROM Item_Supplier  WHERE CompanyCode = @Param1 "
            Dim query As String = "SELECT DISTINCT T0.CardCode,T1.CardName FROM Item_Supplier T0 INNER JOIN OCRD T1 ON T0.CardCode = T1.CardCode  WHERE CompanyCode = @Param1 "
            'Edit By Hieu 12/8/2014 - END
            dt = connect.ObjectGetAll_Query_SAP(query, New Object() {companyCode})
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function ChangePassword(ByVal userID As String, ByVal passWord As String) As String
        Try
            Dim oCompany As SAPbobsCOM.Company
            Dim connect As New Connection()
            Dim dt As New DataSet("AB_ONLINEUSER")
            connect.setHoldingDB(oCompany)
            Dim params = New Object() {passWord, userID}
            Dim Query As String = "UPDATE [@AB_ONLINEUSER] SET U_PWD = @Param1"
            Query += " WHERE Code = @Param2"
            Dim count As Integer = connect.Object_Execute_SAP(Query, params)
            Return count.ToString()
        Catch ex As Exception
            Throw ex
        End Try
        Return String.Empty
    End Function
#End Region
#Region "Report"
    <WebMethod()> _
    Public Function ReportStockTakeList(ByVal outlet As String, ByVal companyCode As String) As DataSet
        Try
            Dim oCompany As SAPbobsCOM.Company
            Dim connect As New Connection()
            Dim dt As New DataSet("Item_Supplier")
            connect.setHoldingDB(oCompany)
            Dim params As Dictionary(Of String, String) = New Dictionary(Of String, String)
            params.Add("@OutletID", outlet)
            params.Add("@CompanyCode", companyCode)
            Dim storeName As String = "sp_AB_RPT_StockTakeList"
            dt = connect.ObjectGetAll_Stored_SAP(storeName, params)
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    <WebMethod()> _
    Public Function ReportPurchaseOrder(ByVal docEntry As String, ByVal companyCode As String) As DataSet
        Try
            Dim connect As New Connection()
            Dim dt As New DataSet("Item_Supplier")
            connect.setSQLDB(companyCode)
            Dim params As Dictionary(Of String, String) = New Dictionary(Of String, String)
            params.Add("@DocKey", docEntry)
            Dim storeName As String = "sp_AB_FRM_PurchaseOrder"
            dt = connect.ObjectGetAll_Stored_SAP(storeName, params)
            Return dt
        Catch ex As Exception
            Throw ex
        End Try
    End Function
#End Region
#Region "Copy Template"
    <WebMethod()> _
    Public Function CopyTemplate(ByVal fromDB As String,
                                 ByVal toDB As String,
                                  ByVal toDBName As String,
                                 ByVal dsOutlet As DataSet) As String
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim count As Integer
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            For Each rowOutLet As DataRow In dsOutlet.Tables(0).Rows
                'Insert Outlet Amount
                Dim outletAmount As String = "IF NOT EXISTS (SELECT * FROM Outlet_OrderAmount WHERE OutletCode ='" + rowOutLet("NewOutletCode") + "' AND CompanyCode='" + toDB + "') BEGIN"
                outletAmount += " INSERT INTO Outlet_OrderAmount"
                outletAmount += " (CardCode,CardName,CompanyCode,CompanyName,OutletCode,OutletName,MinAmt,MaxAmt)"
                outletAmount += " SELECT CardCode,CardName,'" + toDB + "' AS CompanyCode,'" + toDBName + "' AS CompanyName,"
                outletAmount += " '" + rowOutLet("NewOutletCode") + "' AS OutletCode,OutletName ,MinAmt,MaxAmt"
                outletAmount += " FROM Outlet_OrderAmount "
                outletAmount += " WHERE OutletCode ='" + rowOutLet("OutletCode") + "' AND CompanyCode='" + fromDB + "' END"

                count = connect.Object_Execute_SAP(outletAmount)

                'Insert Outlet Calendar
                Dim outletCalendar As String = "IF NOT EXISTS (SELECT * FROM Outlet_Calendar WHERE OutletCode ='" + rowOutLet("NewOutletCode") + "' AND CompanyCode='" + toDB + "') BEGIN"
                outletCalendar += " INSERT INTO Outlet_Calendar"
                outletCalendar += " (CardCode,CardName,CompanyCode,CompanyName,OutletCode,OutletName,Mon,Tue,Wed,Thu,Fri,Sat,Sun)"
                outletCalendar += " SELECT CardCode,CardName,'" + toDB + "' AS CompanyCode,'" + toDBName + "' AS CompanyName,"
                outletCalendar += " '" + rowOutLet("NewOutletCode") + "' AS OutletCode,OutletName,Mon,Tue,Wed,Thu,Fri,Sat,Sun"
                outletCalendar += " FROM Outlet_Calendar"
                outletCalendar += "  WHERE OutletCode ='" + rowOutLet("OutletCode") + "' AND CompanyCode='" + fromDB + "' END"

                count = connect.Object_Execute_SAP(outletCalendar)

                'Get Item Oulet
                Dim Item_Supplier As String = "SELECT ID,CardCode, CardName, CompanyCode, CompanyName,ItemCode,ItemName,LB"
                Item_Supplier += " FROM Item_Supplier"
                Item_Supplier += " WHERE CompanyCode = '" + fromDB + "'"

                Dim dataset As DataSet = connect.ObjectGetAll_Query_SAP(Item_Supplier)

                If dataset IsNot Nothing Then
                    For Each r As DataRow In dataset.Tables(0).Rows
                        Dim maxID As String = "0"
                        'Check Exist CardCode
                        Dim queryCheck As String = "SELECT CardCode FROM Item_Supplier WHERE CardCode='" + r("CardCode").ToString() + "' "
                        queryCheck += "AND CompanyCode ='" + toDB + "'  "
                        queryCheck += "	AND ItemCode = '" + r("ItemCode").ToString() + "' "
                        Dim queryMaxID As String
                        Dim dsCheck As DataSet = connect.ObjectGetAll_Query_SAP(queryCheck)
                        If dsCheck IsNot Nothing AndAlso dsCheck.Tables.Count > 0 AndAlso dsCheck.Tables(0).Rows.Count = 0 Then
                            Dim sqlInsertItemSupplier As String = " INSERT INTO Item_Supplier "
                            sqlInsertItemSupplier += " (CardCode, CardName, CompanyCode, CompanyName,ItemCode,ItemName,LB)"
                            sqlInsertItemSupplier += " VALUES (@Param1, @Param2,@Param3,@Param2,@Param5,@Param6,@Param7)"

                            count = connect.Object_Execute_SAP(sqlInsertItemSupplier, New Object() {r("CardCode"), r("CardName"), toDB,
                                                                                                   toDBName, r("ItemCode"), r("ItemName"), r("LB")})

                            queryMaxID = " (SELECT MAX(ID) AS ID FROM Item_Supplier) "
                        Else
                            queryMaxID = " (SELECT ID FROM Item_Supplier "
                            queryMaxID += " WHERE CardCode='" + r("CardCode") + "' AND CompanyCode ='" + toDB + "' AND ItemCode = '" + r("ItemCode").ToString() + "')"
                        End If
                        'Get MAX ID
                        Dim dsMaxID As DataSet = connect.ObjectGetAll_Query_SAP(queryMaxID)
                        If dsMaxID IsNot Nothing AndAlso dsMaxID.Tables.Count > 0 AndAlso dsMaxID.Tables(0).Rows.Count > 0 Then
                            maxID = dsMaxID.Tables(0).Rows(0)("ID").ToString()
                        End If
                        'Get Outlet Item
                        Dim queryOutletItem As String = "SELECT * "
                        queryOutletItem += " FROM Item_Outlet"
                        queryOutletItem += "  WHERE HeaderID =" & Integer.Parse(r("ID").ToString()) & " AND OutletCode ='" + rowOutLet("OutletCode") + "'"

                        Dim dsItemOutlet As DataSet = connect.ObjectGetAll_Query_SAP(queryOutletItem)

                        If dsItemOutlet IsNot Nothing AndAlso dsItemOutlet.Tables.Count > 0 Then
                            Dim sqlItemOutlet As String = String.Empty
                            For Each row As DataRow In dsItemOutlet.Tables(0).Rows
                                sqlItemOutlet += " IF NOT EXISTS (SELECT OutletCode FROM Item_Outlet JOIN Item_Supplier ON Item_Outlet.HeaderID = Item_Supplier.ID"
                                sqlItemOutlet += " WHERE OutletCode='" + rowOutLet("NewOutletCode") + "'"
                                sqlItemOutlet += " AND CompanyCode ='" + toDB + "'"
                                sqlItemOutlet += " AND ItemCode='" + r("ItemCode") + "'"
                                sqlItemOutlet += " AND CardCode ='" + r("CardCode") + "') BEGIN "
                                sqlItemOutlet += " INSERT INTO Item_Outlet"
                                sqlItemOutlet += " (HeaderID, OutletCode,OutletName,MinQty,MaxQty,MinAmt,MaxAmt) "
                                sqlItemOutlet += " VALUES(" & Integer.Parse(maxID) & ",'" + rowOutLet("NewOutletCode") + "','" + row("OutletName") + "'," & row("MinQty") & "," & row("MaxQty") & "," & row("MinAmt") & "," & row("MaxAmt") & ")"
                                sqlItemOutlet += " END"
                            Next
                            If sqlItemOutlet.Length > 0 Then
                                count = connect.Object_Execute_SAP(sqlItemOutlet)
                            End If
                        End If
                    Next
                End If
            Next
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
            Return ex.Message
        End Try
        Return String.Empty
    End Function
    <WebMethod()> _
    Public Function UpdateXMLLog(ByVal UserCode As String, ByVal passWord As String, _
                                       ByVal Company As String, ByVal ObjType As String, _
                                       ByVal XML As String, ByVal ErrMessage As String, _
                                       ByVal DateTime As Date, ByVal isUpdate As String, ByVal key As String) As String
        Try
            Dim connect As New Connection()
            Dim oCompany As SAPbobsCOM.Company
            Dim errMsg As String = connect.setHoldingDB(oCompany)
            If errMsg.Length = 0 Then
                Dim params = New Object() {UserCode, passWord, Company, ObjType, XML, ErrMessage, DateTime, isUpdate, key}
                Dim query As String = String.Empty
                query = "INSERT INTO AB_XML_Log(UserCode,Password,Company,ObjType,XML,ErrMessage,PostTime,IsUpdate,DocEntry)"
                query += " VALUES(@Param1,@Param2,@Param3,@Param4,@Param5,@Param6,@Param7,@Param8,@Param9)"
                Dim count As Integer = connect.Object_Execute_SAP(query, params)
                Return count
            End If
        Catch ex As Exception
            WriteLog(ex.StackTrace & ":" & ex.Message)
        End Try
        Return 0
    End Function
#End Region

#Region "EmailCheck"

    <WebMethod()> _
    Public Function InsertPODetailsforEmailCheck(ByVal poNumber As String, ByVal CardCode As String, ByVal CardName As String, ByVal SupplierType As String, ByVal EmailId As String) As String
        Try
            Dim oCompany As SAPbobsCOM.Company
            Dim connect As New Connection()
            connect.setHoldingDB(oCompany)
            Dim Query As String = "INSERT INTO [AB_EmailLog] (PONumber, CardCode, CardName, SupplierType, EmailId) VALUES ('" & poNumber & "', '" & CardCode & "', '" & CardName & "', '" & SupplierType & "', '" & EmailId & "'); SELECT SCOPE_IDENTITY();"
            Dim count As String = connect.Object_ExecuteScalar_EmailIssue(Query, "Insert")
            Return count.ToString()
        Catch ex As Exception
            'Throw "Error" & ex.Message
            Return "InsertError " & ex.Message
        End Try

    End Function

    <WebMethod()> _
    Public Function UpdateStatusforEmailCheck(ByVal poNumber As String, ByVal status As String, ByVal errMsg As String, ByVal id As String) As String
        Try
            Dim oCompany As SAPbobsCOM.Company
            Dim connect As New Connection()
            connect.setHoldingDB(oCompany)
            Dim Query As String = "UPDATE [AB_EmailLog] SET Status = '" & status & "' , ErrorMessage = '" & errMsg & "' where PONumber = '" & poNumber & "' and Id = '" & id & "'"
            Dim count As Integer = connect.Object_ExecuteScalar_EmailIssue(Query, "Update")
            Return count.ToString()
        Catch ex As Exception
            Return "UpdateError" & ex.Message
        End Try

    End Function

    <WebMethod()> _
    Public Function SelectMaxIdofPO(ByVal poNumber As String) As String
        Try
            Dim oCompany As SAPbobsCOM.Company
            Dim connect As New Connection()
            Dim dt As New DataSet("log")
            connect.setHoldingDB(oCompany)
            Dim Query As String = "SELECT IsNull(MAX(Id),0) as Id FROM [AB_EmailLog] where PONumber = '" & poNumber & "'"
            dt = connect.ObjectGetAll_Query_SAP(Query) ' for selecting the Id
            Return dt.Tables(0).Rows(0)("Id").ToString()
        Catch ex As Exception
            Throw ex
        End Try
        Return String.Empty
    End Function

#End Region

End Class