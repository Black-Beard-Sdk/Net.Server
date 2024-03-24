namespace Bb.Extensions
{
    public static class FormFileExtension
    {


        /// <summary>
        /// Writes the document on disk.
        /// </summary>
        /// <param name="file">The uploaded document.</param>
        /// <param name="override">by default the file is not overwrites</param>
        /// <returns>return the target document file on the disk. A temp name is used.</returns>
        /// <exception cref="Exception">if the argument is false and the file exists</exception>
        public static FileInfo Save(this IFormFile file, DirectoryInfo targetDirectory, bool @override = false)
        {
            var path = targetDirectory.FullName.Combine(file.FileName);
            return file.Save(path.AsFile(), @override);
        }

        /// <summary>
        /// Writes the document on disk.
        /// </summary>
        /// <param name="file">The uploaded document.</param>
        /// <param name="override">by default the file is not overwrites</param>
        /// <returns>return the target document file on the disk. A temp name is used.</returns>
        /// <exception cref="Exception">if the argument is false and the file exists</exception>
        public static FileInfo Save(this IFormFile file, bool @override = false)
        {
            var path = Path.GetTempPath();
            return file.Save(path.AsFile(), @override);
        }

        /// <summary>
        /// Writes the document on disk.
        /// </summary>
        /// <param name="file">The uploaded document.</param>
        /// <param name="targetFile">The file path.</param>
        /// <param name="override">by default the file is not overwrites</param>
        /// <returns>return the target document file on the disk. if the target file argument is null, a temp name is used.</returns>
        /// <exception cref="Exception">if the argument is false and the file exists</exception>
        public static FileInfo Save(this IFormFile file, string path, bool @override = false)
        {
            return file.Save(path.AsFile(), @override);
        }

        /// <summary>
        /// Writes the document on disk.
        /// </summary>
        /// <param name="file">The uploaded document.</param>
        /// <param name="targetFile">The file path.</param>
        /// <param name="override">by default the file is not overwrites</param>
        /// <returns>return the target document file on the disk. if the target file argument is null, a temp name is used.</returns>
        /// <exception cref="Exception">if the argument is false and the file exists</exception>
        public static FileInfo Save(this IFormFile file, FileInfo targetFile, bool @override = false)
        {

            if (targetFile == null)
                targetFile = Path.GetTempPath().AsFile();

            targetFile.Refresh();

            if (!targetFile.Directory.Exists)
                targetFile.Directory.Create();

            if (targetFile.Exists)
            {
                if (@override)
                    targetFile.Delete();
                else
                    throw new Exception($"File {targetFile.FullName} already exists");
            }

            using (var stream = new FileStream(targetFile.FullName, FileMode.Create))
                file.CopyTo(stream);

            targetFile.Refresh();

            return targetFile;

        }


    }

}
