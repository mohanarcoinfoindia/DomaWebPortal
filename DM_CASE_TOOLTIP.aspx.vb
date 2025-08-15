Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Routing
Imports Arco.Doma.Library.Website

Partial Class DM_CASE_TOOLTIP
    Inherits BaseTooltipPage

    Private _init As Boolean

    Private Function GetTargetObjectTypeLabel(ByVal loCase As cCase) As String
        If loCase.TargetObject.Object_Type = "Case" Then
            Return GetLabel(loCase.TargetObject.Object_Type) & " " & GetLabel("Object")
        Else
            Return GetLabel(loCase.TargetObject.Object_Type)
        End If
    End Function

    Private Sub BuildCaseToolTip(ByVal loCase As cCase)
        If loCase Is Nothing OrElse Not loCase.CanView Then
            Add("Case not found")
            Exit Sub
        End If

        If _labels Is Nothing AndAlso loCase.Proc_ID <> 0 Then
            _labels = Globalisation.LABELList.GetProcedureItemsLabelList(loCase.Proc_ID, EnableIISCaching)
        End If

        For Each item As ScreenItemList.ScreenItemInfo In loCase.GetDetailScreen(Screen.DetailScreenDisplayMode.Tooltip, Device.Web, 7).ScreenItems
            Select Case item.Type
                Case ScreenItem.ItemType.Fixed
                    Select Case item.FixedItemID
                        Case ScreenItem.FixedItem.CaseID
                            If Not _init Then Add(GetLabel("caseid"), loCase.Case_ID)
                        Case ScreenItem.FixedItem.ID
                            If Not _init Then Add(GetTargetObjectTypeLabel(loCase) & " ID", loCase.TargetObject.ID)
                        Case ScreenItem.FixedItem.DIN
                            If Not _init Then Add(GetTargetObjectTypeLabel(loCase) & " DIN", loCase.TargetObject.DIN)
                        Case ScreenItem.FixedItem.Folder
                            If loCase.TargetObject.Object_Type <> "Listitem" Then
                                Dim loF As New FolderLink
                                loF.FolderID = loCase.TargetObject.Parent_ID
                                loF.ShowImages = False
                                loF.JavaScriptOpenFunction = ""
                                Add(GetTargetObjectTypeLabel(loCase) & " " & GetLabel("folder"), loF.GetLinkContent, False, False)
                            End If
                        Case ScreenItem.FixedItem.CaseName
                            If Not _init Then Add(GetLabel("name"), loCase.CaseData.Name, True)
                        Case ScreenItem.FixedItem.Procedure
                            If Not _init Then Add(GetLabel("procedure"), loCase.Proc_ID & " : " & _labels.GetObjectLabel(loCase.Proc_ID, "Procedure", loCase.CurrentProcedure.Name), True)
                        Case ScreenItem.FixedItem.CreationUser
                            If Not _init Then Add(GetLabel("createdby"), ArcoFormatting.FormatUserName(loCase.CaseData.Created_By), False, False)
                        Case ScreenItem.FixedItem.CreationDate
                            If Not _init Then Add(GetLabel("date"), ArcoFormatting.FormatDateLabel(loCase.CaseData.Creation_Date, True, False, False))
                        Case ScreenItem.FixedItem.DueDate
                            If Not _init AndAlso Not String.IsNullOrEmpty(loCase.CaseData.DeathDate) Then
                                Add(GetLabel("duedate"), ArcoFormatting.FormatDateLabel(loCase.CaseData.DeathDate, True, False, False))
                            End If
                        Case ScreenItem.FixedItem.Tenant
                            If Not _init AndAlso loCase.CaseData.TenantId <> 0 AndAlso Security.BusinessIdentity.CurrentIdentity.IsGlobal Then
                                Dim tn As Tenant = Tenant.GetTenant(loCase.CaseData.TenantId)
                                If tn IsNot Nothing Then
                                    Add(GetLabel("tenant"), tn.Name)
                                Else
                                    Add(GetLabel("tenant"), loCase.CaseData.TenantId)
                                End If
                            End If
                        Case ScreenItem.FixedItem.Description
                            Add(GetLabel("description"), loCase.Description)
                        Case ScreenItem.FixedItem.TechID
                            AddSpacer()
                            Add(GetLabel("techid"), loCase.Tech_ID)
                        Case ScreenItem.FixedItem.StepID
                            Add(GetLabel("stepid"), loCase.Step_ID)
                        Case ScreenItem.FixedItem.StepGroup
                            If loCase.CurrentStep IsNot Nothing AndAlso loCase.CurrentStep.Group_ID <> 0 Then
                                Add(GetLabel("stepgroup"), StepGroup.GetStepGroup(loCase.CurrentStep.Group_ID).TranslatedName())
                            End If
                        Case ScreenItem.FixedItem.StepDescription
                            If loCase.CurrentStep IsNot Nothing Then
                                Add(GetLabel("description") & " (" & GetLabel("step") & ")", loCase.CurrentStep.Description)
                            End If
                        Case ScreenItem.FixedItem.ProcedureDescription
                            If Not _init AndAlso loCase.CurrentProcedure IsNot Nothing Then
                                Add(GetLabel("description") & " (" & GetLabel("procedure") & ")", loCase.CurrentProcedure.Description)
                            End If
                        Case ScreenItem.FixedItem.StepName
                            Add(GetLabel("Step"), _labels.GetObjectLabel(loCase.Step_ID, "Step", loCase.CurrentStep.Name), True)
                        Case ScreenItem.FixedItem.StepStartDate
                            Add(GetLabel("stepdate"), ArcoFormatting.FormatDateLabel(loCase.Step_StartDate, True, False, False))
                        Case ScreenItem.FixedItem.StepDueDate
                            If Not String.IsNullOrEmpty(loCase.Step_DueDate) Then
                                Add(GetLabel("stepduedate"), ArcoFormatting.FormatDateLabel(loCase.Step_DueDate, True, False, False))
                            End If
                        Case ScreenItem.FixedItem.StepExecutor
                            If Not String.IsNullOrEmpty(loCase.StepExecutor) Then
                                Add(GetLabel("lockedby"), ArcoFormatting.FormatUserName(loCase.StepExecutor), False, False)
                            End If
                        Case ScreenItem.FixedItem.Priority
                            Add(GetLabel("priority"), loCase.Priority)
                        Case ScreenItem.FixedItem.AssignedTo
                            Dim lsAssignedTo As List(Of String) = New List(Of String)
                            For Each w As WorkItemList.WorkInfo In loCase.WorkItemList
                                Dim lsSubject As String
                                If w.SubjectType = "User" Then
                                    lsSubject = ArcoFormatting.FormatUserName(w.Subject)
                                Else
                                    lsSubject = Server.HtmlEncode(w.Subject)
                                End If
                                If Not lsAssignedTo.Contains(lsSubject) Then lsAssignedTo.Add(lsSubject)
                            Next

                            If lsAssignedTo.Any Then
                                Add(GetLabel("assignedto"), String.Join(", ", lsAssignedTo), False, False)
                            End If
                        Case ScreenItem.FixedItem.LastStepExecutor
                            If Not String.IsNullOrEmpty(loCase.LastStepExecutor) Then
                                Add(GetLabel("laststepexecutor"), ArcoFormatting.FormatUserName(loCase.LastStepExecutor), False, False)
                            End If
                        Case ScreenItem.FixedItem.SuspendedInfo

                            If loCase.Suspended Then
                                If Not String.IsNullOrEmpty(loCase.SuspendUntil) Then
                                    Add(GetDecodedLabel("Suspended until"), ArcoFormatting.FormatDateLabel(loCase.SuspendUntil, True, False, False))
                                Else
                                    Add(GetDecodedLabel("Suspended until"), GetDecodedLabel("Manual"))
                                End If
                            End If

                    End Select
                Case ScreenItem.ItemType.ObjectProperty
                    'todo prop label
                    If Not _init Then
                        Try
                            Dim propInfo As DMObjectPROPERTYList.PROPERTYInfo = loCase.GetPropertyInfo(item.PROP_ID)
                            Try
                                'todo field encoding
                                Add(_labels.GetObjectLabel(item.PROP_ID, "Property", propInfo.Name), loCase.GetPropertyDisplayValue(item.PROP_ID).ToHtmlString(propInfo.PropertyDefinition.EncodeHtml), False, False)
                            Catch ex As Exception
                                Add(propInfo.Name, propInfo.Value, False, propInfo.PropertyDefinition.EncodeHtml)
                            End Try
                        Catch ex As Exceptions.PropertyNotFoundException
                            Add("Property " & item.PROP_ID, "Not found")
                        End Try

                    End If
            End Select
        Next


        _init = True
    End Sub

    Private Sub BuildHistoryCaseToolTip(ByVal loHistCase As HistoryCase)
        If loHistCase Is Nothing OrElse Not loHistCase.CanViewMeta Then
            Add("Case not found")
            Exit Sub
        End If

        If loHistCase.Proc_ID <> 0 Then
            _labels = Globalisation.LABELList.GetProcedureItemsLabelList(loHistCase.Proc_ID, EnableIISCaching)
        End If

        Dim loScreen As Screen = loHistCase.GetDetailScreen(Screen.DetailScreenDisplayMode.Tooltip, Device.Web, 7)
        For Each item As ScreenItemList.ScreenItemInfo In loScreen.ScreenItems
            Select Case item.Type
                Case ScreenItem.ItemType.Fixed
                    Select Case item.FixedItemID
                        Case ScreenItem.FixedItem.CaseID
                            Add(GetLabel("case") & " ID", loHistCase.Case_ID)
                        Case ScreenItem.FixedItem.CaseName
                            Add(GetLabel("name"), loHistCase.Name)
                        Case ScreenItem.FixedItem.Procedure
                            Add(GetLabel("procedure"), loHistCase.Proc_ID & " : " & _labels.GetObjectLabel(loHistCase.Proc_ID, "Procedure", loHistCase.CurrentProcedure.Name))
                        Case ScreenItem.FixedItem.CreationUser
                            Add(GetLabel("createdby"), ArcoFormatting.FormatUserName(loHistCase.Created_By), False, False)
                        Case ScreenItem.FixedItem.CreationDate
                            Add(GetLabel("date"), ArcoFormatting.FormatDateLabel(loHistCase.Creation_Date, True, False, False))
                        Case ScreenItem.FixedItem.CaseEndDate
                            Add(GetLabel("caseenddate"), ArcoFormatting.FormatDateLabel(loHistCase.Date_End, True, False, False))
                    End Select
                Case ScreenItem.ItemType.ObjectProperty
                    'todo prop label

                    Try
                        Dim propInfo As DMObjectPROPERTYList.PROPERTYInfo = loHistCase.GetPropertyInfo(item.PROP_ID)

                        'todo : field encoding
                        Add(_labels.GetObjectLabel(item.PROP_ID, "Property", propInfo.Name), loHistCase.GetPropertyDisplayValue(item.PROP_ID).ToHtmlString(propInfo.PropertyDefinition.EncodeHtml), False, False)
                    Catch ex As Exceptions.PropertyNotFoundException
                        Add("Property " & item.PROP_ID, "Not found")
                    End Try
            End Select
        Next

    End Sub
    Private _labels As Globalisation.LABELList = Nothing

    Private Sub ShowError(ByVal ex As Exception)
        ShowError(ex.Message)
    End Sub
    Private Sub ShowError(ByVal msg As String)
        Response.Write(Arco.Web.ErrorHandler.GetErrorForm(msg))
    End Sub
    Protected Overrides Sub OnLoad(ByVal e As EventArgs)

        Dim llTechID As Int32 = QueryStringParser.GetInt("TECH_ID")
        Dim llCaseID As Int32 = 0
        If llTechID = 0 Then
            llCaseID = QueryStringParser.GetInt("CASE_ID")
        End If

        Dim lbFromArchive As Boolean

        If llTechID = 0 And llCaseID = 0 Then
            ShowError("No tooltip for object 0")
            Exit Sub
        End If


        AddHeader()
        If llTechID > 0 Then
            Try
                Dim loCase As cCase = cCase.GetCase(llTechID)
                BuildCaseToolTip(loCase)
            Catch ex As Exception
                ShowError(String.Format("Case {0} could not be loaded: {1}", llTechID, ex.Message))
                Exit Sub
            End Try

        Else
            lbFromArchive = QueryStringParser.GetBoolean("fromarchive")
            If Not lbFromArchive Then
                Dim lcolcases As cCaseList = cCaseList.GetCases(llCaseID)
                If lcolcases.Any Then
                    For Each loCaseInfo As cCaseList.CaseInfo In lcolcases
                        Try

                            Dim loCase As cCase = cCase.GetCase(loCaseInfo.Tech_ID)
                            BuildCaseToolTip(loCase)
                        Catch ex As Exception
                            ShowError(String.Format("Case {0} could not be loaded: {1}", loCaseInfo.Tech_ID, ex.Message))
                            Exit Sub
                        End Try
                    Next
                Else
                    Dim loHistCase As HistoryCase = HistoryCase.GetHistoryCase(llCaseID)
                    BuildHistoryCaseToolTip(loHistCase)
                End If

            Else
                Dim loHistCase As HistoryCase = HistoryCase.GetHistoryCase(llCaseID)

                BuildHistoryCaseToolTip(loHistCase)
            End If
        End If
        AddFooter()

        litContent.Text = GetContent()
    End Sub

End Class
