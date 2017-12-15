Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Data
Imports DayPilot.Json
Imports DayPilot.Utils
Imports DayPilot.Web.Ui
Imports DayPilot.Web.Ui.Enums
Imports DayPilot.Web.Ui.Events
Imports DayPilot.Web.Ui.Events.Scheduler
Imports CommandEventArgs = DayPilot.Web.Ui.Events.CommandEventArgs

Partial Public Class _Default
	Inherits System.Web.UI.Page

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Not IsPostBack Then
			LoadResources()
			LoadSeparators()

			DayPilotScheduler1.TimeHeaders.Clear()
            DayPilotScheduler1.TimeHeaders.Add(New TimeHeader(GroupByEnum.Day, "dddd, d MMMM yyyy"))
			DayPilotScheduler1.TimeHeaders.Add(New TimeHeader(GroupByEnum.Hour))
            DayPilotScheduler1.TimeHeaders.Add(New TimeHeader(GroupByEnum.Cell))

			DayPilotScheduler1.StartDate = Week.FirstDayOfWeek(Date.Today)
			DayPilotScheduler1.Days = 7
			DayPilotScheduler1.DataSource = (New DataManager()).GetAssignments(DayPilotScheduler1)
			DayPilotScheduler1.DataBind()

			DayPilotScheduler1.SetScrollX(Date.Today)
		End If
	End Sub

	Private ReadOnly Property Filter() As FilterData
		Get
			Return New FilterData(DayPilotScheduler1.ClientState("filter"))
		End Get
	End Property

	Private Sub LoadResources()
		Dim locations As DataTable = (New DataManager()).GetLocations()
		DayPilotScheduler1.Resources.Clear()
		For Each location As DataRow In locations.Rows
			Dim id As Integer = Convert.ToInt32(location("LocationId"))

			Dim r As New Resource(CStr(location("LocationName")), Nothing) ' using null for resources that can't be used
			r.IsParent = True ' marking as parent for the case that no children are loaded
			r.Expanded = True
			DayPilotScheduler1.Resources.Add(r)

			Dim rooms As DataTable

			If Filter.IsTime Then
				rooms = (New DataManager()).GetLocationsFiltered(id, Filter.Start, Filter.End, Filter.Seats)
			Else
				rooms = (New DataManager()).GetLocations(id, Filter.Seats)
			End If


			For Each dr As DataRow In rooms.Rows
				Dim c As New Resource(CStr(dr("LocationName")), Convert.ToString(dr("LocationId")))
				c.Columns.Add(New ResourceColumn(dr("LocationSeats") & " seats"))
				r.Children.Add(c)
			Next dr
		Next location
	End Sub

	Protected Sub LoadSeparators()

		DayPilotScheduler1.Separators.Clear()
		DayPilotScheduler1.Separators.Add(Date.Now, Color.Red)

		If Filter.IsTime Then
			Dim width As Integer = CInt(Fix((Filter.End.Subtract(Filter.Start)).TotalMinutes / DayPilotScheduler1.CellDuration * DayPilotScheduler1.CellWidth))
			DayPilotScheduler1.Separators.Add(Filter.Start, ColorTranslator.FromHtml("#6abc49"), SeparatorLayer.BelowEvents, width, 50)
		End If
	End Sub

	Protected Sub DayPilotScheduler1_EventMove(ByVal sender As Object, ByVal e As EventMoveEventArgs)
		' check if this row is a courtroom
		If e.NewResource Is Nothing Then
			DayPilotScheduler1.DataSource = (New DataManager()).GetAssignments(DayPilotScheduler1)
			DayPilotScheduler1.DataBind()
			DayPilotScheduler1.UpdateWithMessage("Sorry, the reservation can't be created here.")
			Return
		End If

		' check the business hours (11 - 18)
		If e.NewStart.Hour < DayPilotScheduler1.BusinessBeginsHour OrElse e.NewEnd > e.NewEnd.Date.AddHours(DayPilotScheduler1.BusinessEndsHour) Then
			DayPilotScheduler1.DataSource = (New DataManager()).GetAssignments(DayPilotScheduler1)
			DayPilotScheduler1.DataBind()
			DayPilotScheduler1.UpdateWithMessage("Sorry, it's outside of working hours.")
			Return
		End If

		CType(New DataManager(), DataManager).MoveAssignment(Convert.ToInt32(e.Value), e.NewStart, e.NewEnd, Convert.ToInt32(e.NewResource))
		DayPilotScheduler1.UpdateWithMessage("The reservation has been updated.")

		DayPilotScheduler1.DataSource = (New DataManager()).GetAssignments(DayPilotScheduler1)
		DayPilotScheduler1.DataBind()

	End Sub

	Protected Sub DayPilotScheduler1_BeforeCellRender(ByVal sender As Object, ByVal e As BeforeCellRenderEventArgs)
	End Sub

	Protected Sub DayPilotScheduler1_BeforeEventRender(ByVal sender As Object, ByVal e As BeforeEventRenderEventArgs)
	End Sub

	Protected Sub DayPilotScheduler1_Command(ByVal sender As Object, ByVal e As CommandEventArgs)
		Select Case e.Command
			Case "refresh"
				LoadResources()
				LoadSeparators()
				DayPilotScheduler1.DataSource = (New DataManager()).GetAssignments(DayPilotScheduler1)
				DayPilotScheduler1.DataBind()
				If e.Data IsNot Nothing AndAlso e.Data("message") IsNot Nothing Then
					DayPilotScheduler1.UpdateWithMessage(CStr(e.Data("message")))
				Else
					DayPilotScheduler1.UpdateWithMessage("Updated.")
				End If

			Case "clear"
				DayPilotScheduler1.ClientState.Clear()
				LoadResources()
				LoadSeparators()
				DayPilotScheduler1.DataSource = (New DataManager()).GetAssignments(DayPilotScheduler1)
				DayPilotScheduler1.DataBind()
				DayPilotScheduler1.UpdateWithMessage("Filter cleared.")
		End Select
	End Sub

	Protected Sub DayPilotScheduler1_TimeHeaderClick(ByVal sender As Object, ByVal e As TimeHeaderClickEventArgs)
		DayPilotScheduler1.DataSource = (New DataManager()).GetAssignments(DayPilotScheduler1)
		DayPilotScheduler1.DataBind()

		Dim time As New JsonData()
		time("name") = "Time"
		time("value") = String.Format("{0} - {1}", TimeFormatter.GetHourMinutes(e.Start, DayPilotScheduler1.TimeFormat), TimeFormatter.GetHourMinutes(e.End, DayPilotScheduler1.TimeFormat))
		time("start") = e.Start.ToString("s")
		time("end") = e.End.ToString("s")

		If DayPilotScheduler1.ClientState("filter") Is Nothing Then
			DayPilotScheduler1.ClientState("filter") = New JsonData()
		End If
		DayPilotScheduler1.ClientState("filter")("time") = time

		LoadResources()
		LoadSeparators()

		DayPilotScheduler1.Update()

	End Sub


	Private Class FilterData
		Friend Start As Date = Date.MinValue
		Friend [End] As Date = Date.MaxValue
		Friend IsTime As Boolean = False
		Friend Seats As Integer = 0

		Friend Sub New(ByVal filter As JsonData)
			If filter Is Nothing Then
				Return
			End If

			If filter("time") IsNot Nothing Then
				IsTime = True
				Start = CDate(filter("time")("start"))
				[End] = CDate(filter("time")("end"))
			End If

			If filter("seats") IsNot Nothing Then
				Seats = CInt(Fix(filter("seats")("count")))
			End If
		End Sub
	End Class

	Protected Sub DayPilotScheduler1_BeforeTimeHeaderRender(ByVal sender As Object, ByVal e As BeforeTimeHeaderRenderEventArgs)
		If e.Start.DayOfWeek = DayOfWeek.Saturday OrElse e.Start.DayOfWeek = DayOfWeek.Sunday Then
			If e.End.TimeOfDay.TotalHours <= DayPilotScheduler1.BusinessBeginsHour Then
				e.Visible = False
			ElseIf e.Start.TimeOfDay.TotalHours >= DayPilotScheduler1.BusinessEndsHour Then
				e.Visible = False
			Else
				e.Visible = True
			End If
		End If
	End Sub
End Class
