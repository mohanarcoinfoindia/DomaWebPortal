Imports System.Security.Cryptography.X509Certificates
Imports ComponentSpace.SAML2.Protocols

Partial Class Auth_Saml_SSOService
    Inherits BaseAnonymousPage
    ' The session key for saving the SSO state during a local login.
    Private Const ssoSessionKey As String = "sso"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim target As String = QueryStringParser.GetString("target")
        Dim identity As Arco.Doma.Library.Security.BusinessIdentity = Arco.Doma.Library.Security.BusinessIdentity.GetIdentity("SEC_" & Session.SessionID)


        If String.IsNullOrEmpty(target) Then
            'the request was sp initiated
            Dim ssoState As Arco.Saml.SSOState = Session(ssoSessionKey)

            If ssoState Is Nothing Then

                Dim relayState As String = Nothing
                Dim spCertificate As X509Certificate2 = Nothing 'todo!!
                Dim authnRequest As AuthnRequest = Arco.Saml.SingleSignOn.ReceiveAuthnRequest(Me, spCertificate, relayState)

                If authnRequest Is Nothing Then
                    Throw New InvalidOperationException("Not an authorization request")
                End If

                ' Process the authentication request.
                Dim forceAuthn As Boolean = authnRequest.ForceAuthn
                Dim allowCreate As Boolean = False

                If Not authnRequest.NameIDPolicy Is Nothing Then
                    allowCreate = authnRequest.NameIDPolicy.AllowCreate
                End If

                ssoState = New Arco.Saml.SSOState()
                ssoState.RequestID = authnRequest.ID
                'ssoState.authnRequest = authnRequest
                ssoState.RelayState = relayState
                ' ssoState.idpProtocolBinding = SAMLIdentifiers.BindingURIs.URIToBinding(authnRequest.ProtocolBinding)
                ssoState.AssertionConsumerServiceURL = authnRequest.AssertionConsumerServiceURL

                If IsLoginRequired(identity, forceAuthn, allowCreate) Then
                   
                    ' Save the SSO state.
                    Session(ssoSessionKey) = ssoState
                    Response.Redirect("~/Auth/Login.aspx?redirect_uri=~/Auth/Saml/SSOService.aspx")
                    Exit Sub
                End If
        
            End If

            'return result
            Dim samlResponse As SAMLResponse = Arco.Saml.SingleSignOn.CreateAuthnResponse(Me, identity)
            Dim idpCertificate As X509Certificate2 = Nothing 'todo!!


            Arco.Saml.SingleSignOn.SendAuthnResponse(Me, samlResponse, ssoState, idpCertificate)
            ' Clear the SSO state.
            Session(ssoSessionKey) = Nothing
        Else
            'the request was idp initiated
            Dim samlResponse As SAMLResponse = Arco.Saml.SingleSignOn.CreateAuthnResponse(Me, identity)
            Dim idpCertificate As X509Certificate2 = Nothing 'todo!!
            Dim ssoState As Arco.Saml.SSOState = New Arco.Saml.SSOState
            ssoState.RelayState = target
            ssoState.AssertionConsumerServiceURL = target 'todo : get url from target
            Arco.Saml.SingleSignOn.SendAuthnResponse(Me, samlResponse, ssoState, idpCertificate)
        End If
    End Sub

    ' Indicate whether a local login is required.
    Private Shared Function IsLoginRequired(ByVal identity As Arco.Doma.Library.Security.BusinessIdentity, ByVal forceAuthn As Boolean, ByVal allowCreate As Boolean) As Boolean

        If forceAuthn Then
            Return True
        Else
            If (identity Is Nothing OrElse Not identity.IsAuthenticated) AndAlso allowCreate Then
                Return True
            Else
                Return False
            End If
        End If

    End Function

End Class
