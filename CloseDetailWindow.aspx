<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CloseDetailWindow.aspx.vb" Inherits="CloseDetailWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">    
    <title>Closing Window...</title>    
</head>
<body>
    <form id="form1" runat="server">
    
    <script type="text/javascript">
         <% If PersistSize Then %>
        function PersistDetailWindowSize() {

            if (!window.opener) {
                return;
            }

            var CookieContent;
            var winW = window.innerWidth; // document.body.offsetWidth - 4;
            var winH = window.innerHeight; // document.body.offsetHeight - 4;
            var winL = window.screenLeft - 4;
            var winT = window.screenTop - 23;
            var WindowName = window.name;
            var oneYear = 7 * 24 * 60 * 60 * 52000;
            var expDate = new Date();
            expDate.setTime(expDate.getTime() + oneYear);

            if (window.innerWidth) {
                //FF
                winW = window.innerWidth; // document.body.offsetWidth - 4;
                winH = window.innerHeight; // document.body.offsetHeight - 4;
                winL = window.screenX;
                winT = window.screenY;                            
            }
            else {
                //IE
                winW = document.body.offsetWidth - 4;
                winH = document.body.offsetHeight - 4;
                winL = window.screenLeft - 4;
                winT = window.screenTop - 23;
            }
            
            var CookieName = 'DMDETSIZE';
            CookieContent = '_l:' + escape(winL) + '|_t:' + escape(winT);
            CookieContent += '|_h:' + escape(winH) + '|_w:' + escape(winW) + '; expires=' + expDate.toGMTString();

            document.cookie = CookieName + '=' + CookieContent;           
        }
        <%end if %>
        function CloseMe() {
            if (window.opener) {
                self.close();
                return;
            }
            if (window.parent && window.parent.opener) {
                window.parent.close();
                return;
            }
            self.close();
        }
        function onStartup() {
     
            <%if PersistSize Then %>              
            PersistDetailWindowSize();
            <%end if %>
            if (self.opener) {
                try {
                    try {
                        self.opener.parent.CloseDetail(false);
                    }
                    catch (e) {
                    }                  
                    self.opener.PC().ReloadTree(<%= _GoToTreeId %>);
                    self.opener.PC().CascadeReloadContent();
                }
                catch (e) {
                }                
            }            
            CloseMe();
        }

        window.onload = onStartup;
    </script>

    </form>
</body>
</html>
