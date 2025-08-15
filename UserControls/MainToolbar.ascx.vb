Imports System.Xml

Partial Class UserControls_MainToolbar
    Inherits BaseUserControl
    Public HasButtons As Boolean = False

    Private Sub sLoad()


        'Dim moUser As Arco.Security.BusinessPrincipal = CType(System.Threading.Thread.CurrentPrincipal, Arco.Security.BusinessPrincipal)
        'Dim moIdentity As Arco.Security.BusinessIdentity = CType(moUser.Identity, Arco.Security.BusinessIdentity)


        'Dim crit As New Arco.Doma.Library.Website.DMToolbarList.Criteria
        'crit.USER_ID = moIdentity.Name
        'Dim objList As Arco.Doma.Library.Website.DMToolbarList = Arco.Doma.Library.Website.DMToolbarList.GetDMToolbarList(crit)


        'If objList.Count > 0 Then

        '    RadToolbarMain.Visible = True
        '    '  RadToolbarMain.I = "~/Images"
        '    RadToolbarMain.Items.Clear()

        '    For Each objToolbar As Arco.Doma.Library.Website.DMToolbarList.DMToolbarInfo In objList

        '        If objToolbar.Seperator = 0 Then
        '            HasButtons = True
        '            Dim objButton As New Telerik.Web.UI.RadToolBarButton



        '            objButton.ToolTip = objToolbar.Description

        '            If objToolbar.Icon = "" Then
        '                objButton.Text = objToolbar.Description
        '                'objButton.DisplayType = Telerik.WebControls.RadToolbarButton.ButtonDisplayType.TextOnly
        '            Else
        '                ' objButton.DisplayType = Telerik.WebControls.RadToolbarButton.ButtonDisplayType.ImageOnly

        '                objButton.ImageUrl = "~/Images/" & objToolbar.Icon
        '            End If

        '            objButton.CommandName = objToolbar.CommandName & IIf(objToolbar.CommandArgument = "", "", "_" & objToolbar.CommandArgument)

        '            If RadToolbarMain.Items.FindItemByAttribute("CommandName", objButton.CommandName) Is Nothing Then
        '                RadToolbarMain.Items.Add(objButton)
        '            End If

        '        Else
        '            Dim objButton As New Telerik.Web.UI.RadToolBarButton
        '            objButton.IsSeparator = True


        '            RadToolbarMain.Items.Add(objButton)
        '        End If

        '    Next
        'Else 'load default from xml
        '    Dim objXML As New XmlDocument
        '    objXML.Load(SiteManagement.SitesManager.GetFullFilePath("DefaultToolbar.xml"))


        '    If Not objXML Is Nothing Then
        '        Try
        '            Dim loButtonsNode As XmlNode = objXML.SelectSingleNode("buttons")
        '            Dim loButtons As XmlNodeList = loButtonsNode.SelectNodes("button")
        '            If loButtons.Count > 0 Then
        '                RadToolbarMain.Visible = True
        '                '  RadToolbarMain.ImagesDir = "~/Images"
        '                RadToolbarMain.Items.Clear()

        '                Dim loButton As XmlNode
        '                For Each loButton In loButtons
        '                    Dim lsDesc As String
        '                    If Not loButton.Attributes.GetNamedItem("description" & Arco.Security.BusinessIdentity.CurrentIdentity.Language) Is Nothing Then
        '                        lsDesc = loButton.Attributes.GetNamedItem("description" & Arco.Security.BusinessIdentity.CurrentIdentity.Language).Value
        '                    Else
        '                        lsDesc = loButton.Attributes.GetNamedItem("description").Value
        '                    End If

        '                    Dim lsCOmmand As String = ""
        '                    Dim lsIcon As String = ""
        '                    Dim lsCommandArg As String = ""
        '                    Dim lsIsSep As String = ""
        '                    Dim lbShow As Boolean = True


        '                    If Not loButton.Attributes.GetNamedItem("icon") Is Nothing Then
        '                        lsIcon = loButton.Attributes.GetNamedItem("icon").Value
        '                    End If
        '                    If Not loButton.Attributes.GetNamedItem("commandname") Is Nothing Then
        '                        lsCOmmand = loButton.Attributes.GetNamedItem("commandname").Value
        '                    End If
        '                    If Not loButton.Attributes.GetNamedItem("commandargument") Is Nothing Then
        '                        lsCommandArg = loButton.Attributes.GetNamedItem("commandargument").Value
        '                    End If
        '                    If Not loButton.Attributes.GetNamedItem("isseparator") Is Nothing Then
        '                        lsIsSep = loButton.Attributes.GetNamedItem("isseparator").Value
        '                    End If
        '                    Dim lbIsSep As Boolean = (lsIsSep = "Y")
        '                    If Not loButton.Attributes.GetNamedItem("AdminOnly") Is Nothing Then
        '                        If loButton.Attributes.GetNamedItem("AdminOnly").Value = "Y" Then
        '                            lbShow = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.isAdmin
        '                        End If
        '                    End If
        '                    If Not loButton.Attributes.GetNamedItem("Roles") Is Nothing Then
        '                        Dim laRoles() As String = loButton.Attributes.GetNamedItem("Roles").Value.Split(",")
        '                        Dim i As Int32
        '                        lbShow = False
        '                        For i = laRoles.GetLowerBound(0) To laRoles.GetUpperBound(0)
        '                            Dim lsRole As String = laRoles(i).Trim
        '                            If lsRole <> "" Then
        '                                If Arco.Security.BusinessIdentity.CurrentIdentity.IsInRole(lsRole) Then
        '                                    lbShow = True
        '                                    Exit For
        '                                End If
        '                            End If
        '                        Next
        '                    End If

        '                    If lbShow Then
        '                        If Not lbIsSep Then
        '                            HasButtons = True
        '                            Dim objButton As New Telerik.Web.UI.RadToolBarButton



        '                            objButton.ToolTip = lsDesc

        '                            If lsIcon = "" Then
        '                                objButton.Text = lsDesc
        '                                ' objButton.DisplayType = Telerik.WebControls.RadToolbarButton.ButtonDisplayType.TextOnly
        '                            Else
        '                                '  objButton.DisplayType = Telerik.WebControls.RadToolbarButton.ButtonDisplayType.ImageOnly

        '                                objButton.ImageUrl = "~/Images/" & lsIcon
        '                            End If

        '                            objButton.CommandName = lsCOmmand & IIf(lsCommandArg = "", "", "_" & lsCommandArg)

        '                            If RadToolbarMain.Items.FindItemByAttribute("CommandName", objButton.CommandName) Is Nothing Then
        '                                RadToolbarMain.Items.Add(objButton)
        '                            End If

        '                        Else
        '                            Dim objButton As New Telerik.Web.UI.RadToolBarButton
        '                            objButton.IsSeparator = True


        '                            RadToolbarMain.Items.Add(objButton)
        '                        End If
        '                    End If
        '                Next
        '            End If
        '        Catch

        '        End Try
        '    End If
        'End If

    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        sLoad()
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


    End Sub

  
End Class
