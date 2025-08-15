<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DefaultUserPreferences.aspx.vb" Inherits="UserBrowser_DefaultUserPreferences" MasterPageFile="~/MasterPages/base.master" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server" EnableViewState="true">

       <asp:Label ID="lblMsgText" runat="server" Visible="false" />

     <div class="container-fluid detail-form-container">
     <div class="detailView">
         <div style="margin: 10px">
                    <table class="SubList">
                        <tr>
                            <th colspan="2"><%=GetLabel("general")%></th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="row detail-form-row level1">
                                    <div class="col my-2">
                                        <asp:Label ID="lblTheme" runat="server" Text="Theme"></asp:Label>&nbsp;
                                                <asp:DropDownList runat="server" ID="drpTheme" ></asp:DropDownList>

                                        <Arco:SecondaryButton runat="server" OnClientClick="return confirm('Are you sure you want to set the theme for all users?');" ID="lnkApplyThemeToAll" OnClick="doApplyThemeToAll"></Arco:SecondaryButton>
                                    </div>
                                </div>
                                <div class="row detail-form-row level1">
                                    <div class="col-md-6 col-lg-4">
                                        <ArcoControls:CheckBox runat="server" ID="chkShowTree" />
                                        <ArcoControls:CheckBox runat="server" ID="chkShowTreeReadOnly" Text="Read-Only" />
                                    </div>
                                    <div class="col-md-6 col-lg-8">
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWFOLDERLINKICONS" Text="Show folder link icons" />
                                         <ArcoControls:CheckBox runat="server" ID="chkSHOWFOLDERLINKICONSReadonly" Text="Read-Only" />
                                    </div>
                                </div>
                                <div class="row detail-form-row level1">
                                    <div class="col-md-6 col-lg-4">
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWGLOBALSEARCH" Text="Global search"  />
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWGLOBALSEARCHReadOnly" Text="Read-Only" />
                                    </div>
                                    <div class="col-md-6 col-lg-8">
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWSUCCESSMESSAGES" Text="Show sucess messages"  />
                                           <ArcoControls:CheckBox runat="server" ID="chkSHOWSUCCESSMESSAGESReadOnly" Text="Read-Only" />
                                    </div>
                                </div>

                                 <div class="row detail-form-row level1">
                                    <div class="col-md-6 col-lg-4">
                                        <ArcoControls:CheckBox runat="server" ID="chkEnableContextMenus" Text="Show Context menus"  />
                                        <ArcoControls:CheckBox runat="server" ID="chkEnableContextMenusReadOnly" Text="Read-Only" />
                                    </div>
                                  
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <th colspan="2"><%=GetLabel("defaultresultscreen") %></th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="row detail-form-row level1">
                                    <div class="col my-2">
                                        <asp:Label ID="lblRecsPerpage" runat="server"></asp:Label>&nbsp;
                                                <asp:TextBox SkinID="CustomWidth" runat="server" ID="txtRecsperPage" Width="50" ></asp:TextBox>
                                         <ArcoControls:CheckBox runat="server" ID="txtRecsperPageReadOnly" Text="Read-Only" />
                                    </div>
                                </div>
                                <div class="row detail-form-row level1">
                                    <div class="col-md-6 col-lg-4">
                                        <ArcoControls:CheckBox runat="server" ID="chkShowPreview" />
                                            <ArcoControls:CheckBox runat="server" ID="chkShowPreviewReadOnly" Text="Read-Only" />
                                    </div>
                                    <div class="col-md-6 col-lg-8">
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWGRIDSIDEBAR" />
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWGRIDSIDEBARReadOnly" Text="Read-Only" />
                                    </div>
                                </div>
                                <div class="row detail-form-row level1">
                                    <div class="col-md-6 col-lg-4">
                                        <ArcoControls:CheckBox runat="server" ID="chkShowFilters" />
                                        <ArcoControls:CheckBox runat="server" ID="chkShowFiltersReadOnly" Text="Read-Only" />
                                    </div>
                                    <div class="col-md-6 col-lg-8">
                                        <ArcoControls:CheckBox runat="server" ID="chkShowFoldersInList" />
                                        <ArcoControls:CheckBox runat="server" ID="chkShowFoldersInListReadOnly" Text="Read-Only" />
                                    </div>
                                </div>
                                <div class="row detail-form-row level1">
                                    <div class="col-md-6 col-lg-4">
                                        <ArcoControls:CheckBox runat="server" ID="chkShowCurrentFolder" />
                                        <ArcoControls:CheckBox runat="server" ID="chkShowCurrentFolderReadOnly" Text="Read-Only" />
                                    </div>
                                    <div class="col-md-6 col-lg-8">
                                        <ArcoControls:CheckBox runat="server" ID="chkShowQuery"  />
                                        <ArcoControls:CheckBox runat="server" ID="chkShowQueryReadOnly" Text="Read-Only" />
                                    </div>
                                </div>
                                <div class="row detail-form-row level1">
                                    <div class="col-md-6 col-lg-4">
                                        <ArcoControls:CheckBox runat="server" ID="chkShowSearchInList"  />
                                        <ArcoControls:CheckBox runat="server" ID="chkShowSearchInListReadOnly" Text="Read-Only" />
                                    </div>
                                    <div class="col-md-6 col-lg-8">
                                        <ArcoControls:CheckBox runat="server" ID="chkALWAYSSHOWATTACHMENTS"  />
                                        <ArcoControls:CheckBox runat="server" ID="chkALWAYSSHOWATTACHMENTSReadOnly" Text="Read-Only" />
                                    </div>
                                </div>
                            </td>
                        </tr>

                        <tr>
                            <th colspan="2"><%=GetLabel("details") %></th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="row detail-form-row level1 mt-2">
                                    <div class="col-md-6 col-lg-4">
                                        <ArcoControls:CheckBox runat="server" ID="chkOPENDETAILWINDOWMAXIMIZED" />
                                        <ArcoControls:CheckBox runat="server" ID="chkOPENDETAILWINDOWMAXIMIZEDReadONly" Text="Read-Only" />
                                    </div>
                                    <div class="col-md-6 col-lg-8">
                                        <ArcoControls:CheckBox runat="server" ID="chkAUTOOPENFILEINDETAIL" />
                                        <ArcoControls:CheckBox runat="server" ID="chkAUTOOPENFILEINDETAILReadOnly" Text="Read-Only" />
                                    </div>
                                </div>
                                <div class="row detail-form-row level1">
                                    <div class="col-md-6 col-lg-4">
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWFILESINSEPARATEWINDOW" />
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWFILESINSEPARATEWINDOWReadOnly" Text="Read-Only" />
                                    </div>
                                    <div class="col-md-6 col-lg-8">
                                        <ArcoControls:CheckBox runat="server" ID="chkShowPreviewInDetailWindows" />
                                        <ArcoControls:CheckBox runat="server" ID="chkShowPreviewInDetailWindowsReadOnly" Text="Read-Only" />
                                    </div>
                                </div>
                                <div class="row detail-form-row level1">
                                      <div class="col-md-6 col-lg-8">
                                        <ArcoControls:CheckBox runat="server" ID="chkShowFileToolbar" Text="Show file toolbar"  />
                                           <ArcoControls:CheckBox runat="server" ID="chkShowFileToolbarReadOnly" Text="Read-Only" />
                                    </div>
                                     <div class="col-md-6 col-lg-8">
                                         </div>
                                </div>
                                <div class="row detail-form-row level1">
                                    <div class="col mb-2">
                                        <asp:Label ID="lblOnCloseWindow" runat="server"></asp:Label>&nbsp;
                                                <asp:DropDownList runat="server" ID="drpOnCloseWindow" ></asp:DropDownList>
                                        <ArcoControls:CheckBox runat="server" ID="drpOnCloseWindowReadOnly" Text="Read-Only" />
                                    </div>
                                </div>
                            </td>
                        </tr>

                        <tr>
                            <th colspan="2"><%=GetLabel("case")%></th>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="row detail-form-row level1 mt-2">
                                    <div class="col-md-6 col-lg-4">
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWSUSPENDEDDOSSIERS" />
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWSUSPENDEDDOSSIERSReadOnly" Text="Read-Only" />
                                    </div>
                                    <div class="col-md-6 col-lg-8">
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWLOCKEDDOSSIERS"  />
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWLOCKEDDOSSIERSReadOnly" Text="Read-Only" />
                                    </div>
                                </div>
                                <div class="row detail-form-row level1">
                                    <div class="col-md-6 col-lg-4">
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWDELEGATEDDOSSIERS"  />
                                        <ArcoControls:CheckBox runat="server" ID="chkSHOWDELEGATEDDOSSIERSReadOnly" Text="Read-Only" />
                                    </div>
                                    <div class="col-md-6 col-lg-8">
                                        <ArcoControls:CheckBox runat="server" ID="chkOPENNEXTCASEONCLOSE"/>
                                        <ArcoControls:CheckBox runat="server" ID="chkOPENNEXTCASEONCLOSEReadOnly" Text="Read-Only" />
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>



                    <Arco:ButtonPanel runat="server" ID="btnPanel">
                        <Arco:OkButton runat="server" ID="lnkSave" OnClick="doSave" Text="Save"> </Arco:OkButton>                
                            <Arco:SecondaryButton runat="server" OnClientClick="return confirm('Are you sure you want to override all user preferences?');" ID="lnkApplyToAll" OnClick="doApplyToAll"></Arco:SecondaryButton>
                    </Arco:ButtonPanel>
           

         </div>
         </div>

</asp:Content>
