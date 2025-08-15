<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_FileTypes.aspx.vb" Inherits="DM_FileTypes" MasterPageFile="~/masterpages/Empty.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <script type="text/javascript">
      
        function Refresh() {
            currentLoadingPanel = $find("<%= radAjxLoadingPanel.ClientID%>");
            currentLoadingPanel.show("<%=GridView1.ClientId%>");
            document.forms[0].submit();
        }
    </script>
    <asp:Label ID="lblMessage" runat="server" CssClass="InfoMessage"></asp:Label>
 
    <div class="detail-form-container mb-2 ml-2">
        <div class="row detail-form-row">
            <div class="col-auto LabelCell">
                <asp:Label runat="server" ID="lblFilter" CssClass="Label" AssociatedControlID="ddCatFilter"></asp:Label>
            </div>
            <div class="col-auto FieldCell pl-0">
                <asp:DropDownList ID="ddCatFilter" runat="server" AutoPostBack="true"></asp:DropDownList>
            </div>
        </div>
    </div>
    <telerik:RadAjaxLoadingPanel ID="radAjxLoadingPanel" runat="server" EnableEmbeddedSkins="true"></telerik:RadAjaxLoadingPanel>
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSource1" HorizontalAlign="Center" SkinID="StickyHeadersGrid"
        EnableViewState="true">
        <HeaderStyle CssClass="ListHeader" />
        <Columns>
            <asp:ImageField ItemStyle-Wrap="true" ReadOnly="true"></asp:ImageField>
            <asp:BoundField DataField="Extension" HeaderText="Extension" SortExpression="Extension" ReadOnly="true">
                <ItemStyle Wrap="true" />
            </asp:BoundField>
            <asp:BoundField DataField="CatID" ReadOnly="true" >
                <ItemStyle Wrap="true" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Category">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblCat"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField DataField="AllowDownloads" HeaderText="Download Allowed">
                <ItemStyle Wrap="True" />
            </asp:CheckBoxField>
            <asp:CheckBoxField DataField="FILE_MANUAL_UPLOAD" HeaderText="Manual Upload Allowed">
                <ItemStyle Wrap="True" />
            </asp:CheckBoxField>
            <asp:CheckBoxField DataField="FILE_CANEXTRACTFULLTEXT" HeaderText="Fulltext">
                <ItemStyle Wrap="True" />
            </asp:CheckBoxField>
            <asp:CheckBoxField DataField="FILE_EXTRACTKEYWORDS" HeaderText="Keywords">
                <ItemStyle Wrap="false" />
            </asp:CheckBoxField>
            <asp:CheckBoxField DataField="FILE_ABSTRACT" HeaderText="Abstract">
                <ItemStyle Wrap="false" />
            </asp:CheckBoxField>
            <asp:CheckBoxField DataField="DetectLanguage" HeaderText="Detect Language">
                <ItemStyle Wrap="false" />
            </asp:CheckBoxField>
            <asp:CheckBoxField DataField="FILE_OCR" HeaderText="OCR">
                <ItemStyle Wrap="True" />
            </asp:CheckBoxField>

            <asp:TemplateField HeaderText="After Ocr">
              
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Bind("OcrBeheaviour") %>' ID="lblOcrBeheaviour"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Editing">
               
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Bind("EditMode") %>' ID="lblEditMode"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="On Editing">
                
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Bind("EditBeheaviour") %>' ID="lblEditBeheaviour"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField DataField="FILE_CONVERTTOPDF" HeaderText="PDF">
                <ItemStyle Wrap="false" />
            </asp:CheckBoxField>
            <asp:CheckBoxField DataField="FILE_CONVERTTOHTML" HeaderText="Html">
                <ItemStyle Wrap="false" />
            </asp:CheckBoxField>
            <asp:CheckBoxField DataField="FILE_GENERATETHUMBNAIL" HeaderText="Thumbnail">
                <ItemStyle Wrap="false" />
            </asp:CheckBoxField>           
            <asp:CheckBoxField DataField="FILE_CONVERTTOMAIL" HeaderText="Mail">
                <ItemStyle Wrap="false" />
            </asp:CheckBoxField>

            <asp:CheckBoxField DataField="FILE_ASYNCINDEXING" HeaderText="Asynchronous indexing">
                <ItemStyle Wrap="false" />
            </asp:CheckBoxField>
            <asp:TemplateField HeaderText="Preview">              
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Bind("Preview_Rendition") %>' ID="lblPrevRend"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

    </asp:GridView>

    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetFileTypeList"
        TypeName="Arco.Doma.Library.baseObjects.DM_FileTypeList" OldValuesParameterFormatString="original_{0}">
        <SelectParameters>
            <asp:Parameter Name="orderby" DbType="String" />
             
            <asp:ControlParameter Name="filter" ControlID="ddCatFilter" DbType="String" />
           
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
