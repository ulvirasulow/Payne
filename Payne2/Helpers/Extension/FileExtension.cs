namespace Payne.Helpers.Extension;

public static class FileExtension
{
    public static string Upload(this IFormFile file, string rootPath, string folderName)
    {
        string fileName = file.FileName;
        if (fileName.Length > 64)
        {
            fileName = fileName.Substring(fileName.Length - 64);
        }

        fileName = Guid.NewGuid() + fileName;
        string path = Path.Combine(rootPath, folderName, fileName);

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return fileName;
    }

    public static bool DeleteFile(string rootPath, string folderName, string fileName)
    {
        string path = Path.Combine(rootPath, folderName, fileName);

        if (!File.Exists(path))
        {
            return false;
        }

        File.Delete(path);
        return true;
    }
}