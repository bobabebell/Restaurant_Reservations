using System;
using System.Collections;
using System.Data;
using Data;
using DayPilot.Web.Ui.Recurrence;
using Util;

public partial class Assignment : System.Web.UI.Page
{
    private RecurrenceRule _rule = RecurrenceRule.NoRepeat;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            DataRow assignment = new DataManager().GetAssignment(Convert.ToInt32(Request.QueryString["id"]));

            TextBoxName.Text = Convert.ToString(assignment["TaskName"]);
            DropDownDuration.SelectedValue = Convert.ToString(assignment["TaskDuration"]);

            LabelStart.Text = String.Format("{0}", assignment["AssignmentStart"]);
            LabelEnd.Text = String.Format("{0}", assignment["AssignmentEnd"]);
            LabelLocation.Text = String.Format("{0}", assignment["LocationName"]);

            DataBind();
        }
    }

    protected void ButtonDelete_Click(object sender, EventArgs e)
    {
        new DataManager().DeleteAssignment(Convert.ToInt32(Request.QueryString["id"]));
        Hashtable ht = new Hashtable();
        ht["refresh"] = true;
        ht["message"] = "Assignment deleted.";
        Modal.Close(this, ht);
    }

    protected void ButtonCancel_Click(object sender, EventArgs e)
    {
        Modal.Close(this);
    }

}