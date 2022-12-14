<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebDashboardSample.WebForm1" %>

<%@ Register assembly="DevExpress.Dashboard.v21.2.Web.WebForms, Version=21.2.12.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.DashboardWeb" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <script type="text/javascript">
    function onBeforeRender(sender) {
        var control = sender.GetDashboardControl()
        control.registerExtension(new DevExpress.Dashboard.DashboardPanelExtension(control))
        control.registerExtension(new ChartConstantLinesExtension(control))
    }
</script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxDashboard ID="ASPxDashboard1" runat="server" ClientSideEvents-BeforeRender="onBeforeRender">
            </dx:ASPxDashboard>
        </div>
    </form>
   <script src="Content/Extensions/ChartConstantLinesExtension.js"></script>
</body>
</html>
