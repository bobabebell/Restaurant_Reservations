using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Data;
using DayPilot.Json;
using DayPilot.Utils;
using DayPilot.Web.Ui;
using DayPilot.Web.Ui.Enums;
using DayPilot.Web.Ui.Events;
using DayPilot.Web.Ui.Events.Scheduler;
using CommandEventArgs = DayPilot.Web.Ui.Events.CommandEventArgs;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadResources();
            LoadSeparators();

            DayPilotScheduler1.TimeHeaders.Clear();
            DayPilotScheduler1.TimeHeaders.Add(new TimeHeader(GroupByEnum.Day, "dddd, d MMMM yyyy"));
            DayPilotScheduler1.TimeHeaders.Add(new TimeHeader(GroupByEnum.Hour));
            DayPilotScheduler1.TimeHeaders.Add(new TimeHeader(GroupByEnum.Cell));

            DayPilotScheduler1.StartDate = Week.FirstDayOfWeek(DateTime.Today);
            DayPilotScheduler1.Days = 7;
            DayPilotScheduler1.DataSource = new DataManager().GetAssignments(DayPilotScheduler1);
            DayPilotScheduler1.DataBind();

            DayPilotScheduler1.SetScrollX(DateTime.Today);
        }
    }

    private FilterData Filter
    {
        get
        {
            return new FilterData(DayPilotScheduler1.ClientState["filter"]);
        }
    }

    private void LoadResources()
    {
        DataTable locations = new DataManager().GetLocations();
        DayPilotScheduler1.Resources.Clear();
        foreach (DataRow location in locations.Rows)
        {
            int id = Convert.ToInt32(location["LocationId"]);

            Resource r = new Resource((string)location["LocationName"], null); // using null for resources that can't be used
            r.IsParent = true; // marking as parent for the case that no children are loaded
            r.Expanded = true;
            DayPilotScheduler1.Resources.Add(r);

            DataTable rooms;

            if (Filter.IsTime)
            {
                rooms = new DataManager().GetLocationsFiltered(id, Filter.Start, Filter.End, Filter.Seats);
            }
            else
            {
                rooms = new DataManager().GetLocations(id, Filter.Seats);
            }

            
            foreach (DataRow dr in rooms.Rows)
            {
                Resource c = new Resource((string)dr["LocationName"], Convert.ToString(dr["LocationId"]));
                c.Columns.Add(new ResourceColumn(dr["LocationSeats"] + " seats"));
                r.Children.Add(c);
            }
        }
    }

    protected void LoadSeparators()
    {

        DayPilotScheduler1.Separators.Clear();
        DayPilotScheduler1.Separators.Add(DateTime.Now, Color.Red);

        if (Filter.IsTime)
        {
            int width = (int)((Filter.End - Filter.Start).TotalMinutes / DayPilotScheduler1.CellDuration * DayPilotScheduler1.CellWidth);
            DayPilotScheduler1.Separators.Add(Filter.Start, ColorTranslator.FromHtml("#6abc49"), SeparatorLayer.BelowEvents, width, 50);
        }
    }

    protected void DayPilotScheduler1_EventMove(object sender, EventMoveEventArgs e)
    {
        // check if this row is a courtroom
        if (e.NewResource == null)
        {
            DayPilotScheduler1.DataSource = new DataManager().GetAssignments(DayPilotScheduler1);
            DayPilotScheduler1.DataBind();
            DayPilotScheduler1.UpdateWithMessage("Sorry, the reservation can't be created here.");
            return;
        }

        // check the business hours (11 - 18)
        if (e.NewStart.Hour < DayPilotScheduler1.BusinessBeginsHour || e.NewEnd > e.NewEnd.Date.AddHours(DayPilotScheduler1.BusinessEndsHour))
        {
            DayPilotScheduler1.DataSource = new DataManager().GetAssignments(DayPilotScheduler1);
            DayPilotScheduler1.DataBind();
            DayPilotScheduler1.UpdateWithMessage("Sorry, it's outside of working hours.");
            return;
        }

        new DataManager().MoveAssignment(Convert.ToInt32(e.Value), e.NewStart, e.NewEnd, Convert.ToInt32(e.NewResource));
        DayPilotScheduler1.UpdateWithMessage("The reservation has been updated.");

        DayPilotScheduler1.DataSource = new DataManager().GetAssignments(DayPilotScheduler1);
        DayPilotScheduler1.DataBind();

    }

    protected void DayPilotScheduler1_BeforeCellRender(object sender, BeforeCellRenderEventArgs e)
    {
    }

    protected void DayPilotScheduler1_BeforeEventRender(object sender, BeforeEventRenderEventArgs e)
    {
    }

    protected void DayPilotScheduler1_Command(object sender, CommandEventArgs e)
    {
        switch (e.Command)
        {
            case "refresh":
                LoadResources();
                LoadSeparators();
                DayPilotScheduler1.DataSource = new DataManager().GetAssignments(DayPilotScheduler1);
                DayPilotScheduler1.DataBind();
                if (e.Data != null && e.Data["message"] != null)
                {
                    DayPilotScheduler1.UpdateWithMessage((string)e.Data["message"]);
                }
                else
                {
                    DayPilotScheduler1.UpdateWithMessage("Updated.");
                }
        
                break;
            case "clear":
                DayPilotScheduler1.ClientState.Clear();
                LoadResources();
                LoadSeparators();
                DayPilotScheduler1.DataSource = new DataManager().GetAssignments(DayPilotScheduler1);
                DayPilotScheduler1.DataBind();
                DayPilotScheduler1.UpdateWithMessage("Filter cleared.");
                break;
        }
    }

    protected void DayPilotScheduler1_TimeHeaderClick(object sender, TimeHeaderClickEventArgs e)
    {
        DayPilotScheduler1.DataSource = new DataManager().GetAssignments(DayPilotScheduler1);
        DayPilotScheduler1.DataBind();

        JsonData time = new JsonData();
        time["name"] = "Time";
        time["value"] = String.Format("{0} - {1}", TimeFormatter.GetHourMinutes(e.Start, DayPilotScheduler1.TimeFormat), TimeFormatter.GetHourMinutes(e.End, DayPilotScheduler1.TimeFormat));
        time["start"] = e.Start.ToString("s");
        time["end"] = e.End.ToString("s");

        if (DayPilotScheduler1.ClientState["filter"] == null)
        {
            DayPilotScheduler1.ClientState["filter"] = new JsonData();
        }
        DayPilotScheduler1.ClientState["filter"]["time"] = time;

        LoadResources();
        LoadSeparators();

        DayPilotScheduler1.Update();

    }


    private class FilterData
    {
        internal DateTime Start = DateTime.MinValue;
        internal DateTime End = DateTime.MaxValue;
        internal bool IsTime = false;
        internal int Seats = 0;

        internal FilterData(JsonData filter)
        {
            if (filter == null)
            {
                return;
            }

            if (filter["time"] != null)
            {
                IsTime = true;
                Start = (DateTime)filter["time"]["start"];
                End = (DateTime)filter["time"]["end"];
            }

            if (filter["seats"] != null)
            {
                Seats = (int) filter["seats"]["count"];
            }
        }
    }

    protected void DayPilotScheduler1_BeforeTimeHeaderRender(object sender, BeforeTimeHeaderRenderEventArgs e)
    {
        if (e.Start.DayOfWeek == DayOfWeek.Saturday || e.Start.DayOfWeek == DayOfWeek.Sunday)
        {
            if (e.End.TimeOfDay.TotalHours <= DayPilotScheduler1.BusinessBeginsHour)
            {
                e.Visible = false;
            }
            else if (e.Start.TimeOfDay.TotalHours >= DayPilotScheduler1.BusinessEndsHour)
            {
                e.Visible = false;
            }
            else
            {
                e.Visible = true;
            }
        }
    }
}
