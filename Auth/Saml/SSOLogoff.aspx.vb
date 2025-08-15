
Imports System.Security.Cryptography.X509Certificates
Imports Arco.Saml

Partial Class Auth_Saml_SSOLogoff
    Inherits BasePage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim spCertificate As X509Certificate2 = Certificates.LoadCertificate(Settings.GetValue("Saml", "Certificate", "", True), Settings.GetSecret("Saml", "CertificatePassword", True))

        Dim mapperClass As String = Settings.GetValue("Saml", "ClaimsMapper", "Arco.Saml.AdfsClaimsMapper, Arco.Saml", True)
        Dim claimsMapper As IClaimsMapper = Arco.ApplicationServer.Library.Shared.PluginManager.CreateInstance(Of IClaimsMapper)(mapperClass)

        SingleSignOn.RequestLogoutAtIDP(Me, Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity(), claimsMapper, Settings.GetValue("Saml", "IDPMetadata", "", True), Settings.GetValue("Saml", "SPMetadata", "", True), spCertificate)

    End Sub
End Class
