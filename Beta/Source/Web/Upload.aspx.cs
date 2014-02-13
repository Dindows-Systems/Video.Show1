using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using VideoShow.Utility;
using VideoShow.Data;
using VideoShow.Data.DataServices;

public partial class Upload : System.Web.UI.Page
{
    private VideoShowDataContext DataContext = DataContextFactory.DataContext();

    protected void UploadSubmit_Click(object sender, EventArgs e)
    {
        if (fileUpload.HasFile)
            try
            {
                string filename = System.IO.Path.GetFileName(fileUpload.PostedFile.FileName);
                string localFilename = Server.MapPath(".") + @"\_temp_upload\" + filename;
                fileUpload.PostedFile.SaveAs(localFilename);
                string uploadUser = Membership.GetUser().ProviderUserKey.ToString();

                //Add only the new tags
                List<string> selectedTags = Tags.Text.Split(',').ToList<string>();
                var newTagNames = (
                    from newTag in selectedTags 
                    select newTag.Trim()).Except(
                        from tag in DataContext.Tags select tag.Tag1)
                    ;
                List<Tag> addTags = new List<Tag>();

                foreach (string t in newTagNames)
                {
                    Tag nt = new Tag(); 
                    nt.Tag1 = t; 
                    addTags.Add(nt);
                }
                
                DataContext.Tags.AddAll(addTags);
                DataContext.SubmitChanges();

                Video v = new Video();
                v.OwnerUserId = new Guid(uploadUser);
                v.Title = VideoTitle.Text;
                v.Description = Description.Text;
                v.VideoUrl = string.Empty; // string.Format("streaming:/{0}/{1}/{2}", ACCOUNTID, filesetName, filename);
                v.ThumbnailUrl = "/Silverlight/VideoGrid/Images/thumbnail_placeholder.png";
                v.DatePublished = DateTime.Now;
                v.SizeInKilobytes = fileUpload.PostedFile.ContentLength / 1024;
                v.DurationInSeconds = 0;

                //TODO:Should status values be queried?
                v.StatusId = (int)VideoServices.VideoStatus.New; //New
                DataContext.Videos.Add(v);
                DataContext.SubmitChanges();

                var selTagsTable = from st in selectedTags select st;
                var videoTags = DataContext.Tags.Where(t => selTagsTable.Contains(t.Tag1));

                foreach(var tag in videoTags)
                {
                    VideosToTag vtt = new VideosToTag(); 
                    vtt.VideoId = v.VideoId; 
                    vtt.TagId = tag.TagId;
                    DataContext.VideosToTags.Add(vtt);
                }

                DataContext.SubmitChanges();
     
                string filesetName = Guid.NewGuid().ToString().Replace("-",string.Empty);
                ProcessVideoAsync(localFilename, filesetName, v.VideoId);

                ResetForm();
                Results.Text = "Upload completed. Check back in a few minutes to watch the video.";
                UploadForm.Visible = false;
                UploadAnother.Visible = true;
            }
             catch
            {
                Results.Text = "Sorry, that didn't work. Please check your information and try that again.";
            }
        else
        {
            Results.Text = "You haven't picked a video to upload.";
        }
    }

    private void ResetForm()
    {
        this.VideoTitle.Text = string.Empty;
        this.Description.Text = string.Empty;
        this.Tags.Text = string.Empty;
    }

    private void ProcessVideoAsync(string filepath, string filesetName, int videoId)
    {
        VideoProcessor.StartBackgroundThread(
                delegate 
                {
                    VideoProcessor.Process(filepath, filesetName, videoId); 
                }
            );
    }

    protected void UploadAnother_Click(object sender, EventArgs e)
    {
        UploadAnother.Visible = false;
        UploadForm.Visible = true;
    }
}