using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StationeryStore.WebForms
{
    public partial class RequisitionTrendAnalysis : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            startMonth.SelectedIndex = 2;
            startYear.SelectedIndex = 0;
            endMonth.SelectedIndex = 0;
            endYear.SelectedIndex = 0;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            ReportViewer1.LocalReport.Refresh();
        }
    }
}