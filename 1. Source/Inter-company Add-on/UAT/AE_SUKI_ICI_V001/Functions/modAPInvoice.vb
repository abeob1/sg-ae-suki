Imports System.Data.SqlClient
Imports System.Data.OleDb


Module modAPInvoice



    Function APInvoice(ByRef oHoldingCompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, _
                    ByVal oDT_GRPODetails As DataTable, ByVal dARDocTotal As Double, ByRef sErrDesc As String) As Long

        'Function   :   APInvoice()
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
        Dim orset As SAPbobsCOM.Recordset = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim orset1 As SAPbobsCOM.Recordset = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        Dim oDV_GRPODetails As DataView = Nothing
        Dim oDT_Distinct_GRPODocEntry As DataTable = Nothing

        Dim sGRPO_DocEntry As String = String.Empty
        Dim sARINV_DocEntry As String = String.Empty
        Dim sARINV_DocNum As String = String.Empty
        Dim dvariance As Double = 0.0
        Dim dAPInvoiceDocTotal As Double = 0.0
        Dim oARInvoice, oAPInvoice, oGRPO As SAPbobsCOM.Documents

        Try
            sFuncName = "APInvoice()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)
            If Get_APInvoiceDocTotal(oHoldingCompany, oTradingCompany, oDT_GRPODetails, dARDocTotal, dAPInvoiceDocTotal, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)

            dvariance = dARDocTotal - dAPInvoiceDocTotal

            sARINV_DocEntry = oDT_GRPODetails.Rows(0).Item(0).ToString()
            sARINV_DocNum = oDT_GRPODetails.Rows(0).Item(3).ToString()

            oDV_GRPODetails = oDT_GRPODetails.DefaultView
            oDT_Distinct_GRPODocEntry = oDT_GRPODetails.DefaultView.ToTable(True, "GRPODocEntry")

            oARInvoice = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices)
            oAPInvoice = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices)
            oGRPO = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes)

            If oARInvoice.GetByKey(sARINV_DocEntry) Then

                oAPInvoice.CardCode = p_oCompDef.TradingVendor
                oAPInvoice.DocDate = oARInvoice.DocDate
                oAPInvoice.DocDueDate = oARInvoice.DocDueDate
                oAPInvoice.TaxDate = oARInvoice.TaxDate
                oAPInvoice.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items

                oAPInvoice.NumAtCard = sARINV_DocNum
                oAPInvoice.UserFields.Fields.Item("U_AE_Docentry").Value = sARINV_DocEntry

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

                            oAPInvoice.Lines.BaseType = 20
                            oAPInvoice.Lines.BaseEntry = Convert.ToInt32(drv(6).ToString.Trim)
                            oAPInvoice.Lines.BaseLine = Convert.ToInt32(drv(7).ToString.Trim)
                            oAPInvoice.Lines.Quantity = Convert.ToDouble(drv(10).ToString.Trim)
                            oAPInvoice.Lines.WarehouseCode = drv(8).ToString.Trim
                            oAPInvoice.Lines.CostingCode = drv(9).ToString.Trim
                            oAPInvoice.Lines.UserFields.Fields.Item("U_AE_RefDocEntry").Value = drv(0).ToString.Trim
                            oAPInvoice.Lines.UserFields.Fields.Item("U_AE_RefBaseLine").Value = drv(1).ToString.Trim
                            oAPInvoice.Lines.Add()
                        End If
                    Next
                Next imjs
            End If

            If dvariance <> 0 Then
                If oAPInvoice.Rounding = SAPbobsCOM.BoYesNoEnum.tYES Then
                    If oARInvoice.RoundingDiffAmountFC <> 0 Then
                        oAPInvoice.RoundingDiffAmount = oARInvoice.RoundingDiffAmountFC + dvariance
                    ElseIf oARInvoice.RoundingDiffAmountSC <> 0 Then
                        oAPInvoice.RoundingDiffAmount = oARInvoice.RoundingDiffAmountSC + dvariance
                    Else
                        oAPInvoice.RoundingDiffAmount = oARInvoice.RoundingDiffAmount + dvariance
                    End If
                Else
                    oAPInvoice.Rounding = SAPbobsCOM.BoYesNoEnum.tYES
                    oAPInvoice.RoundingDiffAmount = dvariance
                End If

            Else
                oAPInvoice.Rounding = oARInvoice.Rounding
                If oARInvoice.RoundingDiffAmountFC <> 0 Then
                    oAPInvoice.RoundingDiffAmount = oARInvoice.RoundingDiffAmountFC
                ElseIf oARInvoice.RoundingDiffAmountSC <> 0 Then
                    oAPInvoice.RoundingDiffAmount = oARInvoice.RoundingDiffAmountSC
                Else
                    oAPInvoice.RoundingDiffAmount = oARInvoice.RoundingDiffAmount
                End If
            End If

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Adding ", sFuncName)
            lRetCode = oAPInvoice.Add
            If lRetCode <> 0 Then
                sErrDesc = oTradingCompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                APInvoice = RTN_ERROR
            Else
                oTradingCompany.GetNewObjectCode(sDocEntry)
                sErrDesc = sDocEntry
                APInvoice = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

        Catch ex As Exception
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            APInvoice = RTN_ERROR
        End Try
    End Function

    Function UpdateArInvoiceStatus(ByRef oDICompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, ByVal ARDocEntry As String, _
                   ByVal APDocEntry As String, ByVal sError As String, ByVal sStatus As String, ByVal sTable As String, ByRef sErrDesc As String) As Long

        'Function   :   UpdateArInvoiceStatus()
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
        Dim orset As SAPbobsCOM.Recordset = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        Try
            sFuncName = "UpdateArInvoiceStatus()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            Dim oARInvoice As SAPbobsCOM.Documents = oDICompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR Invoice DocEntry " & ARDocEntry, sFuncName)
            orset.DoQuery("SELECT (SELECT T10.[SeriesName] FROM NNM1 T10 WHERE T10.[Series]  = T0.[Series]) + cast(docnum as nvarchar) as [DocNum] FROM "& sTable &" T0 WHERE T0.[Docentry] = '" & APDocEntry & "'")
            If sError.Length >= 198 Then
                sError = sError.ToString.Substring(0, 198)
            End If

            If oARInvoice.GetByKey(ARDocEntry) Then

                oARInvoice.UserFields.Fields.Item("U_AE_Docentry").Value = APDocEntry
                oARInvoice.UserFields.Fields.Item("U_AE_Istatus").Value = sStatus
                If sStatus = "Fail" Then
                    oARInvoice.UserFields.Fields.Item("U_AE_IErrorMsg").Value = sError
                Else
                    oARInvoice.NumAtCard = orset.Fields.Item("DocNum").Value
                    oARInvoice.UserFields.Fields.Item("U_AE_IErrorMsg").Value = ""
                End If
                oARInvoice.UserFields.Fields.Item("U_AE_IDate").Value = Now.Date
            End If

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Updating ", sFuncName)
            lRetCode = oARInvoice.Update
            If lRetCode <> 0 Then
                sErrDesc = oDICompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                UpdateArInvoiceStatus = RTN_ERROR
            Else
                oDICompany.GetNewObjectCode(sDocEntry)
                sErrDesc = sDocEntry
                UpdateArInvoiceStatus = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            UpdateArInvoiceStatus = RTN_ERROR
        End Try
    End Function

    Function Get_APInvoiceDocTotal(ByRef oHoldingCompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, _
                   ByVal oDT_GRPODetails As DataTable, ByVal dARDocTotal As Double, ByRef dAPInvoiceDocTotal As Double, ByRef sErrDesc As String) As Long

        'Function   :   Get_APInvoiceDocTotal()
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
        Dim orset As SAPbobsCOM.Recordset = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
        Dim orset1 As SAPbobsCOM.Recordset = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

        Dim oDV_GRPODetails As DataView = Nothing
        Dim oDT_Distinct_GRPODocEntry As DataTable = Nothing

        Dim sGRPO_DocEntry As String = String.Empty
        Dim sARINV_DocEntry As String = String.Empty
        Dim sARINV_DocNum As String = String.Empty
        Dim dvariance As Double = 0.0


        Try
            sFuncName = "Get_APInvoiceDocTotal()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)
            Dim oARInvoice As SAPbobsCOM.Documents = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices)
            ' Dim oAPInvoice As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseInvoices)
            Dim oAPInvoice As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)
            Dim oGRPO As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes)

            sARINV_DocEntry = oDT_GRPODetails.Rows(0).Item(0).ToString()
            sARINV_DocNum = oDT_GRPODetails.Rows(0).Item(3).ToString()

            oDV_GRPODetails = oDT_GRPODetails.DefaultView
            oDT_Distinct_GRPODocEntry = oDT_GRPODetails.DefaultView.ToTable(True, "GRPODocEntry")

            If oARInvoice.GetByKey(sARINV_DocEntry) Then

                oAPInvoice.CardCode = p_oCompDef.TradingVendor
                oAPInvoice.DocDate = oARInvoice.DocDate
                oAPInvoice.DocDueDate = oARInvoice.DocDueDate
                oAPInvoice.TaxDate = oARInvoice.TaxDate
                oAPInvoice.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items
                oAPInvoice.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseInvoices

                oAPInvoice.Rounding = oARInvoice.Rounding
                If oARInvoice.RoundingDiffAmountFC <> 0 Then
                    oAPInvoice.RoundingDiffAmount = oARInvoice.RoundingDiffAmountFC
                ElseIf oARInvoice.RoundingDiffAmountSC <> 0 Then
                    oAPInvoice.RoundingDiffAmount = oARInvoice.RoundingDiffAmountSC
                Else
                    oAPInvoice.RoundingDiffAmount = oARInvoice.RoundingDiffAmount
                End If

                oAPInvoice.NumAtCard = sARINV_DocNum
                oAPInvoice.UserFields.Fields.Item("U_AE_Docentry").Value = sARINV_DocEntry

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

                            oAPInvoice.Lines.BaseType = 20
                            oAPInvoice.Lines.BaseEntry = Convert.ToInt32(drv(6).ToString.Trim)
                            oAPInvoice.Lines.BaseLine = Convert.ToInt32(drv(7).ToString.Trim)
                            oAPInvoice.Lines.Quantity = Convert.ToDouble(drv(10).ToString.Trim)
                            oAPInvoice.Lines.WarehouseCode = drv(8).ToString.Trim
                            oAPInvoice.Lines.CostingCode = drv(9).ToString.Trim
                            oAPInvoice.Lines.Add()
                        End If
                    Next
                Next imjs
            End If

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Adding ", sFuncName)

            If oTradingCompany.InTransaction = False Then
                oTradingCompany.StartTransaction()
            End If
            lRetCode = oAPInvoice.Add
            If lRetCode <> 0 Then
                sErrDesc = oTradingCompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                Get_APInvoiceDocTotal = RTN_ERROR

            Else
                oTradingCompany.GetNewObjectCode(sDocEntry)
                orset.DoQuery("SELECT T0.DocTotal  FROM ODRF T0 WHERE T0.DocEntry = " & sDocEntry & "")
                dAPInvoiceDocTotal = orset.Fields.Item("DocTotal").Value
                sErrDesc = sDocEntry
                Get_APInvoiceDocTotal = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

            If oTradingCompany.InTransaction = True Then
                oTradingCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack)
            End If
            oARInvoice = Nothing
            oAPInvoice = Nothing
            oGRPO = Nothing

        Catch ex As Exception
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Get_APInvoiceDocTotal = RTN_ERROR
        End Try
    End Function

    Function APInvoiceDraft(ByRef oHoldingCompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, ByVal BaseDocEntry As String, _
                   ByVal sWhsCode As String, ByRef sErrDesc As String) As Long

        'Function   :   APInvoiceDraft()
        'Purpose    :   Create a AP invoice through DI API
        'Parameters :   
        '               oHoldingCompany As SAPbobsCOM.Company
        '               oTradingCompany As SAPbobsCOM.Company
        '               BaseDocEntry As String
        '               sWhsCode As String
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
            sFuncName = "APInvoiceDraft()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            Dim oARInvoice As SAPbobsCOM.Documents = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices)
            Dim oAPInvoiceDraft As SAPbobsCOM.Documents = oTradingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Base DocEntry " & BaseDocEntry, sFuncName)

            Dim orset1 As SAPbobsCOM.Recordset = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
            orset1.DoQuery("SELECT (SELECT T10.[SeriesName] FROM NNM1 T10 WHERE T10.[Series]  = T0.[Series])  +  CAST(T0.DOCNUM AS NVARCHAR) AS [DOCNUM] FROM OINV T0 where T0.DocEntry = '" & BaseDocEntry & "'")


            If oARInvoice.GetByKey(BaseDocEntry) Then
                oAPInvoiceDraft.CardCode = p_oCompDef.TradingVendor
                oAPInvoiceDraft.NumAtCard = orset1.Fields.Item("DOCNUM").Value
                oAPInvoiceDraft.DocDate = oARInvoice.DocDate
                oAPInvoiceDraft.DocDueDate = oARInvoice.DocDueDate
                oAPInvoiceDraft.TaxDate = oARInvoice.TaxDate
                oAPInvoiceDraft.DocType = SAPbobsCOM.BoDocumentTypes.dDocument_Items
                oAPInvoiceDraft.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseInvoices

                oAPInvoiceDraft.Rounding = oARInvoice.Rounding
                If oARInvoice.RoundingDiffAmountFC <> 0 Then
                    oAPInvoiceDraft.RoundingDiffAmount = oARInvoice.RoundingDiffAmountFC
                ElseIf oARInvoice.RoundingDiffAmountSC <> 0 Then
                    oAPInvoiceDraft.RoundingDiffAmount = oARInvoice.RoundingDiffAmountSC
                Else
                    oAPInvoiceDraft.RoundingDiffAmount = oARInvoice.RoundingDiffAmount
                End If

                For IntRow As Integer = 0 To oARInvoice.Lines.Count - 1
                    oARInvoice.Lines.SetCurrentLine(IntRow)
                    oAPInvoiceDraft.Lines.ItemCode = oARInvoice.Lines.ItemCode
                    oAPInvoiceDraft.Lines.ItemDescription = oARInvoice.Lines.ItemDescription
                    oAPInvoiceDraft.Lines.Quantity = oARInvoice.Lines.Quantity
                    oAPInvoiceDraft.Lines.UnitPrice = oARInvoice.Lines.UnitPrice

                    Select Case oARInvoice.Lines.VatGroup

                        Case "SO"
                            oAPInvoiceDraft.Lines.VatGroup = "SI"
                        Case "SI"
                            oAPInvoiceDraft.Lines.VatGroup = "SO"
                        Case "ZO"
                            oAPInvoiceDraft.Lines.VatGroup = "ZI"
                        Case "ZI"
                            oAPInvoiceDraft.Lines.VatGroup = "ZO"

                    End Select

                    'oAPInvoiceDraft.Lines.WarehouseCode = "IM-RHM" 'sWhsCode 'oARInvoice.Lines.WarehouseCode
                    'oAPInvoiceDraft.Lines.CostingCode = "IM-RHM" 'sWhsCode 'oARInvoice.Lines.CostingCode
                    oAPInvoiceDraft.Lines.Add()
                Next IntRow

                oAPInvoiceDraft.DiscountPercent = oARInvoice.DiscountPercent
                oAPInvoiceDraft.UserFields.Fields.Item("U_AE_Docentry").Value = BaseDocEntry
                oAPInvoiceDraft.UserFields.Fields.Item("U_AE_IFlag").Value = "Y"
            End If

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Before Adding ", sFuncName)

            lRetCode = oAPInvoiceDraft.Add
            If lRetCode <> 0 Then
                sErrDesc = oTradingCompany.GetLastErrorDescription
                Call WriteToLogFile(sErrDesc, sFuncName)
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                APInvoiceDraft = RTN_ERROR
            Else
                oTradingCompany.GetNewObjectCode(sDocEntry)
                sErrDesc = sDocEntry
                APInvoiceDraft = RTN_SUCCESS
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS : " & sDocEntry, sFuncName)
            End If

        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            APInvoiceDraft = RTN_ERROR
        End Try
    End Function

    Public Function GetDelivery_Details(ByRef oHoldingCompany As SAPbobsCOM.Company, ByRef oTradingCompany As SAPbobsCOM.Company, ByVal sDocEntry As String, ByVal sHeaderTable As String, _
                                        ByVal sLineTable As String, ByRef sErrDesc As String) As DataTable

        ' **********************************************************************************
        '   Function    :   GetDelivery_Details()
        '   Purpose     :   This function will extract the company information from database HSuki Group in Data view
        '   Parameters  :   ByRef sErrDesc AS String 
        '                       
        '   Author      :   JOHN
        '   Date        :   SEP 2014 26
        ' **********************************************************************************
        Dim sFuncName As String = String.Empty
        Dim oDT_DeliveryDetails As DataTable = Nothing
        Dim sQry As String = String.Empty
        Dim oRset As SAPbobsCOM.Recordset = Nothing

        Try

            sFuncName = "GetDelivery_Details()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)
            oRset = oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

            sQry = "select T0.[DocEntry] [Invoice DocEntry], T0.[LineNum] [Invoice LineNum], T0.[DocDate] [DocDate],T3.[SeriesName] + convert(varchar,T2.[DocNum]) [Invoice DocNum]," & _
           " T0.[BaseEntry] [Delivery DocEntry], T0.[BaseLine] [Delivery LineNum], " & _
           " T1.[DocEntry] [GRPODocEntry], T1.[LineNum] [GRPO LineNum], T1.[whscode] [Warehouse], T1.[OcrCode] [Cost Center], T0.[Quantity] [Invoice Quantity] " & _
           " from " & oHoldingCompany.CompanyDB & ".. " & sLineTable & " T0  " & _
           " inner join " & oHoldingCompany.CompanyDB & ".. " & sHeaderTable & " T2 on T2.DocEntry = T0.DocEntry " & _
           " inner join " & oHoldingCompany.CompanyDB & ".. NNM1 T3 on T3.[Series] = T2.[Series] " & _
           " inner join " & oTradingCompany.CompanyDB & ".. PDN1 T1 on T0.[BaseEntry] = T1.[U_AE_RefDocEntry] and T0.[BaseLine] = T1.[U_AE_RefBaseLine] " & _
           " where T0.DocEntry = '" & sDocEntry & "'"

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Query Statement for fetching the Delivery Details ", sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug(sQry, sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Function ConvertRecordset()", sFuncName)
            '' oDT_DeliveryDetails = ReturnQuery_InDatatable(sQry, sErrDesc)
            oRset.DoQuery(sQry)
            oDT_DeliveryDetails = ConvertRecordset(oRset, sErrDesc)

            If sErrDesc.Length > 0 Then
                Exit Function
            End If

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed With SUCCESS", sFuncName)

            sErrDesc = String.Empty
            Return oDT_DeliveryDetails
        Catch ex As Exception
            sErrDesc = ex.Message
            p_oSBOApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed With ERROR  " & ex.Message, sFuncName)
            Call WriteToLogFile(ex.Message, sFuncName)
        Finally
            oRset = Nothing
        End Try

    End Function


End Module
