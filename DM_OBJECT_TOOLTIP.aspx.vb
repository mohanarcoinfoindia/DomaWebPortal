Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Retention
Imports Arco.Doma.Library.Routing
Imports Arco.Doma.Library.Website

Partial Class DM_OBJECT_TOOLTIP
    Inherits BaseTooltipPage
    Private Sub ShowError(ByVal msg As String)
        Response.Write(Arco.Web.ErrorHandler.GetErrorForm(msg))
    End Sub
    Private Sub ShowError(ByVal ex As Exception)
        ShowError(ex.Message)
    End Sub
    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)

        Dim o As DM_OBJECT = Nothing
        Dim lbFromArchive As Boolean = QueryStringParser.GetBoolean("fromarchive")
        Dim loLoaded As Arco.ApplicationServer.Library.BusinessBase
        Try
            loLoaded = QueryStringParser.CurrentDMObject
            If loLoaded Is Nothing Then
                ShowError("Object not found")
                Exit Sub
            End If

            If TypeOf (loLoaded) Is DM_OBJECT Then
                o = CType(loLoaded, DM_OBJECT)
            Else
                o = CType(loLoaded, cCase).TargetObject
            End If
            If Not o.CanViewMeta Then
                ShowError("Object not found")
                Exit Sub
            End If
        Catch ex As Exception
            ShowError(String.Format("Object {0} could not be loaded: {1}", QueryStringParser.ObjectID, ex.Message))
            Exit Sub
        End Try




        If o.Object_Type = "Case" Then
            Dim lsFromArchive As String = QueryStringParser.GetString("fromarchive")
            If Not o.Case_Active Then
                lsFromArchive = "Y"
            End If
            Server.Transfer("DM_CASE_TOOLTIP.aspx?CASE_ID=" & o.Case_ID & "&fromarchive=" & lsFromArchive)
        End If

        AddHeader()

        If TypeOf o Is Shortcut Then
            Add("Type", GetDecodedLabel(o.Object_Type))
            Add("ID", o.ID)
            Add(GetLabel("doctitle"), o.Name, True, True)
            Add(GetLabel("creationdate"), ArcoFormatting.FormatDateLabel(o.Creation_Date, True, False, False))
            Add(GetLabel("createdby"), ArcoFormatting.FormatUserName(o.Created_By), False, False)

            AddSpacer()
            o = DirectCast(o, Shortcut).GetReferencedObject
        End If

        Dim labels As Globalisation.LABELList = Nothing

        If o.Category <> 0 Then
            labels = Globalisation.LABELList.GetCategoryItemsLabelList(o.Category, EnableIISCaching)
        End If

        For Each item As ScreenItemList.ScreenItemInfo In o.GetDetailScreen(Screen.DetailScreenDisplayMode.Tooltip, Device.Web, 7).ScreenItems
            Select Case item.Type
                Case ScreenItem.ItemType.Fixed
                    Select Case item.FixedItemID
                        Case ScreenItem.FixedItem.Type
                            If o.Object_Type <> "Listitem" Then
                                Add("Type", GetDecodedLabel(o.Object_Type))
                            End If
                        Case ScreenItem.FixedItem.Category
                            Dim lsType As String = GetDecodedLabel(o.Object_Type)
                            Dim catLabel As String = ""

                            If o.HasCategory Then
                                catLabel = o.CategoryObject.TranslatedName(Arco.Security.BusinessIdentity.CurrentIdentity.Language, labels)
                            End If

                            If lsType = "Listitem" Then
                                Add(GetLabel("list"), catLabel)
                            Else
                                If Not String.IsNullOrEmpty(catLabel) Then
                                    Add(GetLabel("category"), catLabel)
                                End If
                            End If
                        Case ScreenItem.FixedItem.Name
                            Add(GetLabel("doctitle"), o.Name, True, True)
                        Case ScreenItem.FixedItem.Description
                            Add(GetLabel("description"), o.Description, True, True)
                        Case ScreenItem.FixedItem.DIN
                            If TypeOf o Is DM_VersionControlledObject Then 'show this always?
                                Add("DIN", o.DIN)
                            End If
                        Case ScreenItem.FixedItem.ID
                            Add("ID", o.ID)
                        Case ScreenItem.FixedItem.TransactionID
                            Add("Transaction ID", o.Transaction_ID)
                        Case ScreenItem.FixedItem.Version
                            If Settings.GetValue("Versioning", "EnableDocumentVersioning", True) AndAlso TypeOf o Is DM_VersionControlledObject Then
                                Add(GetLabel("version"), o.Version.toString)
                            End If
                        Case ScreenItem.FixedItem.Folder
                            If Not lbFromArchive AndAlso o.Object_Type <> "Listitem" Then
                                Dim loF As New FolderLink
                                loF.FolderID = o.Parent_ID
                                loF.ShowImages = False
                                loF.JavaScriptOpenFunction = ""
                                Add(GetLabel("folder"), loF.GetLinkContent, False, False)
                            End If
                        Case ScreenItem.FixedItem.CreationDate
                            Add(GetLabel("creationdate"), ArcoFormatting.FormatDateLabel(o.Creation_Date, True, False, False))
                        Case ScreenItem.FixedItem.CreationUser
                            Add(GetLabel("createdby"), ArcoFormatting.FormatUserName(o.Created_By), False, False)
                        Case ScreenItem.FixedItem.ModifDate
                            Add(GetLabel("modifdate"), ArcoFormatting.FormatDateLabel(o.Modified_Date, True, False, False))
                        Case ScreenItem.FixedItem.ModifUser
                            Add(GetLabel("modifby"), ArcoFormatting.FormatUserName(o.Modified_By), False, False)
                        Case ScreenItem.FixedItem.RetentionPolicy
                            If o.RetentionPolicy <> 0 AndAlso Not String.IsNullOrEmpty(o.Publish_Date) AndAlso o.RetentionPolicyNextAction <> 0 Then

                                Dim policy As Policy = Policy.GetPolicy(o.RetentionPolicy)
                                Dim actions As PolicyActionList = PolicyActionList.GetPolicyActionList(o.RetentionPolicy)
                                Dim currentFound As Boolean = False
                                Dim sb As New StringBuilder(128)
                                Dim pubDate As DateTime = Helpers.DateParser.GetDateTime(o.Publish_Date)
                                Dim origPubDate As DateTime = pubDate
                                For Each action As PolicyActionList.PolicyActionInfo In actions
                                    If action.Sequence = o.RetentionPolicyNextAction Then
                                        currentFound = True
                                    End If
                                    If currentFound Then

                                        If sb.Length <> 0 Then
                                            sb.Append("<br>")
                                        End If
                                        If policy.UpdatePublishDate Then
                                            pubDate = action.Expires.AddTo(pubDate)
                                            sb.Append(ArcoFormatting.FormatDateLabel(pubDate, True, False, False))
                                        Else
                                            sb.Append(ArcoFormatting.FormatDateLabel(action.Expires.AddTo(origPubDate), True, False, False))
                                        End If



                                        sb.Append(" : ")
                                        sb.Append(action.GetLabel())
                                    End If
                                Next
                                Add(GetLabel("retentionpolicy"), sb.ToString, False, False)

                            End If
                        Case ScreenItem.FixedItem.Tenant
                            If o.TenantId <> 0 AndAlso Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsGlobal Then
                                Dim tn As ITenant = Tenant.GetTenant(o.TenantId)
                                If tn IsNot Nothing Then
                                    Add(GetLabel("tenant"), tn.Name)
                                Else
                                    Add(GetLabel("tenant"), o.TenantId)
                                End If
                            End If
                        Case ScreenItem.FixedItem.PublishDate
                            Add(GetLabel("publishdate"), ArcoFormatting.FormatDateLabel(o.Publish_Date, True, False, False))
                        Case ScreenItem.FixedItem.Status
                            If lbFromArchive Then
                                Add(GetLabel("archivedon"), CType(o, DM_VersionControlledObject).Archive_Date)
                                Add(GetLabel("status"), Arco.EnumTranslator.GetEnumLabel(DM_OBJECT.Object_Status.Archived))
                            Else
                                Add(GetLabel("status"), Arco.EnumTranslator.GetEnumLabel(o.Status))

                                'rights
                                If Not o.CanModifyMeta() Then
                                    Add(GetLabel("edit"), o.GetLastError.Description)
                                End If

                                If o.Type.CanHaveFiles Then
                                    If Not o.CanAddFile() Then
                                        Add(GetLabel("ctx_addfile"), o.GetLastError.Description)
                                    End If
                                End If

                                If Not o.CanDelete Then
                                    Add(GetLabel("delete"), o.GetLastError.Description)
                                End If
                                'end rights

                            End If
                        Case ScreenItem.FixedItem.LockedBy
                            Select Case o.Status
                                Case DM_OBJECT.Object_Status.Locked
                                    Add(GetLabel("lockedby"), ArcoFormatting.FormatUserName(o.Modified_By), False, False)
                            End Select
                        Case ScreenItem.FixedItem.CheckOutComment
                            If Settings.GetValue("Versioning", "EnableDocumentVersioning", True) Then
                                Add(GetLabel("checkoutcomment"), o.CheckOut_Comment)
                            End If
                        Case ScreenItem.FixedItem.CheckOutBy
                            If Settings.GetValue("Versioning", "EnableDocumentVersioning", True) Then
                                Add(GetLabel("checkedoutby"), ArcoFormatting.FormatUserName(o.CheckOut_By), False, False)
                            End If
                    End Select
                Case ScreenItem.ItemType.ObjectProperty
                    'todo prop label

                    Try
                        Dim propInfo As DMObjectPROPERTYList.PROPERTYInfo = o.GetPropertyInfo(item.PROP_ID)
                        Try
                            'todo : field encodehtml
                            Add(labels.GetObjectLabel(item.PROP_ID, "Property", propInfo.Name), o.GetPropertyDisplayValue(item.PROP_ID).ToHtmlString(propInfo.PropertyDefinition.EncodeHtml), False, False)
                        Catch ex As Exception
                            Add(labels.GetObjectLabel(item.PROP_ID, "Property", propInfo.Name), propInfo.Value, False, propInfo.PropertyDefinition.EncodeHtml)
                        End Try

                    Catch ex As Exceptions.PropertyNotFoundException
                        Add("Property " & item.PROP_ID, "Not found")

                    End Try

            End Select
        Next


        AddFooter()


        litContent.Text = GetContent()

    End Sub
End Class
