<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Assignment.aspx.cs" Inherits="Assignment" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="~/Styles/Blank.css" rel="stylesheet" type="text/css" />
    <title>Edit</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table border="0" cellspacing="4" cellpadding="0">
            <tr>
                <td></td>
                <td><b>Case</b></td>
            </tr>
            <tr>
                <td align="right" valign="top">Case Number:</td>
                <td><asp:TextBox ID="TextBoxName" runat="server" Enabled="false"></asp:TextBox></td>
            </tr>
            <tr>
                <td align="right">Duration:</td>
                <td>
                <asp:DropDownList ID="DropDownDuration" runat="server" Enabled="false">
                    <asp:ListItem Value="15">15 minutes</asp:ListItem>
                    <asp:ListItem Value="30">30 minutes</asp:ListItem>
                    <asp:ListItem Value="45">45 minutes</asp:ListItem>
                    <asp:ListItem Value="60">1 hour</asp:ListItem>
                    <asp:ListItem Value="120">2 hours</asp:ListItem>
                    <asp:ListItem Value="180">3 hours</asp:ListItem>
                    <asp:ListItem Value="240">4 hours</asp:ListItem>
                </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td></td>
                <td><b>Assignment</b></td>
            </tr>
            <tr>
                <td></td>
                <td><asp:LinkButton ID="ButtonDelete" runat="server" OnClick="ButtonDelete_Click" Text="Delete Assignment" /></td>
            </tr>
            <tr>
                <td align="right">Start:</td>
                <td><asp:Label runat="server" ID="LabelStart" /></td>
            </tr>
            <tr>
                <td align="right">End:</td>
                <td><asp:Label runat="server" ID="LabelEnd" /></td>
            </tr>
            <tr>
                <td align="right">Location:</td>
                <td><asp:Label runat="server" ID="LabelLocation" /></td>
            </tr>
            <tr>
                <td align="right"></td>
                <td>
                    <br />
                    <br />
                    <asp:Button ID="ButtonCancel" runat="server" Text="Close" OnClick="ButtonCancel_Click" />
                </td>
            </tr>
        </table>

        </div>
    </form>
</body>
</html>
