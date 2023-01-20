using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

// https://fast-endpoints.com/
public class ImageDAO
{
  public string name { get; set; }
  public string path { get; set; }
  public string thumbnailPath { get; set; }
}

public class LandingEndpoint : Endpoint<EmptyRequest, List<ImageDAO>>
{
  private string imageFolderToHost = "P:/Pictures/BestOfTheBest/Prints"; // TODO read from settings file

  public override void Configure()
  {
    Get("/api/images/list");
    AllowAnonymous();
  }

  /// <summary>Resizes an image to a new width and height.</summary>
  /// <param name="originalPath">The folder which holds the original image.</param>
  /// <param name="originalFileName">The file name of the original image.</param>
  /// <param name="newPath">The folder which will hold the resized image.</param>
  /// <param name="newFileName">The file name of the resized image.</param>
  /// <param name="maximumWidth">When resizing the image, this is the maximum width to resize the image to.</param>
  /// <param name="maximumHeight">When resizing the image, this is the maximum height to resize the image to.</param>
  /// <param name="enforceRatio">Indicates whether to keep the width/height ratio aspect or not. If set to false, images with an unequal width and height will be distorted and padding is disregarded. If set to true, the width/height ratio aspect is maintained and distortion does not occur.</param>
  /// <param name="addPadding">Indicates whether fill the smaller dimension of the image with a white background. If set to true, the white padding fills the smaller dimension until it reach the specified max width or height. This is used for maintaining a 1:1 ratio if the max width and height are the same.</param>
  private static void ResizeImage(string originalFileName, string newFileName, int maximumWidth, int maximumHeight, bool enforceRatio, bool addPadding)
  {
    var image = System.Drawing.Image.FromFile(originalFileName);
    var imageEncoders = ImageCodecInfo.GetImageEncoders();
    EncoderParameters encoderParameters = new EncoderParameters(1);
    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
    var canvasWidth = maximumWidth;
    var canvasHeight = maximumHeight;
    var newImageWidth = maximumWidth;
    var newImageHeight = maximumHeight;
    var xPosition = 0;
    var yPosition = 0;

    if (enforceRatio)
    {
      var ratioX = maximumWidth / (double)image.Width;
      var ratioY = maximumHeight / (double)image.Height;
      var ratio = ratioX < ratioY ? ratioX : ratioY;
      newImageHeight = (int)(image.Height * ratio);
      newImageWidth = (int)(image.Width * ratio);

      if (addPadding)
      {
        xPosition = (int)((maximumWidth - (image.Width * ratio)) / 2);
        yPosition = (int)((maximumHeight - (image.Height * ratio)) / 2);
      }
      else
      {
        canvasWidth = newImageWidth;
        canvasHeight = newImageHeight;
      }
    }

    var thumbnail = new Bitmap(canvasWidth, canvasHeight);
    var graphic = Graphics.FromImage(thumbnail);

    if (enforceRatio && addPadding)
    {
      graphic.Clear(Color.White);
    }

    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
    graphic.SmoothingMode = SmoothingMode.HighQuality;
    graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
    graphic.CompositingQuality = CompositingQuality.HighQuality;
    graphic.DrawImage(image, xPosition, yPosition, newImageWidth, newImageHeight);

    thumbnail.Save(newFileName, imageEncoders[1], encoderParameters);
  }

  private static bool FileExists(string fileName)
  {
   return File.Exists(fileName);
  }

  private static void CheckOrCreateThumbnailFolder(string thumbnailFolderPath)
  {
    if(FileExists(thumbnailFolderPath))
    {
      return;
    }
    else
    {
      Directory.CreateDirectory(thumbnailFolderPath);
    }
  }

  private static List<ImageDAO> ProcessDirectory(string targetDirectory)
  {
    var result = new List<ImageDAO>();
    // Process the list of files found in the directory.
    string[] fileEntries = Directory.GetFiles(targetDirectory);

    foreach (string fileName in fileEntries)
    {
      var dirInfo = new DirectoryInfo(fileName);
      var parentDirName = dirInfo.Parent?.Name;
      var parentDirPath = dirInfo.Parent?.FullName;
      string thumbnailFolderName = Path.Combine(parentDirPath, "Thumbnails");
      string thumbnailFileName = Path.GetFileNameWithoutExtension(fileName) + "_Thumbnail" + Path.GetExtension(fileName);
      string thumbnailPath = Path.Combine(Path.Combine(parentDirPath, "Thumbnails"), thumbnailFileName);
      
      // 1. check if the folder exists, if not create it
      CheckOrCreateThumbnailFolder(thumbnailFolderName);

      // 2. check if thumbnail exists
      if (!FileExists(thumbnailPath))
      {
        // Generate if not
        ResizeImage(fileName, thumbnailPath, 500, 4000, true, false);
      }

      result.Add(new ImageDAO()
      {
        name = Path.GetFileName(fileName),
        path = parentDirName + "/" + Path.GetFileName(fileName),
        thumbnailPath = Path.Combine(parentDirName, "Thumbnails/") + thumbnailFileName
      }) ;
    }

    return result;
  }

  public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
  {
    var response = ProcessDirectory(imageFolderToHost);
    await SendAsync(response);
  }
}