Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Website

Namespace Doma
    Partial Class DM_EDIT_QUERY
        Inherits BasePage

        Protected lsAction As String = ""

#Region " Labels "
        Private mcolCats As OBJECT_CATEGORYList = Nothing
        Private Function CategoryLabel(ByVal vlCatID As Int32) As String
            If mcolCats Is Nothing Then
                mcolCats = OBJECT_CATEGORYList.GetOBJECT_CATEGORYList(False, EnableIISCaching)
            End If

            For Each loCat As OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In mcolCats
                If loCat.ID = vlCatID Then
                    Return " [" & GetCategoryLabel(loCat) & "]"
                End If
            Next
            Return ""
        End Function

#End Region
        Private Sub FillPropertiesCombo()

            cmbPropID.Items.Clear()
            cmbPropID.Items.Add(New WebControls.ListItem(GetDecodedLabel("category"), Convert.ToInt32(Condition.FixedField.Category).ToString))


            cmbPropID.Items.Add(New WebControls.ListItem(GetDecodedLabel("name"), Convert.ToInt32(Condition.FixedField.Name).ToString))

            cmbPropID.Items.Add(New WebControls.ListItem(GetDecodedLabel("createdby"), Convert.ToInt32(Condition.FixedField.CreatedBy).ToString))
            cmbPropID.Items.Add(New WebControls.ListItem(GetDecodedLabel("modifby"), Convert.ToInt32(Condition.FixedField.ModifiedBy).ToString))            
            cmbPropID.Items.Add(New WebControls.ListItem(GetDecodedLabel("status"), Convert.ToInt32(Condition.FixedField.Status).ToString))

            cmbPropID.Items.Add(New WebControls.ListItem(GetDecodedLabel("procedure"), Convert.ToInt32(Condition.FixedField.Procedure).ToString))
            cmbPropID.Items.Add(New WebControls.ListItem(GetDecodedLabel("step"), Convert.ToInt32(Condition.FixedField.StepName).ToString))
            cmbPropID.Items.Add(New WebControls.ListItem(GetDecodedLabel("assignedto"), Convert.ToInt32(Condition.FixedField.AssignedTo).ToString))


            Dim laProps As New List(Of WebControls.ListItem)

            Dim loPropCrit As New PROPERTYList.Criteria With {
                .PropertyPool = False,
                .PROC_ID = 0,
                .PARENT_PROP_ID = 0
            }
            Dim propLabels As Arco.Doma.Library.Globalisation.LABELList = Arco.Doma.Library.Globalisation.LABELList.GetPropertiesLabelList(EnableIISCaching)

            For Each loProp As PROPERTYList.PROPERTYInfo In PROPERTYList.GetPropertyList(loPropCrit)
                If loProp.PARENT_PROP_ID = 0 And loProp.Index_Status = Arco.Doma.Library.Indexing.Indexer.IndexStatus.IndexOk Then 'only indexed columns can be in a property expansion
                    Dim lsCatLabel As String = ""
                    If loProp.CatID > 0 Then
                        lsCatLabel = CategoryLabel(loProp.CatID)                   
                    End If

                    Dim loItm As WebControls.ListItem = New WebControls.ListItem(propLabels.GetObjectLabel(loProp.ID, "Property", loProp.Name) & lsCatLabel, loProp.ID.ToString)
                    laProps.Add(loItm)
                End If
            Next

            For Each li As WebControls.ListItem In laProps.OrderBy(Function(x) x.Text.ToUpper)
                cmbPropID.Items.Add(li)
            Next
        End Sub

        Private Sub FillResTypeCombo()
            drpResType.Items.Clear()
            AddResultTypeFilter(Screen.ScreenSearchMode.Objects)
            AddResultTypeFilter(Screen.ScreenSearchMode.Files)
            AddResultTypeFilter(Screen.ScreenSearchMode.Work)
            AddResultTypeFilter(Screen.ScreenSearchMode.Cases)
            AddResultTypeFilter(Screen.ScreenSearchMode.MyCases)
            AddResultTypeFilter(Screen.ScreenSearchMode.ArchivedCases)
            AddResultTypeFilter(Screen.ScreenSearchMode.OpenAndArchivedCases)
            AddResultTypeFilter(Screen.ScreenSearchMode.Mails)
            AddResultTypeFilter(Screen.ScreenSearchMode.MailInbox)
            AddResultTypeFilter(Screen.ScreenSearchMode.MailSentItems)
            AddResultTypeFilter(Screen.ScreenSearchMode.MyMails)
            AddResultTypeFilter(Screen.ScreenSearchMode.MailDeletedBox)
            AddResultTypeFilter(Screen.ScreenSearchMode.MailFollowUp)
            AddResultTypeFilter(Screen.ScreenSearchMode.MailInboxWork)
        End Sub

        Private Sub AddResultTypeFilter(ByVal value As Screen.ScreenSearchMode)
            drpResType.Items.Add(New WebControls.ListItem(Arco.EnumTranslator.GetEnumLabel(value), Convert.ToInt32(value).ToString))
        End Sub
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim loparent As DM_OBJECT

            If Not Page.IsPostBack Then
                txtObjectID.Text = QueryStringParser.GetInt("DM_OBJECT_ID").ToString
                txtParentID.Text = QueryStringParser.GetInt("DM_PARENT_ID").ToString

                FillPropertiesCombo()

                radTab.Tabs(0).Text = GetDecodedLabel("folder")
                radTab.Tabs(1).Text = GetDecodedLabel("query")


                Page.Title = GetLabel("addpropexp")
                lblFolder.Text = GetLabel("folder")

                lblName.Text = GetLabel("name")
                Label5.Text = GetLabel("name")
                Label4.Text = GetLabel("queryresultscope")
                lnkSave.Text = GetLabel("save")
                lnkCancel.Text = GetLabel("cancel")
                Label1.Text = GetLabel("property")
                Label2.Text = GetLabel("defaultresultscreen")
                Label3.Text = GetDecodedLabel("icon")
                Label6.Text = GetDecodedLabel("description")

                rdQryScope.Items(0).Text = GetLabel("public")
                rdQryScope.Items(1).Text = GetLabel("private")

                optFolderToUse.Items(0).Text = GetLabel("ontheroot")
                optFolderToUse.Items(1).Text = GetLabel("ontheexecutingfolder")

                Dim canEditQuery As Boolean = True
                If txtObjectID.Text <> "0" Then 'update
                    Dim loObj As QueryShortcut = QueryShortcut.GetQueryShortcut(Convert.ToInt32(txtObjectID.Text))
                    If loObj.Query.ID = 0 Then
                        GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                    End If
                    canEditQuery = (loObj.Query.Type = DMQuery.QueryType.PropertyExpansion OrElse
                        loObj.Query.CanModify(Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.isAdmin OrElse Folder.GetRoot().HasAccess(ACL_Access.Access_Level.ACL_Manage_Public_Saved_Queries)))

                    cmbResultScreen.Prop_Value = loObj.Default_Result_Screen
                    If String.IsNullOrEmpty(loObj.Tree_Icon) Then
                        cmbTreeIcon.Prop_Value = "query.svg"
                    Else
                        cmbTreeIcon.Prop_Value = loObj.Tree_Icon
                    End If

                    txtObjectName.Text = loObj.Name
                    optFolderToUse.SelectedValue = CStr(loObj.Query.UseBaseFolder)
                    rdQryScope.SelectedValue = CStr(loObj.Query.Scope)
                    If loObj.Query.Type = DMQuery.QueryType.PropertyExpansion Then
                        optFolderToUse.Enabled = True
                        FillResTypeCombo()
                        drpResType.SelectedValue = Convert.ToInt32(loObj.Query.ResultType).ToString


                        For Each item As WebControls.ListItem In cmbPropID.Items
                            If item.Value = loObj.Query.Prop_ID.ToString Then
                                item.Selected = True
                            End If
                        Next

                        SetPropertyExpansionsOptions(True)
                    Else
                        optFolderToUse.Enabled = False
                        Page.Title = GetLabel("Query")

                        lblResType.Text = Arco.EnumTranslator.GetEnumLabel(loObj.Query.ResultType)


                        SetPropertyExpansionsOptions(False)
                    End If

                    txtOwner.Text = loObj.Query.Owner
                    lblQueryName.Text = loObj.Query.Name
                    lblQueryDesc.Text = loObj.Query.Description
                    txtParentID.Text = loObj.Query.ParentID.ToString


                    loparent = loObj.GetExecutionFolder()
                Else 'new
                    'this is always a property expansion
                    FillResTypeCombo()

                    txtOwner.Text = Arco.Security.BusinessIdentity.CurrentIdentity.Name
                    cmbTreeIcon.Prop_Value = "query.svg"
                    loparent = ObjectRepository.GetObject(Convert.ToInt32(txtParentID.Text))

                    SetPropertyExpansionsOptions(True)

                End If


                SetQueryScopeOptions(canEditQuery)

                lblParentName.Text = loparent.GetFullPath
                If loparent.Object_Type <> "Folder" Then
                    lblFolder.Text = GetLabel(loparent.Object_Type)

                    optFolderToUse.Items(1).Text = optFolderToUse.Items(1).Text.Replace(GetLabel("folder").ToLower, GetLabel(loparent.Object_Type).ToLower)
                End If

            End If

        End Sub

        Private Sub SetPropertyExpansionsOptions(ByVal isPropExp As Boolean)
            lblResType.Visible = Not isPropExp
            drpResType.Visible = isPropExp
            rowProp.Visible = isPropExp
            rowQueryName.Visible = Not isPropExp
            rowQueryDescription.Visible = Not isPropExp
        End Sub

        Private Sub SetQueryScopeOptions(ByVal canEditQuery As Boolean)
            Dim llParentID As Int32 = Convert.ToInt32(txtParentID.Text)
            Dim lbHasEditAccess As Boolean = Folder.GetRoot().HasAccess(ACL_Access.Access_Level.ACL_Create_QueryShortcut)
            If Not lbHasEditAccess Then
                rdQryScope.Items(0).Enabled = False
                If txtOwner.Text = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
                    rdQryScope.Items(1).Selected = True
                    lbHasEditAccess = True
                End If
            End If

            If lbHasEditAccess Then
                SetEditMode(canEditQuery)
            Else
                SetReadOnlyMode()
            End If

        End Sub
        Private Sub SetReadOnlyMode()
            lnkSave.Visible = False
            txtObjectName.Enabled = False
            optFolderToUse.Enabled = False
            cmbPropID.Enabled = False
            cmbResultScreen.Enabled = False
            cmbTreeIcon.Enabled = False
            drpResType.Enabled = False
            rdQryScope.Enabled = False
        End Sub
        Private Sub SetEditMode(ByVal canEditQuery As Boolean)
            lnkSave.Visible = True
            If Not canEditQuery Then
                optFolderToUse.Enabled = False
            End If

            txtObjectName.Enabled = canEditQuery
            cmbPropID.Enabled = canEditQuery

            cmbResultScreen.Enabled = True
            cmbTreeIcon.Enabled = True
            drpResType.Enabled = canEditQuery
            rdQryScope.Enabled = canEditQuery
        End Sub

        Protected Sub DoSave(ByVal sender As Object, ByVal e As EventArgs)


            Dim lsName As String = txtObjectName.Text.Trim
            If Not String.IsNullOrEmpty(lsName) AndAlso Not String.IsNullOrEmpty(cmbPropID.SelectedValue) Then
                ShowInfoMessage(GetLabel("saveok"))
                If txtObjectID.Text = "0" Then
                    'this is always a property expansion
                    If Not QueryShortcut.Exists(Convert.ToInt32(txtParentID.Text), lsName) Then
                        Dim q As DMQuery = DMQuery.NewPropertyExpansion(Convert.ToInt32(cmbPropID.SelectedValue))

                        q.ParentID = Convert.ToInt32(txtParentID.Text)
                        q.Name = lsName
                        q.Scope = CType(rdQryScope.SelectedValue, DMQuery.QueryScope)
                        q.UseBaseFolder = CType(optFolderToUse.SelectedValue, DMQuery.FolderToUse)

                        q.ResultType = CType(drpResType.SelectedValue, Screen.ScreenSearchMode)
                        If q.IsValid Then
                            q = q.Save 'create the expansion

                            Dim loObj As QueryShortcut = q.CreateShortCut(q.ParentID, True) 'and link to the tree
                            If loObj.ID > 0 Then
                                loObj.Default_Result_Screen = Convert.ToInt32(cmbResultScreen.Prop_Value)
                                loObj.Tree_Icon = cmbTreeIcon.Prop_Value.ToString
                                loObj.Scope = GetSelectedScope()

                                loObj = loObj.Save
                                lsAction = loObj.Parent_ID.ToString
                            Else
                                DMQuery.DeleteQuery(q.ID)
                                ShowErrorMessage(loObj.GetLastError.Description)
                            End If
                        Else
                            ShowErrorMessage(q.ValidationResults(0).ErrorMessage)
                        End If
                    Else
                        ShowErrorMessage(LibError.GetErrorDescription(LibError.ErrorCode.ERR_FOLDEREXISTS))
                    End If

                Else

                    Dim loObj As QueryShortcut = QueryShortcut.GetQueryShortcut(Convert.ToInt32(txtObjectID.Text))
                    If Not QueryShortcut.Exists(loObj.Parent_ID, lsName, loObj.ID) Then
                        loObj.Name = lsName
                        loObj.Default_Result_Screen = Convert.ToInt32(cmbResultScreen.Prop_Value)
                        loObj.Tree_Icon = cmbTreeIcon.Prop_Value.ToString
                        loObj.Scope = GetSelectedScope()

                        loObj = loObj.Save()

                        If loObj.Query.Type = DMQuery.QueryType.PropertyExpansion Then
                            loObj.Query.Prop_ID = Convert.ToInt32(cmbPropID.SelectedValue)

                            loObj.Query.UseBaseFolder = CType(optFolderToUse.SelectedValue, DMQuery.FolderToUse)
                            loObj.Query.ResultType = CType(drpResType.SelectedValue, Screen.ScreenSearchMode)
                            loObj.Query.Scope = GetSelectedScope()

                            loObj.Query.Save()
                        End If

                        lsAction = loObj.Parent_ID.ToString
                    Else
                        ShowErrorMessage(LibError.GetErrorDescription(LibError.ErrorCode.ERR_FOLDEREXISTS))
                    End If
                End If


                container1.Visible = False

            End If


        End Sub

        Private Function GetSelectedScope() As Scope
            If CType(rdQryScope.SelectedValue, DMQuery.QueryScope) = DMQuery.QueryScope.PrivateScope Then
                Return Scope.Private
            End If
            Return Scope.Public
        End Function

        Private Sub ShowInfoMessage(ByVal text As String)
            lblMsgText.CssClass = "InfoMessage"
            ShowMessage(text)
        End Sub
        Private Sub ShowErrorMessage(ByVal text As String)
            lblMsgText.CssClass = "ErrorMessage"
            ShowMessage(text)
        End Sub
        Private Sub ShowMessage(ByVal text As String)
            lblMsgText.Visible = True
            lblMsgText.Text = text
        End Sub
    End Class
End Namespace
