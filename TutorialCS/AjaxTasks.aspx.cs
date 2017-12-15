using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Data;

public partial class AjaxTasks : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var data = new DataManager().GetTasks();
        Repeater1.DataSource = data;
        Repeater1.DataBind();

        if (data.Rows.Count == 0)
        {
            LabelEmpty.Visible = true;
        }
        
    }
}