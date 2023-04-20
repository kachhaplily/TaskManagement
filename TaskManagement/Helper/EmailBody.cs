namespace TaskManagement.Helper
{
    public static class EmailBody
    {
        public static string EmailStringBody(string email, string emailToken)
        {

            return $@" <html>
                <head>
                </head>
                <body>
          <h1> Reste your Password</h1>
            <p> You are receving this e - mail because you requested a password reset for your account. </p>
             <p> Please tap the button below to choose a new password. </p>
    <a href = ""http://localhost:4200/reset?email={email}&code={emailToken}"" target = ""_blank"" > Reset Your password </a>
<p>kind Regards <br><br>
ALtaf Ghori </p>
</body>
</html>";

        }

    }
}
