Imports System.Web.Services
Imports Arco.Doma.Library.Lists

<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class WSManagedLists1

    Inherits System.Web.Services.WebService


    <WebMethod(EnableSession:=True)>
    Public Function switchCategorySelection(ByVal vlItemID As Integer, ByVal vlCatID As Integer, ByVal vbChecked As Boolean) As Boolean

        If vbChecked Then
            If Not ListItemCatList.IsItemLinkedToCat(vlItemID, vlCatID) Then
                Dim catLink As ListItemCat = ListItemCat.NewListItemCat(vlItemID, vlCatID)
                catLink.Save()
            End If
        Else
            If ListItemCatList.IsItemLinkedToCat(vlItemID, vlCatID) Then
                ListItemCat.DeleteListItemCat(vlItemID, vlCatID)
            End If
        End If
        Return True

    End Function

End Class