using BasicWebServer.Server.HTTP;
using System.IO;

namespace BasicWebServer.Server.Responses
{
    public class FileResponse : Response
    {
        public FileResponse(string fileName)
            : base(StatusCode.OK)
        {
            this.FileName = fileName;

            this.Headers.Add(Header.ContentType, ContentType.PlanText);
        }

        public string FileName { get; init; }

        public override string ToString()
        {
            if (File.Exists(this.FileName))
            {
                this.Body = string.Empty;

                FileContent = File.ReadAllBytes(this.FileName);

                var fileBytesCount = new FileInfo(this.FileName).Length;
                this.Headers.Add(Header.ContentLenght, fileBytesCount.ToString());

                this.Headers.Add(Header.ContentDisposition, $"attachment; filename=\"{FileName}\"");
            }

            return base.ToString();
        }
    }
}
