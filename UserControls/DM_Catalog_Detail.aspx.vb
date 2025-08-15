Imports Arco.Doma.Library

Partial Class DM_Catalog_Detail

    Inherits System.Web.UI.Page

    Dim moItem As Lists.ListItem
    Dim moList As Lists.List
    Dim msUserLang As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim llListItemID As Integer = 0

        msUserLang = Arco.Security.BusinessIdentity.CurrentIdentity.Language
        llListItemID = Request.QueryString("id")

        If llListItemID > 0 Then

            moItem = Lists.ListItem.GetListItem(llListItemID)
            If moItem.ID = llListItemID Then

                moList = Lists.List.GetList(moItem.LIST_ID)

                sShowDetails()

                If moList.MULTI_LINGUAL = True Then
                    sShowTranslations(llListItemID)
                End If

                If moList.USE_SYNONYMS = True Then
                    sShowSynonyms(llListItemID)
                End If

                If moList.LIST_TYPE = "CONCEPTTREE" Or _
                   moList.LIST_TYPE = "THESAURUS" Then
                    sShowRelations(llListItemID)
                End If

            End If
          
        End If

    End Sub

    Private Sub sShowDetails()

        Dim lsTable As String = ""
        Dim lsDesc As String = ""
        Dim lsSN As String = ""

        GetTrans(moItem.ID, moItem.Description, moItem.ITEM_DESC_HOMO, moItem.ITEM_SCOPE_NOTE)

        lsDesc = moItem.Description
        If moItem.ITEM_CODE.Length > 0 Then lsDesc = moItem.ITEM_CODE & " " & lsDesc
        If moItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & moItem.ITEM_DESC_HOMO & " ]"

        If moItem.ITEM_SCOPE_NOTE.Length > 0 Then
            lsSN = "(" & moItem.ITEM_SCOPE_NOTE & ")"
        End If

        plhDetail.Text = ""

        lsTable = "<TABLE>"
        lsTable &= vbCrLf & "<TR>"
        lsTable &= vbCrLf & "<TD>" & fsEncode(lsdesc) & "</TD>"
        lsTable &= vbCrLf & "<TD><i><font color=gray>" & fsEncode(lsSN) & "</font></i></TD>"
        lsTable &= vbCrLf & "</TR>"
        lsTable &= vbCrLf & "</TABLE>"

        plhDetail.Text = lsTable

    End Sub

    Private Sub sShowTranslations(ByVal vlListItemID As Integer)

        Dim loItems As Lists.ListItemLanguageList
        Dim loItem As Lists.ListItemLanguageList.ListItemLanguageInfo
        Dim loCrit As Lists.ListItemLanguageList.Criteria
        Dim lsTable As String = ""
        Dim lsDesc As String = ""
        Dim lsSN As String = ""

        plhTranslations.Text = ""

        loCrit = New Lists.ListItemLanguageList.Criteria
        loCrit.ITEM_ID = vlListItemID
        loItems = Lists.ListItemLanguageList.GetListItemLanguageList(loCrit)
        If loItems.Any Then
            lsTable = "<TABLE>"
            lsTable &= "<TR>"
            lsTable &= "<TD colspan=3><b>" & fsEncode("Translations") & "</b></TD>"
            lsTable &= "</TR>"
            For Each loItem In loItems
                lsDesc = loItem.ITEM_DESC
                If loItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & loItem.ITEM_DESC_HOMO & " ]"
                If loItem.ITEM_SCOPE_NOTE.Length > 0 Then
                    lsSN = "(" & loItem.ITEM_SCOPE_NOTE & ")"
                Else
                    lsSN = ""
                End If
                lsTable &= "<TR>"
                lsTable &= "<TD>" & fsEncode(lsDesc) & "</TD>"
                lsTable &= "<TD><i><font color=gray>" & fsEncode(lsSN) & "</font></i></TD>"
                lsTable &= "<TD>" & fsEncode(loItem.LANG_CODE) & "</TD>"
                lsTable &= "</TR>"
            Next
            lsTable &= "</TABLE>"
            plhTranslations.Text = lsTable
        End If

    End Sub

    Private Sub sShowSynonyms(ByVal vlListItemID As Integer)

        Dim loItems As Lists.ListItemSynList
        Dim loItem As Lists.ListItemSynList.ListItemSynInfo
        Dim loCrit As Lists.ListItemSynList.Criteria
        Dim lsTable As String = ""

        plhSynonyms.Text = ""

        loCrit = New Lists.ListItemSynList.Criteria
        loCrit.ITEM_ID = vlListItemID
        loItems = Lists.ListItemSynList.GetListItemSynList(loCrit)
        If loItems.Any Then
            lsTable = "<TABLE>"
            lsTable &= vbCrLf & "<TR>"
            lsTable &= vbCrLf & "<TD colspan=2><b>" & fsEncode("Synonyms") & "</b></TD>"
            lsTable &= vbCrLf & "</TR>"
            For Each loItem In loItems
                lsTable &= vbCrLf & "<TR>"
                lsTable &= vbCrLf & "<TD>" & fsEncode(loItem.SYN_DESC) & "</TD>"
                lsTable &= vbCrLf & "<TD>" & fsEncode(loItem.LANG_CODE) & "</TD>"
                lsTable &= vbCrLf & "</TR>"
            Next
            lsTable &= vbCrLf & "</TABLE>"
            plhSynonyms.Text = lsTable
        End If

    End Sub

    Private Sub sShowRelations(ByVal vlListItemID As Integer)

        Dim items1 As Lists.ListItemRelList

        Dim loItemCrit1 As Lists.ListItemRelList.Criteria

        Dim items2 As Lists.ListItemRelList

        Dim loItemCrit2 As Lists.ListItemRelList.Criteria

        Dim loRels As Lists.ListRelTypeList

        Dim loRelCrit As Lists.ListRelTypeList.Criteria

        Dim lsTable As String = ""
        Dim lsCode As String = ""
        Dim lsItem As String = ""
        Dim lsHomo As String = ""
        Dim lsRel As String = ""
        Dim lsDesc As String = ""

        loRelCrit = New Lists.ListRelTypeList.Criteria
        loRelCrit.LIST_ID = moList.ID
        loRels = Lists.ListRelTypeList.GetListRelTypeList(loRelCrit)

        plhRelations.Text = ""

        loItemCrit1 = New Lists.ListItemRelList.Criteria
        loItemCrit1.ITEM1_ID = vlListItemID
        loItemCrit1.LINK_DIRECT = 1
        items1 = Lists.ListItemRelList.GetListItemRelList(loItemCrit1)

        loItemCrit2 = New Lists.ListItemRelList.Criteria
        loItemCrit2.ITEM2_ID = vlListItemID
        loItemCrit2.LINK_DIRECT = 1
        items2 = Lists.ListItemRelList.GetListItemRelList(loItemCrit2)

        If items1.Any OrElse items2.Any Then

            lsTable = "<TABLE>"
            lsTable &= "<TR><TD colspan=3><b>" & fsEncode("Relationships") & "</b></TD></TR>"

            For Each loItem1 As Lists.ListItemRelList.ListItemRelInfo In items1
                GetItemDetails(loItem1.ITEM2_ID, lsCode, lsItem, lsHomo)
                If loItem1.LINK_TYPE = 1 Then
                    lsRel = "BT"
                Else
                    lsRel = "RT"
                End If
                For Each loRel As Lists.ListRelTypeList.ListRelTypeInfo In loRels
                    If loRel.REL_ID = loItem1.LINK_TYPE Then
                        lsRel = loRel.REL_DESC
                        Exit For
                    End If
                Next
                lsDesc = moItem.Description
                If moItem.ITEM_CODE.Length > 0 Then lsDesc = moItem.ITEM_CODE & " " & lsDesc
                If moItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & moItem.ITEM_DESC_HOMO & " ]"
                lsTable &= vbCrLf & "<TR>"
                lsTable &= vbCrLf & "<TD>" & fsEncode(lsDesc) & "</TD>"
                lsTable &= vbCrLf & "<TD>-&gt; " & fsEncode(lsRel) & " -&gt;</TD>"
                lsDesc = lsItem
                If lsCode.Length > 0 Then lsDesc = lsCode & " " & lsDesc
                If lsHomo.Length > 0 Then lsDesc &= " [" & lsHomo & " ]"
                lsTable &= vbCrLf & "<TD>" & fsEncode(lsDesc) & "</TD>"
                lsTable &= vbCrLf & "</TR>"
            Next


            For Each loItem2 As Lists.ListItemRelList.ListItemRelInfo In items2
                GetItemDetails(loItem2.ITEM1_ID, lsCode, lsItem, lsHomo)
                If loItem2.LINK_TYPE = 1 Then
                    lsRel = "BT"
                Else
                    lsRel = "RT"
                End If
                For Each loRel As Lists.ListRelTypeList.ListRelTypeInfo In loRels
                    If loRel.REL_ID = loItem2.LINK_TYPE Then
                        lsRel = loRel.REL_DESC
                        Exit For
                    End If
                Next
                lsDesc = lsItem
                If lsCode.Length > 0 Then lsDesc = lsCode & " " & lsDesc
                If lsHomo.Length > 0 Then lsDesc &= " [" & lsHomo & " ]"
                lsTable &= vbCrLf & "<TR>"
                lsTable &= vbCrLf & "<TD>" & fsEncode(lsDesc) & "</TD>"
                lsTable &= vbCrLf & "<TD>-&gt; " & fsEncode(lsRel) & " -&gt;</TD>"
                lsDesc = moItem.Description
                If moItem.ITEM_CODE.Length > 0 Then lsDesc = moItem.ITEM_CODE & " " & lsDesc
                If moItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & moItem.ITEM_DESC_HOMO & " ]"
                lsTable &= vbCrLf & "<TD>" & fsEncode(lsDesc) & "</TD>"
                lsTable &= vbCrLf & "</TR>"
            Next

            lsTable &= vbCrLf & "</TABLE>"
            plhRelations.Text = lsTable

        End If


    End Sub

    Private Function fsEncode(ByVal vsText As String) As String

        Return Server.HtmlEncode(vsText)

    End Function

    Private Sub GetItemDetails(ByVal vlItemID As Integer, _
                               ByRef rsCode As String, _
                               ByRef rsItem As String, _
                               ByRef rsHomo As String)

        Dim loItem As Lists.ListItem

        rsCode = ""
        rsItem = ""
        rsHomo = ""


        loItem = Lists.ListItem.GetListItem(vlItemID)
        If loItem.ID = vlItemID Then
            rsCode = loItem.ITEM_CODE
            rsItem = loItem.Description
            rsHomo = loItem.ITEM_DESC_HOMO
        End If
        

    End Sub

    Private Sub GetTrans(ByVal vlItemID As Integer, _
                   ByRef rsDesc As String, _
                   ByRef rsHomo As String, _
                   ByRef rsSN As String)

        ' This function has to be identical in DM_Catalog_Tree.aspx.
        ' This function has to be identical in DM_Catalog_TreeT.aspx.

        Dim loListItemLang As Lists.ListItemLanguage = Lists.ListItemLanguage.GetListItemLanguage(vlItemID, msUserLang)
        If loListItemLang.ITEM_ID = vlItemID And _
           loListItemLang.LANG_CODE = msUserLang Then
            rsDesc = loListItemLang.ITEM_DESC
            If loListItemLang.ITEM_DESC_HOMO.Length > 0 Then rsHomo = loListItemLang.ITEM_DESC_HOMO
            If loListItemLang.ITEM_SCOPE_NOTE.Length > 0 Then rsSN = loListItemLang.ITEM_SCOPE_NOTE
        End If
     
    End Sub

End Class
