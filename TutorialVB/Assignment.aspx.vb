Imports System
Imports System.Collections
Imports System.Data
Imports Data
Imports DayPilot.Web.Ui.Recurrence
Imports Util

Partial Public Class Assignment
	Inherits System.Web.UI.Page

	Private _rule As RecurrenceRule = RecurrenceRule.NoRepeat
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		If Not IsPostBack Then

			Dim assignment As DataRow = (New DataManager()).GetAssignment(Convert.ToInt32(Request.QueryString("id")))

			TextBoxName.Text = Convert.ToString(assignment("TaskName"))
			DropDownDuration.SelectedValue = Convert.ToString(assignment("TaskDuration"))

			LabelStart.Text = String.Format("{0}", assignment("AssignmentStart"))
			LabelEnd.Text = String.Format("{0}", assignment("AssignmentEnd"))
			LabelLocation.Text = String.Format("{0}", assignment("LocationName"))

			DataBind()
		End If
	End Sub

	Protected Sub ButtonDelete_Click(ByVal sender As Object, ByVal e As EventArgs)
		CType(New DataManager(), DataManager).DeleteAssignment(Convert.ToInt32(Request.QueryString("id")))
		Dim ht As New Hashtable()
		ht("refresh") = True
		ht("message") = "Assignment deleted."
		Modal.Close(Me, ht)
	End Sub

	Protected Sub ButtonCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
		Modal.Close(Me)
	End Sub

End Class
