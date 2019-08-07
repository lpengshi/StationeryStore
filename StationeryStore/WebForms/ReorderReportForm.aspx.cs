using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StationeryStore.WebForms
{
    public partial class ReorderReportForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //ReportParameter searchText = new ReportParameter("Search", search.Text);
            //ReportViewer2.LocalReport.SetParameters(searchText);
            ReportViewer2.LocalReport.Refresh();
        }
    }
}