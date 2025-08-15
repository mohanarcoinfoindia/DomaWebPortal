Imports Arco.Doma.Library
Imports Arco.Doma.Library.Customization
Imports Arco.Doma.Library.Licensing

Partial Class DM_About
    Inherits BaseAdminOnlyPage
    Public Shared CurrentTheme As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        CurrentTheme = Me.Theme

        If Not Page.IsPostBack Then
            tabAbout.SelectedIndex = 1
            pgAppServer.Selected = True
        End If

        SetLabels()
        ShowGeneralInfo()
        ShowAppServer()
        ShowWebServer()
        ShowLogfiles()
        ShowAssemblies()
        ShowLicences()
        ShowVersions()
        ShowInstallHistory()
    End Sub

    Private Sub SetLabels()
        Title = "About"
        tabAbout.Tabs(0).Text = GetDecodedLabel("general")

    End Sub
    Private Sub ShowGeneralInfo()
        If Not pgGeneral.Selected Then
            Exit Sub
        End If
        Dim table As StringBuilder = New StringBuilder(256)
        table.Clear()
        table.Append("<table class='DetailTable'>")
        AddGeneralLine("Url", Settings.GetValue("General", "Url"), table)
        AddGeneralLine("Administrator", Settings.GetValue("General", "Administrator"), table)
        AddGeneralLine("Administrator Email(s)", Settings.GetValue("Mail", "AdminEmailAddress"), table)
        AddGeneralLine("Multi-tenant", Settings.GetValue("General", "MultiTenant", False).ToString, table)
        table.Append("</table>")

        lblGeneral.Text = table.ToString
    End Sub

    Private Shared Sub AddGeneralLine(ByVal label As String, ByVal value As String, ByVal table As StringBuilder)
        table.Append("<tr class='DetailContent'><td class='LabelCell'><span class='Label'>")
        table.Append(label)
        table.Append(":</span></td><td class='FieldCell'>")
        table.Append(value)
        table.Append("</td></tr>")
    End Sub

#Region " LogFiles "
    Private Sub ShowLogfiles()
        If Not pgLogFiles.Selected Then
            Exit Sub
        End If

        Dim table As StringBuilder = New StringBuilder(256)
        table.Clear()
        table.Append("<table class='List'><tr><th>")
        table.Append(GetLabel("name"))
        table.Append("</th><th>")
        table.Append(GetLabel("size"))
        table.Append("</th><th>")
        table.Append(GetLabel("date"))
        table.Append("</th><th></th></tr>")
        For Each dir As System.IO.DirectoryInfo In GetLogDirs()
            For Each file As System.IO.FileInfo In dir.EnumerateFileSystemInfos("*.*").Where(Function(x) x.Extension.Equals(".txt") OrElse x.Extension.Equals(".log") OrElse x.Extension.Equals(".bck")).OrderByDescending(Function(x) x.LastWriteTime)
                ShowLogFile(table, file)
            Next
        Next

        table.Append("</table>")

        lblLogFiles.Text = table.ToString
    End Sub

    Private Function GetLogDirs() As List(Of System.IO.DirectoryInfo)


        Dim logDirs As HashSet(Of String) = Arco.ApplicationServer.Library.ApplicationServerClient.GetServerStatistics.LogDirectories

        logDirs.UnionWith(Arco.Utils.Logging.AllDirectories)

        Dim list As New List(Of System.IO.DirectoryInfo)
        For Each logDir As String In logDirs
            Dim logDirInfo As New System.IO.DirectoryInfo(logDir)
            If logDirInfo.Exists() Then
                list.Add(logDirInfo)
            End If
        Next

        Return list
    End Function

    Private Sub ShowLogFile(ByVal sb As StringBuilder, file As System.IO.FileInfo)

        Dim link As String = StreamFileEncryptionService.Encrypt(file.FullName)

        sb.Append("<tr><td>")
        sb.Append("<a href=""javascript:PC().OpenUrlInActionPane('./Tools/Streamfile.aspx?attach=Y&file=")
        sb.Append(link)
        sb.Append("');"" class='ButtonLink'>")
        sb.Append(file.FullName)
        sb.Append("</a></td><td>")
        sb.Append(Arco.IO.File.FormatSize(file.Length))
        sb.Append("</td><td>")
        sb.Append(ArcoFormatting.FormatDateTime(file.LastWriteTime, True))
        sb.Append("</td><td>")

        sb.Append("<a href=""")
        Dim script As String = ClientScript.GetPostBackClientHyperlink(bntRemoveLogFile, link).Replace("javascript:", "javascript: if (confirm(UIMessages[2]))")
        sb.Append(script)
        sb.Append(";"" class='ButtonLink'>")
        sb.Append("<span class='icon icon-delete' title='" & GetLabel("delete") & "' />")
        sb.Append("</a>&nbsp;")

        sb.Append("<a href=""")
        sb.Append(ClientScript.GetPostBackClientHyperlink(btnBackupLogFile, link))
        sb.Append(";"" class='ButtonLink'>")
        sb.Append("<span class='icon icon-backup' title='Backup' />")
        sb.Append("</a></td></tr>")

    End Sub

    Protected Sub bntRemoveLogFile_Click(ByVal sender As Object, ByVal e As EventArgs) Handles bntRemoveLogFile.Click

        Arco.ApplicationServer.Library.Commands.DeleteFile(GetDecryptedButtonArgument(bntRemoveLogFile))
        ShowLogfiles()
    End Sub

    Protected Sub btnBackupLogFile_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnBackupLogFile.Click


        Dim path As String = GetDecryptedButtonArgument(btnBackupLogFile)


        Dim fileName As String = Arco.IO.File.GetFileName(path)
        Dim backupFile As String = System.IO.Path.Combine(Arco.IO.File.GetDirectory(path), fileName & ".bck")
        Arco.ApplicationServer.Library.Commands.MoveFile(path, backupFile, Arco.ApplicationServer.Library.FileTransfer.OnFileExistsBeheaviour.UseOtherName)

        ShowLogfiles()

    End Sub
    Private Function GetDecryptedButtonArgument(ByVal btn As LinkButton) As String

        Return StringEncryptionService.Decrypt(GetButtonArgument(btn))

    End Function
    Private Function GetButtonArgument(ByVal btn As LinkButton) As String

        If Request.Params("__EVENTTARGET") = btn.UniqueID Then
            Return Request.Params("__EVENTARGUMENT")
        End If
        Throw New InvalidOperationException("Wrong button")

    End Function
#End Region

#Region " Version "
    Private Sub ShowVersions()
        If Not pgVersion.Selected Then
            Exit Sub
        End If
        Dim table As New StringBuilder(256)
        table.Clear()
        table.Append("<table class='List'><tr><th>Product</th><th>Version</th></tr>")

        Dim laAdded As New HashSet(Of String)
        For Each installedVersion As InstalledVersions.VersionInfo In InstalledVersions.GetInstalledversions
            If Not laAdded.Contains(installedVersion.Product) Then
                table.Append("<tr><td>")
                table.Append(installedVersion.Product)
                table.Append("</td><td>")
                table.Append(installedVersion.Version)
                table.Append("</td></tr>")
                laAdded.Add(installedVersion.Product)
            End If
        Next

        table.Append("</table>")


        table.Append("<br/><table class='List'><tr><th>Product</th><th>Model Version</th></tr>")


        For Each modelVersion As Arco.Doma.Configuration.Data.ModelVersions.ModelVersionInfo In Arco.Doma.Configuration.Data.ModelVersions.GetModelVersions

            table.Append("<tr><td>")
            table.Append(modelVersion.Product)
            table.Append("</td><td>")
            table.Append(modelVersion.VersionNumber)
            table.Append("</td></tr>")

        Next

        table.Append("</table>")

        lblVersion.Text = table.ToString
    End Sub
#End Region

#Region " Assemblies "
    Private Sub ShowAssemblies()
        If Not pgAss.Selected Then
            Exit Sub
        End If
        Dim table As New StringBuilder(256)

        table.Append("<table class='List'><tr><th>Name</th><th>Version</th><th>File Version</th><th>Location</th></tr>")
        'web dlls
        'AddAssemblyLine("Arco", GetType(Arco.Security.BusinessIdentity), table)
        'AddAssemblyLine("Arco.ApplicationServer.Library", GetType(Arco.ApplicationServer.Library.BusinessBase), table)
        'AddAssemblyLine("Arco.Doma.Library", GetType(Document), table)
        'AddAssemblyLine("Arco.Utils", GetType(Arco.IO.File), table)
        'AddAssemblyLine("Arco.QueryParser", GetType(Arco.QueryParser.TextParser), table)
        'AddAssemblyLine("Arco.Doma.WebControls", GetType(DMObjectForm), table)
        'AddAssemblyLine("Arco.Doma.Treeview", GetType(Arco.Doma.Treeview.Treeview), table)
        'AddAssemblyLine("Arco.Doma.WebServices", GetType(Arco.Doma.WebServices.Docroom), table)

        'app server dlls
        For Each installedAssembly As InstalledAssemblies.AssemblyInfo In InstalledAssemblies.GetInstalledAssemblies(True).Cast(Of InstalledAssemblies.AssemblyInfo).OrderByDescending(Function(x) x.isSystem)

            AddAssemblyLine(installedAssembly.Name, installedAssembly.Version, installedAssembly.FileVersion, installedAssembly.Location, installedAssembly.isSystem, table)

        Next
        table.Append("</table>")
        lblAssemblies.Text = table.ToString

    End Sub
    Private Shared Sub AddAssemblyLine(ByVal vsName As String, ByVal veType As Type, ByVal table As StringBuilder)

        Dim assembly As Reflection.Assembly = Reflection.Assembly.GetAssembly(veType)
        Dim assemblyName As Reflection.AssemblyName = assembly.GetName()

        Dim location As String = assemblyName.CodeBase
        Dim assemblyVersion As String = assemblyName.Version.ToString()
        Dim fileVersion As String = Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion

        AddAssemblyLine(vsName, assemblyVersion, fileVersion, location, True, table)

    End Sub

    Private Shared Sub AddAssemblyLine(ByVal vsName As String, ByVal assemblyVersion As String, ByVal fileVersion As String, ByVal location As String, ByVal isSystemAssembly As Boolean, ByVal table As StringBuilder)


        table.Append("<tr><td>")
        table.Append(vsName)
        table.Append("</td><td>")
        table.Append(assemblyVersion)
        table.Append("</td><td>")
        table.Append(fileVersion)
        table.Append("</td><td>")
        table.Append(location)
        table.Append("</td></tr>")
    End Sub
#End Region

#Region " Licences "
    Private Sub ShowLicences()
        If Not pgLic.Selected Then
            Exit Sub
        End If
        Dim sb As StringBuilder = New StringBuilder(256)
        sb.Append("<table class='List'><tr><th>Active</th><th>Product</th><th>Type</th><th>")
        sb.Append(GetLabel("creationdate"))
        sb.Append("</th><th>Expires</th></tr>")

        For Each lic As LicenceFiles.LicenceFileInfo In LicenceFiles.GetLicenceFiles
            sb.Append("<tr><td>")
            If lic.Active Then
                sb.Append("<span class='icon icon-ok icon-color-light' />")
            Else
                sb.Append("<span class='icon icon-error' />")
            End If
            sb.Append("</td><td>")
            sb.Append(lic.Product)
            sb.Append("</td><td>")
            sb.Append(lic.Type)
            sb.Append("</td><td>")
            sb.Append(ArcoFormatting.FormatDate(lic.Created))
            sb.Append("</td><td>")
            sb.Append(ArcoFormatting.FormatDate(lic.Expires))
            sb.Append("</td></tr>")
        Next
        sb.Append("</table><br/>")

        sb.Append("<table class='List'><tr><th>Active</th><th>Product</th><th>")
        sb.Append(GetLabel("module"))
        sb.Append("</th><th>Option</th><th>Level</th><th>Value</th></tr>")
        For Each lic As Licences.LicenceInfo In Licences.GetLicences
            AddLicenceLine(lic, sb)
        Next
        sb.Append("</table>")

        lblLicences.Text = sb.ToString
    End Sub
    Private Shared Sub AddLicenceLine(ByVal lic As Licences.LicenceInfo, ByVal table As StringBuilder)

        Dim value As String = lic.Value
        If Not String.IsNullOrEmpty(lic.CurrentValue) Then
            value = lic.CurrentValue & "/" & value
        End If

        Select Case lic.Type
            Case "ExternalCount"

            Case Else
                value &= " (" & lic.Type & ")"
        End Select

        table.Append("<tr><td>")

        Select Case lic.Status
            Case 0 'ok
                table.Append("<span class='icon icon-ok icon-color-light' />")
            Case 1 'warning
                table.Append("<span class='icon icon-warning' />")
            Case 2 'error
                table.Append("<span class='icon icon-error' />")
        End Select
        table.Append("</td><td>")
        table.Append(lic.Product)
        table.Append("</td><td>")
        table.Append(lic.Module)
        table.Append("</td><td>")
        table.Append(lic.Option)
        table.Append("</td><td>")
        table.Append(lic.Level)
        table.Append("</td><td>")
        table.Append(value)
        table.Append("</td></tr>")

    End Sub
#End Region

#Region " App server"

    Private Function IsCurrentHost(ByVal host As String) As Boolean
        Dim currentHost As String = Arco.ApplicationServer.Library.ApplicationServerClient.Server.Host

        If currentHost = "127.0.0.1" Then
            Return Arco.ApplicationServer.Library.Utils.Network.IsLocalHostName(host)
        End If

        Return currentHost = host

    End Function
    Public Sub ShowAppServer()
        If Not pgAppServer.Selected Then
            Exit Sub
        End If
        If Not Arco.ApplicationServer.Library.ApplicationServerClient.IsConfigured Then
            litAppServer.Text = "No Application Server Configured"
            Exit Sub
        End If
        Dim port As Integer = Arco.ApplicationServer.Library.ApplicationServerClient.Server.Port
        Dim stats As Arco.ApplicationServer.Library.Server.Statistics = Arco.ApplicationServer.Library.ApplicationServerClient.GetServerStatistics
        Dim lsTable As New StringBuilder()
        lsTable.Append("<center><div style='margin:10px'><table class='DetailTable'>")

        Dim host As String = Arco.ApplicationServer.Library.ApplicationServerClient.Server.Host
        Dim hostName As String = Arco.ApplicationServer.Library.Utils.Network.GetHostName(host)
        If Not String.IsNullOrEmpty(hostName) Then
            host &= " (" & hostName & ")"
        End If
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>Server:</span></td><td class='FieldCell'>{0}</td></tr>", host)
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>Port:</span></td><td class='FieldCell'>{0}</td></tr>", port)
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>Startup time:</span></td><td class='FieldCell'>{0}</td></tr>", stats.ServiceStartup)
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>#Items:</span></td><td class='FieldCell'>{0} (Model:{1}, Data:{2})</td></tr>", stats.AmountOfObjects, (stats.AmountOfObjects - stats.AmountOfObjectsInDataCache), stats.AmountOfObjectsInDataCache)
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>Total memory of the service:</span></td><td class='FieldCell'>{0}</td></tr>", Arco.IO.File.FormatSize(stats.TotalMemory))
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>Total managed memory of the service:</span></td><td class='FieldCell'>{0}</td></tr>", Arco.IO.File.FormatSize(stats.TotalManagedMemory))
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>#Writes:</span></td><td class='FieldCell'>{0}</td></tr>", stats.TotalAdds)
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>#Reads:</span></td><td class='FieldCell'>{0}</td></tr>", (stats.TotalLocalGets + stats.TotalRemoteGets))
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>#Removes:</span></td><td class='FieldCell'>{0}</td></tr>", stats.TotalDeletes)
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>#Local reads:</span></td><td class='FieldCell'>{0}</td></tr>", stats.TotalLocalGets)
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>#Remote reads:</span></td><td class='FieldCell'>{0}</td></tr>", stats.TotalRemoteGets)
        lsTable.AppendFormat("<tr><td class='LabelCell'><span class='Label'>Hit Ratio:</span></td><td class='FieldCell'>{0}%</td></tr>", stats.HitRatio)


        Dim cltstats As Arco.ApplicationServer.Library.Client.Statistics = Arco.ApplicationServer.Library.ApplicationServerClient.GetClientStatistics
        lsTable.Append("<tr><td class='LabelCell'><span class='Label'>#Compressed :</span></td><td class='FieldCell'>")
        lsTable.Append((cltstats.Decompressed + cltstats.Compressed))
        lsTable.Append(" (")
        lsTable.Append(CType(((cltstats.Decompressed + cltstats.Compressed) * 100 / (cltstats.Success * 2)), Long))
        lsTable.Append("%)</td></tr>")


        Dim replicationTargets As List(Of AppServerList.ServerInfo) = AppServerList.GetServerList().Cast(Of AppServerList.ServerInfo).Where(Function(x) Not (x.Port = port AndAlso IsCurrentHost(x.HostName))).ToList()
        If replicationTargets.Count <> 0 Then
            lsTable.Append("<tr><td colspan='2' valign='top'><br/>")
            lsTable.Append("Replication")
            lsTable.Append("<table class='SubList'><tr><th>")
            lsTable.Append(GetLabel("name"))
            lsTable.Append("</th><th>Host</th><th>Port</th></tr>")


            For Each target As AppServerList.ServerInfo In replicationTargets
                lsTable.Append("<tr><td>")
                lsTable.Append(target.ServiceName)
                lsTable.Append("</td><td>")
                lsTable.Append(target.HostName)
                lsTable.Append("</td><td>")
                lsTable.Append(target.Port)
                lsTable.Append("</td></tr>")
            Next

            lsTable.Append("</table><br/></td></tr>")
        End If


        lsTable.Append("<tr><td colspan='2' valign='top'><br/>")
        lsTable.Append(ArcoFormatting.FormatPanelLabel("Services", ""))
        lsTable.Append("<table class='SubList'><tr><th>")
        lsTable.Append(GetLabel("name"))
        lsTable.Append("</th><th></th><th></th></tr>")
        For Each service As Arco.ApplicationServer.Library.ServiceInfo In Arco.ApplicationServer.Library.ApplicationServerClient.Server.GetServices().OrderBy(Function(x) x.Name)
            lsTable.Append("<tr><td>")
            lsTable.Append(service.Name)

            lsTable.Append("</td><td>")
            lsTable.Append(service.Description)
            lsTable.Append("</td><td>")
            'If Not service.IsInternal Then
            If Not service.Pauzed Then

                lsTable.Append("<a href=""")
                lsTable.Append(ClientScript.GetPostBackClientHyperlink(btnPauseService, service.Name))
                lsTable.Append(";"" class='ButtonLink'>")
                lsTable.Append(GetLabel("pause"))
                lsTable.Append("</a>")
            Else
                lsTable.Append("<a href=""")
                lsTable.Append(ClientScript.GetPostBackClientHyperlink(btnResumeService, service.Name))
                lsTable.Append(";"" class='ButtonLink'>")
                lsTable.Append(GetLabel("resume"))
                lsTable.Append("</a>")
            End If
            ' End If

            lsTable.Append("</td></tr>")
        Next
        lsTable.Append("</table><br/></td></tr>")


        lsTable.Append("<tr><td colspan='2' valign='top'>")
        lsTable.Append(ArcoFormatting.FormatPanelLabel("Application Server Cache", ""))
        lsTable.Append("<table class='SubList'><tr><th>Key</th><th>#Hits</th></tr>")
        Dim sItem As String
        For Each sItem In stats.MostUsedItems.Keys
            lsTable.Append("<tr><td  align='left'>")
            lsTable.Append(sItem)
            lsTable.Append("</td><td  align='left'>")
            lsTable.Append(stats.MostUsedItems.Item(sItem))
            lsTable.Append("</td></tr>")
            ' Arco.ApplicationServer.Library.ApplicationServerClient.MemoryCache.UnloadPlugin(plugin.Assembly)
        Next
        lsTable.Append("</table><br/></td></tr>")




        lsTable.Append("<tr><td colspan='2' valign='top'>")
        lsTable.Append(ArcoFormatting.FormatPanelLabel("Plugins", ""))
        lsTable.Append("<table class='SubList'><tr><th>Plugin</th><th>")
        lsTable.Append(GetLabel("date"))
        lsTable.Append("</th></tr>")
        For Each plugin As Arco.ApplicationServer.Library.Shared.PluginManager.Plugin In Arco.ApplicationServer.Library.ApplicationServerClient.Server.GetLoadedPlugins().Values
            lsTable.Append("<tr><td>")
            lsTable.Append(plugin.Assembly)
            lsTable.Append("</td><td>")
            lsTable.Append(plugin.StartupTime)
            lsTable.Append("</td></tr>")
        Next
        lsTable.Append("</table><br/></td></tr>")

        lsTable.Append("</table></div></center>")

        litAppServer.Text = lsTable.ToString
    End Sub

    Protected Sub btnPauseService_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPauseService.Click

        Arco.ApplicationServer.Library.ApplicationServerClient.Server.PauseService(GetButtonArgument(btnPauseService))
        ShowAppServer()

    End Sub
    Protected Sub btnResumeService_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnResumeService.Click

        Arco.ApplicationServer.Library.ApplicationServerClient.Server.ResumeService(GetButtonArgument(btnResumeService))
        ShowAppServer()

    End Sub
    Protected Sub btnClearCache_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClearCache.Click
        Arco.ApplicationServer.Library.ApplicationServerClient.Server.Clear()
        ShowAppServer()
    End Sub
    Protected Sub btnClearDataCache_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClearDataCache.Click
        Arco.ApplicationServer.Library.ApplicationServerClient.Server.ClearDataCache()
        ShowAppServer()
    End Sub

    Protected Sub btnClearWebCache_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClearWebCache.Click
        Arco.Web.ResourceManager.ClearObjectCache()
        ShowWebServer()
    End Sub
    Protected Sub btnClearAllCaches_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClearAllCaches.Click
        Arco.Web.ResourceManager.ClearObjectCache()
        Arco.ApplicationServer.Library.ApplicationServerClient.Server.Clear()
        ShowAppServer()
        ShowWebServer()
    End Sub
#End Region

    Private Sub ShowInstallHistory()
        If Not pgInst.Selected Then
            Exit Sub
        End If
        grdInst.DataSourceID = "VersionListDataSource"
    End Sub
#Region " Webserver "
    Public Sub ShowWebServer()
        If Not pgWebServer.Selected Then
            Exit Sub
        End If
        Dim lsTable As StringBuilder = New StringBuilder()
        lsTable.Append("<center><div style='margin:10px'><table class='DetailTable'>")
        lsTable.Append("<tr><td colspan='2' valign='top'>")
        lsTable.Append(ArcoFormatting.FormatPanelLabel("IIS Cache", ""))
        lsTable.Append("<table class='SubList'><tr><th>Key</th><th>Type</th></tr>")
        For Each de As DictionaryEntry In HttpRuntime.Cache
            lsTable.Append("<tr><td>")
            lsTable.Append(Server.HtmlEncode(de.Key.ToString))
            lsTable.Append("</td><td>")
            lsTable.Append(HttpRuntime.Cache.Item(de.Key.ToString).GetType().ToString)
            lsTable.Append("</td></tr>")
        Next
        lsTable.Append("</table><br/></td></tr>")

        lsTable.Append("</table></div></center>")

        litWebServer.Text = lsTable.ToString
    End Sub
#End Region
End Class
