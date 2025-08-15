<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FilterInfoPage.aspx.cs" Inherits="Help_CMS_FilterInfoPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="content-main">
            <div class="title-main">Condition filter syntax</div>
            <div>
                <div class="title-sub">Left and right hand side value possibilities:</div>
                <ul>
                    <li>A value of any type</li>
                    <li>A Doma property - ([@propertyname@]</li>
                    <li>A property of the parent object [@@property name from a parent object@@])</li>
                    <li>A Doma tag:
                        <ul>
                            <asp:Repeater runat="server" ID="listRepeater">
                                <ItemTemplate>
                                    <li><%# Container.DataItem.ToString() %></li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </li>
                </ul>
                <br />
                <div class="title-sub">Allowed operators, with examples:</div>
                <ul>
                    <li>!like, !contains: true if the left hand value does not contain the right hand value (or field). Ex. [@field1@] !like "myvalue"</li>
                    <li>contains, like: true if the left hand field contains the right hand value (or field)? Ex. [@field1@] contains "myvalue"</li>
                    <li>!=, <>: true if both values/fields are different from each other. Ex. [@field1@] != [@field2@]</li>
                    <li>=: true if both values/fields are the same. Ex. [@field1@] = ABC</li>
                    <li>>: true if the left hand value is larger than the right hand value. Ex. [@field1@] > 5</li>
                    <li>>=: true if the left hand value is larger than or equal to the right hand value. Ex. [@field1@] >= 5</li>
                    <li><: true if the left hand value is smaller than the right hand value. Ex. [@field1@] > 5</li>
                    <li><=: true if the left hand value is smaller than or equal to the right hand value. Ex. [@field1@] >= 5</li>
                    <li>Regex: true if the left hand value matches the regular expression. Ex. “#FILE_NAME#” regex “^[0-9]{8}_[0-9]{6}”</li>
                </ul>
                <br />
                <div class="InfoLabel">
                    <div>Quotes are allowed but not mandatory</div>
                    <div>Brackets can be used in conjunction with operators: And, Or</div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
