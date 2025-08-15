
Partial Class UserCaseHistory
    Inherits BasePage
   
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        lblHistory.USER_ID = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Name
    End Sub    
End Class
