<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="WebViewerFx.aspx.cs" Inherits="WebViewerFx" %>

<%@ Register Assembly="Stimulsoft.Report.WebFx, Version=2012.2.1304.0, Culture=neutral, PublicKeyToken=ebe6666cba19647a" Namespace="Stimulsoft.Report.WebFx" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Просмотр отчета...</title>
    <style type="text/css">
        html, body, form { height: 100%;width: 100%; margin: 0px; }
    </style>

</head>
<body>
    <form id="form1" runat="server">
    <div style="height: inherit;" align="left" >
            <cc1:StiWebViewerFx ID="StiWebViewerFx1" runat="server" Width="100%" 
                Height="100%" Localization="ru" LocalizationDirectory="Localization" 
                ShowExportToHtml="False" ShowExportToExcel="False"
                ShowExportToBmp="False" ShowExportToCsv="False" ShowExportToDbf="False" 
                ShowExportToDif="False" ShowExportToDocument="False" 
                ShowExportToExcelXml="False" ShowExportToGif="False" ShowExportToJpeg="False" 
                ShowExportToMetafile="False" ShowExportToOpenDocumentCalc="False" 
                ShowExportToOpenDocumentWriter="False" ShowExportToPcx="False" 
                ShowExportToPpt="False" ShowExportToRtf="False" ShowExportToSvg="False" 
                ShowExportToSvgz="False" ShowExportToSylk="False" ShowExportToText="False" 
                ShowExportToTiff="False" ShowExportToXml="False"
                
                 />
    </div>
    </form>
</body>
</html>
