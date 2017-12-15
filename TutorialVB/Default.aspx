<%@ Page Title="Restaurant Table Reservation Tutorial Demo" Language="vb" MasterPageFile="~/Site.master" AutoEventWireup="true"
	CodeFile="Default.aspx.vb" Inherits="_Default" %>
<%@ Register Assembly="DayPilot" Namespace="DayPilot.Web.Ui" TagPrefix="DayPilot" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<div id="filter"></div>
<div class="left">
	<div id="places">
	Number of people:
	<div><a href="javascript:seats(1);">1 person</a></div>
	<div><a href="javascript:seats(2);">2 people</a></div>
	<div><a href="javascript:seats(3);">3 people</a></div>
	<div><a href="javascript:seats(4);">4 people</a></div>
	<div><a href="javascript:seats(5);">5 people</a></div>
	<div><a href="javascript:seats(6);">6 people</a></div>
	<div style="margin-top: 10px;"><b>Tip:</b> Click on the number of people and/or the time header ("1 PM") to find free tables.</div>
	</div>
</div>

<div class="right">
	<DayPilot:DayPilotScheduler
		ID="DayPilotScheduler1"
		runat="server"
		DataEndField="AssignmentEnd"
		DataStartField="AssignmentStart"
		DataTextField="AssignmentNote"
		DataValueField="AssignmentId"
		DataResourceField="LocationId"
		EventSortExpression="AssignmentPriority"
		Days="7"
		EventMoveHandling="Notify"
		EventMoveJavaScript="dp.eventMoveCallBack(e, newStart, newEnd, newResource, {external: external} );"
		OnEventMove="DayPilotScheduler1_EventMove"
		EventClickHandling="JavaScript"
		EventClickJavaScript="edit(e);"
		ClientObjectName="dp"
		ShowEventStartEnd="false"
		EventHeight="25"
		Width="100%"
		TreeEnabled="true"
		RowHeaderWidth="200"
		ShowNonBusiness="false"
		BusinessBeginsHour="11"
		BusinessEndsHour="24"
		CellDuration="15"
		CellWidth="25"
		OnBeforeCellRender="DayPilotScheduler1_BeforeCellRender"
		OnBeforeEventRender="DayPilotScheduler1_BeforeEventRender"
		OnCommand="DayPilotScheduler1_Command"
		AfterRenderJavaScript="afterRender()"
		Theme="scheduler_8"
		TreePreventParentUsage="true"
		HeaderHeight="23"
		TimeRangeSelectedHandling="JavaScript"
		TimeRangeSelectedJavaScript="create(start, end, resource)"
		TimeHeaderClickHandling="CallBack"
		OnTimeHeaderClick="DayPilotScheduler1_TimeHeaderClick"
		HeightSpec="Max"
		Height="400"
		RowHeaderColumnWidths="20,20"
		TreeImageMarginTop="5"
		RowHeaderWidthAutoFit="true"
		OnBeforeTimeHeaderRender="DayPilotScheduler1_BeforeTimeHeaderRender"
	/>
</div>

</asp:Content>
