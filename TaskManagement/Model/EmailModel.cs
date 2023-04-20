namespace TaskManagement.Model
{
    public class EmailModel
    {
        public string To { get; set; }  
        public string Content { get; set; }
        public string Subject { get; set; }

        public EmailModel(string to,string subject,string content)
        {
            To = to;
            Content = content;
            Subject = subject;
            
        }
    }
}
