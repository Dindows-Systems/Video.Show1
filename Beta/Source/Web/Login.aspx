<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="True"
    CodeBehind="Login.aspx.cs" Inherits="Login" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
<!-- TODO:// Remove inline styles, fix design, etc. Basically need to redo this page. -->
    <title>Sign In</title>
    <style type="text/css">
        #content-frame
        {
            height:300px;	
        }
        #upload-body td em
        {
            font-style: normal;	
        }
        #upload-body  table
        {
        	width: 100%;
        }
        #upload-body table td input 
        {
            width: 300px;
        }
        div.AspNet-Login 
        {
        }
        div.AspNet-Login-TitlePanel
        {
        	display: none;
        }
        div.AspNet-Login-UserPanel, div.AspNet-Login-PasswordPanel, div.AspNet-Login-RememberMePanel
        {
        	margin-bottom: 10px;
        }
        div.AspNet-Login-UserPanel label, div.AspNet-Login-PasswordPanel label
        {
        	width: 140px;
        	display: block;
        	float: left;
        	text-align: right;
        	padding-right: 10px;
        }
        div#upload-body div.AspNet-Login div.AspNet-Login-RememberMePanel input
        {
        	width: 20px;
        	float: right;
        	margin-right: 440px;
        }
        div#upload-body div.AspNet-Login div.AspNet-Login-RememberMePanel label
        {
            float: left;
            padding-right: 2px;
            padding-top: 4px;
            width: 136px;
        }
        div.AspNet-Login-SubmitPanel
        {
            clear: both;	
        }
         div#upload-body div.AspNet-Login div.AspNet-Login-SubmitPanel input
         {
            width: auto;
            padding: 0px 8px;
            overflow: hidden;
            margin-left: 148px;	
         }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="contentPlaceHolderMain" runat="Server">
    <div id="video-upload">
        <div id="Upload">
        <h2>
            Sign In</h2>
        <div id="upload-top">
        </div>
        <div id="upload-body">
            <table>
                <tr>
                    <td>
                        <a id="CloseSignIn" href="#"><span>close</span></a>
                        <asp:LoginView ID="LoginView" runat="server">
                            <LoggedInTemplate>
                                <div>
                                    You are signed in as 
                                    <asp:LoginName ID="LoginName" runat="server" />
                                </div>
                                <div>
                                    <asp:LoginStatus ID="LoginStatus" LogoutText="Sign Out" runat="server" />
                                </div>
                            </LoggedInTemplate>
                            <AnonymousTemplate>
                                <asp:Panel ID="LoginPanel" runat="server" DefaultButton="Login$LoginButton">
                                    <asp:Login ID="Login" runat="server" RememberMeText="Remember me:" onloggedin="Login_LoggedIn" />
                            </asp:Panel>
                            </AnonymousTemplate>
                        </asp:LoginView>
                        <div class="clearFloat"></div>
                    </td>
                </tr>
            </table>
        </div>
        <div id="upload-bottom">
        </div>
        </div>
    </div>
</asp:Content>
