using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml;
using System.Data;

namespace Suki
{
    public class DocumentXML
    {
       public String ToXMLStringFromDS(String ObjType,DataSet ds)
        {
            try
            {
                GeneralFunctions gf = new GeneralFunctions();
                StringBuilder XmlString = new StringBuilder();
                XmlWriter writer = XmlWriter.Create(XmlString);
                writer.WriteStartDocument();
                {
                    writer.WriteStartElement("BOM");
                    {
                        writer.WriteStartElement("BO");
                        {
                            #region write ADMINFO_ELEMENT
                            writer.WriteStartElement("AdmInfo");
                            {
                                writer.WriteStartElement("Object");
                                {
                                    writer.WriteString(ObjType);
                                }
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                            #endregion

                            #region Header&Line XML
                            foreach (DataTable dt in ds.Tables)
                            {
                                writer.WriteStartElement(dt.TableName.ToString());
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        writer.WriteStartElement("row");
                                        {
                                            foreach (DataColumn column in dt.Columns)
                                            {
                                                if(column.DefaultValue.ToString()!="xx_remove_xx")
                                                {
                                                    if (column.ColumnName != "No")//phan lon cac table deu co column No nay
                                                    {
                                                        if (row[column].ToString() != "")
                                                        {
                                                            writer.WriteStartElement(column.ColumnName); //Write Tag
                                                            {
                                                                writer.WriteString(row[column].ToString());
                                                            }
                                                            writer.WriteEndElement();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        writer.WriteEndElement();
                                    }
                                }
                                writer.WriteEndElement();
                            }
                            #endregion
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndDocument();

                writer.Flush();

                return XmlString.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }  
    }
}