using System;
using System.Collections;
using System.Data;
using Data;
using DayPilot.Web.Ui.Recurrence;
using Util;

public partial class Edit : System.Web.UI.Page
{
    private RecurrenceRule _rule = RecurrenceRule.NoRepeat;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            DataRow dr = new DataManager().GetAssignment(Convert.ToInt32(Request.QueryString["id"]));

            TextBoxStart.Text = ((DateTime)dr["AssignmentStart"]).ToString();
            TextBoxEnd.Text = ((DateTime)dr["AssignmentEnd"]).ToString();
            TextBoxUpdated.Text = ((DateTime)dr["AssignmentPriority"]).ToString();
            TextBoxNote.Text = Convert.ToString(dr["AssignmentNote"]);
            DropDownListLocation.SelectedValue = Convert.ToString(dr["LocationId"]);

            DropDownListLocation.DataSource = new DataManager().GetLocationsChildren();

            TextBoxNote.Focus();

            DataBind();
        }
    }

    protected void ButtonOK_Click(object sender, EventArgs e)
    {
        DateTime start = Convert.ToDateTime(TextBoxStart.Text);
        DateTime end = Convert.ToDateTime(TextBoxEnd.Text);
        string note = TextBoxNote.Text;
        int location = Convert.ToInt32(DropDownListLocation.SelectedValue);

        new DataManager().UpdateAssignment(Convert.ToInt32(Request.QueryString["id"]), start, end, location, note);

        Hashtable ht = new Hashtable();
        ht["refresh"] = "yes";
        ht["message"] = "Reservation updated.";

        Modal.Close(this, ht);
    }

    protected void ButtonCancel_Click(object sender, EventArgs e)
    {
        Modal.Close(this);
    }

    protected void ButtonDelete_Click(object sender, EventArgs e)
    {
        new DataManager().DeleteAssignment(Convert.ToInt32(Request.QueryString["id"]));
        Hashtable ht = new Hashtable();
        ht["refresh"] = "yes";
        ht["message"] = "Reservation deleted.";
        Modal.Close(this, ht);
    }
}