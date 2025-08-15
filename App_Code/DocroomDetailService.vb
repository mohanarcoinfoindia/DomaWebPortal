Imports System.Web.Services
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Routing
' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class DocroomDetailService
    Inherits WebService

    <WebMethod(EnableSession:=True)>
    Public Function GetCasePropertyByName(ByVal techID As Integer, ByVal name As String) As String
        Dim c As cCase = cCase.GetCase(techID)

        If c Is Nothing OrElse Not c.CanView() Then
            Throw New Exception("Case not found")
        End If
        Return c.GetProperty(Of String)(name)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetStepName(ByVal techID As Integer) As String
        Dim c As cCase = cCase.GetCase(techID)

        If c Is Nothing OrElse Not c.CanView() Then
            Throw New Exception("Case not found")
        End If

        Return c.CurrentStep.Name
    End Function
    <WebMethod(EnableSession:=True)>
    Public Function GetStepId(ByVal techID As Integer) As Integer
        Dim c As cCase = cCase.GetCase(techID)

        If c Is Nothing OrElse Not c.CanView() Then
            Throw New Exception("Case not found")
        End If

        Return c.CurrentStep.ID
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetObjectPropertyByName(ByVal objectID As Integer, ByVal name As String) As String
        Dim o As DM_OBJECT = ObjectRepository.GetObject(objectID)
        If o Is Nothing OrElse Not o.CanViewMeta() Then
            Throw New Exception("Object not found")
        End If
        Return o.GetProperty(Of String)(name)
    End Function
End Class
