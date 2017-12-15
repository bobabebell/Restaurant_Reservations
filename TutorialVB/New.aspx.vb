Imports System
Imports System.Collections
Imports Data
Imports Util

Partial Public Class NewDialog
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsPostBack Then
            TextBoxStart.Text = Convert.ToDateTime(Request.QueryString("start")).ToString()
            TextBoxEnd.Text = Convert.ToDateTime(Request.QueryString("end")).ToString()

            TextBoxNote.Focus()

            DropDownListLocation.DataSource = (New DataManager()).GetLocationsChildren()

            If Not String.IsNullOrEmpty(Request.QueryString("table")) Then
                DropDownListLocation.SelectedValue = Request.QueryString("table")
            End If

            DataBind()
        End If
    End Sub

    Protected Sub ButtonOK_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim start As Date = Convert.ToDateTime(TextBoxStart.Text)
        Dim [end] As Date = Convert.ToDateTime(TextBoxEnd.Text)
        Dim note As String = TextBoxNote.Text
        Dim location As Integer = Convert.ToInt32(DropDownListLocation.SelectedValue)

        CType(New DataManager(), DataManager).CreateAssignment(start, [end], location, note)

        ' passed to the modal dialog close handler, see Scripts/DayPilot/event_handling.js
        Dim ht As New Hashtable()
        ht("refresh") = "yes"
        ht("message") = "Reservation created."

        Modal.Close(Me, ht)
    End Sub

    Protected Sub ButtonCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        Modal.Close(Me)
    End Sub
End Class
