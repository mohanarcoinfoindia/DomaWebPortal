Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Security

Partial Class Queries
    Inherits BasePage

    Private mbCanManagePublicQueries As Boolean

    Private mbDD As Boolean

    Private _hasShortCuts As New Dictionary(Of Int32, Boolean)

    Protected ReadOnly Property DragAndDropEnabled() As Boolean
        Get
            Return mbDD
        End Get
    End Property

    Public Property CurrentPage() As Int32

    Public Property LastPage() As Int32

    Public Property RecordsPerPage() As Int32

    Public Property NumberOfResults() As Int32


    Public Function CanSubscribeToQuery(ByVal voQuery As DMQueryList.QueryInfo) As Boolean
        Select Case voQuery.Scope
            Case DMQuery.QueryScope.PrivateScope
                Return voQuery.Owner = Arco.Security.BusinessIdentity.CurrentIdentity.Name
            Case DMQuery.QueryScope.ACLScope
                Return voQuery.Rights().HasAccess(Website.QueryRights.Query_Access_Level.ACL_UseQuerySubscription)
            Case Else
                Return True
        End Select
    End Function
    Public Function EditQueryButton(ByVal voQuery As DMQueryList.QueryInfo) As String
        If CanEditQuery(voQuery) Then
            Return String.Format("<a class='ButtonLink' href='Javascript:EditQuery({0},{1},{2});'><span class='icon icon-edit' title='{3}'></span></a>",
                                 voQuery.ID, voQuery.SearchScreenID, Convert.ToInt32(voQuery.ResultType), GetLabel("edit"))
        End If
        Return ""
    End Function
    Public Function DeleteQueryButton(ByVal voQuery As DMQueryList.QueryInfo) As String
        If CanDeleteQuery(voQuery) Then
            Return String.Format("<a class='ButtonLink' href='Javascript:DeleteQuery({0});'><span class='icon icon-delete' title='{1}'></span></a>",
                                 voQuery.ID, GetLabel("delete"))
        End If
        Return ""
    End Function

    Public Function QueryOwner(ByVal voQuery As DMQueryList.QueryInfo) As String
        If voQuery.Type = DMQuery.QueryType.Normal Then
            Return ArcoFormatting.FormatUserName(voQuery.Owner)
        End If
        Return ""

    End Function
    Private _querysubs As IEnumerable(Of SubscriptionList.SubscriptionInfo)
    Private ReadOnly Property QuerySubscriptions As IEnumerable(Of SubscriptionList.SubscriptionInfo)
        Get
            If _querysubs Is Nothing Then
                Dim crit As New SubscriptionList.Criteria With {
                    .User = BusinessIdentity.CurrentIdentity.Name,
                    .ObjectType = "Query"
                }
                _querysubs = SubscriptionList.GetSubscriptionList(crit).Cast(Of SubscriptionList.SubscriptionInfo)()
            End If
            Return _querysubs
        End Get
    End Property

    Public Function ShortCutsField(ByVal voQuery As DMQueryList.QueryInfo) As String

        If _hasShortCuts.ContainsKey(voQuery.ID) AndAlso _hasShortCuts(voQuery.ID) = False Then
            Return False
        End If

        Dim content As New StringBuilder
        For Each shortcut As Search.DM_OBJECTSearch.OBJECTInfo In DMQuery.GetQueryShortCuts(voQuery.ID)
            content.Append(shortcut.BusinessObject.GetFullPath)
            content.Append("<br/>")

            'content.AppendLine(String.Join("/", shortcut.Path))
        Next
        _hasShortCuts(voQuery.ID) = (content.Length <> 0)

        If content.Length <> 0 Then
            content.Insert(0, "<b>" & GetLabel("usedin") & ":</b><br/>")
            Return ArcoFormatting.AddTooltip("<span class='icon icon-delete icon-disabled'>", content.ToString)
        End If
        Return ""
    End Function

    Public Function SubscriptionButtons(ByVal voQuery As DMQueryList.QueryInfo) As String
        If CanSubscribeToQuery(voQuery) Then

            Dim lsButtons As New StringBuilder
            Dim loSub As SubscriptionList.SubscriptionInfo = QuerySubscriptions.FirstOrDefault(Function(x) x.Object_ID = voQuery.ID AndAlso x.Subject_ID = BusinessIdentity.CurrentIdentity.Name)
            If loSub IsNot Nothing Then
                'subsc on username
                lsButtons.Append("<a class='ButtonLink' href='javascript:EditSubscription(")
                lsButtons.Append(loSub.ID)
                lsButtons.Append(");'><span class='icon icon-subscription' title='")
                lsButtons.Append(GetLabel("alreadysubscribed"))
                lsButtons.Append("'></span></a>")
            Else
                loSub = _querysubs.FirstOrDefault(Function(x) x.Object_ID = voQuery.ID)
                If loSub IsNot Nothing Then
                    'subsc on role/group
                    If BusinessIdentity.CurrentIdentity.isAdmin Then
                        lsButtons.Append("<a class='ButtonLink' href='javascript:EditSubscription(")
                        lsButtons.Append(loSub.ID)
                        lsButtons.Append(");'><span class='icon icon-subscription' title='")
                        lsButtons.Append(GetLabel("edit"))
                        lsButtons.Append("'></span></a>")
                    Else
                        lsButtons.Append("<span class='icon icon-subscription' title='")
                        lsButtons.Append(GetLabel("alreadysubscribed"))
                        lsButtons.Append("'></span>")
                    End If
                Else
                    'not subscribed yet in any way
                    lsButtons.Append("<a class='ButtonLink' href='javascript:SubscribeToQuery(" & voQuery.ID & ");'><span class='icon icon-subscriptions-add' title='")
                    lsButtons.Append(GetLabel("subscribe"))
                    lsButtons.Append("'></span></a>")
                End If
            End If
            If BusinessIdentity.CurrentIdentity.isAdmin Then
                lsButtons.Append("&nbsp;<a class='ButtonLink' href='javascript:ViewSubscriptions(")
                lsButtons.Append(voQuery.ID)
                lsButtons.Append(");'><span class='icon icon-subscriptions' title='")
                lsButtons.Append(GetLabel("subscriptions"))
                lsButtons.Append("'></span></a>")
            End If
            Return lsButtons.ToString
        Else
            Return ""
        End If
    End Function
    Public Function CanRunQuery(ByVal voQuery As DMQueryList.QueryInfo) As Boolean
        Return voQuery.CanUse
    End Function
    Public Function CanEditQuery(ByVal voQuery As DMQueryList.QueryInfo) As Boolean
        Return voQuery.Type = baseObjects.DMQuery.QueryType.Normal AndAlso voQuery.CanModify(mbCanManagePublicQueries)
    End Function

    Public Function CanDeleteQuery(ByVal voQuery As DMQueryList.QueryInfo) As Boolean
        If _hasShortCuts.ContainsKey(voQuery.ID) AndAlso _hasShortCuts(voQuery.ID) = True Then
            Return False
        End If
        Return voQuery.Type = baseObjects.DMQuery.QueryType.Normal AndAlso voQuery.CanDelete(mbCanManagePublicQueries)
    End Function
    Public Function CanSetAcl(ByVal voQuery As DMQueryList.QueryInfo) As Boolean
        Return voQuery.Type = baseObjects.DMQuery.QueryType.Normal AndAlso voQuery.CanSetAcl(mbCanManagePublicQueries)
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim loTreeLayout As SiteManagement.TreeDefinition = SiteManagement.TreeDefinition.GetMainTreeLayout
        mbDD = loTreeLayout.DragDrop

        mbCanManagePublicQueries = BusinessIdentity.CurrentIdentity.isAdmin OrElse Folder.GetRoot().HasAccess(ACL.ACL_Access.Access_Level.ACL_Manage_Public_Saved_Queries)
        If Not String.IsNullOrEmpty(delqry.Value) Then
            Dim llID As Int32 = Convert.ToInt32(delqry.Value)
            Dim loQry As DMQuery = DMQuery.GetQuery(llID)
            If loQry IsNot Nothing Then
                If loQry.CanDelete(mbCanManagePublicQueries) Then
                    DMQuery.DeleteQuery(loQry.ID)
                End If
            End If
            delqry.Value = ""
        End If

        RecordsPerPage = ArcoInfoSettings.DefaultRecordsPerPage

        Dim lsPage As String = Request.Form("Page")

        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If

        Dim llFirstRec As Int32 = ((CInt(lsPage) - 1) * RecordsPerPage) + 1
        Dim llLastRec As Int32 = llFirstRec + RecordsPerPage - 1


        Dim loQueryListCrit As New DMQueryList.Criteria(DMQueryList.Criteria.TypeFilter.Normal Or DMQueryList.Criteria.TypeFilter.System, True) With {
            .Range = ListRangeRequest.Range(llFirstRec, llLastRec),
            .Name = txtFilterName.Text,
            .Description = txtFilterDesc.Text,
            .Owner = txtFilterOwner.Text,
            .ResultType = Convert.ToInt32(drpResultTypeFilter.SelectedValue),
             .Scope = Convert.ToInt32(drpScopeFilter.SelectedValue),
            .OrderBy = GetActualOrderBy()
        }


        Dim lcolQueries As DMQueryList = DMQueryList.GetQueryList(loQueryListCrit)

        If lcolQueries.Any Then
            Dim loPagedDataSource As New PagedDataSource With {
                .AllowPaging = True,
                .PageSize = RecordsPerPage,
                .DataSource = lcolQueries,
                .CurrentPageIndex = CInt(lsPage) - 1
            }

            NumberOfResults = loPagedDataSource.DataSourceCount
            CurrentPage = CInt(lsPage)
            LastPage = loPagedDataSource.PageCount
            lstQueries.DataSource = loPagedDataSource
            lstQueries.DataBind()


            lnkPrev.Text = GetLabel("previous")
            lnkPrev.NavigateUrl = "javascript:Goto(" & CurrentPage - 1 & ");"
            lnkPrev.Enabled = Not loPagedDataSource.IsFirstPage

            lnkNext.Text = GetLabel("next")
            lnkNext.NavigateUrl = "javascript:Goto(" & CurrentPage + 1 & ");"
            lnkNext.Enabled = Not loPagedDataSource.IsLastPage

            litScroller.Text = PageScroller.GetPageScroller(CurrentPage, LastPage).Render("Goto")
        End If
    End Sub

    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "QUERY_NAME", "QUERY_DESCRIPTION", "QUERY_OWNER", "QUERY_SCOPE", "RES_TYPE"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRender
        'add init script

        Dim sb As New StringBuilder


        sb.Append(" function InitPage(){ if (PC().InitDone()){if (parent){if (parent.SetCaption){parent.SetCaption(")
        sb.Append(EncodingUtils.EncodeJsString(GetDecodedLabel("savedQueries")))
        sb.Append(");} if (parent.SetFolder){ parent.SetFolder(")
        sb.Append(PC.ParentID)
        sb.Append(",0);}}} else {setTimeout('InitPage()',100) ;}}setTimeout('InitPage()',100) ;")

        ScriptManager.RegisterStartupScript(Page, Page.GetType, "init" & ClientID, sb.ToString, True)

    End Sub

    Private Sub Queries_Init(sender As Object, e As EventArgs) Handles Me.Init

        drpResultTypeFilter.Items.Add(New WebControls.ListItem("", "-1"))
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.Objects)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.Files)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.Work)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.Cases)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.MyCases)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.ArchivedCases)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.OpenAndArchivedCases)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.Mails)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.MailInbox)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.MailSentItems)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.MyMails)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.MailDeletedBox)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.MailFollowUp)
        AddResultTypeFilter(Website.Screen.ScreenSearchMode.MailInboxWork)

        drpScopeFilter.Items.Add(New WebControls.ListItem("", "-1"))
        AddScopeTypeFilter(DMQuery.QueryScope.PublicScope)
        AddScopeTypeFilter(DMQuery.QueryScope.PrivateScope)
        AddScopeTypeFilter(DMQuery.QueryScope.ACLScope)
    End Sub

    Private Sub AddResultTypeFilter(ByVal value As Website.Screen.ScreenSearchMode)
        drpResultTypeFilter.Items.Add(New WebControls.ListItem(Arco.EnumTranslator.GetEnumLabel(value), Convert.ToInt32(value).ToString))
    End Sub
    Private Sub AddScopeTypeFilter(ByVal value As DMQuery.QueryScope)
        drpScopeFilter.Items.Add(New WebControls.ListItem(Arco.EnumTranslator.GetEnumLabel(value), Convert.ToInt32(value).ToString))
    End Sub
End Class
