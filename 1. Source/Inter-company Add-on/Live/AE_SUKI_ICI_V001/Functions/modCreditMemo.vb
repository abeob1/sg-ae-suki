Module modCreditMemo

    Function APCreditMemo(ByRef oHoldingCompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, ByVal ARCreditmemoDocentry As String, ByVal APDocEntry As String, _
                  ByVal sBaseType As String, ByRef sErrDesc As String) As Long

        'Function   :   APCreditMemo()
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

        ''----------------------------------  From AR Credit memo to AP Credit Memo

        Dim sFuncName As String = String.Empty
        Dim lRetCode As Long
        Dim sDocEntry As String = String.Empty

        Try
            sFuncName = "APCreditMemo()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            Dim orset As SAPbobsCOM.Recordset = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim orset1 As SAPbobsCOM.Recordset = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            Dim oAPInvoice As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices)
            Dim oAPCreditmemo As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes)
            Dim oARCreditmemo As SAPbobsCOM.Documents = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes)
            Dim oAPRetrun As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseReturns)


            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR CreditNote Docentry " & ARCreditmemoDocentry, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AP Invoice Docentry " & APDocEntry, sFuncName)

            orset1.DoQuery("SELECT (SELECT T10.[SeriesName] FROM NNM1 T10 WHERE T10.[Series]  = T0.[Series])  +  CAST(T0.DOCNUM AS NVARCHAR) AS [DOCNUM] FROM ORIN T0 where T0.DocEntry = '" & ARCreditmemoDocentry & "'")


            If sBaseType = "AR Invoice" Then
                If oARCreditmemo.GetByKey(ARCreditmemoDocentry) Then
                    If oAPInvoice.GetByKey(APDocEntry) Then

                        oAPCreditmemo.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseInvoices

                        oAPCreditmemo.CardCode = oAPInvoice.CardCode
                        oAPCreditmemo.NumAtCard = orset1.Fields.Item("DOCNUM").Value
                        oAPCreditmemo.DocDate = oARCreditmemo.DocDate
                        oAPCreditmemo.DocDueDate = oARCreditmemo.DocDueDate
                        oAPCreditmemo.TaxDate = oARCreditmemo.TaxDate
                        oAPCreditmemo.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items

                        oAPCreditmemo.Rounding = oARCreditmemo.Rounding
                        If oARCreditmemo.RoundingDiffAmountFC <> 0 Then
                            oAPCreditmemo.RoundingDiffAmount = oARCreditmemo.RoundingDiffAmountFC
                        Else
                            oAPCreditmemo.RoundingDiffAmount = oARCreditmemo.RoundingDiffAmountSC
                        End If


                        For IntRow As Integer = 0 To oARCreditmemo.Lines.Count - 1
                            oARCreditmemo.Lines.SetCurrentLine(IntRow)

                            orset.DoQuery("SELECT T0.[LineNum],T0.[Price], T0.[whscode], T0.[OcrCode] FROM PCH1 T0 " & _
                                         "WHERE T0.[U_AE_RefDocEntry] = '" & oARCreditmemo.Lines.UserFields.Fields.Item("BaseEntry").Value & "' and  T0.[U_AE_RefBaseLine] = '" & oARCreditmemo.Lines.UserFields.Fields.Item("BaseLine").Value & "'")

                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Fetching Line Number " & "SELECT T0.[LineNum], T0.[whscode], T0.[OcrCode] FROM PCH1 T0 " & _
                                         "WHERE T0.[U_AE_RefDocEntry] = '" & oARCreditmemo.Lines.UserFields.Fields.Item("BaseEntry").Value & "' and  T0.[U_AE_RefBaseLine] = '" & oARCreditmemo.Lines.UserFields.Fields.Item("BaseLine").Value & "'", sFuncName)

                            If String.IsNullOrEmpty(orset.Fields.Item("LineNum").Value) Then
                                ' sErrDesc = "Base line number and Base docentry is empty "
                                sErrDesc = "No AP Credit Memo created.  Please manually create AP Credit Memo"
                                Return RTN_ERROR
                            End If

                            ' MsgBox(oARCreditmemo.Lines.UserFields.Fields.Item("BaseEntry").Value & "  " & orset.Fields.Item("LineNum").Value)
                            oAPCreditmemo.Lines.BaseEntry = APDocEntry
                            oAPCreditmemo.Lines.BaseLine = orset.Fields.Item("LineNum").Value
                            oAPCreditmemo.Lines.BaseType = 18
                            oAPCreditmemo.Lines.Quantity = oARCreditmemo.Lines.Quantity
                            oAPCreditmemo.Lines.UnitPrice = orset.Fields.Item("Price").Value
                            oAPCreditmemo.Lines.WarehouseCode = orset.Fields.Item("whscode").Value
                            oAPCreditmemo.Lines.CostingCode = orset.Fields.Item("OcrCode").Value
                            oAPCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefDocEntry").Value = ARCreditmemoDocentry
                            oAPCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefBaseLine").Value = CStr(oARCreditmemo.Lines.LineNum)

                            oAPCreditmemo.Lines.Add()
                        Next IntRow

                        oAPCreditmemo.UserFields.Fields.Item("U_AE_Docentry").Value = ARCreditmemoDocentry
                    End If
                End If
            ElseIf sBaseType = "AR Return" Then

                If oARCreditmemo.GetByKey(ARCreditmemoDocentry) Then
                    If oAPRetrun.GetByKey(APDocEntry) Then

                        oAPCreditmemo.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseReturns

                        oAPCreditmemo.CardCode = oAPRetrun.CardCode
                        oAPCreditmemo.NumAtCard = orset1.Fields.Item("DOCNUM").Value
                        oAPCreditmemo.DocDate = oARCreditmemo.DocDate
                        oAPCreditmemo.DocDueDate = oARCreditmemo.DocDueDate
                        oAPCreditmemo.TaxDate = oARCreditmemo.TaxDate
                        oAPCreditmemo.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items

                        oAPCreditmemo.Rounding = oARCreditmemo.Rounding
                        If oARCreditmemo.RoundingDiffAmountFC <> 0 Then
                            oAPCreditmemo.RoundingDiffAmount = oARCreditmemo.RoundingDiffAmountFC
                        Else
                            oAPCreditmemo.RoundingDiffAmount = oARCreditmemo.RoundingDiffAmountSC
                        End If


                        For IntRow As Integer = 0 To oARCreditmemo.Lines.Count - 1
                            oARCreditmemo.Lines.SetCurrentLine(IntRow)

                            orset.DoQuery("SELECT T0.[LineNum],T0.[Price], T0.[whscode], T0.[OcrCode] FROM RPD1 T0 " & _
                                         "WHERE T0.[DocEntry] = '" & oARCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefDocEntry").Value & "' and  T0.[LineNum] = '" & oARCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefBaseLine").Value & "'")

                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Fetching Line Number " & "SELECT T0.[LineNum],T0.[Price], T0.[whscode], T0.[OcrCode] FROM RPD1 T0 " & _
                                         "WHERE T0.[DocEntry] = '" & oARCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefDocEntry").Value & "' and  T0.[LineNum] = '" & oARCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefBaseLine").Value & "'", sFuncName)

                            If String.IsNullOrEmpty(orset.Fields.Item("LineNum").Value) Then
                                ' sErrDesc = "Base line number and Base docentry is empty "
                                sErrDesc = "No AP Credit Memo created.  Please manually create AP Credit Memo"
                                Return RTN_ERROR
                            End If

                            ' MsgBox(orset.Fields.Item("LineNum").Value & "  " & APDocEntry & "  " & oARCreditmemo.Lines.Quantity & "  " & orset.Fields.Item("Price").Value & "  " & orset.Fields.Item("whscode").Value)
                            oAPCreditmemo.Lines.BaseEntry = APDocEntry
                            oAPCreditmemo.Lines.BaseLine = orset.Fields.Item("LineNum").Value
                            oAPCreditmemo.Lines.BaseType = 21
                            oAPCreditmemo.Lines.Quantity = oARCreditmemo.Lines.Quantity
                            oAPCreditmemo.Lines.UnitPrice = orset.Fields.Item("Price").Value
                            oAPCreditmemo.Lines.WarehouseCode = orset.Fields.Item("whscode").Value
                            oAPCreditmemo.Lines.CostingCode = orset.Fields.Item("OcrCode").Value
                            oAPCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefDocEntry").Value = ARCreditmemoDocentry
                            oAPCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefBaseLine").Value = CStr(oARCreditmemo.Lines.LineNum)

                            oAPCreditmemo.Lines.Add()
                        Next IntRow

                        oAPCreditmemo.UserFields.Fields.Item("U_AE_Docentry").Value = ARCreditmemoDocentry
                    End If
                End If
            End If

           

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Adding", sFuncName)
            lRetCode = oAPCreditmemo.Add
            If lRetCode <> 0 Then
                sErrDesc = oTradingCompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                APCreditMemo = RTN_ERROR
            Else
                oTradingCompany.GetNewObjectCode(sDocEntry)
                sErrDesc = sDocEntry
                APCreditMemo = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            APCreditMemo = RTN_ERROR
        End Try
    End Function

    Function UpdateArCreditmemoStatus(ByRef oDICompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, ByVal ARCreditDocEntry As String, _
                   ByVal APCreditDocEntry As String, ByVal sError As String, ByVal sStatus As String, ByVal sTable As String, ByRef sErrDesc As String) As Long

        'Function   :   UpdateArCreditmemoStatus()
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
            sFuncName = "UpdateArInvoiceStatus()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            Dim oARCreditmemo As SAPbobsCOM.Documents = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes)
            ' Dim oAPCreditmemo As SAPbobsCOM.Documents = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR CreditNote Docentry " & ARCreditDocEntry, sFuncName)

            Dim orset As SAPbobsCOM.Recordset = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            orset.DoQuery("SELECT (SELECT T10.[SeriesName] FROM NNM1 T10 WHERE T10.[Series]  = T0.[Series]) + cast(docnum as nvarchar) as [DocNum] FROM " & sTable & " T0 WHERE T0.[Docentry] = '" & APCreditDocEntry & "'")
            If sError.Length >= 198 Then
                sError = sError.ToString.Substring(0, 198)
            End If

            If oARCreditmemo.GetByKey(ARCreditDocEntry) Then
                oARCreditmemo.NumAtCard = orset.Fields.Item("DocNum").Value
                oARCreditmemo.UserFields.Fields.Item("U_AE_Docentry").Value = APCreditDocEntry
                oARCreditmemo.UserFields.Fields.Item("U_AE_Istatus").Value = sStatus
                If sStatus = "Fail" Then
                    oARCreditmemo.UserFields.Fields.Item("U_AE_IErrorMsg").Value = sError
                Else
                    oARCreditmemo.UserFields.Fields.Item("U_AE_IErrorMsg").Value = ""
                End If
                oARCreditmemo.UserFields.Fields.Item("U_AE_IDate").Value = Now.Date
            End If

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Updating ", sFuncName)
            lRetCode = oARCreditmemo.Update
            If lRetCode <> 0 Then
                sErrDesc = oDICompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                UpdateArCreditmemoStatus = RTN_ERROR
            Else
                UpdateArCreditmemoStatus = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            UpdateArCreditmemoStatus = RTN_ERROR
        End Try
    End Function

    Function ARCreditMemo(ByRef oHoldingCompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, ByVal APCreditmemoDocentry As String, ByVal ARDocEntry As String, _
                 ByVal sBaseType As String, ByRef sErrDesc As String) As Long

        'Function   :   ARCreditMemo()
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

        ''----------------------------------  From AP Credit memo to AR Credit Memo

        Dim sFuncName As String = String.Empty
        Dim lRetCode As Long
        Dim sDocEntry As String = String.Empty

        Try
            sFuncName = "ARCreditMemo()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            Dim oARInvoice As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices)
            Dim oARCreditmemo As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes)
            Dim oAPCreditmemo As SAPbobsCOM.Documents = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes)
            Dim oARReturn As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oReturns)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AP Creditcard Docentry " & APCreditmemoDocentry, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR Invoice Docentry " & ARDocEntry, sFuncName)


            Dim orset As SAPbobsCOM.Recordset = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            Dim orset1 As SAPbobsCOM.Recordset = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            orset1.DoQuery("SELECT (SELECT T10.[SeriesName] FROM NNM1 T10 WHERE T10.[Series]  = T0.[Series])  +  CAST(T0.DOCNUM AS NVARCHAR) AS [DOCNUM] FROM ORPC T0 where T0.DocEntry = '" & APCreditmemoDocentry & "'")

            If String.IsNullOrEmpty(ARDocEntry) Then
                sErrDesc = "No AR Credit Memo created.  Please manually create AR Credit Memo"
                Return RTN_ERROR
            End If

            If sBaseType = "AP Invoice" Then
                If oAPCreditmemo.GetByKey(APCreditmemoDocentry) Then
                    If oARInvoice.GetByKey(ARDocEntry) Then


                        oARCreditmemo.DocObjectCode = SAPbobsCOM.BoObjectTypes.oInvoices

                        oARCreditmemo.CardCode = oARInvoice.CardCode
                        oARCreditmemo.NumAtCard = orset1.Fields.Item("DOCNUM").Value
                        oARCreditmemo.DocDate = oAPCreditmemo.DocDate
                        oARCreditmemo.DocDueDate = oAPCreditmemo.DocDueDate
                        oARCreditmemo.TaxDate = oAPCreditmemo.TaxDate
                        oARCreditmemo.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items

                        oARCreditmemo.Rounding = oAPCreditmemo.Rounding
                        If oAPCreditmemo.RoundingDiffAmountFC <> 0 Then
                            oARCreditmemo.RoundingDiffAmount = oAPCreditmemo.RoundingDiffAmountFC
                        Else
                            oARCreditmemo.RoundingDiffAmount = oAPCreditmemo.RoundingDiffAmountSC
                        End If


                        For IntRow As Integer = 0 To oAPCreditmemo.Lines.Count - 1
                            oAPCreditmemo.Lines.SetCurrentLine(IntRow)

                            orset.DoQuery("SELECT T0.[LineNum], T0.[Price],T0.[whscode], T0.[OcrCode] FROM INV1 T0 " & _
                                       "WHERE T0.[DocEntry] = '" & oAPCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefDocEntry").Value & "' and  T0.[LineNum] = '" & oAPCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefBaseLine").Value & "'")

                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Fetching Line Number " & "SELECT T0.[LineNum], T0.[whscode], T0.[OcrCode] FROM INV1 T0 " & _
                                        "WHERE T0.[DocEntry] = '" & oAPCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefDocEntry").Value & "' and  T0.[LineNum] = '" & oAPCreditmemo.Lines.UserFields.Fields.Item("U_AE_RefBaseLine").Value & "'", sFuncName)

                            If String.IsNullOrEmpty(orset.Fields.Item("LineNum").Value) Then
                                ' sErrDesc = "Base line number and Base docentry is empty "
                                sErrDesc = "No AR Credit Memo created.  Please manually create AR Credit Memo"
                                Return RTN_ERROR
                            End If

                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR Invoice " & ARDocEntry & " Base Line " & orset.Fields.Item("LineNum").Value & " Quantity " & oAPCreditmemo.Lines.Quantity & " Price " & orset.Fields.Item("Price").Value, sFuncName)
                            oARCreditmemo.Lines.BaseEntry = ARDocEntry
                            oARCreditmemo.Lines.BaseLine = orset.Fields.Item("LineNum").Value
                            oARCreditmemo.Lines.BaseType = 13
                            'oARCreditmemo.Lines.ItemCode = oAPCreditmemo.Lines.ItemCode
                            oARCreditmemo.Lines.Quantity = oAPCreditmemo.Lines.Quantity
                            oARCreditmemo.Lines.UnitPrice = orset.Fields.Item("Price").Value
                            oARCreditmemo.Lines.WarehouseCode = orset.Fields.Item("whscode").Value
                            oARCreditmemo.Lines.CostingCode = orset.Fields.Item("OcrCode").Value
                            oARCreditmemo.Lines.Add()
                        Next IntRow

                        oARCreditmemo.UserFields.Fields.Item("U_AE_Docentry").Value = APCreditmemoDocentry
                    End If
                End If
            ElseIf sBaseType = "AP Return" Then
                If oAPCreditmemo.GetByKey(APCreditmemoDocentry) Then
                    If oARReturn.GetByKey(ARDocEntry) Then

                        oARCreditmemo.DocObjectCode = SAPbobsCOM.BoObjectTypes.oReturns

                        oARCreditmemo.CardCode = oARReturn.CardCode
                        oARCreditmemo.NumAtCard = orset1.Fields.Item("DOCNUM").Value
                        oARCreditmemo.DocDate = oAPCreditmemo.DocDate
                        oARCreditmemo.DocDueDate = oAPCreditmemo.DocDueDate
                        oARCreditmemo.TaxDate = oAPCreditmemo.TaxDate
                        oARCreditmemo.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items

                        oARCreditmemo.Rounding = oAPCreditmemo.Rounding
                        If oAPCreditmemo.RoundingDiffAmountFC <> 0 Then
                            oARCreditmemo.RoundingDiffAmount = oAPCreditmemo.RoundingDiffAmountFC
                        Else
                            oARCreditmemo.RoundingDiffAmount = oAPCreditmemo.RoundingDiffAmountSC
                        End If


                        For IntRow As Integer = 0 To oAPCreditmemo.Lines.Count - 1
                            oAPCreditmemo.Lines.SetCurrentLine(IntRow)

                            orset.DoQuery("SELECT T0.[LineNum], T0.[Price],T0.[whscode], T0.[OcrCode] FROM RDN1 T0 " & _
                                       "WHERE T0.[U_AE_RefDocEntry] = '" & oAPCreditmemo.Lines.BaseEntry & "' and  T0.[U_AE_RefBaseLine] = '" & oAPCreditmemo.Lines.BaseLine & "'")

                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("SELECT T0.[LineNum], T0.[Price],T0.[whscode], T0.[OcrCode] FROM RDN1 T0 " & _
                                       "WHERE T0.[U_AE_RefDocEntry] = '" & oAPCreditmemo.Lines.BaseEntry & "' and  T0.[U_AE_RefBaseLine] = '" & oAPCreditmemo.Lines.BaseLine & "'", sFuncName)

                            If String.IsNullOrEmpty(orset.Fields.Item("LineNum").Value) Then
                                ' sErrDesc = "Base line number and Base docentry is empty "
                                sErrDesc = "No AR Credit Memo created.  Please manually create AR Credit Memo"
                                Return RTN_ERROR
                            End If

                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR Invoice " & ARDocEntry & " Base Line " & orset.Fields.Item("LineNum").Value & " Quantity " & oAPCreditmemo.Lines.Quantity & " Price " & orset.Fields.Item("Price").Value, sFuncName)
                            oARCreditmemo.Lines.BaseEntry = ARDocEntry
                            oARCreditmemo.Lines.BaseLine = orset.Fields.Item("LineNum").Value
                            oARCreditmemo.Lines.BaseType = 16
                            'oARCreditmemo.Lines.ItemCode = oAPCreditmemo.Lines.ItemCode
                            oARCreditmemo.Lines.Quantity = oAPCreditmemo.Lines.Quantity
                            oARCreditmemo.Lines.UnitPrice = orset.Fields.Item("Price").Value
                            oARCreditmemo.Lines.WarehouseCode = orset.Fields.Item("whscode").Value
                            oARCreditmemo.Lines.CostingCode = orset.Fields.Item("OcrCode").Value
                            oARCreditmemo.Lines.Add()
                        Next IntRow

                        oARCreditmemo.UserFields.Fields.Item("U_AE_Docentry").Value = APCreditmemoDocentry
                    End If
                End If
            End If

            

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Adding ", sFuncName)

            lRetCode = oARCreditmemo.Add
            If lRetCode <> 0 Then
                sErrDesc = oTradingCompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                ARCreditMemo = RTN_ERROR
            Else
                oTradingCompany.GetNewObjectCode(sDocEntry)
                sErrDesc = sDocEntry
                ARCreditMemo = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            ARCreditMemo = RTN_ERROR
        End Try
    End Function

    Function UpdateApCreditmemoStatus(ByRef oDICompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, ByVal ARCreditDocEntry As String, _
                   ByVal APCreditDocEntry As String, ByVal sError As String, ByVal sStatus As String, ByVal sTable As String, ByRef sErrDesc As String) As Long

        'Function   :   UpdateApCreditmemoStatus()
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
            sFuncName = "UpdateApCreditmemoStatus()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            'Dim oARCreditmemo As SAPbobsCOM.Documents = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes)
            Dim oAPCreditmemo As SAPbobsCOM.Documents = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AP CreditNote Docentry " & APCreditDocEntry, sFuncName)

            Dim orset As SAPbobsCOM.Recordset = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            orset.DoQuery("SELECT (SELECT T10.[SeriesName] FROM NNM1 T10 WHERE T10.[Series]  = T0.[Series]) + cast(docnum as nvarchar) as [DocNum] FROM " & sTable & " T0 WHERE T0.[Docentry] = '" & ARCreditDocEntry & "'")

            If sError.Length >= 198 Then
                sError = sError.ToString.Substring(0, 198)
            End If

            If oAPCreditmemo.GetByKey(APCreditDocEntry) Then
                oAPCreditmemo.NumAtCard = orset.Fields.Item("DocNum").Value
                oAPCreditmemo.UserFields.Fields.Item("U_AE_Docentry").Value = ARCreditDocEntry
                oAPCreditmemo.UserFields.Fields.Item("U_AE_Istatus").Value = sStatus
                If sStatus = "Fail" Then
                    oAPCreditmemo.UserFields.Fields.Item("U_AE_IErrorMsg").Value = sError
                Else
                    oAPCreditmemo.UserFields.Fields.Item("U_AE_IErrorMsg").Value = ""
                End If
                oAPCreditmemo.UserFields.Fields.Item("U_AE_IDate").Value = Now.Date
            End If

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Updating ", sFuncName)

            lRetCode = oAPCreditmemo.Update
            If lRetCode <> 0 Then
                sErrDesc = oDICompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                UpdateApCreditmemoStatus = RTN_ERROR
            Else
                UpdateApCreditmemoStatus = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            UpdateApCreditmemoStatus = RTN_ERROR
        End Try
    End Function



End Module
