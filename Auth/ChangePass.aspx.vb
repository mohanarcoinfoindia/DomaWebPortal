Imports Arco.Doma.Library
Imports Arco.Doma.Library.Security
Imports Telerik.Web.UI

Public Class Auth_ChangePass
    Inherits BaseAfterLoginPage

    Private complexityValidator As PasswordComplexityValidator

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        complexityValidator = PasswordComplexityValidator.GetValidator(Settings, BusinessIdentity.CurrentIdentity.Name)


        SetMessage("")
        SetLabels()

        SetReturnTo()


        lnkChange.Focus()
    End Sub

    Private Sub SetReturnTo()
        If Not Page.IsPostBack Then
            txtOldPassword.Focus()
            txtReturnTo.Value = QueryStringParser.GetUrl("redirect_uri")
        End If
    End Sub
    Private Sub SetLabels()
        lblHeader.Text = GetLabel("changepassword") & " : " & ArcoFormatting.FormatUserName(Arco.Security.BusinessIdentity.CurrentIdentity.Name)
        lblPassword.Text = GetLabel("password")


        Dim ruleCodes As List(Of PasswordComplexityValidator.ResultCode) = complexityValidator.GetRuleCodes()
        If ruleCodes.Any Then
            Dim rules As New List(Of String)
            For Each ruleCode As PasswordComplexityValidator.ResultCode In ruleCodes
                Select Case ruleCode
                    Case PasswordComplexityValidator.ResultCode.MaxlengthFailed
                        rules.Add(GetDecodedLabel("maxlen").Replace("#FIELDLEN#", complexityValidator.MaxLength))
                    Case PasswordComplexityValidator.ResultCode.MinlengthFailed
                        rules.Add(GetDecodedLabel("minlen").Replace("#FIELDLEN#", complexityValidator.MinLength))
                    Case Else
                        rules.Add(GetDecodedLabel(ruleCode.ToString))
                End Select
            Next

            lblPassword.ToolTip = "- " & String.Join(Environment.NewLine & "- ", rules)
            lblConfirmPassword.ToolTip = "- " & String.Join(Environment.NewLine & "- ", rules)
        End If

        lblOldPassword.Text = GetLabel("oldpassword")
        lblConfirmPassword.Text = GetLabel("confirmpassword")
        lnkChange.Text = GetLabel("changepassword")
        CompareValidator1.ErrorMessage = GetLabel("passwordmatchfailed")
    End Sub
    Private Sub SetMessage(ByVal vsMsg As String)
        If Not String.IsNullOrEmpty(vsMsg) Then
            pnlMsg.Visible = True
            If Not String.IsNullOrEmpty(lblMsg.Text) Then
                lblMsg.Text &= "<br/>" & vsMsg
            Else
                lblMsg.Text = vsMsg
            End If
        Else
            lblMsg.Text = ""
            pnlMsg.Visible = False
        End If
    End Sub
    Protected Sub lnkChange_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkChange.Click
        If Not Page.IsValid Then
            Exit Sub
        End If

        'can't use current one
        complexityValidator.IllegalPasswords.Add(txtOldPassword.Text)

        Dim results As List(Of PasswordComplexityValidator.ResultCode) = complexityValidator.Validate(txtPassword.Text)
        If results.Any Then
            For Each result As PasswordComplexityValidator.ResultCode In results
                Select Case result
                    Case PasswordComplexityValidator.ResultCode.MaxlengthFailed
                        SetMessage(GetLabel("password") & " " & GetLabel("maxlen").Replace("#FIELDLEN#", complexityValidator.MaxLength))
                    Case PasswordComplexityValidator.ResultCode.MinlengthFailed
                        SetMessage(GetLabel("password") & " " & GetLabel("minlen").Replace("#FIELDLEN#", complexityValidator.MinLength))
                    Case Else
                        SetMessage(GetLabel(result.ToString))
                End Select
            Next
            Exit Sub
        End If

        Dim identity As BusinessIdentity = BusinessIdentity.CurrentIdentity
        Dim loUser As ACL.User = ACL.User.GetUser(identity.Name, False, True)
        If loUser.MatchPassword(txtOldPassword.Text) Then
            If txtPassword.Text.Equals(txtConfirmPassword.Text) Then
                loUser.PasswordExpired = False
                loUser.SetPassword(txtPassword.Text)
                loUser = loUser.Save

                If identity.MustChangePassword Then
                    identity.MustChangePassword = False
                    identity.Save()
                End If

                ContinueLogin(txtReturnTo.Value, False)

            Else
                SetMessage(GetLabel("passwordmatchfailed"))
            End If
        Else
            SetMessage(GetLabel("invalidoldpassword"))
        End If
    End Sub
End Class
