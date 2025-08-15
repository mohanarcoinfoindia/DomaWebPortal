Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Security

Public Class Auth_ResetPass
    Inherits BaseAnonymousPage


    Private _user As User
    Private _resetCode As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        'check if we can do local login
        If Settings.GetValue("Saml", "IsServiceProvider", False, GetTenantFromUrl().Name) OrElse OpenAuthProviderList.HasDefaultAndEnabledProvider() Then
            GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INVALIDOPERATION)
            Return
        End If

        _resetCode = QueryStringParser.GetCode("code")
        pnlChange.Visible = False
        SetRedirectUri()
        If Not String.IsNullOrEmpty(_resetCode) Then
            _user = ACL.User.GetUserByPasswordResetCode(_resetCode)
            If _user IsNot Nothing Then
                SetMessage("")
                SetLabels()

                pnlChange.Visible = True

            Else
                SetMessage("Unauthorized access")
            End If

        Else
            SetMessage("No code was provided")
            'Throw New HttpException(401, "Unauthorized access")
        End If

        txtPassword.Focus()
    End Sub
    Private Sub SetRedirectUri()
        If Not IsPostBack Then
            txtRedirectUri.Value = QueryStringParser.GetUrl("redirect_uri")
        End If
    End Sub
    Private Sub SetLabels()
        Language = _user.USER_LANGCODE

        lblHeader.Text = GetLabel("changepassword") & " : " & Server.HtmlEncode(_user.GetDisplayName)
        lblPassword.Text = GetLabel("password")
        lblConfirmPassword.Text = GetLabel("confirmpassword")
        lnkChange.Text = GetLabel("changepassword")
        CompareValidator1.ErrorMessage = GetLabel("passwordmatchfailed")

        Dim complexityValidator As PasswordComplexityValidator = PasswordComplexityValidator.GetValidator(Settings, Nothing)
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
            lblPasswordTooltipImg.ToolTip = "- " & String.Join(Environment.NewLine & "- ", rules)
        End If

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
        If txtPassword.Text.Equals(txtConfirmPassword.Text) Then

            Dim complexityValidator As PasswordComplexityValidator = PasswordComplexityValidator.GetValidator(Settings, _user.USER_LOGIN)

            'can't use current one
            If _user.PasswordStorage = ACL.User.PassWordStoredAs.Hashed Then
                complexityValidator.IllegalPasswordHashes.Add(_user.USER_PASS)
            End If

            Dim results As List(Of PasswordComplexityValidator.ResultCode) = complexityValidator.Validate(txtPassword.Text)
            If results.Any Then
                For Each result As PasswordComplexityValidator.ResultCode In results
                    Select Case result
                        Case PasswordComplexityValidator.ResultCode.MaxlengthFailed
                            SetMessage(GetLabel("password") & " " & GetLabel("maxlen").Replace("#FIELDLEN#", complexityValidator.MaxLength.ToString))
                        Case PasswordComplexityValidator.ResultCode.MinlengthFailed
                            SetMessage(GetLabel("password") & " " & GetLabel("minlen").Replace("#FIELDLEN#", complexityValidator.MinLength.ToString))
                        Case Else
                            SetMessage(GetLabel(result.ToString))
                    End Select
                Next
                Exit Sub
            End If


            BusinessPrincipal.Login(_user.USER_LOGIN, CacheToken.GetCacheToken(CreateSessionID), Arco.Licencing.Modules.WEB, UserInfoLoader.GetEndUserInfo(Me))
            Dim ident As BusinessIdentity = BusinessIdentity.CurrentIdentity

            If ident.IsAuthenticated Then
                _user.PasswordExpired = False
                _user.SetPasswordWithResetCode(txtPassword.Text, _resetCode)
                Try
                    _user = _user.Save
                Catch ex As Exception
                    SetMessage("Access denied")
                    Exit Sub
                End Try

                If ident.MustChangePassword Then
                    ident.MustChangePassword = False
                    ident.Save()
                End If
                Dim lsRedirect As String = txtRedirectUri.Value


                Dim handler As New AfterLoginHandler()
                handler.ContinueLogin(Me, Context, lsRedirect, True)


            Else
                'unexpected
                SetMessage("Access denied")
            End If

        Else
            SetMessage(GetLabel("passwordmatchfailed"))
        End If


    End Sub

End Class
