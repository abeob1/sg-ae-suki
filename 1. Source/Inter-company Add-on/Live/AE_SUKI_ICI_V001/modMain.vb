Option Explicit On



Module modMain


    Public p_iDebugMode As Int16
    Public p_iErrDispMethod As Int16
    Public p_iDeleteDebugLog As Int16

    Public Const RTN_SUCCESS As Int16 = 1
    Public Const RTN_ERROR As Int16 = 0

    Public Const DEBUG_ON As Int16 = 1
    Public Const DEBUG_OFF As Int16 = 0

    Public Const ERR_DISPLAY_STATUS As Int16 = 1
    Public Const ERR_DISPLAY_DIALOGUE As Int16 = 2

    Public sHoldingDBName As String = "HSukiGroup" '"UAT_HSukiGroup" '

    Public Structure CompanyDefault

        Public ServerName As String  ' Database Name
        Public DBName As String  ' Database Name
        Public DBUsername As String  ' Credentials 
        Public DBPass As String      ' Credentials
        Public TradingVendor As String  ' Trading Vendor Details
        Public TradingCustomer As String  ' Trading Customer Details
        Public TradingCompany As String  ' Trading Company Name
        Public TradingConnection As String  ' Trading Connection string
        Public CustomerGroup As String
        Public VendorGroup As String
        Public ItemGroup As String

    End Structure

    Public p_oApps As SAPbouiCOM.SboGuiApi
    Public p_oEventHandler As clsEventHandler
    Public WithEvents p_oSBOApplication As SAPbouiCOM.Application
    Public p_oHoldingCompany As SAPbobsCOM.Company
    Public p_oTradingCompany As SAPbobsCOM.Company
    Public p_oUICompany As SAPbouiCOM.Company
    Public p_oCompDef As CompanyDefault
    Public sFuncName As String
    Public sErrDesc As String
    Public oCompanydetailsDT As DataTable
    Public oCompanydetailsDV As DataView
    Public oHoldingCompanyDV As DataView

    Public p_sSelectedFilepath As String = String.Empty
    Public p_sSelectedFileName As String = String.Empty
    Public p_sRefNuber(100, 4) As String
    Public p_iArrayCount As Integer = 0
    Public p_iArrayAcctCount As Integer = 0
    Public p_iArrayAcctActiveCount As Integer = 0
    Public p_sAccountCodes(100) As String
    Public p_sAccountCodes_ActiveAccount(100) As String
    Public oFormType As Integer = 0



    Sub main(ByVal Args() As String)

        Try

            ''' Changes GetSystemIntializeInfo(), ConnectToTradingCompany()

            sFuncName = "Main()"
            p_iDebugMode = DEBUG_ON
            p_iErrDispMethod = ERR_DISPLAY_STATUS

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Starting Addon startup function", sFuncName)
            p_oApps = New SAPbouiCOM.SboGuiApi
            'p_oApps.Connect(Args(0))

            Dim sconn As String = Environment.GetCommandLineArgs.GetValue(1)
            p_oApps.Connect(sconn)


            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Initializing public SBO Application object", sFuncName)
            p_oSBOApplication = p_oApps.GetApplication

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Retriving SBO application company handle", sFuncName)
            p_oUICompany = p_oSBOApplication.Company


            p_oHoldingCompany = New SAPbobsCOM.Company
            If Not p_oHoldingCompany.Connected Then
                If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling ConnectDICompSSO()", sFuncName)
                If ConnectDICompSSO(p_oHoldingCompany, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)
            End If

            ''If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling AddMenus Functions", sFuncName)
            ''Call AddMenuItems()
            ''If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("AddMenus Functions Completed Successfully.", sFuncName)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling GetSystemIntializeInfo()", sFuncName)
            If GetSystemIntializeInfo(p_oCompDef, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling AddMenus Functions", sFuncName)
            If GetCompanyDetails(p_oHoldingCompany, sErrDesc) <> RTN_SUCCESS Then Throw New ArgumentException(sErrDesc)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Creating Event handler class", sFuncName)
            p_oEventHandler = New clsEventHandler(p_oSBOApplication, p_oHoldingCompany)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Calling SetApplication Function", sFuncName)
            ' Call p_oEventHandler.SetApplication(sErrDesc)

            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Addon started successfully", "Main()")

            Call WriteToLogFile_Debug("Calling EndStatus()", sFuncName)
            ' Call EndStatus(sErrDesc)
            p_oSBOApplication.StatusBar.SetText("Addon Started Successfully", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success)

            System.Windows.Forms.Application.Run()



        Catch exp As Exception
            Call WriteToLogFile(exp.Message, "Main()")
            If p_iDebugMode = DEBUG_ON Then Call WriteToLogFile_Debug("Completed with ERROR", "Main()")
        Finally
        End Try
    End Sub

End Module





