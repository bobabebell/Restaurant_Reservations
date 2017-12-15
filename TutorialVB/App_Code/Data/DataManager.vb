Imports System
Imports System.Data
Imports System.Data.Common
Imports DayPilot.Web.Ui

Namespace Data
	Public Class DataManager

		Private Function CreateDataAdapter(ByVal [select] As String) As DbDataAdapter
			Dim da As DbDataAdapter = Factory.CreateDataAdapter()
			da.SelectCommand = CreateCommand([select])
			Return da
		End Function

		Public Function GetLocations() As DataTable
			Dim da = CreateDataAdapter("select * from [Location] where [ParentId] is null order by [LocationName]")
			Dim dt As New DataTable()
			da.Fill(dt)
			Return dt
		End Function

		Public Function GetLocationsFiltered(ByVal parent As Integer, ByVal start As Date, ByVal [end] As Date, ByVal seats As Integer) As DataTable
			'WHERE NOT (([eventend <= @start) OR ([eventstart] >= @end))
			Dim da = CreateDataAdapter("select * from [Location] left outer join [Assignment] on [Assignment].[LocationId] = [Location].[LocationId] and NOT (([AssignmentEnd] <= @start) OR ([AssignmentStart] >= @end)) where [ParentId] = @parent and [LocationSeats] >= @seats and [AssignmentId] is null order by [LocationName]")
			AddParameterWithValue(da.SelectCommand, "parent", parent)
			AddParameterWithValue(da.SelectCommand, "seats", seats)
			AddParameterWithValue(da.SelectCommand, "start", start)
			AddParameterWithValue(da.SelectCommand, "end", [end])
			Dim dt As New DataTable()
			da.Fill(dt)
			Return dt
		End Function

		Public Function GetLocationsChildren() As DataTable
			Dim da = CreateDataAdapter("select * from [Location] where [ParentId] is not null order by [LocationName]")
			Dim dt As New DataTable()
			da.Fill(dt)
			Return dt
		End Function

		Public Function GetLocations(ByVal parent As Integer, ByVal seats As Integer) As DataTable
			Dim da = CreateDataAdapter("select * from [Location] where [ParentId] = @parent and [LocationSeats] >= @seats order by [LocationName]")
			AddParameterWithValue(da.SelectCommand, "parent", parent)
			AddParameterWithValue(da.SelectCommand, "seats", seats)
			Dim dt As New DataTable()
			da.Fill(dt)
			Return dt
		End Function

		Public Sub CreateTask(ByVal duration As Integer, ByVal name As String)
			Using con As DbConnection = CreateConnection()
				con.Open()

				Dim cmd = CreateCommand("insert into [Task] ([TaskDuration], [TaskName]) values (@duration, @name)", con)
				AddParameterWithValue(cmd, "duration", duration)
				AddParameterWithValue(cmd, "name", name)
				cmd.ExecuteNonQuery()
			End Using
		End Sub


		Public Sub CreateAssignment(ByVal start As Date, ByVal [end] As Date, ByVal location As Integer, ByVal note As String)
			Using con As DbConnection = CreateConnection()
				con.Open()

				Dim cmd = CreateCommand("insert into [Assignment] ([AssignmentStart], [AssignmentEnd], [LocationId], [AssignmentNote], [AssignmentPriority]) values (@start, @end, @location, @note, @priority)", con)
				AddParameterWithValue(cmd, "start", start)
				AddParameterWithValue(cmd, "end", [end])
				AddParameterWithValue(cmd, "location", location)
				AddParameterWithValue(cmd, "note", note)
				AddParameterWithValue(cmd, "priority", Date.Now)
				cmd.ExecuteNonQuery()

			End Using
		End Sub

		Public Sub DeleteAssignment(ByVal id As Integer)
			Using con = CreateConnection()
				con.Open()

				Dim cmd = CreateCommand("delete from [Assignment] where [AssignmentId] = @id", con)
				AddParameterWithValue(cmd, "id", id)
				cmd.ExecuteNonQuery()
			End Using
		End Sub

		Public Function GetAssignments(ByVal scheduler As DayPilotScheduler) As Object
			Dim dt As New DataTable()
			Dim da = CreateDataAdapter("select * from [Assignment] where NOT (([AssignmentEnd] <= @start) OR ([AssignmentStart] >= @end))")
			AddParameterWithValue(da.SelectCommand, "start", scheduler.StartDate)
			AddParameterWithValue(da.SelectCommand, "end", scheduler.EndDate.AddDays(1))
			da.Fill(dt)
			Return dt
		End Function

		Public Function GetAssignments(ByVal month As DayPilotMonth) As Object
			Dim dt As New DataTable()
			Dim da = CreateDataAdapter("select * from [Assignment] join [Task] on [Assignment].[TaskId] = [Task].[TaskId] where NOT (([AssignmentEnd] <= @start) OR ([AssignmentStart] >= @end))")
			AddParameterWithValue(da.SelectCommand, "start", month.VisibleStart)
			AddParameterWithValue(da.SelectCommand, "end", month.VisibleEnd)
			da.Fill(dt)
			Return dt
		End Function

		Public Sub MoveAssignment(ByVal id As Integer, ByVal start As Date, ByVal [end] As Date, ByVal location As Integer)
			Dim original As DataRow = GetAssignment(id)

			' if the reservatino table was changed, update the priority
			Dim tableChanged As Boolean = CLng(Fix(original("LocationId"))) <> location

			If tableChanged Then ' update the priority
				Using con = CreateConnection()
					con.Open()

					Dim cmd = CreateCommand("update [Assignment] set [AssignmentStart] = @start, [AssignmentEnd] = @end, [LocationId] = @location, [AssignmentPriority] = @priority where [AssignmentId] = @id", con)
					AddParameterWithValue(cmd, "id", id)
					AddParameterWithValue(cmd, "start", start)
					AddParameterWithValue(cmd, "end", [end])
					AddParameterWithValue(cmd, "location", location)
					AddParameterWithValue(cmd, "priority", Date.Now)

					cmd.ExecuteNonQuery()
				End Using
			Else
				Using con = CreateConnection()
					con.Open()

					Dim cmd = CreateCommand("update [Assignment] set [AssignmentStart] = @start, [AssignmentEnd] = @end, [LocationId] = @location where [AssignmentId] = @id", con)
					AddParameterWithValue(cmd, "id", id)
					AddParameterWithValue(cmd, "start", start)
					AddParameterWithValue(cmd, "end", [end])
					AddParameterWithValue(cmd, "location", location)

					cmd.ExecuteNonQuery()
				End Using
			End If


		End Sub

		Public Sub UpdateAssignment(ByVal id As Integer, ByVal start As Date, ByVal [end] As Date, ByVal location As Integer, ByVal note As String)
			Using con = CreateConnection()
				con.Open()

				Dim cmd = CreateCommand("update [Assignment] set [AssignmentStart] = @start, [AssignmentEnd] = @end, [LocationId] = @location, [AssignmentNote] = @note where [AssignmentId] = @id", con)
				AddParameterWithValue(cmd, "id", id)
				AddParameterWithValue(cmd, "start", start)
				AddParameterWithValue(cmd, "end", [end])
				AddParameterWithValue(cmd, "location", location)
				AddParameterWithValue(cmd, "note", note)
				cmd.ExecuteNonQuery()
			End Using

		End Sub

		Public Function GetAssignment(ByVal id As Integer) As DataRow
			Dim da = CreateDataAdapter("select * from [Assignment] where [Assignment].[AssignmentId] = @id")
			AddParameterWithValue(da.SelectCommand, "id", id)
			Dim dt As New DataTable()
			da.Fill(dt)
			If dt.Rows.Count = 1 Then
				Return dt.Rows(0)
			End If
			Return Nothing
		End Function

		Public Function GetTasks() As DataTable
			Dim da = CreateDataAdapter("select * from [Task] left join [Assignment] on [Task].[TaskId] = [Assignment].[TaskId] where [AssignmentId] is null")
			Dim dt As New DataTable()
			da.Fill(dt)
			Return dt

		End Function


		#Region "Helper methods"
		Private ReadOnly Property ConnectionString() As String
			Get
				Return Db.ConnectionString()
			End Get
		End Property

		Private ReadOnly Property Factory() As DbProviderFactory
			Get
				Return Db.Factory()
			End Get
		End Property

		Private Function CreateConnection() As DbConnection
			Dim connection As DbConnection = Factory.CreateConnection()
            connection.ConnectionString = ConnectionString
			Return connection
		End Function

		Private Function CreateCommand(ByVal text As String) As DbCommand
			Dim command As DbCommand = Factory.CreateCommand()
			command.CommandText = text
			command.Connection = CreateConnection()

			Return command
		End Function

		Private Function CreateCommand(ByVal text As String, ByVal connection As DbConnection) As DbCommand
			Dim command As DbCommand = Factory.CreateCommand()
			command.CommandText = text
			command.Connection = connection

			Return command
		End Function

		Private Sub AddParameterWithValue(ByVal cmd As DbCommand, ByVal name As String, ByVal value As Object)
			Dim parameter = Factory.CreateParameter()
			parameter.Direction = ParameterDirection.Input
			parameter.ParameterName = name
			parameter.Value = value
			cmd.Parameters.Add(parameter)
		End Sub

		Private Function GetIdentity(ByVal c As DbConnection) As Integer
			Dim cmd = CreateCommand(Db.IdentityCommand(), c)
			Return Convert.ToInt32(cmd.ExecuteScalar())
		End Function

		#End Region

		''' <summary>
		''' Find concurrent events with the same location.
		''' </summary>
		''' <param name="id"></param>
		''' <param name="start"></param>
		''' <param name="end"></param>
		''' <param name="location"></param>
		''' <returns></returns>
		Public Function GetExistingAssignments(ByVal id As Integer, ByVal start As Date, ByVal [end] As Date, ByVal location As Integer) As Integer
			Dim dt As New DataTable()
			Dim da = CreateDataAdapter("select * from [Assignment] where NOT (([AssignmentEnd] <= @start) OR ([AssignmentStart] >= @end)) and [LocationId] = @location and [AssignmentId] <> @id")
			AddParameterWithValue(da.SelectCommand, "id", id)
			AddParameterWithValue(da.SelectCommand, "start", start)
			AddParameterWithValue(da.SelectCommand, "end", [end])
			AddParameterWithValue(da.SelectCommand, "location", location)
			da.Fill(dt)
			Return dt.Rows.Count
		End Function

		Public Function GetTask(ByVal id As Integer) As DataRow
			Dim da = CreateDataAdapter("select * from [Task] where [TaskId] = @id")
			AddParameterWithValue(da.SelectCommand, "id", id)
			Dim dt As New DataTable()
			da.Fill(dt)
			If dt.Rows.Count = 1 Then
				Return dt.Rows(0)
			End If
			Return Nothing
		End Function

		Public Sub UpdateTask(ByVal id As Integer, ByVal name As String, ByVal duration As Integer)
			Using con = CreateConnection()
				con.Open()

				Dim cmd = CreateCommand("update [Task] set [TaskDuration] = @duration, [TaskName] = @name where [TaskId] = @id", con)
				AddParameterWithValue(cmd, "id", id)
				AddParameterWithValue(cmd, "duration", duration)
				AddParameterWithValue(cmd, "name", name)
				cmd.ExecuteNonQuery()
			End Using
		End Sub
	End Class
End Namespace