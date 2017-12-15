<%@ Page Language="vb" AutoEventWireup="true" CodeFile="AjaxTasks.aspx.vb" Inherits="AjaxTasks" %>
<asp:Repeater runat="server" id="Repeater1">
	<ItemTemplate>
		<div class="task" onclick="editTask('<%#DataBinder.Eval(Container.DataItem, "TaskId")%>')" onmousedown="return DayPilotScheduler.dragStart(this, 60*<%#DataBinder.Eval(Container.DataItem, "TaskDuration")%>, '<%#DataBinder.Eval(Container.DataItem, "TaskId")%>', this.innerHTML);">
		<span style="cursor: move">:::</span> <%#DataBinder.Eval(Container.DataItem, "TaskName")%> <span><%#DataBinder.Eval(Container.DataItem, "TaskDuration")%>m</span>
		</div>
	</ItemTemplate>
</asp:Repeater>
<asp:Panel runat="server" ID="LabelEmpty" Visible="false">
<div class="gray padded">No unscheduled cases</div>
</asp:Panel>