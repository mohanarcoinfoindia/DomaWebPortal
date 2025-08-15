
Public Class UserBrowserBasePage
    Inherits BasePage

    Public Overrides Function IsAdmin() As Boolean
        Return (Not AdminShouldBeGlobalUSer OrElse Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal) AndAlso Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsUserAdmin
    End Function

    ''' <summary>
    ''' When set to true, on top of the isUserAdmin check there is also a check to see that the user is a global user
    ''' </summary>
    ''' <returns></returns>
    Public Property AdminShouldBeGlobalUser As Boolean

End Class

Public Class UserBrowserBaseUserControl
    Inherits BaseUserControl

    Public ReadOnly Property IsAdmin() As Boolean
        Get
            Return (Not AdminShouldBeGlobalUser OrElse Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal) AndAlso Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsUserAdmin
        End Get
    End Property
    ''' <summary>
    ''' When set to true, on top of the isUserAdmin check there is also a check to see that the user is a global user
    ''' </summary>
    ''' <returns></returns>
    Public Property AdminShouldBeGlobalUser As Boolean
End Class
