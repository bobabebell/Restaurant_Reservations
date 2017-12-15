Imports System
Imports System.Collections
Imports System.Data
Imports Data
Imports DayPilot.Web.Ui.Recurrence
Imports Util

Partial Public Class Edit
	Inherits System.Web.UI.Page

	Private _rule As RecurrenceRule = RecurrenceRule.NoRepeat
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Not IsPostBack Then

			Dim dr As DataRow = (New DataManager()).GetAssignment(Convert.ToInt32(Request.QueryString("id")))

			TextBoxStart.Text = CDate(dr("AssignmentStart")).ToString()
			TextBoxEnd.Text = CDate(dr("AssignmentEnd")).ToString()
			TextBoxUpdated.Text = CDate(dr("AssignmentPriority")).ToString()
			TextBoxNote.Text = Convert.ToString(dr("AssignmentNote"))
			DropDownListLocation.SelectedValue = Convert.ToString(dr("LocationId"))

			DropDownListLocation.DataSource = (New DataManager()).GetLocationsChildren()

			TextBoxNote.Focus()

			DataBind()
		End If
	End Sub

	Protected Sub ButtonOK_Click(ByVal sender As Object, ByVal e As EventArgs)
		Dim start As Date = Convert.ToDateTime(TextBoxStart.Text)
		Dim [end] As Date = Convert.ToDateTime(TextBoxEnd.Text)
		Dim note As String = TextBoxNote.Text
		Dim location As Integer = Convert.ToInt32(DropDownListLocation.SelectedValue)

		CType(New DataManager(), DataManager).UpdateAssignment(Convert.ToInt32(Request.QueryString("id")), start, [end], location, note)

		Dim ht As New Hashtable()
		ht("refresh") = "yes"
		ht("message") = "Reservation updated."

		Modal.Close(Me, ht)
	End Sub

	Protected Sub ButtonCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
		Modal.Close(Me)
	End Sub

	Protected Sub ButtonDelete_Click(ByVal sender As Object, ByVal e As EventArgs)
		CType(New DataManager(), DataManager).DeleteAssignment(Convert.ToInt32(Request.QueryString("id")))
		Dim ht As New Hashtable()
		ht("refresh") = "yes"
		ht("message") = "Reservation deleted."
		Modal.Close(Me, ht)
	End Sub
End Class
