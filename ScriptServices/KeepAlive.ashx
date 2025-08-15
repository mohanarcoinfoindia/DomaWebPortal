<%@ WebHandler Language="VB" Class="KeepAlive" %>

Imports System
Imports System.Web

Public Class KeepAlive : Implements IHttpHandler, IRequiresSessionState
    
    Private Shared gif() As Byte =
    {
      &H47, &H49, &H46, &H38, &H39, &H61, &H1, &H0, &H1, &H0, &H91,
      &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0,
      &H0, &H0, &H0, &H21, &HF9, &H4, &H9, &H0, &H0, &H0, &H0,
      &H2C, &H0, &H0, &H0, &H0, &H1, &H0, &H1, &H0, &H0, &H8,
      &H4, &H0, &H1, &H4, &H4, &H0, &H3B, &H0
    }
 
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        'keep the user logged in into Docroom, otherwise the loggoff users batchjob will log him off
        '  Arco.Doma.Library.Licensing.License.UpdateLastAction("Docroom",false)
        
        context.Response.ContentType = "image/gif"
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        context.Response.BinaryWrite(gif)
        context.Response.End()
    End Sub
 
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class