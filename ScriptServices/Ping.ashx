<%@ WebHandler Language="VB" Class="Ping" %>

Imports System
Imports System.Web

Public Class Ping : Implements IHttpHandler
    
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        
        If Not Arco.ApplicationServer.Library.ApplicationServerClient.Server.Ping() Then
            context.Response.StatusCode = 500
            context.Response.Status = "The application server is unavailable"
            Exit Sub
        End If
        
        
        context.Response.ContentType = "text/plain"
        context.Response.Write("Ok")
    End Sub
 
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return true
        End Get
    End Property

End Class