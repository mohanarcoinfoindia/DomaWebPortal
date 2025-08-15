
Partial Class UserControls_DM_PageController
    Inherits BaseUserControl


    Public Enum ePageLocation
        Undefined = 0
        MainPage = 1
        ListPage = 2
        TreePage = 3
        DetailPage = 4
        DetailSubPage = 5
        Popup = 6
        PreviewPage = 7
        DetailWindow = 8
    End Enum
    Private mePageLocation As ePageLocation = ePageLocation.Undefined
    Public Property PageLocation() As ePageLocation
        Get
            Return mePageLocation
        End Get
        Set(ByVal value As ePageLocation)
            mePageLocation = value
        End Set
    End Property

    Public Property ReloadFunction() As String
    Public Property OpenDetailWindowsModal As Boolean
    Public Property ShowPreview() As Boolean = UserProfile.ShowPreview

    Public Property PackageID() As String
        Get
            Return PACK_ID.Value
        End Get
        Set(ByVal value As String)
            PACK_ID.Value = value
        End Set
    End Property

    Public Function GetParentId(ByVal returnRootIdAsDefault As Boolean) As Int32
        If String.IsNullOrEmpty(DM_PARENT_ID.Value) Then
            Return GetObjectIDFromRequest("DM_PARENT_ID")
        End If

        Dim parentId As Int32 = CType(DM_PARENT_ID.Value, Integer)
        If parentId <> 0 Then
            Return parentId
        End If
        Dim result As Integer
        If returnRootIdAsDefault AndAlso Integer.TryParse(DM_ROOT_ID.Value, result) Then
            Return result
        End If
        Return 0
    End Function
    Public Property ParentID() As Int32
        Get
            Return GetParentId(True)
        End Get
        Set(ByVal value As Int32)
            DM_PARENT_ID.Value = value
        End Set
    End Property
    Public ReadOnly Property RootID() As Int32
        Get
            Return CType(DM_ROOT_ID.Value, Integer)
        End Get
    End Property
    Private Function GetFromRequest(ByVal s As String) As String

        If Not Page.IsPostBack Then
            Return Request.QueryString(s)
        Else
            Return Request.Form(s)
        End If

    End Function
    Private Function GetObjectIDFromRequest(ByVal s As String) As Int32
        Dim lsTemp As String = GetFromRequest(s)

        Dim ID As Int32 = 0
        If Not String.IsNullOrEmpty(lsTemp) Then
            If Not Int32.TryParse(lsTemp, ID) Then
                Dim f As Arco.Doma.Library.Folder = Arco.Doma.Library.Folder.GetFolder(lsTemp)
                If f Is Nothing Then
                    Dim d As Arco.Doma.Library.Dossier = Arco.Doma.Library.Dossier.GetDossier(lsTemp)
                    If d IsNot Nothing Then
                        ID = d.ID
                    Else
                        Throw New ArgumentException("Folder " & lsTemp & " was not found")
                    End If
                Else
                    ID = f.ID
                End If

            End If
        End If
        Return ID
    End Function
    Public Property ScreenMode() As String
        Get
            Return txtScreenmode.Value
        End Get
        Set(ByVal value As String)
            txtScreenmode.Value = value
        End Set
    End Property

    Protected Overrides Sub OnInit(ByVal e As EventArgs)
        MyBase.OnInit(e)


        If Not Page.IsPostBack Then
            DM_PARENT_ID.Value = GetObjectIDFromRequest("DM_PARENT_ID").ToString

            If TypeOf System.Threading.Thread.CurrentPrincipal Is Arco.Security.BusinessPrincipal Then
                DM_ROOT_ID.Value = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.RootFolder.ToString
            End If

            'If DM_PARENT_ID.Value = "0" Then
            '    DM_PARENT_ID.Value = DM_ROOT_ID.Value
            'End If

            Dim rootIdFromRequest As Int32 = GetObjectIDFromRequest("DM_ROOT_ID")
            If rootIdFromRequest <> 0 Then
                DM_ROOT_ID.Value = rootIdFromRequest.ToString

            End If
            PACK_ID.Value = GetFromRequest("PACK_ID")
            If PACK_ID.Value = "0" Then
                PACK_ID.Value = ""
            End If
            If Not Request.QueryString("screenmode") Is Nothing Then
                txtScreenmode.Value = Request.QueryString("screenmode")
            End If
        End If

    End Sub

    Private Shared _versionString As String

    Protected Overrides Sub OnPreRender(e As EventArgs)
        MyBase.OnPreRender(e)

        If _versionString Is Nothing Then
            Dim a As Reflection.Assembly = Reflection.Assembly.GetAssembly(GetType(Arco.Doma.Library.Document))
            Dim fv As System.Diagnostics.FileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(a.Location)
            _versionString = "?v=" & fv.FileVersion.Replace(".", "")
        End If

        For Each c As Control In Page.Header.Controls
            Dim link As HtmlLink = TryCast(c, HtmlLink)
            If link IsNot Nothing Then

                If link.Href.IndexOf("App_Themes/", StringComparison.InvariantCultureIgnoreCase) >= 0 AndAlso
                    link.Href.EndsWith(".css", StringComparison.InvariantCultureIgnoreCase) Then
                    link.Href &= _versionString
                    link.EnableViewState = False
                End If
            End If
        Next
    End Sub

    Private Function GetScriptReference(ByVal path As String) As ScriptReference
        Return New ScriptReference(path)
    End Function
    Private Function GetServiceReference(ByVal path As String) As ServiceReference
        Return New ServiceReference(path)
    End Function

    Private Sub AddCompositeScriptIfExists(ByVal relativePath As String)

        Dim path As String = Server.MapPath(relativePath)
        If Arco.IO.File.Exists(path) Then
            sc1.CompositeScript.Scripts.Add(GetScriptReference(relativePath))
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim addServiceReferences As Boolean

        AddCompositeScriptIfExists("~/Custom/CustomFunctions.js")

        sc1.CompositeScript.Scripts.Add(GetScriptReference("~/JS/PageController.js"))
        Select Case PageLocation
            Case ePageLocation.MainPage
                sc1.CompositeScript.Scripts.Add(GetScriptReference("~/JS/mainscreen.js"))
                addServiceReferences = False
            Case ePageLocation.TreePage
                sc1.CompositeScript.Scripts.Add(GetScriptReference("~/JS/Tree.js"))
                addServiceReferences = True
            Case ePageLocation.Popup
                addServiceReferences = False
                sc1.CompositeScript.Scripts.Add(GetScriptReference("~/JS/ToolWindow.js"))
            Case ePageLocation.DetailWindow
                addServiceReferences = True

            Case ePageLocation.ListPage
                sc1.CompositeScript.Scripts.Add(GetScriptReference("~/JS/ListControl.js"))
                sc1.CompositeScript.Scripts.Add(GetScriptReference("~/JS/DocumentList.js"))
                addServiceReferences = True
            Case ePageLocation.DetailPage
                sc1.CompositeScript.Scripts.Add(GetScriptReference("~/JS/ListControl.js"))
                sc1.Services.Add(GetServiceReference("~/ScriptServices/DocroomDetailService.asmx"))
                addServiceReferences = True
            Case ePageLocation.DetailSubPage, ePageLocation.PreviewPage
                sc1.CompositeScript.Scripts.Add(GetScriptReference("~/JS/ListControl.js"))

                addServiceReferences = True
        End Select

        If addServiceReferences Then
            sc1.Services.Add(GetServiceReference("~/ScriptServices/ContextMenus.asmx"))
            sc1.Services.Add(GetServiceReference("~/ScriptServices/DocroomListFunctions.asmx"))
        End If

        sc1.CompositeScript.Scripts.Add(GetScriptReference("~/Resources/" & GetLangCode & "/Messages.js"))

        AddCompositeScriptIfExists("~/Resources/" & GetLangCode & "/Custom.js")

        Dim sb As New StringBuilder(128)
        sb.Append("var c")
        sb.Append(ClientID)
        sb.Append(" = new DocroomPageController('")
        sb.Append(ClientID)
        sb.Append("',")
        sb.Append(EncodingUtils.EncodeJsBool(ShowPreview))
        sb.Append(",'")
        sb.Append(EncodingUtils.EncodeJsString(SiteManagement.SitesManager.CurrentSite, False))
        sb.Append("','")
        sb.Append(mnuPageContext.ClientID)
        sb.Append("','")
        sb.Append(ReloadFunction)
        sb.Append("',")
        sb.Append(Convert.ToInt32(mePageLocation))
        sb.Append(",")
        sb.Append(EncodingUtils.EncodeJsBool((Request.QueryString("modal") = "Y")))
        sb.Append(",")
        sb.Append(EncodingUtils.EncodeJsBool((Settings.GetValue("Interface", "ShowTabsInDetailPreview", True))))
        sb.Append(",'")
        sb.Append(ajxMan.ClientID)
        sb.Append("',")
        sb.Append(EncodingUtils.EncodeJsBool(UserProfile.ShowSuccessMessages))
        sb.Append(",")
        sb.Append(EncodingUtils.EncodeJsBool(UserProfile.OpenDetailWindowMaximized))
        sb.Append(",'")
        sb.Append(Page.Theme)
        sb.Append("','")
        sb.Append(notifWnd.ClientID)
        sb.Append("',")
        sb.Append(EncodingUtils.EncodeJsBool(OpenDetailWindowsModal))
        sb.AppendLine(");")

        Dim jsVarName As String = "c" & ClientID
        sb.Append("function PageController(){")
        sb.Append("return PC();}")
        sb.Append("function PC(){")
        sb.Append("return ")
        sb.Append(jsVarName)
        sb.AppendLine(";}")

        sb.Append("function CleanUpPageController(){")
        sb.Append(jsVarName)
        sb.AppendLine(" = null;}")

        sb.Append("LoadingImage = '")
        sb.Append(Telerik.Web.UI.RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif"))
        sb.AppendLine("';")


        ScriptManager.RegisterStartupScript(Page, Page.GetType, "initpagecontroller", sb.ToString, True)

    End Sub
End Class
