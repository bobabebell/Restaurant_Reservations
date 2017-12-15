Imports System.Text
Imports System.Web.UI


Namespace Util
	''' <summary>
	''' Summary description for Modal
	''' </summary>
	Public Class Modal

		Public Shared Sub Close(ByVal page As Page)
			Close(page, Nothing)
		End Sub

		Public Shared Sub Close(ByVal page As Page, ByVal result As Object)
			page.Response.Clear()
			page.Response.ContentType = "text/html"
			page.Response.Buffer = True

			Dim sb As New StringBuilder()
			sb.Append("<html>")
			sb.Append("<head>")
			sb.Append("<script type='text/javascript'>")
			sb.Append("if (parent && parent.DayPilot && parent.DayPilot.ModalStatic) {")
			sb.Append("parent.DayPilot.ModalStatic.result(" & DayPilot.Web.Ui.Json.SimpleJsonSerializer.Serialize(result) & ");")
			sb.Append("if (parent.DayPilot.ModalStatic.hide) parent.DayPilot.ModalStatic.hide();")
			sb.Append("}")
			sb.Append("</script>")
			sb.Append("</head>")
			sb.Append("</html>")

			Dim output As String = sb.ToString()

			Dim s() As Byte = Encoding.UTF8.GetBytes(output)
			page.Response.AddHeader("Content-Length", s.Length.ToString())

			page.Response.Write(output)

			page.Response.Flush()
			page.Response.Close()

		End Sub

	End Class


End Namespace
