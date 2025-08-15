<%@ WebHandler Language="VB" Class="MailRead" %>

Imports System
Imports System.Web

Public Class MailRead : Implements IHttpHandler
    Public Sub New()
        System.Web.HttpContext.Current.SkipAuthorization = True
    End Sub
    
    
    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
      
        
        context.Response.Cache.SetCacheability(HttpCacheability.Public)
        context.Response.Cache.SetMaxAge(New TimeSpan(1, 0, 0))
        
        Dim rawIfModifiedSince As String = context.Request.Headers.Get("If-Modified-Since")
        If String.IsNullOrEmpty(rawIfModifiedSince) Then
            'only do the first time
            Dim id As String = context.Request.QueryString("ID")
            If Not String.IsNullOrEmpty(id) Then
                Dim loMail As Arco.Doma.Library.Mail.DMMail = Arco.Doma.Library.Mail.DMMail.GetMail(Convert.ToInt32(id))
                loMail.MarkReadForAllUsers()
            End If            
            
            context.Response.Cache.SetLastModified(Arco.SystemClock.Now)
            
            context.Response.ContentType = "image/png"
            context.Response.BinaryWrite(Arco.IO.File.ReadFileToByte(context.Server.MapPath("~/Images/MailTracker.png")))
            context.Response.Flush()
        Else
            'second request for same, just set always to not modified            
            'Dim ifModifiedSince As DateTime = DateTime.Parse(rawIfModifiedSince)
            context.Response.StatusCode = 304
        End If

     
    End Sub
 
    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class