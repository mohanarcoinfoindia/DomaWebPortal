Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Telerik.Web.UI

Public Class UserControls_SelectCategory
    Inherits BaseUserControl
    Private mlCatID As Int32

    Public Event CategoryChanged(ByVal CategoryID As Integer)

    Private Function CanInsertCategory(ByVal loCat As OBJECT_CATEGORYList.OBJECT_CATEGORYInfo, ByVal voParent As DM_OBJECT) As Boolean
        Dim oTest As DM_OBJECT = Nothing
        Dim bCanCreate As Boolean = False
        If Not loCat.DisableManualCreation Then
            Try

                Select Case loCat.Type
                    Case "Folder"
                        oTest = Folder.NewFolder()
                    Case "Document"
                        oTest = Document.NewDocument()
                    Case "Case"
                        oTest = CaseObject.NewCaseObject()
                    Case "Dossier"
                        oTest = Dossier.NewDossier
                End Select
                oTest.SetCreationInfo(loCat, voParent)
                bCanCreate = oTest.CanCreate

            Catch ex As Exception
                ' bCanCreate = False
                ' Response.Write("test " & ex.Message & "<br>")
                bCanCreate = False
            End Try


        End If
        Return bCanCreate



    End Function

    Public Property Filter As List(Of Int32)

    Private Function IsInFilter(ByVal catId As Int32) As Boolean

        Return Filter Is Nothing OrElse Not Filter.Any(Function(x) x <> 0) OrElse Filter.Contains(catId)
    End Function

    Private Sub LoadCombobox()

        If Page.IsPostBack Then
            mlCatID = Val(cmbCategory.SelectedValue)
        End If

        ' If Not Page.IsPostBack Then

        Dim lcolCats As OBJECT_CATEGORYList = OBJECT_CATEGORYList.GetOBJECT_CATEGORYList(CategoryType, ForInsert, EnableIISCaching)
        Dim lcolCatLabels As Globalisation.LABELList = Globalisation.LABELList.GetCategoriesLabelList(EnableIISCaching)

        Dim lcolSubTypes As List(Of DM_OBJECT.AllowedCategory)

        Dim items As New List(Of RadComboBoxItem)

        If Not ForInsert Then

            Dim loItem As New RadComboBoxItem With {
                    .Value = "0",
                    .Text = "",
                    .Selected = (mlCatID = 0)
                }
            items.Add(loItem)

            For Each loCat As OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In lcolCats
                If Not IsInFilter(loCat.ID) Then Continue For

                loItem = New RadComboBoxItem(loCat.TranslatedName(lcolCatLabels), loCat.ID) With {.Selected = (loCat.ID = mlCatID)}
                items.Add(loItem)
            Next
        Else
            Dim loP As DM_OBJECT = ObjectRepository.GetObject(ParentID)
            If TypeOf loP Is QueryShortcut Then
                loP = DirectCast(loP, QueryShortcut).GetExecutionFolder
            End If

            lcolSubTypes = loP.GetAllowedSubCategories(CategoryType)

            If lcolSubTypes.Any Then
                Dim lbCurrentFound As Boolean = False
                For Each loCat As OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In lcolCats
                    If Not IsInFilter(loCat.ID) Then Continue For

                    Dim canAdd As Boolean = False

                    'math categories with allowed categories
                    For Each loSubType As DM_OBJECT.AllowedCategory In lcolSubTypes
                        If loSubType.ID = 0 OrElse loCat.ID = loSubType.ID Then
                            canAdd = CanInsertCategory(loCat, loP)
                            Exit For
                        End If
                    Next

                    If canAdd Then
                        Dim loItem As New RadComboBoxItem(loCat.TranslatedName(lcolCatLabels), loCat.ID)

                        If loCat.ID = mlCatID Then
                            loItem.Selected = True
                            lbCurrentFound = True
                        End If
                        items.Add(loItem)
                    End If
                Next loCat

                If Not lbCurrentFound AndAlso AddCurrentCategory AndAlso mlCatID <> 0 Then

                    Dim loCurrentCat As OBJECT_CATEGORY = OBJECT_CATEGORY.GetOBJECT_CATEGORY(mlCatID)
                    Dim loItem As New RadComboBoxItem(loCurrentCat.TranslatedName(Arco.Security.BusinessIdentity.CurrentIdentity.Language, lcolCatLabels), loCurrentCat.ID)
                    loItem.Selected = True
                    items.Add(loItem)

                End If

            End If
        End If

        For Each item As RadComboBoxItem In items.OrderBy(Function(x) x.Text.ToUpper)
            cmbCategory.Items.Add(item)
        Next

        If cmbCategory.Items.Count = 0 Then
            Visible = False
        ElseIf cmbCategory.Items.Count = 1 Then
            Enabled = False
        Else
            Enabled = True
        End If

        cmbCategory.AutoPostBack = AutoRefresh

        ' End If

        If AutoRefresh Then
            Dim sb As New StringBuilder

            sb.AppendLine(vbCrLf & "function ConfirmChange(sender, eventArgs)")
            sb.AppendLine("{")
            sb.AppendLine(vbTab & "if(confirm(" & EncodingUtils.EncodeJsString(GetDecodedLabel("catchangeconfirm")) & "))")
            sb.AppendLine(vbTab & "{")
            sb.AppendLine(vbTab & vbTab & "Page_ValidationActive = false;")
            sb.AppendLine(vbTab & vbTab & "eventArgs.set_cancel(false);")
            sb.AppendLine(vbTab & "} else {eventArgs.set_cancel(true);}")
            sb.AppendLine("}")

            Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "ConfirmChange", sb.ToString, True)
            cmbCategory.OnClientSelectedIndexChanging = "ConfirmChange"

        Else
            cmbCategory.OnClientSelectedIndexChanging = ""
        End If
    End Sub

    Public Property Enabled As Boolean
        Get
            Return cmbCategory.Enabled
        End Get
        Set(value As Boolean)
            cmbCategory.Enabled = value
        End Set
    End Property
    Public Property CategoryType() As String

    Public Property ForInsert() As Boolean
    Public Property AddCurrentCategory As Boolean

    Public Property Category() As Int32
        Get
            Return Val(cmbCategory.SelectedValue)
        End Get
        Set(ByVal value As Int32)
            mlCatID = value
        End Set
    End Property

    Public Property Width As Unit
        Get
            Return cmbCategory.Width
        End Get
        Set(value As Unit)
            cmbCategory.Width = value
        End Set
    End Property
    Public Property AutoRefresh() As Boolean

    Public Property ParentID() As Int32

    Public Property ParentType() As String


    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
    '    LoadCombobox()
    'End Sub


    Protected Sub cmbCategory_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cmbCategory.SelectedIndexChanged

        RaiseEvent CategoryChanged(Integer.Parse(cmbCategory.SelectedValue))

    End Sub

    Private Sub UserControls_SelectCategory_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Visible Then
            LoadCombobox()
        End If
    End Sub
End Class
