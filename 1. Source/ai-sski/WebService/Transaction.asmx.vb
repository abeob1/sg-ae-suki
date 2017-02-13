Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Transaction
    Inherits System.Web.Services.WebService

    Dim lRetCode As Integer
    Dim lErrCode As Integer
    Dim sErrMsg As String
    <WebMethod()> _
    Public Function RemoveMarketingDocumentLine(ByVal dsUpdate As DataSet, ByVal userId As String, ByVal passWord As String, _
                                                ByVal databaseName As String, ByVal DocType As String, ByVal Key As String) As DataSet
        Dim b As New SAP_Functions
        Dim connect As New Connection()
        Dim oCompany As SAPbobsCOM.Company
        Dim obj As MasterData = New MasterData()
        Dim strIsUpdate As String = "1"

        Try
            Dim sStr As String = "Operation Completed successfuly!"
            Dim oDocment
            Select Case DocType
                Case "30"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.JournalEntries)
                Case "97"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.SalesOpportunities)
                Case "191"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.ServiceCalls)
                Case "33"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Contacts)
                Case "221"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Attachments2)
                Case "2"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.BusinessPartners)
                Case "28"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.IJournalVouchers)
                Case Else
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Documents)
            End Select

            Dim constr As String

            Dim dt As New DataSet("Outlet")
            constr = connect.setHoldingDB(oCompany)
            Dim conStrCompany As String = String.Empty
            If constr.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_CompanySetup] WHERE U_DBName = @Param1", New Object() {databaseName})
                If dt IsNot Nothing And dt.Tables.Count > 0 And dt.Tables(0).Rows.Count > 0 Then
                    If dt.Tables(0).Rows(0)("U_ConnString") IsNot Nothing Then
                        conStrCompany = dt.Tables(0).Rows(0)("U_ConnString")
                        oCompany = Nothing
                        constr = connect.ConnectDB(dt.Tables(0).Rows(0)("U_ConnString"), oCompany)
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
            oDocment = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders)
            Dim index = 0
            If oDocment.GetByKey(Key) Then
                For Each row As DataRow In dsUpdate.Tables(0).Rows
                    oDocment.Lines.SetCurrentLine(Integer.Parse(row("LineNum").ToString()) - index)
                    oDocment.Lines.Delete()
                    index += 1
                Next
                lErrCode = oDocment.Update()
            Else
                Return b.ReturnMessage(-1, "Record not found!")
            End If

            If lErrCode <> 0 Then
                oCompany.GetLastError(lErrCode, sErrMsg)
                Return b.ReturnMessage(lErrCode, sErrMsg)
            Else
                Return b.ReturnMessage(lErrCode, "Operation successful!" + "-" + oCompany.GetNewObjectKey)
            End If
        Catch ex As Exception
            Return b.ReturnMessage(-1, ex.ToString & ex.StackTrace)
        Finally
            If oCompany.Connected Then
                oCompany.Disconnect()
            End If
        End Try
    End Function
    <WebMethod()> _
    Public Function CreateMarketingDocument(ByVal strXml As String, ByVal userId As String, ByVal passWord As String, ByVal databaseName As String, ByVal DocType As String, ByVal Key As String, ByVal IsUpdate As Boolean) As DataSet
        Dim b As New SAP_Functions
        Dim connect As New Connection()
        Dim oCompany As SAPbobsCOM.Company
        Dim obj As MasterData = New MasterData()
        Dim strIsUpdate As String = String.Empty

        Try
            Dim sStr As String = "Operation Completed successfuly!"
            Dim oDocment
            Select Case DocType
                Case "30"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.JournalEntries)
                Case "97"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.SalesOpportunities)
                Case "191"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.ServiceCalls)
                Case "33"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Contacts)
                Case "221"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Attachments2)
                Case "2"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.BusinessPartners)
                Case "28"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.IJournalVouchers)
                Case Else
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Documents)
            End Select

            Dim constr As String

            Dim dt As New DataSet("Outlet")
            constr = connect.setHoldingDB(oCompany)
            Dim conStrCompany As String = String.Empty
            If constr.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_CompanySetup] WHERE U_DBName = @Param1", New Object() {databaseName})
                If dt IsNot Nothing And dt.Tables.Count > 0 And dt.Tables(0).Rows.Count > 0 Then
                    If dt.Tables(0).Rows(0)("U_ConnString") IsNot Nothing Then
                        conStrCompany = dt.Tables(0).Rows(0)("U_ConnString")
                        oCompany = Nothing
                        constr = connect.ConnectDB(dt.Tables(0).Rows(0)("U_ConnString"), oCompany)
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

            oCompany.XMLAsString = True
            oDocment = oCompany.GetBusinessObjectFromXML(strXml, 0)
            If IsUpdate Then
                If oDocment.GetByKey(Key) Then
                    oDocment.Browser.ReadXML(strXml, 0)
                    lErrCode = oDocment.Update()
                Else
                    obj.UpdateXMLLog(userId, passWord, databaseName, DocType, strXml, sErrMsg, DateTime.Now, "Record not found!", Key)
                    Return b.ReturnMessage(-1, "Record not found!")
                End If
            Else
                lErrCode = oDocment.Add()
            End If

            If IsUpdate Then
                strIsUpdate = "1"
            Else
                strIsUpdate = "0"
            End If
            If lErrCode <> 0 Then
                oCompany.GetLastError(lErrCode, sErrMsg)
                obj.UpdateXMLLog(userId, passWord, databaseName, DocType, strXml, sErrMsg, DateTime.Now, strIsUpdate, Key)
                Return b.ReturnMessage(lErrCode, sErrMsg)
            Else
                If DocType = 17 Then
                    If oDocment.GetByKey(oCompany.GetNewObjectKey) Then
                        oDocment.UserFields.Fields.Item("U_AE_IFlag").Value = "Y"
                        oDocment.Update()
                    End If
                End If

                obj.UpdateXMLLog(userId, passWord, databaseName, DocType, strXml, "Operation successful!" + "-" + oCompany.GetNewObjectKey, DateTime.Now, strIsUpdate, Key)
                Return b.ReturnMessage(lErrCode, "Operation successful!" + "-" + oCompany.GetNewObjectKey)
            End If
        Catch ex As Exception
            obj.UpdateXMLLog(userId, passWord, databaseName, DocType, strXml, ex.Message, DateTime.Now, strIsUpdate, Key)
            Return b.ReturnMessage(-1, ex.ToString)
        Finally
            If oCompany.Connected Then
                oCompany.Disconnect()
            End If
        End Try
    End Function
    <WebMethod()> _
    Public Function ClosePODraft(docEntry As Integer, ByVal databaseName As String) As String
        Dim b As New SAP_Functions
        Dim connect As New Connection()
        Dim oCompany As SAPbobsCOM.Company
        Try
            Dim constr As String
            Dim dt As New DataSet("Outlet")
            constr = connect.setHoldingDB(oCompany)
            Dim conStrCompany As String = String.Empty
            If constr.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_CompanySetup] WHERE U_DBName = @Param1", New Object() {databaseName})
                If dt IsNot Nothing And dt.Tables.Count > 0 And dt.Tables(0).Rows.Count > 0 Then
                    If dt.Tables(0).Rows(0)("U_ConnString") IsNot Nothing Then
                        conStrCompany = dt.Tables(0).Rows(0)("U_ConnString")
                        constr = connect.ConnectDB(dt.Tables(0).Rows(0)("U_ConnString"), oCompany)
                    Else
                        Return "Can't connect database."
                    End If
                Else
                    Return "Can't connect database."
                End If
            End If
            If constr.Length = 0 Then
                Dim oDraft As SAPbobsCOM.Documents
                oDraft = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)
                Dim errcode As Integer
                If oDraft.GetByKey(docEntry) Then
                    errcode = oDraft.Cancel()
                    If errcode <> 0 Then
                        oCompany.GetLastError(errcode, constr)
                    End If
                End If
            End If
            Return constr
        Catch ex As Exception
        Finally
            If oCompany.Connected Then
                oCompany.Disconnect()
            End If
        End Try
    End Function
    <WebMethod()> _
    Public Function CloseCancelPO(docEntry As Integer, ByVal databaseName As String, ByVal isCancel As Boolean, ByVal isDraft As Boolean) As String
        Dim b As New SAP_Functions
        Dim connect As New Connection()
        Dim oCompany As SAPbobsCOM.Company
        Try
        Dim constr As String
        Dim dt As New DataSet("Outlet")
            constr = connect.setHoldingDB(oCompany)
        Dim conStrCompany As String = String.Empty
        If constr.Length = 0 Then
            dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_CompanySetup] WHERE U_DBName = @Param1", New Object() {databaseName})
            If dt IsNot Nothing And dt.Tables.Count > 0 And dt.Tables(0).Rows.Count > 0 Then
                If dt.Tables(0).Rows(0)("U_ConnString") IsNot Nothing Then
                    conStrCompany = dt.Tables(0).Rows(0)("U_ConnString")
                    constr = connect.ConnectDB(dt.Tables(0).Rows(0)("U_ConnString"), oCompany)
                Else
                    Return "Can't connect database."
                End If
            Else
                Return "Can't connect database."
            End If
        End If
        If constr.Length = 0 Then
            Dim oPO As SAPbobsCOM.Documents
                If isDraft = False Then
                    oPO = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders)
                Else
                    oPO = oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)
                End If
                Dim errcode As Integer
                If oPO.GetByKey(docEntry) Then
                    If isCancel Then
                        errcode = oPO.Cancel()
                    Else
                        errcode = oPO.Close()
                    End If
                    If errcode <> 0 Then
                        oCompany.GetLastError(errcode, constr)
                    End If
                End If
        End If
            Return constr
        Catch ex As Exception
        Finally
            If oCompany.Connected Then
                oCompany.Disconnect()
            End If
        End Try
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
    <WebMethod()> _
    Public Function CreateMarketingDocumentDynamic(ByVal strXml As String, _
                                   ByVal DatabaseName As String, ByVal SAPUser As String, ByVal SAPPwd As String, _
                                   ByVal SQLServer As String, ByVal SQLUser As String, ByVal SQLPwd As String, _
                                   ByVal SAPServer As String, ByVal ServerType As String, ByVal DocType As String, ByVal Key As String, ByVal IsUpdate As Boolean) As DataSet
        Dim b As New SAP_Functions
        Dim connect As New Connection()
        Dim oCompany As SAPbobsCOM.Company
        Dim sFunName As String = String.Empty

        Try
            sFunName = "CreateMarketingDocumentDynamic()"
            WriteLog("Calling Function CreateMarketingDocumentDynamic()" & sFunName)
            Dim sStr As String = "Operation Completed successfuly!"
            Dim oDocment
            Select Case DocType
                Case "30"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.JournalEntries)
                Case "97"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.SalesOpportunities)
                Case "191"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.ServiceCalls)
                Case "33"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Contacts)
                Case "221"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Attachments2)
                Case "2"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.BusinessPartners)
                Case "28"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.IJournalVouchers)
                Case Else
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Documents)
            End Select

            Dim constr As String
            constr = connect.connectDynamicSAPDB(DatabaseName, SAPUser, SAPPwd, SQLServer, SQLUser, SQLPwd, SAPServer, ServerType, oCompany)
            If constr <> "" Then
                Return b.ReturnMessage(-1, constr)
            End If
            oCompany.XMLAsString = True
            oDocment = oCompany.GetBusinessObjectFromXML(strXml, 0)
            If IsUpdate Then
                WriteLog("Start Updating the XML " & sFunName)
                If oDocment.GetByKey(Key) Then
                    oDocment.Browser.ReadXML(strXml, 0)
                    lErrCode = oDocment.Update()
                Else
                    Return b.ReturnMessage(-1, "Record not found!")
                End If
            Else
                WriteLog("Start Adding the XML " & sFunName)
                lErrCode = oDocment.Add()
            End If

            If lErrCode <> 0 Then
                oCompany.GetLastError(lErrCode, sErrMsg)
                WriteLog("Completed with ERROR  " & lErrCode & " - " & sErrMsg & " " & sFunName)
                Return b.ReturnMessage(lErrCode, sErrMsg)
            Else
                WriteLog("Completed with SUCCESS  " & sFunName)
                Return b.ReturnMessage(lErrCode, "Operation successful!")
            End If
        Catch ex As Exception
            Return b.ReturnMessage(-1, ex.ToString)
        Finally
            If oCompany.Connected Then
                oCompany.Disconnect()
            End If
        End Try
    End Function
    <WebMethod()> _
    Public Function CreateMarketingDocumentDebug(ByVal strXml As String, ByVal databaseName As String, _
                                                 ByVal DocType As String, ByVal Key As String, ByVal IsUpdate As Boolean) As DataSet
        Dim b As New SAP_Functions
        Dim connect As New Connection()
        Dim oCompany As SAPbobsCOM.Company

        Try
            Dim sStr As String = "Operation Completed successfuly!"
            Dim oDocment
            Select Case DocType
                Case "30"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.JournalEntries)
                Case "97"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.SalesOpportunities)
                Case "191"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.ServiceCalls)
                Case "33"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Contacts)
                Case "221"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Attachments2)
                Case "2"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.BusinessPartners)
                Case "28"
                    oDocment = DirectCast(oDocment, SAPbobsCOM.IJournalVouchers)
                Case Else
                    oDocment = DirectCast(oDocment, SAPbobsCOM.Documents)
            End Select

            Dim constr As String

            Dim dt As New DataSet("Outlet")
            constr = connect.setHoldingDB(oCompany)
            Dim conStrCompany As String = String.Empty
            If constr.Length = 0 Then
                dt = connect.ObjectGetAll_Query_SAP("SELECT * FROM [@AB_CompanySetup] WHERE U_DBName = @Param1", New Object() {databaseName})
                If dt IsNot Nothing And dt.Tables.Count > 0 And dt.Tables(0).Rows.Count > 0 Then
                    If dt.Tables(0).Rows(0)("U_ConnString") IsNot Nothing Then
                        conStrCompany = dt.Tables(0).Rows(0)("U_ConnString")
                        oCompany = Nothing
                        constr = connect.ConnectDB(dt.Tables(0).Rows(0)("U_ConnString"), oCompany)
                        Return b.ReturnMessage(lErrCode, "Can't connect database.")
                    End If
                Else
                    Return b.ReturnMessage(lErrCode, "Can't connect database.")
                End If
            End If
            If constr <> "" Then
                Return b.ReturnMessage(-1, constr)
            End If
            oCompany.XMLAsString = True
            oDocment = oCompany.GetBusinessObjectFromXML(strXml, 0)
            oCompany.StartTransaction()
            If IsUpdate Then
                If oDocment.GetByKey(Key) Then
                    oDocment.Browser.ReadXML(strXml, 0)
                    lErrCode = oDocment.Update()
                Else
                    Return b.ReturnMessage(-1, "Record not found!")
                End If
            Else
                lErrCode = oDocment.Add()
            End If

            If lErrCode <> 0 Then
                oCompany.GetLastError(lErrCode, sErrMsg)
                Return b.ReturnMessage(lErrCode, sErrMsg)
            Else
                Return b.ReturnMessage(lErrCode, "Operation successful!" + "-" + oCompany.GetNewObjectKey)
            End If
        Catch ex As Exception
            Return b.ReturnMessage(-1, ex.ToString)
        Finally
            If oCompany.InTransaction Then
                oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            If oCompany.Connected Then
                oCompany.Disconnect()
            End If
        End Try
    End Function
End Class