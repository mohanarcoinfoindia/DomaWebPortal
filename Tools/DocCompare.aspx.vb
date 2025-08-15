Imports Arco.Doma.Library

Partial Class Tools_DocCompare
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim lsSelection As String = QueryStringParser.GetString("docs")
        Dim lbPreview As Boolean = QueryStringParser.GetBoolean("Preview")

        Dim laDocs() As String
        Dim lsDoc As String
        Dim llDocID As Int32
        Dim strdocID As String = ""
        Dim strdocNames As String = ""
        Dim lsFromArchive As String = ""
        Dim liFirstDoc As Int32 = 0
        Dim liSecondDoc As Int32 = 0
        Dim lsFirstDocName As String = ""
        Dim lsSecondDocName As String = ""

        laDocs = lsSelection.Split(";")
        For Each lsDoc In laDocs
            lsDoc = lsDoc.Trim
            If Not String.IsNullOrEmpty(lsDoc) Then
                llDocID = CLng(lsDoc)
                Dim loObj As baseObjects.DM_OBJECT = ObjectRepository.SearchObject(llDocID)
                lsFromArchive = ""
                If (TypeOf loObj Is baseObjects.DM_VersionControlledObject) Then
                    If CType(loObj, baseObjects.DM_VersionControlledObject).FromArchive Then
                        lsFromArchive = "Y"
                    End If
                End If

                If liFirstDoc = 0 Then
                    If lbPreview Then
                        radPaneLeftFile.ContentUrl = "../DM_Detailview.aspx?DM_OBJECT_ID=" & llDocID & "&preview=Y&fromarchive=" & lsFromArchive
                    Else
                        radPaneLeftFile.ContentUrl = "../DM_Detail.aspx?DM_OBJECT_ID=" & llDocID & "&fromarchive=" & lsFromArchive
                    End If
                    liFirstDoc = llDocID
                    lsFirstDocName = loObj.Name
                ElseIf liSecondDoc = 0 Then
                    If lbPreview Then
                        radPaneRightFile.ContentUrl = "../DM_Detailview.aspx?DM_OBJECT_ID=" & llDocID & "&preview=Y&fromarchive=" & lsFromArchive
                    Else
                        radPaneRightFile.ContentUrl = "../DM_Detail.aspx?DM_OBJECT_ID=" & llDocID & "&fromarchive=" & lsFromArchive
                    End If
                    liSecondDoc = llDocID
                    lsSecondDocName = loObj.Name

                End If

                If Not String.IsNullOrEmpty(strdocID) Then strdocID &= ","
                strdocID &= "'" & llDocID & "'"

                If Not String.IsNullOrEmpty(strdocNames) Then strdocNames &= ","

                strdocNames &= "'" & loObj.Name & "'"

            End If
        Next

        hplNextLeftFile.ImageUrl = ThemedImage.GetUrl("nextpage.png", Me)
        hplNextRightFile.ImageUrl = ThemedImage.GetUrl("nextpage.png", Me)
        hplPreviousLeftFile.ImageUrl = ThemedImage.GetUrl("previouspage.png", Me)
        hplPreviousRightFile.ImageUrl = ThemedImage.GetUrl("previouspage.png", Me)

        If Not String.IsNullOrEmpty(strdocID) Then
            hplNextLeftFile.Enabled = True
            hplNextLeftFile.Attributes.Add("onclick", "NextLeftFile(1,'" & lsFromArchive & "','preview')")
            hplPreviousLeftFile.Attributes.Add("onclick", "function(){return false;}")

            If liSecondDoc > 0 Then
                hplNextRightFile.Enabled = True
                hplPreviousRightFile.Enabled = True
                hplNextRightFile.Attributes.Add("onclick", "NextRightFile(2,'" & lsFromArchive & "','preview')")
                hplPreviousRightFile.Attributes.Add("onclick", "PreviousRightFile(0,'" & lsFromArchive & "','preview')")

            End If
            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "SetFirstFileTitle", "window.onload = function(){SetLeftSubTitle('" & lsFirstDocName & "');SetRightSubTitle('" & lsSecondDocName & "');SetLeftTooltips(0);SetRightTooltips(1);};", True)

        End If


        'Register clientscript
        Page.ClientScript.RegisterArrayDeclaration("arrLeftFiles", strdocID)
        Page.ClientScript.RegisterArrayDeclaration("arrLeftFileNames", strdocNames)
        Page.ClientScript.RegisterArrayDeclaration("arrRightFiles", strdocID)
        Page.ClientScript.RegisterArrayDeclaration("arrRightFileNames", strdocNames)

        Dim sb As New StringBuilder
        sb.AppendLine(" var currLeftFileIndex = 0;")
        sb.AppendLine("function CurrentLeftFile(){NextLeftFile(currLeftFileIndex,'" & lsFromArchive & "','preview');}")
        sb.AppendLine("function NextLeftFile(index,fromarchive,rend)")
        sb.AppendLine("{")
        sb.AppendLine(" //debugger;")
        sb.AppendLine(" var nextlink = document.getElementById('" & hplNextLeftFile.ClientID & "');")
        sb.AppendLine(" if(index>=arrLeftFiles.length-1)")
        sb.AppendLine(" {")
        sb.AppendLine("     nextlink.disabled=true;")
        sb.AppendLine("     nextlink.style.cursor = 'default';")
        sb.AppendLine("     nextlink.onclick=null;")
        sb.AppendLine(" }")
        sb.AppendLine(" else")
        sb.AppendLine(" {")
        sb.AppendLine("     nextlink.disabled=false;")
        sb.AppendLine("     nextlink.style.cursor = 'pointer';")
        sb.AppendLine("     nextlink.onclick =function(){NextLeftFile(index+1,fromarchive,rend);}")
        sb.AppendLine(" }")
        sb.AppendLine(" var prevlink = document.getElementById('" & hplPreviousLeftFile.ClientID & "');")
        sb.AppendLine(" if(index>0)")
        sb.AppendLine(" {")
        sb.AppendLine("     prevlink.disabled=false;")
        sb.AppendLine("     prevlink.style.cursor = 'pointer';")
        sb.AppendLine("     prevlink.onclick= function(){PreviousLeftFile(index-1,fromarchive,rend);}")
        sb.AppendLine(" }")
        sb.AppendLine(" ShowLeftFile(arrLeftFiles[index],fromarchive,rend);")
        ' sb.AppendLine(" SetSubTitle(arrFileNames[index]);")
        sb.AppendLine("}")
        sb.AppendLine("function PreviousLeftFile(index,fromarchive,rend)")
        sb.AppendLine("{")
        sb.AppendLine(" //debugger;")
        sb.AppendLine(" var prevlink = document.getElementById('" & hplPreviousLeftFile.ClientID & "');")
        sb.AppendLine(" if(index==0)")
        sb.AppendLine(" {")
        sb.AppendLine("     prevlink.disabled=true;")
        sb.AppendLine("     prevlink.style.cursor = 'default';")
        sb.AppendLine("     prevlink.onclick=null;")
        sb.AppendLine(" }")
        sb.AppendLine(" else")
        sb.AppendLine(" {")
        sb.AppendLine("     prevlink.disabled=false;")
        sb.AppendLine("     prevlink.style.cursor = 'pointer';")
        sb.AppendLine("     prevlink.onclick =function(){PreviousLeftFile(index-1,fromarchive,rend);}")
        sb.AppendLine(" }")
        sb.AppendLine(" var nextlink = document.getElementById('" & hplNextLeftFile.ClientID & "');")
        sb.AppendLine(" if(index<arrLeftFiles.length-1)")
        sb.AppendLine(" {")
        sb.AppendLine("     nextlink.disabled=false;")
        sb.AppendLine("     nextlink.style.cursor = 'pointer';")
        sb.AppendLine("     nextlink.onclick= function(){NextLeftFile(index+1,fromarchive,rend);};")
        sb.AppendLine(" }")
        sb.AppendLine(" ShowLeftFile(arrLeftFiles[index],fromarchive,rend);")
        'sb.AppendLine(" SetSubTitle(arrFileNames[index]);")
        sb.AppendLine("}")
        sb.AppendLine("function ShowLeftFile(fileid,fromarchive,rend)")
        sb.AppendLine("{")
        sb.AppendLine(" var splitter = $find('" & radSplitterContent.ClientID & "');")
        sb.AppendLine(" var pane= splitter.GetPaneById('" & radPaneLeftFile.UniqueID & "');")
        sb.AppendLine(" if(pane!=null)")
        sb.AppendLine(" {")
        If lbPreview Then
            sb.AppendLine("     pane.set_contentUrl('../DM_Detailview.aspx?DM_OBJECT_ID=' + fileid + '&preview=Y&fromarchive=' + fromarchive);")
        Else
            sb.AppendLine("     pane.set_contentUrl('../DM_Detail.aspx?DM_OBJECT_ID=' + fileid + '&fromarchive=' + fromarchive);")
        End If
        sb.AppendLine("     for (var i=0;i < arrLeftFiles.length;i++){")
        sb.AppendLine("         if (arrLeftFiles[i] == fileid) {currLeftFileIndex = i;SetLeftSubTitle(arrLeftFileNames[i]);SetLeftTooltips(i);}")
        sb.AppendLine("     }")
        sb.AppendLine(" }")
        sb.AppendLine("}")

        sb.AppendLine("function SetLeftTooltips(index){")
        sb.AppendLine(" var prevlink = document.getElementById('" & hplPreviousLeftFile.ClientID & "');")
        sb.AppendLine(" if(index>0){")
        sb.AppendLine("     prevlink.title = '" & GetDecodedLabel("prevfile") & " (' + (index) + '/' + (arrLeftFiles.length) + ')';")
        sb.AppendLine(" }")
        sb.AppendLine(" var nextlink = document.getElementById('" & hplNextLeftFile.ClientID & "');")
        sb.AppendLine(" if(index<arrLeftFiles.length-1)")
        sb.AppendLine(" {")
        sb.AppendLine("     nextlink.title = '" & GetDecodedLabel("nextfile") & " (' + (index+2) + '/' + (arrLeftFiles.length) + ')';")
        sb.AppendLine(" }")
        sb.AppendLine("}")


        sb.AppendLine(" var currRightFileIndex = 1;")
        sb.AppendLine("function CurrentRightFile(){NextRightFile(currRightFileIndex,'" & lsFromArchive & "','preview');}")
        sb.AppendLine("function NextRightFile(index,fromarchive,rend)")
        sb.AppendLine("{")
        sb.AppendLine(" //debugger;")
        sb.AppendLine(" var nextlink = document.getElementById('" & hplNextRightFile.ClientID & "');")
        sb.AppendLine(" if(index>=arrRightFiles.length-1)")
        sb.AppendLine(" {")
        sb.AppendLine("     nextlink.disabled=true;")
        sb.AppendLine("     nextlink.style.cursor = 'default';")
        sb.AppendLine("     nextlink.onclick=null;")
        sb.AppendLine(" }")
        sb.AppendLine(" else")
        sb.AppendLine(" {")
        sb.AppendLine("     nextlink.disabled=false;")
        sb.AppendLine("     nextlink.style.cursor = 'pointer';")
        sb.AppendLine("     nextlink.onclick =function(){NextRightFile(index+1,fromarchive,rend);}")
        sb.AppendLine(" }")
        sb.AppendLine(" var prevlink = document.getElementById('" & hplPreviousRightFile.ClientID & "');")
        sb.AppendLine(" if(index>0)")
        sb.AppendLine(" {")
        sb.AppendLine("     prevlink.disabled=false;")
        sb.AppendLine("     prevlink.style.cursor = 'pointer';")
        sb.AppendLine("     prevlink.onclick= function(){PreviousRightFile(index-1,fromarchive,rend);}")
        sb.AppendLine(" }")
        sb.AppendLine(" ShowRightFile(arrRightFiles[index],fromarchive,rend);")
        ' sb.AppendLine(" SetSubTitle(arrFileNames[index]);")
        sb.AppendLine("}")
        sb.AppendLine("function PreviousRightFile(index,fromarchive,rend)")
        sb.AppendLine("{")
        sb.AppendLine(" //debugger;")
        sb.AppendLine(" var prevlink = document.getElementById('" & hplPreviousRightFile.ClientID & "');")
        sb.AppendLine(" if(index==0)")
        sb.AppendLine(" {")
        sb.AppendLine("     prevlink.disabled=true;")
        sb.AppendLine("     prevlink.style.cursor = 'default';")
        sb.AppendLine("     prevlink.onclick=null;")
        sb.AppendLine(" }")
        sb.AppendLine(" else")
        sb.AppendLine(" {")
        sb.AppendLine("     prevlink.disabled=false;")
        sb.AppendLine("     prevlink.style.cursor = 'pointer';")
        sb.AppendLine("     prevlink.onclick =function(){PreviousRightFile(index-1,fromarchive,rend);}")
        sb.AppendLine(" }")
        sb.AppendLine(" var nextlink = document.getElementById('" & hplNextRightFile.ClientID & "');")
        sb.AppendLine(" if(index<arrRightFiles.length-1)")
        sb.AppendLine(" {")
        sb.AppendLine("     nextlink.disabled=false;")
        sb.AppendLine("     nextlink.style.cursor = 'pointer';")
        sb.AppendLine("     nextlink.onclick= function(){NextRightFile(index+1,fromarchive,rend);};")
        sb.AppendLine(" }")
        sb.AppendLine(" ShowRightFile(arrRightFiles[index],fromarchive,rend);")
        'sb.AppendLine(" SetSubTitle(arrFileNames[index]);")
        sb.AppendLine("}")
        sb.AppendLine("function ShowRightFile(fileid,fromarchive,rend)")
        sb.AppendLine("{")
        sb.AppendLine(" var splitter = $find('" & radSplitterContent.ClientID & "');")
        sb.AppendLine(" var pane= splitter.GetPaneById('" & radPaneRightFile.UniqueID & "');")
        sb.AppendLine(" if(pane!=null)")
        sb.AppendLine(" {")
        If lbPreview Then
            sb.AppendLine("     pane.set_contentUrl('../DM_Detailview.aspx?DM_OBJECT_ID=' + fileid + '&preview=Y&fromarchive=' + fromarchive);")
        Else
            sb.AppendLine("     pane.set_contentUrl('../DM_Detail.aspx?DM_OBJECT_ID=' + fileid + '&fromarchive=' + fromarchive);")
        End If
        sb.AppendLine("     for (var i=0;i < arrRightFiles.length;i++){")
        sb.AppendLine("         if (arrRightFiles[i] == fileid) {currRightFileIndex = i;SetRightSubTitle(arrRightFileNames[i]);SetRightTooltips(i);}")
        sb.AppendLine("     }")
        sb.AppendLine(" }")
        sb.AppendLine("}")

        sb.AppendLine("function SetRightTooltips(index){")
        sb.AppendLine(" var prevlink = document.getElementById('" & hplPreviousRightFile.ClientID & "');")
        sb.AppendLine(" if(index>0){")
        sb.AppendLine("     prevlink.title = '" & GetDecodedLabel("prevfile") & " (' + (index) + '/' + (arrRightFiles.length) + ')';")
        sb.AppendLine(" }")
        sb.AppendLine(" var nextlink = document.getElementById('" & hplNextRightFile.ClientID & "');")
        sb.AppendLine(" if(index<arrRightFiles.length-1)")
        sb.AppendLine(" {")
        sb.AppendLine("     nextlink.title = '" & GetDecodedLabel("nextfile") & " (' + (index+2) + '/' + (arrRightFiles.length) + ')';")
        sb.AppendLine(" }")
        sb.AppendLine("}")


        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging", sb.ToString, True)

    End Sub
End Class
