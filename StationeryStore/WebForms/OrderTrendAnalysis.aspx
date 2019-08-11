<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderTrendAnalysis.aspx.cs" Inherits="StationeryStore.WebForms.OrderTrendAnalysis" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <strong>Start Month/Year:</strong>
            <asp:DropDownList ID="startMonth" runat="server" DataSourceID="SqlDataSource2" DataTextField="MonthName" DataValueField="MonthInt" AppendDataBoundItems="true">
            <Items>
                <asp:ListItem Text="Select" Value="0" />
            </Items>
            </asp:DropDownList>
            &nbsp;<strong><asp:DropDownList ID="startYear" runat="server" DataSourceID="SqlDataSource1" DataTextField="YearInt" DataValueField="YearInt" AppendDataBoundItems="true">
            <Items>
                <asp:ListItem Text="Select" Value="0" />
            </Items>
            </asp:DropDownList>
            </strong>&nbsp;&nbsp; <strong>End Month/Year:</strong>
            <asp:DropDownList ID="endMonth" runat="server" DataSourceID="SqlDataSource2" DataTextField="MonthName" DataValueField="MonthInt" AppendDataBoundItems="true">
            <Items>
                <asp:ListItem Text="Select" Value="0" />
            </Items>
            </asp:DropDownList>
&nbsp;<asp:DropDownList ID="endYear" runat="server" DataSourceID="SqlDataSource1" DataTextField="YearInt" DataValueField="YearInt" AppendDataBoundItems="true">
            <Items>
                <asp:ListItem Text="Select" Value="0" />
            </Items>            
            </asp:DropDownList>
            &nbsp;&nbsp;
            <asp:Button ID="Button1" runat="server" Text="Filter" OnClick="Button1_Click" />
            &nbsp;
            <br />
            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:StoreConnectionString %>" SelectCommand="SELECT DATENAME(m, DATEADD(S, OrderDate, '1970-01-01')) AS MonthName, DATEPART(m, dateadd(S, OrderDate,'1970-01-01')) AS MonthInt, 
	DATEPART(yyyy, dateadd(S, OrderDate,'1970-01-01')) AS YearInt
FROM PurchaseOrder 
GROUP BY DATENAME(m, DATEADD(S, OrderDate, '1970-01-01')), DATEPART(m, dateadd(S, OrderDate,'1970-01-01')), 
	DATEPART(yyyy, dateadd(S, OrderDate,'1970-01-01'))"></asp:SqlDataSource>
            <strong>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:StoreConnectionString %>" SelectCommand="SELECT DATEPART(yyyy, DATEADD(S, OrderDate, '1970-01-01')) AS YearInt FROM PurchaseOrder GROUP BY DATEPART(yyyy, DATEADD(S, OrderDate, '1970-01-01'))"></asp:SqlDataSource>
            </strong>
            <br />
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" BackColor="" ClientIDMode="AutoID" Height="458px" HighlightBackgroundColor="" InternalBorderColor="204, 204, 204" InternalBorderStyle="Solid" InternalBorderWidth="1px" LinkActiveColor="" LinkActiveHoverColor="" LinkDisabledColor="" PrimaryButtonBackgroundColor="" PrimaryButtonForegroundColor="" PrimaryButtonHoverBackgroundColor="" PrimaryButtonHoverForegroundColor="" SecondaryButtonBackgroundColor="" SecondaryButtonForegroundColor="" SecondaryButtonHoverBackgroundColor="" SecondaryButtonHoverForegroundColor="" SplitterBackColor="" ToolbarDividerColor="" ToolbarForegroundColor="" ToolbarForegroundDisabledColor="" ToolbarHoverBackgroundColor="" ToolbarHoverForegroundColor="" ToolBarItemBorderColor="" ToolBarItemBorderStyle="Solid" ToolBarItemBorderWidth="1px" ToolBarItemHoverBackColor="" ToolBarItemPressedBorderColor="51, 102, 153" ToolBarItemPressedBorderStyle="Solid" ToolBarItemPressedBorderWidth="1px" ToolBarItemPressedHoverBackColor="153, 187, 226" Width="100%" SizeToReportContent="True">
                <LocalReport ReportPath="Reports\OrderTrendAnalysis.rdlc">
                    <DataSources>
                        <rsweb:ReportDataSource DataSourceId="SqlDataSource3" Name="DataSet1" />
                    </DataSources>
                </LocalReport>
            </rsweb:ReportViewer>
            <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:StoreConnectionString %>" SelectCommand="OrderTrend" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:FormParameter DefaultValue="0" FormField="startMonth" Name="StartMonth" ConvertEmptyStringToNull="False" Type="Int32" />
                    <asp:FormParameter DefaultValue="0" FormField="endMonth" Name="EndMonth" ConvertEmptyStringToNull="False" Type="Int32" />
                    <asp:FormParameter ConvertEmptyStringToNull="False" DefaultValue="0" FormField="startYear" Name="StartYear" Type="Int32" />
                    <asp:FormParameter ConvertEmptyStringToNull="False" DefaultValue="0" FormField="endYear" Name="EndYear" Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>
            <br />
        </div>
        <br /><br />
    </form>
</body>
</html>
