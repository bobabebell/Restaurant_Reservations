using System.Text;
using System.Web.UI;


namespace Util
{
    /// <summary>
    /// Summary description for Modal
    /// </summary>
    public class Modal
    {

        public static void Close(Page page)
        {
            Close(page, null);
        }

        public static void Close(Page page, object result)
        {
            page.Response.Clear();
            page.Response.ContentType = "text/html";
            page.Response.Buffer = true;

            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.Append("<head>");
            sb.Append("<script type='text/javascript'>");
            sb.Append("if (parent && parent.DayPilot && parent.DayPilot.ModalStatic) {");
            sb.Append("parent.DayPilot.ModalStatic.result(" +
                      DayPilot.Web.Ui.Json.SimpleJsonSerializer.Serialize(result) + ");");
            sb.Append("if (parent.DayPilot.ModalStatic.hide) parent.DayPilot.ModalStatic.hide();");
            sb.Append("}");
            sb.Append("</script>");
            sb.Append("</head>");
            sb.Append("</html>");

            string output = sb.ToString();

            byte[] s = Encoding.UTF8.GetBytes(output);
            page.Response.AddHeader("Content-Length", s.Length.ToString());

            page.Response.Write(output);

            page.Response.Flush();
            page.Response.Close();

        }

    }


}
