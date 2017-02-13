Option Explicit On
'Imports SAPbouiCOM.Framework
Imports System.Windows.Forms


Public Class clsEventHandler
    Dim WithEvents SBO_Application As SAPbouiCOM.Application ' holds connection with SBO
    'Dim p_oDICompany As New SAPbobsCOM.Company

    Public Sub New(ByRef oApplication As SAPbouiCOM.Application, ByRef oCompany As SAPbobsCOM.Company)
        Dim sFuncName As String = String.Empty
        Try
            sFuncName = "Class_Initialize()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting function", sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Retriving SBO Application handle", sFuncName)
            SBO_Application = oApplication
            p_oHoldingCompany = oCompany

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
        Catch exc As Exception
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            Call WriteToLogFile(exc.Message, sFuncName)
        End Try
    End Sub

    Public Function SetApplication(ByRef sErrDesc As String) As Long
        ' **********************************************************************************
        '   Function   :    SetApplication()
        '   Purpose    :    This function will be calling to initialize the default settings
        '                   such as Retrieving the Company Default settings, Creating Menus, and
        '                   Initialize the Event Filters
        '               
        '   Parameters :    ByRef sErrDesc AS string
        '                       sErrDesc = Error Description to be returned to calling function
        '   Return     :    0 - FAILURE
        '                   1 - SUCCESS
        ' **********************************************************************************
        Dim sFuncName As String = String.Empty

        Try
            sFuncName = "SetApplication()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling SetMenus()", sFuncName)
            If SetMenus(sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling SetFilters()", sFuncName)
            If SetFilters(sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
            SetApplication = RTN_SUCCESS
        Catch exc As Exception
            sErrDesc = exc.Message
            Call WriteToLogFile(exc.Message, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            SetApplication = RTN_ERROR
        End Try
    End Function

    Private Function SetMenus(ByRef sErrDesc As String) As Long
        ' **********************************************************************************
        '   Function   :    SetMenus()
        '   Purpose    :    This function will be gathering to create the customized menu
        '               
        '   Parameters :    ByRef sErrDesc AS string
        '                       sErrDesc = Error Description to be returned to calling function
        '   Return     :    0 - FAILURE
        '                   1 - SUCCESS
        ' **********************************************************************************
        Dim sFuncName As String = String.Empty
        ' Dim oMenuItem As SAPbouiCOM.MenuItem
        Try
            sFuncName = "SetMenus()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Function", sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
            SetMenus = RTN_SUCCESS
        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(ex.Message, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            SetMenus = RTN_ERROR
        End Try
    End Function

    Private Function SetFilters(ByRef sErrDesc As String) As Long

        ' **********************************************************************************
        '   Function   :    SetFilters()
        '   Purpose    :    This function will be gathering to declare the event filter 
        '                   before starting the AddOn Application
        '               
        '   Parameters :    ByRef sErrDesc AS string
        '                       sErrDesc = Error Description to be returned to calling function
        '   Return     :    0 - FAILURE
        '                   1 - SUCCESS
        ' **********************************************************************************

        Dim oFilters As SAPbouiCOM.EventFilters
        Dim oFilter As SAPbouiCOM.EventFilter
        Dim sFuncName As String = String.Empty

        Try
            sFuncName = "SetFilters()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting function", sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Initializing EventFilters object", sFuncName)
            oFilters = New SAPbouiCOM.EventFilters

           

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Adding filters", sFuncName)
            SBO_Application.SetFilter(oFilters)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
            SetFilters = RTN_SUCCESS
        Catch exc As Exception
            sErrDesc = exc.Message
            Call WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            SetFilters = RTN_ERROR
        End Try
    End Function

    Private Sub SBO_Application_AppEvent(ByVal EventType As SAPbouiCOM.BoAppEventTypes) Handles SBO_Application.AppEvent
        ' **********************************************************************************
        '   Function   :    SBO_Application_AppEvent()
        '   Purpose    :    This function will be handling the SAP Application Event
        '               
        '   Parameters :    ByVal EventType As SAPbouiCOM.BoAppEventTypes
        '                       EventType = set the SAP UI Application Eveny Object        
        ' **********************************************************************************
        Dim sFuncName As String = String.Empty
        Dim sErrDesc As String = String.Empty
        Dim sMessage As String = String.Empty

        Try
            sFuncName = "SBO_Application_AppEvent()"
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting function", sFuncName)

            Select Case EventType
                Case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged, SAPbouiCOM.BoAppEventTypes.aet_ShutDown, SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition
                    sMessage = String.Format("Please wait for a while to disconnect the AddOn {0} ....", System.Windows.Forms.Application.ProductName)
                    p_oSBOApplication.SetStatusBarMessage(sMessage, SAPbouiCOM.BoMessageTime.bmt_Medium, False)
                    End
            End Select

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
        Catch ex As Exception
            sErrDesc = ex.Message
            WriteToLogFile(sErrDesc, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            ShowErr(sErrDesc)
        Finally
            ' GC.Collect()  'Forces garbage collection of all generations.
        End Try
    End Sub

    Private Sub SBO_Application_MenuEvent(ByRef pVal As SAPbouiCOM.MenuEvent, ByRef BubbleEvent As Boolean) Handles SBO_Application.MenuEvent
        ' **********************************************************************************
        '   Function   :    SBO_Application_MenuEvent()
        '   Purpose    :    This function will be handling the SAP Menu Event
        '               
        '   Parameters :    ByRef pVal As SAPbouiCOM.MenuEvent
        '                       pVal = set the SAP UI MenuEvent Object
        '                   ByRef BubbleEvent As Boolean
        '                       BubbleEvent = set the True/False        
        ' **********************************************************************************
        ''Dim oForm As SAPbouiCOM.Form = Nothing
        ''Dim sErrDesc As String = String.Empty
        ''Dim sFuncName As String = String.Empty

        ''Try
        ''    sFuncName = "SBO_Application_MenuEvent()"
        ''    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting function", sFuncName)

        ''    If Not p_oDICompany.Connected Then
        ''        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectDICompSSO()", sFuncName)
        ''        If ConnectDICompSSO(p_oDICompany, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
        ''    End If

        ''    If pVal.BeforeAction = False Then
        ''        Select Case pVal.MenuUID
        ''            Case "JE"
        ''                Try
        ''                    LoadFromXML("JournalEntry.srf", SBO_Application)
        ''                    oForm = SBO_Application.Forms.Item("JournalE")

        ''                    oForm.Visible = True
        ''                    Exit Try

        ''                Catch ex As Exception
        ''                    SBO_Application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
        ''                    BubbleEvent = False
        ''                End Try
        ''                Exit Sub

        ''        End Select
        ''    End If

        ''    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
        ''Catch exc As Exception
        ''    BubbleEvent = False
        ''    ShowErr(exc.Message)
        ''    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
        ''    WriteToLogFile(Err.Description, sFuncName)
        ''End Try
    End Sub

    Private Sub SBO_Application_ItemEvent(ByVal FormUID As String, ByRef pVal As SAPbouiCOM.ItemEvent, _
            ByRef BubbleEvent As Boolean) Handles SBO_Application.ItemEvent
        ' **********************************************************************************
        '   Function   :    SBO_Application_ItemEvent()
        '   Purpose    :    This function will be handling the SAP Menu Event
        '               
        '   Parameters :    ByVal FormUID As String
        '                       FormUID = set the FormUID
        '                   ByRef pVal As SAPbouiCOM.ItemEvent
        '                       pVal = set the SAP UI ItemEvent Object
        '                   ByRef BubbleEvent As Boolean
        '                       BubbleEvent = set the True/False        
        ' **********************************************************************************

        Dim sErrDesc As String = String.Empty
        Dim sFuncName As String = String.Empty
        Dim p_oDVJE As DataView = Nothing
        Dim oDTDistinct As DataTable = Nothing
        Dim oDTRowFilter As DataTable = Nothing

        Try


            If Not IsNothing(p_oHoldingCompany) Then
                If Not p_oHoldingCompany.Connected Then
                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectDICompSSO()", sFuncName)
                    If ConnectDICompSSO(p_oHoldingCompany, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                End If
            End If

            If pVal.BeforeAction = False Then

                Select Case pVal.FormTypeEx


                End Select
            Else
                Select Case pVal.FormTypeEx

                    Case "141" ' AP Invoice
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED And pVal.FormMode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                            If pVal.ItemUID = "1" Then
                                sFuncName = "Item Pressed AP Invoice()"
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Item ID 1", sFuncName)

                                Dim oForm As SAPbouiCOM.Form = p_oSBOApplication.Forms.GetFormByTypeAndCount(141, pVal.FormTypeCount)
                                oFormType = 0
                                oFormType = pVal.FormTypeCount
                                p_oCompDef.TradingCompany = String.Empty
                                p_oCompDef.TradingConnection = String.Empty
                                Dim sVendor As String = String.Empty
                                Try
                                    sVendor = oForm.Items.Item("4").Specific.String
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Vendor Name " & sVendor, sFuncName)

                                    oCompanydetailsDV.RowFilter = "U_VendorCode = '" & sVendor & "'"
                                    If oCompanydetailsDV.Count > 0 Then
                                        p_oCompDef.TradingCompany = oCompanydetailsDV.Item(0).Row(2).ToString
                                        p_oCompDef.TradingConnection = oCompanydetailsDV.Item(0).Row(7).ToString
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Company " & p_oCompDef.TradingCompany, sFuncName)
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Connection " & p_oCompDef.TradingConnection, sFuncName)
                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName)
                                Catch ex As Exception
                                    sErrDesc = ex.Message
                                    Call WriteToLogFile(ex.Message, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                End Try
                                Exit Sub
                            End If
                        End If

                    Case "133" ' AR Invoice
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED And pVal.FormMode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                            If pVal.ItemUID = "1" Then

                                sFuncName = "Item Pressed AR Invoice()"
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Item ID 1", sFuncName)

                                Dim oForm As SAPbouiCOM.Form = p_oSBOApplication.Forms.GetFormByTypeAndCount(133, pVal.FormTypeCount)
                                oFormType = 0
                                oFormType = pVal.FormTypeCount
                                p_oCompDef.TradingCompany = String.Empty
                                p_oCompDef.TradingConnection = String.Empty
                                Dim sCustomer As String = String.Empty
                                Try
                                    sCustomer = oForm.Items.Item("4").Specific.String
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Customer Name " & sCustomer, sFuncName)

                                    oCompanydetailsDV.RowFilter = "U_CustomerCode = '" & sCustomer & "'"
                                    If oCompanydetailsDV.Count > 0 Then
                                        p_oCompDef.TradingCompany = oCompanydetailsDV.Item(0).Row(2).ToString
                                        p_oCompDef.TradingConnection = oCompanydetailsDV.Item(0).Row(7).ToString
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Company " & p_oCompDef.TradingCompany, sFuncName)
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Connection " & p_oCompDef.TradingConnection, sFuncName)
                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName)
                                Catch ex As Exception
                                    sErrDesc = ex.Message
                                    Call WriteToLogFile(ex.Message, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                End Try
                                ' oForm.Items.Item("Item_5").Specific.string = p_sSelectedFilepath
                                Exit Sub
                            End If
                        End If

                    Case "179" ' AR Credit memo
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED And pVal.FormMode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                            If pVal.ItemUID = "1" Then
                                Dim oForm As SAPbouiCOM.Form = p_oSBOApplication.Forms.GetFormByTypeAndCount(179, pVal.FormTypeCount)
                                oFormType = 0
                                oFormType = pVal.FormTypeCount
                                p_oCompDef.TradingCompany = String.Empty
                                p_oCompDef.TradingConnection = String.Empty
                                Dim sCustomer As String = String.Empty
                                Try
                                    sFuncName = "Item Pressed AR Credit memo()"
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Item ID 1", sFuncName)

                                    sCustomer = oForm.Items.Item("4").Specific.String
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Customer Name " & sCustomer, sFuncName)

                                    oCompanydetailsDV.RowFilter = "U_CustomerCode = '" & sCustomer & "'"
                                    If oCompanydetailsDV.Count > 0 Then
                                        p_oCompDef.TradingCompany = oCompanydetailsDV.Item(0).Row(2).ToString
                                        p_oCompDef.TradingConnection = oCompanydetailsDV.Item(0).Row(7).ToString

                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Company " & p_oCompDef.TradingCompany, sFuncName)
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Connection " & p_oCompDef.TradingConnection, sFuncName)
                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName)
                                Catch ex As Exception
                                    sErrDesc = ex.Message
                                    Call WriteToLogFile(ex.Message, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                End Try

                                ' oForm.Items.Item("Item_5").Specific.string = p_sSelectedFilepath
                                Exit Sub
                            End If
                        End If

                    Case "181" ' AP Credit memo
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED And pVal.FormMode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                            If pVal.ItemUID = "1" Then
                                Dim oForm As SAPbouiCOM.Form = p_oSBOApplication.Forms.GetFormByTypeAndCount(181, pVal.FormTypeCount)
                                oFormType = 0
                                oFormType = pVal.FormTypeCount
                                p_oCompDef.TradingCompany = String.Empty
                                p_oCompDef.TradingConnection = String.Empty
                                Dim sVendor As String = String.Empty
                                Try
                                    sFuncName = "Item Pressed AP Credit memo()"
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Item ID 1", sFuncName)
                                    sVendor = oForm.Items.Item("4").Specific.String
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Vendor Name " & sVendor, sFuncName)

                                    oCompanydetailsDV.RowFilter = "U_VendorCode = '" & sVendor & "'"
                                    If oCompanydetailsDV.Count > 0 Then
                                        p_oCompDef.TradingCompany = oCompanydetailsDV.Item(0).Row(2).ToString
                                        p_oCompDef.TradingConnection = oCompanydetailsDV.Item(0).Row(7).ToString
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Company " & p_oCompDef.TradingCompany, sFuncName)
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Connection " & p_oCompDef.TradingConnection, sFuncName)
                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName)
                                Catch ex As Exception
                                    sErrDesc = ex.Message
                                    Call WriteToLogFile(ex.Message, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                End Try

                                ' oForm.Items.Item("Item_5").Specific.string = p_sSelectedFilepath
                                Exit Sub
                            End If
                        End If

                    Case "180" ' Sales return
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED And pVal.FormMode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                            If pVal.ItemUID = "1" Then
                                Dim oForm As SAPbouiCOM.Form = p_oSBOApplication.Forms.GetFormByTypeAndCount(180, pVal.FormTypeCount)
                                oFormType = 0
                                oFormType = pVal.FormTypeCount
                                p_oCompDef.TradingCompany = String.Empty
                                p_oCompDef.TradingConnection = String.Empty
                                Dim sCustomer As String = String.Empty
                                Try
                                    sFuncName = "Item Pressed AR Sales Return()"
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Item ID 1", sFuncName)
                                    sCustomer = oForm.Items.Item("4").Specific.String
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Customer Name " & sCustomer, sFuncName)
                                    oCompanydetailsDV.RowFilter = "U_CustomerCode = '" & sCustomer & "'"

                                    If oCompanydetailsDV.Count > 0 Then
                                        p_oCompDef.TradingCompany = oCompanydetailsDV.Item(0).Row(2).ToString
                                        p_oCompDef.TradingConnection = oCompanydetailsDV.Item(0).Row(7).ToString
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Company " & p_oCompDef.TradingCompany, sFuncName)
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Connection " & p_oCompDef.TradingConnection, sFuncName)

                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName)
                                Catch ex As Exception
                                    sErrDesc = ex.Message
                                    Call WriteToLogFile(ex.Message, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                End Try

                                ' oForm.Items.Item("Item_5").Specific.string = p_sSelectedFilepath
                                Exit Sub
                            End If
                        End If

                    Case "182" ' Purchase return
                        If pVal.EventType = SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED And pVal.FormMode <> SAPbouiCOM.BoFormMode.fm_FIND_MODE Then
                            If pVal.ItemUID = "1" Then
                                Dim oForm As SAPbouiCOM.Form = p_oSBOApplication.Forms.GetFormByTypeAndCount(182, pVal.FormTypeCount)
                                oFormType = 0
                                oFormType = pVal.FormTypeCount
                                p_oCompDef.TradingCompany = String.Empty
                                p_oCompDef.TradingConnection = String.Empty
                                Dim sVendor As String = String.Empty
                                Try
                                    sFuncName = "Getting the Trading company Detail"
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Item Event - AP Return", sFuncName)
                                    sVendor = oForm.Items.Item("4").Specific.String
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Vendor Name " & sVendor, sFuncName)

                                    oCompanydetailsDV.RowFilter = "U_VendorCode = '" & sVendor & "'"
                                    If oCompanydetailsDV.Count > 0 Then
                                        p_oCompDef.TradingCompany = oCompanydetailsDV.Item(0).Row(2).ToString
                                        p_oCompDef.TradingConnection = oCompanydetailsDV.Item(0).Row(7).ToString
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Company " & p_oCompDef.TradingCompany, sFuncName)
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Trading Connection " & p_oCompDef.TradingConnection, sFuncName)
                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed With SUCCESS ", sFuncName)
                                Catch ex As Exception
                                    sErrDesc = ex.Message
                                    Call WriteToLogFile(ex.Message, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                End Try

                                ' oForm.Items.Item("Item_5").Specific.string = p_sSelectedFilepath
                                Exit Sub
                            End If
                        End If

                End Select
            End If

        Catch exc As Exception
            BubbleEvent = False
            sErrDesc = exc.Message
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
            WriteToLogFile(Err.Description, sFuncName)
            ShowErr(sErrDesc)
        End Try

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub SBO_Application_FormDataEvent(ByRef BusinessObjectInfo As SAPbouiCOM.BusinessObjectInfo, ByRef BubbleEvent As Boolean) Handles SBO_Application.FormDataEvent
        Try

            Dim oForm As SAPbouiCOM.Form = Nothing
            If BusinessObjectInfo.ActionSuccess = True And BusinessObjectInfo.BeforeAction = False Then
                Select Case BusinessObjectInfo.FormTypeEx

                    Case "141" '------ AP Invoice

                        If BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE Then

                            Dim ARDocEntry As String = String.Empty
                            Dim APDocEntry As String = String.Empty
                            Dim sPOWhsCode As String = String.Empty
                            Dim ErrorMsg As String = String.Empty
                            Dim sIntUDF As String = String.Empty

                            Try
                                sFuncName = "AP Invoice Draft - Form Data Event"
                                oForm = p_oSBOApplication.Forms.GetFormByTypeAndCount(141, oFormType)
                                If Right(oForm.Title, 5) = "Draft" Then
                                    sIntUDF = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_IFlag", 0).ToString.Trim
                                    ARDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim
                                    APDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0).ToString.Trim
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AP DocEntry " & APDocEntry, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR DocEntry " & ARDocEntry, sFuncName)
                                    If Not String.IsNullOrEmpty(ARDocEntry) Then
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling IntegrationFlag()", sFuncName)
                                        If sIntUDF = "Y" Or sIntUDF = "Yes" Then
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTradingCompany()", sFuncName)
                                            If ConnectToTradingCompany(p_oTradingCompany, p_oCompDef.TradingConnection, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                                            sErrDesc = ""

                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Update_DraftNumToTargetDocument() ", sFuncName)
                                            If Update_DraftNumToTargetDocument(p_oHoldingCompany, p_oTradingCompany, SAPbobsCOM.BoObjectTypes.oPurchaseInvoices, _
                                                                            SAPbobsCOM.BoObjectTypes.oInvoices _
                                             , APDocEntry, ARDocEntry, "OPCH", sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "13", ARDocEntry, p_oHoldingCompany.CompanyDB, _
                                                                "18", APDocEntry, "", "Success", "", "Yes")

                                        End If
                                    Else
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No Reference Document in AP Invoice " & APDocEntry, sFuncName)
                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName)
                                End If
                            Catch ex As Exception

                                sErrDesc = ex.Message
                                Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "13", ARDocEntry, p_oHoldingCompany.CompanyDB, _
                                                               "18", APDocEntry, "", "Fail", Left(sErrDesc, 247), "Yes")
                                Call WriteToLogFile(ex.Message, sFuncName)
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                SBO_Application.MessageBox("Error : " & sErrDesc & vbCrLf & " Reference Document No. is not updated in the targeting document", 1, "Ok", "Cancel")

                            End Try
                        End If
                        Exit Sub

                    Case "133" '------ AR Invoice

                        If BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE Then

                            Dim ARDocEntry As String = String.Empty
                            Dim APDocEntry As String = String.Empty
                            Dim sPOWhsCode As String = String.Empty
                            Dim ErrorMsg As String = String.Empty
                            Dim sCardCode As String = String.Empty
                            Dim sIntUDF As String = String.Empty

                            Dim LogMsg As String = String.Empty
                            Dim Status As String = String.Empty
                            Dim sBaseDocEntry As String = String.Empty
                            Dim sRefDocEntry As String = String.Empty
                            Dim sSplitVar() As String
                            Dim dDocTotal As Double = 0.0

                            Dim sDeliveryDocEntry As String = String.Empty
                            Dim sGRPOEntry As String = String.Empty

                            Dim oDT_GRPODetails As DataTable = Nothing

                            Try
                                oForm = p_oSBOApplication.Forms.GetFormByTypeAndCount(133, oFormType)
                                sFuncName = "AR Invoice - Form Data Event"
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting ", sFuncName)

                                If Right(oForm.Title, 5) = "Draft" Then
                                    sFuncName = "AR Invoice Draft - Form Data Event"
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting ", sFuncName)
                                    sIntUDF = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_IFlag", 0).ToString.Trim
                                    APDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim
                                    ARDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0).ToString.Trim
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AP DocEntry " & APDocEntry, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR DocEntry " & ARDocEntry, sFuncName)
                                    If Not String.IsNullOrEmpty(APDocEntry) Then
                                        If sIntUDF = "Y" Or sIntUDF = "Yes" Then
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTradingCompany()", sFuncName)
                                            If ConnectToTradingCompany(p_oTradingCompany, p_oCompDef.TradingConnection, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                                            sErrDesc = ""

                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Update_DraftNumToTargetDocument() ", sFuncName)
                                            If Update_DraftNumToTargetDocument(p_oHoldingCompany, p_oTradingCompany, SAPbobsCOM.BoObjectTypes.oInvoices, _
                                                                            SAPbobsCOM.BoObjectTypes.oPurchaseInvoices _
                                             , ARDocEntry, APDocEntry, "ORIN", sErrDesc) <> RTN_SUCCESS Then
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "18", ARDocEntry, p_oHoldingCompany.CompanyDB, _
                                                                "13", APDocEntry, "", "Fail", Left(sErrDesc, 247), "Yes")

                                            Else
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "18", ARDocEntry, p_oHoldingCompany.CompanyDB, _
                                                                                                                "13", APDocEntry, "", "Success", "", "Yes")

                                            End If
                                            'Throw New ArgumentException(sErrDesc)
                                            
                                        End If
                                    Else
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No Reference Document in AR Invoice " & ARDocEntry, sFuncName)
                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName)
                                    Exit Sub
                                End If

                                sFuncName = "AR Invoice - Form Data Event"
                                If BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE Then
                                    Status = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Istatus", 0).ToString.Trim
                                    sRefDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim

                                    If Status = "Success" And sRefDocEntry <> "" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                                sCardCode = oForm.DataSources.DBDataSources.Item(0).GetValue("CardCode", 0).ToString.Trim
                                ARDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0).ToString.Trim
                                sIntUDF = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_IFlag", 0).ToString.Trim
                                sPOWhsCode = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AB_POWhsCode", 0).ToString.Trim
                                dDocTotal = oForm.DataSources.DBDataSources.Item(0).GetValue("DocTotal", 0)

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug(" sCardCode " & sCardCode & " AR Invoice " & ARDocEntry & _
                                    " sIntUDF " & sIntUDF & " WhsCode " & sPOWhsCode, sFuncName)

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling IntegrationFlag()", sFuncName)
                                If IntegrationFlag(p_oHoldingCompany, sCardCode, "C", sIntUDF, sErrDesc) Then

                                    If oForm.Title.ToString.Trim = "A/R Invoice - Cancellation" Then
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Form Title is A/R Invoice Cancellation ", sFuncName)
                                        SBO_Application.MessageBox("No integration for cancellation.  Please manually cancel any relevant documents.", 1, "Ok", "Cancel")
                                        BubbleEvent = False
                                        Exit Sub
                                    End If

                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Intergration process is started ", sFuncName)

                                    SBO_Application.StatusBar.SetText("Intergration process is started, please wait ....... !", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)

                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTradingCompany()", sFuncName)
                                    If ConnectToTradingCompany(p_oTradingCompany, p_oCompDef.TradingConnection, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                                    sErrDesc = ""
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling DraftFlag()", sFuncName)
                                    If DraftFlag(p_oHoldingCompany, ARDocEntry, "INV1", sErrDesc) = True Then
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Draft Flag True", sFuncName)
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling APInvoiceDraft()", sFuncName)
                                        If APInvoiceDraft(p_oHoldingCompany, p_oTradingCompany, ARDocEntry, sPOWhsCode, sErrDesc) = RTN_SUCCESS Then
                                            APDocEntry = sErrDesc
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArInvoiceStatus() - SUCCESS", sFuncName)
                                            UpdateArInvoiceStatus(p_oHoldingCompany, p_oTradingCompany, ARDocEntry, APDocEntry, "Operation Completed Successfully - Draft", "Success", "ODRF", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "13", ARDocEntry, p_oTradingCompany.CompanyDB, _
                                                                  "18", "", APDocEntry, "Success", "", "No")
                                            SBO_Application.MessageBox("Integration process completed Successfully ...", 1, "Ok", "Cancel")
                                        Else
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArInvoiceStatus() - FAIL", sFuncName)
                                            UpdateArInvoiceStatus(p_oHoldingCompany, p_oTradingCompany, ARDocEntry, "", ErrorMsg, "Fail", "ODRF", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "13", ARDocEntry, p_oTradingCompany.CompanyDB, _
                                                                  "18", "", "", "Fail", Left(ErrorMsg, 247), "No")
                                            SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                            'SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                        End If

                                    Else
                                        ErrorMsg = sErrDesc
                                        If ErrorMsg = "" Then

                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling GetDelivery_Details()", sFuncName)

                                            oDT_GRPODetails = GetDelivery_Details(p_oHoldingCompany, p_oTradingCompany, ARDocEntry, "OINV", "INV1", sErrDesc)
                                            If sErrDesc.Length > 0 Then
                                                UpdateArInvoiceStatus(p_oHoldingCompany, p_oTradingCompany, ARDocEntry, "", sErrDesc, "Fail", "OPCH", sErrDesc)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "13", ARDocEntry, p_oTradingCompany.CompanyDB, _
                                                                  "18", "", "", "Fail", Left(sErrDesc, 247), "No")
                                                SBO_Application.MessageBox("Integration process Fails ... " & vbCrLf & sErrDesc & ".", 1, "Ok", "Cancel")
                                                BubbleEvent = False
                                                Exit Sub
                                            Else
                                                If oDT_GRPODetails.Rows.Count = 0 Then
                                                    UpdateArInvoiceStatus(p_oHoldingCompany, p_oTradingCompany, ARDocEntry, "", "No AP Invoice created.  Please manually create AP Invoice.", "Fail", "OPCH", sErrDesc)
                                                    Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "13", ARDocEntry, p_oTradingCompany.CompanyDB, _
                                                                  "18", "", "", "Fail", "No AP Invoice created.  Please manually create AP Invoice.", "No")
                                                    SBO_Application.MessageBox("Integration process Fails ... " & vbCrLf & "No AP Invoice created.  Please manually create AP Invoice.", 1, "Ok", "Cancel")
                                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No matching Base Document found / No Relevant Document mapped with this Delivery Order.... ", sFuncName)
                                                    BubbleEvent = False
                                                    Exit Sub
                                                End If
                                            End If

                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling APInvoice()", sFuncName)

                                            If APInvoice(p_oHoldingCompany, p_oTradingCompany, oDT_GRPODetails, dDocTotal, sErrDesc) = RTN_SUCCESS Then
                                                APDocEntry = sErrDesc
                                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArInvoiceStatus() - SUCCESS", sFuncName)
                                                UpdateArInvoiceStatus(p_oHoldingCompany, p_oTradingCompany, ARDocEntry, APDocEntry, "Operation Completed Successfully ", "Success", "OPCH", sErrDesc)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "13", ARDocEntry, p_oTradingCompany.CompanyDB, "18", APDocEntry, "", "Success", "", "No")
                                                SBO_Application.MessageBox("Integration process completed Successfully ...", 1, "Ok", "Cancel")
                                            Else
                                                ErrorMsg = sErrDesc
                                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArInvoiceStatus() - FAIL " & ErrorMsg, sFuncName)
                                                UpdateArInvoiceStatus(p_oHoldingCompany, p_oTradingCompany, ARDocEntry, "", ErrorMsg, "Fail", "OPCH", sErrDesc)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "13", ARDocEntry, p_oTradingCompany.CompanyDB, "18", "", "", "Fail", Left(ErrorMsg, 247), "No")
                                                SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                                BubbleEvent = False
                                                Exit Sub
                                            End If
                                        Else
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArInvoiceStatus() - FAIL " & ErrorMsg, sFuncName)
                                            UpdateArInvoiceStatus(p_oHoldingCompany, p_oTradingCompany, ARDocEntry, "", ErrorMsg, "Fail", "OPCH", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "13", ARDocEntry, p_oTradingCompany.CompanyDB, "18", "", "", "Fail", Left(ErrorMsg, 247), "No")
                                            SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                            BubbleEvent = False
                                            Exit Sub
                                            ' Throw New ArgumentException(sErrDesc)
                                        End If
                                    End If
                                End If

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
                                Exit Sub
                            Catch ex As Exception

                                sErrDesc = ex.Message
                                UpdateArInvoiceStatus(p_oHoldingCompany, p_oTradingCompany, ARDocEntry, "", sErrDesc, "Fail", "OPCH", sErrDesc)
                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "13", ARDocEntry, p_oTradingCompany.CompanyDB, "18", "", "", "Fail", Left(sErrDesc, 247), "No")
                                Call WriteToLogFile(ex.Message, sFuncName)
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                'SBO_Application.StatusBar.SetText("Integration process Fails ...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error)
                            End Try
                            Exit Sub
                        End If

                    Case "179"   '--- AR Credit memo
                        If BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE Then

                            Dim ARCreditDocEntry As String = String.Empty
                            Dim APDocEntry As String = String.Empty
                            Dim ErrorMsg As String = String.Empty
                            Dim sCardCode As String = String.Empty
                            Dim sIntUDF As String = String.Empty
                            Dim orset As SAPbobsCOM.Recordset = p_oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)
                            Try
                                oForm = p_oSBOApplication.Forms.GetFormByTypeAndCount(179, oFormType)

                                sFuncName = "AR Credit memo - Form Data Event"
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting ", sFuncName)

                                Dim LogMsg As String = String.Empty
                                Dim Status As String = String.Empty
                                Dim BaseDocEntry As String = String.Empty
                                Dim BaseType As String = String.Empty

                                If Right(oForm.Title, 5) = "Draft" Then
                                    sFuncName = "AR Credit memo Draft - Form Data Event"
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting ", sFuncName)
                                    sIntUDF = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_IFlag", 0).ToString.Trim
                                    APDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim
                                    ARCreditDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0).ToString.Trim
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AP DocEntry " & APDocEntry, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR DocEntry " & ARCreditDocEntry, sFuncName)
                                    If Not String.IsNullOrEmpty(APDocEntry) Then
                                        If sIntUDF = "Y" Or sIntUDF = "Yes" Then
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTradingCompany()", sFuncName)
                                            If ConnectToTradingCompany(p_oTradingCompany, p_oCompDef.TradingConnection, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                                            sErrDesc = ""

                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Update_DraftNumToTargetDocument() ", sFuncName)
                                            If Update_DraftNumToTargetDocument(p_oHoldingCompany, p_oTradingCompany, SAPbobsCOM.BoObjectTypes.oCreditNotes, _
                                                                            SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes _
                                             , ARCreditDocEntry, APDocEntry, "ORIN", sErrDesc) <> RTN_SUCCESS Then
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "19", APDocEntry, p_oHoldingCompany.CompanyDB, _
                                                               "14", ARCreditDocEntry, "", "Fail", Left(sErrDesc, 247), "Yes")

                                            Else

                                                Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "19", APDocEntry, p_oHoldingCompany.CompanyDB, _
                                                                                                               "14", ARCreditDocEntry, "", "Success", "", "Yes")

                                            End If
                                            'Throw New ArgumentException(sErrDesc)
                                           
                                        End If
                                    Else
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No Reference Document in AR Credit Memo " & ARCreditDocEntry, sFuncName)
                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName)
                                    Exit Sub
                                End If

                                sFuncName = "AR Credit memo - Form Data Event"

                                If BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE Then
                                    Status = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Istatus", 0).ToString.Trim
                                    BaseDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim
                                    ' Checking the status if its fail it will countinue otherwise it will exit
                                    If Status = "Success" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                                ARCreditDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0).ToString.Trim
                                APDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim
                                sCardCode = oForm.DataSources.DBDataSources.Item(0).GetValue("CardCode", 0).ToString.Trim
                                sIntUDF = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_IFlag", 0).ToString.Trim

                                orset.DoQuery("SELECT T0.[BaseType] FROM RIN1 T0 WHERE T0.[DocEntry] = '" & ARCreditDocEntry & "'")

                                Select Case orset.Fields.Item("BaseType").Value

                                    Case 13
                                        BaseType = "AR Invoice"
                                    Case 16
                                        BaseType = "AR Return"
                                    Case Else
                                        BaseType = ""
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Base Type is Empty ", sFuncName)
                                End Select


                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug(" AR Creditmemo DocEntry " & ARCreditDocEntry & " sCardCode " & sCardCode & " AP Invoice " & APDocEntry & _
                                   " sIntUDF " & sIntUDF, sFuncName)

                                ' Checking the integration flag

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling IntegrationFlag() ", sFuncName)
                                If IntegrationFlag(p_oHoldingCompany, sCardCode, "C", sIntUDF, sErrDesc) Then

                                    SBO_Application.StatusBar.SetText("Intergration process is started, please wait ....... !", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTradingCompany() ", sFuncName)
                                    If ConnectToTradingCompany(p_oTradingCompany, p_oCompDef.TradingConnection, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                                    sErrDesc = ""
                                    'Checking the Draft flag
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling DraftFlag() ", sFuncName)
                                    If DraftFlag(p_oHoldingCompany, ARCreditDocEntry, "RIN1", sErrDesc) = True Then

                                        'Draft Creation
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Draft_Creation() ", sFuncName)
                                        If Draft_Creation(SAPbobsCOM.BoObjectTypes.oCreditNotes, SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes _
                                 , p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, True, "V", "ORIN", sErrDesc) = RTN_SUCCESS Then
                                            APDocEntry = sErrDesc
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArCreditmemoStatus() - SUCCESS ", sFuncName)
                                            UpdateArCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APDocEntry, "Operation Completed Successfully ", "Success", "ODRF", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "14", ARCreditDocEntry, p_oTradingCompany.CompanyDB, _
                                                                    "19", "", APDocEntry, "Success", "", "No")
                                            SBO_Application.MessageBox("Integration process completed Successfully ...", 1, "Ok", "Cancel")
                                        Else
                                            ErrorMsg = sErrDesc
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArCreditmemoStatus() - Fail" & ErrorMsg, sFuncName)
                                            UpdateArCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APDocEntry, ErrorMsg, "Fail", "ODRF", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "14", ARCreditDocEntry, p_oTradingCompany.CompanyDB, _
                                                                  "19", "", "", "Fail", Left(ErrorMsg, 247), "No")
                                            SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                        End If
                                    Else
                                        ErrorMsg = sErrDesc
                                        If ErrorMsg = "" Then
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling APCreditMemo()", sFuncName)
                                            If APCreditMemo(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APDocEntry, BaseType, sErrDesc) = RTN_SUCCESS Then
                                                APDocEntry = sErrDesc
                                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArCreditmemoStatus() - SUCCESS", sFuncName)
                                                UpdateArCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APDocEntry, "Operation Completed Successfully ", "Success", "ORPC", sErrDesc)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "14", ARCreditDocEntry, p_oTradingCompany.CompanyDB, "19", APDocEntry, "", "Success", "", "No")

                                                SBO_Application.MessageBox("Integration process completed Successfully ...", 1, "Ok", "Cancel")
                                            Else
                                                ErrorMsg = sErrDesc
                                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArCreditmemoStatus() - FAIL " & ErrorMsg, sFuncName)
                                                UpdateArCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APDocEntry, ErrorMsg, "Fail", "ORPC", sErrDesc)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "14", ARCreditDocEntry, p_oTradingCompany.CompanyDB, "19", "", "", "Fail", Left(ErrorMsg, 247), "No")
                                                SBO_Application.MessageBox("Integration process Fails ..." & vbCrLf & ErrorMsg & ".", 1, "Ok", "Cancel")
                                                BubbleEvent = False
                                                Exit Sub
                                            End If
                                        Else
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArCreditmemoStatus() - FAIL " & ErrorMsg, sFuncName)
                                            UpdateArCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APDocEntry, ErrorMsg, "Fail", "ORPC", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "14", ARCreditDocEntry, p_oTradingCompany.CompanyDB, "19", "", "", "Fail", Left(ErrorMsg, 247), "No")
                                            SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                            ' Throw New ArgumentException(sErrDesc)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    End If
                                End If

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)

                                Exit Sub
                            Catch ex As Exception
                                sErrDesc = ex.Message
                                Call WriteToLogFile(ex.Message, sFuncName)
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                UpdateArCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APDocEntry, sErrDesc, "Fail", "ORPC", sErrDesc)
                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "14", ARCreditDocEntry, p_oTradingCompany.CompanyDB, "19", "", "", "Fail", Left(sErrDesc, 247), "No")
                                SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                            End Try
                        End If

                    Case "181" '----- AP Credit memo

                        If BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE Then

                            Dim ARDocEntry As String = String.Empty
                            Dim ARCreditDocEntry As String = String.Empty
                            Dim APCreditDocEntry As String = String.Empty
                            Dim ErrorMsg As String = String.Empty
                            Dim sCardCode As String = String.Empty
                            Dim sIntUDF As String = String.Empty
                            Dim BaseType As String = String.Empty

                            Try
                                oForm = p_oSBOApplication.Forms.GetFormByTypeAndCount(181, oFormType)
                                Dim LogMsg As String = String.Empty
                                Dim Status As String = String.Empty
                                Dim BaseDocEntry As String = String.Empty
                                Dim oreset_AR As SAPbobsCOM.Recordset = p_oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)

                                sFuncName = "AP Credit memo - Form Data Event "
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting ", sFuncName)

                                If Right(oForm.Title, 5) = "Draft" Then
                                    sFuncName = "AR Credit memo Draft - Form Data Event"
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting ", sFuncName)
                                    sIntUDF = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_IFlag", 0).ToString.Trim
                                    ARCreditDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim
                                    APCreditDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0).ToString.Trim
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AP CreditMemo DocEntry " & APCreditDocEntry, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR CreditMemo DocEntry " & ARCreditDocEntry, sFuncName)
                                    If Not String.IsNullOrEmpty(ARCreditDocEntry) Then
                                        If sIntUDF = "Y" Or sIntUDF = "Yes" Then
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTradingCompany()", sFuncName)
                                            If ConnectToTradingCompany(p_oTradingCompany, p_oCompDef.TradingConnection, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                                            sErrDesc = ""

                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Update_DraftNumToTargetDocument() ", sFuncName)
                                            If Update_DraftNumToTargetDocument(p_oHoldingCompany, p_oTradingCompany, SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes, _
                                                                            SAPbobsCOM.BoObjectTypes.oCreditNotes _
                                             , APCreditDocEntry, ARCreditDocEntry, "ORPC", sErrDesc) <> RTN_SUCCESS Then
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "14", ARCreditDocEntry, p_oHoldingCompany.CompanyDB, _
                                                                "19", APCreditDocEntry, "", "Fail", Left(sErrDesc, 247), "Yes")

                                            Else
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "14", ARCreditDocEntry, p_oHoldingCompany.CompanyDB, _
                                                                                                                "19", APCreditDocEntry, "", "Success", "", "Yes")

                                            End If
                                            'Throw New ArgumentException(sErrDesc)
                                            
                                        End If
                                    Else
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No Reference Document in AP Credit Memo " & APCreditDocEntry, sFuncName)
                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName)
                                    Exit Sub
                                End If

                                sFuncName = "AP Credit memo - Form Data Event "
                                If BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE Then
                                    Status = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Istatus", 0).ToString.Trim
                                    BaseDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim

                                    If Status = "Success" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                                APCreditDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0).ToString.Trim
                                oreset_AR.DoQuery("SELECT T0.[BaseType] FROM RPC1 T0 WHERE T0.[DocEntry] ='" & APCreditDocEntry & "'")

                                Select Case oreset_AR.Fields.Item("BaseType").Value

                                    Case 18
                                        BaseType = "AP Invoice"
                                    Case 21
                                        BaseType = "AP Return"
                                    Case Else
                                        BaseType = ""
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Base Type is Empty ", sFuncName)
                                End Select


                                ARDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim
                                If String.IsNullOrEmpty(ARDocEntry) Then
                                    oreset_AR.DoQuery("SELECT T0.[U_AE_RefDocEntry] FROM RPC1 T0 WHERE T0.[DocEntry] ='" & APCreditDocEntry & "'")
                                    ARDocEntry = oreset_AR.Fields.Item("U_AE_RefDocEntry").Value
                                End If
                                sCardCode = oForm.DataSources.DBDataSources.Item(0).GetValue("CardCode", 0).ToString.Trim
                                sIntUDF = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_IFlag", 0).ToString.Trim

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug(" AP Credit memo DocEntry " & APCreditDocEntry & " sCardCode " & sCardCode & " AR Invoice " & ARDocEntry & _
                               " sIntUDF " & sIntUDF, sFuncName)

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling IntegrationFlag() ", sFuncName)
                                If IntegrationFlag(p_oHoldingCompany, sCardCode, "V", sIntUDF, sErrDesc) Then
                                    SBO_Application.StatusBar.SetText("Intergration process is started, please wait ....... !", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTradingCompany() ", sFuncName)
                                    If ConnectToTradingCompany(p_oTradingCompany, p_oCompDef.TradingConnection, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                                    sErrDesc = ""
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling DraftFlag() ", sFuncName)
                                    If DraftFlag(p_oHoldingCompany, APCreditDocEntry, "RPC1", sErrDesc) = True Then
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Draft_Creation() ", sFuncName)
                                        If Draft_Creation(SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes, SAPbobsCOM.BoObjectTypes.oCreditNotes _
                               , p_oHoldingCompany, p_oTradingCompany, APCreditDocEntry, True, "C", "ORPC", sErrDesc) = RTN_SUCCESS Then
                                            ARCreditDocEntry = sErrDesc
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArCreditmemoStatus() - SUCCESS", sFuncName)
                                            UpdateApCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APCreditDocEntry, "Operation Completed Successfully ", "Success", "ODRF", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "19", APCreditDocEntry, p_oTradingCompany.CompanyDB, _
                                                                   "14", "", ARCreditDocEntry, "Success", "", "No")
                                            SBO_Application.MessageBox("Integration process completed Successfully ...", 1, "Ok", "Cancel")
                                        Else
                                            ErrorMsg = sErrDesc
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArCreditmemoStatus() - FAIL " & ErrorMsg, sFuncName)
                                            UpdateApCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APCreditDocEntry, ErrorMsg, "Fail", "ODRF", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "19", APCreditDocEntry, p_oTradingCompany.CompanyDB, _
                                                                  "14", "", "", "Fail", Left(ErrorMsg, 247), "No")
                                            SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                        End If

                                    Else
                                        ErrorMsg = sErrDesc
                                        If ErrorMsg = "" Then

                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ARCreditMemo() ", sFuncName)
                                            If ARCreditMemo(p_oHoldingCompany, p_oTradingCompany, APCreditDocEntry, ARDocEntry, BaseType, sErrDesc) = RTN_SUCCESS Then
                                                ARCreditDocEntry = sErrDesc
                                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArCreditmemoStatus() - SUCCESS ", sFuncName)
                                                UpdateApCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APCreditDocEntry, "Operation Completed Successfully ", "Success", "ORIN", sErrDesc)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "19", APCreditDocEntry, p_oTradingCompany.CompanyDB, _
                                                                      "14", ARCreditDocEntry, "", "Success", "", "No")
                                                SBO_Application.MessageBox("Integration process completed Successfully ...", 1, "Ok", "Cancel")
                                            Else
                                                ErrorMsg = sErrDesc
                                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArCreditmemoStatus() - FAIL " & ErrorMsg, sFuncName)
                                                UpdateApCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APCreditDocEntry, ErrorMsg, "Fail", "ORIN", sErrDesc)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "19", APCreditDocEntry, p_oTradingCompany.CompanyDB, "14", "", "", "Fail", Left(ErrorMsg, 247), "No")
                                                SBO_Application.MessageBox("Integration process Fails ... " & vbCrLf & ErrorMsg & ".", 1, "Ok", "Cancel")
                                                BubbleEvent = False
                                                Exit Sub
                                            End If
                                        Else
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateArCreditmemoStatus() - FAIL " & ErrorMsg, sFuncName)
                                            UpdateApCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APCreditDocEntry, ErrorMsg, "Fail", "ORIN", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "19", APCreditDocEntry, p_oTradingCompany.CompanyDB, "14", "", "", "Fail", Left(ErrorMsg, 247), "No")
                                            SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                            'Throw New ArgumentException(sErrDesc)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    End If
                                End If

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName)

                            Catch ex As Exception
                                sErrDesc = ex.Message
                                Call WriteToLogFile(ex.Message, sFuncName)
                                UpdateApCreditmemoStatus(p_oHoldingCompany, p_oTradingCompany, ARCreditDocEntry, APCreditDocEntry, sErrDesc, "Fail", "ORIN", sErrDesc)
                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "19", APCreditDocEntry, p_oTradingCompany.CompanyDB, _
                                                                 "14", "", "", "Fail", Left(sErrDesc, 247), "No")
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                            End Try
                            Exit Sub
                        End If

                    Case "180" ' Sales Return

                        If BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE Then


                            Dim ARReturnDocEntry As String = String.Empty
                            Dim APReturnDocEntry As String = String.Empty
                            Dim PODocNum As String = String.Empty
                            Dim GRPODocEntry As String = String.Empty
                            Dim sErrDesc As String = String.Empty
                            Dim ErrorMsg As String = String.Empty
                            Dim sCardCode As String = String.Empty
                            Dim sIntUDF As String = String.Empty
                            Dim sWhsCode As String = String.Empty

                            Dim oDT_GRPODetails As DataTable = Nothing
                            Try
                                oForm = p_oSBOApplication.Forms.GetFormByTypeAndCount(180, oFormType)
                                Dim LogMsg As String = String.Empty
                                Dim Status As String = String.Empty
                                Dim sBaseDocEntry As String = String.Empty
                                Dim sSplitVar() As String

                                Dim sDeliveryDocEntry As String = String.Empty
                                Dim sGRPOEntry As String = String.Empty


                                sFuncName = "AR Return - Form Data Event "
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting ", sFuncName)

                                If Right(oForm.Title, 5) = "Draft" Then
                                    sFuncName = "AR Return Draft - Form Data Event"
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting ", sFuncName)
                                    sIntUDF = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_IFlag", 0).ToString.Trim
                                    APReturnDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim
                                    ARReturnDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0).ToString.Trim
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AP Return DocEntry " & APReturnDocEntry, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR Return DocEntry " & ARReturnDocEntry, sFuncName)
                                    If Not String.IsNullOrEmpty(APReturnDocEntry) Then
                                        If sIntUDF = "Y" Or sIntUDF = "Yes" Then
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTradingCompany()", sFuncName)
                                            If ConnectToTradingCompany(p_oTradingCompany, p_oCompDef.TradingConnection, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                                            sErrDesc = ""

                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Update_DraftNumToTargetDocument() ", sFuncName)
                                            If Update_DraftNumToTargetDocument(p_oHoldingCompany, p_oTradingCompany, SAPbobsCOM.BoObjectTypes.oReturns, _
                                                                            SAPbobsCOM.BoObjectTypes.oPurchaseReturns _
                                             , ARReturnDocEntry, APReturnDocEntry, "ORPD", sErrDesc) <> RTN_SUCCESS Then
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "21", APReturnDocEntry, p_oHoldingCompany.CompanyDB, _
                                                               "16", ARReturnDocEntry, "", "Fail", Left(sErrDesc, 247), "Yes")
                                            Else
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "21", APReturnDocEntry, p_oHoldingCompany.CompanyDB, _
                                                                   "16", ARReturnDocEntry, "", "Success", "", "Yes")
                                            End If
                                            'Throw New ArgumentException(sErrDesc)
                                           

                                        End If
                                    Else
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No Reference Document in AR Return " & ARReturnDocEntry, sFuncName)
                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName)
                                    Exit Sub
                                End If


                                If BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE Then
                                    Status = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Istatus", 0).ToString.Trim
                                    If Status = "Success" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                                ARReturnDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0).ToString.Trim

                                sCardCode = oForm.DataSources.DBDataSources.Item(0).GetValue("CardCode", 0).ToString.Trim
                                sIntUDF = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_IFlag", 0).ToString.Trim

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug(" AR Return DocEntry " & ARReturnDocEntry & " sCardCode " & sCardCode & " PO DocNum " & PODocNum & _
                             " sIntUDF " & sIntUDF & " sWhsCode " & sWhsCode, sFuncName)


                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling IntegrationFlag() ", sFuncName)
                                If IntegrationFlag(p_oHoldingCompany, sCardCode, "C", sIntUDF, sErrDesc) Then
                                    SBO_Application.StatusBar.SetText("Intergration process is started, please wait ....... !", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTradingCompany() ", sFuncName)
                                    If ConnectToTradingCompany(p_oTradingCompany, p_oCompDef.TradingConnection, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)

                                    sErrDesc = ""
                                    ''If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling GRPODocEntry() ", sFuncName)
                                    ''GRPODocEntry = GetDocEntry(p_oTradingCompany, PODocNum, "PDN1", sErrDesc)

                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling DraftFlag() ", sFuncName)
                                    If DraftFlag(p_oHoldingCompany, ARReturnDocEntry, "RDN1", sErrDesc) = True Then

                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Draft_Creation() ", sFuncName)
                                        If Draft_Creation(SAPbobsCOM.BoObjectTypes.oReturns, SAPbobsCOM.BoObjectTypes.oPurchaseReturns _
                                     , p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, True, "V", "ORDN", sErrDesc) = RTN_SUCCESS Then
                                            APReturnDocEntry = sErrDesc
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateARreturnStatus() - SUCCESS ", sFuncName)
                                            UpdateARreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, " ", "Success", "ODRF", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "16", ARReturnDocEntry, p_oTradingCompany.CompanyDB, _
                                                                  "21", "", APReturnDocEntry, "Success", "", "No")
                                            SBO_Application.MessageBox("Integration process completed Successfully ...", 1, "Ok", "Cancel")
                                        Else
                                            ErrorMsg = sErrDesc
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateARreturnStatus() - FAIL " & ErrorMsg, sFuncName)
                                            UpdateARreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, ErrorMsg, "Fail", "ODRF", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "16", ARReturnDocEntry, p_oTradingCompany.CompanyDB, _
                                                                  "21", "", "", "Fail", Left(ErrorMsg, 247), "No")
                                            SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                        End If

                                    Else
                                        ErrorMsg = sErrDesc
                                        If ErrorMsg = "" Then

                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling GetDelivery_Details()", sFuncName)

                                            oDT_GRPODetails = GetDelivery_Details(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, "ORDN", "RDN1", sErrDesc)
                                            If sErrDesc.Length > 0 Then
                                                UpdateARreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, sErrDesc, "Fail", "ORPD", sErrDesc)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "16", ARReturnDocEntry, p_oTradingCompany.CompanyDB, _
                                                                      "21", APReturnDocEntry, "", "Fail", Left(sErrDesc, 247), "No")
                                                SBO_Application.MessageBox("Integration process Fails ... " & vbCrLf & sErrDesc & ".", 1, "Ok", "Cancel")
                                                BubbleEvent = False
                                                Exit Sub
                                            Else
                                                If oDT_GRPODetails.Rows.Count = 0 Then
                                                    UpdateARreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, "No AP Return created.  Please manually create AP Return", "Fail", "ORPD", sErrDesc)
                                                    Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "16", ARReturnDocEntry, p_oTradingCompany.CompanyDB, _
                                                                      "21", APReturnDocEntry, "", "Fail", "No AP Return created.  Please manually create AP Return", "No")
                                                    SBO_Application.MessageBox("Integration process Fails ... " & vbCrLf & "No AP Return created.  Please manually create AP Return.", 1, "Ok", "Cancel")
                                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No matching Base Document found .... ", sFuncName)
                                                    BubbleEvent = False
                                                    Exit Sub
                                                End If
                                            End If

                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling APReturn()", sFuncName)
                                            If APReturn(p_oHoldingCompany, p_oTradingCompany, _
                                                  oDT_GRPODetails, sErrDesc) = RTN_SUCCESS Then
                                                APReturnDocEntry = sErrDesc
                                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateARreturnStatus()", sFuncName)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "16", ARReturnDocEntry, p_oTradingCompany.CompanyDB, _
                                                                      "21", APReturnDocEntry, "", "Success", "", "No")
                                                UpdateARreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, " ", "Success", "ORPD", sErrDesc)

                                                SBO_Application.MessageBox("Integration process completed Successfully ...", 1, "Ok", "Cancel")
                                            Else
                                                ErrorMsg = sErrDesc
                                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateARreturnStatus() - FAIL " & ErrorMsg, sFuncName)
                                                UpdateARreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, ErrorMsg, "Fail", "ORPD", sErrDesc)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "16", ARReturnDocEntry, p_oTradingCompany.CompanyDB, _
                                                                      "21", APReturnDocEntry, "", "Fail", Left(ErrorMsg, 247), "No")
                                                SBO_Application.MessageBox("Integration process Fails ..." & vbCrLf & ErrorMsg & ".", 1, "Ok", "Cancel")
                                                BubbleEvent = False
                                                Exit Sub
                                            End If
                                        Else
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateARreturnStatus() - FAIL " & ErrorMsg, sFuncName)
                                            UpdateARreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, ErrorMsg, "Fail", "ORPD", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "16", ARReturnDocEntry, p_oTradingCompany.CompanyDB, _
                                                                  "21", APReturnDocEntry, "", "Fail", Left(ErrorMsg, 247), "No")

                                            SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                            BubbleEvent = False
                                            Exit Sub
                                            'Throw New ArgumentException(sErrDesc)
                                        End If
                                    End If
                                End If

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)

                            Catch ex As Exception

                                sErrDesc = ex.Message
                                Call WriteToLogFile(ex.Message, sFuncName)
                                UpdateARreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, sErrDesc, "Fail", "ORPD", sErrDesc)
                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "16", ARReturnDocEntry, p_oTradingCompany.CompanyDB, _
                                                                     "21", APReturnDocEntry, "", "Fail", Left(sErrDesc, 247), "No")
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")

                            End Try
                            Exit Sub

                        End If

                    Case "182" ' Purchase Return

                        If BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD Or BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE Then

                            Dim ARReturnDocEntry As String = String.Empty
                            Dim APReturnDocEntry As String = String.Empty
                            Dim DeliveryDocentry As String = String.Empty
                            Dim sErrDesc As String = String.Empty
                            Dim ErrorMsg As String = String.Empty
                            Dim sCardCode As String = String.Empty
                            Dim sIntUDF As String = String.Empty

                            Try
                                oForm = p_oSBOApplication.Forms.GetFormByTypeAndCount(182, oFormType)
                                Dim LogMsg As String = String.Empty
                                Dim Status As String = String.Empty
                                Dim BaseDocEntry As String = String.Empty
                                Dim oreset_AR As SAPbobsCOM.Recordset = p_oHoldingCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)


                                sFuncName = "Attempting AP Return " & BusinessObjectInfo.EventType
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting ", sFuncName)

                                If Right(oForm.Title, 5) = "Draft" Then
                                    sFuncName = "AP Return Draft - Form Data Event"
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting ", sFuncName)
                                    sIntUDF = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_IFlag", 0).ToString.Trim
                                    ARReturnDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim
                                    APReturnDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0).ToString.Trim
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AP CreditMemo DocEntry " & APReturnDocEntry, sFuncName)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AR CreditMemo DocEntry " & ARReturnDocEntry, sFuncName)
                                    If Not String.IsNullOrEmpty(ARReturnDocEntry) Then
                                        If sIntUDF = "Y" Or sIntUDF = "Yes" Then
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTradingCompany()", sFuncName)
                                            If ConnectToTradingCompany(p_oTradingCompany, p_oCompDef.TradingConnection, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                                            sErrDesc = ""

                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Update_DraftNumToTargetDocument() ", sFuncName)
                                            If Update_DraftNumToTargetDocument(p_oHoldingCompany, p_oTradingCompany, SAPbobsCOM.BoObjectTypes.oPurchaseReturns, _
                                                                            SAPbobsCOM.BoObjectTypes.oReturns _
                                             , APReturnDocEntry, ARReturnDocEntry, "ORPD", sErrDesc) <> RTN_SUCCESS Then
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "16", ARReturnDocEntry, p_oHoldingCompany.CompanyDB, _
                                                                "21", APReturnDocEntry, "", "Fail", Left(sErrDesc, 247), "Yes")
                                            Else
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oTradingCompany.CompanyDB, "16", ARReturnDocEntry, p_oHoldingCompany.CompanyDB, _
                                                                                                                "21", APReturnDocEntry, "", "Success", "", "Yes")
                                            End If
                                            'Throw New ArgumentException(sErrDesc)
                                            
                                        End If
                                    Else
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("No Reference Document in AP Return " & APReturnDocEntry, sFuncName)
                                    End If
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS ", sFuncName)
                                    Exit Sub
                                End If

                                sFuncName = "Attempting AP Return " & BusinessObjectInfo.EventType
                                If BusinessObjectInfo.EventType = SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE Then
                                    Status = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Istatus", 0).ToString.Trim
                                    BaseDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_Docentry", 0).ToString.Trim

                                    If Status = "Success" Then
                                        BubbleEvent = False
                                        Exit Sub
                                    End If
                                End If

                                APReturnDocEntry = oForm.DataSources.DBDataSources.Item(0).GetValue("DocEntry", 0).ToString.Trim
                                sCardCode = oForm.DataSources.DBDataSources.Item(0).GetValue("CardCode", 0).ToString.Trim
                                sIntUDF = oForm.DataSources.DBDataSources.Item(0).GetValue("U_AE_IFlag", 0).ToString.Trim


                                oreset_AR.DoQuery("SELECT T0.[U_AE_RefDocEntry] FROM RPD1 T0 WHERE T0.[DocEntry] ='" & APReturnDocEntry & "'")
                                DeliveryDocentry = oreset_AR.Fields.Item("U_AE_RefDocEntry").Value

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug(" AP Return DocEntry " & APReturnDocEntry & " sCardCode " & sCardCode & _
                        " sIntUDF " & sIntUDF, sFuncName)


                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling IntegrationFlag() ", sFuncName)
                                If IntegrationFlag(p_oHoldingCompany, sCardCode, "V", sIntUDF, sErrDesc) Then
                                    SBO_Application.StatusBar.SetText("Intergration process is started, please wait ....... !", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectToTradingCompany() ", sFuncName)
                                    If ConnectToTradingCompany(p_oTradingCompany, p_oCompDef.TradingConnection, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
                                    If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling DraftFlag() ", sFuncName)
                                    If DraftFlag(p_oHoldingCompany, APReturnDocEntry, "RPD1", sErrDesc) = True Then
                                        If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling Draft_Creation() ", sFuncName)
                                        If Draft_Creation(SAPbobsCOM.BoObjectTypes.oPurchaseReturns, SAPbobsCOM.BoObjectTypes.oReturns _
                                     , p_oHoldingCompany, p_oTradingCompany, APReturnDocEntry, True, "C", "ORPD", sErrDesc) = RTN_SUCCESS Then
                                            ARReturnDocEntry = sErrDesc
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateAPreturnStatus() - SUCCESS", sFuncName)
                                            UpdateAPreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, "Operation Completed Successfully ", "Success", "ODRF", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "21", APReturnDocEntry, p_oTradingCompany.CompanyDB, _
                                                                 "16", ARReturnDocEntry, "", "Success", "", "No")
                                            SBO_Application.MessageBox("Integration process completed Successfully ...", 1, "Ok", "Cancel")
                                        Else
                                            ErrorMsg = sErrDesc
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateAPreturnStatus() - FAIL", sFuncName)
                                            UpdateAPreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, ErrorMsg, "Fail", "ODRF", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "21", APReturnDocEntry, p_oTradingCompany.CompanyDB, _
                                                                 "16", "", "", "Fail", Left(ErrorMsg, 247), "No")
                                            SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                        End If

                                    Else
                                        ErrorMsg = sErrDesc
                                        If ErrorMsg = "" Then
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ARReturn()", sFuncName)

                                            If ARReturn(p_oHoldingCompany, p_oTradingCompany, APReturnDocEntry, DeliveryDocentry, sErrDesc) = RTN_SUCCESS Then
                                                ARReturnDocEntry = sErrDesc
                                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateAPreturnStatus() - SUCCESS", sFuncName)
                                                UpdateAPreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, "Operation Completed Successfully ", "Success", "ORDN", sErrDesc)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "21", APReturnDocEntry, p_oTradingCompany.CompanyDB, "16", ARReturnDocEntry, "", "Success", "", "No")

                                                SBO_Application.MessageBox("Integration process completed Successfully ...", 1, "Ok", "Cancel")
                                            Else
                                                ErrorMsg = sErrDesc
                                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateAPreturnStatus() - FAIL", sFuncName)
                                                UpdateAPreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, ErrorMsg, "Fail", "ORDN", sErrDesc)
                                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "21", APReturnDocEntry, p_oTradingCompany.CompanyDB, "16", ARReturnDocEntry, "", "Fail", Left(ErrorMsg, 247), "No")
                                                SBO_Application.MessageBox("Integration process Fails ..." & vbCrLf & ErrorMsg & ".", 1, "Ok", "Cancel")
                                                BubbleEvent = False
                                                Exit Sub
                                            End If
                                        Else
                                            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling UpdateAPreturnStatus() - FAIL", sFuncName)
                                            UpdateAPreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, ErrorMsg, "Fail", "ORDP", sErrDesc)
                                            Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "21", APReturnDocEntry, p_oTradingCompany.CompanyDB, "16", ARReturnDocEntry, "", "Fail", Left(ErrorMsg, 247), "No")
                                            SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                                            'Throw New ArgumentException(sErrDesc)
                                            BubbleEvent = False
                                            Exit Sub
                                        End If

                                    End If
                                End If

                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with SUCCESS", sFuncName)
                            Catch ex As Exception
                                sErrDesc = ex.Message
                                Call WriteToLogFile(ex.Message, sFuncName)
                                UpdateAPreturnStatus(p_oHoldingCompany, p_oTradingCompany, ARReturnDocEntry, APReturnDocEntry, sErrDesc, "Fail", "ORDP", sErrDesc)
                                Insert_IntegrationLog(p_oHoldingCompany, p_oHoldingCompany.CompanyDB, "21", ARReturnDocEntry, p_oTradingCompany.CompanyDB, "16", APReturnDocEntry, "", "Fail", Left(ErrorMsg, 247), "No")
                                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
                                SBO_Application.MessageBox("Integration process Fails ...", 1, "Ok", "Cancel")
                            End Try
                            Exit Sub
                        End If
                End Select
            End If


        Catch ex As Exception
            sErrDesc = ex.Message
            Call WriteToLogFile(ex.Message, sFuncName)
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", sFuncName)
        End Try
    End Sub
End Class
