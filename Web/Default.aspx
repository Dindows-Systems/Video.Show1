<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Src="UserControls/VideoGrid.ascx" TagName="VideoGrid" TagPrefix="uc1" %>
<%@ Register Src="UserControls/Login.ascx" TagName="Login" TagPrefix="uc1" %>
<%@ Register Src="UserControls/Signup.ascx" TagName="Signup" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Page-Enter" content="blendTrans(Duration=0.1)" />
    <!-- Prevent IE Flicker -->
    <link rel="shortcut icon" href="favicon.ico" />
    <title>Video.Show</title>
    <script type="text/javascript" src="http://agappdom.net/h/silverlight.js"></script>
    <script type="text/javascript" src="Javascript/siteMenus.js"></script>

    <script type="text/javascript">
            function setDefaultMenu() {
        	if (typeof whichMenu == "undefined") {
        		var whichMenu = 0;
        	}
            var menuDiv = document.getElementById("menu-nav");
            var menu = menuDiv.getElementsByTagName("ul")[0]
            var menuItems = menu.getElementsByTagName("li");
            menuItems[0].className += " selected"
        };
        siteMenu.loadEvent(setDefaultMenu);
    </script>
    <script type="text/javascript">
        siteMenu.loadEvent(siteMenu.setHoverMenu);
    </script>
    <!-- This replaces the FireDefaultEvent with a version which works cross browser -->
    <script type="text/javascript">
        function MainForm_FireDefaultButton(event, target) {
        if (!__defaultFired && event.keyCode == 13 && !(event.srcElement && (event.srcElement.tagName.toLowerCase() == "textarea"))) {
            var defaultButton;
            if (__nonMSDOMBrowser)
                defaultButton = document.getElementById(target);
            else
                defaultButton = document.all[target];

            if (typeof(defaultButton.click) != "undefined") {
                __defaultFired = true;
                defaultButton.click();
                event.cancelBubble = true;
                
                if (event.stopPropagation) event.stopPropagation();
                return false;
            }

            if (typeof(defaultButton.href) != "undefined") {
                __defaultFired = true;
                eval(defaultButton.href.substr(11));
                event.cancelBubble = true;
                
                if (event.stopPropagation) event.stopPropagation();
                return false;
            }

        }
        return true;
    }
    </script>
    <!--[if IE 7]>
        <link runat="server" rel="stylesheet" href="~/CSS/DefaultStyle-IE7.css" />
    <![endif]-->
        <!--[if lt IE 7]>
        <link runat="server" rel="stylesheet" href="~/CSS/DefaultStyle-IE6.css" />
    <![endif]-->
    <!--[if lt IE 7]>
        <script defer type="text/javascript" src="Javascript/pngfix.js"></script>
    <![endif]-->
</head>
<body class="homepage">
    <form id="MainForm" runat="server" defaultbutton="Search" defaultfocus="SearchText">
    <!-- TODO: Move services to applicable pages and use asp:ScriptManagerProxy so they aren't loaded unnecessariliy -->
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
        <Services>
            <asp:ServiceReference Path="~/Webservices/VideoWebservice.asmx" />
            <asp:ServiceReference Path="~/Webservices/TagWebservice.asmx" />
            <asp:ServiceReference Path="~/Webservices/CommentWebservice.asmx" />
            <asp:ServiceReference Path="~/Webservices/FavoriteWebservice.asmx" />
        </Services>
    </asp:ScriptManager>
    <div id="frame">
        <div id="main">
            <div id="masthead">
                <h1 id="logo">
                    <a href="Default.aspx" title="Home">Welcome to Video.Show </a>
                </h1>
                <a id="byVertigo" href="http://www.vertigo.com">Created by Vertigo</a>
                <div class="clearFloat">
                </div>
                <div id="menu-nav">
                    <asp:Menu runat="server" ID="MainMenu" Orientation="Horizontal">
                        <StaticSelectedStyle CssClass="MenuItemSelected" />
                        <Items>
                            <asp:MenuItem Text="Home" NavigateUrl="~/Default.aspx"></asp:MenuItem>
                            <asp:MenuItem Text="Videos" NavigateUrl="~/Tags.aspx"></asp:MenuItem>
                            <asp:MenuItem Text="Members" NavigateUrl="~/MemberList.aspx"></asp:MenuItem>
                        </Items>
                    </asp:Menu>
                    <div class="clearFloat">
                    </div>
                </div>
            </div>
            <div class="clearFloat">
            </div>
            <div id="content-frame">
            <div id="content">
                <div id="site-search">
                    <label>
                        Search Tags</label>
                    <asp:TextBox runat="server" ID="SearchTags" />
                    <asp:LinkButton CssClass="go-search" Text="Go" runat="server" ID="Search" OnClick="Search_Click"></asp:LinkButton>
                </div>
                <div id="tag-cloud">
                    <h2><asp:Literal id="PageHeading" runat="server">Get comfy. Browse our videos...</asp:Literal></h2>
                    <p id="TagCloud" runat="server"><span>Popular tags: </span> <%= GetTagCloud() %></p>
                </div>
                <div class="clearFloat"></div>
                <uc1:VideoGrid ID="HomeVideoGrid" runat="server" />
        <div id="login">
            <asp:LoginView ID="LoginView" runat="server">
                <LoggedInTemplate>
                    <div id="logged-in-user">
                        <span id="welcome">Welcome <asp:LoginName ID="LoginName1" runat="server" />
                    </span><span class="divider">|</span>
                        <a id="A1" runat="server" href="MemberProfile.aspx">Account</a>
                    </span><span class="divider">|</span>
                        <asp:LoginStatus ID="LoginStatus1" LogoutText="Sign Out" runat="server" />
                    </div>
                </LoggedInTemplate>
                <AnonymousTemplate>
                <div id="home-login-panel">
                    <div id="home-login-body">
                        <p>Why not add something of your own to the community?</p>
                        <p>Come on - you know you want to share!</p>
                        <p>&nbsp;</p>
                        <p id="already-member">Already a member?</p>
                        <style type="text/css">
                            .SignupDialog { padding: 20px 20px 20px 20px; }
                        </style>
                    </div>
                </div>
                    <div style="display: block;" id="anonymous-user">
                        <uc1:Login ID="Login" runat="server" />
                        <span id="sign-in-or-sign-up">or</span>
                        <uc1:Signup ID="Signup" LinkText="Upload your first video!" runat="server" />
                    </div>
                </AnonymousTemplate>
            </asp:LoginView>
        </div>
            </div></div>
            <div id="frame-bottom">
                <div id="frame-corner-left">
                </div>
                <div id="frame-corner-right">
                </div>
                <div id="frame-bottom-body">
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
