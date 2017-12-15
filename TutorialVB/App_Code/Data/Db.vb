Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data.Common
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Web
Imports System.Web.UI

Namespace Data

	Public Class Db

		Public Shared Function ConnectionString() As String
			Dim mssql As Boolean = Not SqLiteFound()
			If mssql Then
				Return ConfigurationManager.ConnectionStrings("daypilot").ConnectionString
			End If

			If TryCast(HttpContext.Current.Session("cs"), String) Is Nothing Then
				HttpContext.Current.Session("cs") = GetNew()
			End If

			Return CStr(HttpContext.Current.Session("cs"))

		End Function

		Public Shared Function Factory() As DbProviderFactory
			Return DbProviderFactories.GetFactory(FactoryName())
		End Function

		Public Shared Function FactoryName() As String
			If SqLiteFound() Then
				Return "System.Data.SQLite"
			End If
			Return "System.Data.SqlClient"
		End Function

		Public Shared Function IdentityCommand() As String
			Select Case FactoryName()
				Case "System.Data.SQLite"
					Return "select last_insert_rowid();"
				Case "System.Data.SqlClient"
					Return "select @@identity;"
				Case Else
					Throw New NotSupportedException("Unsupported DB factory.")
			End Select
		End Function


		Private Shared Function GetNew() As String
			Dim today As String = Date.Today.ToString("yyyy-MM-dd")
			Dim guid As String = System.Guid.NewGuid().ToString()
			Dim dir As String = HttpContext.Current.Server.MapPath("~/App_Data/session/" & today & "/")
			Dim master As String = HttpContext.Current.Server.MapPath("~/App_Data/daypilot.sqlite")
			Dim path As String = dir & guid

			Directory.CreateDirectory(dir)
			File.Copy(master, path)

			Return String.Format("Data Source={0}", path)
		End Function

		Private Shared Function SqLiteFound() As Boolean
			Dim path As String = HttpContext.Current.Server.MapPath("~/bin/System.Data.SQLite.dll")
			Return File.Exists(path)
		End Function

	End Class
End Namespace