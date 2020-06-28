using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Threading;
using System.Windows.Forms;
namespace assignment_googleapidrivev3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void Authorize()
        {
            string[] scopes = new string[] { DriveService.Scope.Drive,
                               DriveService.Scope.DriveFile,};
            var clientId = "446138855943-edvr5bskptd3bivfui61tkb13nfdepu7.apps.googleusercontent.com"; 
            var clientSecret = "4RZCxGSDmFaAL7arBj4jSpUB";            
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            }, scopes,
            Environment.UserName, CancellationToken.None, new FileDataStore("MyAppsToken")).Result;
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MyAppName",

            });
            service.HttpClient.Timeout = TimeSpan.FromMinutes(100);
            textBox2.Visible = true;
            var respocne = uploadFile(service, textBox1.Text, "");
            MessageBox.Show("Process completed--- Response--" + respocne);
            textBox1.Text = "";
        }
        public Google.Apis.Drive.v3.Data.File uploadFile(DriveService _service, string _uploadFile, string _parent, string _descrp = "Uploaded with .NET!")
        {
            if (System.IO.File.Exists(_uploadFile))
            {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                body.Name = System.IO.Path.GetFileName(_uploadFile);
                body.Description = _descrp;
                body.MimeType = GetMimeType(_uploadFile);
                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.CreateMediaUpload request = _service.Files.Create(body, stream, GetMimeType(_uploadFile));
                    request.SupportsTeamDrives = true;
                    request.ProgressChanged += Request_ProgressChanged;
                    request.ResponseReceived += Request_ResponseReceived;
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error Occured");
                    return null;
                }
            }
            else
            {
                MessageBox.Show("The file does not exist.");
                return null;
            }
        }
        private string GetMimeType(string uploadFile)
        {
            throw new NotImplementedException();
        }
        private void Request_ProgressChanged(Google.Apis.Upload.IUploadProgress obj)
        {
            textBox2.Text += obj.Status + " " + obj.BytesSent;
        }
        private void Request_ResponseReceived(Google.Apis.Drive.v3.Data.File obj)
        {
            if (obj != null)
            {
                MessageBox.Show("File was uploaded sucessfully--" + obj.Id);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void Browse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
           
        }
        private void Upload_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.ToString()=="")
            {

                MessageBox.Show("No Files Selected");
            }
            else
            {
                Authorize();
            }
            
        }

    }
}
