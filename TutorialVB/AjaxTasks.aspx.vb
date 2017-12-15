Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Data

Partial Public Class AjaxTasks
	Inherits System.Web.UI.Page

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		Dim data = (New DataManager()).GetTasks()
		Repeater1.DataSource = data
		Repeater1.DataBind()

		If data.Rows.Count = 0 Then
			LabelEmpty.Visible = True
		End If

	End Sub
End Class