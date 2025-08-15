Imports Arco.Doma.Library

Partial Class Tools_FileCompare
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim lsSelection As String = QueryStringParser.GetString("docs")

        Dim laDocs() As String
        Dim lsDoc As String
        Dim llDocID As Int32
        Dim strFileID As String = ""
        Dim strFileNames As String = ""
        Dim lsFromArchive As String = ""
        Dim liFirstFile As Int32 = 0
        Dim liSecondFile As Int32 = 0
        Dim lsFirstFileName As String = ""
        Dim lsSecondFileName As String = ""

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


                If loObj IsNot Nothing AndAlso loObj.CanViewFiles Then

                    For Each loFileInfo As FileList.FileInfo In loObj.Files

                        If Not String.IsNullOrEmpty(strFileID) Then strFileID &= ","

                        strFileID &= "'" & loFileInfo.ID.ToString & "'"

                        If Not String.IsNullOrEmpty(strFileNames) Then strFileNames &= ","

                        strFileNames &= "'" & loFileInfo.Name.ToString & "'"

                        If liFirstFile = 0 Then
                            radPaneLeftFile.ContentUrl = "../DM_VIEW_FILE.aspx?FILE_ID=" & loFileInfo.ID.ToString & "&rend=preview&fromarchive=" & lsFromArchive
                            liFirstFile = loFileInfo.ID
                            lsFirstFileName = loFileInfo.Name.ToString
                        ElseIf liSecondFile = 0 Then
                            radPaneRightFile.ContentUrl = "../DM_VIEW_FILE.aspx?FILE_ID=" & loFileInfo.ID.ToString & "&rend=preview&fromarchive=" & lsFromArchive
                            liSecondFile = loFileInfo.ID
                            lsSecondFileName = loFileInfo.Name.ToString

                        End If
                    Next
                End If
            End If
        Next

        hplNextLeftFile.ImageUrl = ThemedImage.GetUrl("nextpage.png", Me)
        hplNextRightFile.ImageUrl = ThemedImage.GetUrl("nextpage.png", Me)
        hplPreviousLeftFile.ImageUrl = ThemedImage.GetUrl("previouspage.png", Me)
        hplPreviousRightFile.ImageUrl = ThemedImage.GetUrl("previouspage.png", Me)

        If Not String.IsNullOrEmpty(strFileID) Then
            hplNextLeftFile.Enabled = True
            hplNextLeftFile.Attributes.Add("onclick", "NextLeftFile(1,'" & lsFromArchive & "','preview')")
            hplPreviousLeftFile.Attributes.Add("onclick", "function(){return false;}")

            If liSecondFile > 0 Then
                hplNextRightFile.Enabled = True
                hplPreviousRightFile.Enabled = True
                hplNextRightFile.Attributes.Add("onclick", "NextRightFile(2,'" & lsFromArchive & "','preview')")
                hplPreviousRightFile.Attributes.Add("onclick", "PreviousRightFile(0,'" & lsFromArchive & "','preview')")

            End If
            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "SetFirstFileTitle", "window.onload = function(){SetLeftSubTitle('" & lsFirstFileName & "');SetRightSubTitle('" & lsSecondFileName & "');SetLeftTooltips(0);SetRightTooltips(1);};", True)

            'Register clientscript
            Page.ClientScript.RegisterArrayDeclaration("arrLeftFiles", strFileID)
            Page.ClientScript.RegisterArrayDeclaration("arrLeftFileNames", strFileNames)
            Page.ClientScript.RegisterArrayDeclaration("arrRightFiles", strFileID)
            Page.ClientScript.RegisterArrayDeclaration("arrRightFileNames", strFileNames)

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
            sb.AppendLine("     pane.set_contentUrl('../DM_VIEW_FILE.aspx?FILE_ID=' + fileid + '&rend=' + rend + '&fromarchive=' + fromarchive);")
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
            sb.AppendLine("     pane.set_contentUrl('../DM_VIEW_FILE.aspx?FILE_ID=' + fileid + '&rend=' + rend + '&fromarchive=' + fromarchive);")
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

        Else
            'Response.Write("No Files")
            GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_FILENOTFOUND)
        End If


      

    End Sub
End Class
