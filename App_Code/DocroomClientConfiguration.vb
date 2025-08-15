Imports System.Reflection
Imports System.Configuration.Internal
Imports System.Configuration 'needed , otherwise double namespace

Namespace Doma
    Public Class DocroomClientConfiguration
        Public Shared Function GetConfigurations() As List(Of String)
            Dim cfgFile As Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
            Dim l As New List(Of String)
            For Each s As ConfigurationSection In cfgFile.Sections
                If s.SectionInformation.Type.Contains("System.Configuration.AppSettingsSection") Then
                    l.Add(s.SectionInformation.Name)
                End If
            Next
            Return l
        End Function
        Public Shared Sub InstallProxy()
            InstallProxy("")
        End Sub
        Public Shared Sub InstallProxy(ByVal appSettingsKey As String)
            Dim o As Object = ConfigurationManager.AppSettings
            Dim s_configSystem As FieldInfo = GetType(ConfigurationManager).GetField("s_configSystem", BindingFlags.Static Or BindingFlags.NonPublic)

            s_configSystem.SetValue(Nothing, New DocRoomConfigProxy(s_configSystem.GetValue(Nothing), appSettingsKey))
        End Sub


        Private Class DocRoomConfigProxy
            Implements IInternalConfigSystem

            Private _overrideKey As String
            Private Const DefaultKey As String = "appSettings"
            Private _base As IInternalConfigSystem
            Private _cachedAppSettings As NameValueCollection
            Public Sub New(ByVal voBase As IInternalConfigSystem, ByVal appSettingsKey As String)
                _base = voBase

                If String.IsNullOrEmpty(appSettingsKey) Then
                    appSettingsKey = DefaultKey
                End If
                _overrideKey = appSettingsKey

            End Sub

            Private Sub sReadAppSettings()
                If _cachedAppSettings Is Nothing Then
                    'we need to copy, the base is read-only
                    _cachedAppSettings = New NameValueCollection
                    _cachedAppSettings.Add(_base.GetSection(_overrideKey))

                    If Not String.IsNullOrEmpty(_cachedAppSettings.Get("VirtualConfigFile")) Then

                        Dim lsVirtualFile As String = _cachedAppSettings.Get("VirtualConfigFile") '"~/DomaConfig/Doma.config"
                        Dim lsPhysicalFile As String = HttpContext.Current.Server.MapPath(lsVirtualFile)

                        'we need to do it manually, using the object doesn't work all the time
                        Dim xmldoc As System.Xml.XmlDocument = New System.Xml.XmlDocument
                        xmldoc.Load(lsPhysicalFile)
                        Dim node As System.Xml.XmlNode = xmldoc.SelectSingleNode(DefaultKey)
                        If Not node Is Nothing Then
                            For Each kv As System.Xml.XmlNode In node.SelectNodes("add")
                                Dim loKey As System.Xml.XmlAttribute = kv.Attributes.GetNamedItem("key")
                                Dim loValue As System.Xml.XmlAttribute = kv.Attributes.GetNamedItem("value")

                                If Not _cachedAppSettings.AllKeys.Any(Function(x) x = loKey.Value) Then
                                    _cachedAppSettings.Add(loKey.Value, loValue.Value)
                                Else
                                    _cachedAppSettings(loKey.Value) = loValue.Value
                                End If
                            Next

                        End If
                    End If
                End If
            End Sub

            Public Function GetSection(ByVal configKey As String) As Object Implements IInternalConfigSystem.GetSection
                If (configKey = DefaultKey) Then
                    'this maps a call to appSettings to our override section
                    sReadAppSettings()

                    Return _cachedAppSettings
                Else
                    Return _base.GetSection(configKey)
                End If
            End Function

            Public Sub RefreshConfig(ByVal sectionName As String) Implements IInternalConfigSystem.RefreshConfig
                If (sectionName = DefaultKey) Then
                    _cachedAppSettings = Nothing
                End If
                _base.RefreshConfig(sectionName)
            End Sub

            Public ReadOnly Property SupportsUserConfig() As Boolean Implements IInternalConfigSystem.SupportsUserConfig
                Get
                    Return _base.SupportsUserConfig
                End Get
            End Property
        End Class
    End Class
End Namespace
