<%@ WebHandler Language="VB" Class="UserInfo" %>


Imports System.Web
Imports Arco.Doma.OpenAuth2
Imports DotNetOpenAuth.OAuth2
Imports Arco.Doma.OpenAuth2.Factories

Public Class UserInfo : Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Using logger As New Arco.Utils.BlockLogger("OAuth.UserInfo.ashx:ProcessRequest")
            Try
                Dim authServer As New Arco.Doma.OpenAuth2.AuthorizationServer()
                Dim resourceserver As New Arco.Doma.OpenAuth2.ResourceServer(authServer.GetTokenAnalyzer())

                'todo : check other scopes also address,phone: atm the basic scopes are enough to give all info

                Dim token As AccessToken = resourceserver.GetAccessToken(New HttpRequestWrapper(context.Request), "openid", "profile", "email")

                context.Response.ContentType = "application/json"
                context.Response.ContentEncoding = Encoding.UTF8
                context.Response.Write(authServer.GetUserInfoFromToken(token).ToJson())
            Catch ex As Exception
                Dim remoteUser As Arco.Doma.Library.Security.EndUserInfo = Arco.Doma.WebControls.UserInfoLoader.GetEndUserInfo(context)
                Arco.Utils.Logging.LogError("Error in Auth/OAuth/UserInfo from " & remoteUser.RemoteAddress & " (" & remoteUser.UserAgent & ")", ex)
                Throw
            End Try
        End Using

    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return True
        End Get
    End Property

End Class