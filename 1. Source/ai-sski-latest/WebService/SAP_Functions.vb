Public Class SAP_Functions
    'Suki Web
    Public Function CheckUser(ByVal UserID As String, ByVal Password As String) As Boolean
        Try
            Dim str As String
            str = "Select UserID, Password from Users Where ID_User = '" + UserID + "' And '" + Password + "'"
            Dim dtUser As DataTable
            Dim connect As New Connection()
            dtUser = connect.ObjectGetAll_Query_DB(str).Tables(0)

            If dtUser.Rows.Count <> 1 Then
                Return False
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function ConvertRS2DT(ByVal RS As SAPbobsCOM.Recordset) As DataSet
        Dim dtTable As New DataSet
        dtTable.Tables.Add()
        Dim NewCol As DataColumn
        Dim NewRow As DataRow
        Dim ColCount As Integer
        Try
            For ColCount = 0 To RS.Fields.Count - 1
                Dim dataType As String = "System."
                Select Case RS.Fields.Item(ColCount).Type
                    Case SAPbobsCOM.BoFieldTypes.db_Alpha
                        dataType = dataType & "String"
                    Case SAPbobsCOM.BoFieldTypes.db_Date
                        dataType = dataType & "DateTime"
                    Case SAPbobsCOM.BoFieldTypes.db_Float
                        dataType = dataType & "Double"
                    Case SAPbobsCOM.BoFieldTypes.db_Memo
                        dataType = dataType & "String"
                    Case SAPbobsCOM.BoFieldTypes.db_Numeric
                        dataType = dataType & "Decimal"
                    Case Else
                        dataType = dataType & "String"
                End Select

                NewCol = New DataColumn(RS.Fields.Item(ColCount).Name, System.Type.GetType(dataType))
                dtTable.Tables(0).Columns.Add(NewCol)
            Next
            RS.MoveFirst()
            Do Until RS.EoF

                NewRow = dtTable.Tables(0).NewRow
                'populate each column in the row we're creating
                For ColCount = 0 To RS.Fields.Count - 1

                    NewRow.Item(RS.Fields.Item(ColCount).Name) = RS.Fields.Item(ColCount).Value

                Next

                'Add the row to the datatable
                dtTable.Tables(0).Rows.Add(NewRow)

                RS.MoveNext()
            Loop
            Return dtTable
        Catch ex As Exception
            MsgBox(ex.ToString & Chr(10) & "Error converting SAP Recordset to DataTable", MsgBoxStyle.Exclamation)
            Return Nothing
        End Try
    End Function
    Public Function GetGopyFromTo(ByVal Type As Integer, ByVal ObjType As String) As DataSet
        Dim ds = New DataSet
        ds.Tables.Add()
        ds.Tables(0).Columns.Add("Code", GetType(String))
        ds.Tables(0).Columns.Add("Name", GetType(String))
        Dim dr As DataRow

        Select Case Type
            Case 1 ' Copy To
                Select Case ObjType
                    Case "22"
                        dr = ds.Tables(0).NewRow
                        dr("Code") = "AA"
                        dr("Name") = "GRPO"
                        ds.Tables(0).Rows.Add(dr)

                        dr = ds.Tables(0).NewRow
                        dr("Code") = "AA"
                        dr("Name") = "AP Invoice"
                        ds.Tables(0).Rows.Add(dr)
                End Select
            Case 2 ' Copy From
                Select Case ObjType
                    Case "22"
                        dr = ds.Tables(0).NewRow
                        dr("Code") = "AA"
                        dr("Name") = "Purchase Quotation"
                        ds.Tables(0).Rows.Add(dr)
                End Select
        End Select
        Return ds
    End Function
    Public Function GetMaxDocEntry(ByVal ObjType As String, ByVal UserID As String, ByVal TableName As String, ByVal KeyName As String) As String
        Dim dt As DataSet
        Dim Str As String = ""
        Dim connect As New Connection()

        dt = connect.ObjectGetAll_Query_SAP("select MAX(" + KeyName + ") DocKey from " + TableName)

        If IsNothing(dt) Then
            Return ""
        End If
        If dt.Tables(0).Rows.Count > 0 Then
            Return dt.Tables(0).Rows(0).Item(KeyName)
        Else
            Return ""
        End If

    End Function
    Public Function ReturnMessage(ByVal ErrCode As Integer, ByVal ErrMsg As String) As DataSet
        Dim dtJE = New DataSet
        dtJE.Tables.Add()
        dtJE.Tables(0).Columns.Add("ErrCode", GetType(Integer))
        dtJE.Tables(0).Columns.Add("ErrMsg", GetType(String))

        Dim dr As DataRow
        dr = dtJE.Tables(0).NewRow
        dr("ErrCode") = ErrCode
        dr("ErrMsg") = ErrMsg
        dtJE.Tables(0).Rows.Add(dr)

        Return dtJE
    End Function


  
   
End Class
