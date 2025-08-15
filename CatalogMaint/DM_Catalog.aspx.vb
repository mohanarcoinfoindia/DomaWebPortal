Imports Arco.Doma.Library

Partial Class DM_Catalog
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim rand As New Random

        Server.Transfer("DM_Catalog_LeftPane.aspx?rnd_str=" & rand.Next)
       
    End Sub

End Class
