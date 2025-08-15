Imports Arco.Doma.CMS
Imports Arco.Doma.CMS.Data
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects

Partial Class Page
    Inherits BasePage


    Public Sub New()
        AllowGuestAccess = True
    End Sub

    Protected _page As Arco.Doma.CMS.Data.Page
    Private _pageMode As PageMode
    Private _panelCount As Integer
    Private _savePage As Boolean

    Public Overrides Property ParentID() As Int32  ' ParentID = doma folder that page is linked to
        Get
            Return thisPageController.ParentID
        End Get
        Set(ByVal value As Int32)
            thisPageController.ParentID = value
        End Set
    End Property

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _pageMode = CType(QueryStringParser.GetInt("mode"), PageMode)
        Dim pageId As Integer = QueryStringParser.GetInt("pageid")
        Dim parentIdIsKnown As Boolean = True
        If ParentID = 0 Then
            If QueryStringParser.Exists("DM_PARENT_ID") Then
                ParentID = QueryStringParser.GetInt("DM_PARENT_ID")
            Else
                parentIdIsKnown = False
            End If
        End If
        If pageId = 0 Then 'no pageID in querystring

            If parentIdIsKnown Then
                ' get the pageId from the object
                Dim lang As String = QueryStringParser.GetString("lang")
                Dim op As ObjectPage = ObjectPage.GetPage(ParentID, lang)
                If op.PageId = 0 Then 'no page is linked to the object
                    Dim o As DM_OBJECT = ObjectRepository.GetObject(ParentID)

                    'new page, check rights to create??
                    Dim p As Arco.Doma.CMS.Data.Page = Arco.Doma.CMS.Data.Page.NewPage()
                    p.LayoutID = PageLayoutList.GetLayoutList()(0).ID
                    p.Name = "Page for " & o.Name
                    p = p.Save

                    'link the page to the object
                    If o IsNot Nothing Then
                        op = ObjectPage.NewPage()
                        op.PageId = p.ID
                        op.ObjectId = ParentID
                        op.Save()
                    End If
                    pageId = p.ID

                Else
                    pageId = op.PageId
                End If
            Else
                'new page without object
                Dim p As Arco.Doma.CMS.Data.Page = Arco.Doma.CMS.Data.Page.NewPage()
                p.LayoutID = PageLayoutList.GetLayoutList()(0).ID
                p.Name = "My page"
                p = p.Save

                pageId = p.ID
            End If
            QueryStringParser.Remove("pageid")
            Response.Redirect("Page.aspx?pageid=" & pageId & QueryStringParser.ToString(True))
        Else
            ' When a tile/button-action must load a docroomlistpanel into a 'normal' panel, this replacement must be done server side
            ' The PanelController (CMS.js) sets following querystringparameters to do that:
            Dim action As String = QueryStringParser.GetString("Action")
            Dim actionID As String = QueryStringParser.GetString("ActionID")
            Dim screenID As String = QueryStringParser.GetString("DM_RESULT_SCREEN")
            Dim targetPanelID As Integer = QueryStringParser.GetInt("TargetPanelID")
            ' for menu behaviour with active buttons/tiles:
            hdnActiveButton.Value = QueryStringParser.GetString("activeButton")
            hdnActiveTile.Value = QueryStringParser.GetString("activeTile")

            _page = Arco.Doma.CMS.Data.Page.GetPage(pageId)
            If _page Is Nothing Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
            ElseIf Not _page.CanView Then
                GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
            ElseIf _pageMode = PageMode.Edit AndAlso Not _page.CanView Then
                GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
            Else
                _panelCount = ContentPage.CreatePage(Me, Me.pageCont, _page, _pageMode, action, actionID, screenID, targetPanelID)
            End If


        End If

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If _page Is Nothing Then Return

        If _pageMode = PageMode.Edit Then
            tlbEditPage.Visible = True
            tlbViewPage.Visible = False
            lnkAdd.NavigateUrl = "javascript:AddPanel(" & _page.ID & ");"
            lnkAdd.ToolTip = GetDecodedLabel("addpanel")
            lnkAdd.EnableImageSprite = True
            lnkAdd.SpriteCssClass = "icon-add-new"


            lnkEdit.ToolTip = GetDecodedLabel("editpage")
            lnkEdit.NavigateUrl = "javascript:EditPage(" & _page.ID & ");"
            lnkEdit.EnableImageSprite = True
            lnkEdit.SpriteCssClass = "icon-edit"

            lnkSave.Visible = False
            '   lnkSave.ToolTip = GetDecodedLabel("save")
            '   lnkSave.EnableImageSprite = True
            '  lnkSave.SpriteCssClass = "icon-save"

            lnkRefresh.ToolTip = GetDecodedLabel("refresh")
            lnkRefresh.NavigateUrl = "javascript:ShowPage(" & ParentID & ", " & _page.ID & ",1);"
            lnkRefresh.EnableImageSprite = True
            lnkRefresh.SpriteCssClass = "icon-refresh"

            'when opening page from Admin/Pages:
            Dim returnTo As String = QueryStringParser.GetString("returnto")
            If returnTo = "pages" Then
                Dim paging As Integer = QueryStringParser.GetInt("returntoPaging", 1)
                lnkToViewMode.NavigateUrl = "javascript:ShowPages(" & paging & ");"
            Else
                lnkToViewMode.NavigateUrl = "javascript:ShowPage(" & ParentID & ", " & _page.ID & ",0);"
            End If
            lnkToViewMode.ToolTip = GetDecodedLabel("close")
            lnkToViewMode.EnableImageSprite = True
            lnkToViewMode.SpriteCssClass = "icon-close"

        Else
            tlbEditPage.Visible = False
            tlbViewPage.Visible = True

            lnkToEditMode.OnClientClick = "javascript:ShowPage(" & ParentID & ", " & _page.ID & ",1);return false;"

            If _panelCount > 0 Then
                lnkToEditMode.CssClass = "icon icon-edit-page edit-page-button"
            Else
                lnkToEditMode.Text = GetLabel("starteditpage")
            End If
        End If

        lnkToEditMode.Visible = _page.CanEdit() AndAlso Not QueryStringParser.GetBoolean("docked")

        Dim isInAsyncPostBack As Boolean = ScriptManager.GetCurrent(Me).IsInAsyncPostBack

        If isInAsyncPostBack Then
            ParentID = QueryStringParser.GetInt("DM_PARENT_ID")
        Else
            'panel callback can't be caught,
            RenderCMSControllerScript()

            'do autosave
            If Page.IsPostBack AndAlso _pageMode = PageMode.Edit Then
                ContentPage.SavePage(Me.pageCont, _page)
            End If
        End If


    End Sub

    Private Sub RenderCMSControllerScript()
        Dim sb = New StringBuilder()
        sb.AppendLine("var control = new CMSController(" & _page.ID & ", " & ParentID & ");")
        sb.AppendLine("function CMS() { return control; }")
        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType, "CMScontroller", sb.ToString, True)
    End Sub

    'Protected Sub tlbEdit_ButtonClick(sender As Object, e As Telerik.Web.UI.RadToolBarEventArgs) Handles tlbEdit.ButtonClick
    '    Dim btn As Telerik.Web.UI.RadToolBarButton = CType(e.Item, Telerik.Web.UI.RadToolBarButton)
    '    Select Case btn.CommandName.ToUpper
    '        Case "SAVE"
    '            ContentPage.SavePage(Me.pageCont, _pageID)
    '    End Select
    'End Sub
End Class
