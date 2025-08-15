Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Mail
Imports Telerik.Web.UI

<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class MailContactLookup
    Inherits System.Web.Services.WebService

    Private Const MaxItems As Int32 = 20

    <WebMethod(EnableSession:=True)>
    Public Function GetContacts(ByVal context As RadAutoCompleteContext) As AutoCompleteBoxData
        If Not Configuration.GetSettings().GetValue("MailClient", "Enabled", True) Then
            Return Nothing
        End If

        Return ToAutoCompleteData(LoadContacts(context.Text))

    End Function
    <WebMethod(EnableSession:=True)>
    Public Function GetContactsWithExternal(ByVal context As RadAutoCompleteContext) As AutoCompleteBoxData
        If Not Configuration.GetSettings().GetValue("MailClient", "Enabled", True) Then
            Return Nothing
        End If

        Return ToAutoCompleteData(LoadContactsWithExternal(context.Text))

    End Function
    Private Function ToAutoCompleteData(ByVal contacts As List(Of MailContact)) As AutoCompleteBoxData
        Dim res As New AutoCompleteBoxData
        If contacts.Count > MaxItems Then
            res.Items = contacts.Take(MaxItems).Select(Function(x) ToAutoCompleteData(x)).ToArray
            res.EndOfItems = False
        Else
            res.Items = contacts.Select(Function(x) ToAutoCompleteData(x)).ToArray
            res.EndOfItems = True
        End If

        Return res

    End Function
    Private Function ToAutoCompleteData(ByVal contact As MailContact) As AutoCompleteBoxItemData
        Dim itm As New AutoCompleteBoxItemData
        If Not String.IsNullOrEmpty(contact.LoginName) Then
            itm.Value = contact.LoginName
        Else
            itm.Value = contact.Email
        End If
        itm.Text = contact.DisplayName

        Return itm
    End Function

    Private Sub AddUsers(ByVal voItems As List(Of MailContact), ByVal vsFilter As String)
        'get userlist from rtusers
        Dim loUserCrit As New USERList.Criteria
        loUserCrit.FILTER = vsFilter
        loUserCrit.STATUS = ACL.User.UserStatus.Valid
        For Each loUser As USERList.USERSInfo In USERList.GetUSERSList(loUserCrit)
            Dim loNewitem As MailContact = New MailContact(loUser.USER_ACCOUNT, loUser.USER_MAIL, loUser.USER_DISPLAY_NAME)
            If Not voItems.Contains(loNewitem) Then
                voItems.Add(loNewitem)
                If voItems.Count > MaxItems Then Exit For
            End If

        Next
    End Sub

    Public Function LoadContacts(ByVal vsFilter As String) As List(Of MailContact)
        Dim oItems As New List(Of MailContact)

        AddUsers(oItems, vsFilter)

        Dim loRoleCrit As New ROLEList.Criteria
        loRoleCrit.FILTER = vsFilter
        For Each loRole As ROLEList.ROLEInfo In ROLEList.GetROLEList(loRoleCrit)
            Dim loNewitem As New MailContact("(Role) " & loRole.Name, "", loRole.Name)
            If Not oItems.Contains(loNewitem) Then
                oItems.Add(loNewitem)
                If oItems.Count > MaxItems Then Exit For
            End If
        Next

        oItems = oItems.OrderBy(Function(x) x.DisplayName).ToList

        Return oItems
    End Function

    Public Function LoadContactsWithExternal(ByVal vsFilter As String) As List(Of MailContact)
        Dim oItems As New List(Of MailContact)
        'get 'personal" userlist from dm_mailinglist

        Dim loCrit As New DM_MailingAllList.Criteria()
        loCrit.Filter = vsFilter
        loCrit.MailSynch = 0
        loCrit.MailLoginName = Arco.Security.BusinessIdentity.CurrentIdentity.Name

        For Each loMailItem As DM_MailingAllList.MailingAllInfo In DM_MailingAllList.GetMailingAllList(loCrit)
            If Not String.IsNullOrEmpty(loMailItem.MailEmail) Then
                Dim loNewitem As MailContact = New MailContact(loMailItem.MailEmail, loMailItem.MailEmail, loMailItem.MailName)
                If Not oItems.Contains(loNewitem) Then
                    oItems.Add(loNewitem)
                    If oItems.Count > MaxItems Then Exit For
                End If
            End If
        Next

        AddUsers(oItems, vsFilter)

        oItems = oItems.OrderBy(Function(x) x.DisplayName).ToList

        Return oItems
    End Function

End Class

<Serializable()>
Public Class MailContact
    Public Property LoginName As String
    Public Property DisplayName As String
    Public Property Email As String
    Public Sub New(ByVal login As String, ByVal mail As String, ByVal display As String)
        LoginName = login
        Email = mail
        If Not String.IsNullOrEmpty(display) Then
            DisplayName = display
        ElseIf Not String.IsNullOrEmpty(LoginName) Then
            DisplayName = LoginName
        Else
            DisplayName = Email
        End If

    End Sub
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim mc As MailContact = TryCast(obj, MailContact)
        If mc IsNot Nothing Then
            Return mc.LoginName.Equals(Me.LoginName, StringComparison.CurrentCultureIgnoreCase)
        End If
        Return False

    End Function
End Class

