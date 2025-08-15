
Partial Class ToolWindow
    Inherits BaseMasterPage

    Private mWidth As Int32 = 400
    Public Property AutoSized() As Boolean

    Public ReadOnly Property EnableAdjustWindow As Boolean
        Get
            Return Modal
        End Get
    End Property

    Public ReadOnly Property Modal() As Boolean
        Get
            Return QueryStringParser.Modal
        End Get
    End Property
    Public Property Width() As Int32
        Get
            Return (mWidth - 40)
        End Get
        Set(ByVal value As Int32)
            mWidth = value
        End Set
    End Property
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        AutoSized = (Request("autosize") = "Y")
    End Sub
    Public ReadOnly Property Margin As String
        Get
            If Not Modal And Not AutoSized Then
                'Return "margin:10px"
                Return "margin:0px;height:100%"
            Else
                Return ""
            End If
        End Get
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        plhAutoSize.Visible = Me.AutoSized
        PC.ReloadFunction = Me.ReloadFunction

        If Modal Then
            DirectCast(Master, masterpages_Base).BodyTag.Attributes("class") = "domaBodyModal"
        End If
    End Sub

    Protected Overrides Sub InitializePageTitle()

    End Sub
End Class

