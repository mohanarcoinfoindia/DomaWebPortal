
Public Class Stats
    Inherits BasePage


    Private Sub SetStatControlParameters(ByVal voStat As UserControls_StatControl, ByVal vsIndex As String)

        voStat.QueryParams = Request.Unvalidated.QueryString

        If QueryStringParser.Exists("REPORT" & vsIndex) Then
            voStat.ReportID = QueryStringParser.GetInt("REPORT" & vsIndex)
            'Else
            '    voStat.Title = QueryStringParser.GetString("TITLE" & vsIndex)
            '    voStat.Source = QueryStringParser.GetInt("SOURCE" & vsIndex)
            '    voStat.Filter = QueryStringParser.GetString("FILTER" & vsIndex)
            '    voStat.GroupBy = QueryStringParser.GetString("GROUPBY" & vsIndex)
            '    voStat.Total = QueryStringParser.GetString("TOTAL" & vsIndex)
            '    voStat.HideConfigurator = QueryStringParser.GetBoolean("HIDECONFIG" & vsIndex, False)
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Not Page.IsPostBack Then
            If QueryStringParser.Exists("REPORT") Then
                SetStatControlParameters(stat1, "")
            Else
                SetStatControlParameters(stat1, "1")
            End If

        End If

    End Sub

End Class
