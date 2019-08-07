﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryStatusReportForm.aspx.cs" Inherits="StationeryStore.WebForms.WebForm1" %>

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
            <strong>Search:</strong>
            <asp:TextBox ID="code" runat="server" OnTextChanged="code_TextChanged"></asp:TextBox>
            &nbsp;&nbsp;
            <asp:Button ID="Button1" runat="server" Text="Filter" OnClick="Button1_Click" />
            <br />
            <br />
        </div>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" BackColor="" ClientIDMode="AutoID" Height="531px" HighlightBackgroundColor="" InternalBorderColor="204, 204, 204" InternalBorderStyle="Solid" InternalBorderWidth="1px" LinkActiveColor="" LinkActiveHoverColor="" LinkDisabledColor="" PrimaryButtonBackgroundColor="" PrimaryButtonForegroundColor="" PrimaryButtonHoverBackgroundColor="" PrimaryButtonHoverForegroundColor="" SecondaryButtonBackgroundColor="" SecondaryButtonForegroundColor="" SecondaryButtonHoverBackgroundColor="" SecondaryButtonHoverForegroundColor="" SplitterBackColor="" ToolbarDividerColor="" ToolbarForegroundColor="" ToolbarForegroundDisabledColor="" ToolbarHoverBackgroundColor="" ToolbarHoverForegroundColor="" ToolBarItemBorderColor="" ToolBarItemBorderStyle="Solid" ToolBarItemBorderWidth="1px" ToolBarItemHoverBackColor="" ToolBarItemPressedBorderColor="51, 102, 153" ToolBarItemPressedBorderStyle="Solid" ToolBarItemPressedBorderWidth="1px" ToolBarItemPressedHoverBackColor="153, 187, 226" Width="804px">
            <LocalReport ReportPath="Reports\InventoryStatusReport.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="SqlDataSource1" Name="DataSet1" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:StoreConnectionString %>" SelectCommand="GetInventoryStatusReport" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:FormParameter DefaultValue="" FormField="code" Name="ItemCode" Type="String" ConvertEmptyStringToNull="False" />
            </SelectParameters>
        </asp:SqlDataSource>
        <br />
    </form>
</body>
</html>