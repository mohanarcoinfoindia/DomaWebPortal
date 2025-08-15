using Arco.Doma.Library;
using System;
using System.Collections.Generic;

public partial class Help_CMS_FilterInfoPage :  Arco.Doma.WebControls.BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            listRepeater.DataSource = GetTags();
            listRepeater.DataBind();
        }
    }

    private List<string> GetTags()
    {
        var tags = TagReplacer.GetTags();
        var tagList = new List<string>();

        foreach (var tag in tags)
        {
            tagList.Add(tag.Tag);
        }
        return tagList;
    }
}