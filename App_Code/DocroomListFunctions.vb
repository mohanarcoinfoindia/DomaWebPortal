Imports System.Web.Services
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.BatchJobs
Imports Arco.Doma.Library.Mail
Imports Arco.Doma.Library.Routing
Imports Arco.Doma.Library.Security

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class DocroomListFunctions
    Inherits System.Web.Services.WebService
    <WebMethod(EnableSession:=True)>
    Public Sub SetMainFile(ByVal fileID As Integer)
        Dim file As File = File.GetFile(fileID)
        If file IsNot Nothing AndAlso file.CanModify Then
            file.SetAsMainFile()
        End If

    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub ToggleFileReadOnly(ByVal fileID As Integer)
        Dim file As File = File.GetFile(fileID)
        If file IsNot Nothing AndAlso file.CanModify Then
            file.IsReadOnly = Not file.IsReadOnly
            file.Save()
        End If
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Function CheckoutFile(ByVal vlFileID As Integer, ByVal vsComment As String) As Integer
        ' Throw New HttpException(403, "cette opération n'est pas permis")        
        Dim oFile As File = File.GetFile(vlFileID)
        If oFile Is Nothing Then Return -1

        If oFile.CanCheckOut Then
            oFile = oFile.CheckOut(vsComment)
            If Not oFile.HasError Then Return oFile.ID
        End If
        Throw New Exception(oFile.GetLastError.Description)
    End Function
    <WebMethod(EnableSession:=True)>
    Public Sub CancelCheckoutFile(ByVal vlID As Integer)
        Dim oFile As File = File.GetFile(vlID)
        If oFile Is Nothing Then Return

        If oFile.CanCheckIn Then
            oFile.CancelCheckout()
        End If
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub ReIndexFile(ByVal vlID As Integer)
        Dim oFile As File = File.GetFile(vlID)
        If oFile Is Nothing Then Return
        If BusinessIdentity.CurrentIdentity.isAdmin OrElse oFile.CanModify Then

            oFile.Reindex()
        End If

    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub DeleteMailFromBox(ByVal vlMailID As Integer, ByVal vlBoxID As MailBoxItem.MailBox)
        Dim loMail As DMMail = DMMail.GetMail(vlMailID)
        If loMail Is Nothing Then Return

        If loMail IsNot Nothing Then
            loMail.DeleteForCurrentUser(vlBoxID)
        End If
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub ToggleReadOnMail(ByVal vlMailID As Integer)
        Dim loMail As DMMail = DMMail.GetMail(vlMailID)
        If loMail IsNot Nothing Then
            loMail.ToggleReadForCurrentUser()
        End If
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub CloseTrackingOnMail(ByVal vlMailID As Integer)
        Dim loMail As DMMail = DMMail.GetMail(vlMailID)
        If loMail IsNot Nothing Then
            loMail.CloseTrackingItemForCurrentUser()
        End If

    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub StopFollowupOnMail(ByVal vlMailID As Integer)
        Dim loMail As DMMail = DMMail.GetMail(vlMailID)

        loMail.StopTrackingFollowup()

    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub RestoreMailBoxItem(ByVal vlMailID As Integer, ByVal vlBoxID As MailBoxItem.MailBox)
        Dim loMailBoxItem As MailBoxItem = MailBoxItem.GetMailBoxItem(vlMailID, vlBoxID)
        If loMailBoxItem IsNot Nothing Then
            If loMailBoxItem.Status = MailBoxItem.MailStatus.Closed Then
                loMailBoxItem.Status = MailBoxItem.MailStatus.Open
                loMailBoxItem.Save()
            End If
        End If
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub DeleteObject(ByVal vlObjectID As Integer)
        Dim loObject As DM_OBJECT = ObjectRepository.GetObject(vlObjectID)
        If loObject Is Nothing Then Return

        Dim lbAsync As Boolean = (loObject.Status = DM_OBJECT.Object_Status.Deleted AndAlso loObject.HasChildren)

        If loObject.CanDelete Then
            If Not loObject.Delete(lbAsync) Then
                Throw New Exception(loObject.GetLastError.Description)
            End If
        Else
            Throw New Exception(loObject.GetLastError.Description)
        End If
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub DeleteFile(ByVal fileID As Integer)
        Dim oFile As File = File.GetFile(fileID)
        If oFile IsNot Nothing AndAlso oFile.CanDelete Then
            oFile.Delete()
        End If
    End Sub


    <WebMethod(EnableSession:=True)>
    Public Sub DeleteCaseNote(ByVal noteID As Integer)
        Dim note As Note = Note.GetCaseNote(noteID)
        If note Is Nothing Then Return

        Dim c As cCase = cCase.GetCaseByCaseID(note.ObjectID)
        If c Is Nothing OrElse Not c.CaseData.CanModifyMeta Then
            Return
        End If

        Note.DeleteCaseNote(noteID)
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub AddCaseNote(ByVal caseId As Integer, ByVal text As String)
        If String.IsNullOrEmpty(text) Then
            Return
        End If

        Dim c As cCase = cCase.GetCaseByCaseID(caseId)
        If c Is Nothing OrElse Not c.CaseData.CanModifyMeta Then
            Return
        End If


        Dim loNote As Note = Note.NewCaseNote(caseId)
        loNote.NoteText = text
        loNote.IsHtml = False
        loNote.Save()

    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub CollectFile(ByVal vlFileID As Integer, ByVal force As Boolean)
        Dim oFile As File = File.GetFile(vlFileID)
        If oFile Is Nothing OrElse Not oFile.CanView Then Return

        Dim loProc As New CollectSingleFileForEditing(vlFileID, force)
        loProc = loProc.Collect()
        If Not String.IsNullOrEmpty(loProc.ErrorMessage) Then
            Throw New Exception(loProc.ErrorMessage)
        End If
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub CheckoutDocument(ByVal vlID As Integer, ByVal vsComment As String)
        Dim obj As Document = Document.GetDocument(vlID)
        If obj Is Nothing Then Return

        obj = obj.Checkout(vsComment)
        If obj.HasError Then
            Throw New Exception(obj.GetLastError.Description)
        End If
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub CancelCheckout(ByVal vlID As Integer)
        Dim obj As Document = Document.GetDocument(vlID)
        If obj Is Nothing Then Return
        obj.CancelCheckout()

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub UnlockDocument(ByVal vlID As Integer)
        Dim obj As DM_OBJECT = ObjectRepository.GetObject(vlID)
        If obj Is Nothing Then Return

        obj.UnLock()
    End Sub


    <WebMethod(EnableSession:=True)>
    Public Sub LinkObjectToRoutingCase(ByVal vlObjectID As Integer, ByVal vlCaseID As Integer, ByVal vlPackID As Integer)
        Dim o As DM_OBJECT = ObjectRepository.GetObject(vlObjectID)
        If o Is Nothing OrElse Not o.CanViewMeta Then Return

        Dim targetCase As cCase = cCase.GetCaseByCaseID(vlCaseID)

        If targetCase Is Nothing Then Return

        targetCase.AddToPackage(vlPackID, o)


    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub LinkObjectToObjectPackage(ByVal vlObjectID As Integer, ByVal vlToObjectID As Integer, ByVal vlPackID As Integer)
        Dim oTobeAdded As DM_OBJECT = ObjectRepository.GetObject(vlObjectID)
        If oTobeAdded Is Nothing OrElse Not oTobeAdded.CanViewMeta() Then Return

        Dim oTobeAddedTo As DM_OBJECT = ObjectRepository.GetObject(vlToObjectID)
        If oTobeAddedTo Is Nothing Then Return


        oTobeAddedTo.AddToPackage(vlPackID, oTobeAdded)


    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub LinkArchivedCaseToRoutingCase(ByVal vlCaseID As Integer, ByVal vlToCaseID As Integer, ByVal vlPackID As Integer)
        Dim sourceCase As HistoryCase = HistoryCase.GetHistoryCase(vlCaseID)
        If sourceCase Is Nothing OrElse Not sourceCase.CanViewMeta Then Return

        Dim targetcase As cCase = cCase.GetCaseByCaseID(vlToCaseID)
        If targetcase Is Nothing Then Return


        targetcase.AddToPackage(vlPackID, sourceCase)


    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub LinkCaseToRoutingCase(ByVal vlCaseID As Integer, ByVal vlToCaseID As Integer, ByVal vlPackID As Integer)
        Dim sourceCase As cCase = cCase.GetCaseByCaseID(vlCaseID)
        If sourceCase Is Nothing OrElse Not sourceCase.CanView Then Return

        Dim targetCase As cCase = cCase.GetCaseByCaseID(vlToCaseID)
        If targetCase Is Nothing Then Return

        targetCase.AddToPackage(vlPackID, sourceCase)


    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub LinkArchivedCaseToObjectPackage(ByVal vlCaseID As Integer, ByVal vlToObjectID As Integer, ByVal vlPackID As Integer)
        Dim oTobeAddedTo As DM_OBJECT = ObjectRepository.GetObject(vlToObjectID)
        If oTobeAddedTo Is Nothing Then Return

        Dim loCase As HistoryCase = HistoryCase.GetHistoryCase(vlCaseID)
        If loCase Is Nothing OrElse Not loCase.CanViewMeta Then Return


        oTobeAddedTo.AddToPackage(vlPackID, loCase)

    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub LinkCaseToObjectPackage(ByVal vlCaseID As Integer, ByVal vlToObjectID As Integer, ByVal vlPackID As Integer)
        Dim oTobeAddedTo As DM_OBJECT = ObjectRepository.GetObject(vlToObjectID)
        If oTobeAddedTo Is Nothing Then Return

        Dim loCase As cCase = cCase.GetCaseByCaseID(vlCaseID)
        If loCase Is Nothing OrElse Not loCase.CanView Then Return


        oTobeAddedTo.AddToPackage(vlPackID, loCase)

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub ToggleUserProfileParameter(ByVal vsName As String)
        Dim loprofile As UserProfile = GetUserProfile()
        If loprofile.GetParamAsBoolean(vsName) Then
            loprofile.SaveParam(vsName, False)
        Else
            loprofile.SaveParam(vsName, True)
        End If

    End Sub

    Private Function GetUserProfile() As UserProfile
        Return BusinessIdentity.CurrentIdentity().GetUserProfile()
    End Function
    <WebMethod(EnableSession:=True)>
    Public Sub SetUserProfileParameter(ByVal vsName As String, ByVal vsValue As String)
        Try
            Dim loprofile As UserProfile = GetUserProfile()

            loprofile.SaveParam(vsName, vsValue)
        Catch ex As Exception
            Arco.Utils.Logging.LogError("Error in SetUserProfileParameter", ex)
        End Try


    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub EmptyRecycleBin()
        If BusinessIdentity.CurrentIdentity.isAdmin Then
            RecycleBin.EmptyRecycleBin()
        End If
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub EmptyRecycleBinSelection(ByVal vsSelection As String)
        If Not BusinessIdentity.CurrentIdentity.isAdmin Then
            Return
        End If

        Dim objectIds As String = String.Join(",", vsSelection.Split(";"c).Select(Function(x) x.Trim).Where(Function(x) x.Length <> 0))
        RecycleBin.EmptyRecycleBin(objectIds)

    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub RestoreRecycleBinSelection(ByVal vsSelection As String)
        If Not BusinessIdentity.CurrentIdentity.isAdmin Then
            Return
        End If


        For Each lsDoc As String In vsSelection.Split(";"c).Select(Function(x) x.Trim).Where(Function(x) x.Length <> 0)
            Dim llDocID As Integer = CType(lsDoc, Integer)
            Dim o As DM_OBJECT = ObjectRepository.GetObject(llDocID)
            If o Is Nothing Then Continue For
            o.RestoreDeleted()

        Next

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub AddSelectionToCasePackage(ByVal vlCaseID As Integer, ByVal vlPackID As Integer, ByVal viSelectionType As DMSelection.SelectionType)
        Dim lcolSelection As DMSelectionList = Selection.GetSelection(viSelectionType)
        If lcolSelection.Any Then
            Dim targetcase As cCase = cCase.GetCaseByCaseID(vlCaseID)
            If targetcase Is Nothing Then Return

            For Each loSelInfo As DMSelectionList.SelectionInfo In lcolSelection
                targetcase.AddToPackage(vlPackID, loSelInfo.GetBusinessObject)
            Next
        End If
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub AddSelectionToObjectPackage(ByVal vlObjectID As Integer, ByVal vlPackID As Integer, ByVal viSelectionType As DMSelection.SelectionType)
        Dim lcolSelection As DMSelectionList = Selection.GetSelection(viSelectionType)
        If lcolSelection.Any Then
            Dim o As DM_OBJECT = ObjectRepository.GetObject(vlObjectID)
            If o Is Nothing Then Return

            For Each loSelInfo As DMSelectionList.SelectionInfo In lcolSelection
                o.AddToPackage(vlPackID, loSelInfo.GetBusinessObject)
            Next

        End If
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub RemoveObjectFromCasePackage(ByVal vlCaseID As Integer, ByVal vlPackID As Integer, ByVal vlItem As Integer)
        Dim targetcase As cCase = cCase.GetCaseByCaseID(vlCaseID)
        If targetcase Is Nothing Then Return
        targetcase.RemoveFromPackage(vlPackID, vlItem)
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub RemoveObjectFromObjectPackage(ByVal vlObjectID As Integer, ByVal vlPackID As Integer, ByVal vlItem As Integer)
        Dim o As DM_OBJECT = ObjectRepository.GetObject(vlObjectID)
        If o Is Nothing Then Return
        o.RemoveFromPackage(vlPackID, vlItem)

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Sub RemoveObjectFromDossier(ByVal dossierId As Integer, ByVal objectDIN As Integer)
        Dim doss As Dossier = Dossier.GetDossier(dossierId)
        If doss Is Nothing Then Return

        Dim o As DM_OBJECT = ObjectRepository.GetObjectByDIN(objectDIN)
        If o Is Nothing Then Return

        For Each p As PackageList.PackageInfo In doss.Packages
            Select Case p.Type
                Case Package.ePackType.Docroom
                    doss.RemoveFromPackage(p.ID, o)
            End Select
        Next

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Sub AddToSelection(ByVal vlObjectID As Integer, ByVal viSelectionType As DMSelection.SelectionType)

        Dim loTestObj As DM_OBJECT = Nothing
        Dim lsObjType As String
        Select Case viSelectionType
            Case DMSelection.SelectionType.CurrentCases, DMSelection.SelectionType.CurrentCasesByCaseID
                lsObjType = "Workflow"
            Case DMSelection.SelectionType.CurrentMails
                lsObjType = "Mail"
            Case Else
                loTestObj = ObjectRepository.GetObject(vlObjectID)
                If loTestObj Is Nothing Then Return
                lsObjType = loTestObj.Object_Type
        End Select


        Select Case viSelectionType
            Case DMSelection.SelectionType.OIP
                Selection.SetOIP(loTestObj)
            Case DMSelection.SelectionType.Recent
                Selection.AddToRecentDocuments(loTestObj)
            Case Else
                Selection.AddToSelection(viSelectionType, vlObjectID, lsObjType)
        End Select

    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub AddListToSelection(ByVal vsObjectList As String, ByVal viSelectionType As DMSelection.SelectionType)
        If viSelectionType = DMSelection.SelectionType.Undefined Then Return

        'current we call by using save complete, so remove first
        Select Case viSelectionType
            Case DMSelection.SelectionType.Current
                Selection.ClearCurrentSelection()
            Case DMSelection.SelectionType.UserCurrent
                Selection.ClearClipboard()
            Case DMSelection.SelectionType.CurrentCases
                Selection.ClearCurrentCasesSelection()
            Case DMSelection.SelectionType.CurrentCasesByCaseID
                Selection.ClearCurrentCasesByCaseIDSelection()
            Case DMSelection.SelectionType.CurrentMails
                Selection.ClearCurrentMailsSelection()
        End Select

        If Not String.IsNullOrEmpty(vsObjectList) Then
            For Each lsDoc As String In vsObjectList.Split(";"c).Select(Function(x) x.Trim).Where(Function(x) x.Length <> 0)
                Dim llDocID As Integer
                If Integer.TryParse(lsDoc, llDocID) Then
                    If llDocID > 0 Then
                        AddToSelection(llDocID, viSelectionType)
                    End If
                End If
            Next
        End If

    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub RemoveListFromSelection(ByVal vsObjectList As String, ByVal viSelectionType As DMSelection.SelectionType)
        If viSelectionType <> DMSelection.SelectionType.Undefined AndAlso Not String.IsNullOrEmpty(vsObjectList) Then
            For Each lsDoc As String In vsObjectList.Split(";"c).Select(Function(x) x.Trim).Where(Function(x) x.Length <> 0)
                Dim llDocID As Integer
                If Integer.TryParse(lsDoc, llDocID) Then
                    If llDocID > 0 Then
                        RemoveFromSelection(llDocID, viSelectionType)
                    End If
                End If
            Next
        End If
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub RemoveFromSelection(ByVal vlObjectID As Integer, ByVal viSelectionType As DMSelection.SelectionType)

        Selection.RemoveSelection(viSelectionType, vlObjectID)

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub ClearSelection(ByVal viSelectionType As DMSelection.SelectionType)
        Selection.RemoveSelection(viSelectionType, 0)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub ToggleReadOnCase(ByVal techid As Integer)
        Dim loCase As cCase = cCase.GetCase(techid)
        If loCase Is Nothing Then Return

        If loCase.CurrentUserHasWork(True) Then
            loCase.ToggleRead()
        End If

    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub ToggleSuspendCase(ByVal vlTechID As Integer)
        Dim loCase As cCase = cCase.GetCase(vlTechID)
        If loCase Is Nothing Then Return

        If loCase.CurrentUserHasWork OrElse loCase.CanAdminCaseStatus Then
            loCase.ToggleSuspended()
        End If

    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub UnlockCase(ByVal vlTechID As Integer)
        Dim loCase As cCase = cCase.GetCase(vlTechID)
        If loCase Is Nothing Then Return

        loCase.UnLockCase()

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub RemoveLink(ByVal vlSourceID As Integer, ByVal vlTargetID As Integer, ByVal vlRelationType As Integer)
        Dim loSource As DM_OBJECT = ObjectRepository.GetObject(vlSourceID)
        If loSource Is Nothing OrElse Not loSource.CanModifyMeta() Then Return

        Dim loTargetObj As DM_OBJECT = ObjectRepository.GetObject(vlTargetID)
        If loTargetObj Is Nothing OrElse Not loTargetObj.CanModifyMeta() Then Return

        loSource.RemoveLink(loTargetObj, vlRelationType)
    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub ToggleRQManualStatus(ByVal vlRQID As Integer, ByVal vbMode As Boolean)
        Dim loRQ As EditChecks.RaisedQuery = EditChecks.RaisedQuery.GetRaisedQuery(vlRQID)
        If loRQ Is Nothing Then Return


        If vbMode Then
            loRQ.ManualValidationStatus = 1
        Else
            loRQ.ManualValidationStatus = 0
        End If
        loRQ.Save()

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Function PromoteMailFileToDoc(ByVal vlFileID As Integer, ByVal vlFolderID As Integer, ByVal vlcatID As Integer) As Integer
        Dim loMailFile As MailFile = MailFile.GetFile(vlFileID, 0)
        If loMailFile Is Nothing OrElse Not loMailFile.CanView() Then Throw New ApplicationException("File was not found")

        Dim loDoc As Document = loMailFile.CreateDocument(vlFolderID, vlcatID, True)
        If loDoc Is Nothing Then Throw New ApplicationException("The docment could not be created")

        If Not loDoc.HasError Then
            Return loDoc.ID
        Else
            Throw New ApplicationException(loDoc.GetLastError.Description)
        End If
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function PromoteMailToDoc(ByVal vlMailID As Integer, ByVal vlFolderID As Integer, ByVal vlcatID As Integer) As Integer
        Dim loMail As DMMail = DMMail.GetMail(vlMailID)
        If loMail Is Nothing OrElse Not loMail.CanView() Then Throw New ApplicationException("Mail was not found")

        Dim loDoc As Document = loMail.CreateDocument(vlFolderID, vlcatID, True)
        If loDoc Is Nothing Then Throw New ApplicationException("The docment could not be created")

        If Not loDoc.HasError Then
            Return loDoc.ID
        Else
            Throw New ApplicationException(loDoc.GetLastError.Description)
        End If
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function GetMyDocumentsFolderID() As Integer
        Return Folder.GetMyDocumentsFolderID()
    End Function



    <WebMethod(EnableSession:=True)>
    Public Sub AddToDataSet(ByVal dataSetId As Integer, ByVal itemId As Integer, ByVal itemType As Integer)
        Dim dataSet As DMObjectDataset = DMObjectDataset.GetDataset(dataSetId)
        If dataSet Is Nothing OrElse Not dataSet.CanEdit() Then
            Return
        End If

        dataSet.AddContent(CType(itemType, DMObjectDataset.ContentType), itemId)
        dataSet = dataSet.Save()


    End Sub
    <WebMethod(EnableSession:=True)>
    Public Sub RemoveFromDataSet(ByVal dataSetId As Integer, ByVal itemId As Integer, ByVal itemType As Integer)
        Dim dataSet As DMObjectDataset = DMObjectDataset.GetDataset(dataSetId)
        If dataSet Is Nothing OrElse Not dataSet.CanEdit() Then
            Return
        End If
        dataSet.RemoveContent(CType(itemType, DMObjectDataset.ContentType), itemId)
        dataSet = dataSet.Save()


    End Sub

#Region " list actions (without feedback) "
    <WebMethod(EnableSession:=True)>
    Public Sub DoMailboxAction(ByVal actionType As MailBoxAction.eActionType, ByVal selectionType As DMSelection.SelectionType, ByVal actionId As Integer, ByVal boxId As MailBoxItem.MailBox)
        Dim loAction As New MailBoxAction(actionType, selectionType, actionId, boxId)
        loAction.Execute()
    End Sub
#End Region

End Class
