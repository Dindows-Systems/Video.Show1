<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="InstallHelp.aspx.cs" Inherits="InstallHelp" MasterPageFile="~/MasterPage.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
    <title>Install Help</title>
    <style type="text/css">
        #frame #main #content-frame
        {
            background: none;	
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentPlaceHolderMain">
    <div id="video-upload">
    <div id="Upload">
    <h2>Upload Videos</h2>
    <div id="upload-top"></div>
    <div id="upload-body">
        Thank you for installing Video.Show. In order to allow uploading files, you'll need to set up a free account with 
        <a target="_blank" href="http://silverlight.live.com">Silverlight Streaming by by Windows Live.</a> You'll also need to install Expression Encoder. Once you've got those set up, the Silverlight Streaming Account Management page will give you an Account ID and and an Account Key. 
        <br />
        <br />
        Set these up on your web.config file:
        
        <br />
        &lt;appSettings&gt;
		<br />
&nbsp; &lt;add key="AccountId" value="12345"/&gt;
		<br />
&nbsp; &lt;add key="AccountKey" value="12345678901234567890123456789012"/&gt;
        <br />
&nbsp; &lt;add key="MediaEncoderPath" value ="C:\Program Files\Microsoft Expression\Encoder 1.0\"/&gt;
        <br />
        &lt;/appSettings&gt;
        <br />
        <br />
        You&#39;ll also need to make sure that you have SQL Server Express installed, and 
        that your web account has access to /App_Data/VideoShow.mdf.</div>
        <div id="upload-bottom"></div>
        <div></div></div>
    </div>
</asp:Content>