Imports System
Imports System.Xml
Imports Arco.ApplicationServer.Library.Linq
Imports Arco.Doma.Library.Helpers

Partial Class Preview
    Inherits BaseMasterPage

#Region " Public properties"


    Private thisObject As Arco.Doma.Library.baseObjects.DM_OBJECT
    Private thisCaseInstance As Arco.Doma.Library.Routing.cCase

    Private _Mode As Arco.Doma.Library.Website.Screen.DetailScreenDisplayMode

    Protected HeaderText As String = ""
    Private CurrentFileName As String = ""
    Private msFileOrderBy As String = ""
    Private mbForceInitialRedirect As Boolean
    Private msTabNode As String = "General"
#End Region

#Region " Event handlers"


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            ParseQueryString()
            If Not thisObject Is Nothing Then
                If thisObject.Active Then
                    HeaderText = thisObject.Name.Replace(vbCrLf, "").Replace(Chr(34), "'")
                Else
                    HeaderText = thisObject.Name.Replace(vbCrLf, "").Replace(Chr(34), "'") & " (Archived)"
                End If
                If TypeOf thisObject Is Arco.Doma.Library.Routing.HistoryCase Then
                    CreateTabs("Case", DirectCast(thisObject, Arco.Doma.Library.Routing.HistoryCase).Proc_ID) 'rthistorycase
                ElseIf thisObject.ID > 0 Then
                    CreateTabs(thisObject.Object_Type, thisObject.Category)
                Else
                    If Not thisCaseInstance Is Nothing Then
                        CreateTabs("Case", thisCaseInstance.Proc_ID) 'rtcase not in dm_object                      
                    End If
                End If
            End If
            SetUpScroller()
        End If

        PC.ReloadFunction = Me.ReloadFunction

    End Sub
    Private Function fsMakeJSUrl(ByVal vsRelativeUrl As String) As String
        Return Arco.Doma.WebControls.EncodingUtils.EncodeJsString(Request.ApplicationPath & "/" & Page.ResolveClientUrl(vsRelativeUrl))
    End Function
    Private Sub SetUpScroller()

        lblClose.ToolTip = GetLabel("close")
        lblClose.Text = "<span class='icon icon-close icon-color-light'></span>"
        'lblClose.ImageUrl = ThemedImage.GetUrl("cancel.png", Me)

        'setup document scroller
        Dim loScroller As QueryStringParser.ObjectScroller = QueryStringParser.GetObjectScroller(True)

        Dim sb As New StringBuilder
        sb.AppendLine("function GotoCurrent(){")
        sb.AppendLine("   self.location.href = " & fsMakeJSUrl(loScroller.CurrentLink.NavigateUrl) & ";")
        sb.AppendLine("}")
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "gotocurrent", sb.ToString, True)

        If loScroller.hasItems Then
            btnPrevious.Visible = True
            btnNext.Visible = True

            btnNext.Text = ThemedImage.GetSpanIconTag("icon icon-arrow-right icon-color-light icon-clickable")
            'btnNext.ImageUrl = ThemedImage.GetUrl("nextpage.png", Me)
            btnPrevious.Text = ThemedImage.GetSpanIconTag("icon icon-arrow-left icon-color-light icon-clickable")
            'btnPrevious.ImageUrl = ThemedImage.GetUrl("previouspage.png", Me)

            btnPrevious.Enabled = loScroller.PreviousLink.Enabled
            btnPrevious.NavigateUrl = loScroller.PreviousLink.NavigateUrl
            If btnPrevious.Enabled Then
                btnPrevious.ToolTip = Server.HtmlDecode(GetLabel("previous")) & " (" & (loScroller.CurrentItem - 1) & "/" & loScroller.ItemCount & ")"
            Else
                btnPrevious.ToolTip = ""
            End If
            btnNext.Enabled = loScroller.NextLink.Enabled
            btnNext.NavigateUrl = loScroller.NextLink.NavigateUrl
            If btnNext.Enabled Then
                btnNext.ToolTip = Server.HtmlDecode(GetLabel("next")) & " (" & (loScroller.CurrentItem + 1) & "/" & loScroller.ItemCount & ")"
            Else
                btnNext.ToolTip = ""
            End If
        Else
            btnPrevious.Visible = False
            btnNext.Visible = False
        End If

    End Sub

#End Region

#Region " Private methods"


    Private Sub ParseQueryString()

        Dim loLoaded As Object = QueryStringParser.CurrentDMObject
        If Not loLoaded Is Nothing Then
            If TypeOf loLoaded Is Arco.Doma.Library.baseObjects.DM_OBJECT Then
                thisObject = CType(loLoaded, Arco.Doma.Library.baseObjects.DM_OBJECT)
                If Not TypeOf thisObject Is Arco.Doma.Library.Routing.HistoryCase Then
                    If thisObject.ID = 0 Then
                        'instead of an error
                        Server.Transfer("DM_ACL_DENIED.aspx?code=" & Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INVALIDOBJECT)
                    End If
                End If
            Else
                thisCaseInstance = CType(loLoaded, Arco.Doma.Library.Routing.cCase)
                thisObject = thisCaseInstance.TargetObject
            End If
        End If

        If QueryStringParser.Exists("tabnode") Then
            msTabNode = QueryStringParser("tabnode")
            Select Case msTabNode.ToLower
                Case "sub"
                    msTabNode = "Sub"
                Case Else
                    msTabNode = "General"
            End Select
        End If
    End Sub

    Public Overrides Property ParentID() As Int32
        Get
            Return PC.ParentID
        End Get
        Set(ByVal value As Int32)
            PC.ParentID = value
        End Set
    End Property

    Private Function GetTabNode(ByVal objXML As XmlDocument, ByVal ObjectType As String, ByVal vlCatOrProcID As Int32) As XmlNode
        Dim rootNode As XmlNode = objXML.GetElementsByTagName("PreviewPanes").Item(0)
        Dim n As XmlNode = rootNode.SelectSingleNode(ObjectType.ToLower & msTabNode & "_" & vlCatOrProcID.ToString)
        If n Is Nothing Then
            n = rootNode.SelectSingleNode(ObjectType.ToLower & msTabNode)
        End If
        Return n
    End Function
    Private Sub CreateTabs(ByVal ObjectType As String, ByVal vlCatOrProcID As Int32)

        Dim objXML As New XmlDocument
        Dim lbGoDefault As Boolean = QueryStringParser.GetBoolean("godefault")

        objXML.Load(SiteManagement.SitesManager.GetFullFilePath("PreviewTabs.xml"))

        If Not objXML Is Nothing Then
            Dim lbHasFiles As Boolean = False
            If thisObject.FileCount > 0 Then
                For Each f As Arco.Doma.Library.FileList.FileInfo In thisObject.Files
                    If f.PackageID = 0 OrElse thisObject.CanViewPackage(f.PackageID) Then
                        lbHasFiles = True
                        Exit For
                    End If
                Next
            End If

            Dim objTabs As XmlNode = GetTabNode(objXML, ObjectType, vlCatOrProcID) 'objXML.GetElementsByTagName("PreviewPanes").Item(0).SelectSingleNode(ObjectType.ToLower & msTabNode)
            If Not objTabs Is Nothing Then
                For Each objChild As XmlNode In objTabs.SelectNodes("tab")
                    Dim visibleAttr As XmlNode = objChild.Attributes.GetNamedItem("visible")
                    If visibleAttr IsNot Nothing AndAlso Not StringConversions.ToBoolean(visibleAttr.Value) Then
                        Continue For
                    End If

                    Dim urlNode As XmlNode = objChild.SelectSingleNode("url")
                    Dim urlText As String = urlNode.InnerText
                    Dim urlTextToLower As String = urlText.ToLower
                    If urlTextToLower.Contains("dm_preview.aspx") AndAlso (Not lbHasFiles OrElse Not thisObject.CanViewFiles) Then
                        Continue For
                    End If

                    If urlTextToLower.Contains("comments.aspx") AndAlso Not thisObject.CanViewComments Then
                        Continue For
                    End If
                    If urlTextToLower.Contains("messages.aspx") AndAlso Not thisObject.CanViewMessages Then
                        Continue For
                    End If
                    If urlTextToLower.Contains("dm_list_preview.aspx?screenmode=prevversions") AndAlso Not thisObject.CanViewPreviousVersions Then
                        Continue For
                    End If
                    If urlTextToLower.Contains("dm_list_preview.aspx?screenmode=links") AndAlso Not thisObject.Active Then
                        Continue For
                    End If

                    If urlTextToLower.Contains("history.aspx") Then
                        If thisCaseInstance IsNot Nothing Then
                            If Not thisCaseInstance.CanViewHistory Then
                                Continue For
                            End If
                        Else
                            If Not thisObject.CanViewHistory Then
                                Continue For
                            End If
                        End If
                    End If

                    Dim radTab As New Telerik.Web.UI.RadTab
                    Dim captionNode As XmlNode = objChild.SelectSingleNode("caption" & Arco.Security.BusinessIdentity.CurrentIdentity.Language)
                    If captionNode IsNot Nothing Then
                        radTab.Text = captionNode.InnerText
                    Else
                        radTab.Text = objChild.SelectSingleNode("captionE").InnerText
                    End If

                    radTab.Value = urlTextToLower
                    QueryStringParser.Remove("godefault")


                    If urlTextToLower.Contains("dm_preview.aspx") Then
                        If thisObject.FileCount > 0 Then
                            For Each f As Arco.Doma.Library.FileList.FileInfo In thisObject.Files.AsQueryable.OrderBy("FILE_ATT ASC,FILE_TITLE ASC")
                                If f.PackageID = 0 OrElse thisObject.CanViewPackage(f.PackageID) Then
                                    QueryStringParser.Add("FILE_ID", f.ID.ToString)
                                    QueryStringParser.Add("rend", "preview")
                                    Exit For
                                End If
                            Next
                        End If
                    ElseIf urlTextToLower.Contains("dm_list_preview.aspx") Then
                        QueryStringParser.Add("DM_PARENT_ID", thisObject.ID)
                        QueryStringParser.Remove("screenmode") 'this is in the url itself, hack
                    ElseIf urlTextToLower.Contains("dm_detailview.aspx") Then
                        QueryStringParser.Add("preview", True)
                        QueryStringParser.Remove("SCREEN_ID") 'hack for Wim
                    End If

                    If urlText.Contains("?") Then
                        urlText &= QueryStringParser.ToString(True)
                    Else
                        urlText &= QueryStringParser.ToString
                    End If
                    Dim imgNode As XmlNode = objChild.SelectSingleNode("image")
                    If imgNode IsNot Nothing Then
                        radTab.ImageUrl = imgNode.InnerText
                    End If
                    radTab.NavigateUrl = urlText
                    TabStrip.Tabs.Add(radTab)

                    Dim strUrl As String = Me.Request.Url.Segments(Me.Request.Url.Segments.Length - 1)
                    strUrl &= Request.Url.Query
                    strUrl = strUrl.ToLower

                    Dim strTab As String = radTab.Value.ToLower
                    If strTab.IndexOf("/") > 0 Then
                        strTab = strTab.Substring(strTab.LastIndexOf("/") + 1)
                    End If

                    If lbGoDefault Then
                        Dim lbRedirect As Boolean = False
                        If Not String.IsNullOrEmpty(CurrentTab) Then
                            If CurrentTab = radTab.Value Then
                                lbRedirect = True
                            End If
                        Else
                            If Not objChild.Attributes.GetNamedItem("isDefault") Is Nothing Then
                                If CType(objChild.Attributes.GetNamedItem("isDefault").Value, Boolean) Then
                                    lbRedirect = True
                                End If
                            End If
                        End If
                        If lbRedirect Then
                            If Not strUrl.Contains(strTab) Then
                                Response.Redirect(radTab.NavigateUrl)
                            Else
                                CurrentTab = radTab.Value
                                radTab.Selected = True
                                HeaderText = HeaderText & " - " & radTab.Text
                            End If
                        End If
                    Else
                        If strUrl.Contains(strTab) Then
                            CurrentTab = radTab.Value
                            radTab.Selected = True
                            HeaderText = HeaderText & " - " & radTab.Text
                        End If
                    End If


                Next
            End If
        End If

    End Sub

    Private Property CurrentTab As String
        Get
            Return Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity().GetExtendedProperty("PreviewTab")
        End Get
        Set(value As String)
            Dim i As Arco.Doma.Library.Security.BusinessIdentity = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity()
            If Not value.Equals(i.GetExtendedProperty("PreviewTab")) Then
                i.SetExtendedProperty("PreviewTab", value)
                i.Save()
            End If
        End Set
    End Property

#End Region

End Class
