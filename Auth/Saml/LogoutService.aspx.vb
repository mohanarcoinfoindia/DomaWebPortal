Imports System.Xml
Imports System.Security.Cryptography.X509Certificates
Imports ComponentSpace.SAML2.Protocols
Imports Arco.Saml


''' <summary>
''' Saml Single Logout Page for Doma as Service Provider
''' </summary>
Partial Class Auth_Saml_LogoutService
    Inherits BaseAnonymousPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim relayState As String = Nothing
        Dim isRequest As Boolean

        'certificate = the service provider prvider certificarte        
        Dim spCertificate = ServiceProviderMetaData.LoadServiceProviderMetaData(Settings.GetValue("Saml", "SPMetadata")).GetSigningCertificate()

        Dim message As XmlElement = SingleSignOn.GetLogoutMessage(Me, spCertificate, relayState, isRequest)
        If isRequest Then
            ProcessLogoutRequest(New LogoutRequest(message), relayState)
        Else
            ProcessLogoutResponse(New LogoutResponse(message), relayState)
        End If

    End Sub


    Private Sub ProcessLogoutResponse(ByVal logoutResponse As LogoutResponse, ByVal relayState As String)
        Debug("ProcessLogoutResponse")
        If logoutResponse.IsSuccess Then
            LogoutSession()

            Response.Write("You are now logged-off")
        Else
            SingleSignOn.ProcessSamlErrorResponse(logoutResponse)
        End If
    End Sub

    Private Sub ProcessLogoutRequest(ByVal logoutRequest As LogoutRequest, ByVal relayState As String)
        Debug("ProcessLogoutRequest for user " & logoutRequest.NameID.NameIdentifier)

        ' Send the logout response.     

        Dim idpMeta As IdentityProviderMetaData = IdentityProviderMetaData.LoadIdentityProviderMetaData(Settings.GetValue("Saml", "IDPMetadata", "", True))
        Dim spMeta As ServiceProviderMetaData = ServiceProviderMetaData.LoadServiceProviderMetaData(Settings.GetValue("Saml", "SPMetadata", "", True))

        Dim logoutUrl As String = idpMeta.SingleLogoutService.Location
        '  Dim certificate As X509Certificate2 = spMeta.GetSigningCertificate()
        Dim spCertificateFile As String = Settings.GetValue("Saml", "Certificate", "", True)
        Dim spCertificateFilePassword As String = Settings.GetSecret("Saml", "CertificatePassword", True)
        Dim certificate As X509Certificate2 = Certificates.LoadCertificate(spCertificateFile, spCertificateFilePassword)

        'log out the current session
        LogoutSession()

        'also log out the user in other sessions
        Dim userToLogOff As String = Arco.Doma.Library.ACL.User.AddDummyDomain(logoutRequest.NameID.NameIdentifier)

        Dim sessions As Arco.Doma.Library.Security.SessionList = Arco.Doma.Library.Security.SessionList.GetSessions()
        For Each sess As Arco.Doma.Library.Security.SessionList.SessionInfo In sessions
            'todo : check on module?

            If sess.Login.Equals(userToLogOff, StringComparison.CurrentCultureIgnoreCase) Then
                Debug("Logoff other session " & sess.Token)
                Arco.Doma.Library.Security.BusinessIdentity.LogoffUser(sess.Login, sess.Application, sess.Token)
            Else
                Debug("Not logging off session " & sess.Token & " " & sess.Login & " <> " & userToLogOff)
            End If
        Next

        SingleSignOn.SendLogoutResponse(Me, logoutUrl, certificate, relayState)
    End Sub


    Private Sub LogoutSession()
        Debug("Attempt to logout session")
        Dim authModule As AuthenticationModule = AuthenticationModule.GetModule(Context)
        If authModule.TrySessionLogin(Context) Then
            authModule.Logout(Context)

            Debug("Logout session")
        End If

    End Sub

    Private Shared Sub Debug(ByVal text As String)
        Arco.Utils.Logging.Debug("Saml_LogoutService: " & text)
    End Sub
End Class
