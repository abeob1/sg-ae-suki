Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Data.SqlTypes
Public Class Connection
    Private sConn As SqlConnection
    Private sConnSAP As SqlConnection
    Private conString As String
    Public bConnect As Boolean
    Public Function setSQLDB(ByVal DatabaseName As String) As String
        Try
            Dim SQLUser, SQLPwd, SQLServer, LicenseServer As String
            Dim strConnect As String = String.Empty
            Dim sCon As String = String.Empty
            Dim SQLType As String = String.Empty
            Dim MyArr As Array
            Dim sErrMsg As String = String.Empty
            strConnect = "DBConnect"
            sCon = System.Configuration.ConfigurationManager.AppSettings.Get(strConnect)
            MyArr = sCon.Split(";")
            If MyArr.Length > 0 Then

                SQLServer = MyArr(3).ToString()
                SQLUser = MyArr(4).ToString()
                SQLPwd = MyArr(5).ToString()
                LicenseServer = MyArr(6).ToString()

                sCon = "server= " + SQLServer + ";database=" + DatabaseName + " ;uid=" + SQLUser + "; pwd=" + SQLPwd + ";"
                conString = sCon
                sConnSAP = New SqlConnection(sCon)
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
        Return String.Empty
    End Function
#Region "Holding"
    Public Function setHoldingDB(ByRef oCompany As SAPbobsCOM.Company) As String
        Try
            Dim DatabaseName, SAPUserID, SAPPassWord, SQLUser, SQLPwd, SQLServer, LicenseServer, ServerType As String
            Dim strConnect As String = String.Empty
            Dim sCon As String = String.Empty
            Dim SQLType As String = String.Empty
            Dim MyArr As Array
            Dim sErrMsg As String = String.Empty
            strConnect = "DBConnect"
            sCon = System.Configuration.ConfigurationManager.AppSettings.Get(strConnect)
            MyArr = sCon.Split(";")
            If MyArr.Length > 0 Then

                DatabaseName = MyArr(0).ToString()
                'WriteLog(DatabaseName)
                SAPUserID = MyArr(1).ToString()
                'WriteLog(SAPUserID)
                SAPPassWord = MyArr(2).ToString()
                'WriteLog(SAPPassWord)
                SQLServer = MyArr(3).ToString()
                'WriteLog(SQLServer)
                SQLUser = MyArr(4).ToString()
                'WriteLog(SQLUser)
                SQLPwd = MyArr(5).ToString()
                'WriteLog(SQLPwd)
                LicenseServer = MyArr(6).ToString()
                'WriteLog(LicenseServer)
                ServerType = MyArr(7).ToString()
                'WriteLog(ServerType)
                sCon = "server= " + SQLServer + ";database=" + DatabaseName + " ;uid=" + SQLUser + "; pwd=" + SQLPwd + ";"
                conString = sCon
                sConnSAP = New SqlConnection(sCon)

                oCompany = New SAPbobsCOM.Company
                oCompany.CompanyDB = DatabaseName
                oCompany.UserName = SAPUserID
                oCompany.Password = SAPPassWord

                oCompany.Server = SQLServer
                oCompany.DbUserName = SQLUser
                oCompany.DbPassword = SQLPwd
                oCompany.LicenseServer = LicenseServer
                If ServerType = "2008" Then
                    oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008
                ElseIf ServerType = "2012" Then
                    oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012
                    'ElseIf ServerType = "2014" Then
                    '    oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012
                End If

            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
        Return String.Empty
    End Function
    Public Function connectHoldingDB(ByRef oCompany As SAPbobsCOM.Company) As String
        Try
            oCompany = New SAPbobsCOM.Company
            Dim sErrMsg As String = String.Empty
            Dim connectOk As Integer = 0
            'If oCompany.Connected Then
            '    oCompany.Disconnect()
            'End If
            setHoldingDB(oCompany)
            If oCompany.Connected = False Then
                If oCompany.Connect() <> 0 Then
                    oCompany.GetLastError(connectOk, sErrMsg)
                    WriteLog(sErrMsg)
                    Return sErrMsg
                Else
                    Return String.Empty
                End If
            End If
        Catch ex As Exception
            WriteLog(ex.Message)
            Return ex.ToString
        End Try
        Return String.Empty
    End Function
#End Region
#Region "Outlet"
    Public Function connectDB(ByVal conString As String, ByRef oCompany As SAPbobsCOM.Company) As String
        Try
            oCompany = New SAPbobsCOM.Company
            Dim sErrMsg As String = String.Empty
            Dim connectOk As Integer = 0
            'If oCompany.Connected Then
            '    oCompany.Disconnect()
            'End If
            setDB(conString, oCompany)
            If oCompany.Connected = False Then
                If oCompany.Connect() <> 0 Then
                    oCompany.GetLastError(connectOk, sErrMsg)
                    Return sErrMsg
                Else
                    Return String.Empty
                End If
            End If
        Catch ex As Exception
            Return ex.ToString
        End Try
        Return String.Empty
    End Function
    Public Sub setDB(ByVal conString As String, ByRef oCompany As SAPbobsCOM.Company)
        Try
            Dim DatabaseName, SAPUserID, SAPPassWord, SQLUser, SQLPwd, SQLServer, LicenseServer, ServerType As String
            Dim sCon As String = String.Empty
            Dim SQLType As String = String.Empty
            Dim MyArr As Array
            Dim sErrMsg As String = String.Empty
            sCon = conString
            MyArr = sCon.Split(";")
            If MyArr.Length > 0 Then

                DatabaseName = MyArr(0).ToString()
                SAPUserID = MyArr(1).ToString()
                SAPPassWord = MyArr(2).ToString()
                SQLServer = MyArr(3).ToString()
                SQLUser = MyArr(4).ToString()
                SQLPwd = MyArr(5).ToString()
                LicenseServer = MyArr(6).ToString()
                ServerType = MyArr(7).ToString()

                sCon = "server= " + SQLServer + ";database=" + DatabaseName + " ;uid=" + SQLUser + "; pwd=" + SQLPwd + ";"
                sConnSAP = New SqlConnection(sCon)

                    oCompany = New SAPbobsCOM.Company
                oCompany.CompanyDB = DatabaseName
                oCompany.UserName = SAPUserID
                oCompany.Password = SAPPassWord

                oCompany.Server = SQLServer
                oCompany.DbUserName = SQLUser
                oCompany.DbPassword = SQLPwd
                oCompany.LicenseServer = LicenseServer
                If ServerType = "2008" Then
                    oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008
                ElseIf ServerType = "2012" Then
                    oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012
                End If
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub
#End Region
#Region "ADO SAP"
    Private Function GetConnectionString_SAP() As SqlConnection
        If sConnSAP.State = ConnectionState.Open Then
            sConnSAP.Close()
        End If
        Try
            If sConnSAP.ConnectionString.Length = 0 Then
                sConnSAP.ConnectionString = conString
            End If
            sConnSAP.Open()
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
        Return sConnSAP
    End Function
    Public Function ObjectGetAll_Stored_SAP(ByVal storedName As String, ByVal ParamArrays As Dictionary(Of String, String)) As DataSet
        Try
            Using myConn = GetConnectionString_SAP()
                Dim MyCommand As SqlCommand = New SqlCommand(storedName, myConn)
                MyCommand.CommandType = CommandType.StoredProcedure

                Dim da As SqlDataAdapter = New SqlDataAdapter()
                Dim mytable As DataSet = New DataSet()
                da.SelectCommand = MyCommand
                For Each item As KeyValuePair(Of String, String) In ParamArrays
                    MyCommand.Parameters.AddWithValue(item.Key, item.Value)
                Next
                da.Fill(mytable)
                myConn.Close()
                Return mytable
            End Using
        Catch ex As Exception
            WriteLog(ex.ToString)
            Return Nothing
        End Try
    End Function
    Public Function ObjectGetAll_Query_SAP(ByVal QueryString As String) As DataSet
        Dim strCon As String = ""
        Try
            Using myConn = GetConnectionString_SAP()
                strCon = myConn.ConnectionString
                Dim MyCommand As SqlCommand = New SqlCommand(QueryString, myConn)
                MyCommand.CommandType = CommandType.Text
                Dim da As SqlDataAdapter = New SqlDataAdapter()
                Dim mytable As DataSet = New DataSet()
                da.SelectCommand = MyCommand
                da.Fill(mytable)
                myConn.Close()
                Return mytable
            End Using
        Catch ex As Exception
            WriteLog(ex.ToString & "_" & QueryString & "_" & strCon)
            Return Nothing
        End Try
    End Function
    Public Function ObjectGetAll_Query_SAP(ByVal QueryString As String, ByVal ParamArrays As Array) As DataSet
        Dim strCon As String = ""
        Try
            Using myConn = GetConnectionString_SAP()
                strCon = myConn.ConnectionString
                Dim MyCommand As SqlCommand = New SqlCommand(QueryString, myConn)
                MyCommand.CommandType = CommandType.Text
                Dim da As SqlDataAdapter = New SqlDataAdapter()
                Dim mytable As DataSet = New DataSet()
                da.SelectCommand = MyCommand
                For i As Integer = 0 To ParamArrays.Length - 1
                    MyCommand.Parameters.AddWithValue(String.Format("{0}{1}", "@Param", i + 1), ParamArrays(i))
                Next
                da.Fill(mytable)
                myConn.Close()
                Return mytable
            End Using
        Catch ex As Exception
            WriteLog(ex.ToString & "_" & QueryString & "_" & strCon)
            Return Nothing
        End Try
    End Function
    Public Function Object_Execute_SAP(ByVal Query As String, ByVal ParamArrays As Array) As Integer
        Dim strCon As String = ""
        Try
            Using myConn = GetConnectionString_SAP()
                strCon = myConn.ConnectionString
                Dim MyCommand As SqlCommand = New SqlCommand(Query, myConn)
                MyCommand.CommandType = CommandType.Text
                For i As Integer = 0 To ParamArrays.Length - 1
                    MyCommand.Parameters.AddWithValue(String.Format("{0}{1}", "@Param", i + 1), ParamArrays(i))
                Next
                Dim count As Integer = MyCommand.ExecuteNonQuery()
                myConn.Close()
                Return count
            End Using
        Catch ex As Exception
            WriteLog(ex.ToString & "_" & Query & "_" & strCon)
            Return 0
        End Try
    End Function
    Public Function Object_Execute_SAP(ByVal Query As String) As Integer
        Dim strCon As String = ""
        Try
            Using myConn = GetConnectionString_SAP()
                strCon = myConn.ConnectionString
                Dim MyCommand As SqlCommand = New SqlCommand(Query, myConn)
                MyCommand.CommandType = CommandType.Text
                Dim count As Integer = MyCommand.ExecuteNonQuery()
                myConn.Close()
                Return count
            End Using
        Catch ex As Exception
            WriteLog(ex.ToString & "_" & Query & "_" & strCon)
            Return 0
        End Try
    End Function

    Public Function Object_ExecuteScalar_SAP(ByVal Query As String) As Integer
        Dim strCon As String = ""
        Try
            Using myConn = GetConnectionString_SAP()
                strCon = myConn.ConnectionString
                Dim MyCommand As SqlCommand = New SqlCommand(Query, myConn)
                MyCommand.CommandType = CommandType.Text
                Dim count As Long = MyCommand.ExecuteScalar()
                myConn.Close()
                Return count
            End Using
        Catch ex As Exception
            WriteLog(ex.ToString & "_" & Query & "_" & strCon)
            Return 0
        End Try
    End Function

    Public Function Object_ExecuteScalar_EmailIssue(ByVal Query As String, ByVal Identify As String) As String
        Dim strCon As String = ""
        Try
            Using myConn = GetConnectionString_SAP()
                strCon = myConn.ConnectionString
                Dim MyCommand As SqlCommand = New SqlCommand(Query, myConn)
                MyCommand.CommandType = CommandType.Text
                Dim count As Long = MyCommand.ExecuteScalar()
                myConn.Close()
                Return count
            End Using
        Catch ex As Exception
            WriteLog(ex.ToString & "_" & Query & "_" & strCon)
            If (Identify = "Insert") Then
                Return "InsertError " & ex.Message
            ElseIf (Identify = "Update") Then
                Return "UpdateError " & ex.Message
            End If
        End Try
    End Function
#End Region
    Public Sub ConnectDB()
        Try
            Dim strConnect As String = String.Empty
            Dim sCon As String = String.Empty
            Dim MyArr As Array
            strConnect = "DBConnect"
            sCon = System.Configuration.ConfigurationSettings.AppSettings.Get(strConnect)
            MyArr = sCon.Split(";")
            Dim DatabaseName, SQLUser, SQLPwd, SQLServer As String

            DatabaseName = MyArr(0).ToString()
            SQLServer = MyArr(3).ToString()
            SQLUser = MyArr(4).ToString()
            SQLPwd = MyArr(5).ToString()

            sCon = "server= " + SQLServer + ";database=" + DatabaseName + " ;uid=" + SQLUser + "; pwd=" + SQLPwd + ";"
            sConn = New SqlConnection(sCon)
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub
    Private Function GetConnectionString_DB() As SqlConnection
        If sConn.State = ConnectionState.Open Then
            sConn.Close()
        End If
        Try
            sConn.Open()
        Catch ex As Exception
            WriteLog(ex.ToString)
            Return Nothing
        End Try
        Return sConn
    End Function
    Public Function ObjectGetAll_Query_DB(ByVal QueryString As String) As DataSet
        Try
            Using myConn = GetConnectionString_DB()
                Dim MyCommand As SqlCommand = New SqlCommand(QueryString, myConn)
                MyCommand.CommandType = CommandType.Text
                Dim da As SqlDataAdapter = New SqlDataAdapter()
                Dim mytable As DataSet = New DataSet()
                da.SelectCommand = MyCommand
                da.Fill(mytable)
                myConn.Close()
                Return mytable
            End Using
        Catch ex As Exception
            WriteLog(ex.ToString & "_" & QueryString)
            Return Nothing
        End Try
    End Function
    Public Function ObjectGetAll_Query_DB(ByVal QueryString As String, ByVal ParamArrays As Array) As DataSet
        Try
            Using myConn = GetConnectionString_DB()
                Dim MyCommand As SqlCommand = New SqlCommand(QueryString, myConn)
                MyCommand.CommandType = CommandType.Text
                Dim da As SqlDataAdapter = New SqlDataAdapter()
                Dim mytable As DataSet = New DataSet()
                da.SelectCommand = MyCommand
                For i As Integer = 0 To ParamArrays.Length - 1
                    MyCommand.Parameters.AddWithValue(String.Format("{0}{1}", "@Param", i + 1), ParamArrays(i))
                Next
                da.Fill(mytable)
                myConn.Close()
                Return mytable
            End Using
        Catch ex As Exception
            WriteLog(ex.ToString & "_" & QueryString)
            Return Nothing
        End Try
    End Function
    Public Shared Sub WriteLog(ByVal Str As String)
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
    End Sub
#Region "Mobile"

    Public Sub ConnectDynamicDB(ByVal DatabaseName As String, ByVal SQLServer As String, ByVal SQLUser As String, ByVal SQLPwd As String)
        Try
            Dim sCon As String = String.Empty
            sCon = "server= " + SQLServer + ";database=" + DatabaseName + " ;uid=" + SQLUser + "; pwd=" + SQLPwd + ";"
            sConn = New SqlConnection(sCon)
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Public Sub SetDynamicSAPDB(ByVal DatabaseName As String, ByVal SAPUser As String, ByVal SAPPwd As String, _
                                   ByVal SQLServer As String, ByVal SQLUser As String, ByVal SQLPwd As String, _
                                   ByVal SAPServer As String, ByVal ServerType As String, ByRef oCompany As SAPbobsCOM.Company)

        Try
            Dim sCon As String = "server= " + SQLServer + ";database=" + DatabaseName + " ;uid=" + SQLUser + "; pwd=" + SQLPwd + ";"
            sConnSAP = New SqlConnection(sCon)

            If IsNothing(oCompany) Then
                oCompany = New SAPbobsCOM.Company
            End If

            oCompany.CompanyDB = DatabaseName
            oCompany.UserName = SAPUser
            oCompany.Password = SAPPwd

            oCompany.Server = SQLServer
            oCompany.DbUserName = SQLUser
            oCompany.DbPassword = SQLPwd
            oCompany.LicenseServer = SAPServer
            If ServerType = "2008" Then
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2008
            ElseIf ServerType = "2012" Then
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2012
            End If


        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try

    End Sub

    Public Function connectDynamicSAPDB(ByVal DatabaseName As String, ByVal SAPUser As String, ByVal SAPPwd As String, _
                                   ByVal SQLServer As String, ByVal SQLUser As String, ByVal SQLPwd As String, _
                                   ByVal SAPServer As String, ByVal ServerType As String, ByRef oCompany As SAPbobsCOM.Company) As String


        Try
            If IsNothing(oCompany) Then
                oCompany = New SAPbobsCOM.Company
            End If
            Dim sErrMsg As String = String.Empty
            Dim connectOk As Integer = 0
            If oCompany.Connected Then
                oCompany.Disconnect()
            End If
            SetDynamicSAPDB(DatabaseName, SAPUser, SAPPwd, SQLServer, SQLUser, SQLPwd, SAPServer, ServerType, oCompany)
            Dim A As String = oCompany.LicenseServer
            If oCompany.Connected = False Then
                If oCompany.Connect() <> 0 Then
                    oCompany.GetLastError(connectOk, sErrMsg)
                    Return sErrMsg
                Else
                    Return String.Empty
                End If
            End If
        Catch ex As Exception
            Return ex.ToString
        End Try
        Return String.Empty
    End Function

    Public Function ObjectUpateByQuery(ByVal QueryString As String, ByVal ParamArrays As Array) As Integer
        Try
            Using myConn = GetConnectionString_DB()
                Dim MyCommand As SqlCommand = New SqlCommand(QueryString, myConn)
                MyCommand.CommandType = CommandType.Text
                For i As Integer = 0 To ParamArrays.Length - 1
                    MyCommand.Parameters.AddWithValue(String.Format("{0}{1}", "@Param", i + 1), ParamArrays(i))
                Next
                Dim effected As Integer = MyCommand.ExecuteNonQuery()
                myConn.Close()
                Return effected
            End Using
        Catch ex As Exception
            WriteLog("Mobile START")
            WriteLog(ex.ToString & "_" & QueryString)
            WriteLog("Mobile END")
            Return -1
        End Try
    End Function
#End Region
End Class
