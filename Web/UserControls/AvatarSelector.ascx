<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AvatarSelector.ascx.cs" Inherits="VideoShow.UserControls.AvatarSelector" %>

<div id="avatar-selection">
    <input runat="server" type="hidden" id="SelectedAvatar" />
    <div id="avatar-prompt">Select an image to represent you when other members see your comments and videos.</div>
    <a class="previous" href="javascript:changeAvatar(-1)"></a>
    <img id="selected-avatar-image" alt="Avatar" src="../Images/Avatars/avatar_baseball.jpg" />
    <a class="next" href="javascript:changeAvatar(1)"></a>
</div>
<script type="text/javascript">
var avatarList = [
    "/App_Themes/Default/Images/Avatars/avatar_baseball.jpg",
    "/App_Themes/Default/Images/Avatars/avatar_boats.jpg",
    "/App_Themes/Default/Images/Avatars/avatar_butterfly.jpg",
    "/App_Themes/Default/Images/Avatars/avatar_feather.jpg",
    "/App_Themes/Default/Images/Avatars/avatar_flower.jpg",
    "/App_Themes/Default/Images/Avatars/avatar_leaf.jpg",
    "/App_Themes/Default/Images/Avatars/avatar_light.jpg",
    "/App_Themes/Default/Images/Avatars/avatar_silverlight.jpg",
    "/App_Themes/Default/Images/Avatars/avatar_strawberry.jpg",
    "/App_Themes/Default/Images/Avatars/avatar_sunset.jpg",
    "/App_Themes/Default/Images/Avatars/avatar_window.jpg"
]

var avatarIndex;
if($get("<%= GetSelectedAvatarControlId() %>") && $get("<%= GetSelectedAvatarControlId() %>").value != "")
{
    avatarIndex = parseInt($get("<%= GetSelectedAvatarControlId() %>").value) - 1;
}
else
{
    avatarIndex = Math.floor(Math.random()*(avatarList.length + 1));
}
changeAvatar(0);

function changeAvatar(skipIndex)
{
    avatarIndex += skipIndex;
    if(avatarIndex >= avatarList.length) 
    {
        avatarIndex = 0;
    }
    if(avatarIndex < 0) 
    {
        avatarIndex = avatarList.length - 1;
    }
    $get("selected-avatar-image").src = avatarList[avatarIndex];
    $get("<%= GetSelectedAvatarControlId() %>").value = avatarIndex + 1;
}

</script>
