using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;

namespace Data
{

    public class Db
    {

        public static string ConnectionString()
        {
            bool mssql = !SqLiteFound();
            if (mssql)
            {
                return ConfigurationManager.ConnectionStrings["daypilot"].ConnectionString;
            }

            if (HttpContext.Current.Session["cs"] as string == null)
            {
                HttpContext.Current.Session["cs"] = GetNew();
            }

            return (string)HttpContext.Current.Session["cs"];

        }

        public static DbProviderFactory Factory()
        {
            return DbProviderFactories.GetFactory(FactoryName());
        }

        public static string FactoryName()
        {
            if (SqLiteFound())
            {
                return "System.Data.SQLite";
            }
            return "System.Data.SqlClient";
        }

        public static string IdentityCommand()
        {
            switch (FactoryName())
            {
                case "System.Data.SQLite":
                    return "select last_insert_rowid();";
                case "System.Data.SqlClient":
                    return "select @@identity;";
                default:
                    throw new NotSupportedException("Unsupported DB factory.");
            }
        }


        private static string GetNew()
        {
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            string guid = Guid.NewGuid().ToString();
            string dir = HttpContext.Current.Server.MapPath("~/App_Data/session/" + today + "/");
            string master = HttpContext.Current.Server.MapPath("~/App_Data/daypilot.sqlite");
            string path = dir + guid;

            Directory.CreateDirectory(dir);
            File.Copy(master, path);

            return String.Format("Data Source={0}", path);
        }

        private static bool SqLiteFound()
        {
            string path = HttpContext.Current.Server.MapPath("~/bin/System.Data.SQLite.dll");
            return File.Exists(path);
        }

    }
}