using System;
using System.Data;
using System.Data.Common;
using DayPilot.Web.Ui;

namespace Data
{
    public class DataManager
    {

        private DbDataAdapter CreateDataAdapter(string select)
        {
            DbDataAdapter da = Factory.CreateDataAdapter();
            da.SelectCommand = CreateCommand(select);
            return da;
        }
        
        public DataTable GetLocations()
        {
            var da = CreateDataAdapter("select * from [Location] where [ParentId] is null order by [LocationName]");
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetLocationsFiltered(int parent, DateTime start, DateTime end, int seats)
        {
            //WHERE NOT (([eventend <= @start) OR ([eventstart] >= @end))
            var da = CreateDataAdapter("select * from [Location] left outer join [Assignment] on [Assignment].[LocationId] = [Location].[LocationId] and NOT (([AssignmentEnd] <= @start) OR ([AssignmentStart] >= @end)) where [ParentId] = @parent and [LocationSeats] >= @seats and [AssignmentId] is null order by [LocationName]");
            AddParameterWithValue(da.SelectCommand, "parent", parent);
            AddParameterWithValue(da.SelectCommand, "seats", seats);
            AddParameterWithValue(da.SelectCommand, "start", start);
            AddParameterWithValue(da.SelectCommand, "end", end);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetLocationsChildren()
        {
            var da = CreateDataAdapter("select * from [Location] where [ParentId] is not null order by [LocationName]");
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public DataTable GetLocations(int parent, int seats)
        {
            var da = CreateDataAdapter("select * from [Location] where [ParentId] = @parent and [LocationSeats] >= @seats order by [LocationName]");
            AddParameterWithValue(da.SelectCommand, "parent", parent);
            AddParameterWithValue(da.SelectCommand, "seats", seats);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public void CreateTask(int duration, string name)
        {
            using (DbConnection con = CreateConnection())
            {
                con.Open();

                var cmd = CreateCommand("insert into [Task] ([TaskDuration], [TaskName]) values (@duration, @name)", con);
                AddParameterWithValue(cmd, "duration", duration);
                AddParameterWithValue(cmd, "name", name);
                cmd.ExecuteNonQuery();
            }
        }


        public void CreateAssignment(DateTime start, DateTime end, int location, string note)
        {
            using (DbConnection con = CreateConnection())
            {
                con.Open();

                var cmd = CreateCommand("insert into [Assignment] ([AssignmentStart], [AssignmentEnd], [LocationId], [AssignmentNote], [AssignmentPriority]) values (@start, @end, @location, @note, @priority)", con);
                AddParameterWithValue(cmd, "start", start);
                AddParameterWithValue(cmd, "end", end);
                AddParameterWithValue(cmd, "location", location);
                AddParameterWithValue(cmd, "note", note);
                AddParameterWithValue(cmd, "priority", DateTime.Now);
                cmd.ExecuteNonQuery();

            }
        }

        public void DeleteAssignment(int id)
        {
            using (var con = CreateConnection())
            {
                con.Open();

                var cmd = CreateCommand("delete from [Assignment] where [AssignmentId] = @id", con);
                AddParameterWithValue(cmd, "id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public object GetAssignments(DayPilotScheduler scheduler)
        {
            DataTable dt = new DataTable();
            var da = CreateDataAdapter("select * from [Assignment] where NOT (([AssignmentEnd] <= @start) OR ([AssignmentStart] >= @end))");
            AddParameterWithValue(da.SelectCommand, "start", scheduler.StartDate);
            AddParameterWithValue(da.SelectCommand, "end", scheduler.EndDate.AddDays(1));
            da.Fill(dt);
            return dt;
        }

        public object GetAssignments(DayPilotMonth month)
        {
            DataTable dt = new DataTable();
            var da = CreateDataAdapter("select * from [Assignment] join [Task] on [Assignment].[TaskId] = [Task].[TaskId] where NOT (([AssignmentEnd] <= @start) OR ([AssignmentStart] >= @end))");
            AddParameterWithValue(da.SelectCommand, "start", month.VisibleStart);
            AddParameterWithValue(da.SelectCommand, "end", month.VisibleEnd);
            da.Fill(dt);
            return dt;
        }

        public void MoveAssignment(int id, DateTime start, DateTime end, int location)
        {
            DataRow original = GetAssignment(id);

            // if the reservatino table was changed, update the priority
            bool tableChanged = (Int64)original["LocationId"] != location;
            
            if (tableChanged)  // update the priority 
            {
                using (var con = CreateConnection())
                {
                    con.Open();

                    var cmd = CreateCommand("update [Assignment] set [AssignmentStart] = @start, [AssignmentEnd] = @end, [LocationId] = @location, [AssignmentPriority] = @priority where [AssignmentId] = @id", con);
                    AddParameterWithValue(cmd, "id", id);
                    AddParameterWithValue(cmd, "start", start);
                    AddParameterWithValue(cmd, "end", end);
                    AddParameterWithValue(cmd, "location", location);
                    AddParameterWithValue(cmd, "priority", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (var con = CreateConnection())
                {
                    con.Open();

                    var cmd = CreateCommand("update [Assignment] set [AssignmentStart] = @start, [AssignmentEnd] = @end, [LocationId] = @location where [AssignmentId] = @id", con);
                    AddParameterWithValue(cmd, "id", id);
                    AddParameterWithValue(cmd, "start", start);
                    AddParameterWithValue(cmd, "end", end);
                    AddParameterWithValue(cmd, "location", location);

                    cmd.ExecuteNonQuery();
                }
            }

           
        }

        public void UpdateAssignment(int id, DateTime start, DateTime end, int location, string note)
        {
            using (var con = CreateConnection())
            {
                con.Open();

                var cmd = CreateCommand("update [Assignment] set [AssignmentStart] = @start, [AssignmentEnd] = @end, [LocationId] = @location, [AssignmentNote] = @note where [AssignmentId] = @id", con);
                AddParameterWithValue(cmd, "id", id);
                AddParameterWithValue(cmd, "start", start);
                AddParameterWithValue(cmd, "end", end);
                AddParameterWithValue(cmd, "location", location);
                AddParameterWithValue(cmd, "note", note);
                cmd.ExecuteNonQuery();
            }

        }

        public DataRow GetAssignment(int id)
        {
            var da = CreateDataAdapter("select * from [Assignment] where [Assignment].[AssignmentId] = @id");
            AddParameterWithValue(da.SelectCommand, "id", id);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0];
            }
            return null;
        }

        public DataTable GetTasks()
        {
            var da = CreateDataAdapter("select * from [Task] left join [Assignment] on [Task].[TaskId] = [Assignment].[TaskId] where [AssignmentId] is null");
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;

        }


        #region Helper methods
        private string ConnectionString
        {
            get { return Db.ConnectionString(); }
        }

        private DbProviderFactory Factory
        {
            get { return Db.Factory(); }
        }

        private DbConnection CreateConnection()
        {
            DbConnection connection = Factory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        private DbCommand CreateCommand(string text)
        {
            DbCommand command = Factory.CreateCommand();
            command.CommandText = text;
            command.Connection = CreateConnection();

            return command;
        }

        private DbCommand CreateCommand(string text, DbConnection connection)
        {
            DbCommand command = Factory.CreateCommand();
            command.CommandText = text;
            command.Connection = connection;

            return command;
        }

        private void AddParameterWithValue(DbCommand cmd, string name, object value)
        {
            var parameter = Factory.CreateParameter();
            parameter.Direction = ParameterDirection.Input;
            parameter.ParameterName = name;
            parameter.Value = value;
            cmd.Parameters.Add(parameter);
        }

        private int GetIdentity(DbConnection c)
        {
            var cmd = CreateCommand(Db.IdentityCommand(), c);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        #endregion

        /// <summary>
        /// Find concurrent events with the same location.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public int GetExistingAssignments(int id, DateTime start, DateTime end, int location)
        {
            DataTable dt = new DataTable();
            var da = CreateDataAdapter("select * from [Assignment] where NOT (([AssignmentEnd] <= @start) OR ([AssignmentStart] >= @end)) and [LocationId] = @location and [AssignmentId] <> @id");
            AddParameterWithValue(da.SelectCommand, "id", id);
            AddParameterWithValue(da.SelectCommand, "start", start);
            AddParameterWithValue(da.SelectCommand, "end", end);
            AddParameterWithValue(da.SelectCommand, "location", location);
            da.Fill(dt);
            return dt.Rows.Count;
        }

        public DataRow GetTask(int id)
        {
            var da = CreateDataAdapter("select * from [Task] where [TaskId] = @id");
            AddParameterWithValue(da.SelectCommand, "id", id);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0];
            }
            return null;
        }

        public void UpdateTask(int id, string name, int duration)
        {
            using (var con = CreateConnection())
            {
                con.Open();

                var cmd = CreateCommand("update [Task] set [TaskDuration] = @duration, [TaskName] = @name where [TaskId] = @id", con);
                AddParameterWithValue(cmd, "id", id);
                AddParameterWithValue(cmd, "duration", duration);
                AddParameterWithValue(cmd, "name", name);
                cmd.ExecuteNonQuery();
            }
        }
    }
}