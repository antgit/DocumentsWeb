<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebViewer.aspx.cs" Inherits="WebViewer" %>

<%@ Register Assembly="Stimulsoft.Report.Web, Version=2012.2.1304.0, Culture=neutral, PublicKeyToken=ebe6666cba19647a" Namespace="Stimulsoft.Report.Web" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Просмотр отчета...</title>
    
</head>
<body>
    <form id="form1" runat="server" method="post">
    <div style="height: inherit;">
        <cc1:StiWebViewer ID="StiWebViewer1" runat="server" Width="100%" Height="100%" ZoomPercent="100"
                          RenderMode="AjaxWithCache" GlobalizationFile="Localization/ru.xml" 
                          ShowExportToHtml="False" ShowExportToExcel="False"
                          ShowExportToBmp="False" ShowExportToCsv="False" ShowExportToDbf="False" 
                          ShowExportToDif="False" ShowExportToDocument="False" 
                          ShowExportToExcelXml="False" ShowExportToGif="False" ShowExportToJpeg="False" 
                          ShowExportToMetafile="False" ShowExportToOds="False" ShowExportToOdt="False" 
                          ShowExportToPcx="False" ShowExportToPowerPoint="False" ShowExportToRtf="False" 
                          ShowExportToSvg="False" ShowExportToSvgz="False" ShowExportToSylk="False" 
                          ShowExportToText="False" ShowExportToTiff="False" ShowExportToXml="False"
                          ToolBarBackColor="WhiteSmoke" ImagesPath="Images/"
                          Theme = "Office2010" 
                          OnGetReportData="StiWebViewer1_GetReportData"
                          />
    </div>
    </form>
</body>
</html>
