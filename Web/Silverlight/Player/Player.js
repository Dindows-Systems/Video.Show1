///////////////////////////////////////////////////////////////////////////////
//
//  PlayerControl
//
//  Extends Expression Media's player template classes by adding time-based
//  commenting to the video playing experience.
//
///////////////////////////////////////////////////////////////////////////////

function PlayerControl(parentId) {
    this._hostname = EePlayer.Player._getUniqueName("xamlHost");
    Silverlight.createHostedObjectEx({
        source: 'Silverlight/Player/Player.xaml?v=1',
        parentElement: $get(parentId || "Player_SilverlightContainer"),
        id: this._hostname,
        properties: {
            width: '900',
            height: '412',
            framerate: '24',
            version: '1.0',
            background: 'transparent',
            isWindowless: 'true'
        },
        events: {
            onLoad: Function.createDelegate(this, this.onLoad)
            },
        initParams: videoUrl + ',' + videoId
    });
    //These are set in Player.ascx.cs
    this._currentMediainfo = 0;
}

PlayerControl.prototype = {

    _control: null,
    _content: null,
    _rootElement: null,
    _mediaOpened: false,

    _fontsDownloaded: false,
    _fonts: null,

    _dbComments: null,
    _commentsCreated: false,
    _numComments: 0,
    _currentMarkerIndex: 0,

    _VideoTimeInSeconds: 0.0,
    _CommentText: null,
    _AddCommentMode: false,

    _favoriteStatus: false,

    _timelineMarkerTemplate: '<TimelineMarker xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" x:Name="CommentMarker$id" Type="$id" Text="" Time="0:0:$seconds"/>',
    _commentBlockTemplate: '<Canvas xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" x:Name="Comment_$id" Canvas.Left="15" Width="313"><Image x:Name="CommentAvatar_$id" Height="16" Width="16" Opacity="0.5"/><TextBlock x:Name="CommentOwner_$id" Canvas.Left="26" Canvas.Top="5" Width="287" FontSize="11" Foreground="Gray" /><TextBlock x:Name="CommentText_$id" Canvas.Left="26" Canvas.Top="23" Width="287" FontSize="12" Foreground="Gray" TextWrapping="Wrap" FontStyle="Italic" /></Canvas>',
    _commentBlockIntroTemplate: '<Canvas xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" x:Name="Comment_$id" Canvas.Left="15" Width="313"><Image x:Name="CommentAvatar_$id" Height="0" Width="0" Visibility="Collapsed" /><TextBlock x:Name="CommentOwner_$id" Height="0" Width="0" Visibility="Collapsed"/><TextBlock x:Name="CommentText_$id" Canvas.Left="26" Width="287" FontSize="12" Foreground="Gray" TextWrapping="Wrap" /></Canvas>',

    /*
    onLoad

    Handles the Silverlight onLoad event.
    Creates the ExtendedPlayer video player and calls back to services for video info.
    */
    onLoad: function(control, userContext, rootElement) {
        this._control = control;
        this._content = control.content;
        this._rootElement = rootElement;
        this._player = $create(ExtendedPlayer.Player, {
            // properties
            autoPlay: true,
            volume: 1.0,
            muted: false
        }, {
            // event handlers
            mediaEnded: Function.createDelegate(this, this.onMediaEnded),
            mediaFailed: Function.createDelegate(this, this.onMediaFailed),
            stateChanged: Function.createDelegate(this, this.onStateChanged),
            markerReached: Function.createDelegate(this, this.onMarkerReached),
            mediaOpened: Function.createDelegate(this, this.onMediaOpened)
            }, null, $get(this._hostname));

        var params = $get(this._hostname).InitParams.split(",");
        this._player.set_mediainfo({
            "mediaUrl": params[0],
            "placeholderImage": "",
            "chapters": []
            });
        var videoId = params[1];
        VideoShow.VideoWebservice.GetVideoDetails(videoId, this.createDelegate(this, this.onVideoDetailsReceived));
        VideoShow.TagWebservice.GetTagsForVideo(videoId, this.createDelegate(this, this.onVideoTagsReceived));
        VideoShow.CommentWebservice.GetComments(videoId, this.createDelegate(this, this.onVideoCommentsReceived));

        if ($get("ToggleFavoriteLink")) {
            this.getFavoriteStatus();
        }

        /*
        var fontDownloader = control.CreateObject('downloader');
        fontDownloader.addEventListener('completed', this.createDelegate(this, this.onFontsDownloaded));
        fontDownloader.open('GET', '/Silverlight/Fonts/Fonts.zip');
        fontDownloader.send();
        */

        this._rootElement.findName('HidePagingControls').addEventListener('completed', this.createDelegate(this, this.OnHidePagingComplete));
        this._rootElement.findName('PagePrevious').addEventListener('mouseEnter', this.createDelegate(this, this.OnPagePreviousMouseEnter));
        this._rootElement.findName('PagePrevious').addEventListener('mouseLeave', this.createDelegate(this, this.OnPagePreviousMouseLeave));
        this._rootElement.findName('PagePrevious').addEventListener('mouseLeftButtonUp', this.createDelegate(this, this.OnPagePrevious));
        this._rootElement.findName('PageNext').addEventListener('mouseEnter', this.createDelegate(this, this.OnPageNextMouseEnter));
        this._rootElement.findName('PageNext').addEventListener('mouseLeave', this.createDelegate(this, this.OnPageNextMouseLeave));
        this._rootElement.findName('PageNext').addEventListener('mouseLeftButtonUp', this.createDelegate(this, this.OnPageNext));
    },

    /*
    onFontsDownloaded, onVideoDetailsReceived, onVideoTagsReceived, onVideoCommentsReceived

    These functions handle the receipt of downloaded elements.
    */
    onFontsDownloaded: function(sender, eventArgs) {
        this._fonts = sender;
        this._fontsDownloaded = true;
        this.SetFonts();
    },

    onVideoDetailsReceived: function(videoDetails) {
        $get("VideoDescription").innerText = videoDetails.description;
    },

    onVideoTagsReceived: function(videoTags) {
        var tagHtml = '';
        for (var i = 0; i < videoTags.length; i++) {
            tagHtml += "<a href='Tags.aspx?Tag=" + videoTags[i] + "'>" + videoTags[i] + "</a>";
        }
        $get("VideoTags").innerHTML = "<span id=\"tag-title\">Tags: </span>" + tagHtml;
    },

    onVideoCommentsReceived: function(dbComments) {
        this._dbComments = dbComments;

        this.addDbComments();
    },

    /*
    onMediaOpened and addDbComments

    Adds comment markers to the video after the media has been opened. 
    Markers added before the media is opened will not register.
    
    Since the addDbComments function cannot run until both comments have been
    received from a service call and the media is opened, both onMediaOpened
    and onVideoCommentsReceived call it.  It runs only on the second call 
    (whichever is last)
    */
    onMediaOpened: function(sender, eventArgs) {
        this._mediaOpened = true;
        this.addDbComments();
    },

    addDbComments: function() {
        if (this._dbComments !== null && this._mediaOpened) {
            var introText;
            if (this._dbComments.length === 0) {
                introText = 'No comments for this video yet...';
            } else {
                introText = 'Comments:';
            }
            this.insertComment(introText, '', '', 0, true);

            for (var i = 0; i < this._dbComments.length; i++) {
                var info = this._dbComments[i];
                this.insertComment(info.CommentText, info.Username, info.AvatarUrl, info.VideoTimeInSeconds, false);
            }
            this._commentsCreated = true;

            this.resetCommentWindow();
            this.SetPagingVisibility();
            this.SetFonts();
        }
    },

    /*
    insertComment

    Inserts a comment into the comment window and adds a comment marker to the video.
    Note: this function does not attempt to place the comment spatially within the window.
    Spacing is done only after all comments are added.  This is done in order to easily reset 
    the layout after adding a comment dynamically.
    
    The isIntro parameter specifies whether the comment is the "intro" comment--a 
    static marker for the beginning of the comment list.
    */
    insertComment: function(text, ownerName, avatarUrl, timeInSeconds, isIntro) {
        var timelineMarkerXaml;
        var textBlockXaml;
        var marker;
        var commentBlock;

        var commentsCanvas = this._rootElement.findName('CommentsCanvas');
        var video = this._player._mediaElement;

        var commentId = this._numComments++;

        timelineMarkerXaml = this._timelineMarkerTemplate.replace(/\$id/g, commentId);
        timelineMarkerXaml = timelineMarkerXaml.replace(/\$seconds/g, timeInSeconds);
        commentBlockXaml = isIntro ? this._commentBlockIntroTemplate: this._commentBlockTemplate;
        commentBlockXaml = commentBlockXaml.replace(/\$id/g, commentId);

        marker = this._content.createFromXaml(timelineMarkerXaml);
        commentBlock = this._content.createFromXaml(commentBlockXaml);

        var commentText = commentBlock.findName('CommentText_' + commentId);
        var commentAvatar = commentBlock.findName('CommentAvatar_' + commentId);
        var commentOwner = commentBlock.findName('CommentOwner_' + commentId);

        commentText.Text = text;
        commentAvatar.Source = avatarUrl;
        commentOwner.Text = ownerName + ' added:';
        commentBlock.Height = commentText.ActualHeight + commentText['Canvas.Top'];

        commentsCanvas.children.add(commentBlock);

        var markerIndex = video.markers.add(marker);
        if (markerIndex < this._currentMarkerIndex) {
            this._currentMarkerIndex++;
        }
        return commentId;
    },

    /*
    resetCommentWindow

    Resets vertical layout for all comments in the comment window.  
    This is done in order to easily reset the layout after adding a comment dynamically.
    */
    resetCommentWindow: function() {
        var marker;
        var comment;
        var totalHeight = 0;
        var commentBuffer = 30;
        var commentsCanvas = this._rootElement.findName('CommentsCanvas');

        for (var i = 0; i < this._numComments; i++) {
            marker = this.GetMarker(i);
            var commentId = parseInt(marker.Type,10);
            comment = this.GetCommentById(commentId);

            if (i === 0) {

                if (this._numComments > 1) {
                    comment.findName('CommentText_' + commentId).Text = 'Comments:';
                }
                commentsCanvas['Canvas.Top'] = (commentsCanvas.Height / 2) - comment.Height;
                comment['Canvas.Top'] = 0;
                totalHeight += comment.Height + commentBuffer;
            } else {
                comment['Canvas.Top'] = totalHeight;
                totalHeight += comment.Height + commentBuffer;
            }
        }

        var loadingMessage = this._rootElement.findName('LoadingMessage');
        loadingMessage.Visibility = 'Collapsed';

        this.SetFonts();
    },

    /*
    onMarkerReached and jumpToComment

    onMarkerReached Handles the markerReached video player event.
    jumpToComment moves the highlights the specified comment in the comment window.
    If syncPlayer is true, it will also move the video to the corresponding spot 
    (this behavior is desired for navigating from comment to comment in pause mode,
    but not in play mode.)
    */
    onMarkerReached: function(sender, markerEventArgs) {
        this.jumpToComment(this.getMarkerIndexFromId(markerEventArgs._marker.Type), false);
    },

    jumpToComment: function(markerIndex, syncPlayer) {
        var marker = this.GetMarker(markerIndex);
        var commentId = parseInt(marker.Type,10);
        if(isNaN(commentId)) {
            return;
        }
        var comment = this.GetCommentById(commentId);
        if(typeof comment == 'undefined') {
            return;
        }
        var translateTransform = this._rootElement.findName('CommentsTranslate');
        var commentAnimationStart = this._rootElement.findName('SlideCommentStart');
        var commentAnimationSpline = this._rootElement.findName('SlideCommentSpline');
        var storyboard = this._rootElement.findName('SlideCommentStoryboard');

        commentAnimationStart.Value = translateTransform.Y;
        commentAnimationSpline.Value = -1 * comment['Canvas.Top'];
        if (commentAnimationStart.Value != commentAnimationSpline.Value) {
            storyboard.begin();
        }
        var commentText = comment.findName('CommentText_' + commentId);
        var commentAvatar = comment.findName('CommentAvatar_' + commentId);
        var commentOwner = comment.findName('CommentOwner_' + commentId);

        commentText.foreground = 'White';
        commentOwner.foreground = 'White';
        commentAvatar.Opacity = 1;
        if (markerIndex != this._currentMarkerIndex) {
            marker = this.GetMarker(this._currentMarkerIndex);
            commentId = parseInt(marker.Type,10);
            comment = this.GetCommentById(commentId);
            commentText = comment.findName('CommentText_' + commentId);
            commentAvatar = comment.findName('CommentAvatar_' + commentId);
            commentOwner = comment.findName('CommentOwner_' + commentId);
            commentText.foreground = 'Gray';
            commentOwner.foreground = 'Gray';
            commentAvatar.Opacity = 0.5;
            this._currentMarkerIndex = markerIndex;
        }

        this.SetPagingVisibility();

        if (syncPlayer) {
            marker = this.GetMarker(markerIndex);
            this._player.set_timeIndex(marker.Time.Seconds);
            this._player._updatePosition();
        }
    },

    /*
    SetPagingVisibility

    Determines whether to show or hide each paging control depending on the position 
    of the currently highlighted comment.
    */
    SetPagingVisibility: function() {
        var pageNext = this._rootElement.findName('PageNext');
        var pagePrevious = this._rootElement.findName('PagePrevious');
        if (this._currentMarkerIndex === 0) {
            pagePrevious.Visibility = 'Collapsed';
            this.HighlightElement(this._rootElement.findName('PagePreviousArrow'), false);
        } else {
            pagePrevious.Visibility = 'Visible';
        }
        if (this._currentMarkerIndex == this._numComments - 1) {
            pageNext.Visibility = 'Collapsed';
            this.HighlightElement(this._rootElement.findName('PageNextArrow'), false);
        } else {
            pageNext.Visibility = 'Visible';
        }
    },

    /*
    ShowPagingControls and HidePagingControls
    */
    ShowPagingControls: function() {
        this._rootElement.findName('PagingControls').visibility = 'Visible';
        this._rootElement.findName('ShowPagingControls').begin();
    },

    HidePagingControls: function() {
        this._rootElement.findName('HidePagingControls').begin();
    },

    OnHidePagingComplete: function() {
        this._rootElement.findName('PagingControls').visibility = 'Collapsed';
    },

    /*
    OnPageNext, OnPagePrevious
    
    Handles page button clicks
    */

    OnPageNext: function(sender, mouseEventArgs) {
        this.jumpToComment(this._currentMarkerIndex + 1, true);
    },

    OnPagePrevious: function(sender, mouseEventArgs) {
        this.jumpToComment(this._currentMarkerIndex - 1, true);
    },

    /*
    OnPageNextMouseEnter, OnPageNextMouseLeave, OnPagePreviousMouseEnter, OnPagePreviousMouseLeave
    
    Handles hover behavior for paging buttons
    */

    OnPageNextMouseEnter: function(sender, mouseEventArgs) {
        this.HighlightElement(this._rootElement.findName('PageNextArrow'), true);
    },

    OnPageNextMouseLeave: function(sender, mouseEventArgs) {
        this.HighlightElement(this._rootElement.findName('PageNextArrow'), false);
    },

    OnPagePreviousMouseEnter: function(sender, mouseEventArgs) {
        this.HighlightElement(this._rootElement.findName('PagePreviousArrow'), true);
    },

    OnPagePreviousMouseLeave: function(sender, mouseEventArgs) {
        this.HighlightElement(this._rootElement.findName('PagePreviousArrow'), false);
    },

    /*
    HighlightElement
    
    Highlights or fades a comment
    */

    HighlightElement: function(element, isHighlighted) {
        element.Fill = isHighlighted ? 'White': 'Gray';
    },

    /*
    SetFonts
    
    Sets font resources for all comments.
    */

    SetFonts: function() {
        if (this._fontsDownloaded && this._commentsCreated) {
            for (var i = 0; i < this._numComments; i++) {
                var comment = this.GetCommentByMarkerIndex(i);
                var commentText = comment.findName('CommentText_' + i);
                var commentOwner = comment.findName('CommentOwner_' + i);
                commentText.setFontSource(this._fonts);
                commentText.fontFamily = 'Segoe UI';
                if (i > 0) {
                    commentText.fontStyle = 'Italic';
                }
                commentOwner.setFontSource(this._fonts);
                commentOwner.fontFamily = 'Segoe UI';
            }
        }

        var loadingMessage = this._rootElement.findName('LoadingMessage');
        loadingMessage.setFontSource(this._fonts);
        loadingMessage.fontFamily = 'Segoe UI';
    },

    /*
    Comment accessors
    
    These functions get comments and markers:
    GetCommentById: Finds a comment element by its canvas ID
    GetCommentByMarkerIndex: Finds a comment by its position relative to other markers.
    GetMarker: Finds a marker by its index relative to other markers.
    GetMarkerIndexFromId: Finds a marker index for a given ID.
    
    IDs vs. Indexes
    IDs are unique identifiers for a comment/marker pair.  They are assigned at load.
    As comments are added by the user, each one receives a new unique ID.\
    While indexes are also unique, the index of a comment may change if a new one is
    added before it. The ID will not. Since the ID is used to name the Silverlight
    canvas, it cannot change, so indexes provide a way to go to the next or previous
    comment without knowing its ID.
    */

    GetCommentById: function(id) {
        return this._rootElement.findName('Comment_' + id);
    },

    GetCommentByMarkerIndex: function(markerIndex) {
        var marker = this.GetMarker(markerIndex);
        return this.GetCommentById(parseInt(marker.Type,10));
    },

    GetMarker: function(markerIndex) {
        var sortedMarkers = this._player._mediaElement.Markers;
        return sortedMarkers.GetItem(markerIndex);
    },

    getMarkerIndexFromId: function(id) {
        var sortedMarkers = this._player._mediaElement.Markers;
        for (var i = 0; i < sortedMarkers.Count; i++) {
            var marker = sortedMarkers.GetItem(i);
            if (marker.Type == id) {
                return i;
            }
        }
    },

    /*
    Media event handlers

    onMediaEnded resets the video to the beginning and pauses it.
    onMediaFailed raises an error.
    onStateChanged toggles the playing vs. paused states of the comment view.
    */

    onMediaEnded: function(sender, eventArgs) {
        this._player.set_timeIndex(0);
        this._player._updatePosition();
        this._player.pause();
    },

    onMediaFailed: function(sender, eventArgs) {
        alert(String.format(Ee.UI.Xaml.Media.Res.mediaFailed, this._player.get_mediaUrl()));
    },

    onStateChanged: function(sender, eventArgs) {

        var playState = this._player.get_playState();
        //Comment mode pauses the video
        if (playState == "Paused" && !this._AddCommentMode) {
            this.ShowPagingControls();

            if (AjaxControlToolkit.Animation) {
                $get("screen-cover").style.display = "block";
                $get("screen-cover").style.width = $get("main").offsetWidth;
                //Width must be a fixed value for fade-out
                AjaxControlToolkit.Animation.FadeOutAnimation.play($get("screen-cover"), 0.3, 30);
                AjaxControlToolkit.Animation.FadeOutAnimation.play($get("frame-bottom2"), 0.3, 30);
                $get("screen-cover").style.width = "100%";
                //Reset width to 100% to allow screen resize
                }

        } else if (playState == "Playing") {
            this.HidePagingControls();

            if (AjaxControlToolkit.Animation) {
                $get("screen-cover").style.display = "block";
                AjaxControlToolkit.Animation.FadeInAnimation.play($get("screen-cover"), 0.3, 30);
                AjaxControlToolkit.Animation.FadeInAnimation.play($get("frame-bottom2"), 0.3, 30);
            }
        }
    },

    /*
    showCommentForm, hideCommentForm

    shows and hides the "add comment" dialog.
    */

    showCommentForm: function() {
        this._AddCommentMode = true;
        this._player.pause();
        this._content.findname("CommentsView").Opacity = 0;
        this._content.findname("AddCommentCanvas").Visibility = "Visible";
        var timeline = this._content.findname("TimeLine");
        timeline["Canvas.Left"] = 560;
        timeline["Canvas.Top"] = 40;
        timeline.Width = 100;
        timeline["Canvas.ZIndex"] = 10;
        this._content.findname("PlayerBase").Visibility = "Collapsed";
        this._content.findname("MuteButton").Visibility = "Collapsed";
        this._content.findname("PlayPauseButton").Visibility = "Collapsed";
        this._content.findname("TimeReadout").Visibility = "Collapsed";
        this._content.findname("FullScreenButton").Visibility = "Collapsed";
        this._content.findname("VolumeControl").Visibility = "Collapsed";
        this._content.findname("ShowVolumeControl").Visibility = "Collapsed";
        //$get("TabContainer").style.display="none";
        $get("commentForm").style.display = "inline";
    },

    hideCommentForm: function() {
        this._AddCommentMode = false;
        //$get("TabContainer").style.display="inline";
        $get("commentForm").style.display = "none";
        this._content.findname("AddCommentCanvas").Visibility = "Collapsed";
        this._content.findname("PlayerBase").Visibility = "Visible";
        this._content.findname("MuteButton").Visibility = "Visible";
        this._content.findname("PlayPauseButton").Visibility = "Visible";
        this._content.findname("TimeReadout").Visibility = "Visible";
        this._content.findname("FullScreenButton").Visibility = "Visible";
        this._content.findname("VolumeControl").Visibility = "Visible";
        this._content.findname("ShowVolumeControl").Visibility = "Visible";
        this._content.findname("CommentsView").Opacity = 1;
        var timeline = this._content.findname("TimeLine");
        timeline["Canvas.Left"] = 114;
        timeline["Canvas.Top"] = 392.556;
        timeline.Width = 390.162;
        timeline["Canvas.ZIndex"] = 10;
    },

    /*
    submitComment, onCommentAdded

    Adds a new comment to the comment window, sets the player to that marker.
    */

    SubmitComment: function() {
        CommentText = $get("CommentText").value;
        timeInSeconds = this._player.get_timeIndex().toFixed(1);
        if (timeInSeconds === 0) {
            timeInSeconds += 0.1;
        }
        VideoShow.CommentWebservice.AddComment(userId, videoId, CommentText, timeInSeconds, this.createDelegate(this, this.onCommentAdded));
        var commentId = this.insertComment(CommentText, userName, userAvatarUrl, timeInSeconds, false);
        this.resetCommentWindow();
        this.jumpToComment(this.getMarkerIndexFromId(commentId), true);
    },

    onCommentAdded: function() {
        //Add the comment to the current display so we can see it.
        //var markerXaml = "<TimelineMarker Time=\"0:0:" + this._VideoTimeInSeconds + "\" Type=\"You just said...\" Text=\"" + CommentText + "\" />"
        //var marker = this._content.createFromXaml(markerXaml);
        //this._player._mediaElement.markers.add(marker);

        this._player.play();
        this.hideCommentForm();
    },

    /*
    favorites

    Methods to handle favorites button behavior
    */

    getFavoriteStatus: function() {
        this._favoriteStatus = VideoShow.FavoriteWebservice.GetFavoriteStatus(userId, videoId, PlayerControl.prototype.createDelegate(this, this.onFavoriteStatusReceived));
    },

    onFavoriteStatusReceived: function(status) {
        this._favoriteStatus = status;
        if (status) {
            $get("ToggleFavoriteLink").innerText = "Remove Favorite";
        } else {
            $get("ToggleFavoriteLink").innerText = "Favorite";
        }
    },

    toggleFavorite: function() {
        VideoShow.FavoriteWebservice.ToggleFavoriteStatus(userId, videoId, PlayerControl.prototype.createDelegate(this, this.getFavoriteStatus));
    },

    /*
    createDelegate

    Helper method for delegate creation.
    */

    createDelegate: function(instance, method) {
        return function() {
            return method.apply(instance, arguments);
        };
    }
};