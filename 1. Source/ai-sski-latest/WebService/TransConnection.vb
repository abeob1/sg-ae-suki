Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Data.SqlTypes
Public Class TransConnection
    Public sConn As SqlConnection
    Public sConnSAP As SqlConnection
    Public bConnect As Boolean

    Public Function connectDB(ByVal conString As String, ByRef oCompany As SAPbobsCOM.Company) As String
        Try
            If IsNothing(oCompany) Then
                oCompany = New SAPbobsCOM.Company
            End If
            Dim sErrMsg As String = String.Empty
            Dim connectOk As Integer = 0
            If oCompany.Connected Then
                oCompany.Disconnect()
            End If
            setDB(conString, oCompany)
            Dim A As String = oCompany.LicenseServer
            If oCompany.Connect() <> 0 Then
                oCompany.GetLastError(connectOk, sErrMsg)
                Return sErrMsg
            Else
                Return String.Empty
            End If
        Catch ex As Exception
            Return ex.ToString
        End Try
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

                If IsNothing(oCompany) Then
                    oCompany = New SAPbobsCOM.Company
                End If
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
    Private Function GetConnectionString_SAP() As SqlConnection
        If sConnSAP.State = ConnectionState.Open Then
            sConnSAP.Close()
        End If
        Try
            sConnSAP.Open()
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
        Return sConnSAP
    End Function
    Public Function ObjectGetAll_Query_SAP(ByVal QueryString As String) As DataSet
        Try
            Using myConn = GetConnectionString_SAP()
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
            WriteLog(ex.ToString)
            Return Nothing
        End Try
    End Function
    Public Function ObjectGetAll_Query_SAP(ByVal QueryString As String, ByVal ParamArrays As Array) As DataSet
        Try
            Using myConn = GetConnectionString_SAP()
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
            WriteLog(ex.ToString)
            Return Nothing
        End Try
    End Function
    Public Shared Sub WriteLog(ByVal Str As String)
        Dim oWrite As IO.StreamWriter
        Dim FilePath As String
        FilePath = System.Configuration.ConfigurationSettings.AppSettings("LogPath")

        If IO.File.Exists(FilePath) Then
            oWrite = IO.File.AppendText(FilePath)
        Else
            oWrite = IO.File.CreateText(FilePath)
        End If
        oWrite.Write(Now.ToString() + ":" + Str + vbCrLf)
        oWrite.Close()
    End Sub
End Class
