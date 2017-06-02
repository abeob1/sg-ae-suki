Imports System.Reflection
Imports System.IO



Public Class clsLog

    Public Const RTN_SUCCESS As Int16 = 1
    Public Const RTN_ERROR As Int16 = 0
    Public Const DEBUG_ON As Int16 = 1
    Public Const DEBUG_OFF As Int16 = 0
    Public p_iErrDispMethod As Int16
    Public p_iDeleteDebugLog As Int16 = 0
    Public p_sLogDir As String
    Public path As String
    Private Const MAXFILESIZE_IN_MB As Int16 = 5
    Private Const LOG_FILE_ERROR As String = "ErrorLog"
    Private Const LOG_FILE_ERROR_ARCH As String = "ErrorLog_"
    Private Const LOG_FILE_DEBUG As String = "DebugLog"
    Private Const LOG_FILE_DEBUG_ARCH As String = "DebugLog_"
    Private Const FILE_SIZE_CHECK_ENABLE As Int16 = 1
    Private Const FILE_SIZE_CHECK_DISABLE As Int16 = 0


    Public Function WriteToLogFile_Debug(ByVal strErrText As String, ByVal strSourceName As String, Optional ByVal intCheckFileForDelete As Int16 = 1) As Long
        Dim functionReturnValue As Long = 0

        ' **********************************************************************************
        '   Function   :    WriteToLogFile_Debug()
        '   Purpose    :    This function checks if given input file name exists or not
        '
        '   Parameters :    ByVal strErrText As String
        '                       strErrText = Text to be written to the log
        '                   ByVal intLogType As Integer
        '                       intLogType = Log type (1 - Log ; 2 - Error ; 0 - None)
        '                   ByVal strSourceName As String
        '                       strSourceName = Function name calling this function
        '                   Optional ByVal intCheckFileForDelete As Integer
        '                       intCheckFileForDelete = Flag to indicate if file size need to be checked before logging (0 - No check ; 1 - Check)
        '   Return     :    0 - FAILURE
        '                   1 - SUCCESS
        '   Author     :    JOHN
        '   Date       :    MAY 2013
        '   Changes    : 
        '                   
        ' **********************************************************************************

        Dim oStreamWriter As StreamWriter = Nothing
        Dim strFileName As String = String.Empty
        Dim strArchFileName As String = String.Empty
        ' string strTempString = string.Empty;
        Dim strTempString As String = String.Empty

        Dim lngFileSizeInMB As Double = 0
        'int iFileCount = 0;


        Try

            If strSourceName.Length > 30 Then
                strTempString = strTempString.PadLeft(0)
            Else
                strTempString = strTempString.PadLeft(30 - strSourceName.Length)
            End If

            'strTempString = Space(IIF(Len(strSourceName) > 30, 0 ,30 - Len(strSourceName)))
            '    strSourceName = strTempString & strSourceName
            strSourceName = strTempString.ToString().Trim() + strSourceName.Trim()

            strErrText = Convert.ToString((Convert.ToString("[" + String.Format(DateTime.Now.ToString(), "MM/dd/yyyy HH:mm:ss") + "]" + "[") & strSourceName) + "] ") & strErrText

            'strFileName = p_sLogDir + "\\" + LOG_FILE_DEBUG + ".log";
            'strArchFileName = p_sLogDir + "\\" + LOG_FILE_DEBUG_ARCH + string.Format(DateTime.Now.ToString(), "yyMMddHHMMss") + ".log";

            Dim codeBase As String = Assembly.GetExecutingAssembly().CodeBase
            Dim uri__1 As New UriBuilder(codeBase)
            Dim Datapath As String = Uri.UnescapeDataString(uri__1.Path)
            path = System.IO.Path.GetDirectoryName(Datapath)

            strFileName = (Convert.ToString(Path & Convert.ToString("\")) & LOG_FILE_DEBUG) + ".log"
            strArchFileName = (Convert.ToString(Path & Convert.ToString("\")) & LOG_FILE_DEBUG_ARCH) + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log"

            If intCheckFileForDelete = FILE_SIZE_CHECK_ENABLE Then
                If File.Exists(strFileName) Then
                    Dim fi As New FileInfo(strFileName)

                    lngFileSizeInMB = (fi.Length / 1024) / 1024

                    If lngFileSizeInMB >= MAXFILESIZE_IN_MB Then
                        'If intCheckDeleteDebugLog=1 then remove all debug_log file
                        If p_iDeleteDebugLog = 1 Then
                            For Each sFileName As String In Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), LOG_FILE_DEBUG_ARCH & Convert.ToString("*"))
                                File.Delete(sFileName)
                            Next
                        End If
                        File.Move(strFileName, strArchFileName)
                    End If
                End If
            End If
            oStreamWriter = File.AppendText(strFileName)
            oStreamWriter.WriteLine(strErrText)
            functionReturnValue = RTN_SUCCESS
        Catch generatedExceptionName As Exception
            functionReturnValue = RTN_ERROR
        Finally
            If (oStreamWriter IsNot Nothing) Then
                oStreamWriter.Flush()
                oStreamWriter.Close()
                oStreamWriter = Nothing
            End If
        End Try
        Return functionReturnValue

    End Function

    Public Function WriteToLogFile(ByVal strErrText As String, ByVal strSourceName As String, Optional ByVal intCheckFileForDelete As Int16 = 1) As Long
        Dim functionReturnValue As Long = 0

        ' **********************************************************************************
        '   Function   :    WriteToLogFile()
        '   Purpose    :    This function checks if given input file name exists or not
        '
        '   Parameters :    ByVal strErrText As String
        '                       strErrText = Text to be written to the log
        '                   ByVal intLogType As Integer
        '                       intLogType = Log type (1 - Log ; 2 - Error ; 0 - None)
        '                   ByVal strSourceName As String
        '                       strSourceName = Function name calling this function
        '                   Optional ByVal intCheckFileForDelete As Integer
        '                       intCheckFileForDelete = Flag to indicate if file size need to be checked before logging (0 - No check ; 1 - Check)
        '   Return     :    0 - FAILURE
        '                   1 - SUCCESS
        '   Author     :    JOHN
        '   Date       :    MAY 2014
        ' **********************************************************************************

        Dim oStreamWriter As StreamWriter = Nothing
        Dim strFileName As String = String.Empty
        Dim strArchFileName As String = String.Empty
        ' string strTempString = string.Empty;
        Dim lngFileSizeInMB As Double = 0
        Dim strTempString As String = String.Empty

        Try
            If strSourceName.Length > 30 Then
                strTempString = strTempString.PadLeft(0)
            Else
                strTempString = strTempString.PadLeft(30 - strSourceName.Length)
            End If


            strSourceName = strTempString.ToString() & strSourceName

            strErrText = Convert.ToString((Convert.ToString("[" + String.Format(DateTime.Now.ToString(), "MM/dd/yyyy HH:mm:ss") + "]" + "[") & strSourceName) + "] ") & strErrText

            'strFileName = p_sLogDir + "\\" + LOG_FILE_DEBUG + ".log";
            'strArchFileName = p_sLogDir + "\\" + LOG_FILE_DEBUG_ARCH + string.Format(DateTime.Now.ToString(), "yyMMddHHMMss") + ".log";

            Dim codeBase As String = Assembly.GetExecutingAssembly().CodeBase
            Dim uri__1 As New UriBuilder(codeBase)
            Dim Datapath As String = Uri.UnescapeDataString(uri__1.Path)
            path = System.IO.Path.GetDirectoryName(Datapath)

            strFileName = (Convert.ToString(Path & Convert.ToString("\")) & LOG_FILE_ERROR) + ".log"
            strArchFileName = (Convert.ToString(Path & Convert.ToString("\")) & LOG_FILE_ERROR_ARCH) + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log"

            'strFileName = System.IO.Directory.GetCurrentDirectory() + "\\" + LOG_FILE_DEBUG + ".log";
            'strArchFileName = System.IO.Directory.GetCurrentDirectory() + "\\" + LOG_FILE_DEBUG_ARCH + string.Format(DateTime.Now.ToString(), "yyMMddHHMMss") + ".log";

            If intCheckFileForDelete = FILE_SIZE_CHECK_ENABLE Then
                If File.Exists(strFileName) Then
                    Dim fi As New FileInfo(strFileName)

                    lngFileSizeInMB = (fi.Length / 1024) / 1024

                    If lngFileSizeInMB >= MAXFILESIZE_IN_MB Then
                        'If intCheckDeleteDebugLog=1 then remove all debug_log file
                        If p_iDeleteDebugLog = 1 Then
                            For Each sFileName As String In Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), LOG_FILE_ERROR_ARCH & Convert.ToString("*"))
                                File.Delete(sFileName)
                            Next
                        End If
                        File.Move(strFileName, strArchFileName)
                    End If
                End If
            End If
            oStreamWriter = File.AppendText(strFileName)
            oStreamWriter.WriteLine(strErrText)
            functionReturnValue = RTN_SUCCESS
        Catch generatedExceptionName As Exception
            functionReturnValue = RTN_ERROR
        Finally
            If (oStreamWriter IsNot Nothing) Then
                oStreamWriter.Flush()
                oStreamWriter.Close()
                oStreamWriter = Nothing
            End If
        End Try
        Return functionReturnValue
    End Function
End Class
