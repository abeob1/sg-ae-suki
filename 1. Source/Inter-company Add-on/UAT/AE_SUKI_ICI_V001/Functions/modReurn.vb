Module modReurn


    Function APReturn(ByRef oHoldingCompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, ByVal oDT_GRPODetails As DataTable, ByRef sErrDesc As String) As Long

        'Function   :   APReturn()
        'Purpose    :   Create a AP invoice through DI API
        'Parameters :   
        '               oDICompany As SAPbobsCOM.Company
        '               ByRef sErrDesc As String

        '                   sErrDesc=Error Description to be returned to calling function

        'Return     :   0 - FAILURE
        '               1 - SUCCESS

        'Author     :   JOHN
        'Date       :   26/09/2014
        'Change     :

        Dim sFuncName As String = String.Empty
        Dim lRetCode As Long
        Dim sDocEntry As String = String.Empty

        Dim oDV_GRPODetails As DataView = Nothing
        Dim oDT_Distinct_GRPODocEntry As DataTable = Nothing

        Dim sGRPO_DocEntry As String = String.Empty
        Dim sARRTN_DocEntry As String = String.Empty
        Dim sARRTN_DocNum As String = String.Empty

        Try
            sFuncName = "APReturn()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)
            Dim orset As SAPbobsCOM.Recordset = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim orset1 As SAPbobsCOM.Recordset = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)


            Dim oAPReturn As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseReturns)
            Dim oGRPO As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes)
            Dim oARReturn As SAPbobsCOM.Documents = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oReturns)

            sARRTN_DocEntry = oDT_GRPODetails.Rows(0).Item(0).ToString()
            sARRTN_DocNum = oDT_GRPODetails.Rows(0).Item(3).ToString()

            oDV_GRPODetails = oDT_GRPODetails.DefaultView
            oDT_Distinct_GRPODocEntry = oDT_GRPODetails.DefaultView.ToTable(True, "GRPODocEntry")


            If oARReturn.GetByKey(sARRTN_DocEntry) Then

                oAPReturn.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes
                oAPReturn.CardCode = p_oCompDef.TradingVendor
                oAPReturn.DocDate = oARReturn.DocDate
                oAPReturn.DocDueDate = oARReturn.DocDueDate
                oAPReturn.TaxDate = oARReturn.TaxDate
                oAPReturn.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items

                oAPReturn.Rounding = oARReturn.Rounding
                If oARReturn.RoundingDiffAmountFC <> 0 Then
                    oAPReturn.RoundingDiffAmount = oARReturn.RoundingDiffAmountFC
                ElseIf oARReturn.RoundingDiffAmountSC <> 0 Then
                    oAPReturn.RoundingDiffAmount = oARReturn.RoundingDiffAmountSC
                Else
                    oAPReturn.RoundingDiffAmount = oARReturn.RoundingDiffAmount
                End If

                oAPReturn.NumAtCard = sARRTN_DocNum
                oAPReturn.UserFields.Fields.Item("U_AE_Docentry").Value = sARRTN_DocEntry

                For imjs As Integer = 0 To oDT_Distinct_GRPODocEntry.Rows.Count - 1

                    oDV_GRPODetails.RowFilter = "GRPODocEntry = '" & oDT_Distinct_GRPODocEntry.Rows(imjs).Item(0).ToString() & "'"
                    For Each drv As DataRowView In oDV_GRPODetails
                        If oGRPO.GetByKey(Convert.ToInt32(drv(6).ToString.Trim)) Then
                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("GRPO DocEntry " & drv(6).ToString.Trim & "  Line No. " & drv(7).ToString.Trim, sFuncName)

                            If String.IsNullOrEmpty(drv(6).ToString.Trim) Then
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Base Docentry is Empty", sFuncName)
                                sErrDesc = "Base Docentry is Empty "
                                Return RTN_ERROR
                            End If

                            If String.IsNullOrEmpty(drv(7).ToString.Trim) Then
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Base line Number is Empty", sFuncName)
                                sErrDesc = "Base line Number is Empty "
                                Return RTN_ERROR
                            End If

                            oAPReturn.Lines.BaseEntry = Convert.ToInt32(drv(6).ToString.Trim)
                            oAPReturn.Lines.BaseLine = Convert.ToInt32(drv(7).ToString.Trim)
                            oAPReturn.Lines.BaseType = 20
                            oAPReturn.Lines.Quantity = Convert.ToDouble(drv(10).ToString.Trim)
                            oAPReturn.Lines.WarehouseCode = drv(8).ToString.Trim
                            oAPReturn.Lines.CostingCode = drv(9).ToString.Trim
                            oAPReturn.Lines.Add()
                        End If
                    Next
                Next
            End If


            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Adding ", sFuncName)
            lRetCode = oAPReturn.Add
            If lRetCode <> 0 Then
                sErrDesc = oTradingCompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR ", sFuncName)
                APReturn = RTN_ERROR
            Else
                oTradingCompany.GetNewObjectCode(sDocEntry)
                sErrDesc = sDocEntry
                APReturn = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            APReturn = RTN_ERROR
        End Try
    End Function

    Function ARReturn(ByRef oHoldingCompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, ByVal APReturnDocentry As String, ByVal DeliveryDocentry As String, _
               ByRef sErrDesc As String) As Long

        'Function   :   ARReturn()
        'Purpose    :   Create a AP invoice through DI API
        'Parameters :   
        '               oDICompany As SAPbobsCOM.Company
        '               ByRef sErrDesc As String

        '                   sErrDesc=Error Description to be returned to calling function

        'Return     :   0 - FAILURE
        '               1 - SUCCESS

        'Author     :   JOHN
        'Date       :   26/09/2014
        'Change     :

        Dim sFuncName As String = String.Empty
        Dim lRetCode As Long
        Dim sDocEntry As String = String.Empty

        Try
            sFuncName = "ARReturn()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            Dim oAPReturn As SAPbobsCOM.Documents = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseReturns)
            Dim oDelivery As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes)
            Dim oARReturn As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oReturns)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AP Return DocEntry " & APReturnDocentry, sFuncName)

            Dim orset As SAPbobsCOM.Recordset = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim orset1 As SAPbobsCOM.Recordset = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            orset1.DoQuery("SELECT (SELECT T10.[SeriesName] FROM NNM1 T10 WHERE T10.[Series]  = T0.[Series])  +  CAST(T0.DOCNUM AS NVARCHAR) AS [DOCNUM] FROM ORPD T0 where T0.DocEntry = '" & APReturnDocentry & "'")



            If oAPReturn.GetByKey(APReturnDocentry) Then

                oARReturn.DocObjectCode = SAPbobsCOM.BoObjectTypes.oDeliveryNotes
                oARReturn.CardCode = p_oCompDef.TradingCustomer 'Delivery.CardCode 
                oARReturn.NumAtCard = orset1.Fields.Item("DOCNUM").Value 'APReturnDocentry
                oARReturn.DocDate = oAPReturn.DocDate
                oARReturn.DocDueDate = oAPReturn.DocDueDate
                oARReturn.TaxDate = oAPReturn.TaxDate
                oARReturn.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items

                oARReturn.Rounding = oAPReturn.Rounding
                If oAPReturn.RoundingDiffAmountFC <> 0 Then
                    oARReturn.RoundingDiffAmount = oAPReturn.RoundingDiffAmountFC
                ElseIf oAPReturn.RoundingDiffAmountSC Then
                    oARReturn.RoundingDiffAmount = oAPReturn.RoundingDiffAmountSC
                Else
                    oARReturn.RoundingDiffAmount = oAPReturn.RoundingDiffAmount
                End If

                If Not String.IsNullOrEmpty(DeliveryDocentry) Then

                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Creating the AR Return With the Base Document " & DeliveryDocentry, sFuncName)

                    If oDelivery.GetByKey(DeliveryDocentry) Then


                        For IntRow As Integer = 0 To oAPReturn.Lines.Count - 1
                            oAPReturn.Lines.SetCurrentLine(IntRow)
                            orset.DoQuery("SELECT T0.[LineNum], T0.[Price],T0.[whscode], T0.[OcrCode] FROM DLN1 T0 " & _
                                      "WHERE T0.[DocEntry] = '" & oAPReturn.Lines.UserFields.Fields.Item("U_AE_RefDocEntry").Value & "' and  T0.[LineNum] = '" & oAPReturn.Lines.UserFields.Fields.Item("U_AE_RefBaseLine").Value & "'")

                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Fetching Line Number " & "SELECT T0.[LineNum], T0.[whscode], T0.[OcrCode] FROM DLN1 T0 " & _
                                        "WHERE T0.[DocEntry] = '" & oAPReturn.Lines.UserFields.Fields.Item("U_AE_RefDocEntry").Value & "' and  T0.[LineNum] = '" & oAPReturn.Lines.UserFields.Fields.Item("U_AE_RefBaseLine").Value & "'", sFuncName)

                            If String.IsNullOrEmpty(orset.Fields.Item("LineNum").Value) Then
                                ' sErrDesc = "Base line number and Base docentry is empty "
                                sErrDesc = "No AR Return created.  Please manually create AR Return"
                                Return RTN_ERROR
                            End If
                            oARReturn.Lines.BaseEntry = DeliveryDocentry
                            oARReturn.Lines.BaseLine = orset.Fields.Item("LineNum").Value 'oARReturn.Lines.BaseLine
                            oARReturn.Lines.BaseType = 15

                            ' oARReturn.Lines.ItemCode = oAPReturn.Lines.ItemCode
                            oARReturn.Lines.Quantity = oAPReturn.Lines.Quantity
                            ' oAPReturn.Lines.UnitPrice = oGRPO.Lines.UnitPrice
                            oARReturn.Lines.WarehouseCode = orset.Fields.Item("whscode").Value 'Delivery.Lines.WarehouseCode
                            oARReturn.Lines.CostingCode = orset.Fields.Item("OcrCode").Value 'Delivery.Lines.
                            If IntRow = 0 Then
                                oARReturn.UserFields.Fields.Item("U_AB_POWhsCode").Value = oAPReturn.Lines.WarehouseCode
                            End If
                            oARReturn.Lines.Add()
                        Next IntRow
                    End If
                Else
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Creating the AR Return Standalone ", sFuncName)
                    For IntRow As Integer = 0 To oAPReturn.Lines.Count - 1
                        oAPReturn.Lines.SetCurrentLine(IntRow)
                        orset.DoQuery("SELECT T0.[DfltWH] FROM OITM T0 WHERE T0.[ItemCode] = '" & oAPReturn.Lines.ItemCode & "'")
                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Fetching Default Whs " & "SELECT T0.[DfltWH] FROM OITM T0 WHERE T0.[ItemCode] = '" & oAPReturn.Lines.ItemCode & "'", sFuncName)
                        oARReturn.Lines.ItemCode = oAPReturn.Lines.ItemCode
                        oARReturn.Lines.Quantity = oAPReturn.Lines.Quantity
                        oARReturn.Lines.UnitPrice = oAPReturn.Lines.UnitPrice
                        oARReturn.Lines.WarehouseCode = orset.Fields.Item("DfltWH").Value 'Delivery.Lines.WarehouseCode
                        oARReturn.Lines.CostingCode = orset.Fields.Item("DfltWH").Value 'Delivery.Lines.CostingCode
                        oARReturn.Lines.UserFields.Fields.Item("U_AE_RefDocEntry").Value = APReturnDocentry
                        oARReturn.Lines.UserFields.Fields.Item("U_AE_RefBaseLine").Value = Convert.ToString(oAPReturn.Lines.LineNum)
                        If IntRow = 0 Then
                            oARReturn.UserFields.Fields.Item("U_AB_POWhsCode").Value = oAPReturn.Lines.WarehouseCode
                        End If
                        oARReturn.Lines.Add()
                    Next IntRow
                End If
            End If

            oARReturn.UserFields.Fields.Item("U_AE_Docentry").Value = APReturnDocentry

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Adding ", sFuncName)
            lRetCode = oARReturn.Add
            If lRetCode <> 0 Then
                sErrDesc = oTradingCompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                ARReturn = RTN_ERROR
            Else
                oTradingCompany.GetNewObjectCode(sDocEntry)
                sErrDesc = sDocEntry
                ARReturn = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            ARReturn = RTN_ERROR
        End Try
    End Function

    Function UpdateARreturnStatus(ByRef oDICompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, ByVal ARReturnDocEntry As String, _
                  ByVal APReturnDocEntry As String, ByVal sError As String, ByVal sStatus As String, ByVal sTable As String, sErrDesc As String) As Long

        'Function   :   UpdateARreturnStatus()
        'Purpose    :   Create a AP invoice through DI API
        'Parameters :   
        '               oDICompany As SAPbobsCOM.Company
        '               ByRef sErrDesc As String

        '                   sErrDesc=Error Description to be returned to calling function

        'Return     :   0 - FAILURE
        '               1 - SUCCESS

        'Author     :   JOHN
        'Date       :   26/09/2014
        'Change     :

        Dim sFuncName As String = String.Empty
        Dim lRetCode As Long
        Dim sDocEntry As String = String.Empty

        Try
            sFuncName = "UpdateARreturnStatus()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)
            Dim orset As SAPbobsCOM.Recordset = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            Dim oARreturn As SAPbobsCOM.Documents = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oReturns)
            orset.DoQuery("SELECT (SELECT T10.[SeriesName] FROM NNM1 T10 WHERE T10.[Series]  = T0.[Series]) + cast(docnum as nvarchar) as [DocNum] FROM " & sTable & " T0 WHERE T0.[Docentry] = '" & APReturnDocEntry & "'")

            Dim DDDD = "SELECT (SELECT T10.[SeriesName] FROM NNM1 T10 WHERE T10.[Series]  = T0.[Series]) + cast(docnum as nvarchar) as [DocNum] FROM " & sTable & " T0 WHERE T0.[Docentry] = '" & APReturnDocEntry & "'"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Query " & DDDD, sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR Return Docentry " & ARReturnDocEntry, sFuncName)

            If sError.Length >= 198 Then
                sError = sError.ToString.Substring(0, 198)
            End If

            If oARreturn.GetByKey(ARReturnDocEntry) Then
                oARreturn.NumAtCard = orset.Fields.Item("DocNum").Value 'APReturnDocEntry
                oARreturn.UserFields.Fields.Item("U_AE_Docentry").Value = APReturnDocEntry
                oARreturn.UserFields.Fields.Item("U_AE_Istatus").Value = sStatus
                If sStatus = "Fail" Then
                    oARreturn.UserFields.Fields.Item("U_AE_IErrorMsg").Value = sError
                Else
                    oARreturn.UserFields.Fields.Item("U_AE_IErrorMsg").Value = ""
                End If

                oARreturn.UserFields.Fields.Item("U_AE_IDate").Value = Now.Date
            End If

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Updating " & ARReturnDocEntry, sFuncName)

            lRetCode = oARreturn.Update
            If lRetCode <> 0 Then
                sErrDesc = oDICompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                UpdateARreturnStatus = RTN_ERROR
            Else
                oDICompany.GetNewObjectCode(sDocEntry)
                sErrDesc = sDocEntry
                UpdateARreturnStatus = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            UpdateARreturnStatus = RTN_ERROR
        End Try
    End Function

    Function UpdateAPreturnStatus(ByRef oDICompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, ByVal ARReturnDocEntry As String, _
                 ByVal APReturnDocEntry As String, ByVal sError As String, ByVal sStatus As String, ByVal sTable As String, ByRef sErrDesc As String) As Long

        'Function   :   UpdateARreturnStatus()
        'Purpose    :   Create a AP invoice through DI API
        'Parameters :   
        '               oDICompany As SAPbobsCOM.Company
        '               ByRef sErrDesc As String

        '                   sErrDesc=Error Description to be returned to calling function

        'Return     :   0 - FAILURE
        '               1 - SUCCESS

        'Author     :   JOHN
        'Date       :   26/09/2014
        'Change     :

        Dim sFuncName As String = String.Empty
        Dim lRetCode As Long
        Dim sDocEntry As String = String.Empty

        Try
            sFuncName = "UpdateARreturnStatus()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            Dim orset As SAPbobsCOM.Recordset = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            orset.DoQuery("SELECT (SELECT T10.[SeriesName] FROM NNM1 T10 WHERE T10.[Series]  = T0.[Series]) + cast(docnum as nvarchar) as [DocNum] FROM " & sTable & " T0 WHERE T0.[Docentry] = '" & ARReturnDocEntry & "'")

            Dim oAPreturn As SAPbobsCOM.Documents = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseReturns)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AP Return DocEntry " & APReturnDocEntry, sFuncName)
            If sError.Length >= 198 Then
                sError = sError.ToString.Substring(0, 198)
            End If

            If oAPreturn.GetByKey(APReturnDocEntry) Then
                oAPreturn.NumAtCard = orset.Fields.Item("DocNum").Value 'ARReturnDocEntry
                oAPreturn.UserFields.Fields.Item("U_AE_Docentry").Value = ARReturnDocEntry
                oAPreturn.UserFields.Fields.Item("U_AE_Istatus").Value = sStatus
                If sStatus = "Fail" Then
                    oAPreturn.UserFields.Fields.Item("U_AE_IErrorMsg").Value = sError
                Else
                    oAPreturn.UserFields.Fields.Item("U_AE_IErrorMsg").Value = ""
                End If
                oAPreturn.UserFields.Fields.Item("U_AE_IDate").Value = Now.Date
            End If
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Updating ", sFuncName)
            lRetCode = oAPreturn.Update
            If lRetCode <> 0 Then
                sErrDesc = oDICompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                UpdateAPreturnStatus = RTN_ERROR
            Else
                oDICompany.GetNewObjectCode(sDocEntry)
                sErrDesc = sDocEntry
                UpdateAPreturnStatus = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            UpdateAPreturnStatus = RTN_ERROR
        End Try
    End Function


End Module
