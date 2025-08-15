Imports Arco.Doma.Library
Imports Arco.Doma.Library.Security
Imports Arco.Doma.Library.Security.UserConsent

Partial Class Auth_Consent
    Inherits BaseAfterLoginPage

    Private _consentForm As UserConsentForm
    Private _consentService As IUserConsentService
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load


        SetLabels()

        SetReturnTo()

        _consentService = New UserConsentServiceFactory().GetUserContentService(Settings)
        _consentForm = _consentService.GetConsentForm()

        'this shouldn't happen but check anyways
        If _consentForm Is Nothing Then
            ContinueLogin(txtReturnTo.Value, False)
            Exit Sub
        End If


        'set the content
        consentContent.Text = _consentForm.Content

        lnkConsent.Focus()
    End Sub

    Private Sub SetReturnTo()
        If Not Page.IsPostBack Then

            txtReturnTo.Value = QueryStringParser.GetUrl("redirect_uri")
        End If
    End Sub
    Private Sub SetLabels()

        lnkConsent.Text = "Accept"
        lnkCancel.Text = GetLabel("cancel")
    End Sub

    Protected Sub lnkConsent_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkConsent.Click
        If _consentForm Is Nothing Then
            Exit Sub
        End If

        GiveConsent(True)
    End Sub

    Protected Sub lnkCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkCancel.Click
        If _consentForm Is Nothing Then
            Exit Sub
        End If

        If Not _consentForm.Required OrElse Not Security.BusinessIdentity.CurrentIdentity.MustGiveConsent Then
            GiveConsent(False)
        Else
            Response.Redirect("Logoff.aspx")
        End If


    End Sub

    Private Sub GiveConsent(ByVal permanent As Boolean)
        If permanent Then
            'change flag

            _consentService.GiveConsent(_consentForm, Request.Form)
        End If

        'reset the flag on the session
        Dim ident As BusinessIdentity = BusinessIdentity.CurrentIdentity
        If ident.MustGiveConsent Then
            ident.MustGiveConsent = False
            ident.Save()
        End If

        ContinueLogin(txtReturnTo.Value, False)
    End Sub

End Class
