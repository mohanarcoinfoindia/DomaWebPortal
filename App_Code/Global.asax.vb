Imports System
Imports System.Web
Imports System.Web.SessionState
Imports System.Threading
Imports Arco
Imports System.IO
Imports System.Configuration
Imports System.Web.Http
Imports System.Web.Routing
Imports System.Net.Http.Formatting
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Serialization
Imports System.Web.Http.WebHost
Imports Arco.Doma.Library.Helpers
Imports System.Net

Namespace Doma


    Public Class [Global]
        Inherits HttpApplication


        Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            ' Fires when the application is started
            DocroomClientConfiguration.InstallProxy()


            InitializeRoutes()

        End Sub

        Private Shared _routesInit As Boolean

        Private Shared Sub InitializeRoutes()
            If _routesInit Then Exit Sub

            Try

                For Each webApiConfig As IWebApiConfig In GetWebApiConfigs()

                    webApiConfig.Register(GlobalConfiguration.Configuration)

                    'make sure routing is initialized, needed for the registerroutes
                    GlobalConfiguration.Configuration.EnsureInitialized()

                    'register rotues separately to combine with the rest of the website
                    webApiConfig.RegisterRoutes(RouteTable.Routes)
                Next

                _routesInit = True
            Catch ex As Exception
                'Error initalizing routes. This will be retried later
                Arco.Utils.Logging.LogError("Error initializing routes", ex)
            End Try
        End Sub

        Private Shared Function GetWebApiConfigs() As IEnumerable(Of IWebApiConfig)
            Dim settings As ArcoInfo = ArcoInfo.GetParameters()
            Dim configs As New List(Of IWebApiConfig)
            If settings.GetValue("DocumentViewer", "Enabled", False) Then
                Arco.Utils.Logging.Log("Enabling DocumentViewer service")
                configs.Add(New Arco.Doma.DocumentViewer.Service.WebApiConfig())
            End If
            If settings.GetValue("ReportViewer", "Enabled", False) Then
                Arco.Utils.Logging.Log("Enabling ReportViewer service")
                configs.Add(Arco.ApplicationServer.Library.Shared.PluginManager.CreateInstance(Of IWebApiConfig)("Soluzio.Solutions.Reporting.WebApiConfig,Soluzio.Solutions.Reporting"))
            End If

            Return configs
        End Function


        Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
            ' Fires when the session is started                  
            ' ' Arco.Utils.Logging.Debug("Session started " & Session.SessionID)
        End Sub
        Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
            ' Arco.Utils.Logging.Debug("Session ended " & Session.SessionID)
            ' Fires when the session ends           
        End Sub
        Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
            'init the routes if not done yet
            InitializeRoutes()
            '    RewriteFriendlyUrls()
        End Sub

        'Private Sub RewriteFriendlyUrls()
        '    Dim context As HttpContext = HttpContext.Current
        '    Dim requestPath As String = context.Request.FilePath
        '    If requestPath.Equals("/doma/goto/test", StringComparison.CurrentCultureIgnoreCase) Then
        '        context.RewritePath("~/DefaultLib.aspx", "/", "", False)
        '    End If
        'End Sub
        Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
            ' Fires upon attempting to authenticate the use
        End Sub

        Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
            ' Fires when an error occurs
            ErrorHandler.OnError(Me, e)
        End Sub



        Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
            ' Fires when the application ends
            Arco.Utils.Logging.Log("Application_End")
        End Sub


        Private Class RouteHandlerWithSessionState
            Implements IRouteHandler

            Public Function GetHttpHandler(requestContext As RequestContext) As IHttpHandler Implements IRouteHandler.GetHttpHandler
                Return New HttpControllerHandlerWithSessionState(requestContext.RouteData)
            End Function
        End Class

        Private Class HttpControllerHandlerWithSessionState
            Inherits HttpControllerHandler
            Implements IRequiresSessionState

            Public Sub New(ByVal routeData As RouteData)
                MyBase.New(routeData)
            End Sub
        End Class

    End Class


End Namespace
