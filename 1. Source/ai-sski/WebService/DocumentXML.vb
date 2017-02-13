Imports System.Collections.Generic
Imports System.Collections
Imports System.Linq
Imports System.Web
Imports System.Text
Imports System.Xml
Imports System.Data

Public Class DocumentXML
    Public Function ToXMLStringFromDS(ByVal ObjType As String, ByVal ds As DataSet) As String
        Try
            'Dim gf As New GeneralFunctions()
            Dim XmlString As New StringBuilder()
            Dim writer As XmlWriter = XmlWriter.Create(XmlString)
            writer.WriteStartDocument()
            If True Then
                writer.WriteStartElement("BOM")
                If True Then
                    writer.WriteStartElement("BO")
                    If True Then
                        '#Region "write ADMINFO_ELEMENT"
                        writer.WriteStartElement("AdmInfo")
                        If True Then
                            writer.WriteStartElement("Object")
                            If True Then
                                writer.WriteString(ObjType)
                            End If
                            writer.WriteEndElement()
                        End If
                        writer.WriteEndElement()
                        '#End Region

                        '#Region "Header&Line XML"
                        For Each dt As DataTable In ds.Tables
                            writer.WriteStartElement(dt.TableName.ToString())
                            If True Then
                                For Each row As DataRow In dt.Rows
                                    writer.WriteStartElement("row")
                                    If True Then
                                        For Each column As DataColumn In dt.Columns
                                            If column.DefaultValue.ToString() <> "xx_remove_xx" Then
                                                If column.ColumnName <> "No" Then
                                                    'phan lon cac table deu co column No nay
                                                    If row(column).ToString() <> "" Then
                                                        writer.WriteStartElement(column.ColumnName)
                                                        'Write Tag
                                                        If True Then
                                                            writer.WriteString(row(column).ToString())
                                                        End If
                                                        writer.WriteEndElement()
                                                    End If
                                                End If
                                            End If
                                        Next
                                    End If
                                    writer.WriteEndElement()
                                Next
                            End If
                            writer.WriteEndElement()
                        Next
                        '#End Region
                    End If
                    writer.WriteEndElement()
                End If
                writer.WriteEndElement()
            End If
            writer.WriteEndDocument()

            writer.Flush()

            Return XmlString.ToString()
        Catch ex As Exception
            Return ex.ToString()
        End Try
    End Function

End Class