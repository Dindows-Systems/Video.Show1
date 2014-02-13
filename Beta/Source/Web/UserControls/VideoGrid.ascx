<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VideoGrid.ascx.cs" Inherits="VideoShow.UserControls.VideoGrid" %>
    <asp:ScriptManagerProxy runat="server" ID="VideoGridScriptManagerProxy">
        <Scripts>
            <asp:ScriptReference Path="~/Silverlight/VideoGrid/VideoGrid.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <div id="<%= GridName %>" class="VideoGrid"><%= GetStartupScript() %>
    <script type="text/javascript">
        var PostInstallGuidance = document.getElementById('PostInstallGuidance');

        if ( document.getElementById('PostInstallGuidance') )
        {
            if ( Silverlight.ua.Browser == "MSIE" ) {
                if ( Silverlight.available ) {
                    PostInstallGuidance.innerHTML="<p>When installation is complete, restart your browser to activate your Silverlight content.</p>";
                }
                else {
                    PostInstallGuidance.innerHTML= "";
                }
            }
            else if ( Silverlight.ua.Browser == "Firefox" || Silverlight.ua.Browser == "Safari") {
                PostInstallGuidance.innerHTML="<p>Your browser is "+ Silverlight.ua.Browser + ". When installation is complete,<br />restart your browser to activate your Silverlight content.</p>";
            }
            else {
                PostInstallGuidance.innerHTML="<p>Your browser may not be supported by Microsoft Silverlight.<br />Please visit http://www.microsoft.com/silverlight/system-requirements.aspx for more information.</p>";
            }
        }
    </script>

    </div>

    
