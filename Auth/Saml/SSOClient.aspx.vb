Imports System.Security.Cryptography.X509Certificates
Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Saml

Partial Class Auth_Saml_SSOClient
    Inherits BaseAnonymousPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim tenantName As String = GetTenantFromUrl.Name

        Dim IDPMetaFile As String = Settings.GetValue("Saml", "IDPMetadata", "", tenantName)
        Dim SPMetaFile As String = Settings.GetValue("Saml", "SPMetadata", "", tenantName)

        'we use the same cert for signing the request and decrypting the response
        Dim spCertificateFile As String = Settings.GetValue("Saml", "Certificate", "", tenantName)
        Dim spCertificateFilePassword As String = Settings.GetSecret("Saml", "CertificatePassword", tenantName)
        Dim spCertificate As X509Certificate2 = Certificates.LoadCertificate(spCertificateFile, spCertificateFilePassword)
        Dim persistUsers As Boolean = Settings.GetValue("Saml", "PersistUsers", True, tenantName)

        If spCertificate Is Nothing Then
            Response.Write("unable to load certificate")
            Exit Sub
        End If
        Dim action As String = QueryStringParser.GetCode("action")

        If action = "login" Then

            Response.Write("<script type='text/javascript'>function pageInIframe() { return (window.location !== window.parent.location);}	function checkIframe() { if (pageInIframe()) window.parent.location = '../../DefaultLib.aspx';  } checkIframe(); </script>")

            'send login message to IDP         
            SingleSignOn.RequestLoginAtIDP(Me, IDPMetaFile, SPMetaFile, spCertificate, QueryStringParser.GetUrl("redirect_uri"))
        ElseIf action = "logout" Then
            Response.Redirect("SSOLogoff.aspx")
            Exit Sub
        ElseIf action = "metadata" Then
            Dim callbackUrl As String = GetClientUrl() & "Auth/Saml/SSOClient.aspx"
            Dim signOutUrl As String = GetClientUrl() & "Auth/Saml/LogoutService.aspx"

            Dim meta As ServiceProviderMetaData = ServiceProviderMetaData.CreateServiceProviderMetaData(Settings.GetValue("Saml", "EntityId", "", tenantName), callbackUrl, signOutUrl, spCertificate, spCertificate, persistUsers)

            Dim tempFile As String = Settings.GetTempPath() & Arco.IO.File.GetUniqueFileName("xml")

            meta.Save(tempFile)

            Response.Redirect("~/Tools/StreamFile.aspx?file=" + StreamFileEncryptionService.Encrypt(tempFile) + "&attach=Y")

        Else
            'callback from IDP
            Dim mapperClass As String = Settings.GetValue("Saml", "ClaimsMapper", "Arco.Saml.AdfsClaimsMapper, Arco.Saml", True)
            Dim claimsMapper As IClaimsMapper = Arco.ApplicationServer.Library.Shared.PluginManager.CreateInstance(Of IClaimsMapper)(mapperClass) ' New AdfsClaimsMapper()
            Dim matchInResponseTo As Boolean = Settings.GetValue("Saml", "MatchInResponseTo", True, True)
            SingleSignOn.ProcessIDPResponse(Me, New SSOCallbackHandler(claimsMapper, persistUsers), IDPMetaFile, SPMetaFile, spCertificate, matchInResponseTo)
        End If

    End Sub

    Public Class SSOCallbackHandler
        Implements SingleSignOn.ICallBackHandler

        Private ReadOnly _mapper As IClaimsMapper

        Public Property PersistUser As Boolean

        Public Sub New(ByVal mapper As IClaimsMapper, ByVal persistUser As Boolean)
            _mapper = mapper
            Me.PersistUser = persistUser
        End Sub

        Public Sub LoginIDPUser(Page As Page, User As SingleSignOn.IDPUser, resourceUrl As String) Implements SingleSignOn.ICallBackHandler.LoginIDPUser
            If PersistUser Then
                CreateOrUpdateUser(User)
            End If
            Login(Page, User, resourceUrl)
        End Sub

        Private Sub CreateOrUpdateUser(ByVal User As SingleSignOn.IDPUser)

            Dim loCrit As New USERList.Criteria
            loCrit.LOGINNAME = _mapper.GetUserName(User)
            ' loCrit.OnlyDBUsers = True
            Dim louser As User
            Dim users As USERList = USERList.GetUSERSList(loCrit)
            If users.Any Then
                'user already exists
                louser = ACL.User.GetUser(users(0).USER_LOGIN)
            Else
                louser = ACL.User.NewUser(_mapper.GetUserName(User))
                louser.USER_AUTH_METHOD = ACL.User.AuthenticationMethod.External
            End If
            Dim email As String = _mapper.GetEmail(User)
            If email IsNot Nothing Then
                louser.USER_MAIL = email
            End If
            Dim displ As String = _mapper.GetDisplayName(User)
            If displ IsNot Nothing Then
                louser.USER_DISPLAY_NAME = displ
            End If

            louser = louser.Save()
        End Sub
        Private Sub Login(Page As Page, ByVal User As SingleSignOn.IDPUser, resourceUrl As String)
            Dim lsusername As String = ACL.User.AddDummyDomain(_mapper.GetUserName(User))

            Security.BusinessPrincipal.Login(lsusername, Security.CacheToken.GetCacheToken(Page.Session.SessionID), Arco.Licencing.Modules.WEB, Not PersistUser, UserInfoLoader.GetEndUserInfo(Page))

            Dim identity As Security.BusinessIdentity = Security.BusinessIdentity.CurrentIdentity
            Dim lbLoggedIn As Boolean = False
            If identity.IsAuthenticated Then
                lbLoggedIn = True

                For Each mapperRole As String In _mapper.GetRoles(User)
                    Dim role As Role = Role.GetRole(mapperRole)
                    If role IsNot Nothing Then
                        identity.UseMembershipTable = False
                        'add recursive?
                        identity.AddRole(role.Name, True)
                    Else
                        'role doesn't exist in doma
                    End If
                Next

                _mapper.Map(User, identity)

                identity = identity.Save()

                If Not String.IsNullOrEmpty(resourceUrl) AndAlso Not Arco.Utils.Web.Url.IsLocalUrl(resourceUrl) Then
                    Page.Response.Redirect(resourceUrl, False)
                Else
                    Page.Response.Redirect(identity.GetDefaultWebApplication().Url, False)
                End If

                HttpContext.Current.ApplicationInstance.CompleteRequest()

            Else
                Page.Response.Write("Unable to login")
            End If

        End Sub

    End Class
End Class
