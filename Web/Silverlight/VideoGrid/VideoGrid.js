///////////////////////////////////////////////////////////////////////////////
//
//  VideoGrid
//
//  Shows a grid of videos with paging and hover-over in-place previews.
//
///////////////////////////////////////////////////////////////////////////////

if (!window.Silverlight) {
	window.Silverlight = {};
}
if (!window.VideoGrid) {
	window.VideoGrid = {};
}
/*
Silverlight.installAndCreateSilverlight

Checks for installation of Microsoft Silverlight. 
If installed, it calls the CreateSilverlight method.
If not, it displays the content preview div and prompts the user to install Silverlight
If the user is running IE, a timout-driven loop will automatically load the Silverlight upon installation.

This method is based on recommendations from 
"Optimizing the Silverlight Installation Experience," 
Microsoft Silverlight, authors: PiotrP & JohnPay
*/
Silverlight.installAndCreateSilverlight = function(version, parentElementId, installExperienceHTML, installPromptDivID, createSilverlightDelegate, queryType, queryValue, configuration, selectedIndex, isMyPage) {
    var RetryTimeout=5000;	              //The interval at which Silverlight instantiation is attempted(ms)	
    if ( Silverlight.isInstalled(version) ) {
    	createSilverlightDelegate(parentElementId, queryType, queryValue, configuration, selectedIndex, isMyPage);
    } else {
        var parentElement = document.getElementById(parentElementId);
	    if ( installExperienceHTML && parentElement ) {
	        parentElement.innerHTML=installExperienceHTML;
	    } 
	    if (installPromptDivID) {
            var installPromptDiv = document.getElementById(installPromptDivID);
            if ( installPromptDiv ) {
                installPromptDiv.innerHTML = Silverlight.createObject(null, null, null, {version: version, inplaceInstallPrompt:true},{}, null);
            }
        }

	    if ( ! (Silverlight.available || Silverlight.ua.Browser != 'MSIE' ) ) {
	        TimeoutDelegate = function() {
                Silverlight.installAndCreateSilverlight(version, parentElementId, installExperienceHTML, installPromptDivID, createSilverlightDelegate, queryType, queryValue, configuration, selectedIndex, isMyPage);
	        };
	        setTimeout(TimeoutDelegate, RetryTimeout);
	    }

    }
};



/*
Silverlight.createSilverlight

Creates the Silverlight control and instantiates a scene object to drive it.
*/
Silverlight.createSilverlight = function(parentElementId, queryType, queryValue, configuration, selectedIndex, isMyPage) {
    scene = new VideoGrid.Scene(queryType, queryValue, configuration, selectedIndex, isMyPage);

    Silverlight.createHostedObjectEx( {
        source: "/Silverlight/VideoGrid/VideoGrid.xaml",
	    parentElement: document.getElementById(parentElementId),
        id: "VideoGrid_" + parentElementId,
        properties: {
            width: scene._layout.sceneWidth.toString(),
            height: scene._layout.sceneHeight.toString(),
            inplaceInstallPrompt:true,
            background:'#00FFFFFF',
            isWindowless:'true',
            framerate:'24',
            version:'1.0'
        },
	    events: {
		    onLoad: Silverlight.createDelegate(scene, scene.onLoad),
		    onError: Silverlight.createDelegate(scene, scene.onError)
	    },
        initParams:null,
        context:null
    } );                          
};

/*
Silverlight.createDelegate

Helper method for creating delegates from object-based methods.
*/
Silverlight.createDelegate = function(instance, method) {
	return function() {
        return method.apply(instance, arguments);
    };
};




/*
VideoGrid.Scene

Represents the overall video grid. 
Handles overall initialization, paging, and web service data access 
*/

VideoGrid.Scene = function(queryType, queryValue, configuration, selectedIndex, isMyPage) {
    this._queryType = queryType;
    this._queryValue = queryValue;
    this._isMyPage = isMyPage;

    this._layout = new VideoGrid.Layout(configuration);
    
    // todo: should _currentPage be 0-indexed?
    this._currentPage = Math.floor(selectedIndex/this._layout.gridMaxItems) + 1;
    this._selectedIndex = selectedIndex % this._layout.gridMaxItems;
};

VideoGrid.Scene.prototype = {
    /* 
        Scene resources
    */
    _control: null, // Silverlight plugin
    _rootElement: null, // Silverlight root canvas element
    _xamlResources: null, // Downloaded XAML resource zip archive
    _fonts: null, // Download font zip archive
    _layout: null, // Layout object containing layout paramaters for the video grid
    
    /* 
        Video metadata
        (Two grids are created to achieve the paging effect. When the user pages the results,
        the scene populates the inactive grid. It then slides the active grid out and slides
        the inactive grid in to accomplish the page.  Active and inactive states are then 
        switched.
    */
    _videoList: null, // Collection of metadata for current video grid (received from video web service)
    _queryType: null, // The type of query. Values: Tag, Owner, Favorite, RecentViews
    _queryValue: null, // The value of the query
    _ownerName: null, // For Owner and Favorite queries, the owner/favoriter's user name. Used for no-results messaging.
    _isMyPage: false, // Specifies if an Owner query is for the current user. Used for no-results messaging.
    _noResultsUrl: null, // Specifies the destination of the no-results link.
    
    /* 
        Grids & Paging
        (Two grids are created to achieve the paging effect. When the user pages the results,
        the scene populates the inactive grid. It then slides the active grid out and slides
        the inactive grid in to accomplish the page.  Active and inactive states are then 
        switched.
    */
	_grids: [2], // Video grid array of Grid objects
    _sceneEnabled: true, // Indicates whether the scene disabled during paging
    _activeGridId: 0, // Specifies which grid is currently being displayed
    _inactiveGridId: 1, // Specifies which grid is currently hidden 
    _totalPages: null, // Number of pages in the current result set 
    _totalVideos: null, // Number of videos in the current result set 
    _currentPage: 1, // Current result page
    _currentPageAction: 'none', // specifies whether the current/last paging action is next, previous, or none
                                // stores the state across asynchronous calls.

    /*
        onLoad
        handles the scene canvas load event. Begins grid initialization.
    */
	onLoad: function(control, userContext, rootElement) {
	    this._control = control;
	    this._rootElement = rootElement;

        this._rootElement.Width = this._layout.sceneWidth;
        this._rootElement.Height = this._layout.sceneHeight;
        
        mask = this._rootElement.findName('SceneMask');
        mask.Width = this._layout.gridWidth;
        mask.Height = this._layout.gridHeight;

        pagingSeparator = this._rootElement.findName('PagingSeparator');
        pagingSeparator.X1 = this._layout.sceneBufferLeft;
        pagingSeparator.X2 = this._layout.sceneBufferLeft + this._layout.gridWidth;
        pagingSeparator.Y1 = this._layout.gridHeight + 20;
        pagingSeparator.Y2 = this._layout.gridHeight + 20;

        previousButton = this._rootElement.findName('PreviousPage');
        previousButton['Canvas.Left'] = this._layout.sceneBufferLeft;
        previousButton['Canvas.Top'] = this._layout.gridHeight + 30;

        nextButton = this._rootElement.findName('NextPage');
        nextButton['Canvas.Left'] = this._layout.sceneWidth - this._layout.sceneBufferRight - nextButton.ActualWidth;
        nextButton['Canvas.Top'] = previousButton['Canvas.Top'];

        var pageCount = this._rootElement.findName('PageCount');
        pageCount['Canvas.Top'] = previousButton['Canvas.Top'];

		this.wireupSceneEvents();

		var resourceDownloader = control.CreateObject('downloader');
		resourceDownloader.addEventListener('completed', Silverlight.createDelegate(this, this.resourceDownloadComplete));
		resourceDownloader.open('GET', '/Silverlight/VideoGrid/VideoGrid.zip');
		resourceDownloader.send();

        /*
		var fontDownloader = control.CreateObject('downloader');
		fontDownloader.addEventListener('completed', Silverlight.createDelegate(this, this.fontDownloadComplete));
		fontDownloader.open('GET', '/Silverlight/Fonts/Fonts.zip');
		fontDownloader.send();
		*/

        this.getVideos();
        
        //TODO: explain strategy for starting two parallel async events which must come back together later on.
	},
	
    /*
        wireupSceneEvents
        Initializes all scene event handlers. Called on load
    */
    wireupSceneEvents: function() {
        this._nextPage = this._rootElement.findName('NextPage');
        this._previousPage = this._rootElement.findName('PreviousPage');
        this._nextPage.addEventListener('MouseLeftButtonUp', Silverlight.createDelegate(this, this.pageNext));
        this._previousPage.addEventListener('MouseLeftButtonUp', Silverlight.createDelegate(this, this.pagePrevious));
        var mask = this._rootElement.findName('SceneMask');
        var storyboard = mask.findName('HideSceneMask');
        storyboard.addEventListener('Completed', Silverlight.createDelegate(this, this.onSceneEnabled));
        var nextButton = mask.findName('NextPage');
        var previousButton = mask.findName('PreviousPage');
        nextButton.addEventListener('MouseEnter', Silverlight.createDelegate(this, this.onMouseEnterPageButton));
        previousButton.addEventListener('MouseEnter', Silverlight.createDelegate(this, this.onMouseEnterPageButton));
        nextButton.addEventListener('MouseLeave', Silverlight.createDelegate(this, this.onMouseLeavePageButton));
        previousButton.addEventListener('MouseLeave', Silverlight.createDelegate(this, this.onMouseLeavePageButton));

        var secondaryMessage = this._rootElement.findName('SecondaryMessage');
        secondaryMessage.addEventListener('MouseLeftButtonUp', Silverlight.createDelegate(this, this.onNoResultsClick));
    },
    
    /*
        resourceDownloadComplete
        Called when xaml resources have been downloaded. Completes grid initialization.
    */
	resourceDownloadComplete: function(sender, eventArgs) {
	    // Store downloaded XAML templates
	    this._xamlResources = sender;

	    // Create grids
	    this._grids[this._activeGridId] = new VideoGrid.Grid(this, true);
	    this._grids[this._inactiveGridId] = new VideoGrid.Grid(this, false);

        this.setFonts();
        this.addGridItems();
    },
    
    /*
        fontDownloadComplete
        Called when fonts have been downloaded. Begins font initialization.
    */
	fontDownloadComplete: function(sender, eventArgs) {
        this._fonts = sender;
        this.setFonts();
    },
    
    /*
        setFonts
        Completes font initialization. 
        This function can occur only after two asynchronous events have completed:
        1) xaml resources must have downloaded (resourceDownloadComplete)
        2) fonts must have downloaded (fontDownloadComplete)
        To accomplish this, both methods above call setFonts, which only executes
        after the second call.
    */
    setFonts: function() {
        if (this._xamlResources !== null && this._fonts !== null) {
            var nextPage = this._rootElement.findName('NextPage');
            var previousPage = this._rootElement.findName('PreviousPage');
            var primaryMessage = this._rootElement.findName('PrimaryMessage');
            var secondaryMessage = this._rootElement.findName('SecondaryMessage');
            var pageCount = this._rootElement.findName('PageCount');

            nextPage.setFontSource(this._fonts);
            previousPage.setFontSource(this._fonts);
            primaryMessage.setFontSource(this._fonts);
            secondaryMessage.setFontSource(this._fonts);
            pageCount.setFontSource(this._fonts);

            nextPage.fontFamily = 'Segoe UI';
            previousPage.fontFamily = 'Segoe UI';
            primaryMessage.fontFamily = 'Segoe UI';
            secondaryMessage.fontFamily = 'Segoe UI';
            pageCount.fontFamily = 'Segoe UI';

	        this._grids[this._activeGridId].setFonts(this._fonts);
	        this._grids[this._inactiveGridId].setFonts(this._fonts);
        }
    },
    
    /*
        getVideos
        Makes an asynchronous call to video services for a new collection of videos
        Called at initialization and on paging actions (next/previous)
    */
    getVideos: function() {
        //load videos
        switch (this._queryType) {
            case 'Tag':
                if (this._queryValue === '') {
        	        VideoShow.VideoWebservice.GetVideos(this._layout.gridMaxItems,this._currentPage,'', Silverlight.createDelegate(this, this.onVideoListReceived));
        	    } else {
        	        VideoShow.VideoWebservice.GetVideosByTag(this._layout.gridMaxItems,this._currentPage,'', this._queryValue, Silverlight.createDelegate(this, this.onVideoListReceived));
        	    }
        	    break;
            case 'Owner':
        	    VideoShow.VideoWebservice.GetVideosByOwner(this._layout.gridMaxItems,this._currentPage,'', this._queryValue, Silverlight.createDelegate(this, this.onVideoListReceived));
        	    break;
            case 'Favorite':
        	    VideoShow.VideoWebservice.GetVideosByFavorite(this._layout.gridMaxItems,this._currentPage,'', this._queryValue, Silverlight.createDelegate(this, this.onVideoListReceived));
        	    break;
            case 'RecentViews':
        	    VideoShow.VideoWebservice.GetVideosByRecentViews(this._layout.gridMaxItems,this._currentPage,'', this._queryValue, Silverlight.createDelegate(this, this.onVideoListReceived));
        	    break;
        }
    },
    
    /*
        onVideoListReceived
        Handles web service callback. Begins logic for populating a grid with videos.
    */
    onVideoListReceived: function(result) {
        var streamingUrls = '';
        this._totalVideos = result.totalVideos;
        this._totalPages = result.totalPages;
        this._ownerName = result.ownerName;
        this._videoList = result.videos;
        
        if (this._videoList.length > 0) {
            var len = result.videos.length;
            for (var i=0; i<len; i++) {
                streamingUrls += result.videos[i].thumbnailUrl + ',';
                streamingUrls += result.videos[i].previewUrl + ',';
            }
            
            SLStreaming.translateUrls(streamingUrls,Silverlight.createDelegate(this, this.onSLStreamingUrlsTranslated));
        } else {
            this.showNoResultsMessage();
        }
    },


    /*
        onSLStreamingUrlsTranslated
        Handles callback for Silverlight Streaming "translate URLs" method. 
        Updates the video collection with absolute URLs for Silverlight Streaming
        and begins grid initialization.
    */
    onSLStreamingUrlsTranslated: function(result) {
        var len = this._videoList.length;
        for (var i=0; i < len; i++) {
            this._videoList[i].thumbnailUrl = result[this._videoList[i].thumbnailUrl];
            this._videoList[i].previewUrl = result[this._videoList[i].previewUrl];
        }
        
        this.addGridItems();
    },
    
    /*
        addGridItems
        Initializes a grid of grid items based on the current video list.
        This function can occur only after two asynchronous events have completed:
        1) xaml resources must have downloaded (resourceDownloadComplete)
        2) the video list has been received and translated (onSLStreamingUrlsTranslated)
        To accomplish this, both methods above call addGridItems, which only executes
        after the second call.
    */
    addGridItems: function() {
        // if both async events (creating grids and receiving video list) have occurred
        if (this._xamlResources !== null && this._videoList !== null) {
            //alert("addGridItems - GO!")
            this._grids[this._activeGridId].initializeGridItems(this._videoList, this._currentPageAction == 'none');

            // set new state of paging buttons
            var nextButton = this._rootElement.findName('NextPage');
            var previousButton = this._rootElement.findName('PreviousPage');
            var pageCount = this._rootElement.findName('PageCount');
            
            if (this._currentPage < this._totalPages) {
                nextButton.Visibility = "Visible";
            } else {
                nextButton.Visibility = "Collapsed";
            }
            if (this._currentPage > 1) {
                previousButton.Visibility = "Visible";
            } else {
                previousButton.Visibility = "Collapsed";
            }   
            pageCount.Text = ((this._currentPage-1) * this._layout.gridMaxItems + 1).toString();
            pageCount.Text += '-';
            if (this._currentPage < this._totalPages) {
                pageCount.Text += (this._currentPage * this._layout.gridMaxItems).toString();
            } else {
                pageCount.Text += this._totalVideos.toString();
            }
            pageCount.Text += ' of ';
            pageCount.Text += this._totalVideos.toString();
            pageCount['Canvas.Left'] = this._rootElement.Width / 2 - pageCount.ActualWidth / 2;

            switch(this._currentPageAction) {
                case 'next':
                    this._currentPageAction = 'none';
                    this.completePageNext();
                    break;
                case 'previous':
                    this._currentPageAction = 'none';
                    this.completePagePrevious();
                    break;
            }
        }
    },
    
    /*
        pageNext and pagePrevious
        Handles "paging" click events--begins paging process 
    */
    pageNext: function(sender, mouseEventArgs) {
        // disable scene
        this.disableScene();

        this._currentPageAction = 'next';
        this._currentPage++;
        this.getVideos();
        
        this._activeGridId = this._inactiveGridId;
        this._inactiveGridId = 1 - this._activeGridId;
    },
    pagePrevious: function(sender, mouseEventArgs) {
        // disable scene
        this.disableScene();

        this._currentPageAction = 'previous';
        this._currentPage--;
        this.getVideos();

        // slide grids
        this._activeGridId = this._inactiveGridId;
        this._inactiveGridId = 1 - this._activeGridId;
    },

    /*
        completePageNext and completePagePrevious
        Completes paging process by starting paging animations after new grid has been initialized.
    */
    completePageNext: function() {
        // page grids
        this._grids[this._inactiveGridId].pageOutToLeft();
        this._grids[this._activeGridId].pageInFromLeft();
    },
    completePagePrevious: function() {
        // page grids
        this._grids[this._inactiveGridId].pageOutToRight();
        this._grids[this._activeGridId].pageInFromRight();
    },

    /*
        disableScene, onDisableSceneTimeout, enableScene, and onSceneEnabled

        Disables and enables scene during paging (dims the grid and prevents other events from occurring)
        Paging relies on a successful asynchronous web service call to occur
        before the scene can be paged and re-enabled. If this does not occur, we
        set a 5 second timeout to re-enable the scene so the user isn't frozen.
    */

    disableScene: function() {
        this._sceneEnabled = false;

        var mask = this._rootElement.findName('SceneMask');
        mask.Visibility = "Visible";

        var storyboard = mask.findName('ShowSceneMask');
        storyboard.begin();
        
        setTimeout(Silverlight.createDelegate(this, this.onDisableSceneTimeout), 5000);
    },
    
    onDisableSceneTimeout: function() {
        this.enableScene();  
    },

    enableScene: function() {
        if (!this._sceneEnabled) {
            this._sceneEnabled = true;
            var mask = this._rootElement.findName('SceneMask');
            var storyboard = mask.findName('HideSceneMask');
            storyboard.begin();
        }
    },

    onSceneEnabled: function(sender, eventArgs) {
        var mask = this._rootElement.findName('SceneMask');
        mask.Visibility = "Collapsed";
        
        var nextButton = this._rootElement.findName('NextPage');
        nextButton.textDecorations = 'None';

        var previousButton = this._rootElement.findName('PreviousPage');
        previousButton.textDecorations = 'None';
    },
    
    /*
        onMouseEnterPageButton and onMouseLeavePageButton
        Handles hover behavior for paging links
    */

    onMouseEnterPageButton: function(sender, eventArgs) {
        sender.textDecorations = 'Underline';
    },
    
    onMouseLeavePageButton: function(sender, eventArgs) {
        sender.textDecorations = 'None';
    },

    /*
        showNoResultsMessage and onNoResultsClick
        handles the "no results" case, where the video grid has no videos.
        A simple message and link fill the grid.
    */
    
    showNoResultsMessage: function() {
        var noResultsCanvas = this._rootElement.findName('NoResultsMessage');
        var primaryMessage = this._rootElement.findName('PrimaryMessage');
        var secondaryMessage = this._rootElement.findName('SecondaryMessage');
        var storyboard = this._rootElement.findName('ShowNoResultsMessage');
        
        switch (this._queryType) {
            case 'Tag':
                if (this._queryValue === '') {
                    primaryMessage.Text = VideoGrid.Strings.prototype.noResults_allPrimary;
                    secondaryMessage.Text = VideoGrid.Strings.prototype.noResults_allSecondary;
                    this._noResultsUrl = VideoGrid.Strings.prototype.noResults_allSecondaryUrl;
                } else {
                    primaryMessage.Text = VideoGrid.Strings.prototype.noResults_tagPrimary.replace(/\$tag/g, this._queryValue);
                    secondaryMessage.Text = VideoGrid.Strings.prototype.noResults_tagSecondary;
                    this._noResultsUrl = VideoGrid.Strings.prototype.noResults_tagSecondaryUrl;
                }
        	    break;
            case 'Owner':
                if (this._isMyPage) {
                    primaryMessage.Text = VideoGrid.Strings.prototype.noResults_myOwnerPrimary.replace(/\$userName/g, this._ownerName);
                    secondaryMessage.Text = VideoGrid.Strings.prototype.noResults_myOwnerSecondary;
                    this._noResultsUrl = VideoGrid.Strings.prototype.noResults_myOwnerSecondaryUrl;
                } else {
                    primaryMessage.Text = VideoGrid.Strings.prototype.noResults_ownerPrimary.replace(/\$userName/g, this._ownerName);
                    secondaryMessage.Text = VideoGrid.Strings.prototype.noResults_ownerSecondary;
                    this._noResultsUrl = VideoGrid.Strings.prototype.noResults_ownerSecondaryUrl;
                }
        	    break;
            case 'Favorite':
                if (this._isMyPage) {
                    primaryMessage.Text = VideoGrid.Strings.prototype.noResults_myFavoritesPrimary.replace(/\$userName/g, this._ownerName);
                    secondaryMessage.Text = VideoGrid.Strings.prototype.noResults_myFavoritesSecondary;
                    this._noResultsUrl = VideoGrid.Strings.prototype.noResults_myFavoritesSecondaryUrl;
                } else {
                    primaryMessage.Text = VideoGrid.Strings.prototype.noResults_favoritesPrimary.replace(/\$userName/g, this._ownerName);
                    secondaryMessage.Text = VideoGrid.Strings.prototype.noResults_favoritesSecondary;
                    this._noResultsUrl = VideoGrid.Strings.prototype.noResults_favoritesSecondaryUrl;
                }
        	    break;
            case 'RecentViews':
                primaryMessage.Text = VideoGrid.Strings.prototype.noResults_recentViewsPrimary.replace(/\$userName/g, this._ownerName);
                secondaryMessage.Text = VideoGrid.Strings.prototype.noResults_recentViewsSecondary;
                this._noResultsUrl = VideoGrid.Strings.prototype.noResults_recentViewsSecondaryUrl;
        	    break;
        }

        // center message on canvas
        noResultsCanvas.Width = (primaryMessage.ActualWidth > secondaryMessage.ActualWidth) ? primaryMessage.ActualWidth : secondaryMessage.ActualWidth;
        noResultsCanvas.Height = primaryMessage.ActualHeight + secondaryMessage.ActualHeight;
        noResultsCanvas['Canvas.Left'] = this._rootElement.Width / 2 - noResultsCanvas.Width / 2;
        noResultsCanvas['Canvas.Top'] = this._rootElement.Height / 2 - noResultsCanvas.Height / 2;
        primaryMessage['Canvas.Left'] = noResultsCanvas.Width / 2 - primaryMessage.ActualWidth / 2;
        secondaryMessage['Canvas.Left'] = noResultsCanvas.Width / 2 - secondaryMessage.ActualWidth / 2;
        secondaryMessage['Canvas.Top'] = primaryMessage.ActualHeight;
        
        storyboard.begin();
    },

    onNoResultsClick: function( sender, mouseEventArgs ) {
        var url = window.location.protocol + '//' + window.location.host + '/' + this._noResultsUrl;
        window.location = url;
    },
    
    /*
        goToVideo
        redirects the browser to the player page for the selected video
    */
    goToVideo: function( videoId ) {
        var url = window.location.protocol + '//' + window.location.host + '/View.aspx';
        url += '?video=' + videoId;
        url += '&viewType=' + this._queryType;
        url += '&filterValue=' + this._queryValue;
        window.location = url;
    },
    
    /*
        onError
        Silverlight plugin generic error handler
        (Using sample error handler from Silverlight SDK)
    */
	onError: function(sender, errorEventArgs) {
        // The error message to display.
        var errorMsg = "Silverlight Error: \n\n";
        
        // Error information common to all errors.
        errorMsg += "Error Type:    " + errorEventArgs.errorType + "\n";
        errorMsg += "Error Message: " + errorEventArgs.errorMessage + "\n";
        errorMsg += "Error Code:    " + errorEventArgs.errorCode + "\n";
        
        // Determine the type of error and add specific error information.
        switch(errorEventArgs.errorType) {
            case "RuntimeError":
                // Display properties specific to RuntimeErrorEventArgs.
                if (errorEventArgs.lineNumber !== 0) {
                    errorMsg += "Line: " + errorEventArgs.lineNumber + "\n";
                    errorMsg += "Position: " +  errorEventArgs.charPosition + "\n";
                }
                errorMsg += "MethodName: " + errorEventArgs.methodName + "\n";
                break;
            case "ParserError":
                // Display properties specific to ParserErrorEventArgs.
                errorMsg += "Xaml File:      " + errorEventArgs.xamlFile      + "\n";
                errorMsg += "Xml Element:    " + errorEventArgs.xmlElement    + "\n";
                errorMsg += "Xml Attribute:  " + errorEventArgs.xmlAttribute  + "\n";
                errorMsg += "Line:           " + errorEventArgs.lineNumber    + "\n";
                errorMsg += "Position:       " + errorEventArgs.charPosition  + "\n";
                break;
            default:
                break;
        }

        // Display the error message.
        alert(errorMsg);
    }    
};



/*
VideoGrid.Grid

Represents a grid of videos.
At any one time, one grid is actively displayed (the other is used for paging in a new set of results).
*/

VideoGrid.Grid = function(scene, isActive) {
    this._scene = scene;
    this._layout = scene._layout;
    this.createGridFromTemplate(isActive);
};

VideoGrid.Grid.prototype = {
    /* 
        Grid members
    */
    _scene: null, // Scene parent canvas
    _layout: null, // Layout parameters
    _canvas: null, // Grid canvas object
    _gridCanvasTemplate: null, // Grid xaml template (downloaded in xaml resources)
    _gridItems: null, // grid metadata
    
    /* 
        createGridFromTemplate
        Creates a new grid from the XAML template.
        Called at construction
    */
    createGridFromTemplate: function(isActive) {
	    var gridCanvasXaml = this._scene._xamlResources.getResponseText('GridCanvas.xaml');

	    this._canvas = this._scene._control.content.createFromXaml(gridCanvasXaml, true);

	    this._canvas.Width = this._layout.gridWidth; 
	    this._canvas.Height = this._layout.gridHeight;
        this._canvas['Canvas.Left'] = this._layout.sceneBufferLeft;
	    this._canvas['Canvas.Top'] = this._layout.sceneBufferTop;

        // initial position is determined by active state
        translateTransform = this._canvas.findName('GridCanvasTranslateTransform');
        if (isActive) {
            translateTransform.X = 0;
	    } else {
            translateTransform.X = this._layout.sceneWidth;
        }
        this.wireupGridEvents();

        this._scene._rootElement.children.add(this._canvas);
        
        this._gridItems = [this._layout.gridMaxItems];
        var len = this._layout.gridMaxItems;
        for (i = 0; i < len; i++) {
            // grid items are initially hidden if this is the active grid.
            // if not, grid items will be visible (no "onload" animation)
            this._gridItems[i] = new VideoGrid.GridItem(this, i, !isActive);
        }
    },

    /* 
        wireupGridEvents
        Adds event listeners for grid.  Called at initialization
    */
    wireupGridEvents: function() {
        var storyboard = this._canvas.findName('GridCanvasStoryboardInFromLeft');
        storyboard.addEventListener('Completed', Silverlight.createDelegate(this, this.onPagingComplete));
        storyboard = this._canvas.findName('GridCanvasStoryboardInFromRight');
        storyboard.addEventListener('Completed', Silverlight.createDelegate(this, this.onPagingComplete));
    },
    
    /* 
        setFonts
        Sets font resources for the grid
    */
    setFonts: function(fontDownloader) {
        var len = this._layout.gridMaxItems;
        for (i = 0; i < len; i++) {
            this._gridItems[i].setFonts(fontDownloader);
        }
    },

    /* 
        initializeGridItems
        Initializes the grid with new video data.
        Called at initialization or on paging requests.
    */
    initializeGridItems: function(videoList, showInitialAnimation) {
        this._numItems = videoList.length;
        var len = this._layout.gridMaxItems;
        for (var i = 0; i < len; i++) {
            if (i < this._numItems) {
                this._gridItems[i].initializeGridItem(videoList[i], showInitialAnimation);
            } else {
                this._gridItems[i].disableGridItem();
            }
        }
    },
    
    /* 
        pageOutToLeft, pageOutToRight, pageInFromLeft, pageInFromRight, pageHelper, and onPagingComplete
        Paging methods. Triggers paging animations depending on whether the grid is 
        being paged in or out of view, and in which direction.
    */

    pageOutToLeft: function() {
        this.pageHelper(this._canvas.findName('GridCanvasStoryboardOutToLeft'),0);
    },

    pageOutToRight: function() {
        this.pageHelper(this._canvas.findName('GridCanvasStoryboardOutToRight'),0);
    },
    
    pageInFromLeft: function() {
        this.pageHelper(this._canvas.findName('GridCanvasStoryboardInFromLeft'),this._layout.sceneWidth);
    },
    
    pageInFromRight: function() {
        this.pageHelper(this._canvas.findName('GridCanvasStoryboardInFromRight'),this._layout.sceneWidth*-1);
    },
    
    pageHelper: function(storyboard, startingX) {
        translateTransform = this._canvas.findName('GridCanvasTranslateTransform');
        translateTransform.X = startingX;
        storyboard.begin();
    },
    
    onPagingComplete: function(sender, eventArgs) {
        this._scene.enableScene();
    }
};









/*
VideoGrid.GridItem

Represents a single video in the video grid, as well as its hover state.
*/

VideoGrid.GridItem = function(grid, itemIndex, isVisible) {
    this._grid = grid;
    this._layout = grid._layout;
    this._itemIndex = itemIndex;
    this.createItemFromTemplate(isVisible);
};

VideoGrid.GridItem.prototype =  {
    /* 
        GridItem members
    */
    _grid: null, // GridItem parent Grid object
    _layout: null, // Layout parameters
    _itemIndex: null, // Specifies the position of the item in the grid
    _videoInfo: null, // Video metadata 
    _showInitialAnimation: false, // Boolean which indicates if the gridItem should show 
                                  // it's "load" animation, which only occurs when the page
                                  // is first loaded (not on paging)
    _gridItemTemplate: null, // xaml template for grid item (downloaded)
    _gridItemHoverTemplate: null, // xaml template for grid item hover (downloaded)
    _canvas: null, // grid item canvas object
    _hoverCanvas: null, // grid item hover canvas object
    
    /* 
        createItemFromTemplate
        Creates a GridItem from xaml and adds it to the parent canvas.
        Called only at item creation by constructor.
    */
    createItemFromTemplate: function(isVisible) {
        var gridItemXaml = this._grid._scene._xamlResources.getResponseText('GridItem.xaml');
        var itemHoverXaml = this._grid._scene._xamlResources.getResponseText('GridItemHover.xaml');

        // create Silverlight object
        this._canvas = this._grid._scene._control.content.createFromXaml(gridItemXaml, true); //todo: this object referencing could be cleaner
        this._hoverCanvas = this._grid._scene._control.content.createFromXaml(itemHoverXaml, false); //todo: this object referencing could be cleaner

        this._canvas.children.add(this._hoverCanvas);
        this._grid._canvas.children.add(this._canvas);
        
        // place item on Grid
        this._canvas['Canvas.Left'] = (this._canvas.Width + this._layout.itemPaddingX)*(this._itemIndex % this._layout.gridNumColumns);
        this._canvas['Canvas.Top'] = (this._canvas.Height+this._layout.itemPaddingY)*Math.floor(this._itemIndex/this._layout.gridNumColumns);

        this._hoverCanvas['Canvas.Left'] = -15;
        this._hoverCanvas['Canvas.Top'] = -5;

        this._canvas['Canvas.ZIndex'] = 100;
        
        // if this is the last column, the hover state should pop from the upper right.
        if (this._itemIndex % this._layout.gridNumColumns == this._layout.gridNumColumns - 1) {
            var scaleTransform = this._hoverCanvas.findName('ItemHoverScaleTransform');
            this._hoverCanvas['Canvas.Left'] = this._canvas.Width - this._hoverCanvas.Width;
            scaleTransform.CenterX = this._canvas.Width;
        }

        this._canvas.findName('ItemHoverBackground').Source = 'Silverlight/VideoGrid/Images/video-popup-squat.png';
        this.wireupItemEvents();
    },
    
    /* 
        wireupItemEvents
        Adds event listeners for the grid item.  
        Called at item construction
    */
    wireupItemEvents: function() {
        var thumbnail = this._canvas.findName("ItemThumbnail");
        thumbnail.addEventListener("downloadProgressChanged", Silverlight.createDelegate(this, this.onImageProgressChanged));
        thumbnail.addEventListener("mouseEnter", Silverlight.createDelegate(this, this.onMouseEnterThumbnail));
        this._canvas.addEventListener("mouseLeave", Silverlight.createDelegate(this, this.onMouseLeaveItemCanvas));

        this._hoverCanvas.addEventListener("mouseLeftButtonUp", Silverlight.createDelegate(this, this.onClick));

        var preview = this._canvas.findName("ItemHoverPreview");
        preview.addEventListener("currentStateChanged", Silverlight.createDelegate(this, this.onPreviewStateChanged));
        preview.addEventListener("bufferingProgressChanged", Silverlight.createDelegate(this, this.onPreviewBufferingProgressChanged));
        
        this._canvas.findName("ItemHoverShowStoryboard").addEventListener("completed", Silverlight.createDelegate(this, this.onShowHoverComplete));
        this._canvas.findName("ItemHoverHideStoryboard").addEventListener("completed", Silverlight.createDelegate(this, this.onHideHoverComplete));
    },
    
    /* 
        setFonts
        Sets font resources for the GridItem
    */
    setFonts: function(fontDownloader) {
        var title = this._canvas.findName("ItemTitle");
        var tags = this._canvas.findName("ItemTags");
        var hoverTitle = this._canvas.findName("ItemHoverTitle");
        var hoverDetail = this._canvas.findName("ItemHoverInfoDetail");
        var previewProgress = this._canvas.findName("ItemHoverLoadingProgress");
        var hoverTags = this._canvas.findName("ItemHoverTags");
        title.setFontSource(fontDownloader);
        tags.setFontSource(fontDownloader);
        hoverTitle.setFontSource(fontDownloader);
        hoverDetail.setFontSource(fontDownloader);
        previewProgress.setFontSource(fontDownloader);

        hoverTags.setFontSource(fontDownloader);
        title.FontFamily = "Segoe UI";
        tags.FontFamily = "Segoe UI";
        hoverTitle.FontFamily = "Segoe UI";
        hoverDetail.FontFamily = "Segoe UI";
        previewProgress.FontFamily = "Segoe UI";
        hoverTags.FontFamily = "Segoe UI";
    },
    
    /* 
        initializeGridItem
        Initializes a grid item with new video metadata
    */
    initializeGridItem: function(videoInfo, showInitialAnimation) {
        this._videoInfo = videoInfo;
        this._showInitialAnimation = showInitialAnimation;
        
        this.hideItem();

        this._canvas.findName("Item").Visibility = "Visible";
        this._canvas.findName("ItemTitle").Text = videoInfo.title;
        this._canvas.findName("ItemThumbnail").Source = videoInfo.thumbnailUrl;

        this._canvas.findName("ItemHoverTitle").Text = videoInfo.title;
        this._canvas.findName("ItemHoverThumbnail").Source = videoInfo.thumbnailUrl;
        this._canvas.findName("ItemHoverInfoAvatar").Source = videoInfo.avatarUrl;
        this._canvas.findName("ItemHoverInfoOwner").Text = "Uploaded by " + videoInfo.ownerName;
        this._canvas.findName("ItemHoverInfoProperties").Text = videoInfo.views + (videoInfo.views != 1 ? " views | " : " view | ") + videoInfo.favorites + (videoInfo.favorites != 1 ? " favorites" : " favorite");
        
        var tagString = "Tags: ";
        var tagCount = videoInfo.tags.length > 3 ? 3 : videoInfo.tags.length;
        for (i=0; i<tagCount-1; i++) {
            tagString += videoInfo.tags[i] + ', ';
        }
        tagString += tagCount > 0 ? videoInfo.tags[i] : 'none';
           
        this._canvas.findName("ItemTags").Text = tagString;
        this._canvas.findName("ItemHoverTags").Text = tagString;
    },
    
    /* 
        disableGridItem
        When a grid page doesn't have enough videos to fill the grid,
        grid items are disabled.  They appear blank on the grid.
    */
    disableGridItem: function() {
        this._videoInfo = null;
        
        this._canvas.findName("Item").Visibility = "Collapsed";
    },
    
    /* 
        showItem
        Makes the gridItem visible after loading its image thumbnail.
        Triggers either the initial-load storyboard or the paging storyboard
    */
    showItem: function() {
        if (this._videoInfo !== null) {
            var storyboard;
            if (this._showInitialAnimation) {
                storyboard = this._canvas.findName('LoadStoryboard');
                storyboard.beginTime = '0:0:' + (Math.floor(Math.random()*20))/20;
                storyboard.begin();
            } else {
                storyboard = this._canvas.findName('PageStoryboard');
                storyboard.begin();
            }
        }
    },
    
    /* 
        hideItem
        Called at beginning of paging to make sure the item isn't seen until the new
        video info is loaded
    */
    hideItem: function() {
        this._canvas.findName('Item').Opacity = 0;
    },
    
    /* 
        onImageProgressChanged
        Event handler for thumbnail image download. Shows the item only after
        the image is downloaded (prevents html-like "popping" of resources after load).
    */
    onImageProgressChanged: function(sender, eventArgs) {
        if (sender.DownloadProgress == 1) {
            this.showItem();
        }
    },
    
    /* 
        onMouseEnterThumbnail
        Triggers the item's hover state
    */
    onMouseEnterThumbnail: function(sender, mouseEventArgs) { 
        this._STOP = false;
        this._hoverCanvas.Visibility = "Visible";
        this._canvas['Canvas.ZIndex'] = 200;
        this._canvas.findName("ItemHoverShowStoryboard").begin();
    },

    /* 
        onMouseLeaveItemCanvas
        Hides the item's hover state. This event is tied to the overall item canvas, 
        so that the event fires only if the mouse leaves either the griditem or the 
        griditem's hover area (which is a child)
    */
    onMouseLeaveItemCanvas: function(sender) { 
        this._STOP = true;
        this._canvas['Canvas.ZIndex'] = 100;
        var preview = this._canvas.findName("ItemHoverPreview");
        if (preview.downloadProgress < 1) {
            preview.source = '';
        } else {
            preview.stop();
        }
        this._canvas.findName("ItemHoverHideStoryboard").begin();
    },
    
    /* 
        onShowHoverComplete
        Called when the hover state show animation completes.  Triggers 
        loading/playing of the video preview.
    */
    onShowHoverComplete: function( sender, eventArgs ) {
        if (!this._STOP) {
            var preview = this._canvas.findName("ItemHoverPreview");
            if (preview.Source === '') {
                preview.Source = this._videoInfo.previewUrl;
            } else {
                preview.play();
            }
        }
    },
    
    /* 
        onHideHoverComplete
        Called after the hover state is hidden. 
        Collapses the hover area, ensuring it can't be clicked.
    */
    onHideHoverComplete: function( sender, eventArgs ) {
        this._hoverCanvas.Visibility = "Collapsed";
    },

    /* 
        onPreviewStateChanged
        Hides/shows the buffering progress indicator for the video preview
    */
    onPreviewStateChanged: function(sender, eventArgs) {
        if (sender.CurrentState == 'Buffering') {
            this._canvas.findName("ItemHoverLoadingPreview").Visibility = "Visible";
        } else {
            this._canvas.findName("ItemHoverLoadingPreview").Visibility = "Collapsed";
        }
    },
    
    /* 
        onPreviewBufferingProgressChanged
        Sets the buffering progress status for the video preview
    */
    onPreviewBufferingProgressChanged: function(sender, eventArgs) {
        this._canvas.findName("ItemHoverLoadingProgress").Text = "buffering: " + Math.round(sender.bufferingProgress*100).toString() + "%";
    },
    
    /* 
        onClick
        Navigates to the player page for the item's video.
    */
    onClick: function( sender, mouseEventArgs ) {
        this._grid._scene.goToVideo(this._videoInfo.videoId);
    }
};






/*
VideoGrid.Layout

Stores layout parameters for the VideoGrid
*/



VideoGrid.Layout = function(configuration) {
    // constants
    this.sceneBufferLeft = 14;
    this.sceneBufferRight = 14;
    this.sceneBufferTop = 10;
    this.sceneBufferBottom = 50;

    // configuration-specific settings
    this.configuration = configuration;
    switch (configuration) {
        case 'Playlist':
            this.sceneWidth = 900;
            this.sceneHeight = 203;
            this.itemWidth = 162.4;
            this.itemHeight = 137;
            this.gridNumRows = 1;
            this.gridNumColumns = 5;
            this.gridWidth = 900;
            this.gridHeight = 153;
            this.itemPaddingX = 14;
            this.itemPaddingY = 16;
            break;
        case 'Home':
            this.sceneWidth = 900;
            this.sceneHeight = 340;
            this.itemWidth = 162.4;
            this.itemHeight = 137;
            this.gridNumRows = 2;
            this.gridNumColumns = 5;
            this.gridWidth = 900;
            this.gridHeight = 290;
            this.itemPaddingX = 16;
            this.itemPaddingY = 16;
            break;
        default:
            this.sceneWidth = 900;
            this.sceneHeight = 493;
            this.itemWidth = 162.4;
            this.itemHeight = 137;
            this.gridNumRows = 3;
            this.gridNumColumns = 5;
            this.gridWidth = 900;
            this.gridHeight = 443;
            this.itemPaddingX = 16;
            this.itemPaddingY = 16;
    }

    this.gridMaxItems = this.gridNumRows * this.gridNumColumns;
    
};

VideoGrid.Layout.prototype = 
{
};






/*
VideoGrid.Strings

Stores static strings for the VideoGrid
*/



VideoGrid.Strings = function()
{
};

VideoGrid.Strings.prototype = {
    noResults_tagPrimary: "No videos have the tag \"$tag\"",
    noResults_tagSecondary: "Check out popular tags.",
    noResults_tagSecondaryUrl: "default.aspx",
    
    noResults_ownerPrimary: "$userName has not shared any videos.",  
    noResults_ownerSecondary: "Check out other videos.",
    noResults_ownerSecondaryUrl: "tags.aspx",

    noResults_favoritesPrimary: "$userName has not favorited any videos.",
    noResults_favoritesSecondary: "Check out other videos.",
    noResults_favoritesSecondaryUrl: "tags.aspx",

    noResults_myOwnerPrimary: "You have no videos.",  
    noResults_myOwnerSecondary: "Upload one.",
    noResults_myOwnerSecondaryUrl: "upload.aspx",

    noResults_myFavoritesPrimary: "You have not favorited any videos.",
    noResults_myFavoritesSecondary: "Check out other videos.",
    noResults_myFavoritesSecondaryUrl: "tags.aspx",

    noResults_recentViewsPrimary: "You have not watched any videos.",  
    noResults_recentViewsSecondary: "Watch some.",
    noResults_recentViewsSecondaryUrl: "tags.aspx",

    noResults_allPrimary: "No videos have been uploaded.",
    noResults_allSecondary: "Upload one.",
    noResults_allSecondaryUrl: "upload.aspx"
};
