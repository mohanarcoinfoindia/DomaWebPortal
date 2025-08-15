
Imports Arco.Doma.Library.baseObjects

Public Class DM_FileTypes
    Inherits BaseAdminOnlyPage


    Private Enum CellIndex
        Extension = 1
        CatId = 2
        Pdf = 14
        Html = 15
        Thumbnail = 16
        Mail = 17
        Preview = 19
    End Enum
    Protected Sub GridView1_RowCreated(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowCreated
        Dim r As GridViewRow = e.Row
        If r.RowType = DataControlRowType.Header Then
            r.Cells(CellIndex.Extension).Text = GetLabel("extension")
            r.Cells(CellIndex.CatId).Text = ""
            r.Cells(3).Text = GetLabel("category")
            r.Cells(4).Text = GetLabel("download")
            r.Cells(5).Text = GetLabel("manualuploadallowed")
            r.Cells(6).Text = GetLabel("fulltext")
            r.Cells(7).Text = GetLabel("keywords")
            r.Cells(8).Text = GetLabel("abstract")
            r.Cells(9).Text = GetLabel("language")
            r.Cells(10).Text = "Auto " & GetLabel("ocr")
            r.Cells(11).Text = GetLabel("afterocr")
            r.Cells(12).Text = GetLabel("fileeditmode")
            r.Cells(13).Text = GetLabel("editfile")
            r.Cells(CellIndex.Pdf).Text = "Pdf"
            r.Cells(CellIndex.Html).Text = "Html"
            r.Cells(CellIndex.Thumbnail).Text = GetLabel("thumbnail")
            r.Cells(CellIndex.Mail).Text = GetLabel("mail")
            r.Cells(19).Text = GetLabel("asyncindexing")
            r.Cells(CellIndex.Preview).Text = GetLabel("preview")
        End If
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound

        Dim r As GridViewRow = e.Row
        If r.RowType = DataControlRowType.DataRow Then

            Dim ft As DM_FileTypeList.FileTypeInfo = CType(r.DataItem, DM_FileTypeList.FileTypeInfo)

            r.Cells(0).Text = Icons.GetFileIconImage(ft.Extension)
            Dim l As Label

            Dim catLabel As String = Nothing
            If ft.CatID = 0 Then
                catLabel = "Global"
            Else
                For Each loCat As OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In mcolCats
                    If loCat.ID = ft.CatID Then
                        catLabel = loCat.TranslatedName(mcolCatLabels)
                        Exit For
                    End If
                Next
            End If
            l = DirectCast(r.FindControl("lblCat"), Label)
            If Not l Is Nothing Then
                l.Text = catLabel

            End If

            l = DirectCast(r.FindControl("lblPrevRend"), Label)
            If l IsNot Nothing Then
                Select Case l.Text
                    Case "pdf"
                        l.Text = "Pdf"
                    Case "htm"
                        l.Text = "Html"
                    Case "thumb"
                        l.Text = GetDecodedLabel("thumbnail")
                    Case "idx"
                        l.Text = "Text"
                    Case "native"
                        l.Text = Arco.IO.MimeTypes.GetWebDavApplicationName(ft.Extension, GetLabel("nativeapp"))
                    Case "download"
                        l.Text = GetDecodedLabel("download")
                    Case "mail"
                        l.Text = "Mail"
                End Select
            End If


            l = DirectCast(r.FindControl("lblEditMode"), Label)
            If l IsNot Nothing Then
                Select Case l.Text
                    Case "0"
                        l.Text = ""
                    Case "1"
                        l.Text = "In-Line"
                    Case "2"
                        l.Text = "WebDav"
                    Case "3"
                        l.Text = "Both"
                End Select
            End If



            l = DirectCast(r.FindControl("lblEditBeheaviour"), Label)
            If Not l Is Nothing Then
                Select Case l.Text
                    Case "0", "OverwriteCurrent"
                        l.Text = GetLabel("OverwriteVersion")
                    Case "1", "CreateNewMinor"
                        l.Text = GetLabel("savenewsubversion")
                    Case "2", "CreateNewMajor"
                        l.Text = GetLabel("savenewversion")
                End Select
            End If


            l = DirectCast(r.FindControl("lblOcrBeheaviour"), Label)
            If Not l Is Nothing Then
                Select Case l.Text
                    Case "0", "KeepOriginal"
                        l.Text = GetLabel("keeporiginal")
                    Case "1", "OverwriteCurrent"
                        l.Text = GetLabel("OverwriteVersion")
                    Case "2", "CreateNewMinor"
                        l.Text = GetLabel("savenewsubversion")
                    Case "3", "CreateNewMajor"
                        l.Text = GetLabel("savenewversion")
                End Select
            End If

            r.Cells(CellIndex.Pdf).Enabled = Arco.Utils.PDFManager.CanConvertToPdf(ft.Extension)
            r.Cells(CellIndex.Html).Enabled = Arco.Utils.Html.CanConvertToHtml(ft.Extension)
            r.Cells(CellIndex.Thumbnail).Enabled = Arco.Utils.Thumbnail.CanCreateThumbnail(ft.Extension)
            r.Cells(CellIndex.Mail).Enabled = (ft.Extension = "eml" OrElse ft.Extension = "msg")


            Dim forCatLabel As String = ""
            If ft.CatID <> 0 Then
                Select Case Language
                    Case "N"
                        forCatLabel = " voor categorie " & catLabel & " "
                    Case "F"
                        forCatLabel = " pour catégorie " & catLabel & " "
                    Case Else
                        forCatLabel = " for category " & catLabel & " "
                End Select
            End If

        End If

    End Sub

    Private Function CanEditInline(ByVal ext As String) As Boolean
        Select Case ext
            Case "htm", "txt", "rtf", "docx"
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private mcolCats As OBJECT_CATEGORYList
    Private mcolCatLabels As Arco.Doma.Library.Globalisation.LABELList


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Title = "Manage File Extensions"


        lblFilter.Text = GetLabel("filter") + " " + GetLabel("category") + ":"
        lblMessage.Visible = False



        mcolCats = OBJECT_CATEGORYList.GetOBJECT_CATEGORYList(False, EnableIISCaching)
        mcolCatLabels = Arco.Doma.Library.Globalisation.LABELList.GetCategoriesLabelList(EnableIISCaching)

        If Not IsPostBack Then


            ddCatFilter.Items.Add(New ListItem("", ""))
            ddCatFilter.Items.Add(New ListItem("Global", "0"))
            For Each cat As OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In mcolCats
                If cat.FileCardinalityConstraint <> Arco.Doma.Library.ConstraintEnum.NotAllowed Then
                    ddCatFilter.Items.Add(New ListItem(cat.TranslatedName(mcolCatLabels), cat.ID.ToString()))
                    'add filter
                End If
            Next
        End If

    End Sub

End Class
