<%@ WebHandler Language="VB" Class="Token" %>

Imports System.Threading
Imports DotNetOpenAuth.Messaging
Imports DotNetOpenAuth.OAuth2

Public Class Token : Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Using logger As New Arco.Utils.BlockLogger("OAuth.Token.ashx:ProcessRequest")
            Dim authServer As New AuthorizationServer(New Arco.Doma.OpenAuth2.AuthorizationServer())

            Try
                Dim response As OutgoingWebResponse = authServer.HandleTokenRequest(New HttpRequestWrapper(context.Request))
                If response.Status <> Net.HttpStatusCode.OK Then
                    logger.WriteLog("Returning error body: " & response.Body)
                End If
                response.Respond(New HttpContextWrapper(context))
                context.Response.End()
            Catch tex As ThreadAbortException
                'catch it, can't get it to work wihtout the exception :(   
            Catch ex As Exception
                Dim remoteUser As Arco.Doma.Library.Security.EndUserInfo = Arco.Doma.WebControls.UserInfoLoader.GetEndUserInfo(context)
                Arco.Utils.Logging.LogError("Error in Token.ashx:ProcessRequest  from " & remoteUser.RemoteAddress & " (" & remoteUser.UserAgent & ")", ex)
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