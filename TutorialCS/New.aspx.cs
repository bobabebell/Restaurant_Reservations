using System;
using System.Collections;
using Data;
using Util;

public partial class New : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            TextBoxStart.Text = Convert.ToDateTime(Request.QueryString["start"]).ToString();
            TextBoxEnd.Text = Convert.ToDateTime(Request.QueryString["end"]).ToString();

            TextBoxNote.Focus();

            DropDownListLocation.DataSource = new DataManager().GetLocationsChildren();

            if (!String.IsNullOrEmpty(Request.QueryString["table"]))
            {
                DropDownListLocation.SelectedValue = Request.QueryString["table"];
            }

            DataBind();
        }
    }

    protected void ButtonOK_Click(object sender, EventArgs e)
    {
        DateTime start = Convert.ToDateTime(TextBoxStart.Text);
        DateTime end = Convert.ToDateTime(TextBoxEnd.Text);
        string note = TextBoxNote.Text;
        int location = Convert.ToInt32(DropDownListLocation.SelectedValue);

        new DataManager().CreateAssignment(start, end, location, note);

        // passed to the modal dialog close handler, see Scripts/DayPilot/event_handling.js
        Hashtable ht = new Hashtable();
        ht["refresh"] = "yes";
        ht["message"] = "Reservation created.";

        Modal.Close(this, ht);
    }

    protected void ButtonCancel_Click(object sender, EventArgs e)
    {
        Modal.Close(this);
    }
}
