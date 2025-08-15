Imports Arco.Doma.CMS.Data
Imports Arco.Doma.CMS.WebPanels
Imports Arco.Doma.Library.baseObjects

Partial Class CMS_AddPanel
    Inherits BaseAdminOnlyPage

    Protected Overrides ReadOnly Property ErrorPageUrl As String
        Get
            Return "../DM_ACL_DENIED.aspx"
        End Get
    End Property

    Private _page As Arco.Doma.CMS.Data.Page


    Private Position As Integer

    Protected Sub CMS_AddPanel_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Title = GetLabel("addpanel")

        Dim pageId As Integer = QueryStringParser.GetInt("pageid")


        If pageId <= 0 Then
            GotoErrorPage(LibError.ErrorCode.ERR_UNEXPECTED)
            Return
        End If

        _page = Arco.Doma.CMS.Data.Page.GetPage(pageId)
        If _page Is Nothing Then
            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
            Return
        End If
        If Not _page.CanEdit Then
            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
            Return
        End If

        Position = QueryStringParser.GetInt("position")
        If Not Page.IsPostBack Then
            Dim showV8PanelTypes = Not String.IsNullOrEmpty(Settings.GetValue("General", "url_v8", ""))
            Dim currentPanels As PanelList = Nothing
            Dim lcolTypes As PanelTypeList = PanelTypeList.GetPanelTypeList
            drpDwnTypes.Items.Clear()
            For Each type As PanelTypeList.PanelTypeInfo In PanelTypeList.GetPanelTypeList
                If Not showV8PanelTypes AndAlso IsV8OnlyPanelType(type) Then Continue For

                Dim canAdd As Boolean = True
                Dim handler As IWebPanel = Arco.ApplicationServer.Library.Shared.PluginManager.CreateInstance(Of IWebPanel)(type.Assembly, type.Class)
                If handler.SingleInstance Then
                    If currentPanels Is Nothing Then
                        currentPanels = PanelList.GetPanelList(pageId)
                    End If
                    canAdd = Not currentPanels.Cast(Of PanelList.PanelInfo).Any(Function(x) x.PanelType = type.ID)
                End If
                If canAdd Then
                    drpDwnTypes.Items.Add(New ListItem(GetPanelTypeName(handler), type.ID.ToString))
                End If
            Next
        End If

    End Sub

    Private Function IsV8OnlyPanelType(type As PanelTypeList.PanelTypeInfo) As Boolean
        If type.ID >= 16 Then Return True

        Return False
    End Function

    Private Function GetPanelTypeName(ByVal panel As IWebPanel) As String
        Try
            Dim label As String = GetDecodedLabel(panel.Name.ToLower())
            If Not String.IsNullOrEmpty(label) Then
                Return label
            End If

        Catch ex As Arco.Web.Exceptions.ResourceNotFoundException
            'ignore
        End Try
        Return panel.Name
    End Function

    Protected Sub btnSave_Click(sender As Object, e As System.EventArgs) Handles btnSave.Click

        Dim loNew As Panel = Panel.NewPanel(_page.ID, Position)
        loNew.PanelType = Convert.ToInt32(drpDwnTypes.SelectedValue)
        loNew = loNew.Save

        'no 'Close()' --> to prevent postback, otherwise statistics control loads with configuration fields
        Page.ClientScript.RegisterStartupScript(Me.GetType, "CloseModalAndReloadPage", "parent.location.href = parent.location;", True)
    End Sub
End Class
